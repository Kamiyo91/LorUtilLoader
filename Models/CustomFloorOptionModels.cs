using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class CustomFloorOptionRoot
    {
        [XmlElement("IconId")] public string IconId;
        [XmlElement("FloorNameId")] public string FloorNameId;
        [XmlElement("CustomFloorMap")] public MapModelRoot CustomFloorMap;
        [XmlElement("EmotionCardId")] public List<LorIdRoot> EmotionCardsId = new List<LorIdRoot>();
        [XmlElement("EgoCardId")] public List<LorIdRoot> EgoCardsId = new List<LorIdRoot>();


        [XmlElement("PackageId")] public string PackageId = "";
        [XmlIgnore] public List<EmotionCardXmlInfo> OriginalEmotionCards { get; set; } = new List<EmotionCardXmlInfo>();
        [XmlIgnore] public List<EmotionEgoXmlInfo> OriginalEgoCards { get; set; } = new List<EmotionEgoXmlInfo>();
    }
}