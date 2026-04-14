using System.Collections.Generic;
using System.Reflection;
using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using Watcher.Code.Cards.Basic;
using Watcher.Code.Relics;
using Watcher.Code.Stances;

namespace Watcher.Code.Character;

public class Watcher : CustomCharacterModel
{
    public const string CharacterId = "Watcher";

    public static readonly Color Color = new(0.5f, 0.0f, 0.5f);
    public override Color MapDrawingColor => Color;
    public override string CustomIconTexturePath => "res://Watcher/images/watcher/character_icon_watcher.png";
    public override string CustomCharacterSelectIconPath => "res://Watcher/images/watcher/char_select_watcher.png";


    public override CustomEnergyCounter? CustomEnergyCounter => 
        new CustomEnergyCounter(EnergyCounterPaths, new Color(0.4f, 0.1f, 0.9f), new Color(0.7f, 0.1f, 0.9f));

    //public override string CustomEnergyCounterPath => "res://Watcher/scenes/watcher/watcher_energy_counter_empty.tscn";

    public override string CustomCharacterSelectLockedIconPath =>
        "res://Watcher/images/watcher/char_select_watcher_locked.png";

    public override string CustomVisualPath => "res://Watcher/scenes/watcher/watcher.tscn";
    public override string CustomTrailPath => "res://Watcher/scenes/watcher/card_trail_watcher.tscn";
    public override string CustomIconPath => "res://Watcher/scenes/watcher/watcher_icon.tscn";
    public override string CustomRestSiteAnimPath => "res://Watcher/scenes/watcher/watcher_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://Watcher/scenes/watcher/watcher_merchant.tscn";

    public override string CustomArmPointingTexturePath =>
        "res://Watcher/images/watcher/hands/multiplayer_hand_watcher_point.png";

    public override string CustomArmRockTexturePath =>
        "res://Watcher/images/watcher/hands/multiplayer_hand_watcher_rock.png";

    public override string CustomArmPaperTexturePath =>
        "res://Watcher/images/watcher/hands/multiplayer_hand_watcher_paper.png";

    public override string CustomArmScissorsTexturePath =>
        "res://Watcher/images/watcher/hands/multiplayer_hand_watcher_scissors.png";

    public override string CustomCharacterSelectBg => "res://Watcher/scenes/watcher/char_select_bg_watcher2.tscn";

    public override string CustomCharacterSelectTransitionPath =>
        "res://Watcher/images/watcher/transitions/watcher_transition_mat.tres";

    public override string CustomMapMarkerPath => "res://Watcher/images/watcher/map_marker_watcher.png";
    public override string CustomAttackSfx => "event:/sfx/characters/ironclad/ironclad_attack";
    //public override string CustomCastSfx => "res://";
    //public override string CustomDeathSfx => "res://";
    public override string CharacterSelectSfx => "res://Watcher/audio/watcher_select.ogg";

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    protected override CharacterModel? UnlocksAfterRunAs => null;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeWatcher>(),
        ModelDb.Card<StrikeWatcher>(),
        ModelDb.Card<StrikeWatcher>(),
        ModelDb.Card<StrikeWatcher>(),
        ModelDb.Card<DefendWatcher>(),
        ModelDb.Card<DefendWatcher>(),
        ModelDb.Card<DefendWatcher>(),
        ModelDb.Card<DefendWatcher>(),
        ModelDb.Card<Vigilance>(),
        ModelDb.Card<Eruption>()
    ];

    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        "res://Watcher/scenes/watcher_mod/vfx/calm_aura.tscn",
        "res://Watcher/scenes/watcher_mod/vfx/wrath_aura.tscn",
        "res://Watcher/scenes/watcher_mod/vfx/divinity_aura.tscn",
        .. ModelDb.Power<CalmStance>().AssetPaths,
        .. ModelDb.Power<WrathStance>().AssetPaths,
        .. ModelDb.Power<DivinityStance>().AssetPaths,
        "res://Watcher/images/vfx/screenflash.png",
        "res://Watcher/images/vfx/eye_anim.png",
        "res://Watcher/images/vfx/frost_streak.png",
        "res://Watcher/images/vfx/big_blur.png",
        "res://Watcher/images/vfx/strike_line.png",
        "res://Watcher/images/vfx/glow_spark.png"
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<PureWater>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<WatcherCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WatcherRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WatcherPotionPool>();

    private string EnergyCounterPaths(int i)
    {
        return "res://Watcher/images/ui/combat/energy_counters/watcher/watcher_orb_layer_" + i + ".png";
    }


    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }


    public override CreatureAnimator? SetupCustomAnimationStates(MegaSprite controller)
    {
        return SetupAnimationState(controller, "Idle", hitName: "Hit");
    }
}


[HarmonyPatch(typeof(NEnergyCounter), nameof(NEnergyCounter._Ready))]
internal static class NEnergyCounterReadyPatch
{
    [HarmonyPrefix]
    static void SafeReady(NEnergyCounter __instance)
    {
        if (!__instance.HasNode("%BurstBack"))
        {
            var node = new CpuParticles2D
            {
                Name = (StringName)"BurstBack", Emitting = false, Amount = 0,Visible = false,
            };
            __instance.AddChild(node);
            node.Owner = __instance;
            node.UniqueNameInOwner = true;
        }

        if (__instance.HasNode("%BurstFront")) return;
        {
            var node = new CpuParticles2D
            {
                Name = (StringName)"BurstFront", Emitting = false, Amount = 0,Visible = false,
            };
            __instance.AddChild(node);
            node.Owner = __instance;
            node.UniqueNameInOwner = true;
        }
    }
}