using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Watcher.Code.Cards.CardModels;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class Establishment() : WatcherCardModel(1, CardType.Power, CardRarity.Rare, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<EstablishmentPower>(1)
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EstablishmentPower>(Owner.Creature, DynamicVars["EstablishmentPower"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}