using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class AssetsBundleOptionsRoot
    {
        [XmlElement("AssetsBundleOption")]
        public List<AssetsBundleOptionRoot> AssetsBundleOption = new List<AssetsBundleOptionRoot>();
    }

    public class AssetsBundleOptionRoot
    {
        [XmlAttribute("Name")] public string Name = "";
        [XmlAttribute("PackageId")] public string PackageId = "";
    }
}