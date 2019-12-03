using CommandLine;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Fusee.Tools.fuseeCmdLine
{
    [Verb("install", HelpText = "Install the blender add-on. The -u option undoes the respective step.")]
    internal class Install
    {
        [Option('t', "type", Default = InstallationType.User, HelpText = "Machine-wide or per-user installation. '-t User' will set \"FuseeRoot\" and \"PATH\" for the current user only and will install the Blender Add-on below <user>/Appdata/Roaming/Blender Foundation. '-t Machine' will set \"FuseeRoot\" and \"PATH\" for all users and install the Blender Add-on below 'Program Files/Blender Foundation'. Start shell (cmd or powershell) as Administrator for machine-wide installation")]
        public InstallationType InstType { get; set; }

        [Option('u', "uninstall", Default = false, HelpText = "De-Register this FUSEE installation. This will only de-register this FUSEE instance (deregister the 'dotnet new fusee' template, remove fusee.exe from the \"PATH\" and remove the \"FuseeRoot\" environment varable). This will NOT delete the contents of the installation folder.")]
        public bool Uninstall { get; set; }

        [Option('b', "blender", Default = false, HelpText = "Only install/uninstall the Blender FUSEE Add-on.")]
        public bool Blender { get; set; }

        [Option('i', "blenderdir", HelpText = "Manually set the directory where to (un/)install the Blender FUSEE Add-on to/from. If not set, fusee.exe tries to find an appropriate Add-on directory based on the installation type (option '--type' per-user or machine-wide).")]
        public string BlenderDir { get; set; }

        [Option('f', "fileass", Default = false, HelpText = "Sets up file association for .fus and .fuz files as well as a custom uri handler for 'fusee://'.")]
        public bool fileAss { get; set; }

        public int Run()
        {
            // Find FuseeRoot from this assembly
            var fuseePaths = Helper.InitFuseeDirectories();

            // Set the individual installation steps (currently four). If NONE of them is set, select ALL OF THEM.
            bool instBlender = Blender;

            if (!(instBlender || fileAss))
            {
                instBlender = true;
                fileAss = true;
            }

            ErrorCode exitCode = ErrorCode.Success;

            // Install or uninstall ? 
            if (!Uninstall)
            {
                // Install the Blender AddOn
                if (instBlender)
                {
                    string blenderAddOnDstDir = "";
                    string blenderAddOnSrcDir = "";

                    try
                    {
                        IEnumerable<string> possibleDirs;
                        if (!string.IsNullOrEmpty(BlenderDir))
                        {
                            if (!Directory.Exists(BlenderDir))
                            {
                                throw new ArgumentException($"ERROR: The specified path doesn't exist {BlenderDir}.");
                            }
                            string allLower = BlenderDir.ToLower();
                            if (!allLower.Contains("addon") && !allLower.Contains("blender"))
                            {
                                Console.Error.WriteLine($"WARNING: The specified path doesn't look like a typical Blender Add-on folder.");
                            }
                            blenderAddOnDstDir = BlenderDir;
                        }
                        else
                        {
                            possibleDirs = GetBlenderAddOnDir();
                            blenderAddOnDstDir = possibleDirs.FirstOrDefault();
                            if (!string.IsNullOrEmpty(blenderAddOnDstDir) && !Directory.Exists(blenderAddOnDstDir))
                                Directory.CreateDirectory(blenderAddOnDstDir);
                        }

                        if (!string.IsNullOrEmpty(blenderAddOnDstDir))
                        {
                            blenderAddOnSrcDir = Path.Combine(fuseePaths.fuseeCmdLineRoot, "BlenderScripts", "addons");
                            foreach (var addOnSrcDir in Directory.EnumerateDirectories(blenderAddOnSrcDir, "*", SearchOption.TopDirectoryOnly))
                            {
                                string addOnName = new DirectoryInfo(addOnSrcDir).Name;
                                string addOnDstDir = Path.Combine(blenderAddOnDstDir, addOnName);
                                FileTools.DirectoryCopy(addOnSrcDir, addOnDstDir, true, true);
                                Console.Error.WriteLine($"SUCCESS: Installed the {addOnName} Blender Add-on to {blenderAddOnDstDir}.");
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine($"ERROR: Could not find a suitable Blender installation to install the Add-on to. Consider using the '-i' option to specifiy the Blender Add-on installation directory.");
                            exitCode = ErrorCode.OutputFile;
                        }
                    }
                    catch (UnauthorizedAccessException /* ex */)
                    {
                        Console.Error.WriteLine($"ERROR: Could not install FUSEE Blender Add-on to {blenderAddOnDstDir} due to access restrictions. Try running the shell as Administrator before calling fusee.exe.");
                        exitCode = ErrorCode.OutputFile;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: Unable to install the FUSEE Blender Add-on.\n{ex}");
                        exitCode = ErrorCode.InternalError;
                    }
                }

                if (fileAss)
                {
                    var pathtofuseedll = Assembly.GetEntryAssembly().Location;
                    var pathtofuseeexe = "";
                    var userprofilepath = Environment.GetEnvironmentVariable("USERPROFILE");

                    if (pathtofuseedll.Contains(userprofilepath + @"\.dotnet\tools\") && File.Exists(userprofilepath + @"\.dotnet\tools\fusee.exe"))
                    {
                        pathtofuseeexe = userprofilepath + @"\.dotnet\tools\fusee.exe";
                    }
                    else if (File.Exists(Path.ChangeExtension(pathtofuseedll, "exe")))
                    {
                        pathtofuseeexe = Path.ChangeExtension(pathtofuseedll, "exe");
                    }
                    else
                    {
                        Console.WriteLine("Error: Could not find fusee.exe.");
                        exitCode = ErrorCode.CouldNotFindFusee;
                    }

                    if (!string.IsNullOrEmpty(pathtofuseeexe))
                    {
                        try
                        {
                            RegistryKey schemeKey1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\fusee");
                            schemeKey1.SetValue("", "Fusee");
                            schemeKey1.SetValue("URL Protocol", "");

                            RegistryKey schemeKey2 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\fusee\DefaultIcon");
                            schemeKey2.SetValue("", pathtofuseeexe + ",0");

                            RegistryKey schemeKey3 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\fusee\Shell");

                            RegistryKey schemeKey4 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\fusee\Shell\Open");

                            RegistryKey schemeKey5 = Registry.CurrentUser.CreateSubKey(@"Software\Classes\fusee\Shell\Open\Command");
                            schemeKey5.SetValue("", pathtofuseeexe + " player \"%1\"");

                            Console.WriteLine("Set 'fusee://' Uri Scheme handler to use " + pathtofuseeexe);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: Could not set registry keys for custom uri scheme handler: " + e.Message);
                            exitCode = ErrorCode.CouldNotWriteRegistry;
                        }

                        try
                        {
                            RegistryKey fileKey1 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\.fus");
                            fileKey1.SetValue("", "fusee"); //this associates with the fusee custom uri handler

                            RegistryKey fileKey2 = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Classes\.fuz");
                            fileKey2.SetValue("", "fusee"); //this associates with the fusee custom uri handler
                            fileKey2.SetValue("Content Type", "application/zip");

                            Console.WriteLine("Set file association for .fus and .fuz files.");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Could not set registry keys for file association: " + e.Message);
                            exitCode = ErrorCode.CouldNotWriteRegistry;
                        }
                    }
                }
            }
            else
            {
                // UNINSTALL

                // Uninstall the Blender AddOn
                if (instBlender)
                {
                    string blenderAddOnDstDir = "";
                    string blenderAddOnSrcDir = "";

                    try
                    {
                        IEnumerable<string> possibleDirs;
                        if (!string.IsNullOrEmpty(BlenderDir))
                        {
                            if (!Directory.Exists(BlenderDir))
                            {
                                throw new ArgumentException($"ERROR: The specified path doesn't exist {BlenderDir}.");
                            }
                            string allLower = BlenderDir.ToLower();
                            if (!allLower.Contains("addon") && !allLower.Contains("blender"))
                            {
                                Console.Error.WriteLine($"WARNING: The specified path doesn't look like a typical Blender Add-on folder.");
                            }
                            blenderAddOnDstDir = BlenderDir;
                        }
                        else
                        {
                            possibleDirs = GetBlenderAddOnDir();
                            blenderAddOnDstDir = possibleDirs.First();
                        }

                        if (!string.IsNullOrEmpty(blenderAddOnDstDir))
                        {
                            blenderAddOnSrcDir = Path.Combine(fuseePaths.fuseeCmdLineRoot, "BlenderScripts", "addons");
                            foreach (var addOnSrcDir in Directory.EnumerateDirectories(blenderAddOnSrcDir, "*", SearchOption.TopDirectoryOnly))
                            {
                                string addOnName = new DirectoryInfo(addOnSrcDir).Name;
                                string addOnDstDir = Path.Combine(blenderAddOnDstDir, addOnName);
                                if (Directory.Exists(addOnDstDir))
                                {
                                    Directory.Delete(addOnDstDir, true);
                                    Console.Error.WriteLine($"SUCCESS: Removed the the {addOnName} Blender Add-on from {blenderAddOnDstDir}.");
                                }
                                else
                                {
                                    Console.Error.WriteLine($"WARNING: Could not remove the {addOnName} Blender Add-on from {blenderAddOnDstDir}. Either the Add-on was already removed, or this FUSEE instance installed the Add-on to a previous Blender version, or a different installation type (Machine/User) was specified at installation time.");
                                }
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine($"ERROR: Could not find a suitable Blender installation where to remove the FUSEE Add-on from. Consider using the `-i` option to specifiy the Blender Add-on installation directory.");
                            exitCode = ErrorCode.OutputFile;
                        }
                    }
                    catch (UnauthorizedAccessException /* ex */)
                    {
                        Console.Error.WriteLine($"ERROR: Could not uninstall FUSEE Blender Add-on from {blenderAddOnDstDir} due to access restrictions. Try running the shell as Administrator before calling fusee.exe.");
                        exitCode = ErrorCode.OutputFile;
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: Unable to uninstall the FUSEE Blender Add-on.\n{ex}");
                        exitCode = ErrorCode.InternalError;
                    }
                }

                if (fileAss)
                {
                    Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\.fus", true);
                    Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\.fuz", true);
                    Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\fusee", true);
                }
            }

            if (exitCode == ErrorCode.Success)
                Console.Error.WriteLine($"SUCCESS: All required FUSEE installation tasks succeeded.");
            else
                Console.Error.WriteLine($"WARNING: One or more required FUSEE installation tasks failed. See error messages above.");

            return 0;
        }

        private IEnumerable<string> GetBlenderAddOnDir()
        {
            // Start with some possible start directories (e.g. "C:\Program Files\" and C:\Program Files (x86)\"
            List<string> baseDirs = new List<string>();
            baseDirs.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
            baseDirs.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

            // Find [B|b]lender sub-subdirectories
            List<string> blenderDirs = new List<string>();
            foreach (var baseDir in baseDirs)
            {
                try
                {
                    blenderDirs.AddRange(Directory.EnumerateDirectories(baseDir, "?lender*", SearchOption.TopDirectoryOnly));
                }
                catch (UnauthorizedAccessException /* ex */)
                {
                    // Do nothing - simply ignore paths we cannot access.
                }
            }

            /* Removed - no FolderBrowserDialog in DotNet Core. Plus, opening a user dialog makes fusee.exe batch-incompatible
            // No Blender Installations found! Let the user pick Blender Path with file dialog!
            if (!blenderDirs.Any())
            {
                Console.WriteLine("WARNING: Blender Installation not found!\nINPUT REQUIRED: Please select Blender installation folder to proceed.");
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Console.WriteLine($"OK! Selected Blender installation path: {fbd.SelectedPath}");
                    blenderDirs.Add(fbd.SelectedPath);
                } // ERROR message will be shown @909 if no blender addon could be installed... 
            }
            */

            // Find addon sub-subdirectories
            List<string> addonDirs = new List<string>();
            foreach (var blenderDir in blenderDirs)
            {
                addonDirs.AddRange(Directory.EnumerateDirectories(blenderDir, "addon?", SearchOption.AllDirectories));
            }

            // 
            List<string> realAddonDirs;
            if (InstType == InstallationType.User)
            {
                // PER-USER Installation. Assume the respective Add-on-Directory below AppData\Roaming
                realAddonDirs = new List<string>();
                var appDataRoamingDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // This is "<user>\AppData\Roaming"
                foreach (var addonDir in addonDirs)
                {
                    string blVer = GetBlenderVersionFromPath(addonDir);
                    if (!string.IsNullOrEmpty(blVer))
                    {
                        realAddonDirs.Add(Path.Combine(appDataRoamingDir, "Blender Foundation", "Blender", blVer, "scripts", "addons"));
                    }
                }
            }
            else
            {
                // PER-MACHINE installation. We already have the addonDirs at hand
                realAddonDirs = addonDirs;
            }

            // Reverse-Sort directories according to some sub-dir name containting the blender version (e.g. 2.79 in ...\blender\2.79\addons\) 
            // (Highest version number first)
            return realAddonDirs.OrderByDescending(dir =>
            {
                int sortNumber = 0;
                foreach (var dirName in dir.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Reverse())
                {
                    // Build a sortable number from the (maximum) first four version digits. Typically Blender versions contain only two digits.
                    // Assume that digits are not bigger than 999 (e.g. Blender 2.999.111) 
                    var digits = dirName.Split('.');
                    var digitCount = digits.Length > 4 ? 4 : digits.Length;
                    for (int iDigit = 0; iDigit < digitCount; iDigit++)
                    {
                        if (int.TryParse(digits[iDigit], out int digit))
                            sortNumber += digit * (int)System.Math.Pow(1000, (3 - iDigit));
                    }
                    if (sortNumber > 0)
                        break;
                }
                return sortNumber;
            });
        }

        private string GetBlenderVersionFromPath(string path)
        {
            foreach (var dirName in path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Reverse())
            {
                bool isVersion = true;
                var digits = dirName.Split('.');
                foreach (var digit in digits)
                {
                    if (!int.TryParse(digit, out int digiNum))
                    {
                        isVersion = false;
                    }
                }
                if (isVersion)
                    return dirName;
            }
            return "";
        }

    }

    public enum InstallationType
    {
        User,
        Machine,
    }
}
