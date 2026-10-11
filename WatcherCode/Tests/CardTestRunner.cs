using System.Reflection;
using Watcher.Code.Compatibility;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace Watcher.Code.Tests;

public class CardTestRunner
{
	private readonly List<(string testName, Exception ex)> _failures = [];
	private readonly List<string> _passed = [];
	private RunState _run = null!;

	/// <param name="filter">Optional case-insensitive substring matched against "Type.Method"; null runs everything.</param>
	public async Task<TestRunResult> RunAllTestsAsync(string seed, CancellationToken ct, string? filter = null)
	{
		var started = DateTime.UtcNow;
		var wasTestMode = TestMode.IsOn;
		TestMode.IsOn = true;
		var selectorScope = CardSelectCmdCompatibility.UseSelector(new FirstCardSelector());

		try
		{
			// Scans every loaded assembly for [CardTest] methods. GetTypes() can throw
			// ReflectionTypeLoadException for some assemblies - those are skipped, not fatal.
			var testMethods = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(GetLoadableTypes)
				.SelectMany(t => t.GetMethods())
				.Where(m => m.GetCustomAttributes(typeof(CardTestAttribute), false).Length > 0)
				.Where(m => string.IsNullOrEmpty(filter) ||
				            $"{m.DeclaringType?.Name}.{m.Name}".Contains(filter, StringComparison.OrdinalIgnoreCase))
				// Quick single-combat tests first, slow "play every card" pool tests last.
				.OrderBy(m => typeof(Task).IsAssignableFrom(m.ReturnType) ? 0 : 1)
				.ThenBy(m => m.DeclaringType?.Name)
				.ToList();

			AutoSlayLog.Action($"[TestRunner] Found {testMethods.Count} test cases" +
			                   (string.IsNullOrEmpty(filter) ? "." : $" matching '{filter}'."));

			foreach (var method in testMethods)
			{
				ct.ThrowIfCancellationRequested();
				var testName = $"{method.DeclaringType?.Name}.{method.Name}";
				var attr = (CardTestAttribute)method.GetCustomAttributes(typeof(CardTestAttribute), false).First();

				try
				{
					if (typeof(Task).IsAssignableFrom(method.ReturnType))
					{
						// Single-combat test: one combat, run the method, tear down.
						await RunSingleTest(method, testName, seed, attr);
						_passed.Add(testName);
					}
					else if (typeof(IEnumerable<CardTestCase>).IsAssignableFrom(method.ReturnType))
					{
						// Pool test: a fresh combat per card, the old way.
						await RunPoolTest(method, testName, seed, attr, ct);
					}
					else
					{
						throw new InvalidOperationException(
							$"{testName} must return Task or IEnumerable<CardTestCase>, got {method.ReturnType.Name}.");
					}
				}
				catch (Exception ex)
				{
					var actualEx = ex.InnerException ?? ex;
					_failures.Add((testName, actualEx));
					AutoSlayLog.Error($"[FAILED] {testName}: {actualEx.Message}");
				}
			}
		}
		finally
		{
			selectorScope.Dispose();
			TestMode.IsOn = wasTestMode;
			Report();
		}

		return new TestRunResult(seed, DateTime.UtcNow - started, _passed.ToList(),
			_failures.Select(f => new TestFailure(f.testName, f.ex.Message, f.ex.ToString())).ToList());
	}

	private async Task RunSingleTest(MethodInfo method, string testName, string seed,
									 CardTestAttribute attr)
	{
		var (combat, players) = await NewCombatAsync(seed, attr);
		var context = new TestContext(combat, players);
		try
		{
			AutoSlayLog.Info($"Running: {testName}");
			var instance = Activator.CreateInstance(method.DeclaringType!);
			var task = (Task)method.Invoke(instance, [context])!;
			await task;
		}
		finally
		{
			EndCombat();
		}
	}

	private async Task RunPoolTest(MethodInfo method, string testName, string seed,
								   CardTestAttribute attr, CancellationToken ct)
	{
		AutoSlayLog.Info($"Running: {testName}");
		
		var characterType = attr.CharacterType ?? typeof(Ironclad);
		var characterModel = ModelDb.GetById<CharacterModel>(ModelDb.GetId(characterType));

		var instance = Activator.CreateInstance(method.DeclaringType!);
		var cases = (IEnumerable<CardTestCase>)method.Invoke(instance, [characterModel])!;

		foreach (var testCase in cases)
		{
			ct.ThrowIfCancellationRequested();
			
			var (combat, players) = await NewCombatAsync(seed, attr);
			var context = new TestContext(combat, players);

			try
			{
				await testCase.Run(context);
				_passed.Add($"{testName}:{testCase.Name}");
			}
			catch (Exception ex)
			{
				var actual = ex.InnerException ?? ex;
				_failures.Add(($"{testName}:{testCase.Name}", actual));
				AutoSlayLog.Error($"[FAILED] {testName}:{testCase.Name}: {actual.Message}");
			}
			finally
			{
				EndCombat();
			}
		}
	}

