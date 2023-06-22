using System.Linq;
using HarmonyLib;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class CustomFloorHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIFloorTitlePanel), "SetData")]
        public static void UIFloorTitlePanel_SetData(UIFloorTitlePanel __instance, SephirahType sep)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(sep, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            if (string.IsNullOrEmpty(savedOptions.FloorOptions.IconId)) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            __instance.img_floorTitle.sprite = icon.Sprite;
            var name = GenericUtil.GetEffectText(savedOptions.FloorOptions.PackageId, "Custom Floor",
                savedOptions.FloorOptions.FloorNameId, true);
            if (!string.IsNullOrEmpty(name)) __instance.txt_titlename.text = name;
            __instance.txt_titlename.rectTransform.sizeDelta = new Vector2(__instance.txt_titlename.preferredWidth,
                __instance.txt_titlename.rectTransform.sizeDelta.y);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISephirahButton), "SetButtonState")]
        public static void UISephirahButton_SetButtonState(UISephirahButton __instance)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(__instance.sephirahType, out var savedOptions))
                return;
            if (!savedOptions.IsActive) return;
            if (string.IsNullOrEmpty(savedOptions.FloorOptions.IconId)) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            __instance.img_Icon.sprite = icon.Sprite;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISephirahSelectionButton), "InitAndActivate")]
        public static void UISephirahSelectionButton_InitAndActivate(UISephirahSelectionButton __instance,
            SephirahType _sephirahType)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(_sephirahType, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            if (string.IsNullOrEmpty(savedOptions.FloorOptions.IconId)) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            __instance.sephirahImage.sprite = icon.Sprite;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleEmotionBarTeamSlotUI), "InitPlayerTeamIcon")]
        public static void BattleEmotionBarTeamSlotUI_InitPlayerTeamIcon(BattleEmotionBarTeamSlotUI __instance,
            EmotionBattleTeamModel team)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(team.sep, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            if (string.IsNullOrEmpty(savedOptions.FloorOptions.IconId)) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            __instance.img_Icon.sprite = icon.Sprite;
        }

        [HarmonyPatch(typeof(UISephirahFloor), "Init")]
        [HarmonyPostfix]
        public static void UISephirahFloor_Init_Post(UISephirahFloor __instance)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(__instance.sephirah, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            if (__instance.sephirah != SephirahType.Keter) __instance.imgLockIcon.sprite = icon.Sprite;
            else if (ModParameters.DaatFloorFound)
                __instance.transform.GetChild(0).GetChild(3).gameObject.GetComponent<Image>().sprite = icon.Sprite;
            else
                __instance.transform.GetChild(1).GetChild(3).gameObject.GetComponent<Image>().sprite = icon.Sprite;
        }

        [HarmonyPatch(typeof(LevelUpUI), "InitBase")]
        [HarmonyPostfix]
        public static void LevelUpUI_InitBase_Post(LevelUpUI __instance)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(Singleton<StageController>.Instance.CurrentFloor,
                    out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            var icon = ModParameters.ArtWorks.FirstOrDefault(x =>
                x.PackageId == savedOptions.FloorOptions.PackageId && x.Name == savedOptions.FloorOptions.IconId);
            if (icon == null) return;
            __instance.FloorIconImage.sprite = icon.Sprite;
            __instance.ego_FloorIconImage.sprite = icon.Sprite;
            __instance.NeedSelectAb_FloorIconImage.sprite = icon.Sprite;
        }

        [HarmonyPatch(typeof(BattleSceneRoot), "LoadFloor")]
        [HarmonyPostfix]
        public static void BattleSceneRoot_LoadFloor(BattleSceneRoot __instance, SephirahType sephirah)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(sephirah, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            if (savedOptions.FloorOptions.CustomFloorMap == null) return;
            foreach (var map in __instance.mapList.Where(x => x.sephirahType == sephirah))
            {
                map.gameObject.SetActive(false);
                Object.Destroy(map.gameObject);
            }

            __instance.mapList.RemoveAll(x => x.sephirahType == sephirah);
            MapUtil.InitSephirahMap(savedOptions.FloorOptions.PackageId, savedOptions.FloorOptions.CustomFloorMap,
                sephirah);
        }

        [HarmonyPatch(typeof(BattleSceneRoot), "ChangeToSephirahMap")]
        [HarmonyPostfix]
        public static void BattleSceneRoot_ChangeToSephirahMap(SephirahType sephirah, bool playEffect)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(sephirah, out var savedOptions)) return;
            if (!savedOptions.IsActive) return;
            if (savedOptions.FloorOptions.CustomFloorMap == null) return;
            MapUtil.ChangeToSephirahMap(savedOptions.FloorOptions.PackageId, savedOptions.FloorOptions.CustomFloorMap,
                sephirah, playEffect);
        }
    }
}