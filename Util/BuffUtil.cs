using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLoader21341.Interface;

namespace UtilLoader21341.Util
{
    public static class BuffUtil
    {
        public static void OnRoundEnd(this BattleUnitBuf buff, ref int sceneCount, int adderStackEachScene,
            bool infinite = false, int lastForXScenes = 0, bool lastOneScene = false, bool motionChanged = false,
            int minStack = 0, int maxStack = 25, bool destroyedAt0Stack = false)
        {
            if (adderStackEachScene != 0)
                buff.OnAddBufCustom(adderStackEachScene, destroyedAt0Stack, minStack, maxStack);
            if (infinite) return;
            if (lastForXScenes > 0)
            {
                if (lastForXScenes == sceneCount)
                {
                    if (motionChanged) buff._owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
                    buff._owner.bufListDetail.RemoveBuf(buff);
                    return;
                }

                sceneCount++;
            }

            if (!lastOneScene) return;
            if (motionChanged) buff._owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
        }

        public static void OnAddBufCustom(this BattleUnitBuf buff, int addedStack, bool destroyedAt0Stack = false,
            int minStack = 0, int maxStack = 25)
        {
            buff.stack += addedStack;
            buff.stack = Mathf.Clamp(buff.stack, minStack, maxStack);
            if (destroyedAt0Stack && buff.stack == 0) buff._owner.bufListDetail.RemoveBuf(buff);
        }

        public static void Init(BattleUnitModel owner, ActionDetail actionDetail)
        {
            if (actionDetail == ActionDetail.NONE) return;
            owner.view.charAppearance.ChangeMotion(actionDetail);
        }

        public static void OnRollSpeedDiceLock(this BattleUnitBuf buff, ref int breakedDice)
        {
            breakedDice = buff._owner.view.speedDiceSetterUI.SpeedDicesCount;
            for (var i = 0; i < breakedDice; i++)
            {
                buff._owner.speedDiceResult[i].value = 0;
                buff._owner.speedDiceResult[i].breaked = true;
                buff._owner.view.speedDiceSetterUI.GetSpeedDiceByIndex(i).BreakDice(true, true);
            }
        }

        public static List<KeywordBuf> CanAddBuffCustom(BattleUnitBufListDetail instance, ref KeywordBuf keyword)
        {
            var keywords = new List<KeywordBuf>();
            foreach (var passive in instance._self.passiveDetail._passiveList.OfType<ISwitchBuff>())
                keywords.Add(passive.SwitchBuff(keyword));
            if (!keywords.Any()) return keywords;
            keyword = keywords[0];
            keywords.RemoveAt(0);
            return keywords;
        }
    }
}