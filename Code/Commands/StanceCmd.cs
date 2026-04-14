using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Watcher.Code.Nodes;
using Watcher.Code.Stances;

namespace Watcher.Code.Commands;

public static class StanceCmd
{
    public static Task EnterWrath(Creature creature, CardModel? cardSource)
    {
        return Execute(creature, ModelDb.Power<WrathStance>(), cardSource);
    }

    public static Task EnterCalm(Creature creature, CardModel? cardSource)
    {
        return Execute(creature, ModelDb.Power<CalmStance>(), cardSource);
    }

    public static Task EnterDivinity(Creature creature, CardModel? cardSource)
    {
        return Execute(creature, ModelDb.Power<DivinityStance>(), cardSource);
    }

    public static Task ExitStance(Creature creature, CardModel? cardSource)
    {
        return Execute(creature, ModelDb.Power<NoStance>(), cardSource);
    }

    private static async Task Execute(Creature creature, StancePower? newStance, CardModel? cardSource)
    {
        // Ensure eye decoration exists (lids visible by default)
        //StancePower.EnsureEyeSetup(creature);

        var current = creature.Powers.OfType<StancePower>().FirstOrDefault();

        var currentIsNone = current is null or NoStance;
        var newIsNone = newStance is null or NoStance;

        if ((currentIsNone && newIsNone) || current?.GetType() == newStance?.GetType() || creature.Player == null)
            return;

        if (current != null)
        {
            await current.OnExitStance(creature);
            current.RemoveInternal();
        }

        if (newStance != null)
        {
            var mutable = newStance.ToMutable();
            await PowerCmd.Apply(mutable, creature, 1, creature, cardSource);
            await ((StancePower)mutable).OnEnterStance(creature);
        }

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
        var visuals = creatureNode?.Visuals as WatcherNCreatureVisuals;
        visuals?.SetEyeStance(newStance switch
        {
            WrathStance => "wrath",
            CalmStance => "calm",
            DivinityStance => "divinity",
            _ => "RESET"
        });
    }
}