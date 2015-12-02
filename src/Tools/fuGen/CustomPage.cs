using System.IO;
using System.Reflection;

namespace Fusee.Tools.fuGen
{
    partial class WebPage
    {
        private readonly string _title;
        private readonly string _entryPoint;
        private readonly string _mainAssembly;
        private readonly string _customCSS;

        public WebPage(string target, string customCSS)
        {
            _mainAssembly = Path.GetFileNameWithoutExtension(target);
            Assembly asml = Assembly.ReflectionOnlyLoadFrom(target);
            MethodInfo entryPointInfo = asml.EntryPoint;
            _entryPoint = entryPointInfo.DeclaringType.Namespace + "." + entryPointInfo.DeclaringType.Name + "." + entryPointInfo.Name;
            _title = entryPointInfo.DeclaringType.Name + " FUSEE App";
            _customCSS = (string.IsNullOrEmpty(customCSS)) ? "browser.css" : customCSS; // != "") ? Path.Combine("Assets", customCSS) : "";
        }
    }
}
