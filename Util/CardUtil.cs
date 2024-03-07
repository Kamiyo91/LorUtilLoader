using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EmotionCardUtil;
using HarmonyLib;
using LOR_DiceSystem;
using UnityEngine;
using UtilLoader21341.Comparers;
using UtilLoader21341.GameObjects;
using UtilLoader21341.Models;
using LorIdRoot = UtilLoader21341.Models.LorIdRoot;
using Object = UnityEngine.Object;

namespace UtilLoader21341.Util
{
    public static class CardUtil
    {
        public static void ChangeCardItem(ItemXmlDataList instance, string packageId)
        {
            try
            {
                var defaultKeyword = ModParameters.DefaultKeyWordOptions.FirstOrDefault(x => x.PackageId == packageId);
                var dictionary = instance._cardInfoTable;
                var list = instance._cardInfoList;
                if (dictionary == null) return;
                var cardsToChange = ModParameters.CardOptions.Where(x => x.PackageId == packageId).ToList();
                foreach (var cardOption in cardsToChange)
                foreach (var item in dictionary.Where(x =>
                             x.Key.packageId == packageId && cardOption.Ids.Contains(x.Key.id)))
                    SetCustomCardOption(cardOption.Option, cardOption.Keywords, item.Value, list,
                        defaultKeyword?.Keyword ?? "");
                var baseGameCardsToChange =
                    ModParameters.CardOptions.Where(x => x.PackageId == packageId && x.IsBaseGameCard);
                foreach (var cardOption in baseGameCardsToChange)
                foreach (var item in dictionary
                             .Where(x => string.IsNullOrEmpty(x.Key.packageId) && cardOption.Ids.Contains(x.Key.id))
                             .ToList())
                    SetCustomCardOption(cardOption.Option, cardOption.Keywords, item.Value, list,
                        defaultKeyword?.Keyword ?? "");
                foreach (var item in dictionary
                             .Where(x => x.Key.packageId == packageId && !cardsToChange.Exists(y =>
                                 x.Key.packageId == y.PackageId && y.Ids.Contains(x.Key.id)))
                             .ToList())
                    item.Value.Keywords.SetKeywords(new List<string> { defaultKeyword?.Keyword ?? "" });
            }
            catch (Exception ex)
            {
                Debug.LogError("There was an error while changing the Cards values " + ex.Message + " ModId : " +
                               packageId);
            }
        }

        private static void SetCustomCardOption(CardOption option, IEnumerable<string> keywords, DiceCardXmlInfo card,
            ICollection<DiceCardXmlInfo> cardXmlList, string defaultKeyword = "")
        {
            if (!string.IsNullOrEmpty(defaultKeyword)) keywords = keywords.Prepend(defaultKeyword);
            else if (option == CardOption.Basic) return;
            card.Keywords.SetKeywords(keywords.ToList());
            card.optionList.Add(option);
            cardXmlList.Add(card);
        }

        private static void SetKeywords(this List<string> cardKeywords, IReadOnlyCollection<string> keywords)
        {
            if (keywords.All(string.IsNullOrEmpty)) return;
            cardKeywords.AddRange(keywords.Where(x => !cardKeywords.Contains(x)));
            cardKeywords = cardKeywords.OrderBy(x =>
            {
                var index = x.IndexOf("ModPage", StringComparison.InvariantCultureIgnoreCase);
                return index < 0 ? 9999 : index;
            }).ToList();
        }

