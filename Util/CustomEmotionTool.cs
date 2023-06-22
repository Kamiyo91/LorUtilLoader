using System.Collections.Generic;
using UnityEngine;
using UtilLoader21341.GameObjects;

namespace UtilLoader21341.Util
{
    public static class CustomEmotionTool
    {
        public static SelectableEmotionCardsGameObject Script;

        static CustomEmotionTool()
        {
            var gameobj = new GameObject("CustomEmotionSelectionScreen_DLL21341");
            Script = gameobj.AddComponent<SelectableEmotionCardsGameObject>();
            Object.Instantiate(gameobj);
            Script.Init();
            gameobj.SetActive(false);
        }

        public static void SetParameters(CustomEmotionParameters parameters)
        {
            Script.gameObject.SetActive(true);
            Script.ChangeParametersValues(true, parameters);
            Script.ActiveEmotion();
        }
    }

    public class CustomEmotionParameters
    {
        public List<EmotionCardXmlInfo> EmotionCards { get; set; }
        public int EmotionLevel { get; set; }
        public LorId BookId { get; set; }
        public bool IsOnlyForUser { get; set; }
    }
}