using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Watcher.Code.Compatibility;

public static class CardModelCompatExtensions
{
    private static readonly MethodInfo? DupeWithPlayer =
        typeof(CardModel).GetMethod("CreateDupe", [typeof(Player)]);

    private static readonly MethodInfo? DupeNoArgs =
        typeof(CardModel).GetMethod("CreateDupe", Type.EmptyTypes);

    public static CardModel CreateDupeCompat(this CardModel card, Player? newOwner = null)
    {
        if (DupeWithPlayer != null)
            return (CardModel)DupeWithPlayer.Invoke(card, [newOwner ?? card.Owner])!;

        if (DupeNoArgs != null)
            return (CardModel)DupeNoArgs.Invoke(card, null)!;

        throw new MissingMethodException("CardModel", "CreateDupe");
    }
}