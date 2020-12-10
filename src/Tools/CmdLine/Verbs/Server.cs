using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Fusee.Tools.CmdLine.Verbs
{
    [Verb("server", HelpText = "Launch a minimalistic local webserver and start the default browser.")]
    internal class Server
    {
        [Value(0, HelpText = "Directory or File path to be used as WWWRoot. If a file name is given, it will be started in the browser. If not, a default start file will be looked for.", MetaName = "Root")]
        public string Root { get; set; }

        [Option('p', "port", Default = 4655, HelpText = "Port the server should start running on.")]
        public int Port { get; set; }

        [Option('s', "serveronly", Default = false, HelpText = "Launches the server but does not start the default browser.")]
        public bool Serveronly { get; set; }

        public int Run()
        {
            FuseeHttpServer _httpServer;
            Thread _httpThread;
            string wwwRoot = null;
            string htmlFile = null;

            // If no root is given assume the current working directory
            if (string.IsNullOrEmpty(Root))
            {
                Root = Directory.GetCurrentDirectory();
            }

            // See if a file or a directory is specified
            if (File.Exists(Root))
            {
                htmlFile = Path.GetFileName(Root);
                wwwRoot = Path.Combine(Path.GetPathRoot(Root), Path.GetDirectoryName(Root));
            }
            else
            {
                wwwRoot = Root;
            }

            if (!Directory.Exists(wwwRoot))
            {
                Console.Error.WriteLine($"ERROR: Root directory {wwwRoot} not present or not accessible.");
                Environment.Exit((int)ErrorCode.InputFile);
            }

            // If no file is specified, try to find index.htm[l], default.htm[l] or any other html
            if (string.IsNullOrEmpty(htmlFile))
            {
                string[] htmlFiles = Directory.GetFiles(wwwRoot, "*.htm?", SearchOption.TopDirectoryOnly);
                htmlFile = htmlFiles.FirstOrDefault(s => s.ToLower().Contains("index")) ?? htmlFiles.FirstOrDefault(s => s.ToLower().Contains("default")) ?? htmlFiles.FirstOrDefault();
                if (string.IsNullOrEmpty(htmlFile))
                {
                    Serveronly = true;
                }
                else
                {
                    htmlFile = Path.GetFileName(htmlFile);
                }
            }

            // Fire up the http server
            try
            {
                _httpServer = new FuseeHttpServer(wwwRoot, Port);
                _httpThread = new Thread(_httpServer.listen);
                _httpThread.Start();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: starting local HTTP server at {wwwRoot} on port {Port}.\n{ex}");
                Environment.Exit((int)ErrorCode.InternalError);
            }
            Console.Error.WriteLine($"SUCCESS: Local HTTP server running at {wwwRoot} on port {Port}.");

            if (!Serveronly)
                Process.Start($"http://localhost:{Port}/" + Path.GetFileName(htmlFile));

            return 0;
        }
    }
}