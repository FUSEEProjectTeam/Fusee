
using Fusee.Engine.Core.Scene;

namespace Fusee.PointCloud.Common
{
    public interface IPtOctreeFileReader
    {
        public SceneNode GetScene();

        public int NumberOfOctants { get; }
    }
}