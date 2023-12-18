using System;
using System.Collections.Generic;
using System.Reflection;
using LOR_DiceSystem;
using LOR_XML;
using UnityEngine;
using UtilLoader21341.Enum;
using UtilLoader21341.Models;
using UtilLoader21341.StageManager;
using UtilLoader21341.Util;

namespace UtilLoader21341
{
    public static class ModParameters
    {
        public static string UtilDLLPackageId = "DLLUtilLoader21341";
        public static string BaseFolderUri = "/UtilLoader/";
        public static string DLLName = "1UtilLoader21341";
        public static HarmonyLib.Harmony Harmony = new HarmonyLib.Harmony("LOR.1UtilLoader21341_MOD");
        public static string Language = GlobalGameManager.Instance.CurrentOption.language;
        public static List<Assembly> Assemblies = new List<Assembly>();
        public static List<string> PackageIds = new List<string>();
        public static List<CustomSprite> ArtWorks = new List<CustomSprite>();
        public static List<DefaultKeywordOption> DefaultKeyWordOptions = new List<DefaultKeywordOption>();
        public static List<CardOptionRoot> CardOptions = new List<CardOptionRoot>();
        public static List<CategoryOptionRoot> CategoryOptions = new List<CategoryOptionRoot>();
        public static List<RewardOptionRoot> RewardOptions = new List<RewardOptionRoot>();
        public static List<KeypageOptionRoot> KeypageOptions = new List<KeypageOptionRoot>();
        public static List<PassiveOptionRoot> PassiveOptions = new List<PassiveOptionRoot>();
        public static List<SkinOptionRoot> SkinOptions = new List<SkinOptionRoot>();
        public static List<CustomSkinOptionRoot> CustomSkinOptions = new List<CustomSkinOptionRoot>();
        public static List<SpriteOptionRoot> SpriteOptions = new List<SpriteOptionRoot>();
        public static List<StageOptionRoot> StageOptions = new List<StageOptionRoot>();
        public static List<BuffOptionRoot> BuffOptions = new List<BuffOptionRoot>();
        public static List<MapModelRoot> MapModels = new List<MapModelRoot>();
        public static List<UnitModelRoot> UnitModels = new List<UnitModelRoot>();
        public static List<RushBattleModelMainRoot> RushBattleModels = new List<RushBattleModelMainRoot>();
        public static Dictionary<string, LocalizedItem> LocalizedItems = new Dictionary<string, LocalizedItem>();
        public static Dictionary<string, Type> CustomEffects = new Dictionary<string, Type>();
        public static Dictionary<string, string> Path = new Dictionary<string, string>();

        public static Dictionary<SephirahType, SavedFloorOptions> EgoAndEmotionCardChanged =
            new Dictionary<SephirahType, SavedFloorOptions>();

        public static LorId OnPlayEmotionCardUsedBy = null;
        public static bool OnPlayCardEmotion = false;
        public static bool DaatFloorFound = false;
        public static bool EmotionCardUtilLoaderFound = false;
        public static bool ColorCardCardUtilLoaderFound = false;
        public static bool BaseModFound = false;
        public static bool ModLoaded = false;
        public static bool ChangingAct = false;

        public static Tuple<string, List<string>> NextActManager =
            new Tuple<string, List<string>>(string.Empty, new List<string>());

        public static int StartWaveIndex = 0;
        public static Tuple<bool, int> ChangedFormation = new Tuple<bool, int>(false, 0);
        public static FieldInfo MatchInfoEmotionSelection = null;
        public static Dictionary<string, Assets> AssetBundle = new Dictionary<string, Assets>();
        public static EmenyTeamStageManager_RushBattleLoader_24321 RushBattleManager = null;
    }

    public class CustomSprite
    {
        public string PackageId { get; set; }
        public string Name { get; set; }
        public Sprite Sprite { get; set; }
    }

    public class LocalizedItem
    {
        public Dictionary<int, string> CardNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> DropBookNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> StageNames { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> EnemyNames { get; set; } = new Dictionary<int, string>();

        public Dictionary<string, List<string>> BattleCardAbilitiesText { get; set; } =
            new Dictionary<string, List<string>>();

        public Dictionary<string, EffectText> EffectTexts { get; set; } = new Dictionary<string, EffectText>();
        public Dictionary<int, EffectText> PassiveTexts { get; set; } = new Dictionary<int, EffectText>();
        public List<BattleDialogCharacter> BattleDialogCharacterList { get; set; } = new List<BattleDialogCharacter>();
        public List<BookDesc> Keypages { get; set; } = new List<BookDesc>();
        public Dictionary<string, string> Etc { get; set; } = new Dictionary<string, string>();
    }

    public static class UIOptions
    {
        public static bool ChangedMultiView;
    }

    public class SavedFloorOptions
    {
        public SavedFloorOptions(bool isActive = false, CustomFloorOptionRoot floorOptions = null,
            int? keypageId = null)
        {
            IsActive = isActive;
            FloorOptions = floorOptions;
            KeypageId = keypageId;
        }

        public int? KeypageId { get; set; }
        public bool IsActive { get; set; }
        public CustomFloorOptionRoot FloorOptions { get; set; }
    }

    public class EffectText
    {
        public string Name { get; set; }
        public string Desc { get; set; }
    }

    public class AtkResistType
    {
        public AtkResistType(BehaviourDetail behaviourDetail, CustomDamageType damageType,
            AtkResist atkResist = AtkResist.None)
        {
            AtkResist = atkResist;
            BehaviourDetail = behaviourDetail;
            DamageType = damageType;
        }

        public AtkResist AtkResist { get; set; }
        public BehaviourDetail BehaviourDetail { get; set; }
        public CustomDamageType DamageType { get; set; }
    }

    public class FarAreaMassAttackEffectParameters
    {
        public string AttackEffect;
        public float AttackEffectScale;
        public ActionDetail AttackMotion;
        public string AudioFileName;
        public bool CharacterMove;
        public bool FollowUnits;
        public bool IsBaseGameAudio;
        public string PackageId;
        public bool SlowMotion;
        public bool Zoom;

        public FarAreaMassAttackEffectParameters(string packageId, ActionDetail attackMotion, string audioFileName,
            string attackEffect,
            float attackEffectScale = 1f, bool isBaseGameAudio = false, bool slowMotion = true, bool zoom = true,
            bool characterMove = true, bool followUnits = true)
        {
            AttackMotion = attackMotion;
            AudioFileName = audioFileName;
            AttackEffect = attackEffect;
            AttackEffectScale = attackEffectScale;
            IsBaseGameAudio = isBaseGameAudio;
            SlowMotion = slowMotion;
            Zoom = zoom;
            CharacterMove = characterMove;
            FollowUnits = followUnits;
            PackageId = packageId;
        }
    }
}