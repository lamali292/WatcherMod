using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Watcher.Code.Abstract;
using Watcher.Code.Cards.Token;
using Watcher.Code.Character;
using Watcher.Code.Commands;

namespace Watcher.Code.Cards.Rare;

[Pool(typeof(WatcherCardPool))]
public sealed class ConjureBlade : WatcherCardModel
{
    public ConjureBlade() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithTip(typeof(Expunger));
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay play)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded)
            x += 1;
        var expunger = await WatcherCmd.GiveCard<Expunger>(Owner, PileType.Draw, CardPilePosition.Random);
        expunger?.DynamicVars.Repeat.UpgradeValueBy(x + 1);
    }
}