using LOR_DiceSystem;

namespace UtilLoader21341.Extensions
{
    public class BattleUnitBuf_ResistChangeOneHitOnly_DLL21341 : BattleUnitBuf
    {
        private BehaviourDetail _behaviourDetail;
        private AtkResist _resist;

        public BattleUnitBuf_ResistChangeOneHitOnly_DLL21341()
        {
            stack = 0;
        }

        public override int paramInBufDesc => 0;

        public override AtkResist GetResistHP(AtkResist origin, BehaviourDetail detail)
        {
            if (detail == _behaviourDetail) return origin < _resist ? base.GetResistBP(origin, detail) : _resist;
            return base.GetResistHP(origin, detail);
        }

        public override AtkResist GetResistBP(AtkResist origin, BehaviourDetail detail)
        {
            if (detail == _behaviourDetail) return origin < _resist ? base.GetResistBP(origin, detail) : _resist;
            return base.GetResistBP(origin, detail);
        }

        public void SetResists(AtkResist resist, BehaviourDetail detail)
        {
            _resist = resist;
            _behaviourDetail = detail;
        }
    }
}