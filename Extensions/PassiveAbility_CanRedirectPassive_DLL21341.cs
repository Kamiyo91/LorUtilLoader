using System.Linq;
using UtilLoader21341.Interface;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_CanRedirectPassive_DLL21341 : PassiveAbilityBase, ICanRedirect
    {
        private string _buffKeyword = string.Empty;

        public bool CanRedirectAggro(BattleUnitModel target, BattleUnitModel enemyCardTarget)
        {
            if (target == null || enemyCardTarget == null) return false;
            return enemyCardTarget.bufListDetail.GetActivatedBufList().Any(x => x.keywordId == _buffKeyword);
        }

        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }

        public void SetKeyword(string keyword)
        {
            _buffKeyword = keyword;
        }
    }
}