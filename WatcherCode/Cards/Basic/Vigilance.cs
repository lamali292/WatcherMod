using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Ancient;
using Watcher.Code.Character;
using Watcher.Code.Commands;
using Watcher.Code.Stances;

namespace Watcher.Code.Cards.Basic;

[Pool(typeof(WatcherCardPool))]
public sealed class Vigilance : WatcherCardModel, ITranscendenceCard
{
    public Vigilance() : base(2, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(8, 4);
        WithStanceTip<CalmStance>();
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Revelation>();
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await StanceCmd.EnterCalm(ctx, Owner, cardPlay.Card);
    }
}