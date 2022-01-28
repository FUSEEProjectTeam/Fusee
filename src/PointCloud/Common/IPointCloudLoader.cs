using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
    public interface IPointCloudLoader : IDisposable
    {
        /// <summary>
        ///All nodes that are visible in this frame.
        /// </summary>
        public List<string> VisibleNodes { get; }

        /// <summary>
        /// Is set to true internally when all visible nodes are loaded.
        /// </summary>
        public bool WasSceneUpdated { get; }

        /// <summary>
        /// Current Field of View - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float Fov { get; set; }

        /// <summary>
        /// Current camera position - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public float3 CamPos { get; set; }

        /// <summary>
        /// Current height of the viewport - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public int ViewportHeight { get; set; }

        /// <summary>
        /// Current camera frustum - set by the SceneRenderer if a PointCloud Component is visited.
        /// </summary>
        public FrustumF RenderFrustum { get; set; }

        /// <summary>
        /// Provides access to properties of different point types.
        /// </summary>
        public IPointAccessor PtAccessor { get; }

        /// <summary>
        /// The octree structure of the point cloud.
        /// </summary>
        public IPointCloudOctree Octree
        {
            get;
        }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints { get;}

        /// <summary>
        /// Changes the minimum size of octants. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get;
            set;
        }

        /// <summary>
        /// The path to the folder that holds the file.
        /// </summary>
        public string FileFolderPath { get; }

        /// <summary>
        /// Maximal number of points that are visible in one frame - trade off between performance and quality.
        /// </summary>
        public int PointThreshold { get; set; }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public double UpdateRate { get; set; }

        /// <summary>
        /// Updates the visible octree hierarchy in the scene and updates the VisibleOctreeHierarchyTex in the shaders.
        /// </summary>
        public void Update();
    }
}