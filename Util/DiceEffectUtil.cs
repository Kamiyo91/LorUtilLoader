using System.IO;
using Battle.DiceAttackEffect;
using UnityEngine;

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
    }
}