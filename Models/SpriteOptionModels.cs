using System.Collections.Generic;
using System.Xml.Serialization;
using UtilLoader21341.Enum;

namespace UtilLoader21341.Models
{
    public class SpriteOptionsRoot
    {
        [XmlElement("SpriteOption")] public List<SpriteOptionRoot> SpriteOption = new List<SpriteOptionRoot>();
    }

    public class SpriteOptionRoot
    {
        [XmlElement("Id")] public List<int> Ids = new List<int>();
        [XmlAttribute("PackageId")] public string PackageId = "";
        [XmlElement("SpriteOption")] public SpriteEnum SpriteOption = SpriteEnum.Custom;
        [XmlElement("SpritePK")] public string SpritePK = "";
    }
}