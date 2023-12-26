using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UtilLoader21341.Extensions;

namespace UtilLoader21341.Util
{
    public static class PassiveUtil
    {
        public static void OnWaveStartChangeDialog(this PassiveAbilityBase passive, ref BattleDialogueModel dlg,
            bool hidePassive = true)
        {
            if (hidePassive) passive.Hide();
            var keypageItem = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == passive.owner.Book.BookId.packageId && x.KeypageId == passive.owner.Book.BookId.id);
            if (keypageItem?.BookCustomOptions == null) return;
            dlg = passive.owner.UnitData.unitData.battleDialogModel;
            if (keypageItem.BookCustomOptions.CustomDialogId != null)
                passive.owner.UnitData.unitData.InitBattleDialogByDefaultBook(keypageItem.BookCustomOptions
                    .CustomDialogId.ToLorId());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnRoundStartAfterSpecialDraw(this PassiveAbilityBase passive, List<LorId> cardsIds)
        {
            var cardNumber = RandomUtil.SelectOne(cardsIds);
            var card = passive.owner.allyCardDetail.AddNewCard(cardNumber);
            card.AddBuf(new BattleDiceCardBuf_TempCard_DLL21341());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnUseCardSpecialDraw(this PassiveAbilityBase passive,
            BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.HasBuf<BattleDiceCardBuf_TempCard_DLL21341>())
                passive.owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnRoundEndTheLastSpecialDraw(this PassiveAbilityBase passive)
        {
            passive.owner.allyCardDetail.GetAllDeck()
                .Where(x => x.HasBuf<BattleDiceCardBuf_TempCard_DLL21341>())
                .Do(x => passive.owner.allyCardDetail.ExhaustACardAnywhere(x));
        }

        public static int OnWaveStartUsedEmotionCards(string poolName)
        {
            return Singleton<StageController>.Instance.GetStageModel()
                .GetStageStorageData<int>($"EmotionUnit{poolName}", out var usedEmotionCards)
                ? usedEmotionCards
                : 0;
        }

        public static /*async*/ void OnRoundEndTheLastActiveEmotion(this PassiveAbilityBase passive,
            List<EmotionCardXmlInfo> emotionCardsXml, int selectionMaxNumber, int emotionCards,
            bool onlyForUser = false)
        {
            //await GenericUtil.PutTaskDelay(1000);
            passive.owner.emotionDetail.CheckLevelUp();
            if (passive.owner.emotionDetail.EmotionLevel > selectionMaxNumber + 1 ||
                emotionCards >= passive.owner.emotionDetail.EmotionLevel) return;
            CustomEmotionTool.SetParameters(new CustomEmotionParameters
            {
                EmotionCards = emotionCardsXml,
                BookId = passive.owner.Book.BookId,
                IsOnlyForUser = onlyForUser,
                EmotionLevel = passive.owner.emotionDetail.EmotionLevel
            });
        }

        public static void OnBattleEndEmotion(string poolName, int emotionCards)
        {
            var stageModel = Singleton<StageController>.Instance.GetStageModel();
            stageModel.SetStageStorgeData($"EmotionUnit{poolName}", emotionCards);
        }

        public static void ChangePassiveItem(PassiveXmlList instance, string packageId)
        {
            var passiveOptions =
                ModParameters.PassiveOptions.Where(x => x.PackageId == packageId && x.IsBaseGamePassive);
            foreach (var passiveToChange in passiveOptions)
            {
                var passive = instance.GetDataAll().FirstOrDefault(x =>
                    string.IsNullOrEmpty(x.id.packageId) && x.id.id == passiveToChange.PassiveId);
                if (passive == null) continue;
                passive.script = passiveToChange.Script;
            }
        }

        public static void ChangeLoneFixerPassive()
        {
            var loneFixerPassive = PassiveXmlList.Instance.GetDataAll()
                .FirstOrDefault(x => string.IsNullOrEmpty(x.id.packageId) && x.id.id == 230008);
            if (loneFixerPassive == null) return;
            loneFixerPassive.script = "LoneFixer_DLL21341";
        }

        //Private because it should not be used
        private static void AddPassiveItem(List<PassiveXmlInfo> passives)
        {
            if (!passives.Any()) return;
            PassiveXmlList.Instance.AddPassivesByMod(passives);
        }
    }
}