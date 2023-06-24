using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EmotionCardUtil;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UtilLoader21341.Util
{
    public static class GenericUtil
    {
        public static string GetEffectText(string packageId, string baseMessage, string messageId, bool name = false)
        {
            if (string.IsNullOrEmpty(messageId)) return baseMessage;
            var tryloc = ModParameters.LocalizedItems.TryGetValue(packageId, out var localizedItem);
            if (tryloc && localizedItem.EffectTexts.TryGetValue(messageId, out var text))
                return name ? text.Name : text.Desc;
            return baseMessage;
        }

        public static string GetCharacterName(string packageId, string baseMessage, int messageId)
        {
            if (messageId < 1) return baseMessage;
            var tryloc = ModParameters.LocalizedItems.TryGetValue(packageId, out var localizedItem);
            if (tryloc && localizedItem.EnemyNames.TryGetValue(messageId, out var text))
                return text;
            return baseMessage;
        }

        public static async Task PutTaskDelay(int delay)
        {
            await Task.Delay(delay);
        }

        public static void OtherModCheck()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            ModParameters.DaatFloorFound = assemblies.Any(x => x.GetName().Name == "Daat Floor MOD");
            ModParameters.BaseModFound = assemblies.Any(x =>
                x.GetName().Name == "BaseMod" && x.GetType("SummonLiberation.Harmony_Patch") != null);
            ModParameters.EmotionCardUtilLoaderFound = assemblies.Any(x => x.GetName().Name == "1EmotionCardUtil");
        }

        public static void GameOver()
        {
            StageController.Instance.GetCurrentStageFloorModel().Defeat();
            StageController.Instance.EndBattle();
        }

        public static void OnLoadingScreen(Scene scene, LoadSceneMode _)
        {
            if (scene.name != "Stage_Hod_New" || ModParameters.ModLoaded) return;
            ModParameters.ModLoaded = true;
            UtilModLoader.LoadModsAfter();
            if (ModParameters.EmotionCardUtilLoaderFound) LoadEmotionPart();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadEmotionPart()
        {
            EmotionCardUtilLoader.LoadModsAfter();
        }

        public static BattleEffectSound PlaySound(AudioClip audio, float volumeControl = 1.5f, bool loop = false)
        {
            if (SingletonBehavior<BattleSoundManager>.Instance == null) return null;
            var sound = Object.Instantiate(SingletonBehavior<BattleSoundManager>.Instance.effectSoundPrefab);
            var soundVolume = SingletonBehavior<BattleSoundManager>.Instance.VolumeFX * volumeControl;
            sound.Init(audio, soundVolume, loop);
            return sound;
        }
    }
}