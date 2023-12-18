using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class UnitModelsRoot
    {
        [XmlElement("UnitModel")] public List<UnitModelRoot> UnitModels = new List<UnitModelRoot>();
    }

    public class UnitModelRoot
    {
        [XmlElement("CustomPos")] public XmlVector2 CustomPos;
        [XmlElement("SkinName")] public string SkinName = "";
        [XmlElement("AdditionalPassiveId")] public List<LorIdRoot> AdditionalPassiveIds = new List<LorIdRoot>();
        [XmlElement("AdditionalBuff")] public List<string> AdditionalBuffs = new List<string>();

        [XmlElement("LockedEmotion")] public bool LockedEmotion;
        [XmlElement("MaxEmotionLevel")] public int MaxEmotionLevel;
        [XmlElement("AutoPlay")] public bool AutoPlay;

        [XmlElement("HideInfo")] public bool HideInfo;
        [XmlAttribute("Id")] public int Id;


        [XmlAttribute("PackageId")] public string PackageId = "";

        [XmlElement("SummonedOnPlay")] public bool SummonedOnPlay;
        [XmlElement("UnitNameId")] public int UnitNameId;
    }
}