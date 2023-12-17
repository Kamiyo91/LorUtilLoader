using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class PassiveOptionsRoot
    {
        [XmlElement("PassiveOption")] public List<PassiveOptionRoot> PassiveOptions = new List<PassiveOptionRoot>();
    }

    public class PassiveOptionRoot
    {
        [XmlElement("ChainReleasePassives")] public List<LorIdRoot> ChainReleasePassives = new List<LorIdRoot>();
        [XmlElement("GainCoins")] public bool GainCoins = true;

        [XmlElement("BannedEmotionCardSelection")]
        public bool BannedEmotionCardSelection;

        [XmlElement("BannedEgoFloorCards")] public bool BannedEgoFloorCards;

        [XmlElement("BannedEgoAndEmotionCards")]
        public bool BannedEgoAndEmotionCards;


        [XmlElement("IsMultiDeck")] public bool IsMultiDeck;
        [XmlElement("IsSupportPassive")] public bool IsSupportPassive;

        [XmlElement("CannotBeUsedWithPassives")]
        public List<LorIdRoot> CannotBeUsedWithPassives = new List<LorIdRoot>();

        [XmlElement("CanBeUsedWithPassivesAll")]
        public List<LorIdRoot> CanBeUsedWithPassivesAll = new List<LorIdRoot>();

        [XmlElement("CanBeUsedWithPassivesOne")]
        public List<LorIdRoot> CanBeUsedWithPassivesOne = new List<LorIdRoot>();


        [XmlElement("ForceAggroOptions")] public ForceAggroOptionsRoot ForceAggroOptions;


        [XmlElement("IgnoreClashPassive")] public bool IgnoreClashPassive;

        [XmlElement("IsBaseGamePassive")] public bool IsBaseGamePassive;


        [XmlElement("Script")] public string Script;
        [XmlElement("MultiDeckLabelId")] public List<string> MultiDeckLabelIds = new List<string>();
        [XmlAttribute("PackageId")] public string PackageId = "";
        [XmlAttribute("Id")] public int PassiveId;
    }
}