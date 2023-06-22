using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EmotionCardUtil;
using HarmonyLib;
using LOR_XML;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class EmotionSelectionUnitPatch
    {
        private static readonly Predicate<BattleUnitModel> MatchAddon = x =>
            ModParameters.KeypageOptions.Any(y =>
                y.PackageId == x.Book.BookId.packageId && y.KeypageId == x.Book.BookId.id && y.BannedEmotionCards) ||
            ModParameters.PassiveOptions.Any(y =>
                x.passiveDetail.PassiveList.Any(z => y.PackageId == z.id.packageId && y.PassiveId == z.id.id) &&
                y.BannedEmotionCardSelection);


        [HarmonyPostfix]
        public static void LevelUpUI_Predicate_Patch(LevelUpUI __instance, BattleUnitModel x, ref bool __result)
        {
            if (x?.Book == null) return;
            if (ModParameters.OnPlayEmotionCardUsedBy != null)
            {
                __result |= x.Book.BookId != ModParameters.OnPlayEmotionCardUsedBy;
                return;
            }

            if (ModParameters.EmotionCardUtilLoaderFound)
                if (CheckCustomEmotionCard(__instance, x, ref __result))
                    return;
            __result |= MatchAddon(x);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool CheckCustomEmotionCard(LevelUpUI instance, BattleUnitModel x, ref bool result)
        {
            if (!EmotionCardUtil.ModParameters.EmotionEgoCards.Any()) return false;
            var emotionCards = EmotionCardUtil.ModParameters.EmotionCards.Where(y => y != null);
            if (instance.selectedEmotionCard == null ||
                !(instance.selectedEmotionCard.Card is EmotionCardXmlExtension card)) return false;
            var cardOptions = emotionCards.FirstOrDefault(y => y.LorId == card.LorId);
            if (cardOptions == null || !cardOptions.UsableByBookIds.Any()) return false;
            result |= !cardOptions.UsableByBookIds.Contains(x.Book.BookId.ToEmotionLorIdRoot(),
                new LorIdRootEmotionComparer()) && MatchAddon(x);
            return true;
        }

        [HarmonyTargetMethod]
        public static MethodBase LevelUpUI_Predicate_Find()
        {
            var typeInfo = typeof(LevelUpUI).GetTypeInfo().DeclaredNestedTypes
                .FirstOrDefault(x => x.Name.Contains("<>c"));
            ModParameters.MatchInfoEmotionSelection =
                typeInfo?.DeclaredFields.FirstOrDefault(x => x.Name.Contains("<>9__55_0"));
            return typeInfo?.DeclaredMethods.FirstOrDefault(x => x.Name.Contains("OnSelectRoutine"));
        }
    }

    [HarmonyPatch]
    public class BlockUiRepeat
    {
        private static FieldInfo _state;

        [HarmonyPrefix]
        public static void LevelUpUI_OnSelectRoutine_Pre(object __instance, ref int __state)
        {
            __state = (int)_state.GetValue(__instance);
        }

        [HarmonyPostfix]
        public static void LevelUpUI_OnSelectRoutine_Post(object __instance, ref int __state)
        {
            if (__state != 1 || (int)_state.GetValue(__instance) != -1 ||
                !SingletonBehavior<BattleManagerUI>.Instance.ui_levelup._needUnitSelection) return;
            var list = BattleObjectManager.instance.GetAliveList(Faction.Player);
            list.RemoveAll((Predicate<BattleUnitModel>)ModParameters.MatchInfoEmotionSelection.GetValue(null));
            if (list.Count > 0) return;
            StageController.Instance.GetCurrentStageFloorModel().team.egoSelectionPoint--;
            StageController.Instance.GetCurrentStageFloorModel().team.currentSelectEmotionLevel++;
            SingletonBehavior<BattleManagerUI>.Instance.ui_levelup._needUnitSelection = false;
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Player))
                UnitUtil.BattleAbDialog(unit.view.dialogUI, new List<AbnormalityCardDialog>
                {
                    new AbnormalityCardDialog
                    {
                        id = "EmotionError",
                        dialog = "Emotion Error, can't be used on any character"
                    }
                }, Color.red);
        }

        [HarmonyTargetMethod]
        public static MethodBase LevelUpUIOnSelectRoutine_Find()
        {
            var typeInfo = typeof(LevelUpUI).GetTypeInfo().DeclaredNestedTypes
                .FirstOrDefault(x => x.Name.Contains("<OnSelectRoutine>d__55"));
            _state = typeInfo?.DeclaredFields.ToList().FirstOrDefault(x => x.Name.Contains("__state"));
            return typeInfo?.DeclaredMethods.ToList().FirstOrDefault(x => x.Name.Contains("MoveNext"));
        }
    }

    [HarmonyPatch]
    public class LevelUpUIHotfix
    {
        [HarmonyPatch(typeof(LevelUpUI), "Init")]
        [HarmonyPrefix]
        public static void LevelUpUI_Init(LevelUpUI __instance, ref int count)
        {
            if (count >= __instance._emotionLevels.Length)
                count = __instance._emotionLevels.Length - 1;
        }

        [HarmonyPatch(typeof(LevelUpUI), "InitEgo")]
        [HarmonyPrefix]
        public static void LevelUpUI_InitEgo(LevelUpUI __instance, ref int count)
        {
            if (count >= __instance._emotionLevels.Length)
                count = __instance._emotionLevels.Length - 1;
        }

        [HarmonyPatch(typeof(StageLibraryFloorModel), "OnPickPassiveCard")]
        [HarmonyPrefix]
        public static void StageLibraryFloorModel_OnPickPassiveCard_Pre()
        {
            ModParameters.OnPlayEmotionCardUsedBy = null;
        }

        [HarmonyPatch(typeof(StageLibraryFloorModel), "OnPickPassiveCard")]
        [HarmonyPostfix]
        public static void StageLibraryFloorModel_OnPickPassiveCard_Post(StageLibraryFloorModel __instance)
        {
            if (!ModParameters.OnPlayCardEmotion) return;
            ModParameters.OnPlayCardEmotion = false;
            __instance.team.currentSelectEmotionLevel--;
        }
    }

    [HarmonyPatch]
    public class EmotionSelectionUnitPatchWithoutEmotionUtil
    {
        private static readonly Predicate<BattleUnitModel> MatchAddon = x =>
            ModParameters.KeypageOptions.Any(y =>
                y.PackageId == x.Book.BookId.packageId && y.KeypageId == x.Book.BookId.id && y.BannedEmotionCards) ||
            ModParameters.PassiveOptions.Any(y =>
                x.passiveDetail.PassiveList.Any(z => y.PackageId == z.id.packageId && y.PassiveId == z.id.id) &&
                y.BannedEmotionCardSelection);


        [HarmonyPostfix]
        public static void LevelUpUI_Predicate_Patch(LevelUpUI __instance, BattleUnitModel x, ref bool __result)
        {
            if (x?.Book == null) return;
            if (ModParameters.OnPlayEmotionCardUsedBy != null)
            {
                __result |= x.Book.BookId != ModParameters.OnPlayEmotionCardUsedBy;
                return;
            }

            __result |= MatchAddon(x);
        }

        [HarmonyTargetMethod]
        public static MethodBase LevelUpUI_Predicate_Find()
        {
            var typeInfo = typeof(LevelUpUI).GetTypeInfo().DeclaredNestedTypes
                .FirstOrDefault(x => x.Name.Contains("<>c"));
            ModParameters.MatchInfoEmotionSelection =
                typeInfo?.DeclaredFields.FirstOrDefault(x => x.Name.Contains("<>9__55_0"));
            return typeInfo?.DeclaredMethods.FirstOrDefault(x => x.Name.Contains("OnSelectRoutine"));
        }
    }
}