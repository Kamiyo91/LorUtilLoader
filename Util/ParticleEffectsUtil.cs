using System;
using Sound;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UtilLoader21341.Util
{
    public static class ParticleEffectsUtil
    {
        public static void MakeEffect(BattleUnitModel unit, string path, float sizeFactor = 1f,
            BattleUnitModel target = null, float destroyTime = -1f)
        {
            try
            {
                SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(path, sizeFactor, unit.view,
                    target?.view, destroyTime);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void SetParticleIndexRelease(BattleUnitModel owner, ref GameObject aura)
        {
            if (aura != null) return;
            var @object = Resources.Load("Prefabs/Battle/SpecialEffect/IndexRelease_Aura");
            if (@object != null)
            {
                var gameObject = Object.Instantiate(@object) as GameObject;
                if (gameObject != null)
                {
                    gameObject.transform.parent = owner.view.charAppearance.transform;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                    var component = gameObject.GetComponent<IndexReleaseAura>();
                    if (component != null) component.Init(owner.view);
                    aura = gameObject;
                }
            }

            var object2 = Resources.Load("Prefabs/Battle/SpecialEffect/IndexRelease_ActivateParticle");
            if (object2 != null)
            {
                var gameObject2 = Object.Instantiate(object2) as GameObject;
                if (gameObject2 != null)
                {
                    gameObject2.transform.parent = owner.view.charAppearance.transform;
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localRotation = Quaternion.identity;
                    gameObject2.transform.localScale = Vector3.one;
                }
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Buf/Effect_Index_Unlock");
        }

        public static void BlueWolfAura(BattleUnitModel owner)
        {
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            MakeEffect(owner, "6/BigBadWolf_Emotion_Aura", 1f, owner);
        }

        public static void BaseGameLoadPrefabEffect(BattleUnitModel unit, string prefabPath, string playSoundPath)
        {
            var gameObject = global::Util.LoadPrefab(prefabPath);
            if (gameObject != null)
                if (unit?.view != null)
                {
                    gameObject.transform.parent = unit.view.camRotationFollower;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localScale = Vector3.one;
                    gameObject.transform.localRotation = Quaternion.identity;
                }

            SoundEffectPlayer.PlaySound(playSoundPath);
        }

        public static void IndexReleaseBreakEffect(BattleUnitModel unit)
        {
            var object2 = Resources.Load("Prefabs/Battle/SpecialEffect/IndexRelease_ActivateParticle");
            if (object2 != null)
            {
                var gameObject2 = Object.Instantiate(object2) as GameObject;
                if (gameObject2 != null)
                {
                    gameObject2.transform.parent = unit.view.charAppearance.transform;
                    gameObject2.transform.localPosition = Vector3.zero;
                    gameObject2.transform.localRotation = Quaternion.identity;
                    gameObject2.transform.localScale = Vector3.one;
                }
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Buf/Effect_Index_Unlock");
        }
    }
}