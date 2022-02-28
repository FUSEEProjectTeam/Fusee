using System;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Allows the use of Engine specific objects, properties and methods in Modules, e.g. PointCloud.
    /// </summary>
    public sealed class ModuleExtensionPoint : IDisposable
    {
        #region Singleton 
        private static readonly ModuleExtensionPoint _instance = new();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ModuleExtensionPoint() { }

        private ModuleExtensionPoint() { }

        /// <summary>
        /// Static instance property.
        /// </summary>
        public static ModuleExtensionPoint Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Delegate of the method that knows how to create a GpuMesh - without passing the RenderContext down into the module.
        /// Set in the constructor the RenderContext.
        /// </summary>
        public static CreateGpuMesh CreateGpuMesh
        {
            get { return Instance._createGpuMesh; }
            set { Instance._createGpuMesh = value; }
        }
        private CreateGpuMesh _createGpuMesh;

        #region Dispose
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        protected void Dispose(bool disposing)
        {

            if (!disposed)
            {

                if (disposing)
                {
                    // Dispose managed resources.
                }
                disposed = true;
            }
        }

        ~ModuleExtensionPoint()
        {
            Dispose(disposing: false);
        }
        #endregion
    }
}
