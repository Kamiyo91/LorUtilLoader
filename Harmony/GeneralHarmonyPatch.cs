using System;
using System.Collections.Generic;
using System.Linq;
using Battle.DiceAttackEffect;
using HarmonyLib;
using LOR_DiceSystem;
using UI;
using UnityEngine;
using UtilLoader21341.Comparers;
using UtilLoader21341.Extensions;
using UtilLoader21341.Models;
using UtilLoader21341.Util;
using Workshop;
using Object = UnityEngine.Object;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class GeneralHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookModel), "SetXmlInfo")]
        public static void BookModel_SetXmlInfo(BookModel __instance)
        {
            if (!ModParameters.PackageIds.Contains(__instance.BookId.packageId)) return;
            foreach (var cardOption in ModParameters.CardOptions.Where(x =>
                         x.Option == CardOption.OnlyPage &&
                         x.BookId.Contains(__instance.BookId.ToLorIdRoot(), new LorIdRootComparer())))
                __instance._onlyCards.AddRange(cardOption.Ids.Select(x =>
                    ItemXmlDataList.instance.GetCardItem(new LorId(cardOption.PackageId, x))));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitModel), "CanChangeAttackTarget")]
        public static void BattleUnitModel_CanChangeAttackTarget(BattleUnitModel __instance, BattleUnitModel target,
            int myIndex, int targetIndex, ref bool __result)
        {
            if (__instance == null || target == null || !target.AllowTargetChanging(__instance, targetIndex) ||
                __instance.DirectAttack() ||
                !target.IsTauntable() || (target.faction == Faction.Enemy &&
                                          Singleton<StageController>.Instance.IsBlockEnemyAggroChange()))
                return;
            var slottedCard = __instance.cardSlotDetail.cardAry[myIndex];
            var cardAbility = slottedCard?.card.CreateDiceCardSelfAbilityScript();
            if (cardAbility != null && !cardAbility.IsTargetChangable(target))
            {
                __result = false;
                return;
            }

            var passive = __instance.GetActivePassive<PassiveAbility_RedirectDiePassive_DLL21341>();
            if (passive != null && passive.SpeedDieSlot == myIndex)
            {
                __result = true;
                return;
            }

            var isLastDie = myIndex == __instance.speedDiceResult.Count - 1;
            var keypageOption = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == __instance.Book.BookId.packageId && x.KeypageId == __instance.Book.BookId.id);
            if (keypageOption != null && UnitUtil.CheckForceAggroByKeypage(__instance, target, keypageOption, myIndex,
                    targetIndex, isLastDie, ref __result)) return;
            if (UnitUtil.CheckForceAggroByPassive(__instance, target, myIndex, targetIndex, isLastDie, ref __result))
                return;
            UnitUtil.CheckForceAggroByBuff(__instance, target, myIndex, targetIndex, isLastDie, ref __result);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpriteDataManager), "Init")]
        public static void UISpriteDataManager_Init(UISpriteDataManager __instance)
        {
            foreach (var artWork in ModParameters.ArtWorks.Where(x =>
                         !x.Name.Contains("Glow") && !__instance._storyicons.Exists(y => y.type.Equals(x.Name))))
                __instance._storyicons.Add(new UIIconManager.IconSet
                {
                    type = artWork.Name,
                    icon = artWork.Sprite,
                    iconGlow = ModParameters.ArtWorks.FirstOrDefault(x => x.Name == $"{artWork.Name}Glow")?.Sprite ??
                               artWork.Sprite
                });
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DropBookInventoryModel), "LoadFromSaveData")]
        public static void DropBookInventoryModel_LoadFromSaveData(DropBookInventoryModel __instance)
        {
            foreach (var book in ModParameters.RewardOptions.SelectMany(x => x.Books))
            {
                var lorId = new LorId(book.LorId.PackageId, book.LorId.Id);
                var bookCount = __instance.GetBookCount(lorId);
                if (bookCount < 99) __instance.AddBook(lorId, 99 - bookCount);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryModel), "LoadFromSaveData")]
        public static void InventoryModel_LoadFromSaveData(InventoryModel __instance)
        {
            foreach (var cardItem in from cardItem in ModParameters.RewardOptions.SelectMany(x => x.Cards)
                     let cardCount = __instance.GetCardCount(new LorId(cardItem.LorId.PackageId, cardItem.LorId.Id))
                     where cardCount < cardItem.Quantity
                     select cardItem)
                __instance.AddCard(new LorId(cardItem.LorId.PackageId, cardItem.LorId.Id), cardItem.Quantity);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitCardsInHandUI), "UpdateCardList")]
        public static void BattleUnitCardsInHandUI_UpdateCardList(BattleUnitCardsInHandUI __instance)
        {
            if (__instance.CurrentHandState != BattleUnitCardsInHandUI.HandState.EgoCard) return;
            try
            {
                var unit = __instance.SelectedModel ?? __instance.HOveredModel;
                var passiveEgoBanned = unit.passiveDetail.PassiveList.Exists(x =>
                    ModParameters.PassiveOptions.Any(y =>
                        y.PackageId == x.id.packageId && y.PassiveId == x.id.id && y.BannedEgoFloorCards));
                var keypageEgoBanned = ModParameters.KeypageOptions.Exists(x =>
                    x.PackageId == unit.Book.BookId.packageId && x.KeypageId == unit.Book.BookId.id &&
                    x.BannedEgoFloorCards);
                if (!passiveEgoBanned && !keypageEgoBanned)
                    return;
                var list = ArtUtil.ReloadEgoHandUI(__instance, __instance.GetCardUIList(), unit,
                    __instance._activatedCardList,
                    ref __instance._xInterval).ToList();
                __instance.SetSelectedCardUI(null);
                for (var i = list.Count; i < __instance.GetCardUIList().Count; i++)
                    __instance.GetCardUIList()[i].gameObject.SetActive(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BookInventoryModel), "LoadFromSaveData")]
        public static void BookInventoryModel_LoadFromSaveData(BookInventoryModel __instance)
        {
            foreach (var keypageId in ModParameters.RewardOptions.SelectMany(x => x.Keypages).Where(keypageId =>
                         !Singleton<BookInventoryModel>.Instance.GetBookListAll().Exists(x =>
                             x.GetBookClassInfoId() == new LorId(keypageId.LorId.PackageId, keypageId.LorId.Id))))
            {
                var lorId = new LorId(keypageId.LorId.PackageId, keypageId.LorId.Id);
                var book = Singleton<BookXmlList>.Instance.GetData(lorId);
                var quantity = 1;
                switch (book.Rarity)
                {
                    case Rarity.Common:
                        quantity = 5;
                        break;
                    case Rarity.Uncommon:
                        quantity = 4;
                        break;
                    case Rarity.Rare:
                        quantity = 3;
                        break;
                }

                for (var i = 0; i < quantity; i++)
                    __instance.CreateBook(lorId);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISpriteDataManager), "GetStoryIcon")]
        public static void UISpriteDataManager_GetStoryIcon(ref string story)
        {
            if (story.Contains("Binah_Se21341"))
                story = "Chapter1";
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StageController), "StartParrying")]
        public static bool StageController_StartParrying_Pre(StageController __instance,
            BattlePlayingCardDataInUnitModel cardA,
            BattlePlayingCardDataInUnitModel cardB, bool __runOriginal)
        {
            if (!__runOriginal) return true;
            try
            {
                var passiveIgnore = ModParameters.PassiveOptions.Any(
                    x => cardA.owner.passiveDetail.PassiveList.Any(y =>
                        x.PackageId == y.id.packageId &&
                        x.PassiveId == y.id.id && x.IgnoreClashPassive));
                var cardIgnore = ModParameters.CardOptions.Any(x =>
                    x.PackageId == cardA.card.GetID().packageId &&
                    x.Ids.Contains(cardA.card.GetID().id) && (x.OneSideOnlyCard || x.OneSideOnlyCardOnlyForAlly));
                if (passiveIgnore || cardIgnore)
                {
                    if (cardB.owner.faction != cardA.owner.faction)
                    {
                        var clashOptions = ModParameters.CardOptions.FirstOrDefault(x =>
                            x.PackageId == cardA.card.GetID().packageId && x.Ids.Contains(cardA.card.GetID().id) &&
                            x.OneSideOnlyCardOnlyForAlly);
                        if (clashOptions != null) return true;
                    }

                    __instance._phase = StageController.StagePhase.ExecuteOneSideAction;
                    cardA.owner.turnState = BattleUnitTurnState.DOING_ACTION;
                    cardA.target.turnState = BattleUnitTurnState.DOING_ACTION;
                    cardB.owner.currentDiceAction = null;
                    Singleton<BattleOneSidePlayManager>.Instance.StartOneSidePlay(cardA);
                    return false;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DiceEffectManager), "CreateBehaviourEffect")]
        public static void DiceEffectManager_CreateBehaviourEffect(ref DiceAttackEffect __result, string resource,
            float scaleFactor, BattleUnitView self, BattleUnitView target, float time)
        {
            if (string.IsNullOrEmpty(resource) || __result != null ||
                !ModParameters.CustomEffects.ContainsKey(resource)) return;
            var componentType = ModParameters.CustomEffects[resource];
            var diceAttackEffect = new GameObject(resource).AddComponent(componentType) as DiceAttackEffect;
            if (diceAttackEffect == null) return;
            diceAttackEffect.Initialize(self, target, time);
            diceAttackEffect.SetScale(scaleFactor);
            __result = diceAttackEffect;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleObjectManager), "GetTargetByCardForPlayer")]
        public static void BattleObjectManager_GetTargetByCardForPlayer(BattleUnitModel actor, BattleDiceCardModel card,
            ref BattleUnitModel __result, bool teamkill = false)
        {
            var cardOption = ModParameters.CardOptions.FirstOrDefault(x =>
                x.PackageId == card.GetID().packageId && x.Ids.Contains(card.GetID().id));
            if (cardOption == null || !cardOption.OnlyAllyTargetCard) return;
            var factions = new List<Faction> { Faction.Player, Faction.Enemy };
            var units = BattleObjectManager.instance.GetAliveList(!actor.IsControlable() && teamkill
                ? RandomUtil.SelectOne(factions)
                : actor.faction);
            if (units == null) return;
            units.RemoveAll(x => x == actor);
            if (units.Any())
                __result = actor.targetSetter.SelectTargetUnit(units);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BattleDiceCardBuf), "GetBufIcon")]
        public static void BattleDiceCardBuf_GetBufIcon(BattleDiceCardBuf __instance)
        {
            if (__instance._iconInit) return;
            if (string.IsNullOrEmpty(__instance.keywordIconId)) return;
            var keywords = __instance.keywordIconId.Split('/');
            if (keywords.Length != 2) return;
            var bufIconCustom =
                ModParameters.ArtWorks.FirstOrDefault(x => x.PackageId == keywords[0] && x.Name == keywords[1]);
            if (bufIconCustom == null) return;
            __instance._iconInit = true;
            __instance._bufIcon = bufIconCustom.Sprite;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SdCharacterUtil), "CreateSkin")]
        public static void SdCharacterUtil_CreateSkin(ref CharacterAppearance __result, UnitDataModel unit,
            Faction faction, Transform characterRoot)
        {
            SkinOptionRoot skin;
            string stringKey;
            if (!string.IsNullOrEmpty(unit.workshopSkin))
            {
                stringKey = unit.workshopSkin;
                skin = ModParameters.SkinOptions.FirstOrDefault(x => x.SkinName == unit.workshopSkin);
            }
            else if (unit.bookItem != unit.CustomBookItem)
            {
                stringKey = unit.CustomBookItem.ClassInfo.GetCharacterSkin();
                skin = ModParameters.SkinOptions.FirstOrDefault(x =>
                    x.SkinName == unit.CustomBookItem.ClassInfo.GetCharacterSkin());
            }
            else
            {
                stringKey = unit.bookItem.ClassInfo.GetCharacterSkin();
                skin = ModParameters.SkinOptions.FirstOrDefault(x =>
                    x.SkinName == unit.bookItem.ClassInfo.GetCharacterSkin());
            }

            if (skin == null) return;
            var customizeData = unit.customizeData;
            var giftInventory = unit.giftInventory;
            Object.Destroy(__result.gameObject);
            var gameObject = Object.Instantiate(
                Singleton<AssetBundleManagerRemake>.Instance.LoadCharacterPrefab(stringKey, "",
                    out var resourceName), characterRoot);
            var workshopBookSkinData =
                Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(
                    skin.PackageId, stringKey);
            gameObject.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
            __result = gameObject.GetComponent<CharacterAppearance>();
            __result.Initialize(resourceName);
            var soundInfo = __result._soundInfo;
            var motionSounds = soundInfo._motionSounds;
            var dic = soundInfo._dic;
            UnitUtil.PrepareSounds(skin.PackageId, motionSounds, dic, skin.MotionSounds);
            __result.InitCustomData(customizeData, unit.defaultBook.GetBookClassInfoId());
            __result.InitGiftDataAll(giftInventory.GetEquippedList());
            __result.ChangeMotion(ActionDetail.Standing);
            __result.ChangeLayer("Character");
            __result.SetLibrarianOnlySprites(faction);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BattleUnitView), "ChangeSkin")]
        public static void BattleUnitView_ChangeSkin(BattleUnitView __instance, string charName)
        {
            var skin = ModParameters.SkinOptions.FirstOrDefault(x => x.SkinName == charName);
            if (skin == null) return;
            var skinInfo = __instance._skinInfo;
            skinInfo.state = BattleUnitView.SkinState.Changed;
            skinInfo.skinName = charName;
            var currentMotionDetail = __instance.charAppearance.GetCurrentMotionDetail();
            __instance.DestroySkin();
            var gameObject =
                Object.Instantiate(
                    Singleton<AssetBundleManagerRemake>.Instance.LoadCharacterPrefab(charName, "",
                        out var resourceName), __instance.model.view.characterRotationCenter);
            var workshopBookSkinData =
                Singleton<CustomizingBookSkinLoader>.Instance.GetWorkshopBookSkinData(
                    skin.PackageId, charName);
            gameObject.GetComponent<WorkshopSkinDataSetter>().SetData(workshopBookSkinData);
            __instance.charAppearance = gameObject.GetComponent<CharacterAppearance>();
            __instance.charAppearance.Initialize(resourceName);
            var soundInfo = __instance.charAppearance._soundInfo;
            var motionSounds = soundInfo._motionSounds;
            var dic = soundInfo._dic;
            UnitUtil.PrepareSounds(skin.PackageId, motionSounds, dic, skin.MotionSounds);
            __instance.charAppearance.ChangeMotion(currentMotionDetail);
            __instance.charAppearance.ChangeLayer("Character");
            __instance.charAppearance.SetLibrarianOnlySprites(__instance.model.faction);
            if (skin.CustomHeight == 0) return;
            __instance.ChangeHeight(skin.CustomHeight);
        }
    }
}