using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class BuffOptionsRoot
    {
        [XmlElement("BuffOption")] public List<BuffOptionRoot> BuffOption = new List<BuffOptionRoot>();
    }

    public class BuffOptionRoot
    {
        [XmlAttribute("BuffId")] public string BuffId;

        [XmlElement("TargetableBySpecialCards")]
        public bool TargetableBySpecialCards = true;

        [XmlElement("ForceAggroOptions")] public ForceAggroOptionsRoot ForceAggroOptions;
    }
}