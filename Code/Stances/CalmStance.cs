using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Relics;
using Watcher.Code.Vfx;

namespace Watcher.Code.Stances;

public class CalmStance : WatcherStanceModel
{
    public override bool ShouldReceiveCombatHooks => true;
    protected override StanceVfxConfig VfxConfig => new(
        AuraScenePath: "res://Watcher/scenes/watcher_mod/vfx/calm_aura.tscn",
        BodyTint: new Color(0.7f, 0.85f, 1.3f),
        EnterSfxPath: "res://Watcher/audio/calm_enter.ogg",
        AmbienceLoopPath: "res://Watcher/audio/calm_loop.ogg",
        ScreenFlashColor: new Color(0.4f, 0.7f, 1f)
    );

    public override Task OnExitStance(PlayerChoiceContext ctx, Player player, CardModel? source)
    {
        var amount = 2;
        if (player.GetRelic<VioletLotus>() != null) amount += 1;
        player.PlayerCombatState!.GainEnergy(amount);
        return base.OnExitStance(ctx, player,source);
    }
}