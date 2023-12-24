using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UI;
using UtilLoader21341.Comparers;
using UtilLoader21341.Models;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class PassiveHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "CanSuccessionPassive")]
        public static void BookModel_CanSuccessionPassive(BookModel __instance, PassiveModel targetpassive,
            ref GivePassiveState haspassiveState, ref bool __result)
        {
            var passiveOptions = ModParameters.PassiveOptions
                .Where(x => x.PackageId == targetpassive.originData.currentpassive.id.packageId).ToList();
            if (passiveOptions.Any())
            {
                var passiveItem = passiveOptions.FirstOrDefault(x =>
                    x.PackageId == targetpassive.originData.currentpassive.id.packageId &&
                    x.PassiveId == targetpassive.originData.currentpassive.id.id);
                if (passiveItem == null) return;
                var unitPassiveList = __instance.GetPassiveModelList();
                if (__instance.GetPassiveModelList().Exists(x =>
                        passiveItem.CannotBeUsedWithPassives.Contains(x.reservedData.currentpassive.id.ToLorIdRoot(),
                            new LorIdRootComparer())))
                {
                    haspassiveState = GivePassiveState.Lock;
                    __result = false;
                    return;
                }

                if (!passiveItem.CanBeUsedWithPassivesAll.All(passiveId =>
                        unitPassiveList.Exists(x =>
                            x.reservedData.currentpassive.id.id == passiveId.Id &&
                            x.reservedData.currentpassive.id.packageId == passiveId.PackageId)))
                {
                    haspassiveState = GivePassiveState.Lock;
                    __result = false;
                    return;
                }

                if (passiveItem.CanBeUsedWithPassivesOne.Any() && !__instance.GetPassiveModelList().Exists(x =>
                        passiveItem.CanBeUsedWithPassivesOne.Contains(x.reservedData.currentpassive.id.ToLorIdRoot(),
                            new LorIdRootComparer())))
                {
                    haspassiveState = GivePassiveState.Lock;
                    __result = false;
                    return;
                }

                if (!passiveItem.IsMultiDeck ||
                    (!__instance.ClassInfo.categoryList.Contains(BookCategory.DeckFixed) &&
                     !__instance.ClassInfo.optionList.Contains(BookOption.MultiDeck) &&
                     !__instance.IsMultiDeck())) return;
                haspassiveState = GivePassiveState.Lock;
                __result = false;
            }
            else
            {
                var cannotBeUsedList = ModParameters.PassiveOptions
                    .Where(x => x.CannotBeUsedWithPassives.Contains(targetpassive.originData.currentpassive.id
                        .ToLorIdRoot(), new LorIdRootComparer()))
                    .Select(x => new LorId(x.PackageId, x.PassiveId));
                if (!__instance.GetPassiveModelList()
                        .Exists(x => cannotBeUsedList.Contains(x.reservedData.currentpassive.id))) return;
                haspassiveState = GivePassiveState.Lock;
                __result = false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "IsMultiDeck")]
        public static void BookModel_IsMultiDeck(BookModel __instance, ref bool __result)
        {
            try
            {
                __result = __instance.GetPassiveInfoList()
                               .Exists(x => ModParameters.PassiveOptions.Any(y =>
                                   y.PackageId == x.passive.id.packageId && y.PassiveId == x.passive.id.id &&
                                   y.IsMultiDeck)) ||
                           __result;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UILibrarianEquipDeckPanel), "IsMultiDeck")]
        public static void UILibrarianEquipDeckPanel_IsMultiDeck(UILibrarianEquipDeckPanel __instance,
            ref bool __result)
        {
            __result = (__instance.Unitdata != null && __instance.Unitdata.bookItem.GetPassiveInfoList()
                .Exists(x => ModParameters.PassiveOptions.Any(y =>
                    y.PackageId == x.passive.id.packageId &&
                    y.PassiveId == x.passive.id.id && y.IsMultiDeck))) || __result;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BookModel), "ReleasePassive")]
        public static void BookModel_ReleasePassive(BookModel __instance, PassiveModel passive)
        {
            try
            {
                var currentPassive = passive.originData.currentpassive.id != new LorId(9999999)
                    ? passive.originData.currentpassive
                    : passive.reservedData.currentpassive;
                var passiveItem = ModParameters.PassiveOptions.FirstOrDefault(x =>
                    x.PackageId == currentPassive.id.packageId && x.PassiveId == currentPassive.id.id);
                if (passiveItem == null) return;
                var passivesToRelease = __instance.GetPassiveModelList().Where(x =>
                    passiveItem.ChainReleasePassives.Contains(x.reservedData.currentpassive.id.ToLorIdRoot(),
                        new LorIdRootComparer()));
                foreach (var passiveToRelease in passivesToRelease)
                    __instance.ReleasePassive(passiveToRelease);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BookModel), "UnEquipGivePassiveBook")]
        public static void BookModel_UnEquipGivePassiveBook(BookModel __instance, BookModel unequipbook)
        {
            try
            {
                var passiveOptions = ModParameters.PassiveOptions.Where(x => unequipbook.GetPassiveModelList().Exists(
                    y => (x.PackageId == y.originData.currentpassive.id.packageId &&
                          x.PassiveId == y.originData.currentpassive.id.id) ||
                         (x.PackageId == y.reservedData.currentpassive.id.packageId &&
                          x.PassiveId == y.reservedData.currentpassive.id.id))).ToList();
                if (ModParameters.EmotionCardUtilLoaderFound)
                {
                    var passives = passiveOptions.Where(x => x.CustomFloorOptions != null);
                    foreach (var p in passives)
                        CustomFloorUtil.ResetFloorByPassiveId(p.CustomFloorOptions.PackageId, p.PassiveId);
                }

                var passiveOptionsToRelease = passiveOptions.SelectMany(x => x.ChainReleasePassives).ToList();
                var passiveToRelease = __instance.GetPassiveModelList().Where(x =>
                    passiveOptionsToRelease.Contains(x.originData.currentpassive.id.ToLorIdRoot(),
                        new LorIdRootComparer()) ||
                    passiveOptionsToRelease.Contains(x.reservedData.currentpassive.id.ToLorIdRoot(),
                        new LorIdRootComparer()));
                foreach (var passiveRelease in passiveToRelease.Distinct())
                    __instance.ReleasePassive(passiveRelease);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PassiveModel), "ReleaseSuccesionGivePassive")]
        public static void PassiveModel_ReleaseSuccesionGivePassive(PassiveModel __instance)
        {
            try
            {
                var currentPassive = __instance.originData.currentpassive.id != new LorId(9999999)
                    ? __instance.originData
                    : __instance.reservedData;
                var passiveItem = ModParameters.PassiveOptions.FirstOrDefault(x =>
                    x.PackageId == currentPassive.currentpassive.id.packageId &&
                    x.PassiveId == currentPassive.currentpassive.id.id);
                if (passiveItem == null) return;
                var book = Singleton<BookInventoryModel>.Instance.GetBookByInstanceId(currentPassive.givePassiveBookId);
                var passiveModels = book != null
                    ? book.GetPassiveModelList().Where(x =>
                        passiveItem.ChainReleasePassives.Contains(x.originData.currentpassive.id.ToLorIdRoot(),
                            new LorIdRootComparer()))
                    : Singleton<BookInventoryModel>.Instance.GetBlackSilenceBook().GetPassiveModelList().Where(
                        x => passiveItem.ChainReleasePassives.Contains(x.originData.currentpassive.id.ToLorIdRoot(),
                            new LorIdRootComparer()));
                foreach (var passiveModel in passiveModels)
                    passiveModel.ReleaseSuccesionReceivePassive(true);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEquipDeckCardList), "SetDeckLayout")]
        public static void UIEquipDeckCardList_SetDeckLayout(UIEquipDeckCardList __instance)
        {
            var keypageMultiDeck = ModParameters.KeypageOptions.Any(x =>
                x.PackageId == __instance.currentunit.bookItem.BookId.packageId &&
                x.KeypageId == __instance.currentunit.bookItem.BookId.id && x.IsMultiDeck);
            var passiveMultiDeck = ModParameters.PassiveOptions.Any(x => __instance
                .currentunit.bookItem.GetPassiveInfoList().Exists(y =>
                    x.PackageId == y.passive.id.packageId && x.PassiveId == y.passive.id.id && x.IsMultiDeck));
            if (keypageMultiDeck || passiveMultiDeck)
            {
                var labels = new List<string>();
                var packageId = string.Empty;

                var keypageOption = ModParameters.KeypageOptions.FirstOrDefault(x =>
                    x.PackageId == __instance.currentunit.bookItem.BookId.packageId &&
                    x.KeypageId == __instance.currentunit.bookItem.BookId.id && x.IsMultiDeck);
                if (keypageOption != null)
                {
                    labels = keypageOption.MultiDeckLabelIds;
                    packageId = __instance.currentunit.bookItem.BookId.packageId;
                }

                if (!labels.Any())
                {
                    PassiveOptionRoot item = null;
                    foreach (var passiveOption in ModParameters.PassiveOptions.Where(passiveOption =>
                                 __instance.currentunit.bookItem.GetPassiveInfoList().Exists(x =>
                                     x.passive.id.packageId == passiveOption.PackageId &&
                                     x.passive.id.id == passiveOption.PassiveId && passiveOption.IsMultiDeck)))
                    {
                        item = passiveOption;
                        packageId = passiveOption.PackageId;
                    }

                    if (item == null) return;
                    labels = item.MultiDeckLabelIds;
                }

                UIOptions.ChangedMultiView = true;
                if (__instance.currentunit.bookItem.GetCurrentDeckIndex() > labels.Count)
                    __instance.currentunit.ReEquipDeck();
                ArtUtil.PrepareMultiDeckUI(__instance.multiDeckLayout, labels,
                    packageId);
            }
            else if (UIOptions.ChangedMultiView)
            {
                UIOptions.ChangedMultiView = false;
                ArtUtil.RevertMultiDeckUI(__instance.multiDeckLayout);
                __instance.SetDeckLayout();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIPassiveSuccessionPopup), "Close")]
        public static void UIPassiveSuccessionPopup_Close_Pre(UIPassiveSuccessionPopup __instance)
        {
            if (__instance.CurrentBookModel == null) return;
            try
            {
                if (ModParameters.EmotionCardUtilLoaderFound && __instance._currentUnit != null &&
                    __instance._currentUnit.isSephirah && ModParameters.EgoAndEmotionCardChanged.ContainsKey(__instance
                        ._currentUnit
                        .OwnerSephirah)) CustomFloorUtil.ResetFloor(__instance._currentUnit.OwnerSephirah);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIPassiveSuccessionPopup), "Close")]
        public static void UIPassiveSuccessionPopup_Close(UIPassiveSuccessionPopup __instance)
        {
            if (__instance.CurrentBookModel == null) return;
            try
            {
                if (ModParameters.EmotionCardUtilLoaderFound && __instance._currentUnit != null &&
                    __instance._currentUnit.isSephirah)
                {
                    var passive = ModParameters.PassiveOptions.FirstOrDefault(x =>
                        __instance.CurrentBookModel.GetPassiveInfoList().Exists(y =>
                            x.PackageId == y.passive.id.packageId && x.PassiveId == y.passive.id.id &&
                            x.CustomFloorOptions != null));
                    if (passive != null)
                        CustomFloorUtil.ChangeFloor(passive.CustomFloorOptions,
                            __instance._currentUnit.OwnerSephirah,
                            __instance.CurrentBookModel.BookId.id, passive.PassiveId);
                    var ui =
                        UI.UIController.Instance.GetUIPanel(UIPanelType.CharacterList_Right) as
                            UILibrarianCharacterListPanel;
                    ui?.SetLibrarianCharacterListPanel_Default(__instance._currentUnit.OwnerSephirah);
                }

                SingletonBehavior<UIEquipDeckCardList>.Instance.SetDeckLayout();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}