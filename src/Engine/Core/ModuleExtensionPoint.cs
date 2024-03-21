using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Allows the use of Engine specific objects, properties and methods in Modules, e.g. PointCloud.
    /// </summary>
    public sealed class ModuleExtensionPoint
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
        public static CreateGpuMesh? CreateGpuMesh
        {
            get { return Instance._createGpuMesh; }
            set { Instance._createGpuMesh = value; }
        }
        private CreateGpuMesh? _createGpuMesh;

        /// <summary>
        /// Bound to the platform specific RenderContext implementation. Set by the <see cref="RenderContext"/>.
        /// </summary>
        public static FuseePlatformId PlatformId { get; internal set; }
    }
}