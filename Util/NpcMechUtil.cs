using System.Collections.Generic;
using System.Linq;
using UtilLoader21341.Extensions;

namespace UtilLoader21341.Util
{
    public static class NpcMechUtil
    {
        public static bool MechHpCheck(this BattleUnitModel owner, int dmg, int mechHp, ref bool mechChanging)
        {
            if (mechChanging) return false;
            if (mechHp == 0 || owner.hp - dmg > mechHp) return false;
            mechChanging = true;
            owner.SetHp(mechHp);
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.breakDetail.nextTurnBreak = false;
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_Immortal_DLL21341());
            return true;
        }

        public static void AddStartBuffsToPlayerUnits<T>(this BattleUnitModel owner, List<T> buffs)
            where T : BattleUnitBuf, new()
        {
            foreach (var buff in buffs)
            foreach (var unit in BattleObjectManager.instance.GetAliveList(
                         owner.faction.ReturnOtherSideFaction()))
                unit.bufListDetail.AddBuf(buff);
        }

        public static void OnEndBattleSave(this BattleUnitModel owner, string saveDataId, int phase)
        {
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            var currentWaveModel = Singleton<StageController>.Instance.GetCurrentWaveModel();
            if (currentWaveModel == null || currentWaveModel.IsUnavailable()) return;
            stageModel.SetStageStorgeData(saveDataId, phase);
            var list = BattleObjectManager.instance.GetAliveList(owner.faction)
                .Where(unit => !unit.IsSupportCharCheck()).Select(unit => unit.UnitData)
                .ToList();
            currentWaveModel.ResetUnitBattleDataList(list);
        }

        public static int RestartPhase(string saveDataId)
        {
            var curPhaseTryGet = Singleton<StageController>.Instance.GetStageModel()
                .GetStageStorageData<int>(saveDataId, out var curPhase);
            return curPhaseTryGet ? curPhase : 0;
        }
    }
}