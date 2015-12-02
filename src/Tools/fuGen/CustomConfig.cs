using System.IO;
using System.Xml.Serialization;

namespace Fusee.Tools.fuGen
{
    partial class JsilConfig
    {
        private readonly string _fileName;
        private readonly string _customManifest;

        private readonly string _useProgrBar;
        private readonly string _preMultAlpha;

        public JsilConfig(string targApp, string targDir, bool customConf)
        {
            _fileName = Path.GetFileName(targApp);
            var fileNameWOext = Path.GetFileNameWithoutExtension(targApp);

            _customManifest = fileNameWOext + ".contentproj";

            if (!customConf)
            {
                _useProgrBar = "true";
                _preMultAlpha = "true";
            }
            else
            {
                var xmlSer = new XmlSerializer(typeof (ConfXMLReader));

                StreamReader confReader = File.OpenText(Path.Combine(targDir, "Assets", "fusee_config.xml"));
                var conf = (ConfXMLReader) xmlSer.Deserialize(confReader);
                confReader.Close();

                // read settings
                _useProgrBar = (string.IsNullOrEmpty(conf.WebBuildConf.UseProgressBar))
                    ? "true"
                    : conf.WebBuildConf.UseProgressBar.ToLower();

                _preMultAlpha = (string.IsNullOrEmpty(conf.WebBuildConf.PreMultipliedAlpha))
                    ? "true"
                    : conf.WebBuildConf.PreMultipliedAlpha.ToLower(); 
            }
        }
    }
}