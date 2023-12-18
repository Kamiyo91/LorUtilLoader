using System;
using System.Collections.Generic;
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
            ModParameters.RushBattleManager = null;
            ModParameters.StartWaveIndex = 0;
            if (ModParameters.ChangedFormation.Item1)
            {
                __instance._formation =
                    new FormationModel(
                        Singleton<FormationXmlList>.Instance.GetData(ModParameters.ChangedFormation.Item2));
                ModParameters.ChangedFormation = new Tuple<bool, int>(false, 0);
            }

            if (ModParameters.ChangingAct)
            {
                ModParameters.ChangingAct = false;
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
                ModParameters.StartWaveIndex = index;
            }
            else
            {
                selectedWave = rushBattleOptions.Waves.FirstOrDefault();
                if (selectedWave == null) return;
            }

            var stageName = string.Empty;
            if (!string.IsNullOrEmpty(selectedWave.StageManagerName))
                stageName = selectedWave.StageManagerName;
            ModParameters.NextActManager = new Tuple<string, List<string>>(stageName, selectedWave.MapNames);
            __instance._availableUnitNumber = selectedWave.UnitAllowed;
            __instance._formation =
                new FormationModel(Singleton<FormationXmlList>.Instance.GetData(selectedWave.FormationId));
            __instance._unitList.Clear();
            foreach (var unitModel in selectedWave.UnitModels)
                __instance._unitList.Add(
                    UnitUtil.CreateUnitBattleDataByEnemyUnitId(stage,
                        new LorId(unitModel.PackageId, unitModel.Id), unitModel.HideInfo));
            __instance.team.Init(__instance._unitList, Faction.Enemy, stage.ClassInfo);
        }

        [HarmonyPatch(typeof(StageController), nameof(StageController.StartBattle))]
        [HarmonyPrefix]
        public static void StageController_StartBattle(StageController __instance)
        {
            var rushBattleOptions = ModParameters.RushBattleModels.FirstOrDefault(x =>
                x.Id == __instance._stageModel.ClassInfo.id.id &&
                x.PackageId == __instance._stageModel.ClassInfo.id.packageId);
            if (rushBattleOptions == null) return;
            var stage = __instance._stageModel.GetWave(__instance._currentWave);
            if (!string.IsNullOrEmpty(ModParameters.NextActManager.Item1))
                stage._managerScript = ModParameters.NextActManager.Item1;
            if (ModParameters.NextActManager.Item2.Any())
            {
                __instance._stageModel.ClassInfo.mapInfo = new List<string>();
                foreach (var map in ModParameters.NextActManager.Item2)
                    __instance._stageModel.ClassInfo.mapInfo.Add(map);
            }

            ModParameters.NextActManager = new Tuple<string, List<string>>(string.Empty, new List<string>());
            ModParameters.RushBattleManager = new EmenyTeamStageManager_RushBattleLoader_24321();
        }

        [HarmonyPatch(typeof(StageController), nameof(StageController.StartBattle))]
        [HarmonyPostfix]
        public static void StageController_StartBattle_Post()
        {
            ModParameters.RushBattleManager?.OnWaveStart();
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
        }
    }
}