using System.Linq;
using HarmonyLib;
using UI;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class SkinHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "GetThumbSprite")]
        [HarmonyPatch(typeof(BookXmlInfo), "GetThumbSprite")]
        public static void General_GetThumbSprite(object __instance, ref Sprite __result)
        {
            switch (__instance)
            {
                case BookXmlInfo bookInfo:
                    ArtUtil.GetThumbSprite(bookInfo.id, ref __result);
                    break;
                case BookModel bookModel:
                    ArtUtil.GetThumbSprite(bookModel.BookId, ref __result);
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UICustomizePopup), "Open")]
        public static void UICustomizePopup_Open(UICustomizePopup __instance)
        {
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.SelectedUnit.bookItem.ClassInfo.id.packageId &&
                x.KeypageId == __instance.SelectedUnit.bookItem.ClassInfo.id.id);
            if (keypageItem?.BookCustomOptions == null) return;
            __instance.SelectedUnit.customizeData.SetCustomData(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UICustomizePopup), "OnClickExit")]
        public static void UICustomizePopup_OnClickExit(UICustomizePopup __instance)
        {
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.SelectedUnit.bookItem.ClassInfo.id.packageId &&
                x.KeypageId == __instance.SelectedUnit.bookItem.ClassInfo.id.id);
            if (keypageItem?.BookCustomOptions == null) return;
            if (__instance.SelectedUnit.bookItem == __instance.SelectedUnit.CustomBookItem &&
                string.IsNullOrEmpty(__instance.SelectedUnit.workshopSkin))
                __instance.SelectedUnit.customizeData.SetCustomData(keypageItem.BookCustomOptions.CustomFaceData);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UICustomizePopup), "OnClickSave")]
        public static void UICustomizePopup_OnClickSave(UICustomizePopup __instance)
        {
            var tempName = __instance.SelectedUnit._tempName;
            __instance.SelectedUnit.ResetTempName();
            if (string.IsNullOrEmpty(__instance.SelectedUnit.workshopSkin)) return;
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.SelectedUnit.bookItem.ClassInfo.id.packageId &&
                x.KeypageId == __instance.SelectedUnit.bookItem.ClassInfo.id.id);
            if (keypageItem?.BookCustomOptions != null)
            {
                __instance.SelectedUnit.customizeData.SetCustomData(keypageItem.BookCustomOptions.CustomFaceData);
                if (keypageItem.BookCustomOptions.NameTextId != 0) __instance.SelectedUnit.SetTempName(tempName);
                return;
            }

            var customSkinOption =
                ModParameters.CustomSkinOptions.FirstOrDefault(x =>
                    x.SkinName.Contains(__instance.SelectedUnit.workshopSkin));
            if (customSkinOption?.CharacterNameId == null) return;
            var locItem = ModParameters.LocalizedItems.FirstOrDefault(x => x.Key == customSkinOption.PackageId);
            if (locItem.Key == null || locItem.Value == null ||
                !locItem.Value.EnemyNames.TryGetValue(customSkinOption.CharacterNameId.Value, out var name))
                return;
            __instance.SelectedUnit.SetTempName(name);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "LoadFromSaveData")]
        public static void UnitDataModel_LoadFromSaveData(UnitDataModel __instance)
        {
            if (string.IsNullOrEmpty(__instance.workshopSkin)) return;
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.bookItem.ClassInfo.id.packageId &&
                x.KeypageId == __instance.bookItem.ClassInfo.id.id);
            if (keypageItem?.BookCustomOptions != null)
            {
                __instance.ResetTempName();
                return;
            }

            var skin = ModParameters.CustomSkinOptions.FirstOrDefault(x => x.SkinName == __instance.workshopSkin);
            if (skin?.CharacterNameId == null) return;
            var locItem = ModParameters.LocalizedItems.FirstOrDefault(x => x.Key == skin.PackageId);
            if (locItem.Key == null || locItem.Value == null ||
                !locItem.Value.EnemyNames.TryGetValue(skin.CharacterNameId.Value, out var name))
                return;
            __instance.SetTempName(name);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UILibrarianAppearanceInfoPanel), "OnClickCustomizeButton")]
        public static bool UILibrarianAppearanceInfoPanel_OnClickCustomizeButton(
            UILibrarianAppearanceInfoPanel __instance)
        {
            if (!ModParameters.PackageIds.Contains(__instance.unitData.bookItem.BookId.packageId)) return true;
            var keypageOption =
                ModParameters.KeypageOptions.FirstOrDefault(x =>
                    x.PackageId == __instance.unitData.bookItem.BookId.packageId &&
                    x.KeypageId == __instance.unitData.bookItem.BookId.id);
            if (keypageOption == null || keypageOption.Editable) return true;
            UIAlarmPopup.instance.SetAlarmText(GenericUtil.GetEffectText(__instance.unitData.bookItem.BookId.packageId,
                "Can't edit this keypage", keypageOption.EditErrorMessageId));
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FarAreaEffect_Xiao_Taotie), "LateInit")]
        public static void FarAreaEffect_Xiao_Taotie_LateInit(FarAreaEffect_Xiao_Taotie __instance)
        {
            var keypageItem =
                ModParameters.KeypageOptions.FirstOrDefault(x =>
                    x.PackageId == __instance._self.UnitData.unitData.bookItem.ClassInfo.id.packageId &&
                    x.KeypageId == __instance._self.UnitData.unitData.bookItem.ClassInfo.id.id);
            if (keypageItem?.BookCustomOptions == null ||
                keypageItem.BookCustomOptions.XiaoTaotieAction == ActionDetail.NONE) return;
            __instance._self.view.charAppearance.ChangeMotion(keypageItem.BookCustomOptions.XiaoTaotieAction);
        }
    }
}