using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UtilLoader21341.Enum;
using UtilLoader21341.Extensions;
using Object = UnityEngine.Object;

namespace UtilLoader21341.Util
{
    public static class ArtUtil
    {
        public static void GetArtWorks(DirectoryInfo dir, string packageId)
        {
            try
            {
                if (dir.GetDirectories().Length != 0)
                {
                    var directories = dir.GetDirectories();
                    foreach (var t in directories) GetArtWorks(t, packageId);
                }

                foreach (var fileInfo in dir.GetFiles())
                {
                    var texture2D = new Texture2D(2, 2);
                    texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                    var value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                        new Vector2(0f, 0f));
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                    ModParameters.ArtWorks.Add(new CustomSprite
                        { PackageId = packageId, Name = fileNameWithoutExtension, Sprite = value });
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void SetEpisodeSlots(UIBookStoryChapterSlot instance, UIBookStoryPanel panel,
            List<UIBookStoryEpisodeSlot> episodeSlots)
        {
            var listOfAddedSlots = new List<UIBookStoryEpisodeSlot>();
            foreach (var packageId in ModParameters.PackageIds)
            {
                var episodeSlot = episodeSlots.FirstOrDefault(x => !listOfAddedSlots.Contains(x) && x.books.Any(y =>
                    y.id.packageId == packageId));
                foreach (var category in ModParameters.CategoryOptions.Where(x => x.PackageId == packageId))
                    switch (category.CredenzaType)
                    {
                        case CredenzaEnum.ModifiedCredenza:
                        {
                            var panelData = panel.panel.GetChapterBooksData(instance.chapter);
                            if (panelData == null) continue;
                            var panelBooks = panelData.FindAll(x =>
                                x.id.packageId == category.PackageId && category.CredenzaBooksId.Contains(x.id.id));
                            if (panelBooks.Any())
                            {
                                var newSlot = InstatiateAdditionalSlot(instance);
                                newSlot.Init(panelBooks, instance);
                                newSlot.episodeText.text = GenericUtil.GetEffectText(packageId,
                                    "Category", category.CategoryNameId, true);
                                var icon = GetIcon(category.CustomIconSpriteId,
                                    category.BaseIconSpriteId,
                                    panelBooks[0].BookIcon, category.PackageId);
                                newSlot.episodeIconGlow.sprite = icon;
                                newSlot.episodeIcon.sprite = icon;
                                listOfAddedSlots.Add(newSlot);
                            }

                            episodeSlot?.books.RemoveAll(x =>
                                x.id.packageId == category.PackageId && category.CredenzaBooksId.Contains(x.id.id));
                            break;
                        }
                        case CredenzaEnum.NoCredenza:
                        {
                            episodeSlot?.books.RemoveAll(x =>
                                x.id.packageId == category.PackageId && category.CredenzaBooksId.Contains(x.id.id));
                            break;
                        }
                    }

                episodeSlot?.books.RemoveAll(x =>
                    x.id.packageId == packageId);
            }
        }

        public static UIBookStoryEpisodeSlot InstatiateAdditionalSlot(UIBookStoryChapterSlot instance)
        {
            var storyEpisodeSlot = Object.Instantiate(instance.EpisodeSlotModel, instance.EpisodeSlotsListRect);
            storyEpisodeSlot.selectable.ChildSelectable = instance.panel.bookSlots[0].selectable;
            storyEpisodeSlot.selectable.parentSelectable = instance.selectable;
            instance.EpisodeSlots.Add(storyEpisodeSlot);
            if (instance.EpisodeSlots.Count >= 2)
            {
                instance.EpisodeSlots[instance.EpisodeSlots.Count - 1].selectable.SetNavigationObject(NavigationDir.Up,
                    instance.EpisodeSlots[instance.EpisodeSlots.Count - 2].selectable);
                instance.EpisodeSlots[instance.EpisodeSlots.Count - 2].selectable
                    .SetNavigationObject(NavigationDir.Down,
                        instance.EpisodeSlots[instance.EpisodeSlots.Count - 1].selectable);
            }
            else
            {
                if (instance.EpisodeSlots.Count != 1)
                    return storyEpisodeSlot;
                instance.selectable.ChildSelectable = instance.EpisodeSlots[instance.EpisodeSlots.Count - 1].selectable;
            }

            return storyEpisodeSlot;
        }

        private static Sprite GetIcon(string customIconId, string baseIconId, string baseIcon, string packageId)
        {
            return ModParameters.ArtWorks.FirstOrDefault(x => x.PackageId == packageId && x.Name == customIconId)
                       ?.Sprite ??
                   UISpriteDataManager.instance.GetStoryIcon(string.IsNullOrEmpty(baseIconId)
                       ? baseIcon
                       : baseIconId).icon;
        }

        public static void GetThumbSprite(LorId bookId, ref Sprite result)
        {
            if (!ModParameters.PackageIds.Contains(bookId.packageId)) return;
            var spriteOptions =
                ModParameters.SpriteOptions.FirstOrDefault(x =>
                    x.PackageId == bookId.packageId && x.Ids.Contains(bookId.id));
            if (spriteOptions == null) return;
            switch (spriteOptions.SpriteOption)
            {
                case SpriteEnum.Base:
                    result = Resources.Load<Sprite>(spriteOptions.SpritePK);
                    break;
                case SpriteEnum.Custom:
                    var spriteArt = ModParameters.ArtWorks.FirstOrDefault(x =>
                        x.PackageId == bookId.packageId && x.Name == spriteOptions.SpritePK);
                    if (spriteArt != null) result = spriteArt.Sprite;
                    break;
            }
        }

        public static void OnSelectEpisodeSlot(UIBookStoryPanel instance,
            UIBookStoryEpisodeSlot slot, TextMeshProUGUI selectedEpisodeText, Image selectedEpisodeIcon,
            Image selectedEpisodeIconGlow)
        {
            if (slot == null) return;
            if (slot.books.Exists(x => x.id.IsBasic())) return;
            var book = slot.books.FirstOrDefault(x => ModParameters.PackageIds.Contains(x.id.packageId));
            if (book == null) return;
            var category = ModParameters.CategoryOptions.FirstOrDefault(x =>
                x.PackageId == book.id.packageId && x.CredenzaBooksId.Contains(book.id.id));
            if (category == null) return;
            selectedEpisodeText.text = GenericUtil.GetEffectText(category.PackageId,
                "Category", category.CategoryNameId, true);
            var iconCategory = GetIcon(category.CustomIconSpriteId, category.BaseIconSpriteId,
                slot.books[0].BookIcon, category.PackageId);
            selectedEpisodeIcon.sprite = iconCategory;
            selectedEpisodeIconGlow.sprite = iconCategory;
            instance.UpdateBookSlots();
        }

        public static void SetBooksData(UISettingInvenEquipPageListSlot instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            var categoryOption =
                ModParameters.CategoryOptions.FirstOrDefault(
                    x => storyKey.workshopId == x.PackageId + "_" + x.AdditionalValue);
            if (categoryOption == null || books.Count < 0) return;
            instance.img_IconGlow.enabled = true;
            instance.img_Icon.enabled = true;
            var icon = GetIcon(categoryOption.CustomIconSpriteId, categoryOption.BaseIconSpriteId,
                "Chapter" + storyKey.chapter, categoryOption.PackageId);
            instance.img_Icon.sprite = icon;
            instance.img_IconGlow.sprite = icon;
            instance.txt_StoryName.text = GenericUtil.GetEffectText(categoryOption.PackageId,
                "Category", categoryOption.CategoryNameId, true);
        }

        public static void SetBooksData(UIInvenEquipPageListSlot instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            var categoryOption =
                ModParameters.CategoryOptions.FirstOrDefault(
                    x => storyKey.workshopId == x.PackageId + "_" + x.AdditionalValue);
            if (categoryOption == null || books.Count < 0) return;
            instance.img_IconGlow.enabled = true;
            instance.img_Icon.enabled = true;
            var icon = GetIcon(categoryOption.CustomIconSpriteId, categoryOption.BaseIconSpriteId,
                "Chapter" + storyKey.chapter, categoryOption.PackageId);
            instance.img_Icon.sprite = icon;
            instance.img_IconGlow.sprite = icon;
            instance.txt_StoryName.text = GenericUtil.GetEffectText(categoryOption.PackageId,
                "Category", categoryOption.CategoryNameId, true);
        }

        public static void SetMainData(List<BookModel> currentBookModelList, List<UIStoryKeyData> totalkeysdata,
            Dictionary<UIStoryKeyData, List<BookModel>> currentStoryBooksDic)
        {
            var categoryOptions =
                ModParameters.CategoryOptions.Where(x =>
                    currentBookModelList.Exists(y => y.BookId.packageId == x.PackageId)).ToList();
            foreach (var packageId in ModParameters.PackageIds)
            {
                var index = totalkeysdata.FindIndex(x => x.IsWorkshop && x.workshopId == packageId);
                if (index == -1) continue;
                totalkeysdata.RemoveAt(index);
                foreach (var categoryOption in categoryOptions.Where(x => x.PackageId == packageId)
                             .OrderBy(x => x.Order))
                {
                    var categoryKey =
                        currentStoryBooksDic.FirstOrDefault(x =>
                            x.Key.IsWorkshop && x.Key.workshopId == categoryOption.PackageId);
                    if (categoryKey.Key != null) currentStoryBooksDic.Remove(categoryKey.Key);

                    var actualKey = new UIStoryKeyData(categoryOption.Chapter,
                        categoryOption.PackageId + $"_{categoryOption.AdditionalValue}");
                    if (totalkeysdata.Contains(actualKey) && !categoryOption.BaseGameCategory.HasValue)
                        totalkeysdata.Remove(actualKey);
                    var bookFound = false;
                    if (categoryOption.BaseGameCategory.HasValue)
                        actualKey = totalkeysdata.Find(x => x.StoryLine == categoryOption.BaseGameCategory.Value);
                    foreach (var book in categoryOption.CategoryBooksId.SelectMany(bookId =>
                                 currentBookModelList.Where(x =>
                                     x.BookId.packageId == categoryOption.PackageId && x.BookId.id == bookId)))
                    {
                        if (actualKey == null)
                        {
                            actualKey = new UIStoryKeyData(book.ClassInfo.Chapter,
                                categoryOption.BaseGameCategory.Value);
                            totalkeysdata.Add(actualKey);
                        }

                        bookFound = true;
                        if (!currentStoryBooksDic.ContainsKey(actualKey))
                        {
                            var list = new List<BookModel> { book };
                            currentStoryBooksDic.Add(actualKey, list);
                        }
                        else
                        {
                            currentStoryBooksDic[actualKey].Add(book);
                        }
                    }

                    if (!bookFound || categoryOption.BaseGameCategory.HasValue) continue;
                    totalkeysdata.Insert(index, actualKey);
                    index++;
                }
            }
        }

        public static IEnumerable<BattleDiceCardModel> ReloadEgoHandUI(BattleUnitCardsInHandUI instance,
            List<BattleDiceCardUI> cardList, BattleUnitModel unit, List<BattleDiceCardUI> activatedCardList,
            ref float xInt)
        {
            var list = unit.personalEgoDetail.GetHand();
            if (list.Count >= 9) xInt = 65f * 8f / list.Count;
            var num = 0;
            activatedCardList.Clear();
            while (num < list.Count)
            {
                cardList[num].gameObject.SetActive(true);
                cardList[num].SetCard(list[num], Array.Empty<BattleDiceCardUI.Option>());
                cardList[num].SetDefault();
                cardList[num].ResetSiblingIndex();
                activatedCardList.Add(cardList[num]);
                num++;
            }

            for (var i = 0; i < activatedCardList.Count; i++)
            {
                var navigation = default(Navigation);
                navigation.mode = Navigation.Mode.Explicit;
                if (i > 0)
                    navigation.selectOnLeft = activatedCardList[i - 1].selectable;
                else if (activatedCardList.Count >= 2)
                    navigation.selectOnLeft = activatedCardList[activatedCardList.Count - 1].selectable;
                else
                    navigation.selectOnLeft = null;
                if (i < activatedCardList.Count - 1)
                    navigation.selectOnRight = activatedCardList[i + 1].selectable;
                else if (activatedCardList.Count >= 2)
                    navigation.selectOnRight = activatedCardList[0].selectable;
                else
                    navigation.selectOnRight = null;
                activatedCardList[i].selectable.navigation = navigation;
                activatedCardList[i].selectable.parentSelectable = instance.selectablePanel;
            }

            return list;
        }

        public static void PrepareMultiDeckUI(GameObject multiDeckUI, List<string> labels, string packageId)
        {
            var uiButtons = multiDeckUI.GetComponentsInChildren<UICustomTabButton>(true);
            var num = 0;
            foreach (var uiButton in uiButtons)
            {
                if (num < labels.Count && !string.IsNullOrEmpty(labels[num]))
                    uiButton.TabName.text = GenericUtil.GetEffectText(packageId, "Not Found", labels[num]);
                else
                    uiButton.gameObject.SetActive(false);
                num++;
            }
        }

        public static void RevertMultiDeckUI(GameObject multiDeckUI)
        {
            var uiButtons = multiDeckUI.GetComponentsInChildren<UICustomTabButton>(true);
            foreach (var uiButton in uiButtons)
                uiButton.gameObject.SetActive(true);
            var num = 0;
            foreach (var uiButton in uiButtons)
            {
                switch (num)
                {
                    case 0:
                        uiButton.TabName.text = TextDataModel.GetText("ui_slash_form");
                        break;
                    case 1:
                        uiButton.TabName.text = TextDataModel.GetText("ui_penetrate_form");
                        break;
                    case 2:
                        uiButton.TabName.text = TextDataModel.GetText("ui_hit_form");
                        break;
                    case 3:
                        uiButton.TabName.text = TextDataModel.GetText("ui_defense_form");
                        break;
                }

                num++;
            }
        }

        public static void PreLoadBufIcons()
        {
            foreach (var baseGameIcon in Resources.LoadAll<Sprite>("Sprites/BufIconSheet/")
                         .Where(x => !BattleUnitBuf._bufIconDictionary.ContainsKey(x.name)))
                BattleUnitBuf._bufIconDictionary.Add(baseGameIcon.name, baseGameIcon);
            foreach (var artWork in ModParameters.ArtWorks.Where(x =>
                         !x.Name.Contains("Glow") && !x.Name.Contains("Default") &&
                         !BattleUnitBuf._bufIconDictionary.ContainsKey(x.Name)))
                BattleUnitBuf._bufIconDictionary.Add(artWork.Name, artWork.Sprite);
        }

        public static void MakeCustomBook(string packageId)
        {
            var customSkins = ModParameters.CustomSkinOptions.Where(x => x.PackageId == packageId).ToList();
            if (!customSkins.Any()) return;
            var dictionary = Singleton<CustomizingResourceLoader>.Instance._skinData;
            foreach (var workshopSkinData in Singleton<CustomizingBookSkinLoader>.Instance
                         .GetWorkshopBookSkinData(packageId).Where(workshopSkinData =>
                             !workshopSkinData.dataName.Contains("x_proj")))
            {
                var customSkinOption = customSkins.FirstOrDefault(x => workshopSkinData.dataName.Contains(x.SkinName));
                if (customSkinOption == null) continue;
                var keypageName = string.Empty;
                if (customSkinOption.UseLocalization)
                {
                    var localization = ModParameters.LocalizedItems.TryGetValue(packageId, out var localizatedItem);
                    if (localization)
                        if (customSkinOption.KeypageId.HasValue)
                        {
                            var keypageLoc =
                                localizatedItem.Keypages.FirstOrDefault(x =>
                                    x.bookID == customSkinOption.KeypageId.Value);
                            if (keypageLoc != null) keypageName = keypageLoc.bookName;
                        }
                        else if (!string.IsNullOrEmpty(customSkinOption.KeypageName))
                        {
                            keypageName = customSkinOption.KeypageName;
                        }
                }

                dictionary.Add(workshopSkinData.dataName, new WorkshopSkinDataExtension
                {
                    dic = workshopSkinData.dic,
                    contentFolderIdx = workshopSkinData.dataName,
                    dataName = string.IsNullOrEmpty(keypageName) ? workshopSkinData.dataName : keypageName,
                    id = dictionary.Count,
                    PackageId = packageId,
                    RealKeypageId = customSkinOption.KeypageId
                });
            }
        }

        public static void LocalizationCustomBook()
        {
            var dictionary = Singleton<CustomizingResourceLoader>.Instance._skinData;
            var dictionaryChanged = dictionary.Where(x => x.Value is WorkshopSkinDataExtension).Select(x =>
                    new KeyValuePair<string, WorkshopSkinDataExtension>(x.Key, x.Value as WorkshopSkinDataExtension))
                .ToList();
            foreach (var packageId in ModParameters.PackageIds)
            foreach (var workshopSkinData in dictionaryChanged
                         .Where(x => ModParameters.CustomSkinOptions.Exists(y =>
                             y.SkinName == x.Key && x.Value.PackageId == packageId))
                         .ToList())
            {
                var customSkinOption =
                    ModParameters.CustomSkinOptions.FirstOrDefault(x => workshopSkinData.Key.Contains(x.SkinName));
                if (customSkinOption?.KeypageId == null || !customSkinOption.UseLocalization) continue;
                var localization = ModParameters.LocalizedItems.TryGetValue(packageId, out var localizatedItem);
                if (!localization) continue;
                var keypageLoc =
                    localizatedItem.Keypages.FirstOrDefault(x => x.bookID == customSkinOption.KeypageId.Value);
                if (keypageLoc == null) continue;
                workshopSkinData.Value.dataName = keypageLoc.bookName;
                dictionary[workshopSkinData.Key] = workshopSkinData.Value;
            }
        }

        public static void InitCustomEffects(List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                assembly.GetTypes().ToList().FindAll(x => x.Name.StartsWith("DiceAttackEffect_"))
                    .ForEach(delegate(Type x)
                    {
                        ModParameters.CustomEffects[x.Name.Replace("DiceAttackEffect_", "")] = x;
                    });
        }
    }
}