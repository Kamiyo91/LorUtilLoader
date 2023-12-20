using System;
using System.IO;
using Battle.DiceAttackEffect;
using UnityEngine;
using UtilLoader21341.Extensions;
using Object = UnityEngine.Object;

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

        public static void CreateDamagedTextEffectCustom(this AttackEffectManager atkManager, int number,
            BattleUnitModel unit, AtkResist atkResist, Color color, string text = "", BattleUnitModel attacker = null,
            Sprite icon = null, bool useDirection = false, Vector3? scale = null)
        {
            var damageTextEffect = Object.Instantiate(atkManager.damagedTextPrefab, unit.view.damageTextEffectRoot);
            if (damageTextEffect == null) return;
            if (icon != null)
            {
                damageTextEffect.img_resistIcon.sprite = icon;
            }
            else
            {
                damageTextEffect.img_resistIcon.enabled = false;
                damageTextEffect.txt_resist.enabled = false;
            }

            damageTextEffect.img_resistIconBg.enabled = false;
            damageTextEffect.img_resistIconFg.enabled = false;
            damageTextEffect.maxEffect = false;
            damageTextEffect.isAtk = true;
            damageTextEffect.txt_resist.text = text;
            damageTextEffect.txt_resist.color = color;
            var value = color;
            value.a = 0.5f;
            damageTextEffect.txt_resist.fontMaterial.SetColor("_UnderlayColor", value);
            var finalNumber = 0;
            if (number > 0)
            {
                var num = (int)Mathf.Log10(number);
                var num2 = (int)Mathf.Pow(10f, num);
                finalNumber = number / num2;
            }

            var damageNumber = Object.Instantiate(atkManager.damageNumberPrefabs[finalNumber],
                damageTextEffect.damageNumParent);
            damageNumber.SetColor(color, atkResist);
            damageTextEffect.numberList.Add(damageNumber);
            if (!useDirection)
            {
                if (attacker != null && attacker.view.WorldPosition.x > unit.view.WorldPosition.x)
                    damageTextEffect.sign = -2f;
                else damageTextEffect.sign = 2f;
            }
            else if (unit.direction == Direction.RIGHT)
            {
                damageTextEffect.sign = -2f;
            }
            else
            {
                damageTextEffect.sign = 2f;
            }

            var localPosition = damageTextEffect.rotatePivot.localPosition;
            if (scale.HasValue) localPosition.Scale(new Vector3(scale.Value.x, scale.Value.y, scale.Value.z));
            damageTextEffect.rotatePivot.localPosition = localPosition;
            atkManager.SetEffectSizeByCamZoom(damageTextEffect);
            atkManager.SetEffectSizeByUnitHeight(unit, damageTextEffect);
        }
    }
}