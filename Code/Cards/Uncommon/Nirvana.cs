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
public sealed class Nirvana() : WatcherCardModel(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<NirvanaPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<NirvanaPower>(),
        HoverTipFactory.FromKeyword(WatcherKeywords.Scry)
    ];

    


    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<NirvanaPower>(
            Owner.Creature,
            DynamicVars["NirvanaPower"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NirvanaPower"].UpgradeValueBy(1m);
    }
}