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

        ///The maximal number of lights we can render when using the forward pipeline.
        public const int NumberOfLightsForward = 8;

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        private void Dispose(bool disposing)
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

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~ModuleExtensionPoint()
        {
            Dispose(disposing: false);
        }
        #endregion
    }
}