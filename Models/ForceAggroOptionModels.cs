using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class ForceAggroOptionsRoot
    {
        [XmlElement("ForceAggro")] public bool ForceAggro;
        [XmlElement("ForceAggroLastDie")] public bool ForceAggroLastDie;
        [XmlElement("ForceAggroSpeedDie")] public List<int> ForceAggroSpeedDie = new List<int>();
        [XmlElement("ForceAggroByBuff")] public List<string> ForceAggroByBuff = new List<string>();


        [XmlElement("RedirectOnlyWithSlowerSpeed")]
        public bool RedirectOnlyWithSlowerSpeed;
    }
}