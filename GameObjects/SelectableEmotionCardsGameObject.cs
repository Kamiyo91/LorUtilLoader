using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLoader21341.Util;

namespace UtilLoader21341.GameObjects
{
    public class SelectableEmotionCardsGameObject : MonoBehaviour
    {
        public bool Active;
        public List<CustomEmotionParameters> PoolParameters = new List<CustomEmotionParameters>();

        public void Init()
        {
            DontDestroyOnLoad(this);
        }

        private void FixedUpdate()
        {
            if (!Active) return;
            if (!PoolParameters.Any())
            {
                Active = false;
                gameObject.SetActive(false);
                return;
            }

            var parameters = PoolParameters.FirstOrDefault();
            if (OpenEmotionSelectionTab(parameters)) ChangeParametersValues(false, parameters);
        }

        public bool OpenEmotionSelectionTab(CustomEmotionParameters parameters)
        {
            if (SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.IsEnabled) return false;
            var emotionList = CardUtil.CustomCreateSelectableList(parameters.EmotionLevel, parameters.EmotionCards);
            ModParameters.OnPlayCardEmotion = true;
            if (emotionList.Count <= 0) return true;
            if (!SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.IsEnabled)
                SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.SetRootCanvas(true);
            if (parameters.IsOnlyForUser) ModParameters.OnPlayEmotionCardUsedBy = parameters.BookId;
            SingletonBehavior<BattleManagerUI>.Instance.ui_levelup.Init(
                Mathf.Clamp(parameters.EmotionLevel - 1, 0, 4), emotionList);
            return true;
        }

        public void ChangeParametersValues(bool addOrRemove, CustomEmotionParameters parameters)
        {
            if (addOrRemove) PoolParameters.Add(parameters);
            else PoolParameters.Remove(parameters);
        }

        public void ActiveEmotion()
        {
            Active = true;
        }
    }
}