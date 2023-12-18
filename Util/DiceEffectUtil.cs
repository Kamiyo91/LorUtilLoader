using System;
using System.IO;
using Battle.DiceAttackEffect;
using UnityEngine;
using UtilLoader21341.Extensions;

namespace UtilLoader21341.Util
{
    public static class DiceEffectUtil
    {
        public static void InitializeEffect<T>(string path, float positionX, float positionY, bool overSelf, T ef,
            BattleUnitView self, BattleUnitView target) where T : DiceAttackEffect
        {
            ef._self = self.model;
            ef._selfTransform = self.atkEffectRoot;
            ef._targetTransform = overSelf ? self.atkEffectRoot : target.atkEffectRoot;
            ef.transform.parent = overSelf ? self.charAppearance.transform : target.transform;
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(path + "/CustomEffect/" +
                                                  typeof(T).Name.Replace("DiceAttackEffect_", "") + ".png"));
            ef.spr.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                new Vector2(positionX, positionY));
            ef.gameObject.layer = LayerMask.NameToLayer("Effect");
            ef.ResetLocalTransform(ef.transform);
        }

        public static float CalculateScale(bool dynamicScale, float scaleFactor, float scale)
        {
            return dynamicScale ? scaleFactor * scale : scale;
        }

        public static void InitializeMassEffect<T>(this T ef, string path, float positionX, float positionY,
            float offsetX,
            float offsetY, float destroyTime,
            BattleUnitView self, float? duration = null) where T : DiceAttackEffect
        {
            ef._self = self.model;
            var atkEffectRoot = self.atkEffectRoot;
            if (self.charAppearance != null)
                atkEffectRoot = self.charAppearance.atkEffectRoot;
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(path + "/CustomEffect/" +
                                                  typeof(T).Name.Replace("DiceAttackEffect_", "") + ".png"));
            ef.spr.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                new Vector2(positionX, positionY));
            ef.gameObject.layer = LayerMask.NameToLayer("Effect");
            ef.ResetLocalTransform(ef.transform);
            ef.transform.parent = atkEffectRoot;
            ef.transform.localScale = Vector3.one + ef.additionalScale;
            ef.transform.localPosition = Vector3.zero + new Vector3(offsetX, offsetY, 0f);
            ef.transform.localRotation = Quaternion.identity;
            ef._destroyTime = duration ?? destroyTime;
            ef.animator.speed = 1f / ef._destroyTime;
            ef._elapsed = 0f;
        }

        public static void InitializeEffect<T>(this T ef, string path, float positionX, float positionY, bool overSelf,
            BattleUnitView self, BattleUnitView target) where T : DiceAttackEffect
        {
            ef._self = self.model;
            ef._selfTransform = self.atkEffectRoot;
            ef._targetTransform = overSelf ? self.atkEffectRoot : target.atkEffectRoot;
            ef.transform.parent = overSelf ? self.charAppearance.transform : target.transform;
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(path + "/CustomEffect/" +
                                                  typeof(T).Name.Replace("DiceAttackEffect_", "") + ".png"));
            ef.spr.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                new Vector2(positionX, positionY));
            ef.gameObject.layer = LayerMask.NameToLayer("Effect");
            ef.ResetLocalTransform(ef.transform);
        }

        public static FarAreaEffect SetFarAreaAtkEffect<T>(this T ba, FarAreaMassAttackEffectParameters parameters,
            BattleUnitModel self) where T : BehaviourActionBase
        {
            ba._self = self;
            var massAttackEffect =
                new GameObject().AddComponent(typeof(FarAreaEffect_MassAttackBase_DLL21341)) as
                    FarAreaEffect_MassAttackBase_DLL21341;
            massAttackEffect?.SetParameters(parameters);
            massAttackEffect?.Init(self, Array.Empty<object>());
            return massAttackEffect;
        }
    }
}