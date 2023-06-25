using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BattleUnitInformationUI;
using static UnityEngine.UI.GridLayoutGroup;
using System.Xml.Linq;
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
