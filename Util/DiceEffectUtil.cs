using System.IO;
using UnityEngine;

namespace UtilLoader21341.Util
{
    public static class DiceEffectUtil
    {
        public static void InitializeEffect<T>(float? _duration,
            float positionX, float positionY, bool overSelf, BattleUnitView self, BattleUnitView target,
            float destroyTime,
            string path,
            GameObject gameObject, ref BattleUnitModel _self, ref Transform selfTransform,
            ref Transform targetTransform, ref float duration, ref SpriteRenderer spr, Transform transform
        )
        {
            //base.Initialize(self, target, destroyTime);
            _self = self.model;
            selfTransform = self.atkEffectRoot;
            targetTransform = overSelf ? self.atkEffectRoot : target.atkEffectRoot;
            transform.parent = overSelf ? self.charAppearance.transform : target.transform;
            duration = _duration ?? destroyTime;
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(path + "/CustomEffect/" +
                                                  typeof(T).Name.Replace("DiceAttackEffect_", "") + ".png"));
            spr.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                new Vector2(positionX, positionY));
            gameObject.layer = LayerMask.NameToLayer("Effect");
            //ResetLocalTransform(transform);
        }
    }
}