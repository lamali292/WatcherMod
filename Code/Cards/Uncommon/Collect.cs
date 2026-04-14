using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Powers;

namespace Watcher.Code.Cards.Uncommon;

[Pool(typeof(WatcherCardPool))]
public sealed class Collect : WatcherCardModel
{
    public Collect() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(new TooltipSource(_ =>
        {
            var beam = ModelDb.GetById<Miracle>(ModelDb.Card<Miracle>().Id).ToMutable();
            beam.UpgradeInternal();
            return HoverTipFactory.FromCard(beam);
        }));
    }
    
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var numOfXs = ResolveEnergyXValue();
        if (IsUpgraded) numOfXs++;
        await CommonActions.ApplySelf<CollectPower>(this, numOfXs);
    }
}