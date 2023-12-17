using System.Collections.Generic;
using System.Xml.Serialization;
using UtilLoader21341.Enum;

namespace UtilLoader21341.Models
{
    public class StageOptionsRoot
    {
        [XmlElement("StageOption")] public List<StageOptionRoot> StageOption = new List<StageOptionRoot>();
    }

    public class StageOptionRoot
    {
        [XmlElement("BannedEmotionLevel")] public bool BannedEmotionLevel;
        [XmlElement("StageRequirements")] public StageRequirementRoot StageRequirements;
        [XmlElement("StageRewardOptions")] public RewardOptionRoot StageRewardOptions;
        [XmlElement("PreBattleOptions")] public PreBattleOptionRoot PreBattleOptions;
        [XmlElement("HidePreview")] public bool HidePreview;
        [XmlAttribute("PackageId")] public string PackageId = "";


        [XmlAttribute("Id")] public int StageId;
    }

    public class StageRequirementRoot
    {
        [XmlElement("RequiredLibraryLevel")] public int? RequiredLibraryLevel;
        [XmlElement("RequiredStageId")] public List<LorIdRoot> RequiredStageIds = new List<LorIdRoot>();
    }

    public class PreBattleOptionRoot
    {
        [XmlElement("BattleType")] public PreBattleType BattleType;
        [XmlElement("CustomUnits")] public List<CustomUnitsRoot> CustomUnits = new List<CustomUnitsRoot>();
        [XmlElement("SetToggles")] public bool SetToggles;
        [XmlElement("SephirahUnits")] public List<SephiorahUnitsRoot> SephirahUnits = new List<SephiorahUnitsRoot>();
        [XmlElement("FillWithBaseUnits")] public bool FillWithBaseUnits;
        [XmlElement("OnlySephirah")] public bool OnlySephirah;
        [XmlElement("SephirahLocked")] public bool SephirahLocked;


        [XmlElement("UnlockedSephirah")] public List<SephirahType> UnlockedSephirah = new List<SephirahType>();
    }

    public class SephiorahUnitsRoot
    {
        [XmlElement("Floor")] public SephirahType Floor;
        [XmlElement("SephirahUnit")] public List<SephirahType> SephirahUnit = new List<SephirahType>();
    }

    public class CustomUnitsRoot
    {
        [XmlElement("Floor")] public SephirahType Floor;
        [XmlElement("CustomUnit")] public List<UnitModelRoot> CustomUnit = new List<UnitModelRoot>();
    }
}