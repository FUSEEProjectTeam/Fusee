using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.PotreeReader.V1;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Will render a Potree 1.0 Point Cloud if visited by the SceneRenderer.
    /// </summary>
    /// <typeparam name="TPoint">The type of the point cloud points.</typeparam>
    public class PointCloud<TPoint> : SceneComponent where TPoint : new()
    {
        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        /// <param name="pointAccessor"></param>
        /// <param name="fileFolderPath"></param>
        /// <param name="pointType"></param>
        public PointCloud(PointAccessor<TPoint> pointAccessor, string fileFolderPath, PointType pointType)
        {
            _pointCloudLoader = new PointCloudLoader<TPoint>(fileFolderPath, pointType)
            {
                Octree = ReadPotreeData<TPoint>.GetOctree(pointAccessor, fileFolderPath),
                FileFolderPath = fileFolderPath,
                PtAccessor = pointAccessor
            };
            Center = new float3(_pointCloudLoader.Octree.Root.Center);
            Size = (float)_pointCloudLoader.Octree.Root.Size;
        }

        /// <summary>
        /// Center of the point cloud.
        /// </summary>
        public float3 Center { get; private set; }

        /// <summary>
        /// Size of the point clouds (quadratic) bounding box.
        /// </summary>
        public float Size { get; private set; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => _pointCloudLoader.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => _pointCloudLoader.MinProjSizeModifier;
            set
            {
                _pointCloudLoader.MinProjSizeModifier = value;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - tradeoff between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get => _pointCloudLoader.PointThreshold;
            set
            {
                _pointCloudLoader.PointThreshold = value;
            }
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public double UpdateRate
        {
            get => _pointCloudLoader.UpdateRate;
            set
            {
                _pointCloudLoader.UpdateRate = value;
            }
        }

        internal float Fov;
        internal float3 CamPos;
        internal FrustumF RenderFrustum;
        internal int ViewportHeight;

        private PointCloudLoader<TPoint> _pointCloudLoader;
        internal List<Mesh> GetMeshes()
        {
            _pointCloudLoader.RenderFrustum = RenderFrustum;
            _pointCloudLoader.ViewportHeight = ViewportHeight;
            _pointCloudLoader.Update(Fov, CamPos);
            
            return _pointCloudLoader.MeshesToRender;
        }
    }
}
