using System.Linq;
using UtilLoader21341.Models;

namespace UtilLoader21341.Util
{
    public static class CustomFloorUtil
    {
        public static void ChangeFloor(CustomFloorOptionRoot options, SephirahType floorType, int? keypageId = null,
            int? passiveId = null)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.ContainsKey(floorType)) return;
            ModParameters.EgoAndEmotionCardChanged[floorType] =
                new SavedFloorOptions(true, options, keypageId, passiveId);
            CardUtil.SaveCardsBeforeChange(floorType);
            CardUtil.ChangeAbnoAndEgo(floorType, options);
        }

        public static void ResetFloor(SephirahType floorType)
        {
            if (!ModParameters.EgoAndEmotionCardChanged[floorType].IsActive) return;
            CardUtil.RevertAbnoAndEgo(floorType);
            ModParameters.EgoAndEmotionCardChanged[floorType] = new SavedFloorOptions();
        }

        public static void ResetFloorByPassiveId(string packageId, int passiveId)
        {
            var floorTypes = ModParameters.EgoAndEmotionCardChanged.Where(x =>
                x.Value?.FloorOptions != null && x.Value.PassiveId == passiveId &&
                x.Value.FloorOptions.PackageId == packageId &&
                x.Value.IsActive).Select(x => x.Key);
            foreach (var floorType in floorTypes)
                ResetFloor(floorType);
        }
    }
}