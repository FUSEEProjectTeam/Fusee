using System;
using System.IO;
using System.Reflection;

namespace Fusee.Tools.CmdLine
{
    internal static class Helper
    {
        public static FuseePaths InitFuseeDirectories()
        {
            FuseePaths fuseePaths = default;

            // Check if player is present
            try
            {
                fuseePaths.fuseeCmdLineRoot = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                fuseePaths.fuseeCmdLineRoot = FileTools.PathAddTrailingSeperator(fuseePaths.fuseeCmdLineRoot);

                fuseePaths.fuseeBuildRoot = Path.GetFullPath(Path.Combine(fuseePaths.fuseeCmdLineRoot, ".."));        // one hop down to remove "Tools" from %FuseeRoot%/bin/[Debug|Release]/Tools.
                fuseePaths.fuseeBuildRoot = FileTools.PathAddTrailingSeperator(fuseePaths.fuseeBuildRoot);

                fuseePaths.fuseeRoot = Path.GetFullPath(Path.Combine(fuseePaths.fuseeCmdLineRoot, "..", "..", "..")); // three hops from %FuseeRoot%/bin/[Debug|Release]/Tools down to the root.
                fuseePaths.fuseeRoot = FileTools.PathAddTrailingSeperator(fuseePaths.fuseeRoot);

                if (fuseePaths.fuseeCmdLineRoot.Contains("Debug"))
                {
                    fuseePaths.fuseeConfiguration = "Debug";
                }
                else
                {
                    fuseePaths.fuseeConfiguration = fuseePaths.fuseeCmdLineRoot.Contains("Release") ? "Release" : "Unknown";
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: this instance of fusee.exe at {fuseePaths.fuseeCmdLineRoot} doesn't seem to be part of a FUSEE installation.\n{ex}");
                Environment.Exit((int)ErrorCode.InternalError);
            }

            return fuseePaths;
        }

        internal struct FuseePaths
        {
            public string fuseeCmdLineRoot;
            public string fuseeRoot;
            public string fuseeBuildRoot;
            public string fuseeConfiguration;
        }
    }
}
