using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using Watcher.Code.Cards.Basic;
using Watcher.Code.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using System.Reflection;
using MegaCrit.Sts2.Core.Assets;
using BaseLib.Utils.Patching;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Watcher.Code.Patches;

namespace Watcher.Code.Character;

public class Watcher : CustomCharacterModel
{
    public const string CharacterId = "Watcher";

    public static readonly Color Color = StsColors.purple;
    
    public override string CustomIconTexturePath => "res://Watcher/images/watcher/character_icon_watcher.png";
    public override string CustomCharacterSelectIconPath => "res://Watcher/images/watcher/char_select_watcher.png";

    public override CustomEnergyCounter? CustomEnergyCounter => new CustomEnergyCounter(EnergyCounterPaths,StsColors.red, StsColors.blue);
    public override string CustomEnergyCounterPath => "res://Watcher/scenes/watcher/watcher_energy_counter.tscn";

    private string EnergyCounterPaths(int i)
    {
        return "res://Watcher/images/ui/combat/energy_counters/watcher/watcher_orb_layer_"+i+".png";
    }
    
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

    public override string CustomCharacterSelectBg => "res://Watcher/scenes/watcher/char_select_bg_watcher.tscn";

    public override string CustomCharacterSelectTransitionPath =>
        "res://Watcher/images/watcher/transitions/watcher_transition_mat.tres";

    public override string CustomMapMarkerPath => "res://Watcher/images/watcher/map_marker_watcher.png";
    public override string CustomAttackSfx => "res://";
    public override string CustomCastSfx => "res://";
    public override string CustomDeathSfx => "res://";

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

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<PureWater>()
    ];

    public override float AttackAnimDelay => 0.15f;

    public override float CastAnimDelay => 0.25f;

    public override CardPoolModel CardPool => ModelDb.CardPool<WatcherCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WatcherRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WatcherPotionPool>();

    public override List<string> GetArchitectAttackVfx()
    {
        return
        [
            "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash", "vfx/vfx_bloody_impact",
            "vfx/vfx_rock_shatter"
        ];
    }
    
}
