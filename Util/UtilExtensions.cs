using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EmotionCardUtil;
using LOR_DiceSystem;
using UnityEngine;
using UtilLoader21341.Extensions;
using LorIdRoot = UtilLoader21341.Models.LorIdRoot;
using Random = UnityEngine.Random;

namespace UtilLoader21341.Util
{
    public static class UtilExtensions
    {
        public static void RemoveDiceTargets(this BattleUnitModel unit, bool breakUnit)
        {
            unit.view.speedDiceSetterUI.DeselectAll();
            foreach (var speedDice in unit.speedDiceResult)
                speedDice.breaked = true;
            var actionableEnemyList = Singleton<StageController>.Instance.GetActionableEnemyList();
            if (unit.faction != Faction.Player)
                return;
            foreach (var actor in actionableEnemyList)
            {
                if (actor.turnState != BattleUnitTurnState.BREAK)
                    actor.turnState = BattleUnitTurnState.WAIT_CARD;
                try
                {
                    for (var index2 = 0; index2 < actor.speedDiceResult.Count; ++index2)
                    {
                        if (actor.speedDiceResult[index2].breaked || index2 >= actor.cardSlotDetail.cardAry.Count)
                            continue;
                        var cardDataInUnitModel = actor.cardSlotDetail.cardAry[index2];
                        if (cardDataInUnitModel?.card == null) continue;
                        if (cardDataInUnitModel.card.GetSpec().Ranged == CardRange.FarArea ||
                            cardDataInUnitModel.card.GetSpec().Ranged == CardRange.FarAreaEach)
                        {
                            if (cardDataInUnitModel.subTargets.Exists(x => x.target == unit))
                            {
                                cardDataInUnitModel.subTargets.RemoveAll(x => x.target == unit);
                            }
                            else if (cardDataInUnitModel.target == unit)
                            {
                                if (cardDataInUnitModel.subTargets.Count > 0)
                                {
                                    var subTarget = RandomUtil.SelectOne(cardDataInUnitModel.subTargets);
                                    cardDataInUnitModel.target = subTarget.target;
                                    cardDataInUnitModel.targetSlotOrder = subTarget.targetSlotOrder;
                                    cardDataInUnitModel.earlyTarget = subTarget.target;
                                    cardDataInUnitModel.earlyTargetOrder = subTarget.targetSlotOrder;
                                }
                                else
                                {
                                    actor.allyCardDetail.ReturnCardToHand(actor.cardSlotDetail.cardAry[index2].card);
                                    actor.cardSlotDetail.cardAry[index2] = null;
                                }
                            }
                        }
                        else
                        {
                            if (cardDataInUnitModel.subTargets.Exists(x => x.target == unit))
                                cardDataInUnitModel.subTargets.RemoveAll(x => x.target == unit);
                            if (cardDataInUnitModel.target == unit)
                            {
                                var targetByCard = BattleObjectManager.instance.GetTargetByCard(actor,
                                    cardDataInUnitModel.card, index2, actor.TeamKill());
                                if (targetByCard != null)
                                {
                                    var targetSlot = Random.Range(0, targetByCard.speedDiceResult.Count);
                                    var num = actor.ChangeTargetSlot(cardDataInUnitModel.card, targetByCard, index2,
                                        targetSlot, actor.TeamKill());
                                    cardDataInUnitModel.target = targetByCard;
                                    cardDataInUnitModel.targetSlotOrder = num;
                                    cardDataInUnitModel.earlyTarget = targetByCard;
                                    cardDataInUnitModel.earlyTargetOrder = num;
                                }
                                else
                                {
                                    actor.allyCardDetail.ReturnCardToHand(actor.cardSlotDetail.cardAry[index2].card);
                                    actor.cardSlotDetail.cardAry[index2] = null;
                                }
                            }
                            else if (cardDataInUnitModel.earlyTarget == unit)
                            {
                                var targetByCard = BattleObjectManager.instance.GetTargetByCard(actor,
                                    cardDataInUnitModel.card, index2, actor.TeamKill());
                                if (targetByCard != null)
                                {
                                    var targetSlot = Random.Range(0, targetByCard.speedDiceResult.Count);
                                    var num = actor.ChangeTargetSlot(cardDataInUnitModel.card, targetByCard, index2,
                                        targetSlot, actor.TeamKill());
                                    cardDataInUnitModel.earlyTarget = targetByCard;
                                    cardDataInUnitModel.earlyTargetOrder = num;
                                }
                                else
                                {
                                    cardDataInUnitModel.earlyTarget = cardDataInUnitModel.target;
                                    cardDataInUnitModel.earlyTargetOrder = cardDataInUnitModel.targetSlotOrder;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Debug.LogError("target change error");
                }
            }

            unit.view.speedDiceSetterUI.BlockDiceAll(true);
            if (breakUnit) unit.view.speedDiceSetterUI.BreakDiceAll(true);
            SingletonBehavior<BattleManagerUI>.Instance.ui_TargetArrow.UpdateTargetList();
        }

        public static void ReadyCounterCard(this BattleUnitModel owner, int id, string packageId)
        {
            var card = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(packageId, id)));
            owner.cardSlotDetail.keepCard.AddBehaviours(card, card.CreateDiceCardBehaviorList());
            owner.allyCardDetail.ExhaustCardInHand(card);
        }

        public static Faction ReturnOtherSideFaction(this Faction faction)
        {
            return faction == Faction.Player ? Faction.Enemy : Faction.Player;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void RemoveImmortalBuff(this BattleUnitModel owner)
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_Immortal_DLL21341) is
                BattleUnitBuf_Immortal_DLL21341 buf)
                if (buf.LastOneScene)
                    owner.bufListDetail.RemoveBuf(buf);
            if (!(owner.bufListDetail.GetActivatedBufList()
                        .Find(x => x is BattleUnitBuf_ImmunityToStatusAlimentType_DLL21341) is
                    BattleUnitBuf_ImmunityToStatusAlimentType_DLL21341 buf2)) return;
            if (buf2.LastOneScene) owner.bufListDetail.RemoveBuf(buf2);
        }

