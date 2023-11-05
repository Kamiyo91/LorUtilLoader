using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class RushBattleModelsRoot
    {
        [XmlElement("RushBattleModel")] public List<RushBattleModelMainRoot> RushBattleModels;
    }

    public class RushBattleModelMainRoot
    {
        [XmlAttribute("PackageId")] public string PackageId { get; set; }
        [XmlAttribute("Id")] public int Id { get; set; }
        [XmlElement("IsInfinite")] public bool IsInfinite { get; set; }
        [XmlElement("IsRandom")] public bool IsRandom { get; set; }
        [XmlElement("WaveCode")] public string WaveCode { get; set; }
        [XmlElement("Wave")] public List<RushBattleModelSubRoot> Waves { get; set; }
    }

    public class RushBattleModelSubRoot
    {
        [XmlElement("StageManagerName")] public string StageManagerName { get; set; }
        [XmlElement("MapStageName")] public List<string> MapStageNames { get; set; }
        [XmlElement("EnemyUnit")] public List<UnitModelRoot> UnitModels { get; set; }
        [XmlElement("CustomUnits")] public List<RushBattleFloorUnitModel> PlayerUnitModels { get; set; }
        [XmlElement("RecoverPlayerUnits")] public bool RecoverPlayerUnits { get; set; }

        [XmlElement("ReloadOriginalPlayerUnits")]
        public List<SephirahType> ReloadOriginalPlayerUnits { get; set; }

        //[XmlElement("StarterMapPhase")] public int StarterMapPhase { get; set; }
        [XmlElement("StartEmotionLevel")] public int StartEmotionLevel { get; set; }
        [XmlElement("FormationId")] public int FormationId { get; set; }
        [XmlElement("UnitAllowed")] public int UnitAllowed { get; set; }
        [XmlElement("CmhPackageId")] public string CmhPackageId { get; set; }
        [XmlElement("WaveOrder")] public int WaveOrder { get; set; }
        [XmlElement("SwitchWaveCode")] public string SwitchWaveCode { get; set; }
    }

    public class RushBattleFloorUnitModel
    {
        [XmlAttribute("Floor")] public SephirahType Floor;
        [XmlElement("Unit")] public List<UnitModelRoot> UnitModels { get; set; }
    }
}