	private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			return ex.Types.Where(t => t != null).Select(t => t!);
		}
		catch
		{
			return [];
		}
	}

	private void Report()
	{
		if (_failures.Count == 0)
		{
			AutoSlayLog.Action("[TestRunner]: All tests passed!");
			return;
		}
		AutoSlayLog.Warn($"[TestRunner]: {_failures.Count} test(s) failed:");
		foreach (var (name, ex) in _failures)
			AutoSlayLog.Warn($"  - {name}: {ex.Message}");
	}

	private async Task<(CombatState combat, IReadOnlyList<Player> players)> NewCombatAsync(
		string seed, CardTestAttribute attr)
	{
		if (CombatManager.Instance.DebugOnlyGetState() != null)
			CombatManager.Instance.Reset(true);

		var characterType = attr.CharacterType ?? typeof(Ironclad);
		var characterModel = (CharacterModel)ModelDb.GetById<AbstractModel>(ModelDb.GetId(characterType));

		// Net ids 1..N; the singleplayer net service reports id 1, so player 1 is "us" and the rest are
		// treated as remote teammates. Card selection is short-circuited by FirstCardSelector, so
		// nothing ever waits on a remote choice.
		var newPlayers = Enumerable.Range(1, Math.Max(1, attr.PlayerCount))
			.Select(netId => Player.CreateForNewRun(characterModel, UnlockState.all, (ulong)netId))
			.ToList();

		_run = RunState.CreateForTest(players: newPlayers, seed: seed);
		var run = _run;
		// A bare test RunState has no map progression, so RunState.CurrentMapPointHistoryEntry is
		// null - that NREs in code that assumes a real run (e.g. CardReward.OnSelect logging card
		// choices to run history). Seed one entry so that machinery works under test too.
		run.AppendToMapPointHistory(MapPointType.Monster, RoomType.Monster, null);

		RunManager.Instance.SetUpTest(_run, new NetSingleplayerGameService(), shouldSave: false);
		LocalContext.NetId = RunManager.Instance.NetService.NetId;
		var players = run.Players.ToList();
		var player = players[0];

		var encounter = attr.EncounterType != null
			? ((EncounterModel)ModelDb.GetById<AbstractModel>(ModelDb.GetId(attr.EncounterType))).ToMutable()
			: ModelDb.AllEncounters.First().ToMutable();
		encounter.DebugRandomizeRng();

		var combat = new CombatState(encounter, run, run.Modifiers, run.BadgeModels, run.MultiplayerScalingModel);
		foreach (var p in players) combat.AddPlayer(p);

		if (!encounter.HaveMonstersBeenGenerated)
			encounter.GenerateMonstersWithSlots(run);
		foreach (var (monster, slot) in encounter.MonstersWithSlots)
		{
			monster.AssertMutable();
			combat.AddCreature(combat.CreateCreature(monster, CombatSide.Enemy, slot));
		}
		combat.SortEnemiesBySlotName();

		CombatManager.Instance.SetUpCombat(combat);
		CombatManager.Instance.AfterCombatRoomLoaded();

		var sw = System.Diagnostics.Stopwatch.StartNew();
		while (!CombatManager.Instance.IsInProgress && sw.Elapsed < TimeSpan.FromSeconds(10))
			await Task.Yield();
		if (!CombatManager.Instance.IsInProgress)
			throw new InvalidOperationException("Combat never reached IsInProgress after AfterCombatRoomLoaded.");
		while (player.PlayerCombatState?.Phase != PlayerTurnPhase.Play && sw.Elapsed < TimeSpan.FromSeconds(10))
			await Task.Yield();

		return (combat, players);
	}

	private void EndCombat()
	{
		try { CombatManager.Instance.Reset(true); } catch { /* best effort */ }
		
		try
		{
			RunManager.Instance.State = null;
			LocalContext.NetId = null;
		}
		catch { /* best effort */ }
	}
}


/// <summary>Auto-selects the first eligible card(s) for any prompt. For blind "play every card" runs.</summary>
public class FirstCardSelector : ICardSelector
{
	public Task<IEnumerable<CardModel>> GetSelectedCards(
		IEnumerable<CardModel> options, int minSelect, int maxSelect)
	{
		var list = options.ToList();
		//var count = Math.Min(Math.Max(minSelect, 0), Math.Min(maxSelect, list.Count));
		IEnumerable<CardModel> chosen = list.Take(maxSelect).ToList();
		return Task.FromResult(chosen);
	}

	public CardRewardSelection GetSelectedCardReward(
		IReadOnlyList<CardCreationResult> options,
		IReadOnlyList<CardRewardAlternative> alternatives)
	{
		return new CardRewardSelection { card = options.FirstOrDefault()?.Card };
	}
}