        public static void SetAutoCardForPlayer(this BattleUnitModel unit)
        {
            if (unit.faction == Faction.Enemy) return;
            for (var j = 0; j < unit.speedDiceResult.Count; j++)
            {
                if (unit.speedDiceResult[j].breaked || unit.cardSlotDetail.cardAry[j] != null) continue;
                unit.cardOrder = j;
                unit.allyCardDetail.PlayTurnAutoForPlayer(j);
            }

            var selectedAllyDice = SingletonBehavior<BattleManagerUI>.Instance.selectedAllyDice;
            SingletonBehavior<BattleManagerUI>.Instance.ui_TargetArrow.UpdateTargetList();
            SingletonBehavior<BattleManagerUI>.Instance.ui_emotionInfoBar.UpdateCardsStateUI();
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitInformationPlayer.ReleaseSelectedCard();
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitInformationPlayer.CloseUnitInformation(true);
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitCardsInHand.OnPointerOverInSpeedDice = null;
            SingletonBehavior<BattleManagerUI>.Instance.ui_unitCardsInHand.SetToDefault();
            if (selectedAllyDice != null) BattleUIInputController.Instance.ResetCharacterCursor(false);
        }

        public static void LevelUpEmotion(this BattleUnitModel owner, int value)
        {
            for (var i = 0; i < value; i++)
            {
                owner.emotionDetail.LevelUp_Forcely(1);
                owner.emotionDetail.CheckLevelUp();
            }

            StageController.Instance.GetCurrentStageFloorModel().team.UpdateCoin();
        }

        public static void AddEmotionPassives(this BattleUnitModel unit)
        {
            var playerUnitsAlive = BattleObjectManager.instance.GetAliveList(Faction.Player);
            if (!playerUnitsAlive.Any()) return;
            foreach (var emotionCard in playerUnitsAlive.FirstOrDefault()
                         .emotionDetail.PassiveList.Where(x =>
                             x.XmlInfo.TargetType == EmotionTargetType.AllIncludingEnemy ||
                             x.XmlInfo.TargetType == EmotionTargetType.All))
            {
                if (unit.faction == Faction.Enemy &&
                    emotionCard.XmlInfo.TargetType == EmotionTargetType.All) continue;
                unit.emotionDetail.ApplyEmotionCard(emotionCard.XmlInfo);
            }
        }

