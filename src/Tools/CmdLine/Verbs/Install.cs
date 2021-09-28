using CommandLine;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace Fusee.Tools.CmdLine.Verbs
{
#if WINDOWS
#nullable enable
    [Verb("install", HelpText = "Sets up file association for .fus and .fuz files as well as a custom uri handler for 'fusee://'.")]
    internal class Install
    {
        [Option('u', "uninstall", Default = false, HelpText = "Removes the file association for .fus and .fuz files as well as the custom uri handler for 'fusee://'.")]
        public bool Uninstall { get; set; }

        public int Run()
        {
            ErrorCode exitCode = ErrorCode.Success;

            if (!Uninstall)
            {
                var pathtofuseedll = Assembly.GetEntryAssembly()?.Location; ;
                var pathtofuseeexe = "";
                var userprofilepath = Environment.GetEnvironmentVariable("USERPROFILE");

                if (!string.IsNullOrEmpty(pathtofuseedll))
                {
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
            else
            {
                Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\.fus", true);
                Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\.fuz", true);
                Registry.CurrentUser.DeleteSubKeyTree(@"SOFTWARE\Classes\fusee", true);
            }

            if (exitCode == ErrorCode.Success)
                Console.Error.WriteLine($"SUCCESS: All required FUSEE installation tasks succeeded.");
            else
                Console.Error.WriteLine($"WARNING: One or more required FUSEE installation tasks failed. See error messages above.");

            return 0;
        }
    }
#endif
}