using System;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.GameObjects
{
    public class ChangeDiceOrderGameObject : MonoBehaviour
    {
        private readonly StartBattleEffect effect = new StartBattleEffect();
        public Type DieAbilityType;
        public bool IsFirst;
        public BattleUnitModel Owner;

        public void SetParameters(BattleUnitModel owner, Type dieAbilityType, bool isFirst)
        {
            Owner = owner;
            DieAbilityType = dieAbilityType;
            IsFirst = isFirst;
        }

        public void Init()
        {
            effect.isDone = false;
            Singleton<StageController>.Instance.RegisterStartBattleEffect(effect);
        }

        private void FixedUpdate()
        {
            if (Owner == null || DieAbilityType == null) return;
            if (Singleton<StageController>.Instance.Phase != StageController.StagePhase.WaitStartBattleEffect) return;
            if (IsFirst) CardUtil.PutCounterDieAsFirst(Owner, DieAbilityType);
            else CardUtil.PutCounterDieAsLast(Owner, DieAbilityType);
            effect.isDone = true;
            Destroy(gameObject);
        }
    }
}