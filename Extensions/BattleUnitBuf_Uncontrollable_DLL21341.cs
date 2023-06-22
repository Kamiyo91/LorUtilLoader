namespace UtilLoader21341.Extensions
{
    public class BattleUnitBuf_Uncontrollable_DLL21341 : BattleUnitBuf_BaseBufChanged_DLL21341
    {
        public BattleUnitBuf_Uncontrollable_DLL21341(ActionDetail actionDetail = ActionDetail.NONE,
            bool infinite = true, bool lastOneScene = false, int lastForXScenes = 0) : base(actionDetail, infinite,
            lastOneScene, lastForXScenes)
        {
        }

        public override bool IsControllable => false;
    }
}