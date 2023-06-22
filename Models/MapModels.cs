using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class MapModelsRoot
    {
        [XmlElement("MapModel")] public List<MapModelRoot> MapModels;
    }

    public class MapModelRoot
    {
        [XmlElement("IsPlayer")] public bool IsPlayer;
        [XmlElement("OneTurnEgo")] public bool OneTurnEgo;
        [XmlElement("Bgx")] public float Bgx = 0.5f;
        [XmlElement("Bgy")] public float Bgy = 0.5f;
        [XmlElement("UnderX")] public float UnderX = 0.5f;
        [XmlElement("UnderY")] public float UnderY = 0.2777778f;
        [XmlElement("InitBgm")] public bool InitBgm;
        [XmlElement("CardId")] public List<LorIdRoot> CardIds;
        [XmlElement("Component")] public string Component = "";
        [XmlElement("MapName")] public string MapName = "";
        [XmlElement("Fx")] public float Fx = 0.5f;
        [XmlElement("Fy")] public float Fy = 407.5f / 1080f;


        [XmlElement("Stage")] public string Stage = "";
        [XmlElement("OriginalMapStageId")] public List<LorIdRoot> OriginalMapStageIds = new List<LorIdRoot>();
    }
}