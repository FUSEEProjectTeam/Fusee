using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Base.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Potree
{
    /// <summary>
    /// Non-point-type-specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2CloudInstanced : IPointCloudImp<InstanceData, VisualizationPoint>
    {
        public InvalidateGpuDataCache InvalidateGpuDataCache { get; } = new();

        /// <summary>
        /// The complete list of meshes that can be rendered.
        /// </summary>
        public List<InstanceData> GpuDataToRender { get; set; }

        /// <summary>
        /// Nows which octants are visible and when to trigger the point loading.
        /// </summary>
        public VisibilityTester VisibilityTester { get; }

        /// <summary>
        /// Handles the point and mesh data.
        /// </summary>
        public PointCloudDataHandlerBase<InstanceData> DataHandler { get; }

        /// <summary>
        /// The number of points that are currently visible.
        /// </summary>
        public int NumberOfVisiblePoints
        {
            get => VisibilityTester.NumberOfVisiblePoints;
        }

        /// <summary>
        /// Changes the minimum size of a octant. If an octant is smaller it won't be rendered.
        /// </summary>
        public float MinProjSizeModifier
        {
            get => VisibilityTester.MinProjSizeModifier;
            set
            {
                _doUpdate = true;
                VisibilityTester.MinProjSizeModifier = value;
            }
        }

        /// <summary>
        /// Maximal number of points that are visible in one frame - trade-off between performance and quality.
        /// </summary>
        public int PointThreshold
        {
            get => VisibilityTester.PointThreshold;
            set
            {
                _doUpdate = true;
                VisibilityTester.PointThreshold = value;
            }
        }

        /// <summary>
        /// The amount of milliseconds needed to pass before rendering next frame
        /// </summary>
        public float UpdateRate
        {
            get => VisibilityTester.UpdateRate;
            set
            {
                VisibilityTester.UpdateRate = value;
            }
        }

        /// <summary>
        /// The center of the point clouds AABB / Octree root.
        /// </summary>
        public float3 Center => (float3)VisibilityTester.Octree.Root.Center;

        /// <summary>
        /// The size (longest edge) of the point clouds AABB / Octree root.
        /// </summary>
        public float3 Size => new((float)VisibilityTester.Octree.Root.Size);

        private bool _doUpdate = true;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        public Potree2CloudInstanced(PointCloudDataHandlerBase<InstanceData> dataHandler, IPointCloudOctree octree)
        {
            GpuDataToRender = new List<InstanceData>();
            DataHandler = dataHandler;
            DataHandler.UpdateGpuDataCache = UpdateGpuDataCache;
            VisibilityTester = new VisibilityTester(octree, dataHandler.TriggerPointLoading);
        }

        /// <summary>
        /// Allows to update meshes with data from the points.
        /// </summary>
        /// <param name="meshes">The meshes that have to be updated.</param>
        /// <param name="points">The points with the desired values.</param>
        public void UpdateGpuDataCache(ref IEnumerable<InstanceData> meshes, MemoryOwner<VisualizationPoint> points)
        {
            Diagnostics.Warn("Not implemented. Cache will not be updated.");
        }

        /// <summary>
        /// Uses the <see cref="VisibilityTester"/> and <see cref="PointCloudDataHandler{TGpuData}"/> to update the visible meshes.
        /// Called every frame.
        /// </summary>
        /// <param name="fov">The camera's field of view.</param>
        /// <param name="viewportHeight">The viewport height.</param>
        /// <param name="renderFrustum">The camera's frustum.</param>
        /// <param name="camPos">The camera position in world coordinates.</param>
        /// <param name="modelMat">The model matrix of the SceneNode the PointCloud(Component) is part of.</param>
        public void Update(float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos, float4x4 modelMat)
        {
            DataHandler.ProcessDisposeQueue();

            if (!_doUpdate &&
                renderFrustum == VisibilityTester.RenderFrustum &&
                viewportHeight == VisibilityTester.ViewportHeight &&
                fov == VisibilityTester.Fov &&
                camPos == VisibilityTester.CamPos) return;

            GpuDataToRender.Clear();

            VisibilityTester.RenderFrustum = renderFrustum;
            VisibilityTester.ViewportHeight = viewportHeight;
            VisibilityTester.Fov = fov;
            VisibilityTester.CamPos = camPos;
            VisibilityTester.Model = modelMat;

            VisibilityTester.Update();

            foreach (var guid in VisibilityTester.VisibleNodes)
            {
                if (!guid.Valid) continue;

                var instanceData = DataHandler.GetGpuData(guid, null, out _);

                if (instanceData == null) continue; //points for this octant aren't loaded yet.

                GpuDataToRender.AddRange(instanceData);
            }
        }
    }
}