using CommandLine;

namespace Fusee.Tools.CmdLine.Verbs
{
    [Verb("web", HelpText = "Launches a Fusee-App .dll via Fusees' WebAssembly web player")]
    internal class Web
    {
        [Value(0, HelpText = "Path to Fusee-App .dll.", MetaName = "Input", Required = false)]
        public string InputArgs { get; set; }

        public int Run()
        {

            return 0;
        }
    }

}