using System.Collections.Generic;
using System.Xml.Serialization;
using UtilLoader21341.Enum;

namespace UtilLoader21341.Models
{
    public class SpriteOptionsRoot
    {
        [XmlElement("SpriteOption")] public List<SpriteOptionRoot> SpriteOption;
    }

    public class SpriteOptionRoot
    {
        [XmlElement("Id")] public List<int> Ids;
        [XmlAttribute("PackageId")] public string PackageId = "";
        [XmlElement("SpriteOption")] public SpriteEnum SpriteOption = SpriteEnum.Custom;
        [XmlElement("SpritePK")] public string SpritePK = "";
    }
}