using System;
using System.Linq;
using TMPro;
using UI;

namespace UtilLoader21341.Util
{
    public static class SephirahUtil
    {
        public static void SetOperationPanel(UIOriginEquipPageSlot instance,
            UICustomGraphicObject button_Equip, TextMeshProUGUI txt_equipButton, BookModel bookDataModel)
        {
            if (bookDataModel == null || !ModParameters.PackageIds.Contains(bookDataModel.ClassInfo.id.packageId) ||
                instance.BookDataModel == null ||
                instance.BookDataModel.owner != null) return;
            var currentUnit = UI.UIController.Instance.CurrentUnit;
            if (currentUnit == null) return;
            var keypage = ModParameters.KeypageOptions.FirstOrDefault(x =>
                x.PackageId == bookDataModel.ClassInfo.id.packageId && x.KeypageId == bookDataModel.ClassInfo.id.id);
            if (keypage == null) return;
            switch (keypage.EveryoneCanEquip)
            {
                case false when !keypage.OnlySephirahCanEquip:
                    return;
                case false when keypage.SephirahType != currentUnit.OwnerSephirah ||
                                (keypage.SephirahType == currentUnit.OwnerSephirah && !currentUnit.isSephirah):
                    button_Equip.interactable = false;
                    txt_equipButton.text = TextDataModel.GetText("ui_equippage_notequip", Array.Empty<object>());
                    return;
            }

            if (!IsLockedCharacter(currentUnit)) return;
            button_Equip.interactable = true;
            txt_equipButton.text = TextDataModel.GetText("ui_bookinventory_equipbook", Array.Empty<object>());
        }

        private static bool IsLockedCharacter(UnitDataModel unitData)
        {
            return unitData.isSephirah && (unitData.OwnerSephirah == SephirahType.Binah ||
                                           unitData.OwnerSephirah == SephirahType.Keter);
        }
    }
}