using System.Reflection;
using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Watcher.Code.Character;
using Watcher.Code.Events;
using Watcher.Code.Extensions;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public abstract class WatcherStanceModel : AbstractModel
{
    private Player? _player;

    private StanceVfxController? _vfx;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    private LocString Title => new("powers", $"{WatcherMainFile.ModId.ToUpperInvariant()}-{Id.Entry}.title");

    private LocString Description =>
        new("powers", $"{WatcherMainFile.ModId.ToUpperInvariant()}-{Id.Entry}.description");

    private string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    private Texture2D Icon => ResourceLoader.Load<Texture2D>(PackedIconPath);

    public HoverTip DumbHoverTip
    {
        get
        {
            var description = Description;
            AddDumbVariablesToDescription(description);
            return new HoverTip(Title, description.GetFormattedText(), Icon);
        }
    }

    protected abstract StanceVfxConfig VfxConfig { get; }

    public IEnumerable<string> AssetPaths => VfxConfig.AssetPaths;

    public WatcherStanceModel ToMutable(Player player)
    {
        var mutable = (WatcherStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    private void AddDumbVariablesToDescription(LocString description)
    {
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        var pool = IsMutable ? Owner.Character.CardPool : ModelDb.CardPool<WatcherCardPool>();
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(pool));
    }

    public virtual async Task OnEnterStance(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        _vfx = new StanceVfxController(VfxConfig);
        await _vfx.OnEnter(owner.Creature);
    }

    public virtual async Task OnExitStance(PlayerChoiceContext ctx, Player owner, CardModel? source)
    {
        if (_vfx != null)
            await _vfx.OnExit(owner.Creature);
        _vfx = null;
    }

    // Subclasses override THIS — never the game's ModifyDamageMultiplicative.
    public virtual decimal WatcherModifyDamageMultiplicative(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 1;
}

