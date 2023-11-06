using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLoader21341.Models;
using UtilLoader21341.Util;

namespace UtilLoader21341.StageManager
{
    public class EmenyTeamStageManager_RushBattleLoader_24321 : EnemyTeamStageManager
    {
        private readonly Dictionary<int, RushBattleModelSubRoot> _clonedPhases =
            new Dictionary<int, RushBattleModelSubRoot>();

        private bool _isLastWave;
        public RushBattleModelSubRoot ActualPhase;

        public int ActualPhaseInt;

        //public CustomMapHandler Cmh;
        public bool ForcedChanged;
        public List<int> FoughtWaves = new List<int>();
        public bool IsInfinite;
        public bool IsRandom;

        public RushBattleModelMainRoot MainRushBattleOptions;

        //public int MapPhase;
        public Dictionary<int, RushBattleModelSubRoot> Phases = new Dictionary<int, RushBattleModelSubRoot>();
        public string WaveCode = string.Empty;

        public void SetParameter(StageModel stageModel, List<RushBattleModelSubRoot> phases,
            bool isInfinite = false, bool isRandom = false,
            bool clearData = false)
        {
            try
            {
                IsInfinite = isInfinite;
                IsRandom = isRandom;
                foreach (var phase in phases.Select((x, i) => (i, x)))
                {
                    Phases.Add(phase.i, phase.x);
                    _clonedPhases.Add(phase.i, phase.x);
                }

                if (!Singleton<StageController>.Instance.GetStageModel()
                        .GetStageStorageData("RushBattlePhaseSave23421", out ActualPhaseInt))
                    ActualPhaseInt = ModParameters.RandomWaveStart;
                ActualPhase = Phases[ActualPhaseInt];
                //MapPhase = ActualPhase.StarterMapPhase;
                if (clearData || !Singleton<StageController>.Instance.GetStageModel()
                        .GetStageStorageData("FoughtPhaseSave23421", out FoughtWaves)) FoughtWaves = new List<int>();
                if (FoughtWaves != null && FoughtWaves.Any())
                    foreach (var key in FoughtWaves)
                        Phases.Remove(key);
                //if (!string.IsNullOrEmpty(ActualPhase.CmhPackageId))
                //    Cmh = CustomMapHandler.GetCMU(ActualPhase.CmhPackageId);
                //stageModel.ClassInfo.mapInfo = new List<string>();
                //foreach (var map in ActualPhase.MapStageNames)
                //    stageModel.ClassInfo.mapInfo.Add(map);
                _isLastWave = Phases.Count < 2;
                if (!IsInfinite && _isLastWave) return;
                stageModel._waveList.Add(new StageWaveModel());
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.InnerException);
            }
        }

        public void ChangeParameters(List<RushBattleModelSubRoot> phases, bool isInfinite = false,
            bool isRandom = false)
        {
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            IsInfinite = isInfinite;
            IsRandom = isRandom;
            Phases.Clear();
            _clonedPhases.Clear();
            FoughtWaves.Clear();
            foreach (var phase in phases.Select((x, i) => (i, x)))
            {
                Phases.Add(phase.i, phase.x);
                _clonedPhases.Add(phase.i, phase.x);
            }

            _isLastWave = Phases.Count < 2;
            if (_isLastWave && !IsInfinite) stageModel._waveList.RemoveAt(stageModel._waveList.Count - 1);
            ForcedChanged = true;
        }

        //public void SetMapPhase(int value)
        //{
        //    MapPhase = value;
        //}

        public override void OnWaveStart()
        {
            try
            {
                Singleton<StageController>.Instance.GetStageModel()
                    .GetStageStorageData("FoughtSwitchSaved23421", out WaveCode);
                var stageModel = Singleton<StageController>.Instance.GetStageModel();
                MainRushBattleOptions = ModParameters.RushBattleModels.FirstOrDefault(x =>
                    x.Id == stageModel.ClassInfo.id.id && x.PackageId == stageModel.ClassInfo.id.packageId &&
                    (string.IsNullOrEmpty(WaveCode) || x.WaveCode.Contains(WaveCode)));
                if (MainRushBattleOptions == null)
                {
                    Debug.LogError("Rush Battle Options not Found!");
                    return;
                }

                SetParameter(stageModel, MainRushBattleOptions.Waves.OrderBy(x => x.WaveOrder).ToList(),
                    MainRushBattleOptions.IsInfinite,
                    MainRushBattleOptions.IsRandom);
                //if (FoughtWaves != null && FoughtWaves.Any())
                //    foreach (var key in FoughtWaves)
                //        Phases.Remove(key);
                if (ActualPhase.StartEmotionLevel == 0) return;
                foreach (var unit in BattleObjectManager.instance.GetList(Faction.Enemy))
                    unit.LevelUpEmotion(ActualPhase.StartEmotionLevel);
                //if (Cmh == null) return;
                //foreach (var map in ActualPhase.Maps)
                //    MapUtil.InitEnemyMapBattleRush(ActualPhase.CmhPackageId, map);
                //if (MapPhase == -1) return;
                //Cmh.EnforceMap(MapPhase);
                //Singleton<StageController>.Instance.CheckMapChange();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.InnerException);
            }
        }

        //public void ChangeMusic(string packageId, string musicFileName, string mapName)
        //{
        //    CustomMapHandler.GetCMU(packageId).SetMapBgm(musicFileName, true, mapName);
        //}

