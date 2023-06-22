﻿using System;
using System.Collections.Generic;
using System.Linq;
using CustomMapUtility;
using LOR_DiceSystem;
using LOR_XML;
using UI;
using UnityEngine;
using UtilLoader21341.Models;
using Random = UnityEngine.Random;

namespace UtilLoader21341.Util
{
    public static class UnitUtil
    {
        public static void PhaseChangeAllFactionUnitRecoverBonus(int hp, int stagger, int light,
            bool fullLightRecover = false, Faction faction = Faction.Player)
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(faction))
            {
                unit.RecoverHP(hp);
                unit.breakDetail.RecoverBreak(stagger);
                var finalLightRecover = fullLightRecover ? unit.cardSlotDetail.GetMaxPlayPoint() : light;
                unit.cardSlotDetail.RecoverPlayPoint(finalLightRecover);
            }
        }

        public static void RefreshCombatUI(bool forceReturn = false, bool returnEffect = false)
        {
            foreach (var (battleUnit, num) in BattleObjectManager.instance.GetList()
                         .Select((value, i) => (value, i)))
            {
                SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnit.UnitData.unitData, num,
                    true);
                if (forceReturn)
                    battleUnit.moveDetail.ReturnToFormationByBlink(returnEffect);
            }

            try
            {
                BattleObjectManager.instance.InitUI();
            }
            catch (IndexOutOfRangeException)
            {
                // ignored
            }
        }

        public static bool CheckSkinProjection(BattleUnitModel owner)
        {
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin)) return true;
            if (owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem) return false;
            owner.view.ChangeSkin(owner.UnitData.unitData.CustomBookItem.GetCharacterName());
            return true;
        }

        public static bool CheckSkinUnitData(UnitDataModel unitData)
        {
            if (!string.IsNullOrEmpty(unitData.workshopSkin)) return true;
            return unitData.bookItem != unitData.CustomBookItem;
        }

        public static bool CheckCardCost(BattleUnitModel owner, int baseValue)
        {
            return owner.allyCardDetail.GetAllDeck().Any(x => x.GetCost() > baseValue);
        }

        public static void BattleAbDialog(BattleDialogUI instance, List<AbnormalityCardDialog> dialogs,
            Color color)
        {
            var component = instance.GetComponent<CanvasGroup>();
            var dialog = dialogs[Random.Range(0, dialogs.Count)].dialog;
            var txtAbnormalityDlg = instance._txtAbnormalityDlg;
            if (txtAbnormalityDlg != null)
            {
                txtAbnormalityDlg.text = dialog;
                txtAbnormalityDlg.fontMaterial.SetColor("_GlowColor", color);
                txtAbnormalityDlg.color = color;
                var canvas = instance._canvas;
                if (canvas != null) canvas.enabled = true;
                component.interactable = true;
                component.blocksRaycasts = true;
                txtAbnormalityDlg.GetComponent<AbnormalityDlgEffect>().Init();
            }

            instance.AbnormalityDlgRoutine();
        }

        public static List<UnitBattleDataModel> UnitsToRecover(StageModel stageModel, UnitDataModel data,
            IEnumerable<SephirahType> unitTypes)
        {
            var list = new List<UnitBattleDataModel>();
            foreach (var sephirah in unitTypes)
                list.AddRange(stageModel.GetFloor(sephirah).GetUnitBattleDataList()
                    .Where(x => x.unitData == data));
            return list;
        }


        public static int AlwaysAimToTheSlowestDice(BattleUnitModel target, int targetSlot, bool aim)
        {
            if (!aim) return targetSlot;
            var speedValue = 999;
            var finalTarget = 0;
            foreach (var dice in target.speedDiceResult.Select((x, i) => new { i, x }))
            {
                if (speedValue <= dice.x.value) continue;
                speedValue = dice.x.value;
                finalTarget = dice.i;
            }

            return finalTarget;
        }

        public static BattleUnitModel IgnoreSephiraSelectionTarget(bool ignore, Faction faction = Faction.Player)
        {
            if (!ignore) return null;
            var instance = BattleObjectManager.instance.GetAliveList(faction);
            return instance.Any(x => !x.UnitData.unitData.isSephirah)
                ? RandomUtil.SelectOne(instance.Where(x => !x.UnitData.unitData.isSephirah).ToList())
                : null;
        }

        public static AudioClip GetSound(CustomMapHandler cmh, string audioName, bool isBaseGame)
        {
            if (string.IsNullOrEmpty(audioName)) return null;
            if (isBaseGame) return Resources.Load<AudioClip>("Sounds/MotionSound/" + audioName);
            cmh.LoadEnemyTheme(audioName, out var audioClip);
            return audioClip;
        }

        public static void PrepareSounds(string packageId, List<CharacterSound.Sound> motionSounds,
            Dictionary<MotionDetail, CharacterSound.Sound> dicMotionSounds,
            List<MotionSoundOptionRoot> customMotionSounds)
        {
            var cmh = CustomMapHandler.GetCMU(packageId);
            foreach (var customMotionSound in customMotionSounds)
                try
                {
                    var audioClipWin = GetSound(cmh, customMotionSound.MotionSound.FileNameWin,
                        customMotionSound.MotionSound.IsBaseSoundWin);
                    var audioClipLose = GetSound(cmh, customMotionSound.MotionSound.FileNameLose,
                        customMotionSound.MotionSound.IsBaseSoundLose);
                    var item = motionSounds.FirstOrDefault(x => x.motion == customMotionSound.Motion);
                    var sound = new CharacterSound.Sound
                    {
                        motion = customMotionSound.Motion,
                        winSound = audioClipWin,
                        loseSound = audioClipLose
                    };
                    if (item != null)
                        motionSounds.Remove(item);
                    motionSounds.Add(sound);
                    if (dicMotionSounds.ContainsKey(customMotionSound.Motion))
                        dicMotionSounds.Remove(customMotionSound.Motion);
                    dicMotionSounds.Add(customMotionSound.Motion, sound);
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        public static void AddCustomUnits(StageLibraryFloorModel instance, StageModel stage,
            List<UnitBattleDataModel> unitList, PreBattleOptionRoot preBattleOptions, string packageId)
        {
            var unitModels = preBattleOptions.CustomUnits.FirstOrDefault(x => x.Floor == instance.Sephirah);
            if (unitModels == null) return;
            foreach (var unitParameters in unitModels.CustomUnit)
            {
                var unitDataModel = new UnitDataModel(new LorId(packageId, unitParameters.Id),
                    instance.Sephirah, true);
                unitDataModel.SetTemporaryPlayerUnitByBook(new LorId(packageId,
                    unitParameters.Id));
                unitDataModel.bookItem.ClassInfo.categoryList.Add(BookCategory.DeckFixed);
                unitDataModel.isSephirah = false;
                unitDataModel.SetCustomName(GenericUtil.GetCharacterName(unitParameters.PackageId, "Not Found",
                    unitParameters.UnitNameId));
                if (unitParameters.AdditionalPassiveIds.Any())
                    foreach (var passive in unitParameters.AdditionalPassiveIds.Where(x =>
                                 !unitDataModel.bookItem.ClassInfo.EquipEffect.PassiveList.Contains(
                                     new LorId(x.PackageId, x.Id))))
                        unitDataModel.bookItem.ClassInfo.EquipEffect.PassiveList.Add(new LorId(passive.PackageId,
                            passive.Id));
                unitDataModel.CreateDeckByDeckInfo();
                unitDataModel.forceItemChangeLock = true;
                if (!string.IsNullOrEmpty(unitParameters.SkinName))
                    unitDataModel.bookItem.ClassInfo.CharacterSkin = new List<string> { unitParameters.SkinName };
                var unitBattleDataModel = new UnitBattleDataModel(stage, unitDataModel);
                unitBattleDataModel.Init();
                unitList.Add(unitBattleDataModel);
            }
        }

        public static void AddSephirahUnits(StageLibraryFloorModel instance, StageModel stage,
            List<UnitBattleDataModel> unitList, PreBattleOptionRoot options)
        {
            var sephirahUnitTypes = options.SephirahUnits.FirstOrDefault(x => x.Floor == instance.Sephirah);
            if (sephirahUnitTypes == null) return;
            unitList?.AddRange(sephirahUnitTypes.SephirahUnit.Select(sephirah => InitUnitDefault(stage,
                LibraryModel.Instance.GetOpenedFloorList()
                    .FirstOrDefault(x => x.Sephirah == sephirah)
                    ?.GetUnitDataList()
                    .FirstOrDefault(y => y.isSephirah))));
        }

        public static UnitBattleDataModel InitUnitDefault(StageModel stage, UnitDataModel data)
        {
            var unitBattleDataModel = new UnitBattleDataModel(stage, data);
            unitBattleDataModel.Init();
            return unitBattleDataModel;
        }

        public static bool CheckForceAggroByKeypage(BattleUnitModel instance, BattleUnitModel target,
            KeypageOptionRoot keypageOption, int myIndex, int targetIndex, bool isLastDie, ref bool result)
        {
            if (keypageOption.ForceAggroOptions == null) return false;
            if (keypageOption.ForceAggroOptions.ForceAggro)
            {
                result = true;
                return true;
            }

            if (keypageOption.ForceAggroOptions.ForceAggroSpeedDie.Contains(myIndex) ||
                (keypageOption.ForceAggroOptions.ForceAggroLastDie && isLastDie))
            {
                result = true;
                return true;
            }

            if (!keypageOption.ForceAggroOptions.RedirectOnlyWithSlowerSpeed) return false;
            var speed = instance.GetSpeed(myIndex);
            var speed2 = target.GetSpeed(targetIndex);
            result = speed < speed2;
            return true;
        }

        public static bool CheckForceAggroByPassive(BattleUnitModel instance, BattleUnitModel target, int myIndex,
            int targetIndex, bool isLastDie, ref bool result)
        {
            var passives = ModParameters.PassiveOptions.Where(x =>
                instance.passiveDetail.PassiveList.Exists(y =>
                    x.PackageId == y.id.packageId && x.PassiveId == y.id.id));
            var check = false;
            foreach (var passive in passives)
            {
                if (passive?.ForceAggroOptions == null) continue;
                if (passive.ForceAggroOptions.ForceAggroSpeedDie.Contains(myIndex) ||
                    (passive.ForceAggroOptions.ForceAggroLastDie && isLastDie))
                {
                    result = true;
                    return true;
                }

                if (passive.ForceAggroOptions.ForceAggro)
                {
                    result = true;
                    return true;
                }

                if (!passive.ForceAggroOptions.RedirectOnlyWithSlowerSpeed) continue;
                var speed = instance.GetSpeed(myIndex);
                var speed2 = target.GetSpeed(targetIndex);
                result = speed < speed2;
                check = true;
            }

            return check;
        }

        public static bool CheckForceAggroByBuff(BattleUnitModel instance, BattleUnitModel target, int myIndex,
            int targetIndex, bool isLastDie, ref bool result)
        {
            var buffs = ModParameters.BuffOptions.Where(x =>
                instance.bufListDetail.GetActivatedBufList().Exists(y => y.keywordId == x.BuffId));
            var check = false;
            foreach (var buff in buffs)
            {
                if (buff?.ForceAggroOptions == null) continue;
                if (buff.ForceAggroOptions.ForceAggroSpeedDie.Contains(myIndex) ||
                    (buff.ForceAggroOptions.ForceAggroLastDie && isLastDie))
                {
                    result = true;
                    return true;
                }

                if (buff.ForceAggroOptions.ForceAggro)
                {
                    result = true;
                    return true;
                }

                if (!buff.ForceAggroOptions.RedirectOnlyWithSlowerSpeed) continue;
                var speed = instance.GetSpeed(myIndex);
                var speed2 = target.GetSpeed(targetIndex);
                result = speed < speed2;
                check = true;
            }

            return check;
        }

        public static bool IsLocked(StageRequirementRoot stageExtra)
        {
            if (stageExtra.RequiredLibraryLevel.HasValue &&
                LibraryModel.Instance.GetLibraryLevel() < stageExtra.RequiredLibraryLevel.Value)
                return true;
            return stageExtra.RequiredStageIds.Any(num =>
                LibraryModel.Instance.ClearInfo.GetClearCount(new LorId(num.PackageId, num.Id)) <= 0);
        }

        public static void ChangeLoneFixerPassive(Faction unitFaction, PassiveAbilityBase passive)
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(unitFaction))
            {
                if (!(unit.passiveDetail.PassiveList.Find(x => !x.destroyed && x is PassiveAbility_230008) is
                        PassiveAbility_230008
                        passiveLone)) continue;
                unit.passiveDetail.DestroyPassive(passiveLone);
                unit.passiveDetail.AddPassive(passive);
                //unit.passiveDetail.OnCreated();
            }
        }

        public static int SupportCharCheck(BattleUnitModel owner, bool otherSide = false)
        {
            return BattleObjectManager.instance
                .GetAliveList(otherSide ? owner.faction.ReturnOtherSideFaction() : owner.faction).Count(x =>
                    ModParameters.PassiveOptions.Any(y =>
                        x.passiveDetail.PassiveList.Exists(z =>
                            y.PackageId == z.id.packageId && y.PassiveId == z.id.id && y.IsSupportPassive)));
        }

        public static List<BattleUnitModel> ExcludeSupportChars(BattleUnitModel owner, bool otherSide = false)
        {
            return BattleObjectManager.instance
                .GetAliveList(otherSide ? owner.faction.ReturnOtherSideFaction() : owner.faction).Where(x =>
                    ModParameters.PassiveOptions.Any(y =>
                        x.passiveDetail.PassiveList.Exists(z =>
                            y.PackageId == z.id.packageId && y.PassiveId == z.id.id && y.IsSupportPassive)))
                .ToList();
        }

        public static BattleUnitModel AddNewUnitPlayerSideCustomData(UnitModelRoot unit,
            int pos, int emotionLevel = 0, bool addEmotionPassives = true, bool onWaveStartEffects = true)
        {
            var currentFloor = Singleton<StageController>.Instance.CurrentFloor;
            var unitData = new UnitDataModel((int)currentFloor * 10, currentFloor);
            var customBook = Singleton<BookInventoryModel>.Instance.GetBookListAll()
                .FirstOrDefault(x => x.BookId.Equals(new LorId(unit.PackageId, unit.Id)));
            if (customBook != null)
            {
                customBook.owner = null;
                unitData.EquipBook(customBook);
            }
            else
            {
                unitData = new UnitDataModel(new LorId(unit.PackageId, unit.Id), currentFloor);
            }

            unitData.SetCustomName(GenericUtil.GetCharacterName(unit.PackageId, "Not Found", unit.UnitNameId));
            var allyUnit = BattleObjectManager.CreateDefaultUnit(Faction.Player);
            allyUnit.index = pos;
            allyUnit.grade = unitData.grade;
            allyUnit.formation = unit.CustomPos != null
                ? new FormationPosition(new FormationPositionXmlData
                {
                    vector = unit.CustomPos
                })
                : Singleton<StageController>.Instance.GetCurrentStageFloorModel().GetFormationPosition(allyUnit.index);
            var unitBattleData = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitData);
            unitBattleData.Init();
            allyUnit.SetUnitData(unitBattleData);
            if (unit.AdditionalPassiveIds.Any())
                foreach (var passiveId in unit.AdditionalPassiveIds.Where(x =>
                             !allyUnit.passiveDetail.PassiveList.Exists(y =>
                                 y.id.packageId == x.PackageId && y.id.id == x.Id)))
                {
                    allyUnit.passiveDetail.AddPassive(passiveId.ToLorId());
                    allyUnit.passiveDetail.OnCreated();
                }

            allyUnit.OnCreated();
            if (unit.SummonedOnPlay) allyUnit.speedDiceResult = new List<SpeedDice>();
            BattleObjectManager.instance.RegisterUnit(allyUnit);
            allyUnit.passiveDetail.OnUnitCreated();
            allyUnit.LevelUpEmotion(emotionLevel);
            if (unit.LockedEmotion)
                allyUnit.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.cardSlotDetail.RecoverPlayPoint(allyUnit.cardSlotDetail.GetMaxPlayPoint());
            if (addEmotionPassives)
                allyUnit.AddEmotionPassives();
            if (onWaveStartEffects) allyUnit.OnWaveStart();
            //if (unit.AdditionalBuffs.Any())
            //    foreach (var buff in unit.AdditionalBuffs.Where(x => !allyUnit.HasBuff(x.GetType(), out _)))
            //        allyUnit.bufListDetail.AddBuf(buff);
            if (!unit.SummonedOnPlay) return allyUnit;
            allyUnit.OnRoundStart_speedDice();
            allyUnit.RollSpeedDice();
            if (unit.AutoPlay) allyUnit.SetAutoCardForPlayer();
            return allyUnit;
        }

        public static BattleUnitModel AddOriginalPlayerUnit(int index, int emotionLevel, bool addEmotionPassives = true)
        {
            var allyUnit = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(index);
            allyUnit.OnWaveStart();
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.LevelUpEmotion(emotionLevel);
            allyUnit.cardSlotDetail.RecoverPlayPoint(allyUnit.cardSlotDetail.GetMaxPlayPoint());
            if (addEmotionPassives) allyUnit.AddEmotionPassives();
            return allyUnit;
        }

        public static BattleUnitModel AddNewUnitWithDefaultData(UnitModelRoot unit, int pos,
            bool addEmotionPassives = true, int emotionLevel = 0, Faction unitSide = Faction.Player,
            bool onWaveStartEffects = true)
        {
            var currentFloor = Singleton<StageController>.Instance.CurrentFloor;
            var unitData = new UnitDataModel(new LorId(unit.PackageId, unit.Id),
                unitSide == Faction.Player ? currentFloor : SephirahType.None);
            unitData.SetCustomName(GenericUtil.GetCharacterName(unit.PackageId, "Not Found", unit.UnitNameId));
            var allyUnit = BattleObjectManager.CreateDefaultUnit(unitSide);
            allyUnit.index = pos;
            allyUnit.grade = unitData.grade;
            allyUnit.formation = unit.CustomPos != null
                ? new FormationPosition(new FormationPositionXmlData
                {
                    vector = unit.CustomPos
                })
                : unitSide == Faction.Player
                    ? Singleton<StageController>.Instance.GetCurrentStageFloorModel()
                        .GetFormationPosition(allyUnit.index)
                    : Singleton<StageController>.Instance.GetCurrentWaveModel().GetFormationPosition(allyUnit.index);
            var unitBattleData = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitData);
            unitBattleData.Init();
            allyUnit.SetUnitData(unitBattleData);
            if (unit.AdditionalPassiveIds.Any())
                foreach (var passiveId in unit.AdditionalPassiveIds.Where(x =>
                             !allyUnit.passiveDetail.PassiveList.Exists(y =>
                                 y.id.packageId == x.PackageId && y.id.id == x.Id)))
                {
                    allyUnit.passiveDetail.AddPassive(passiveId.ToLorId());
                    allyUnit.passiveDetail.OnCreated();
                }

            allyUnit.OnCreated();
            if (unit.SummonedOnPlay) allyUnit.speedDiceResult = new List<SpeedDice>();
            BattleObjectManager.instance.RegisterUnit(allyUnit);
            allyUnit.passiveDetail.OnUnitCreated();
            allyUnit.LevelUpEmotion(emotionLevel);
            if (unit.LockedEmotion)
                allyUnit.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.cardSlotDetail.RecoverPlayPoint(allyUnit.cardSlotDetail.GetMaxPlayPoint());
            if (addEmotionPassives)
                allyUnit.AddEmotionPassives();
            if (onWaveStartEffects) allyUnit.OnWaveStart();
            //if (unit.AdditionalBuffs.Any())
            //    foreach (var buff in unit.AdditionalBuffs.Where(x => !allyUnit.HasBuff(x.GetType(), out _)))
            //        allyUnit.bufListDetail.AddBuf(buff);
            if (!unit.SummonedOnPlay) return allyUnit;
            allyUnit.OnRoundStart_speedDice();
            allyUnit.RollSpeedDice();
            if (unit.AutoPlay) allyUnit.SetAutoCardForPlayer();
            return allyUnit;
        }

        public static bool NotTargetableCharCheck(BattleUnitModel target)
        {
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == target.Book.BookId.packageId && x.KeypageId == target.Book.BookId.id);
            return keypageItem == null || keypageItem.TargetableBySpecialCards;
        }

        public static List<BattleEmotionCardModel> AddValueToEmotionCardList(
            IEnumerable<BattleEmotionCardModel> addedEmotionCards, List<BattleEmotionCardModel> savedEmotionCards,
            bool ignoreDuplication = false)
        {
            savedEmotionCards.AddRange(addedEmotionCards.Where(emotionCard =>
                !savedEmotionCards.Exists(x => x.XmlInfo.Equals(emotionCard.XmlInfo) || ignoreDuplication)));
            return savedEmotionCards;
        }

        public static IEnumerable<BattleEmotionCardModel> GetEmotionCardByUnit(BattleUnitModel unit)
        {
            return unit.emotionDetail.PassiveList.ToList();
        }
    }
}