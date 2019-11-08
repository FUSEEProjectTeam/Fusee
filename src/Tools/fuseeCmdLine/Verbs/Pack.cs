using CommandLine;
using System;
using System.IO;
using System.IO.Compression;

namespace Fusee.Tools.fuseeCmdLine
{
    [Verb("pack", HelpText = "Packs a compiled Fusee-App into one .fuz-File.")]
    internal class Pack
    {
        [Value(0, HelpText = "Input Fusee-App .dll", MetaName = "Input", Required = true)]
        public string Input { get; set; }

        public int Run()
        {
            if (!string.IsNullOrWhiteSpace(Input) && Path.GetExtension(Input).ToLower().Equals(".dll"))
            {
                var path = Path.GetDirectoryName(Input);
                var appname = Path.GetFileNameWithoutExtension(Input);

                var zipfile = appname + ".fuz";

                var temppath = Path.GetTempPath();

                var zipfilepath = Path.Combine(path, zipfile);
                var zipfiletemppath = Path.Combine(temppath, zipfile);

                if (File.Exists(Path.Combine(zipfilepath)))
                    File.Delete(Path.Combine(zipfilepath));

                if (File.Exists(Path.Combine(zipfiletemppath)))
                    File.Delete(Path.Combine(zipfiletemppath));

                ZipFile.CreateFromDirectory(path, zipfiletemppath, CompressionLevel.Optimal, false);
                File.Move(zipfiletemppath, zipfilepath);

                Console.WriteLine("Packed to: " + zipfilepath);
            }
            return 0;
        }

    }
}
