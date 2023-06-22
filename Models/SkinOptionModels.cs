using System.Collections.Generic;
using System.Xml.Serialization;

namespace UtilLoader21341.Models
{
    public class SkinOptionsRoot
    {
        [XmlElement("SkinOption")] public List<SkinOptionRoot> SkinOption;
    }

    public class CustomSkinOptionsRoot
    {
        [XmlElement("CustomSkinOption")] public List<CustomSkinOptionRoot> CustomSkinOption;
    }

    public class SkinOptionRoot
    {
        [XmlElement("PackageId")] public string PackageId = "";
        [XmlElement("CustomHeight")] public int CustomHeight;

        [XmlElement("MotionSounds")]
        public List<MotionSoundOptionRoot> MotionSounds = new List<MotionSoundOptionRoot>();


        [XmlAttribute("SkinName")] public string SkinName = "";
    }

    public class CustomSkinOptionRoot
    {
        [XmlElement("KeypageId")] public int? KeypageId;
        [XmlElement("KeypageName")] public string KeypageName = "";
        [XmlElement("CharacterNameId")] public int? CharacterNameId;


        [XmlAttribute("PackageId")] public string PackageId = "";


        [XmlElement("SkinName")] public string SkinName = "";
        [XmlElement("UseLocalization")] public bool UseLocalization = true;
    }
}