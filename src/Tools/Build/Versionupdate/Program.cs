using CommandLine;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fusee.Tools.Build.Versionupdate
{
    public class Program
    {
        public class Options
        {
            [Option('f', "file", Required = true, HelpText = "Path to file.")]
            public string FilePath { get; set; }

            [Option('v', "version", Required = true, HelpText = "Version string to set (can be a filename). Format: major.minor.patch-note")]
            public string VersionStringOrFilePath { get; set; }

            [Option('t', "type", Required = true, HelpText = "Type of replace operation. Valid options: Blender, Csproj, ...")]
            public ReplaceType ReplaceType { get; set; }
        }

        public struct Version
        {
            public int major;
            public int minor;
            public int patch;
            public string note;

            public string ToCsprojVersionString()
            {
                if (string.IsNullOrWhiteSpace(note))
                    return $"{major}.{minor}.{patch}";

                return $"{major}.{minor}.{patch}-{note}";
            }
            public string ToBlenderVersionString()
            {
                return $"{major}, {minor}, {patch}";
            }
        }

        public enum ReplaceType
        {
            Nope,
            Blender,
            Csproj,
            Props,
            AssemblyInfo,
            VsixManifest
        }

        public enum ErrorCode : int
        {
            Success,
            ErrorFilePath = -1,
            ErrorVersionString = -2,
            InputFormat = -3,
            OutputFile = -4,
            PlatformNotHandled = -5,
            InsufficentPrivileges = -6,
            CouldNotDownloadInputFile = -7,
            CouldNotWriteRegistry = -8,
            CouldNotFindFusee = -9,

            InternalError = -42
        }

        public const string versionStringPattern = @"^(\d+)\.(\d+)\.(\d+)(\-(\w+))?$";

        public const string blenderPluginReplacePattern = @"(\s*""version"":\s)(\(\d*,\s\d*,\s\d*\))(,)";
        public const string csprojReferenceReplacePattern = @"(<PackageReference.*?Include=""Fusee.*?Version=)""(\d+\.\d+\.\d+(\.\d+)?(\-(\w+))?)""(.*?\/>)";
        public const string propsVersionReplacePattern = @"(<\w*?Version)(>\d+\.\d+\.\d+(\.\d+)?(\-(\w+))?<)(\/\w*?Version>)";
        public const string propsCopyrightReplacePattern = @"(\s*<Copyright).*(\/Copyright>)";
        public const string assemblyInfoReplacePattern = @"(\[assembly:\sAssembly(File)?Version\()(""\d+\.\d+\.\d+(\.\d+)?(\-(\w+))?"")(\)\])";
        public const string vsixManifestReplacePattern = @"(\*?<Identity.*?Version=)(""\d+\.\d+\.\d+(\.\d+)?(\-(\w+))?"")(.*?\/>)";

        public static void Main(string[] args)
        {
            ErrorCode errorCode = ErrorCode.Success;

            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed<Options>(o =>
                  {
                      bool fileOk = false;
                      bool versionOk = false;
                      bool typeOk = false;

                      string filePath = "";
                      Version version = default;
                      ReplaceType replaceType = ReplaceType.Nope;

                      if (!string.IsNullOrEmpty(o.FilePath) && File.Exists(o.FilePath))
                      {
                          filePath = o.FilePath;
                          fileOk = true;
                      }

                      if (!string.IsNullOrEmpty(o.VersionStringOrFilePath)) // Check if VersionString is formatted correctly
                      {
                          MatchCollection matches = null;

                          if (Regex.Match(o.VersionStringOrFilePath, versionStringPattern).Success)
                          {
                              matches = Regex.Matches(o.VersionStringOrFilePath, versionStringPattern);
                          }
                          else if (File.Exists(o.VersionStringOrFilePath))
                          {
                              var fileContent = File.ReadAllText(o.VersionStringOrFilePath);

                              matches = Regex.Matches(fileContent, versionStringPattern);
                          }

                          if (matches != null && matches.Count > 0)
                          {
                              version.major = int.Parse(matches[0].Groups[1].Value);
                              version.minor = int.Parse(matches[0].Groups[2].Value);
                              version.patch = int.Parse(matches[0].Groups[3].Value);

                              if (matches[0].Groups.Count >= 6)
                                  version.note = matches[0].Groups[5].Value;

                              versionOk = true;
                          }
                      }

                      if (o.ReplaceType != ReplaceType.Nope)
                      {
                          replaceType = o.ReplaceType;
                          typeOk = true;
                      }

                      if (fileOk && versionOk && typeOk)
                      {
                          switch (replaceType)
                          {
                              case ReplaceType.Csproj:
                                  ReplaceCsproj(filePath, version.ToCsprojVersionString());
                                  break;
                              case ReplaceType.Blender:
                                  ReplaceBlender(filePath, version.ToBlenderVersionString());
                                  break;
                              case ReplaceType.Props:
                                  ReplaceProps(filePath, version.ToCsprojVersionString());
                                  break;
                              case ReplaceType.AssemblyInfo:
                                  ReplaceAssemblyInfo(filePath, version.ToCsprojVersionString());
                                  break;
                              case ReplaceType.VsixManifest:
                                  ReplaceVsixManifest(filePath, version.ToCsprojVersionString());
                                  break;
                              default:
                                  break;
                          }

                      }
                      else
                      {
                          // Error handling
                      }
                  });

            Environment.Exit((int)errorCode);
        }

        private static void ReplaceCsproj(string filePath, string newVersion)
        {
            var fileContent = File.ReadAllText(filePath);

            var newFileContent = Regex.Replace(fileContent, csprojReferenceReplacePattern, $"$1\"{newVersion}\"$6");

            File.WriteAllText(filePath, newFileContent);
        }

        private static void ReplaceBlender(string filePath, string newVersion)
        {
            var fileContent = File.ReadAllText(filePath);

            var newFileContent = Regex.Replace(fileContent, blenderPluginReplacePattern, $"$1({newVersion})$3");

            File.WriteAllText(filePath, newFileContent);
        }

        private static void ReplaceProps(string filePath, string newVersion)
        {
            var fileContent = File.ReadAllText(filePath);
            var currentYear = DateTime.Now.Year;

            var newFileContent1 = Regex.Replace(fileContent, propsVersionReplacePattern, $"$1>{newVersion}<$6");
            var newFileContent2 = Regex.Replace(newFileContent1, propsCopyrightReplacePattern, $"$1>Copyright 2013-{currentYear}<$2");

            File.WriteAllText(filePath, newFileContent2);
        }

        private static void ReplaceAssemblyInfo(string filePath, string newVersion)
        {
            var fileContent = File.ReadAllText(filePath);

            var newFileContent = Regex.Replace(fileContent, assemblyInfoReplacePattern, $"$1\"{newVersion}\"$7");

            File.WriteAllText(filePath, newFileContent);
        }

        private static void ReplaceVsixManifest(string filePath, string newVersion)
        {
            var fileContent = File.ReadAllText(filePath);

            var newFileContent = Regex.Replace(fileContent, vsixManifestReplacePattern, $"$1\"{newVersion}\"$6");

            File.WriteAllText(filePath, newFileContent);
        }


    }
}