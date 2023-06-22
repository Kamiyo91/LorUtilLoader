using System.Linq;
using HarmonyLib;
using UI;
using UtilLoader21341.Enum;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class StageHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(0)]
        [HarmonyPatch(typeof(UIBattleSettingPanel), "SetToggles")]
        public static void UIBattleSettingPanel_SetToggles(UIBattleSettingPanel __instance)
        {
            var stageId = Singleton<StageController>.Instance.GetStageModel().ClassInfo.id;
            if (!ModParameters.PackageIds.Contains(stageId.packageId)) return;
            var stageOption =
                ModParameters.StageOptions.FirstOrDefault(x =>
                    x.PackageId == stageId.packageId && x.StageId == stageId.id);
            if (stageOption?.PreBattleOptions == null || stageOption.PreBattleOptions.SetToggles) return;
            foreach (var currentAvailbleUnitslot in __instance.currentAvailbleUnitslots)
            {
                currentAvailbleUnitslot.SetToggle(false);
                currentAvailbleUnitslot.SetYesToggleState();
            }

            __instance.SetAvailibleText();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UnitDataModel), "IsLockUnit")]
        public static void UnitDataModel_IsLockUnit(UnitDataModel __instance, ref bool __result)
        {
            if (UI.UIController.Instance.CurrentUIPhase != UIPhase.BattleSetting) return;
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            if (stageModel == null || !ModParameters.PackageIds.Contains(stageModel.ClassInfo.id.packageId)) return;
            var stageOption = ModParameters.StageOptions.FirstOrDefault(x =>
                x.PackageId == stageModel.ClassInfo.id.packageId && x.StageId == stageModel.ClassInfo.id.id);
            if (stageOption?.PreBattleOptions == null) return;
            if (stageOption.PreBattleOptions.OnlySephirah)
            {
                __result = !__instance.isSephirah && __instance._ownerSephirah != SephirahType.None;
                return;
            }

            if (stageOption.PreBattleOptions.SephirahLocked)
                __result = __instance.isSephirah && (!stageOption.PreBattleOptions.UnlockedSephirah.Any() ||
                                                     stageOption.PreBattleOptions.UnlockedSephirah.Contains(
                                                         __instance._ownerSephirah));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StageLibraryFloorModel), "InitUnitList")]
        public static void StageLibraryFloorModel_InitUnitList(StageLibraryFloorModel __instance, StageModel stage,
            LibraryFloorModel floor)
        {
            if (!ModParameters.PackageIds.Contains(stage.ClassInfo.id.packageId)) return;
            var stageOption = ModParameters.StageOptions.FirstOrDefault(x =>
                x.PackageId == stage.ClassInfo.id.packageId && x.StageId == stage.ClassInfo.id.id);
            if (stageOption?.PreBattleOptions == null ||
                (stageOption.PreBattleOptions.CustomUnits.All(x => x.Floor != __instance.Sephirah) &&
                 stageOption.PreBattleOptions.SephirahUnits.All(x => x.Floor != __instance.Sephirah))) return;
            __instance._unitList.Clear();
            switch (stageOption.PreBattleOptions.BattleType)
            {
                case PreBattleType.CustomUnits:
                    UnitUtil.AddCustomUnits(__instance, stage, __instance._unitList, stageOption.PreBattleOptions,
                        stage.ClassInfo.id.packageId);
                    break;
                case PreBattleType.SephirahUnits:
                    UnitUtil.AddSephirahUnits(__instance, stage, __instance._unitList, stageOption.PreBattleOptions);
                    break;
                case PreBattleType.HybridUnits:
                    UnitUtil.AddSephirahUnits(__instance, stage, __instance._unitList, stageOption.PreBattleOptions);
                    UnitUtil.AddCustomUnits(__instance, stage, __instance._unitList, stageOption.PreBattleOptions,
                        stage.ClassInfo.id.packageId);
                    break;
            }

            if (!stageOption.PreBattleOptions.FillWithBaseUnits /*|| __instance._unitList.Count >= 5*/) return;
            foreach (var unitDataModel in floor.GetUnitDataList().Where(x => !x.isSephirah))
                //if (__instance._unitList.Count < 5)
                __instance._unitList.Add(UnitUtil.InitUnitDefault(stage, unitDataModel));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StageController), "BonusRewardWithPopup")]
        public static void StageController_BonusRewardWithPopup(LorId stageId)
        {
            if (!ModParameters.PackageIds.Contains(stageId.packageId)) return;
            var stageOption =
                ModParameters.StageOptions.FirstOrDefault(x =>
                    x.PackageId == stageId.packageId && x.StageId == stageId.id);
            if (stageOption?.StageRewardOptions == null) return;
            var message = false;
            foreach (var book in stageOption.StageRewardOptions.Books)
            {
                if (!message) message = true;
                Singleton<DropBookInventoryModel>.Instance.AddBook(new LorId(book.LorId.PackageId, book.LorId.Id),
                    book.Quantity);
            }

            foreach (var keypageId in stageOption.StageRewardOptions.Keypages.Where(keypageId =>
                         !Singleton<BookInventoryModel>.Instance.GetBookListAll().Exists(x =>
                             x.GetBookClassInfoId() == new LorId(keypageId.PackageId, keypageId.Id))))
            {
                if (!message) message = true;
                Singleton<BookInventoryModel>.Instance.CreateBook(new LorId(keypageId.PackageId,
                    keypageId.Id));
            }

            foreach (var card in stageOption.StageRewardOptions.Cards.Where(x =>
                         !stageOption.StageRewardOptions.SingleTimeReward ||
                         Singleton<InventoryModel>.Instance.GetCardCount(new LorId(x.LorId.PackageId, x.LorId.Id)) < 1))
            {
                if (!message) message = true;
                Singleton<InventoryModel>.Instance.AddCard(new LorId(card.LorId.PackageId, card.LorId.Id),
                    card.Quantity);
            }

            if (!message) return;
            UIAlarmPopup.instance.SetAlarmText(GenericUtil.GetEffectText(stageId.packageId, "Reward Added",
                stageOption.StageRewardOptions.MessageId));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UICharacterListPanel), "RefreshBattleUnitDataModel")]
        public static void RefreshBattleUnitDataModel(UICharacterListPanel __instance,
            UnitDataModel data)
        {
            if (Singleton<StageController>.Instance.GetStageModel() == null ||
                !ModParameters.PackageIds.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id
                    .packageId)) return;
            var stageId = Singleton<StageController>.Instance
                .GetStageModel()
                .ClassInfo.id;
            var unitOptions =
                ModParameters.StageOptions.FirstOrDefault(x =>
                    x.PackageId == stageId.packageId && x.StageId == stageId.id);
            if (unitOptions?.PreBattleOptions == null) return;
            var slot = __instance.CharacterList;
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            var sephirahUnitTypes = unitOptions.PreBattleOptions.SephirahUnits.Where(x =>
                x.Floor == Singleton<StageController>.Instance.CurrentFloor).SelectMany(x => x.SephirahUnit);
            var list = UnitUtil.UnitsToRecover(stageModel, data, sephirahUnitTypes);
            foreach (var unit in list)
            {
                unit.Refreshhp();
                var uicharacterSlot = slot?.slotList.Find(x => x.unitBattleData == unit);
                if (uicharacterSlot == null || uicharacterSlot.unitBattleData == null) continue;
                uicharacterSlot.ReloadHpBattleSettingSlot();
            }
        }

        [HarmonyPatch(typeof(StageClassInfo), "currentState", MethodType.Getter)]
        [HarmonyPostfix]
        public static void StageClassInfo_Get(StageClassInfo __instance, ref StoryState __result)
        {
            var stage = ModParameters.StageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.id.packageId && x.StageId == __instance.id.id);
            if (stage == null) return;
            if (stage.HidePreview)
            {
                __result = StoryState.FirstOpen;
                return;
            }

            if (stage.StageRequirements == null) return;
            if (UnitUtil.IsLocked(stage.StageRequirements)) __result = StoryState.Close;
            //__result = LibraryModel.Instance.ClearInfo.GetClearCount(__instance.id) > 0 ? __result = StoryState.Clear : LibraryModel.Instance.ClearInfo.IsUnlockedStage(__instance.id) ? StoryState.Open : StoryState.FirstOpen;
        }
    }
}