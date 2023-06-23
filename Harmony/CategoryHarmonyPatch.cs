using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UI;
using UtilLoader21341.Util;

namespace UtilLoader21341.Harmony
{
    [HarmonyPatch]
    public class CategoryHarmonyPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(0)]
        [HarmonyPatch(typeof(UIBookStoryChapterSlot), "SetEpisodeSlots")]
        public static void UIBookStoryChapterSlot_SetEpisodeSlots(UIBookStoryChapterSlot __instance)
        {
            ArtUtil.SetEpisodeSlots(__instance, __instance.panel, __instance.EpisodeSlots);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIStoryArchivesPanel), "GetEpisodeBooksData")]
        public static void UIStoryArchivesPanel_GetEpisodeBooksData(UIStoryLine ep, ref List<BookXmlInfo> __result)
        {
            var categoryOptions =
                ModParameters.CategoryOptions.Where(x => x.BaseGameCategory != null && x.BaseGameCategory == ep);
            __result.AddRange(categoryOptions.SelectMany(x =>
                x.CredenzaBooksId.Select(y => Singleton<BookXmlList>.Instance.GetData(new LorId(x.PackageId, y)))));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIBookStoryPanel), "OnSelectEpisodeSlot")]
        public static void UIBookStoryPanel_OnSelectEpisodeSlot(UIBookStoryPanel __instance,
            UIBookStoryEpisodeSlot slot)
        {
            ArtUtil.OnSelectEpisodeSlot(__instance, slot, __instance.selectedEpisodeText,
                __instance.selectedEpisodeIcon,
                __instance.selectedEpisodeIconGlow);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UISettingInvenEquipPageListSlot), "SetBooksData")]
        [HarmonyPatch(typeof(UIInvenEquipPageListSlot), "SetBooksData")]
        public static void General_SetBooksData(object __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            switch (__instance)
            {
                case UISettingInvenEquipPageListSlot instance:
                    ArtUtil.SetBooksData(instance, books, storyKey);
                    break;
                case UIInvenEquipPageListSlot instance:
                    ArtUtil.SetBooksData(instance, books, storyKey);
                    break;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UISettingEquipPageScrollList), "CalculateSlotsHeight")]
        [HarmonyPatch(typeof(UIEquipPageScrollList), "CalculateSlotsHeight")]
        public static void General_CalculateSlotsHeight(object __instance)
        {
            switch (__instance)
            {
                case UISettingEquipPageScrollList instance:
                    ArtUtil.SetMainData(instance.currentBookModelList, instance.totalkeysdata,
                        instance.currentStoryBooksDic);
                    break;
                case UIEquipPageScrollList instance:
                    ArtUtil.SetMainData(instance.currentBookModelList, instance.totalkeysdata,
                        instance.currentStoryBooksDic);
                    break;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIInvenEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UIInvenLeftEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UISettingInvenEquipPageSlot), "SetOperatingPanel")]
        [HarmonyPatch(typeof(UISettingInvenEquipPageLeftSlot), "SetOperatingPanel")]
        public static void General_SetOperatingPanel(object __instance)
        {
            switch (__instance)
            {
                case UIInvenEquipPageSlot instance:
                    SephirahUtil.SetOperationPanel(instance, instance.button_Equip, instance.txt_equipButton,
                        instance._bookDataModel);
                    break;
                case UIInvenLeftEquipPageSlot instance:
                    SephirahUtil.SetOperationPanel(instance, instance.button_Equip, instance.txt_equipButton,
                        instance._bookDataModel);
                    break;
                case UISettingInvenEquipPageSlot instance:
                    SephirahUtil.SetOperationPanel(instance, instance.button_Equip, instance.txt_equipButton,
                        instance._bookDataModel);
                    break;
                case UISettingInvenEquipPageLeftSlot instance:
                    SephirahUtil.SetOperationPanel(instance, instance.button_Equip, instance.txt_equipButton,
                        instance._bookDataModel);
                    break;
            }
        }
    }
}