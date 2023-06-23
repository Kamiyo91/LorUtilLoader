using System.Collections.Generic;
using System.Linq;
using CustomMapUtility;
using LOR_XML;
using UnityEngine;
using UtilLoader21341.Extensions;
using UtilLoader21341.Models;

namespace UtilLoader21341.Util
{
    public static class MechUtil
    {
        public static bool SurviveCheck<T>(this BattleUnitModel owner, int dmg, int surviveHp, ref bool ignore,
            int recoverToHp = 20, bool nearDeathBuff = false, bool recoverLight = false,
            List<AbnormalityCardDialog> dialog = null, Color? color = null, bool positiveColor = false,
            bool negativeColor = false) where T : BattleUnitBuf, new()
        {
            if (owner.hp - dmg > surviveHp || ignore) return false;
            ignore = true;
            owner.UnitReviveAndRecovery(0, recoverLight);
            if (dialog != null && dialog.Any())
                UnitUtil.BattleAbDialog(owner.view.dialogUI, dialog, color ?? Color.green, positiveColor,
                    negativeColor);
            owner.SetHp(recoverToHp);
            if (nearDeathBuff)
                owner.bufListDetail.AddBufWithoutDuplication(new T());
            owner.bufListDetail.AddBufWithoutDuplication(
                new BattleUnitBuf_ImmunityToStatusAlimentType_DLL21341());
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_Immortal_DLL21341());
            return true;
        }

        public static void ReviveCheck(this BattleUnitModel owner, ref bool ignore, int recoverHp = 20,
            bool recoverLight = false, List<AbnormalityCardDialog> dialog = null, Color? color = null,
            bool forcedRetreat = false, bool positiveColor = false, bool negativeColor = false)
        {
            if (ignore || !owner.IsDead()) return;
            ignore = true;
            owner.UnitReviveAndRecovery(recoverHp, recoverLight);
            if (dialog != null && dialog.Any())
                UnitUtil.BattleAbDialog(owner.view.dialogUI, dialog, color ?? Color.green, positiveColor,
                    negativeColor);
            if (forcedRetreat) owner.forceRetreat = true;
        }

        public static bool EgoActiveWithMapChange<T, T2>(this BattleUnitModel owner, ref bool ignore,
            ref bool mapActive, string egoskinName = "",
            bool refreshUI = false, bool isBaseGameSkin = false, List<LorId> emotionCardsId = null,
            List<AbnormalityCardDialog> dialog = null, Color? color = null, bool positiveColor = false,
            bool negativeColor = false,
            MapModelRoot mapModel = null)
            where T : BattleUnitBuf, new() where T2 : MapManager, ICMU, new()
        {
            if (ignore || owner.bufListDetail.HasAssimilation()) return false;
            ignore = true;
            if (!string.IsNullOrEmpty(egoskinName))
            {
                if (isBaseGameSkin)
                    owner.UnitData.unitData.bookItem.SetCharacterName(
                        egoskinName.Equals("BlackSilence4") ? "BlackSilence3" : egoskinName);
                else
                    owner.UnitData.unitData.bookItem.ClassInfo.CharacterSkin =
                        new List<string> { egoskinName };
                owner.view.SetAltSkin(egoskinName);
            }

            owner.bufListDetail.AddBufWithoutDuplication(new T());
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            if (owner.faction == Faction.Player && emotionCardsId != null && emotionCardsId.Any())
                foreach (var id in emotionCardsId)
                    owner.personalEgoDetail.AddCard(id);
            if (refreshUI) UnitUtil.RefreshCombatUI();
            if (dialog != null && dialog.Any())
                UnitUtil.BattleAbDialog(owner.view.dialogUI, dialog, color ?? Color.green, positiveColor,
                    negativeColor);
            if (mapModel != null)
                MapUtil.ChangeToEgoMap<T2>(mapModel, CustomMapHandler.GetCMU(owner.Book.BookId.packageId),
                    ref mapActive);
            return true;
        }

        public static bool EgoActive<T>(this BattleUnitModel owner, ref bool ignore, string egoskinName = "",
            bool refreshUI = false, bool isBaseGameSkin = false, List<LorId> emotionCardsId = null,
            List<AbnormalityCardDialog> dialog = null, Color? color = null, bool positiveColor = false,
            bool negativeColor = false)
            where T : BattleUnitBuf, new()
        {
            if (ignore || owner.bufListDetail.HasAssimilation()) return false;
            ignore = true;
            if (!string.IsNullOrEmpty(egoskinName))
            {
                if (isBaseGameSkin)
                    owner.UnitData.unitData.bookItem.SetCharacterName(
                        egoskinName.Equals("BlackSilence4") ? "BlackSilence3" : egoskinName);
                else
                    owner.UnitData.unitData.bookItem.ClassInfo.CharacterSkin =
                        new List<string> { egoskinName };
                owner.view.SetAltSkin(egoskinName);
            }

            owner.bufListDetail.AddBufWithoutDuplication(new T());
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            if (owner.faction == Faction.Player && emotionCardsId != null && emotionCardsId.Any())
                foreach (var id in emotionCardsId)
                    owner.personalEgoDetail.AddCard(id);
            if (refreshUI) UnitUtil.RefreshCombatUI();
            if (dialog != null && dialog.Any())
                UnitUtil.BattleAbDialog(owner.view.dialogUI, dialog, color ?? Color.green, positiveColor,
                    negativeColor);
            return true;
        }

        public static void DeactiveEgo<T>(this BattleUnitModel owner, ref bool ignore, ref bool mapActive,
            string egoskinName = "",
            bool refreshUI = false, LorId egoCard = null, List<LorId> emotionCardsId = null,
            List<LorId> passivesToRemove = null, LorId mapCardId = null) where T : BattleUnitBuf, new()
        {
            if (!ignore) return;
            ignore = false;
            if (emotionCardsId != null)
                foreach (var cardId in emotionCardsId)
                    owner.personalEgoDetail.RemoveCard(cardId);
            if (egoCard != null)
                owner.personalEgoDetail.AddCard(egoCard);
            if (passivesToRemove != null)
                owner.passiveDetail.PassiveList.RemoveAll(x => passivesToRemove.Contains(x.id));
            if (!string.IsNullOrEmpty(egoskinName))
            {
                owner.view.model.UnitData.unitData.bookItem.ClassInfo.CharacterSkin =
                    new List<string> { egoskinName };
                owner.view.CreateSkin();
            }

            if (refreshUI) UnitUtil.RefreshCombatUI();
            if (mapActive && mapCardId != null)
                MapUtil.ReturnFromEgoAssimilationMap(owner.Book.BookId.packageId, ref mapActive, mapCardId);
        }
    }
}