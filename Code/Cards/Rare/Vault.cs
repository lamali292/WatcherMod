using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using Watcher.Code.Abstract;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Vault : WatcherCardModel
{
    public Vault() : base(3, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithPower<VaultPower>(1, false);
    }
    
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<VaultPower>(ctx, this);
        PlayerCmd.EndTurn(Owner, false);
    }
}


