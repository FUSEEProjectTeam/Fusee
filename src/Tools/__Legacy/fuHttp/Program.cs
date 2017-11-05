using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Fusee.Tools.fuHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            var htdocsRoot = "";
            var port = 4655; // HEX FU
            var htmlFile = "";
            if (args == null || args.Length == 0)
                htdocsRoot = Directory.GetCurrentDirectory();


            // See if a htdocs folder and/or a start page was specified
            if (args != null && args.Length >= 1 && !string.IsNullOrEmpty(args[0]))
            {
                var path = args[0];

                // html file given?
                if (File.Exists(path) && Path.GetExtension(path).ToLower().Substring(0, 4) == ".htm")
                {
                    htmlFile = Path.GetFileName(path);
                    path = Path.GetDirectoryName(path);
                }

                // Does the folder exist?
                if (Directory.Exists(path))
                {
                    htdocsRoot = path;
                }
                else
                {
                    Console.Error.WriteLine("Cannot use " + path + " as http document root - directory does not exist.");
                    Environment.Exit(-1);
                }
            }

            // See if a port was specified
            if (args != null && args.Length >= 2)
            {
                if (!int.TryParse(args[1], out port))
                {
                    Console.Error.WriteLine("Cannot use " + args[1] + " as http port - not a valid number.");
                    Environment.Exit(-1);
                }
            }

            // Start server
            Console.Error.WriteLine("FUSEE Simple HTTP Server listening to requests on port " + port + ".\nhtdocs root folder is '" + htdocsRoot + "'.");
            var server = new FuseeHttpServer(htdocsRoot, port);
            Thread thread = new Thread(server.listen);
            thread.Start();

            // Start browser (if html file is given)
            if (!string.IsNullOrEmpty(htmlFile))
            {
                Thread.Sleep(500);
                var url = "http://localhost:" + port + "/" + htmlFile;
                Console.Error.WriteLine("Opening " + url + " in standard browser.");
                var proc = Process.Start(url);
                // doesn't work proc.WaitForExit();
            }

            do
            {
                Console.Error.WriteLine("Hit 'C' to exit");
            } while ('c' != Console.ReadKey().KeyChar);

            server.Stop();
            thread.Join(1000);
        }
    }
}
