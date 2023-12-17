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
        [XmlAttribute("Id")] public int Id;
        [XmlElement("IsInfinite")] public bool IsInfinite;
        [XmlElement("IsRandom")] public bool IsRandom;
        [XmlAttribute("PackageId")] public string PackageId = "";
        [XmlElement("Wave")] public List<RushBattleModelSubRoot> Waves = new List<RushBattleModelSubRoot>();
    }

    public class RushBattleModelSubRoot
    {
        [XmlElement("ReloadOriginalPlayerUnits")]
        public List<SephirahType> ReloadOriginalPlayerUnits = new List<SephirahType>();

        [XmlElement("StartEmotionLevel")] public int StartEmotionLevel;
        [XmlElement("FormationId")] public int FormationId = 1;
        [XmlElement("UnitAllowed")] public int UnitAllowed = 5;
        [XmlElement("WaveOrder")] public int WaveOrder;
        [XmlElement("SwitchWaveCode")] public string SwitchWaveCode = "";
        [XmlElement("LastOneInfinite")] public bool LastOneInfinite;
        [XmlElement("StageManagerName")] public string StageManagerName = "";
        [XmlElement("EnemyUnit")] public List<UnitModelRoot> UnitModels = new List<UnitModelRoot>();
        [XmlElement("MapName")] public List<string> MapNames = new List<string>();

        [XmlElement("CustomUnits")]
        public List<RushBattleFloorUnitModel> PlayerUnitModels = new List<RushBattleFloorUnitModel>();

        [XmlElement("RecoverPlayerUnits")] public bool RecoverPlayerUnits;


        [XmlElement("WaveCode")] public string WaveCode = "";


        [XmlIgnore] public bool Fought { get; set; }
    }

    public class RushBattleFloorUnitModel
    {
        [XmlAttribute("Floor")] public SephirahType Floor;
        [XmlElement("Unit")] public List<UnitModelRoot> UnitModels = new List<UnitModelRoot>();
    }
}