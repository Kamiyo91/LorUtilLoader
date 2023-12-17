using System.Collections.Generic;
using System.Xml.Serialization;
using UI;
using UtilLoader21341.Enum;

namespace UtilLoader21341.Models
{
    public class CategoryOptionsRoot
    {
        [XmlElement("CategoryOption")] public List<CategoryOptionRoot> CategoryOption = new List<CategoryOptionRoot>();
    }

    public class CategoryOptionRoot
    {
        [XmlAttribute("CategoryNumber")] public string AdditionalValue = "";
        [XmlElement("BaseIconSpriteId")] public string BaseIconSpriteId = "";
        [XmlElement("CategoryNameId")] public string CategoryNameId = "";
        [XmlElement("BaseGameCategory")] public UIStoryLine? BaseGameCategory;

        [XmlElement("CredenzaType")] public CredenzaEnum CredenzaType = CredenzaEnum.ModifiedCredenza;
        [XmlElement("PackageId")] public string PackageId = "";
        [XmlElement("Chapter")] public int Chapter = 7;
        [XmlElement("CategoryBooksId")] public List<int> CategoryBooksId = new List<int>();


        [XmlElement("Order")] public int Order;
        [XmlElement("CredenzaBooksId")] public List<int> CredenzaBooksId = new List<int>();


        [XmlElement("CustomIconSpriteId")] public string CustomIconSpriteId = "";
    }
}