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
                buff.AddBufCustom(adderStackEachScene, destroyedAt0Stack, minStack, maxStack);
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

        public static HashSet<KeywordBuf> CanAddBuffCustom(BattleUnitBufListDetail instance, ref KeywordBuf keyword)
        {
            var keywords = new HashSet<KeywordBuf>();
            foreach (var passive in instance._self.passiveDetail._passiveList.Where(x => x.isActiavted)
                         .OfType<ISwitchBuff>())
                if (passive.SwitchBuff(keyword, out var changedKeyword))
                    keywords.Add(changedKeyword);
            if (!keywords.Any()) return keywords;
            keyword = keywords.FirstOrDefault();
            keywords.Remove(keyword);
            return keywords;
        }

        public static BattleUnitBuf AddBuffCustom<T>(this BattleUnitModel owner, int stack,
            bool destroyat0Stack = false,
            int minStack = 0, int maxStack = 25)
            where T : BattleUnitBuf, new()
        {
            var buff = owner.GetActiveBuff<T>();
            if (buff == null)
            {
                buff = new T { stack = 0 };
                owner.bufListDetail.AddBuf(buff);
                buff.stack = 0;
            }

            buff.AddBufCustom(stack, destroyat0Stack, minStack, maxStack);
            return buff;
        }

        public static BattleUnitBuf AddBuffCustom<T>(this T buff, BattleUnitModel owner, int stack,
            bool destroyat0Stack = false,
            int minStack = 0, int maxStack = 25)
            where T : BattleUnitBuf, new()
        {
            if (owner.GetActiveBuff<T>() == null)
            {
                buff = new T { stack = 0 };
                owner.bufListDetail.AddBuf(buff);
            }

            buff.AddBufCustom(stack, destroyat0Stack, minStack, maxStack);
            return buff;
        }

        public static T CheckPermanentBuff<T>(this BattleUnitModel owner, bool active = true, int startStacks = 0)
            where T : BattleUnitBuf, new()
        {
            if (!active) return null;
            if (owner.bufListDetail.HasBuf<T>()) return owner.GetActiveBuff<T>();
            return (T)owner.AddBuffCustom<T>(startStacks);
        }

        public static void AddBufCustom(this BattleUnitBuf buff, int addedStack, bool destroyedAt0Stack = false,
            int minStack = 0, int maxStack = 25)
        {
            buff.stack += addedStack;
            buff.stack = Mathf.Clamp(buff.stack, minStack, maxStack);
            if (destroyedAt0Stack && buff.stack == 0) buff._owner.bufListDetail.RemoveBuf(buff);
        }
    }
}