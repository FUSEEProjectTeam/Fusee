using CommandLine;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Fusee.Tools.Build.Blazorpatch
{
    public class Program
    {
        public class Options
        {
            [Option('p', "path", Required = true, HelpText = "Path to Blazor folder")]
            public string? BlazorPath { get; set; }

            [Option('t', "type", Default = ReplaceType.All, HelpText = "Type of replace operation. Valid options: All, RemoveHashingCheck, DecodePatch, RemoveBaseUrl, CopyPlayerCore, CopyIcos")]
            public ReplaceType ReplaceType { get; set; }

            [Option('f', "fuseeroot", HelpText = "Points to the Fusee root directory if not run from there.")]
            public string? FuseeRoot { get; set; }
        }

        public enum ReplaceType
        {
            All,
            RemoveHashingCheck,
            DecodePatch,
            RemoveBaseUrl,
            CopyPlayerCore,
            CopyIcos
        }

        public enum ErrorCode : int
        {
            Success,
            ErrorFilePath = -1,
            ErrorMatches = -2,
            ErrorFileCopy = -3,
            ErrorFileDownload = -4,

            InternalError = -42
        }

        public const string hashFile = @"service-worker.js";
        public const string hashPattern = @"(\(asset.url.*?integrity: asset.hash.*?\))";
        public const string hashReplace = @"(asset.url)";

        public const string baseUrlPattern = @"\s*?<base.*?\/>";
        public const string baseUrlReplace = @"";

        public const string indexFile = @"index.html";

        public const string decodeUrl = @"https://raw.githubusercontent.com/google/brotli/master/js/decode.min.js";
        public const string decodePattern = @"(<script.*?blazor\.webassembly\.js""><\/script>)";
        public const string decodeReplace = @"<script src=""_framework/blazor.webassembly.js"" autostart=""false""></script>
    <script type=""module"">
      import { BrotliDecode } from './decode.min.js';
      Blazor.start({
        loadBootResource: function (type, name, defaultUri, integrity) {
          if (type !== 'dotnetjs' && location.hostname !== 'localhost') {
            return (async function () {
              const response = await fetch(defaultUri + '.br', { cache: 'no-cache' });
              if (!response.ok) {
                throw new Error(response.statusText);
              }
              const originalResponseBuffer = await response.arrayBuffer();
              const originalResponseArray = new Int8Array(originalResponseBuffer);
              const decompressedResponseArray = BrotliDecode(originalResponseArray);
              const contentType = type === 
                'dotnetwasm' ? 'application/wasm' : 'application/octet-stream';
              return new Response(decompressedResponseArray, 
                { headers: { 'content-type': contentType } });
            })();
          }
        }
      });
    </script>";

        public const string deliverablesPath = @"art/Deliverables";

        public static void Main(string[] args)
        {
            ErrorCode errorCode = ErrorCode.Success;

            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed<Options>(o =>
                  {
                      if (o != null && o.BlazorPath != null)
                      {
                          switch (o.ReplaceType)
                          {
                              case ReplaceType.RemoveHashingCheck:
                                  errorCode = RemoveHashingCheck(o.BlazorPath, errorCode);
                                  break;
                              case ReplaceType.RemoveBaseUrl:
                                  errorCode = RemoveBaseUrl(o.BlazorPath, errorCode);
                                  break;
                              case ReplaceType.DecodePatch:
                                  errorCode = DecodePatch(o.BlazorPath, errorCode);
                                  break;
                              case ReplaceType.CopyPlayerCore:
                                  errorCode = CopyPlayerCore(o.BlazorPath, errorCode);
                                  break;
                              case ReplaceType.CopyIcos:
                                  errorCode = CopyIcos(o.BlazorPath, o.FuseeRoot, errorCode);
                                  break;
                              case ReplaceType.All:
                                  if (errorCode == ErrorCode.Success)
                                      errorCode = RemoveHashingCheck(o.BlazorPath, errorCode);

                                  if (errorCode == ErrorCode.Success)
                                      errorCode = RemoveBaseUrl(o.BlazorPath, errorCode);

                                  if (errorCode == ErrorCode.Success)
                                      errorCode = DecodePatch(o.BlazorPath, errorCode);

                                  if (errorCode == ErrorCode.Success)
                                      errorCode = CopyPlayerCore(o.BlazorPath, errorCode);

                                  if (errorCode == ErrorCode.Success)
                                      errorCode = CopyIcos(o.BlazorPath, o.FuseeRoot, errorCode);

                                  break;
                          }
                      }
                      else
                      {
                          errorCode = ErrorCode.InternalError;
                      }
                  });

            Environment.Exit((int)errorCode);
        }

        public static ErrorCode RemoveHashingCheck(string filePath, ErrorCode errorCode)
        {
            var fileContent = File.ReadAllText(Path.Combine(filePath, hashFile));

            var matches = Regex.Matches(fileContent, hashPattern);

            if (matches.Count == 1)
            {
                var newFileContent = Regex.Replace(fileContent, hashPattern, hashReplace);
                File.WriteAllText(Path.Combine(filePath, hashFile), newFileContent);
                Console.WriteLine("Removed hashing check.");
            }
            else
            {
                Console.WriteLine($"Error: unexpected number of matches for RemoveHashingCheck. Expected: 1, Found: {matches.Count}");
                return ErrorCode.ErrorMatches;
            }
            return errorCode;
        }

        public static ErrorCode RemoveBaseUrl(string filePath, ErrorCode errorCode)
        {
            var fileContent = File.ReadAllText(Path.Combine(filePath, indexFile));

            var matches = Regex.Matches(fileContent, baseUrlPattern);

            if (matches.Count == 1)
            {
                var newFileContent = Regex.Replace(fileContent, baseUrlPattern, baseUrlReplace);
                File.WriteAllText(Path.Combine(filePath, indexFile), newFileContent);
                Console.WriteLine("Removed <base> tag.");
            }
            else
            {
                Console.WriteLine($"Error: unexpected number of matches for RemoveBaseUrl. Expected: 1, Found: {matches.Count}");
                return ErrorCode.ErrorMatches;
            }
            return errorCode;
        }

        public static ErrorCode DecodePatch(string filePath, ErrorCode errorCode)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = client.GetAsync(decodeUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        using var fs = File.Create(Path.Combine(filePath, "decode.min.js"));
                        response.Content.CopyToAsync(fs);
                    }
                    else
                    {
                        throw new FileNotFoundException(decodeUrl);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not download decode.min.js: " + ex);
                    return ErrorCode.ErrorFileDownload;
                }
            }

            var fileContent = File.ReadAllText(Path.Combine(filePath, indexFile));

            var matches = Regex.Matches(fileContent, decodePattern);

            if (matches.Count == 1)
            {
                var newFileContent = Regex.Replace(fileContent, decodePattern, decodeReplace);
                File.WriteAllText(Path.Combine(filePath, indexFile), newFileContent);
                Console.WriteLine("Added and linked decode.min.js.");
            }
            else
            {
                Console.WriteLine($"Error: unexpected number of matches for DecodePatch. Expected: 1, Found: {matches.Count}");
                return ErrorCode.ErrorMatches;
            }
            return errorCode;
        }

        public static ErrorCode CopyPlayerCore(string filePath, ErrorCode errorCode)
        {
            try
            {
                File.Copy(Path.Combine(filePath, "_framework", "Fusee.Engine.Player.Core.dll"), Path.Combine(filePath, "Fusee.Engine.Player.Core.dll"), true);
                Console.WriteLine("Copied Fusee.Engine.Player.Core.dll.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Could not copy Fusee.Engine.Player.Core.dll: " + ex);
                return ErrorCode.ErrorFileCopy;
            }
            return errorCode;
        }
        public static ErrorCode CopyIcos(string filePath, string? fuseeRoot, ErrorCode errorCode)
        {
            var sourcePath = deliverablesPath;

            if (!string.IsNullOrWhiteSpace(fuseeRoot))
                sourcePath = Path.Combine(fuseeRoot, sourcePath);

            try
            {
                File.Copy(Path.Combine(sourcePath, "FuseeLogo.ico"), Path.Combine(filePath, "favicon.ico"), true);
                File.Copy(Path.Combine(sourcePath, "FuseeIcon512WithText.png"), Path.Combine(filePath, "icon-512.png"), true);
                Console.WriteLine("Copied ico files.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error could not copy ico files: " + ex);
                return ErrorCode.ErrorFileCopy;
            }
            return errorCode;
        }
    }
}