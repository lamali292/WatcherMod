using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Powers;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Pray : WatcherCardModel
{
    public Pray() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<MantraPower>(3, 1);
        WithStanceTip<DivinityStance>();
        WithTip(typeof(Insight));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MantraPower>(this);
        await WatcherCmd.GiveCard<Insight>(Owner, PileType.Draw, CardPilePosition.Random);
    }
}