using System.Linq;
using HarmonyLib;
using UtilLoader21341.Models;
using UtilLoader21341.StageManager;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class BattleRushHarmonyPatch
    {
        [HarmonyPatch(typeof(StageWaveModel), "Init")]
        [HarmonyPostfix]
        public static void StageWaveModel_Init(StageWaveModel __instance, StageModel stage)
        {
            ModParameters.RandomWaveStart = 0;
            if (ModParameters.ChangingAct)
            {
                ModParameters.ChangingAct = false;
                __instance._managerScript = ModParameters.NextActManager;
                ModParameters.NextActManager = string.Empty;
                return;
            }

            var rushBattleOptions = ModParameters.RushBattleModels.FirstOrDefault(x =>
                x.Id == stage.ClassInfo.id.id && x.PackageId == stage.ClassInfo.id.packageId);
            if (rushBattleOptions == null) return;
            if (!rushBattleOptions.Waves.Any()) return;
            RushBattleModelSubRoot selectedWave;
            if (rushBattleOptions.IsRandom)
            {
                var index = RandomUtil.Range(0, rushBattleOptions.Waves.Count - 1);
                selectedWave = rushBattleOptions.Waves.ElementAtOrDefault(RandomUtil.Range(0,
                    rushBattleOptions.Waves.Count - 1));
                if (selectedWave == null) return;
                ModParameters.RandomWaveStart = index;
            }
            else
            {
                selectedWave = rushBattleOptions.Waves.FirstOrDefault();
                if (selectedWave == null) return;
            }

            if (!string.IsNullOrEmpty(selectedWave.StageManagerName))
                __instance._managerScript = selectedWave.StageManagerName;
            __instance._availableUnitNumber = selectedWave.UnitAllowed;
            __instance._formation =
                new FormationModel(Singleton<FormationXmlList>.Instance.GetData(selectedWave.FormationId));
            __instance._unitList.Clear();
            UnitUtil.PreparePreBattleEnemyUnits(selectedWave.UnitModels, stage, __instance._unitList);
            __instance.team.Init(__instance._unitList, Faction.Enemy, stage.ClassInfo);
        }

        [HarmonyPatch(typeof(StageController), nameof(StageController.StartBattle))]
        [HarmonyPostfix]
        public static void StageController_Init(StageController __instance)
        {
            var rushBattleOptions = ModParameters.RushBattleModels.FirstOrDefault(x =>
                x.Id == __instance._stageModel.ClassInfo.id.id &&
                x.PackageId == __instance._stageModel.ClassInfo.id.packageId);
            if (rushBattleOptions == null) return;
            ModParameters.RushBattleManager = new EmenyTeamStageManager_RushBattleLoader_24321();
            ModParameters.RushBattleManager.OnWaveStart();
        }

        [HarmonyPatch(typeof(StageController), nameof(StageController.EndBattlePhase))]
        [HarmonyPostfix]
        public static void StageController_EndBattlePhase(StageController __instance)
        {
            var rushBattleOptions = ModParameters.RushBattleModels.FirstOrDefault(x =>
                x.Id == __instance._stageModel.ClassInfo.id.id &&
                x.PackageId == __instance._stageModel.ClassInfo.id.packageId);
            if (rushBattleOptions == null || ModParameters.RushBattleManager == null) return;
            ModParameters.RushBattleManager.OnEndBattle();
            ModParameters.RushBattleManager = null;
        }
    }
}