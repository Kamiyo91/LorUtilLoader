using UnityEngine;
using UnityEngine.SceneManagement;
using UtilLoader21341.Harmony;
using UtilLoader21341.Util;

namespace UtilLoader21341
{
    public class UtilLoaderManager : MonoBehaviour
    {
        private static UtilLoaderManager _instance;
        private string _language = string.Empty;

        public static void Init()
        {
            if (_instance != null) return;
            InitGameObject();
            GenericUtil.OtherModCheck();
            PassiveUtil.ChangeLoneFixerPassive();
            CardUtil.FillDictionary();
            UtilModLoader.LoadMods();
            Patch();
            LocalizationUtil.RemoveError();
            SceneManager.sceneLoaded += GenericUtil.OnLoadingScreen;
        }

        private static void InitGameObject()
        {
            var gameObject = new GameObject("LoR.UtilLoaderManager21341");
            DontDestroyOnLoad(gameObject);
            _instance = gameObject.AddComponent<UtilLoaderManager>();
        }

        private static void Patch()
        {
            ModParameters.Harmony.CreateClassProcessor(typeof(CategoryHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(GeneralHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(HotfixTranspilers)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(KeypageHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(PassiveHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(BattleUnitBufListDetailHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(SkinHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(StageHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(UpdateEmotionCoinPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(BlockUiRepeat)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(BattleRushHarmonyPatch)).Patch();
            if (!ModParameters.BaseModFound) ModParameters.Harmony.CreateClassProcessor(typeof(UnitLimitPatch)).Patch();
            if (!ModParameters.ColorCardCardUtilLoaderFound)
                ModParameters.Harmony.CreateClassProcessor(typeof(SkinProjectionPatch)).Patch();
            if (ModParameters.EmotionCardUtilLoaderFound) EmotionCardPatch();
            else
                ModParameters.Harmony.CreateClassProcessor(typeof(EmotionSelectionUnitPatchWithoutEmotionUtil)).Patch();
        }

        private static void EmotionCardPatch()
        {
            ModParameters.Harmony.CreateClassProcessor(typeof(CustomFloorHarmonyPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(LevelUpUIHotfix)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(EmotionSelectionUnitPatch)).Patch();
            ModParameters.Harmony.CreateClassProcessor(typeof(EmotionEgoCardSelectionPatch)).Patch();
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "Stage_Hod_New" ||
                _language == GlobalGameManager.Instance.CurrentOption.language) return;
            _language = GlobalGameManager.Instance.CurrentOption.language;
            LocalizationUtil.LoadLocalization(GlobalGameManager.Instance.CurrentOption.language);
            var onLocalize = LocalizationUtil.OnLocalize;
            onLocalize?.Invoke(GlobalGameManager.Instance.CurrentOption.language);
        }
    }
}