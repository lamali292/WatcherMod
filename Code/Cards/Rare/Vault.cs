using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Vault() : WatcherCardModel(3, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    private bool _hasExtraTurn;
    private bool _paelsEyeWasAlreadyUsed;

    public override HashSet<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return _hasExtraTurn && player == Owner;
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _hasExtraTurn = true;
        
        var paelsEye = Owner.Relics.OfType<PaelsEye>().FirstOrDefault();
        if (paelsEye != null)
            _paelsEyeWasAlreadyUsed = Traverse.Create(paelsEye).Field("_usedThisCombat").GetValue<bool>();

        // End your turn
        PlayerCmd.EndTurn(Owner, false);
        return Task.CompletedTask;
    }

    public override Task AfterTakingExtraTurn(Player player)
    {
        if (player != Owner) return Task.CompletedTask;
        if (!_hasExtraTurn) return Task.CompletedTask; // Not our extra turn, don't touch PaelsEye
    
        _hasExtraTurn = false;

        if (_paelsEyeWasAlreadyUsed) return Task.CompletedTask;
        var paelsEye = player.Relics.OfType<PaelsEye>().FirstOrDefault();
        if (paelsEye == null) return Task.CompletedTask;
        Traverse.Create(paelsEye).Field("_usedThisCombat").SetValue(false);

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}