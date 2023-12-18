using System.Collections.Generic;
using CustomMapUtility;
using Sound;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.Extensions
{
    public class FarAreaEffect_MassAttackBase_DLL21341 : FarAreaEffect
    {
        private string _attackEffect;
        private float _attackEffectScale;

        private ActionDetail _attackMotion;
        private string _audioFileName;

        private ActionDetail _beforeMotion;

        private CameraFilterPack_FX_EarthQuake _camFilter;
        private bool _characterMove;
        private float _elapsed;
        private bool _followUnits;
        private bool _isBaseGameAudio;
        private string _packageId;
        private bool _slowMotion;
        private bool _zoom;

        public void SetParameters(FarAreaMassAttackEffectParameters p)
        {
            _attackMotion = p.AttackMotion;
            _audioFileName = p.AudioFileName;
            _attackEffect = p.AttackEffect;
            _attackEffectScale = p.AttackEffectScale;
            _isBaseGameAudio = p.IsBaseGameAudio;
            _slowMotion = p.SlowMotion;
            _zoom = p.Zoom;
            _characterMove = p.CharacterMove;
            _followUnits = p.FollowUnits;
            _packageId = p.PackageId;
        }

        public override void Init(BattleUnitModel self, params object[] args)
        {
            base.Init(self, args);
            if (_characterMove) self.moveDetail.Move(Vector3.zero, 200f);
            OnEffectStart();
            _elapsed = 0f;
            Singleton<BattleFarAreaPlayManager>.Instance.SetActionDelay(0f);
            var list = new List<BattleUnitModel> { self };
            list.AddRange(
                BattleObjectManager.instance.GetAliveList(
                    self.faction == Faction.Enemy ? Faction.Player : Faction.Enemy));
            if (_followUnits) SingletonBehavior<BattleCamManager>.Instance.FollowUnits(false, list);
            _beforeMotion = ActionDetail.Default;
        }

        public override void Update()
        {
            switch (state)
            {
                case EffectState.Start:
                {
                    if (_self.moveDetail.isArrived) state = EffectState.GiveDamage;
                    break;
                }
                case EffectState.GiveDamage:
                {
                    _elapsed += Time.deltaTime;
                    if (_elapsed >= 0.25f)
                    {
                        _beforeMotion = _self.view.charAppearance.GetCurrentMotionDetail();
                        _self.view.charAppearance.ChangeMotion(_attackMotion);
                        _elapsed = 0f;
                        isRunning = false;
                        state = EffectState.End;
                        if (_zoom)
                        {
                            var instance = SingletonBehavior<BattleCamManager>.Instance;
                            var camera = instance != null ? instance.EffectCam : null;
                            if (camera != null)
                                _camFilter = camera.gameObject.AddComponent<CameraFilterPack_FX_EarthQuake>();
                        }

                        if (_slowMotion) TimeManager.Instance.SlowMotion(0.25f, 0.125f, true);
                        var audioClip = UnitUtil.GetSound(CustomMapHandler.GetCMU(_packageId), _audioFileName,
                            _isBaseGameAudio);
                        SingletonBehavior<SoundEffectManager>.Instance.PlayClip(audioClip);
                        SingletonBehavior<DiceEffectManager>.Instance.CreateBehaviourEffect(_attackEffect,
                            _attackEffectScale, _self.view, null);
                    }

                    break;
                }
                case EffectState.End:
                {
                    _elapsed += Time.deltaTime;
                    if (_camFilter != null && _zoom)
                    {
                        _camFilter.Speed = 30f * (1f - _elapsed);
                        _camFilter.X = 0.1f * (1f - _elapsed);
                        _camFilter.Y = 0.1f * (1f - _elapsed);
                    }

                    if (_elapsed > 1f)
                    {
                        if (_camFilter != null)
                        {
                            Destroy(_camFilter);
                            _camFilter = null;
                        }

                        _self.view.charAppearance.ChangeMotion(_beforeMotion);
                        state = EffectState.None;
                        _elapsed = 0f;
                    }

                    break;
                }
                case EffectState.None:
                {
                    if (_followUnits)
                        SingletonBehavior<BattleCamManager>.Instance.FollowUnits(false,
                            BattleObjectManager.instance.GetAliveList());
                    if (_self.view.FormationReturned) Destroy(gameObject);
                    break;
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (_camFilter == null) return;
            Destroy(_camFilter);
            _camFilter = null;
        }
    }
}