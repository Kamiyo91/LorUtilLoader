using UtilLoader21341.Interface;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_SwitchBuffs_DLL21341 : PassiveAbilityBase, ISwitchBuff
    {
        private bool _active;
        private bool _addOneExtra;
        private KeywordBuf _buffToAdd;
        private KeywordBuf _buffToRemove;

        public KeywordBuf SwitchBuff(KeywordBuf bufType)
        {
            if (_active && _buffToRemove == bufType) return _buffToAdd;
            return bufType;
        }

        public override void OnRoundStartAfter()
        {
            ConvertBuff();
        }

        public void SetActive(bool value)
        {
            _active = value;
        }

        public void SetKeywords(KeywordBuf buffToAdd, KeywordBuf buffToRemove)
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
            var buffStacksThisScene = owner.bufListDetail.GetKewordBufStack(_buffToRemove);
            var buffNextScene = owner.bufListDetail._readyBufList.Find(x => x.bufType == _buffToRemove);
            if (buffNextScene != null)
            {
                owner.bufListDetail.AddKeywordBufByEtc(_buffToAdd, buffNextScene.stack, owner);
                owner.bufListDetail.GetReadyBufList().RemoveAll(x => x.bufType == _buffToRemove);
            }

            if (buffStacksThisScene != 0)
            {
                owner.bufListDetail.AddKeywordBufThisRoundByEtc(_buffToAdd, buffStacksThisScene, owner);
                owner.bufListDetail.GetActivatedBufList().RemoveAll(x => x.bufType == _buffToRemove);
            }

            if (_addOneExtra) owner.bufListDetail.AddKeywordBufThisRoundByEtc(_buffToAdd, 1, owner);
        }
    }
}