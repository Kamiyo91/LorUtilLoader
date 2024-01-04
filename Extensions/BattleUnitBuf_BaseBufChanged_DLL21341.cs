using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.Extensions
{
    public class BattleUnitBuf_BaseBufChanged_DLL21341 : BattleUnitBuf
    {
        private readonly ActionDetail _actionDetail;
        private readonly bool _infinite;
        private readonly int _lastForXScenes;
        public readonly bool LastOneScene;
        private bool _motionChanged;
        private int _sceneCount;
        public GameObject Aura;

        public BattleUnitBuf_BaseBufChanged_DLL21341(ActionDetail actionDetail = ActionDetail.NONE,
            bool infinite = false, bool lastOneScene = true, int lastForXScenes = 0)
        {
            _actionDetail = actionDetail;
            _infinite = infinite;
            LastOneScene = lastOneScene;
            _lastForXScenes = lastForXScenes;
            _sceneCount = 0;
            _motionChanged = false;
        }

        public virtual string BufName { get; set; }
        public virtual int MinStack => 0;
        public virtual int MaxStack => 25;
        public virtual int AdderStackEachScene => 0;
        public virtual bool DestroyedAt0Stack => false;

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            if (_actionDetail == ActionDetail.NONE) return;
            _motionChanged = true;
            owner.view.charAppearance.ChangeMotion(_actionDetail);
        }

        public override void OnRoundEnd()
        {
            if (AdderStackEachScene != 0) this.AddBufCustom(AdderStackEachScene);
            if (_infinite) return;
            if (_lastForXScenes > 0)
            {
                if (_lastForXScenes == _sceneCount)
                {
                    if (_motionChanged) _owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
                    RemoveBuff();
                    return;
                }

                _sceneCount++;
            }

            if (!LastOneScene) return;
            if (_motionChanged) _owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            RemoveBuff();
        }

        private void RemoveBuff()
        {
            if (Aura != null) Object.Destroy(Aura);
            _owner.bufListDetail.RemoveBuf(this);
        }
    }
}