        //public override void OnRoundStart()
        //{
        //    if (MapPhase == -1 || Cmh == null) return;
        //    Cmh.EnforceMap(MapPhase);
        //}

        //public override void OnRoundEndTheLast()
        //{
        //    if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count < 1) ChangeInnerPhase();
        //}

        //public void ChangeInnerPhase()
        //{
        //    if (!ActualPhase.InnerPhases.TryGetValue(ActualPhase.ActualInnerPhase, out var units)) return;
        //    ActualPhase.ActualInnerPhase++;
        //    foreach (var unit in BattleObjectManager.instance.GetList(Faction.Enemy))
        //        BattleObjectManager.instance.UnregisterUnit(unit);
        //    foreach (var model in units.Select((x, i) => (i, x)))
        //        UnitUtil.AddNewUnitWithDefaultData(model.x, model.i);
        //}

        public override void OnEndBattle()
        {
            if (_isLastWave && !IsInfinite) return;
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            if (!string.IsNullOrEmpty(WaveCode)) stageModel.SetStageStorgeData("FoughtSwitchSaved23421", WaveCode);
            if (!string.IsNullOrEmpty(ActualPhase.SwitchWaveCode))
            {
                stageModel.SetStageStorgeData("FoughtSwitchSaved23421", ActualPhase.SwitchWaveCode);
                var newWaves =
                    ModParameters.RushBattleModels.FirstOrDefault(x => x.WaveCode.Contains(ActualPhase.SwitchWaveCode));
                if (newWaves != null)
                    ChangeParameters(newWaves.Waves, newWaves.IsInfinite, newWaves.IsRandom);
            }

            if (ActualPhase.RecoverPlayerUnits)
                foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player))
                    unit.UnitReviveAndRecovery(unit.MaxHp, true);
            var nextWaveModel = stageModel._waveList.ElementAt(Singleton<StageController>.Instance._currentWave);
            if (nextWaveModel == null) return;
            if (!ForcedChanged)
            {
                FoughtWaves.Add(ActualPhaseInt);
                Phases.Remove(ActualPhaseInt);
                stageModel.SetStageStorgeData("FoughtPhaseSave23421", FoughtWaves);
            }

            ActualPhaseInt = !IsRandom
                ? ActualPhaseInt++
                : Phases.Any()
                    ? Phases.Keys.ElementAt(RandomUtil.Range(0, Phases.Count - 1))
                    : 0;
            stageModel.SetStageStorgeData("RushBattlePhaseSave23421", ActualPhaseInt);
            if (!Phases.TryGetValue(ActualPhaseInt, out ActualPhase))
            {
                Debug.LogError($"Entry no more phases - Infinite ? {IsInfinite}");
                if (!IsInfinite) return;
                stageModel.SetStageStorgeData("FoughtPhaseSave23421", new List<int>());
                ActualPhaseInt =
                    !IsRandom ? 0 : _clonedPhases.Keys.ElementAt(RandomUtil.Range(0, Phases.Count - 1));
                stageModel.SetStageStorgeData("RushBattlePhaseSave23421", ActualPhaseInt);
                if (!_clonedPhases.TryGetValue(ActualPhaseInt, out ActualPhase))
                {
                    Debug.LogError("Infinite Battle Error - Next Phase not found, Ending the battle");
                    return;
                }
            }

            var stageWaveInfo = Singleton<StageController>.Instance.GetCurrentWaveModel()._stageWaveInfo;
            stageWaveInfo.formationId = Mathf.Clamp(ActualPhase.FormationId, 1, 41);
            stageWaveInfo.availableNumber = ActualPhase.UnitAllowed;
            ModParameters.ChangingAct = true;
            var mapList = new List<string>();
            mapList.AddRange(ActualPhase.MapStageNames);
            ModParameters.NextActManager = new Tuple<string, List<string>>(ActualPhase.StageManagerName, mapList);
            stageModel._waveList.ElementAt(stageModel._waveList.Count - 1).Init(stageModel, stageWaveInfo);
            var list = new List<UnitBattleDataModel>();
            UnitUtil.PreparePreBattleEnemyUnits(ActualPhase.UnitModels, stageModel, list);
            nextWaveModel.ResetUnitBattleDataList(list);
            if (!ActualPhase.PlayerUnitModels.Any() && !ActualPhase.ReloadOriginalPlayerUnits.Any()) return;
            foreach (var stageFloor in stageModel.GetAvailableFloorList())
            {
                var playerUnits = ActualPhase.PlayerUnitModels.Where(x => x.Floor == stageFloor.Sephirah)
                    .SelectMany(x => x.UnitModels).ToList();
                if (playerUnits.Any())
                {
                    stageFloor._unitList.Clear();
                    UnitUtil.PreparePreBattleAllyUnits(stageFloor, playerUnits, stageModel, stageFloor._unitList);
                }

                if (!ActualPhase.ReloadOriginalPlayerUnits.Contains(stageFloor.Sephirah)) continue;
                foreach (var unitDataModel in stageFloor._floorModel.GetUnitDataList()
                             .Where(unitDataModel => stageFloor._unitList.Count < 5))
                    stageFloor._unitList.Add(UnitUtil.InitUnitDefault(stageModel, unitDataModel));
            }
        }
    }
}