        public static void UnitReviveAndRecovery(this BattleUnitModel owner, int hp, bool recoverLight)
        {
            hp = Mathf.Clamp(hp, 0, owner.MaxHp);
            if (owner.IsDead())
            {
                owner.bufListDetail.GetActivatedBufList()
                    .RemoveAll(x => !x.CanRecoverHp(999) || !x.CanRecoverBreak(999));
                owner.Revive(hp);
                owner.moveDetail.ReturnToFormationByBlink(true);
                owner.view.EnableView(true);
                owner.view.EnableStatNumber(true);
            }
            else
            {
                owner.bufListDetail.GetActivatedBufList()
                    .RemoveAll(x => !x.CanRecoverHp(999) || !x.CanRecoverBreak(999));
                owner.RecoverHP(hp);
            }

            owner.bufListDetail.RemoveBufAll(BufPositiveType.Negative);
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_sealTemp));
            owner.breakDetail.ResetGauge();
            owner.breakDetail.GetDefaultBreakGauge();
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.RecoverBreakLife(1, true);
            if (recoverLight) owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
        }

        public static void ChangeCardCostByValue(this BattleUnitModel owner, int changeValue, int baseValue,
            bool startDraw)
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck()
                         .Where(x => x.GetOriginCost() < baseValue))
            {
                battleDiceCardModel.GetBufList();
                battleDiceCardModel.AddCost(changeValue);
            }

            if (startDraw) owner.allyCardDetail.DrawCards(owner.UnitData.unitData.GetStartDraw());
        }

        public static void DrawUntilX(this BattleUnitModel owner, int x)
        {
            var count = owner.allyCardDetail.GetHand().Count;
            var num = x - count;
            if (num > 0) owner.allyCardDetail.DrawCards(num);
        }

        public static void VipDeath(this BattleUnitModel owner)
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction)
                         .Where(x => x != owner))
                unit.Die();
            if (owner.faction == Faction.Enemy) return;
            StageController.Instance.GetCurrentStageFloorModel().Defeat();
            StageController.Instance.EndBattle();
        }

        public static void ChangeSameCardsCost(this BattleUnitModel owner, BattlePlayingCardDataInUnitModel card,
            int value)
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck()
                         .FindAll(x => x != card.card && x.GetID() == card.card.GetID()))
            {
                battleDiceCardModel.GetBufList();
                battleDiceCardModel.AddCost(value);
            }
        }

        public static void ChangeAllCardCostByCardId(this BattleUnitModel owner, LorId cardId, int value)
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck()
                         .FindAll(x => x.GetID() == cardId))
            {
                battleDiceCardModel.GetBufList();
                battleDiceCardModel.AddCost(value);
            }
        }

        public static T GetActivePassive<T>(this BattleUnitModel owner) where T : PassiveAbilityBase
        {
            return (T)owner.passiveDetail.PassiveList.FirstOrDefault(x => x is T && !x.destroyed);
        }

        public static T GetActiveBuff<T>(this BattleUnitModel owner) where T : BattleUnitBuf
        {
            return (T)owner.bufListDetail.GetActivatedBufList().FirstOrDefault(x => x is T && !x.IsDestroyed());
        }

        public static void SetPassiveCombatLog(this BattleUnitModel owner, PassiveAbilityBase passive)
        {
            var battleCardResultLog = owner.battleCardResultLog;
            battleCardResultLog?.SetPassiveAbility(passive);
        }

        public static void SetEmotionCombatLog(this BattleUnitModel owner, BattleEmotionCardModel emotionCard)
        {
            owner.battleCardResultLog.SetEmotionAbility(true, emotionCard, emotionCard.XmlInfo.id);
        }

        public static void SetDieAbility(this BattleUnitModel owner, DiceCardAbilityBase ability)
        {
            var battleCardResultLog = owner.battleCardResultLog;
            battleCardResultLog?.SetDiceBehaviourAbility(true, ability.behavior, ability.card.card);
        }

        public static Type TrasformBuffNameInType(string name, List<Assembly> assemblies)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(x => x.Name.Equals($"BattleUnitBuf_{name}"));
        }

        public static Type TrasformMapNameInType(string name, List<Assembly> assemblies)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return assemblies.SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(x => x.Name.Equals($"{name}"));
        }

        public static bool CheckTargetSpeedByCard(this BattlePlayingCardDataInUnitModel card, int value)
        {
            var speedDiceResultValue = card.speedDiceResultValue;
            var target = card.target;
            var targetSlotOrder = card.targetSlotOrder;
            if (targetSlotOrder < 0 || targetSlotOrder >= target.speedDiceResult.Count) return false;
            var speedDice = target.speedDiceResult[targetSlotOrder];
            var targetDiceBroken = target.speedDiceResult[targetSlotOrder].breaked;
            return speedDiceResultValue - speedDice.value > value || targetDiceBroken;
        }

        public static IEnumerable<BattleEmotionCardModel> GetEmotionCardByUnit(this BattleUnitModel unit)
        {
            return unit.emotionDetail.PassiveList.ToList();
        }

        public static void ApplyEmotionCards(this BattleUnitModel unit,
            IEnumerable<BattleEmotionCardModel> emotionCardList)
        {
            foreach (var card in emotionCardList) unit.emotionDetail.ApplyEmotionCard(card.XmlInfo);
        }

        public static BattleUnitBuf AddBuff<T>(this BattleUnitModel owner, int stack, bool destroyat0Stack = false,
            int minStack = 0, int maxStack = 25)
            where T : BattleUnitBuf, new()
        {
            var buff = owner.GetActiveBuff<T>();
            if (buff == null)
            {
                buff = new T();
                owner.bufListDetail.AddBuf(buff);
                buff.stack = 0;
            }

            stack += buff.stack;
            stack = Mathf.Clamp(stack, minStack, maxStack);
            buff.stack = stack;
            if (destroyat0Stack && stack == 0) owner.bufListDetail.RemoveBuf(buff);
            return buff;
        }

        public static bool IsSupportCharCheck(this BattleUnitModel owner)
        {
            return ModParameters.PassiveOptions.Any(x =>
                owner.passiveDetail.PassiveList.Exists(y =>
                    x.PackageId == y.id.packageId && x.PassiveId == y.id.id && x.IsSupportPassive));
        }

        public static void DestroyPassive(this BattleUnitModel owner, LorId passiveId)
        {
            var passive = owner.passiveDetail.PassiveList.FirstOrDefault(x => x.id == passiveId && !x.destroyed);
            if (passive != null) passive.destroyed = true;
        }

        public static LorId ToLorId(this LorIdRoot root)
        {
            return new LorId(root.PackageId, root.Id);
        }

        public static LorIdRoot ToLorIdRoot(string packageId, int id)
        {
            return new LorIdRoot { Id = id, PackageId = packageId };
        }

        public static LorIdRoot ToLorIdRoot(this LorId root)
        {
            return new LorIdRoot { Id = root.id, PackageId = root.packageId };
        }

        public static EmotionCardUtil.LorIdRoot ToEmotionLorIdRoot(this LorId root)
        {
            return new EmotionCardUtil.LorIdRoot { Id = root.id, PackageId = root.packageId };
        }

        public static bool ActivatedEmotionCard(this BattleUnitModel owner, string packageId, int id)
        {
            return owner.GetActivatedCustomEmotionCard(packageId, id, out _);
        }

        public static T CheckPermanentBuff<T>(this BattleUnitModel owner, bool active = true, int startStacks = 0)
            where T : BattleUnitBuf, new()
        {
            if (!active || owner.bufListDetail.HasBuf<T>()) return null;
            return (T)owner.AddBuff<T>(startStacks);
        }
    }
}