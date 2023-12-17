using System.Collections.Generic;
using System.Xml.Serialization;
using LOR_DiceSystem;

namespace UtilLoader21341.Models
{
    public class LorIdRoot
    {
        [XmlAttribute("Id")] public int Id;
        [XmlAttribute("PackageId")] public string PackageId = "";
    }

    public class DefaultKeywordRoot
    {
        [XmlElement("DefaultKeyword")] public DefaultKeywordOption DefaultKeywordOption;
    }

    public class RewardOptionsRoot
    {
        [XmlElement("RewardOptions")] public List<RewardOptionRoot> RewardOption = new List<RewardOptionRoot>();
    }

    public class DefaultKeywordOption
    {
        [XmlAttribute("Keyword")] public string Keyword = "";
        [XmlAttribute("PackageId")] public string PackageId = "";
    }

    public class MotionSoundOptionRoot
    {
        [XmlElement("Motion")] public MotionDetail Motion;
        [XmlElement("MotionSound")] public MotionSoundRoot MotionSound;
    }

    public class MotionSoundRoot
    {
        [XmlElement("FileNameLose")] public string FileNameLose = "";
        [XmlElement("FileNameWin")] public string FileNameWin = "";
        [XmlElement("IsBaseSoundLose")] public bool IsBaseSoundLose;
        [XmlElement("IsBaseSoundWin")] public bool IsBaseSoundWin;
    }

    public class RewardOptionRoot
    {
        [XmlElement("Books")] public List<ItemQuantityRoot> Books = new List<ItemQuantityRoot>();
        [XmlElement("Cards")] public List<ItemQuantityRoot> Cards = new List<ItemQuantityRoot>();
        [XmlElement("Keypages")] public List<ItemQuantityRoot> Keypages = new List<ItemQuantityRoot>();
        [XmlElement("MessageId")] public string MessageId = "";
        [XmlElement("SingleTimeReward")] public bool SingleTimeReward = true;
    }

    public class ItemQuantityRoot
    {
        [XmlElement("LorId")] public LorIdRoot LorId;
        [XmlAttribute("Quantity")] public int Quantity;
    }

    public class EtcRoot
    {
        [XmlElement("Text")] public List<EtcText> Text = new List<EtcText>();
    }

    public class EtcText
    {
        [XmlText] public string Desc = "";
        [XmlAttribute("ID")] public string ID = "";
    }
}