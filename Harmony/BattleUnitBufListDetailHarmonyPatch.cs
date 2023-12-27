using System.Linq;
using HarmonyLib;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class BattleUnitBufListDetailHarmonyPatch
    {
        [HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.AddKeywordBufByCard))]
        [HarmonyPrefix]
        public static void AddKeywordBufByCard(BattleUnitBufListDetail __instance, ref KeywordBuf bufType, int stack,
            BattleUnitModel actor)
        {
            var keywords = BuffUtil.CanAddBuffCustom(__instance, ref bufType);
            if (!keywords.Any()) return;
            foreach (var keyword in keywords) __instance.AddKeywordBufByCard(keyword, stack, actor);
        }

        [HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.AddKeywordBufByEtc))]
        [HarmonyPrefix]
        public static void AddKeywordBufByEtc(BattleUnitBufListDetail __instance, ref KeywordBuf bufType, int stack,
            BattleUnitModel actor)
        {
            var keywords = BuffUtil.CanAddBuffCustom(__instance, ref bufType);
            if (!keywords.Any()) return;
            foreach (var keyword in keywords) __instance.AddKeywordBufByEtc(keyword, stack, actor);
        }

        [HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.AddKeywordBufThisRoundByEtc))]
        [HarmonyPrefix]
        public static void AddKeywordBufThisRoundByEtc(BattleUnitBufListDetail __instance, ref KeywordBuf bufType,
            int stack, BattleUnitModel actor)
        {
            var keywords = BuffUtil.CanAddBuffCustom(__instance, ref bufType);
            if (!keywords.Any()) return;
            foreach (var keyword in keywords) __instance.AddKeywordBufThisRoundByEtc(keyword, stack, actor);
        }

        [HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.AddKeywordBufThisRoundByCard))]
        [HarmonyPrefix]
        public static void AddKeywordBufThisRoundByCard(BattleUnitBufListDetail __instance, ref KeywordBuf bufType,
            int stack, BattleUnitModel actor)
        {
            var keywords = BuffUtil.CanAddBuffCustom(__instance, ref bufType);
            if (!keywords.Any()) return;
            foreach (var keyword in keywords) __instance.AddKeywordBufThisRoundByCard(keyword, stack, actor);
        }

        [HarmonyPatch(typeof(BattleUnitBufListDetail), nameof(BattleUnitBufListDetail.AddKeywordBufNextNextByCard))]
        [HarmonyPrefix]
        public static void AddKeywordBufNextNextByCard(BattleUnitBufListDetail __instance, ref KeywordBuf bufType,
            int stack, BattleUnitModel actor)
        {
            var keywords = BuffUtil.CanAddBuffCustom(__instance, ref bufType);
            if (!keywords.Any()) return;
            foreach (var keyword in keywords) __instance.AddKeywordBufNextNextByCard(keyword, stack, actor);
        }
    }
}