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
            public string BlazorPath { get; set; }

            [Option('t', "type", Default = ReplaceType.All, HelpText = "Type of replace operation. Valid options: RemoveHashingCheck, DecodePatch, All")]
            public ReplaceType ReplaceType { get; set; }
            [Option('f', "fuseeroot", HelpText = "Points to the Fusee root directory if not run from there.")]
            public string FuseeRoot { get; set; }
        }

        public enum ReplaceType
        {
            All,
            RemoveHashingCheck,
            DecodePatch,
            CopyPlayerCore,
            CopyIcos,
            NoJekyll
        }

        public enum ErrorCode : int
        {
            Success,
            ErrorFilePath = -1,

            InternalError = -42
        }

        public const string hashFile = @"service-worker.js";
        public const string hashPattern = @"(\(asset.url.*?integrity: asset.hash.*?\))";
        public const string hashReplace = @"(asset.url)";

        public const string decodeFile = @"index.html";
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
                      switch (o.ReplaceType)
                      {
                          case ReplaceType.RemoveHashingCheck:
                              RemoveHashingCheck(o.BlazorPath);
                              break;
                          case ReplaceType.DecodePatch:
                              DecodePatch(o.BlazorPath);
                              break;
                          case ReplaceType.CopyPlayerCore:
                              CopyPlayerCore(o.BlazorPath);
                              break;
                          case ReplaceType.CopyIcos:
                              CopyIcos(o.BlazorPath, o.FuseeRoot);
                              break;
                          case ReplaceType.NoJekyll:
                              CreateNojekyll(o.BlazorPath);
                              break;
                          case ReplaceType.All:
                              RemoveHashingCheck(o.BlazorPath);
                              DecodePatch(o.BlazorPath);
                              CopyPlayerCore(o.BlazorPath);
                              CopyIcos(o.BlazorPath, o.FuseeRoot);
                              CreateNojekyll(o.BlazorPath);
                              break;
                      }
                  });

            Environment.Exit((int)errorCode);
        }

        public static void RemoveHashingCheck(string filePath)
        {
            var fileContent = File.ReadAllText(Path.Combine(filePath, hashFile));

            var newFileContent = Regex.Replace(fileContent, hashPattern, hashReplace);

            File.WriteAllText(Path.Combine(filePath, hashFile), newFileContent);
        }

        public static void DecodePatch(string filePath)
        {
            using (var client = new HttpClient())
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

            var fileContent = File.ReadAllText(Path.Combine(filePath, decodeFile));

            var newFileContent = Regex.Replace(fileContent, decodePattern, decodeReplace);

            File.WriteAllText(Path.Combine(filePath, decodeFile), newFileContent);
        }

        public static void CopyPlayerCore(string filePath)
        {
            File.Copy(Path.Combine(filePath, "_framework", "Fusee.Engine.Player.Core.dll"), Path.Combine(filePath, "Fusee.Engine.Player.Core.dll"));
        }
        public static void CopyIcos(string filePath, string? fuseeRoot)
        {
            var sourcePath = deliverablesPath;

            if (!string.IsNullOrWhiteSpace(fuseeRoot))
                sourcePath = Path.Combine(fuseeRoot, sourcePath);

            File.Copy(Path.Combine(sourcePath, "FuseeLogo.ico"), Path.Combine(filePath, "favicon.ico"));
            File.Copy(Path.Combine(sourcePath, "FuseeIcon512WithText.png"), Path.Combine(filePath, "icon-512.png"));
        }

        public static void CreateNojekyll(string filePath)
        {
            File.Create(Path.Combine(filePath, ".nojekyll"));
        }
    }
}