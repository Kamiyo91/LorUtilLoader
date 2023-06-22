using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.GameObjects
{
    public class SephirahSelectableEmotionCardsGameObject : MonoBehaviour
    {
        public bool Active;
        public int EmotionLevel;
        public List<SephirahType> SephirahTypes = new List<SephirahType>();

        public void Init()
        {
            DontDestroyOnLoad(this);
        }

        private void FixedUpdate()
        {
            if (!Active) return;
            if (!SephirahTypes.Any())
            {
                Active = false;
                gameObject.SetActive(false);
                return;
            }

            var sephirah = SephirahTypes.FirstOrDefault();
            if (OpenEmotionSelectionTab(sephirah)) ChangeSephirahTypeValues(false, sephirah);
        }

        public bool OpenEmotionSelectionTab(SephirahType sephirah)
        {
            if (SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.IsEnabled) return false;
            if (BattleObjectManager.instance.GetAliveList(Faction.Player)
                .All(x => x.UnitData.unitData.OwnerSephirah != sephirah)) return true;
            var emotionList = CardUtil.CreateSephirahSelectableList(EmotionLevel, sephirah);
            ModParameters.OnPlayCardEmotion = true;
            if (emotionList.Count <= 0) return true;
            if (!SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.IsEnabled)
                SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.SetRootCanvas(true);
            SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.Init(Mathf.Clamp(EmotionLevel - 1, 0, 4),
                emotionList);
            return true;
        }

        public void ChangeSephirahTypeValues(bool addOrRemove, SephirahType type)
        {
            if (addOrRemove) SephirahTypes.Add(type);
            else SephirahTypes.Remove(type);
        }

        public void SetEmotionLevel(int value)
        {
            EmotionLevel = value;
        }

        public void ActiveEmotion()
        {
            Active = true;
        }
    }
}