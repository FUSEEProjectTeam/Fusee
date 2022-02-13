
using Fusee.Engine.Core.Scene;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Type-independent interface for classes that can read out-of-core-files.
    /// </summary>
    public interface IPtOctreeFileReader
    {
        /// <summary>
        /// Reads the meta.json and .hierarchy files and returns an octree in form of <see cref="SceneNode"/>s.
        /// The Octant-data is stored in <see cref="OctantD"/>s or <see cref="OctantF"/>s.
        /// </summary>
        public SceneNode GetScene();

        /// <summary>
        /// Number of octants/nodes that are currently loaded.
        /// </summary>
        public int NumberOfOctants { get; }
    }
}