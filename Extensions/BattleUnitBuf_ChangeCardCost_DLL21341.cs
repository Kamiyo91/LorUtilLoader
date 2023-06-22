using UtilLoader21341.Util;

namespace UtilLoader21341.Extensions
{
    public class BattleUnitBuf_ChangeCardCost_DLL21341 : BattleUnitBuf_BaseBufChanged_DLL21341
    {
        public int AdditionalDraw;
        public int Cost;

        public BattleUnitBuf_ChangeCardCost_DLL21341(int cost = -999, int additionalDraw = 6,
            ActionDetail actionDetail = ActionDetail.NONE, bool infinite = true, bool lastOneScene = false,
            int lastForXScenes = 0) : base(actionDetail, infinite, lastOneScene,
            lastForXScenes)
        {
            Cost = cost;
            AdditionalDraw = additionalDraw;
        }

        public override void OnRoundStartAfter()
        {
            _owner.DrawUntilX(AdditionalDraw);
        }

        public override int GetCardCostAdder(BattleDiceCardModel card)
        {
            return Cost;
        }
    }
}