using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace C4d
{

    public class Plugin
    {
        private readonly List<object> _pluginInstanceList;
        private readonly List<NodeDataAllocator> _nodeAllocatorList;

        public const int ID_PLUGINTEMPLATE = 1000004;

        public Plugin()
        {
            Logger.Loglevel = Logger.LogLevel.Debug;
            Logger.Debug("Plugin()");

            PluginAllocator._pluginInstanceList = _pluginInstanceList = new List<object>();
            _nodeAllocatorList = new List<NodeDataAllocator>();
        }

        public bool Start()
        {
            Logger.Debug("Start()");
 
            // Scan directories for suitable plugin assemblies
            DirectoryInfo di = new DirectoryInfo(GetPluginsPath());

            ScanDirectories(di, 2);

            return true;
        }

        private void ScanDirectories(DirectoryInfo di, int recursionDepth)
        {
            Logger.Info("ScanDirectories(" + di.FullName + ", " + recursionDepth + ")");
            if (recursionDepth < 0)
                return;

            // First scan all files in the directory
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension.Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    CheckDll(fi);
                }
            }

            // Then, recursively scan all subdirectories
            foreach (DirectoryInfo sdi in di.GetDirectories())
            {
                ScanDirectories(sdi, recursionDepth - 1);
            }
        }

        private void CheckDll(FileInfo fi)
        {
            Logger.Debug("CheckDll(" + fi.FullName + ")");

            Assembly plugIn;
            try
            {
                plugIn = Assembly.LoadFrom(fi.FullName);
            }
            catch (BadImageFormatException)
            {
                // Swallow this because we could just step over an unmanaged dll
                return;
            }

            if (plugIn == null)
                return;

            foreach (Type t in plugIn.GetTypes())
            {
                // Is it a "generic" plugin?
                if (t.IsDefined(typeof(PluginAttribute), true))
                {
                    PluginAttribute attr =
                        (PluginAttribute) Attribute.GetCustomAttribute(t, typeof (PluginAttribute), true);
                    Logger.Debug("  Class " + t.Name + " is attributed with [Plugin]");

                    MethodInfo miStart = t.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public);
                    if (miStart == null)
                        Logger.Warn("Class " + t.Name + " in " + fi.Name + " seems suitable as a plugin but is missing the Start() method");
                    MethodInfo miEnd = t.GetMethod("End", BindingFlags.Instance | BindingFlags.Public);
                    if (miStart == null)
                        Logger.Warn("Class " + t.Name + " in " + fi.Name + " seems suitable as a plugin but is missing the End() method");
                        
                    if (miStart != null && miEnd != null)
                    {
                        // We have a class
                        ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor != null)
                        {
                            object instance = ctor.Invoke(null);
                            MethodInfo mi = t.GetMethod("Start");
                            _pluginInstanceList.Add(instance);
                            object ret = mi.Invoke(instance, null);

                            // TODO: Do something with the return value
                        }
                        else
                        {
                            Logger.Warn("Class " + t.Name + " in " + fi.Name + " is missing a parameterless constructor");
                        }
                    }
                }
                // Or is it an attributed CommandPlugin ?
                else if (t.IsDefined(typeof(CommandPluginAttribute), true))
                {
                    CommandPluginAttribute attr =
                        (CommandPluginAttribute) Attribute.GetCustomAttribute(t, typeof (CommandPluginAttribute), true);
                    Logger.Debug("  Class " + t.Name + " is attributed with [CommandPlugin(ID=" + attr.ID + ", Name=\"" +
                                 attr.Name + "\")]");

                    if (InheritsFrom(t, typeof(CommandData)))
                    {
                        // Register the command plugin
                        string name;
                        BaseBitmap bmp;
                        GetPluginDescription(t, attr, out name, out bmp);
                        // Fallback help text
                        string helpText = attr.HelpText;
                        if (string.IsNullOrEmpty(helpText))
                        {
                            helpText = "Execute the " + name + " command.";
                        }

                        ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor != null)
                        {
                            CommandData commandData = (CommandData) ctor.Invoke(null);
                            C4dApi.RegisterCommandPlugin(attr.ID, name, 0, bmp, helpText, commandData);
                        }
                        else
                        {
                            Logger.Warn("Class " + t.Name + " in " + fi.Name + " is missing a parameterless constructor");
                        }
                    }
                    else
                    {
                        Logger.Warn("  Class " + t.Name + " in " + fi.Name +
                                    " is attributed with [CommandPlugin] but does not inherit from CommandData");
                    }
                } 
                // Or is it an attributed ObjectPlugin ?
                else if (t.IsDefined(typeof(ObjectPluginAttribute), true))
                {
                    ObjectPluginAttribute attr = (ObjectPluginAttribute)Attribute.GetCustomAttribute(t, typeof(ObjectPluginAttribute), true);
                    Logger.Debug("  Class " + t.Name + " is attributed with [ObjectPlugin(ID=" + attr.ID + ", Name=\"" +
                                 attr.Name + "\")]");

                    if (InheritsFrom(t, typeof(ObjectDataM)))
                    {
                        // Register the object plugin
                        string name;
                        BaseBitmap bmp;
                        GetPluginDescription(t, attr, out name, out bmp);

                        ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor != null)
                        {
                            PluginAllocator pa = new PluginAllocator(ctor);
                            NodeDataAllocator nda = pa.Allocate;
                            _nodeAllocatorList.Add(nda);
                            C4dApi.RegisterObjectPlugin(attr.ID, name, attr.Info, nda, "obase", bmp, 0);
                        }
                        else
                        {
                            Logger.Warn("Class " + t.Name + " in " + fi.Name + " is missing a parameterless constructor");
                        }
                    }
                    else
                    {
                        Logger.Warn("  Class " + t.Name + " in " + fi.Name +
                                    " is attributed with [ObjectPlugin] but does not inherit from ObjectData");
                    }

                }
                // Or is it an attributed SceneSaverPlugin?
                else if (t.IsDefined(typeof (SceneSaverPluginAttribute), true))
                {
                    SceneSaverPluginAttribute attr =
                        (SceneSaverPluginAttribute)
                            Attribute.GetCustomAttribute(t, typeof (SceneSaverPluginAttribute), true);
                    Logger.Debug("  Class " + t.Name + " is attributed with [SceneSaverPlugin(ID=" + attr.ID +
                                 ", Name=\"" +
                                 attr.Name + "\")]");

                    if (InheritsFrom(t, typeof (SceneSaverData)))
                    {
                        // Register the object plugin
                        string name;
                        BaseBitmap bmp;
                        GetPluginDescription(t, attr, out name, out bmp);

                        ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                        if (ctor != null)
                        {
                            PluginAllocator pa = new PluginAllocator(ctor);
                            NodeDataAllocator nda = pa.Allocate;
                            _nodeAllocatorList.Add(nda);
                            C4dApi.RegisterSceneSaverPlugin(attr.ID, name, attr.Info, nda, "obase", attr.Suffix);
                        }
                        else
                        {
                            Logger.Warn("Class " + t.Name + " in " + fi.Name + " is missing a parameterless constructor");
                        }
                    }
                    else
                    {
                        Logger.Warn("  Class " + t.Name + " in " + fi.Name +
                                    " is attributed with [ObjectPlugin] but does not inherit from ObjectData");
                    }
                }

            }
        }

        private static bool InheritsFrom(Type t, Type ancestor)
        {
            bool isCommandData = false;
            for (Type bt = t.BaseType; bt != null; bt = bt.BaseType)
            {
                if (bt == ancestor)
                {
                    isCommandData = true;
                    break;
                }
            }
            return isCommandData;
        }

        private static void GetPluginDescription(Type t, PluginBaseAttribute attr, out string name, out BaseBitmap bmp)
        {
            name = attr.Name;
            if (string.IsNullOrEmpty(name))
                name = t.Name;

            // Falback icon
            String iconFile = null;
            if (!string.IsNullOrEmpty(attr.IconFile))
            {
                iconFile = Path.Combine(Path.GetDirectoryName(t.Assembly.Location), "res", attr.IconFile);
                if (!File.Exists(iconFile))
                    iconFile = null;
            }
            if (string.IsNullOrEmpty(iconFile))
                iconFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        "res", "CSharp.tif");
            bmp = BaseBitmap.AutoBitmap(iconFile);
        }

        private static string GetPluginsPath()
        {
            const string plugins = "Plugins";
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Remove(path.LastIndexOf(plugins, StringComparison.InvariantCultureIgnoreCase) + plugins.Length);
            return path;
        }

        public void End()
        {
            Logger.Debug("End()");

            foreach (object instance in _pluginInstanceList)
            {
                MethodInfo mi;
                if (null != (mi = instance.GetType().GetMethod("End")) )
                {
                    object ret = mi.Invoke(instance, null);
                    // TODO: Do something with the return value
                }
            }

            _pluginInstanceList.RemoveAll(o => true);
            _nodeAllocatorList.RemoveAll(pa => true);
        }


        public bool Message(int id)
        {
            Logger.Debug("Message(" + id + ")");
            return true;
        }
    }
}