        public static void InitKeywordsList(List<Assembly> assemblies)
        {
            var dictionary = BattleCardAbilityDescXmlList.Instance._dictionaryKeywordCache;
            foreach (var assembly in assemblies)
            {
                assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DiceCardSelfAbilityBase))
                                               && x.Name.StartsWith("DiceCardSelfAbility_"))
                    .Do(x => dictionary[x.Name.Replace("DiceCardSelfAbility_", "")] =
                        new List<string>(((DiceCardSelfAbilityBase)Activator.CreateInstance(x)).Keywords));
                assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DiceCardAbilityBase))
                                               && x.Name.StartsWith("DiceCardAbility_"))
                    .Do(x => dictionary[x.Name.Replace("DiceCardAbility_", "")] =
                        new List<string>(((DiceCardAbilityBase)Activator.CreateInstance(x)).Keywords));
            }
        }

        public static List<EmotionCardXmlInfo> CustomCreateSelectableList(int emotionLevel,
            IEnumerable<EmotionCardXmlInfo> cards)
        {
            var emotionLevelPull = emotionLevel <= 2 ? 1 : emotionLevel <= 4 ? 2 : 3;
            var dataCardList = cards.Where(x => x.EmotionLevel == emotionLevelPull && !x.Locked).ToList();
            if (!dataCardList.Any()) return dataCardList;
            var instance = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            var selectedList = instance._selectedList;
            if (selectedList != null && selectedList.Any())
                foreach (var item in selectedList)
                    dataCardList.Remove(item);
            var center = CalcuateSelectionCoins(instance, emotionLevel);
            dataCardList.Sort((x, y) => Mathf.Abs(x.EmotionRate - center) - Mathf.Abs(y.EmotionRate - center));
            var list = new List<EmotionCardXmlInfo>();
            while (dataCardList.Count > 0 && list.Count < 3)
            {
                var er = Mathf.Abs(dataCardList[0].EmotionRate - center);
                var list2 = dataCardList.FindAll(x => Mathf.Abs(x.EmotionRate - center) == er);
                if (list2.Count + list.Count <= 3)
                {
                    list.AddRange(list2);
                    using (var enumerator2 = list2.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            var item2 = enumerator2.Current;
                            dataCardList.Remove(item2);
                        }

                        continue;
                    }
                }

                var i = 0;
                while (i < 3 - list.Count && list2.Count != 0)
                {
                    var item3 = RandomUtil.SelectOne(list2);
                    list2.Remove(item3);
                    dataCardList.Remove(item3);
                    list.Add(item3);
                    i++;
                }
            }

            return list;
        }

        public static List<EmotionCardXmlInfo> CreateSephirahSelectableList(int emotionLevel, SephirahType sephirah)
        {
            var emotionLevelPull = emotionLevel <= 2 ? 1 : emotionLevel <= 4 ? 2 : 3;
            var floorLevel = 0;
            var floor = LibraryModel.Instance.GetFloor(sephirah);
            if (floor != null)
                floorLevel = Singleton<StageController>.Instance.IsRebattle ? floor.TemporaryLevel : floor.Level;
            var dataCardList =
                Singleton<EmotionCardXmlList>.Instance.GetDataList(sephirah, floorLevel, emotionLevelPull);
            if (!dataCardList.Any()) return dataCardList;
            var instance = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            var selectedList = instance._selectedList;
            if (selectedList != null && selectedList.Any())
                foreach (var item in selectedList)
                    dataCardList.Remove(item);
            var center = CalcuateSelectionCoins(instance, emotionLevel);
            dataCardList.Sort((x, y) => Mathf.Abs(x.EmotionRate - center) - Mathf.Abs(y.EmotionRate - center));
            var list = new List<EmotionCardXmlInfo>();
            while (dataCardList.Count > 0 && list.Count < 3)
            {
                var er = Mathf.Abs(dataCardList[0].EmotionRate - center);
                var list2 = dataCardList.FindAll(x => Mathf.Abs(x.EmotionRate - center) == er);
                if (list2.Count + list.Count <= 3)
                {
                    list.AddRange(list2);
                    using (var enumerator2 = list2.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            var item2 = enumerator2.Current;
                            dataCardList.Remove(item2);
                        }

                        continue;
                    }
                }

                var i = 0;
                while (i < 3 - list.Count && list2.Count != 0)
                {
                    var item3 = RandomUtil.SelectOne(list2);
                    list2.Remove(item3);
                    dataCardList.Remove(item3);
                    list.Add(item3);
                    i++;
                }
            }

            return list;
        }

        public static int CalcuateSelectionCoins(StageLibraryFloorModel instance, int emotionLevel)
        {
            var num = 0;
            var num2 = 0;
            var unitList = instance._unitList;
            if (unitList == null || !unitList.Any()) return 0;
            foreach (var unitBattleDataModel in
                     unitList.Where(unitBattleDataModel => unitBattleDataModel.IsAddedBattle))
            {
                num += unitBattleDataModel.emotionDetail.totalPositiveCoins.Count;
                num2 += unitBattleDataModel.emotionDetail.totalNegativeCoins.Count;
            }

            var num4 = num + num2 > 0 ? (num - num2) / (float)(num + num2) : 0.5f;
            var num5 = num4 / ((11f - emotionLevel) / 10f);
            var center = Mathf.Abs(num5) < 0.1 ? 0 : Mathf.Abs(num5) < 0.3 ? num5 > 0f ? 1 : -1 : num5 > 0f ? 2 : -2;
            return center;
        }

        public static List<EmotionEgoXmlInfo> CustomCreateSelectableEgoList(List<EmotionEgoXmlInfo> dataEgoCardList)
        {
            var sephirah = Singleton<StageController>.Instance.GetCurrentStageFloorModel().Sephirah;
            var egoCardList = new List<EmotionEgoXmlInfo>();
            if (!Singleton<SpecialCardListModel>.Instance._cardSelectedDataByFloor.TryGetValue(sephirah,
                    out var cardList)) return egoCardList;
            if (cardList.Any())
                using (var enumerator = cardList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        var cardModel = enumerator.Current;
                        dataEgoCardList.RemoveAll(x => cardModel != null && x.CardId == cardModel.GetID());
                    }
                }

            egoCardList.AddRange(MathUtil.Combination(3, dataEgoCardList.Count)
                .Select(index => dataEgoCardList[index]));
            return egoCardList;
        }

        public static List<EmotionEgoXmlInfo> GetCustomEgoCardsList(List<LorIdRoot> Ids)
        {
            return Ids.Select(cardId => EmotionCardLoader.GetEmotionEgoCard(cardId.PackageId, cardId.Id)).ToList();
        }

        public static List<EmotionCardXmlInfo> GetCustomEmotionCardsList(List<LorIdRoot> Ids)
        {
            return Ids.Select(cardId => EmotionCardLoader.GetEmotionCard(cardId.PackageId, cardId.Id)).ToList();
        }

        public static void RevertAbnoAndEgo(SephirahType sephirah)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(sephirah, out var savedOptions)) return;
            var emotionCardList = EmotionCardXmlList.Instance._list;
            if (emotionCardList != null)
            {
                foreach (var card in emotionCardList.Where(x => x.Sephirah == sephirah))
                    card.Sephirah = SephirahType.ETC;
                foreach (var card in emotionCardList.Where(x =>
                             x.Name.Contains($"RevertCardUtilLoaderDLL21431{sephirah}")))
                {
                    card.Name = card.Name.Replace($"RevertCardUtilLoaderDLL21431{sephirah}", "");
                    card.Sephirah = sephirah;
                }
            }

            var floorEgoCards = savedOptions.FloorOptions.OriginalEgoCards;
            var emotionEgoCardList = EmotionEgoXmlList.Instance._list;
            if (emotionEgoCardList == null) return;
            foreach (var card in emotionEgoCardList.Where(x => x.Sephirah == sephirah))
                card.Sephirah = SephirahType.ETC;
            foreach (var card in emotionEgoCardList.Where(x =>
                         x.Sephirah == SephirahType.ETC && floorEgoCards.Exists(y => y.id == x.id)))
                card.Sephirah = sephirah;
        }

        public static void SaveCardsBeforeChange(SephirahType sephirah)
        {
            if (!ModParameters.EgoAndEmotionCardChanged.TryGetValue(sephirah, out var savedOptions)) return;
            var listEmotionXmlCards = EmotionCardXmlList.Instance._list;
            if (listEmotionXmlCards != null)
            {
                var listEmotionCards = (from emotionCardXmlInfo in listEmotionXmlCards
                    where emotionCardXmlInfo.Sephirah == sephirah
                    select new EmotionCardXmlInfo
                    {
                        Name = emotionCardXmlInfo.Name,
                        _artwork = emotionCardXmlInfo._artwork,
                        State = emotionCardXmlInfo.State,
                        Sephirah = sephirah,
                        EmotionLevel = emotionCardXmlInfo.EmotionLevel,
                        TargetType = emotionCardXmlInfo.TargetType,
                        Script = emotionCardXmlInfo.Script,
                        Level = emotionCardXmlInfo.Level,
                        EmotionRate = emotionCardXmlInfo.EmotionRate,
                        Locked = emotionCardXmlInfo.Locked
                    }).ToList();
                savedOptions.FloorOptions.OriginalEmotionCards = listEmotionCards;
            }

            var listEmotionEgoXmlCards = EmotionEgoXmlList.Instance._list;
            if (listEmotionEgoXmlCards == null) return;
            var listFloorEgoCards = (from emotionEgoXmlInfo in listEmotionEgoXmlCards
                where emotionEgoXmlInfo.Sephirah == sephirah
                select new EmotionEgoXmlInfo
                {
                    _CardId = emotionEgoXmlInfo._CardId,
                    id = emotionEgoXmlInfo.id,
                    Sephirah = sephirah,
                    isLock = emotionEgoXmlInfo.isLock
                }).ToList();
            savedOptions.FloorOptions.OriginalEgoCards = listFloorEgoCards;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ChangeAbnoAndEgo(SephirahType sephirah, CustomFloorOptionRoot floorOptions)
        {
            var customEmotionCardList = EmotionCardUtil.ModParameters.EmotionCards.Where(x =>
                floorOptions.EmotionCardsId.Contains(x.LorId.ToLorIdRoot(), new LorIdRootComparer())).ToList();

            if (customEmotionCardList.Any())
            {
                var listEmotionXmlCards = EmotionCardXmlList.Instance._list;
                if (listEmotionXmlCards != null)
                {
                    foreach (var item in listEmotionXmlCards.Where(x => x.Sephirah == sephirah))
                    {
                        item.Name = $"{item.Name}RevertCardUtilLoaderDLL21431{sephirah}";
                        item.Sephirah = SephirahType.ETC;
                    }

                    foreach (var card in listEmotionXmlCards.Where(x => customEmotionCardList.Contains(x)))
                        card.Sephirah = sephirah;
                }
            }

            var customEmotionEgoCardList = EmotionCardUtil.ModParameters.EmotionEgoCards
                .Where(x => floorOptions.EgoCardsId.Exists(y => x.id == y.Id && x.PackageId == y.PackageId)).ToList();
            if (!customEmotionEgoCardList.Any()) return;
            var listEmotionEgoXmlCards = EmotionEgoXmlList.Instance._list;
            if (listEmotionEgoXmlCards == null) return;
            foreach (var item in listEmotionEgoXmlCards.Where(x => x.Sephirah == sephirah))
                item.Sephirah = SephirahType.ETC;
            foreach (var card in listEmotionEgoXmlCards.Where(x => customEmotionEgoCardList.Contains(x)))
                card.Sephirah = sephirah;
        }

        public static void PutCounterDieAsFirst(BattleUnitModel owner, Type dieAbilityType)
        {
            var diceList = owner.cardSlotDetail.keepCard.cardBehaviorQueue.ToList();
            owner.cardSlotDetail.keepCard.cardBehaviorQueue?.Clear();
            foreach (var die in diceList.OrderBy(x => x.abilityList.Any(y => y.GetType() == dieAbilityType) ? 1 : 2))
                owner.cardSlotDetail.keepCard.cardBehaviorQueue?.Enqueue(die);
        }

        public static void PutCounterDieAsLast(BattleUnitModel owner, Type dieAbilityType)
        {
            var diceList = owner.cardSlotDetail.keepCard.cardBehaviorQueue.ToList();
            owner.cardSlotDetail.keepCard.cardBehaviorQueue?.Clear();
            foreach (var die in diceList.OrderBy(x => x.abilityList.Any(y => y.GetType() == dieAbilityType) ? 2 : 1))
                owner.cardSlotDetail.keepCard.cardBehaviorQueue?.Enqueue(die);
        }

        public static void PrepareCounterDieOrderGameObject(BattleUnitModel owner, Type dieAbilityType, bool isFirst)
        {
            var gameobj = new GameObject();
            var mono = gameobj.AddComponent<ChangeDiceOrderGameObject>();
            Object.Instantiate(gameobj);
            mono.SetParameters(owner, dieAbilityType, isFirst);
            mono.Init();
        }

        public static void FillDictionary()
        {
            var sephirahTypeList = new List<SephirahType>
            {
                SephirahType.Keter, SephirahType.Hokma, SephirahType.Binah,
                SephirahType.Chesed, SephirahType.Gebura, SephirahType.Tiphereth,
                SephirahType.Netzach, SephirahType.Hod, SephirahType.Yesod, SephirahType.Malkuth
            };
            if (ModParameters.DaatFloorFound) sephirahTypeList.Add((SephirahType)12);
            foreach (var sephirah in sephirahTypeList)
                ModParameters.EgoAndEmotionCardChanged.Add(sephirah, new SavedFloorOptions());
        }
    }
}