
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Type-independent interface for classes that manage the out-of-core (on demand) loading of point clouds.
    /// </summary>
    public interface IPtOctantLoader
    {
        public bool ShowOctants { get; set; }

        /// <summary>
        /// The initial camera position.
        /// </summary>
        public double3 InitCamPos { get; set; }

        /// <summary>
        /// Used for breaking the loading loop when the application is shutting down.
        /// </summary>
        public bool IsShuttingDown { get; set; }

        /// <summary>
        /// The <see cref="RenderContext"/> the app uses.
        /// Needed to determine the field of view and the camera position.
        /// </summary>
        public RenderContext RC { get; }

        /// <summary>
        /// Initializes the <see cref="RC"/> dependent properties and starts the loading task.
        /// </summary>
        /// <param name="rc">The RenderContext for this loader.</param>
        public void Init(RenderContext rc);

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        public void UpdateScene();
        
        /// <summary>
        /// The root node of the octree that is used to render the point cloud.
        /// </summary>
        public SceneNode RootNode { get; set; }

        /// <summary>
        /// Is set to true internally when all visible nodes are loaded.
        /// </summary>
        public bool WasSceneUpdated { get; }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold { get; set; }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier { get; set; }

        /// <summary>
        /// The path to the folder that holds the file.
        /// </summary>
        public string FileFolderPath { get; set; }
    }
}