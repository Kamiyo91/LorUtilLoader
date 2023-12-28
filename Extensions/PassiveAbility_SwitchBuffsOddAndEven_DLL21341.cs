using System.Collections.Generic;
using UtilLoader21341.Interface;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_SwitchBuffsOddAndEven_DLL21341 : PassiveAbilityBase, ISwitchBuff
    {
        private readonly StageController _stageController = StageController.Instance;
        private bool _active;
        private bool _addOneExtra;
        private List<KeywordBuf> _keywords = new List<KeywordBuf>();
        public bool IsOddOrEven;

        public bool SwitchBuff(KeywordBuf bufType, out KeywordBuf outKeywordBuf)
        {
            outKeywordBuf = KeywordBuf.None;
            if (!_active) return false;
            switch (IsOddOrEven)
            {
                case true when _keywords[0] == bufType:
                    outKeywordBuf = _keywords[1];
                    return true;
                case false when _keywords[1] == bufType:
                    outKeywordBuf = _keywords[0];
                    return true;
                default:
                    return false;
            }
        }

        public override void OnRoundStartAfter()
        {
            IsOddOrEven = _stageController.RoundTurn % 2 == 0;
            ConvertBuff();
        }

        public void SetActive(bool value)
        {
            _active = value;
        }

        public void SetKeywords(List<KeywordBuf> value)
        {
            _keywords = value;
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
            var keywordBuffRemove = IsOddOrEven ? _keywords[0] : _keywords[1];
            var keywordBuffAdd = IsOddOrEven ? _keywords[1] : _keywords[0];
            var buffStacksThisScene = owner.bufListDetail.GetKewordBufStack(keywordBuffRemove);
            var buffNextScene = owner.bufListDetail._readyBufList.Find(x => x.bufType == keywordBuffRemove);
            if (buffNextScene != null)
            {
                owner.bufListDetail.AddKeywordBufByEtc(keywordBuffAdd, buffNextScene.stack, owner);
                owner.bufListDetail.GetReadyBufList().RemoveAll(x => x.bufType == keywordBuffRemove);
            }

            if (buffStacksThisScene != 0)
            {
                owner.bufListDetail.AddKeywordBufThisRoundByEtc(keywordBuffAdd, buffStacksThisScene, owner);
                owner.bufListDetail.GetActivatedBufList().RemoveAll(x => x.bufType == keywordBuffRemove);
            }

            if (_addOneExtra) owner.bufListDetail.AddKeywordBufThisRoundByEtc(keywordBuffAdd, 1, owner);
        }
    }
}