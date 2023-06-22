using System.Collections.Generic;
using System.Linq;
using CustomMapUtility;
using UtilLoader21341.Comparers;
using UtilLoader21341.Extensions;
using UtilLoader21341.Models;

namespace UtilLoader21341.Util
{
    public static class MapUtil
    {
        public static void InitSephirahMap(string packageId, MapModelRoot model, SephirahType sephirah)
        {
            typeof(MapUtil).GetMethod("InitSephirahMapGeneric")
                ?.MakeGenericMethod(UtilExtensions.TrasformMapNameInType(model.Component, ModParameters.Assemblies))
                .Invoke(model, new object[] { packageId, model, sephirah });
        }

        public static void InitSephirahMapGeneric<T>(string packageId, MapModelRoot model, SephirahType sephirah)
            where T : MapManager, ICMU, new()
        {
            var cmh = CustomMapHandler.GetCMU(packageId);
            cmh.InitCustomSephirahMap<T>(sephirah, model.Stage, false, model.InitBgm, model.Bgx,
                model.Bgy, model.Fx, model.Fy, model.UnderX, model.UnderY);
        }

        public static void ChangeToSephirahMapGeneric<T>(string packageId, MapModelRoot model,
            SephirahType sephirah, bool playEffect) where T : MapManager, ICMU, new()
        {
            var cmh = CustomMapHandler.GetCMU(packageId);
            cmh.ChangeToCustomSephirahMap<T>(sephirah, model.Stage, Faction.Player, false, playEffect);
        }

        public static void InitEnemyMap<T>(CustomMapHandler cmh, MapModelRoot model)
            where T : MapManager, ICMU, new()
        {
            cmh.InitCustomMap<T>(model.Stage, false, true, model.Bgx,
                model.Bgy, model.Fx, model.Fy, model.UnderX, model.UnderY);
        }

        public static void ReturnFromEgoMap(CustomMapHandler cmh, string mapName, List<LorId> ids,
            bool isAssimilationMap = false)
        {
            if (CheckStageMap(ids) ||
                Singleton<StageController>.Instance.GetStageModel().ClassInfo.stageType ==
                StageType.Creature) return;
            cmh.RemoveCustomEgoMapByAssimilation(mapName);
            RemoveValueInAddedMap(mapName);
            if (!isAssimilationMap)
            {
                Singleton<StageController>.Instance.CheckMapChange();
                return;
            }

            MapChangedValue(true);
            if (!string.IsNullOrEmpty(Singleton<StageController>.Instance.GetStageModel().GetCurrentMapInfo()))
                cmh.EnforceTheme();
            Singleton<StageController>.Instance.CheckMapChange();
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }

        public static void RemoveValueInAddedMap(string name, bool removeAll = false)
        {
            var mapList = BattleSceneRoot.Instance._addedMapList;
            if (removeAll)
                mapList?.Clear();
            else
                mapList?.RemoveAll(x => x.name.Contains(name));
        }

        public static void MapChangedValue(bool value)
        {
            Singleton<StageController>.Instance._mapChanged = value;
        }

        public static bool ChangeMapGeneric<T>(CustomMapHandler cmh, MapModelRoot model,
            Faction faction = Faction.Player) where T : MapManager, ICMU, new()
        {
            if (CheckStageMap(model.OriginalMapStageIds) || SingletonBehavior<BattleSceneRoot>
                    .Instance.currentMapObject.isEgo ||
                Singleton<StageController>.Instance.GetStageModel().ClassInfo.stageType == StageType.Creature)
                return false;
            cmh.InitCustomMap<T>(model.Stage, model.IsPlayer, model.InitBgm, model.Bgx,
                model.Bgy, model.Fx, model.Fy, model.UnderX, model.UnderY);
            if (model.IsPlayer && !model.OneTurnEgo)
            {
                cmh.ChangeToCustomEgoMapByAssimilation<T>(model.Stage, faction);
                return true;
            }

            cmh.ChangeToCustomEgoMap<T>(model.Stage, faction);
            MapChangedValue(true);
            return true;
        }

        public static void ActiveCreatureBattleCamFilterComponent(bool value = true)
        {
            var battleCamera = SingletonBehavior<BattleCamManager>.Instance._effectCam;
            if (!(battleCamera is null)) battleCamera.GetComponent<CameraFilterPack_Drawing_Paper3>().enabled = value;
        }

        public static bool CheckStageMap(List<LorIdRoot> ids)
        {
            var lorId = Singleton<StageController>.Instance.GetStageModel().ClassInfo.id;
            return ids.Contains(lorId.ToLorIdRoot(), new LorIdRootComparer());
        }

        public static bool CheckStageMap(List<LorId> ids)
        {
            return ids.Contains(Singleton<StageController>.Instance.GetStageModel().ClassInfo.id);
        }

        public static bool ChangeMap(CustomMapHandler cmh, MapModelRoot model, Faction faction = Faction.Player)
        {
            return (bool)typeof(MapUtil).GetMethod("ChangeMapGeneric")
                .MakeGenericMethod(UtilExtensions.TrasformMapNameInType(model.Component, ModParameters.Assemblies))
                .Invoke(model, new object[] { cmh, model, faction });
        }

        public static void ChangeToSephirahMap(string packageId, MapModelRoot model, SephirahType sephirah,
            bool playEffect)
        {
            typeof(MapUtil).GetMethod("ChangeToSephirahMapGeneric")
                ?.MakeGenericMethod(UtilExtensions.TrasformMapNameInType(model.Component, ModParameters.Assemblies))
                .Invoke(model, new object[] { packageId, model, sephirah, playEffect });
        }

        public static void ChangeToEgoMap<T>(MapModelRoot mapModel, CustomMapHandler cmh, ref bool mapActive)
            where T : MapManager, ICMU, new()
        {
            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo || mapModel == null) return;
            if (ChangeMapGeneric<T>(cmh, mapModel)) mapActive = true;
        }

        public static void ReturnFromEgoAssimilationMap(string packageId, ref bool mapActive, LorId cardId)
        {
            if (!mapActive) return;
            mapActive = false;
            var mapModel = ModParameters.MapModels.FirstOrDefault(x =>
                x.CardIds.Contains(cardId.ToLorIdRoot(), new LorIdRootComparer()));
            if (mapModel == null) return;
            ReturnFromEgoMap(CustomMapHandler.GetCMU(packageId), mapModel.Stage,
                mapModel.OriginalMapStageIds.Select(x => x.ToLorId()).ToList(), true);
        }
    }
}