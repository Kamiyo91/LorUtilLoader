using System.Collections.Generic;
using System.Linq;
using UtilLoader21341.Interface;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_SwitchBuffs_DLL21341 : PassiveAbilityBase, ISwitchBuff
    {
        private bool _active;
        private bool _addOneExtra;
        private KeywordBuf _buffToAdd;
        private List<KeywordBuf> _buffToRemove;

        public bool SwitchBuff(KeywordBuf bufType, out KeywordBuf outKeywordBuf)
        {
            outKeywordBuf = KeywordBuf.None;
            if (!_active || !_buffToRemove.Contains(bufType)) return false;
            outKeywordBuf = _buffToAdd;
            return true;
        }

        public override void OnRoundStartAfter()
        {
            ConvertBuff();
        }

        public void SetActive(bool value)
        {
            _active = value;
        }

        public void SetKeywords(KeywordBuf buffToAdd, List<KeywordBuf> buffToRemove)
        {
            _buffToAdd = buffToAdd;
            _buffToRemove = buffToRemove;
        }

        public void SetAddOneExtra(bool value)
        {
            _addOneExtra = value;
        }

        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }

        public void ConvertBuff()
        {
            var stackThisScene = _buffToRemove.Sum(buff => owner.bufListDetail.GetKewordBufStack(buff));
            var buffsNextScene = _buffToRemove
                .Select(buff => owner.bufListDetail._readyBufList.Find(x => x.bufType == buff)).ToList();
            if (buffsNextScene.Any())
            {
                var buffStackNextScene = buffsNextScene.Sum(buff => buff.stack);
                if (buffStackNextScene > 0)
                {
                    owner.bufListDetail.AddKeywordBufByEtc(_buffToAdd, buffStackNextScene, owner);
                    owner.bufListDetail.GetReadyBufList().RemoveAll(x => _buffToRemove.Contains(x.bufType));
                }
            }

            if (stackThisScene != 0)
            {
                owner.bufListDetail.AddKeywordBufThisRoundByEtc(_buffToAdd, stackThisScene, owner);
                owner.bufListDetail.GetActivatedBufList().RemoveAll(x => _buffToRemove.Contains(x.bufType));
            }

            if (_addOneExtra) owner.bufListDetail.AddKeywordBufThisRoundByEtc(_buffToAdd, 1, owner);
        }
    }
}