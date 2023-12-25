using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class ForceAggroOptionsRoot
    {
        [XmlElement("ForceAggro")] public bool ForceAggro;
        [XmlElement("ForceAggroLastDie")] public bool ForceAggroLastDie;
        [XmlElement("ForceAggroSpeedDie")] public List<int> ForceAggroSpeedDie = new List<int>();

        [XmlElement("ForceAggroByBuffByKeywordId")]
        public List<string> ForceAggroByBuffByKeywordId = new List<string>();


        [XmlElement("RedirectOnlyWithSlowerSpeed")]
        public bool RedirectOnlyWithSlowerSpeed;
    }
}