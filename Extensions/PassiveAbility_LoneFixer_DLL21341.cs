using UtilLoader21341.Util;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_LoneFixer_DLL21341 : PassiveAbilityBase
    {
        public override void OnCreated()
        {
            rare = Rarity.Uncommon;
            name = Singleton<PassiveDescXmlList>.Instance.GetName(230008);
            desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(230008);
        }

        public override void OnRoundEnd()
        {
            if (UnitUtil.SupportCharCheck(owner) < 2)
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Strength, 3);
        }
    }
}