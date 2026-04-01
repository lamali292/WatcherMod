using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class SpiritShield() : WatcherCardModel(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner);

        var totalBlock = hand.Cards.Count * DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(
            Owner.Creature,
            totalBlock,
            ValueProp.Move,
            cardPlay
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1);
    }
}