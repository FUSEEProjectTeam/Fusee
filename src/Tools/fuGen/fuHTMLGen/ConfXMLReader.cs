using System.Xml.Serialization;

namespace fuHTMLGen
{
    [XmlRoot("fusee")]
    public class ConfXMLReader
    {
        [XmlElement("webbuild")]
        public WebBuild WebBuildConf { get; set; }
    }

    public class WebBuild
    {
        [XmlElement("useProgressBar")]
        public string UseProgressBar { get; set; }
        [XmlElement("premultipliedAlpha")]
        public string PreMultipliedAlpha { get; set; }
    }
}