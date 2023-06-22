using System.Collections.Generic;
using System.Xml.Serialization;
using LOR_DiceSystem;

namespace UtilLoader21341.Models
{
    public class CardOptionsRoot
    {
        [XmlElement("CardOption")] public List<CardOptionRoot> CardOption;
    }

    public class CardOptionRoot
    {
        [XmlElement("Option")] public CardOption Option = CardOption.Basic;
        [XmlElement("Keywords")] public List<string> Keywords = new List<string>();
        [XmlElement("BookId")] public List<LorIdRoot> BookId = new List<LorIdRoot>();
        [XmlElement("OneSideOnlyCard")] public bool OneSideOnlyCard;

        [XmlElement("OneSideOnlyCardOnlyForAlly")]
        public bool OneSideOnlyCardOnlyForAlly;

        [XmlElement("IsBaseGameCard")] public bool IsBaseGameCard;
        [XmlElement("Id")] public List<int> Ids = new List<int>();


        [XmlElement("OnlyAllyTargetCard")] public bool OnlyAllyTargetCard;


        [XmlAttribute("PackageId")] public string PackageId = "";
    }
}