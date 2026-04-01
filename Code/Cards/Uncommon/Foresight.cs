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
using Watcher.Code.Keywords;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Foresight() : WatcherCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ForesightPower>(),
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ForesightPower>(3)
    ];

    


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ForesightPower>(Owner.Creature, DynamicVars["ForesightPower"].IntValue, Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ForesightPower"].UpgradeValueBy(1);
    }
}