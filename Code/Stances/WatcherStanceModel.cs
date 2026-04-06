using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Character;
using Watcher.Code.Extensions;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public abstract class WatcherStanceModel : AbstractModel
{
    //public override PowerType Type => PowerType.Buff;
    //public override PowerStackType StackType => PowerStackType.None;
    //protected override bool IsVisibleInternal => false;

    //public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    //public override string CustomBigIconPath => CustomPackedIconPath;

    private Player? _player;
    public Player Owner => _player ?? throw new InvalidOperationException("Not a mutable instance");

    public WatcherStanceModel ToMutable(Player player)
    {
        var mutable = (WatcherStanceModel)MutableClone();
        mutable._player = player;
        return mutable;
    }

    private LocString Title => new("powers", $"{WatcherMainFile.ModId.ToUpperInvariant()}-{Id.Entry}.title");
    private LocString Description => new("powers", $"{WatcherMainFile.ModId.ToUpperInvariant()}-{Id.Entry}.description");
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
    
    
    
    private void AddDumbVariablesToDescription(LocString description)
    {
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");
        var pool = IsMutable ? Owner.Character.CardPool : ModelDb.CardPool<WatcherCardPool>();
        description.Add("energyPrefix", EnergyIconHelper.GetPrefix(pool));
    }


    protected abstract StanceVfxConfig VfxConfig { get; }

    private StanceVfxController? _vfx;

    public IEnumerable<string> AssetPaths => VfxConfig.AssetPaths;

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
}