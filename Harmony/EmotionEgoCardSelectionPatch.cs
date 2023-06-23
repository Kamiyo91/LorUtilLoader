using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class EmotionEgoCardSelectionPatch
    {
        [HarmonyPatch(typeof(StageLibraryFloorModel), "RandomSelectEgo")]
        [HarmonyPostfix]
        public static void StageLibraryFloorModel_RandomSelectEgo(List<EmotionEgoXmlInfo> duplicated,
            ref EmotionEgoXmlInfo __result)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(Singleton<StageController>.Instance.CurrentFloor,
                    out _)) return;
            var cardList = Singleton<EmotionEgoXmlList>.Instance
                .GetDataList(Singleton<StageController>.Instance.CurrentFloor).Where(x => !x.isLock).ToList();
            cardList.RemoveAll(x => duplicated.Exists(y => x.CardId == y.CardId && x.Sephirah == y.Sephirah));
            __result = cardList.Any() ? RandomUtil.SelectOne(cardList) : null;
        }
    }
}