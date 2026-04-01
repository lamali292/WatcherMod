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

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class MentalFortress() : WatcherCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<MentalFortressPower>(4)];
    


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MentalFortressPower>(Owner.Creature, DynamicVars["MentalFortressPower"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MentalFortressPower"].UpgradeValueBy(2);
    }
}