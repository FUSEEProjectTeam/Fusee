using System;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    public static class NvidiaDedicatedGpuEnabler
    {
        // Found at https://stackoverflow.com/a/46749111

        [System.Runtime.InteropServices.DllImport("nvapi64.dll", EntryPoint = "fake")]
        static extern int LoadNvApi64();

        [System.Runtime.InteropServices.DllImport("nvapi.dll", EntryPoint = "fake")]
        static extern int LoadNvApi32();

        public static void InitializeDedicatedGraphics()
        {
            try
            {
                if (Environment.Is64BitProcess)
                    LoadNvApi64();
                else
                    LoadNvApi32();
            }
            catch { } // will always fail since 'fake' entry point doesn't exists
        }
    }
}
