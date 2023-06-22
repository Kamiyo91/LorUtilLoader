using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class KeypageOptionsRoot
    {
        [XmlElement("KeypageOption")] public List<KeypageOptionRoot> KeypageOption;
    }

    public class KeypageOptionRoot
    {
        [XmlElement("MultiDeckLabelId")] public List<string> MultiDeckLabelIds = new List<string>();
        [XmlElement("BookCustomOptions")] public BookCustomOptionRoot BookCustomOptions;
        [XmlElement("BannedEgoFloorCards")] public bool BannedEgoFloorCards;
        [XmlElement("BannedEmotionCards")] public bool BannedEmotionCards;

        [XmlElement("EveryoneCanEquip")] public bool EveryoneCanEquip;
        [XmlElement("OnlySephirahCanEquip")] public bool OnlySephirahCanEquip;
        [XmlElement("BookIconId")] public string BookIconId = "";

        [XmlElement("IsMultiDeck")] public bool IsMultiDeck;

        [XmlElement("TargetableBySpecialCards")]
        public bool TargetableBySpecialCards = true;

        [XmlElement("CustomFloorOptions")] public CustomFloorOptionRoot CustomFloorOptions;
        [XmlElement("Editable")] public bool Editable = true;
        [XmlElement("EditErrorMessageId")] public string EditErrorMessageId = "";


        [XmlElement("SephirahType")] public SephirahType SephirahType;
        [XmlElement("ForceAggroOptions")] public ForceAggroOptionsRoot ForceAggroOptions;


        [XmlAttribute("Id")] public int KeypageId;


        [XmlAttribute("PackageId")] public string PackageId = "";
    }

    public class BookCustomOptionRoot
    {
        [XmlElement("CustomFaceData")] public bool CustomFaceData;
        [XmlElement("OriginalSkin")] public string OriginalSkin = "";
        [XmlElement("EgoSkin")] public List<string> EgoSkin;
        [XmlElement("CustomDialogId")] public LorIdRoot CustomDialogId;


        [XmlElement("NameTextId")] public int NameTextId;


        [XmlElement("OriginalSkinIsBaseGame")] public bool OriginalSkinIsBaseGame;
        [XmlElement("XiaoTaotieAction")] public ActionDetail XiaoTaotieAction = ActionDetail.NONE;
    }
}