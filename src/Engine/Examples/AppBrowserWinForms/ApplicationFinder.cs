using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Fusee.Engine;

namespace Examples.WinFormsFusee
{
    internal class ApplicationFinder
    {
        public ApplicationFinder()
        {
            SearchRoot = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        }

        private struct App
        {
            public string Name;
            public string Description;
            public string FileName;
            public string DirectoryName;
            public Type TypeDesc;
        }


        private string _searchRoot;

        public string SearchRoot
        {
            set
            {
                _searchRoot = value;
                PerformSearch();
            }
            get { return _searchRoot; }
        }

        private List<App> _apps;

        public void PerformSearch()
        {
            _apps = new List<App>();
            ScanDirectory(new DirectoryInfo(_searchRoot));
        }

        private void ScanDirectory(DirectoryInfo dir)
        {
            foreach (FileInfo file in dir.GetFiles("*.dll"))
            {
                ScanFile(file);
            }
            foreach (FileInfo file in dir.GetFiles("*.exe"))
            {
                ScanFile(file);
            }
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                ScanDirectory(subdir);
            }
        }

        private void ScanFile(FileInfo file)
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(file.FullName);
                foreach (Type type in assembly.GetTypes())
                {
                    // Check if type inherits from RenderCanvas
                    bool bFound = false;
                    for (Type t = type; t != null; t = t.BaseType)
                        if (t.FullName == typeof (RenderCanvas).FullName)
                        {
                            bFound = true;
                            break;
                        }
                    if (!bFound)
                        continue;

                    // Check if type contains the FuseeApplicationAttribute
                    object[] attribs = type.GetCustomAttributes(true);
                    if (attribs.Length > 0)
                    {
                        var fa = (FuseeApplicationAttribute) attribs[0];
                        _apps.Add(new App
                            {
                                Name = fa.Name,
                                Description = fa.Description,
                                FileName = file.Name,
                                DirectoryName = file.DirectoryName,
                                TypeDesc = type
                            });
                    }
                }
            }
            catch
            {
                // Couldn't load the file as a managed assembly - don't bother
                Debug.WriteLine("Error loading a non-managed assembly.");
            }
        }

        public int Length
        {
            get { return _apps.Count; }
        }

        public string GetNameAt(int i)
        {
            return _apps[i].Name;
        }

        public string GetDescriptionAt(int i)
        {
            return _apps[i].Description;
        }

        public string GetFileNameAt(int i)
        {
            return _apps[i].FileName;
        }

        public RenderCanvas Instantiate(int i)
        {
            Directory.SetCurrentDirectory(_apps[i].DirectoryName);
            return (RenderCanvas) Activator.CreateInstance(_apps[i].TypeDesc);
        }
    }
}
