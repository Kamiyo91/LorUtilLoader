using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UI;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class KeypageHarmonyPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnitDataModel), "EquipBook")]
        public static void UnitDataModel_EquipBookPrefix(UnitDataModel __instance, BookModel newBook, bool force,
            ref BookModel __state)
        {
            if (force) return;
            __state = newBook;
            if (ModParameters.EmotionCardUtilLoaderFound && __instance.isSephirah &&
                ModParameters.EgoAndEmotionCardChanged.ContainsKey(__instance.OwnerSephirah))
            {
                CustomFloorUtil.ResetFloor(__instance.OwnerSephirah);
                if (UI.UIController.Instance.CurrentUIPhase == UIPhase.BattleSetting)
                {
                    var ui =
                        UI.UIController.Instance.GetUIPanel(UIPanelType.CharacterList_Right) as
                            UILibrarianCharacterListPanel;
                    ui?.SetLibrarianCharacterListPanel_Battle();
                }
            }

            if (!ModParameters.PackageIds.Contains(__instance.bookItem.ClassInfo.id.packageId)) return;
            var bookOptions =
                ModParameters.KeypageOptions.FirstOrDefault(x =>
                    x.PackageId == __instance.bookItem.ClassInfo.id.packageId &&
                    x.KeypageId == __instance.bookItem.ClassInfo.id.id && x.BookCustomOptions != null);
            if (bookOptions == null) return;
            __instance.ResetTempName();
            __instance.customizeData.SetCustomData(true);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "EquipBook")]
        public static void UnitDataModel_EquipBookPostfix(UnitDataModel __instance, bool force, BookModel __state)
        {
            if (force) return;
            var floorChanged = false;
            if (ModParameters.EmotionCardUtilLoaderFound && __instance.isSephirah)
            {
                var customFloorPassive = ModParameters.PassiveOptions.FirstOrDefault(x =>
                    __state.GetPassiveInfoList().Exists(y =>
                        y.passive.id.id == x.PassiveId && y.passive.id.packageId == x.PackageId &&
                        x.CustomFloorOptions != null));
                if (customFloorPassive != null)
                {
                    floorChanged = true;
                    CustomFloorUtil.ChangeFloor(customFloorPassive.CustomFloorOptions, __instance.OwnerSephirah,
                        __state.ClassInfo.id.id, customFloorPassive.PassiveId);
                    ArtUtil.ReloadPreBattleIconsUI();
                }
            }

            if (__state == null || !ModParameters.PackageIds.Contains(__state.ClassInfo.id.packageId)) return;
            var bookOptions = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __state.ClassInfo.id.packageId && x.KeypageId == __state.ClassInfo.id.id);
            if (bookOptions == null) return;
            if (bookOptions.BookCustomOptions != null)
            {
                if (!bookOptions.Editable) __instance.EquipCustomCoreBook(null);
                if (bookOptions.BookCustomOptions.EgoSkin.Contains(__state.GetCharacterName()) ||
                    __state.ClassInfo.CharacterSkin.Any(x => bookOptions.BookCustomOptions.EgoSkin.Contains(x)))
                    if (bookOptions.BookCustomOptions.OriginalSkinIsBaseGame)
                        __state.SetCharacterName(bookOptions.BookCustomOptions.OriginalSkin);
                    else
                        __state.ClassInfo.CharacterSkin = new List<string>
                        {
                            bookOptions.BookCustomOptions.OriginalSkin
                        };
                if (ModParameters.EmotionCardUtilLoaderFound && __instance.isSephirah && !floorChanged)
                {
                    if (bookOptions.CustomFloorOptions != null)
                        CustomFloorUtil.ChangeFloor(bookOptions.CustomFloorOptions, __instance.OwnerSephirah,
                            __state.ClassInfo.id.id);
                    ArtUtil.ReloadPreBattleIconsUI();
                }

                if (!UnitUtil.CheckSkinUnitData(__instance))
                {
                    __instance.customizeData.SetCustomData(bookOptions.BookCustomOptions.CustomFaceData);
                    var locTryGet =
                        ModParameters.LocalizedItems.TryGetValue(__state.BookId.packageId, out var localizedItem);
                    if (locTryGet && localizedItem.EnemyNames.TryGetValue(
                            bookOptions.BookCustomOptions.NameTextId,
                            out var name))
                        __instance.SetTempName(name);
                }
            }

            if (((bookOptions.EveryoneCanEquip && (__instance.OwnerSephirah == SephirahType.Keter ||
                                                   __instance.OwnerSephirah == SephirahType.Binah)) ||
                 (bookOptions.SephirahType == SephirahType.Keter && __instance.OwnerSephirah == SephirahType.Keter) ||
                 (bookOptions.SephirahType == SephirahType.Binah && __instance.OwnerSephirah == SephirahType.Binah)) &&
                __instance.isSephirah)
                __instance.EquipBook(__state, false, true);
        }

        [HarmonyPatch(typeof(UnitDataModel), "ResetForBlackSilence")]
        [HarmonyPrefix]
        private static void UnitDataModel_ResetForBlackSilence_Pre(UnitDataModel __instance, ref BookModel __state)
        {
            __state = __instance.bookItem;
        }

        [HarmonyPatch(typeof(UnitDataModel), "ResetForBlackSilence")]
        [HarmonyPostfix]
        private static void UnitDataModel_ResetForBlackSilence_Post(UnitDataModel __instance, BookModel __state)
        {
            if (__state == null) return;
            var keypageOption = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __state.ClassInfo.id.packageId && x.KeypageId == __state.ClassInfo.id.id);
            if (keypageOption == null) return;
            if (__instance.isSephirah && __instance.OwnerSephirah == SephirahType.Keter &&
                !LibraryModel.Instance.IsBlackSilenceLockedInLibrary() && (keypageOption.EveryoneCanEquip ||
                                                                           (keypageOption.SephirahType ==
                                                                            SephirahType.Keter &&
                                                                            keypageOption.OnlySephirahCanEquip)))
                __instance.EquipBook(__state, false, true);
        }
    }
}