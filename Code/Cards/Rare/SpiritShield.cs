using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Character;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class SpiritShield : WatcherCardModel
{
    public SpiritShield() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(3, 1);
    }

    private static decimal Calc(Player player)
    {
        return PileType.Hand.GetPile(player).Cards.Count;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue * Calc(Owner), DynamicVars.Block.Props, cardPlay);
    }
}