using CommunityToolkit.HighPerformance.Buffers;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;
using System.Collections.Generic;

namespace Fusee.PointCloud.Potree
{
    /// <summary>
    /// Non-point-type-specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2CloudDynamic : IPointCloudImp<Mesh, VisualizationPoint>
    {
        /// <summary>
        /// Object for handling the invalidation of the gpu data cache.
        /// </summary>
        public InvalidateGpuDataCache InvalidateGpuDataCache { get => DataHandler.InvalidateCacheToken; }

        /// <summary>
        /// The complete list of meshes that can be rendered.
        /// </summary>
        public List<Mesh> GpuDataToRender { get; set; }

        /// <summary>
        /// Nows which octants are visible and when to trigger the point loading.
        /// </summary>
        public VisibilityTester VisibilityTester { get; }

        /// <summary>
        /// Handles the point and mesh data.
        /// </summary>
        public PointCloudDataHandlerBase<Mesh> DataHandler { get; }

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
        public float3 Center { get; private set; }

        /// <summary>
        /// The size (longest edge) of the point clouds AABB / Octree root.
        /// </summary>
        public float3 Size { get; private set; }

        /// <summary>
        /// Action that is run on every mesh that is determined as newly visible.
        /// </summary>
        public Action<Mesh>? NewMeshAction
        {
            get => _newMeshAction;
            set
            {
                _newMeshAction = value;
                (DataHandler).NewMeshAction = _newMeshAction;
            }
        }
        private Action<Mesh>? _newMeshAction;

        /// <summary>
        /// Action that is run on every mesh that was updated.
        /// </summary>
        public Action<Mesh>? UpdatedMeshAction
        {
            get => _updateMeshAction;
            set
            {
                _updateMeshAction = value;
                DataHandler.UpdatedMeshAction = _updateMeshAction;
            }
        }
        private Action<Mesh>? _updateMeshAction;

        private bool _doUpdate = true;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        public Potree2CloudDynamic(PointCloudDataHandlerBase<Mesh> dataHandler, IPointCloudOctree octree, float3 size, float3 center)
        {
            GpuDataToRender = new();
            DataHandler = dataHandler;
            DataHandler.UpdateGpuDataCache = UpdateGpuDataCache;
            VisibilityTester = new VisibilityTester(octree, dataHandler.TriggerPointLoading);
            Size = size;
            Center = center;
        }

        /// <summary>
        /// Allows to update meshes with data from the points.
        /// </summary>
        /// <param name="meshes">The meshes that have to be updated.</param>
        /// <param name="points">The points with the desired values.</param>
        public void UpdateGpuDataCache(ref IEnumerable<Mesh> meshes, MemoryOwner<VisualizationPoint> points)
        {
            var countStartSlice = 0;

            foreach (var mesh in meshes)
            {
                mesh.Name = string.Empty;
                if (mesh.Flags == null) continue;
                var slice = points.Span.Slice(countStartSlice, mesh.Flags.Length);

                for (int i = 0; i < slice.Length; i++)
                {
                    var pt = slice[i];
                    if (mesh.Flags[i] != pt.Flags)
                        mesh.Flags[i] = pt.Flags;
                }
                countStartSlice += mesh.Flags.Length;
            }
        }

        /// <summary>
        /// Determines if new Meshes should be loaded.
        /// </summary>
        public bool LoadNewMeshes { get; set; } = true;


        private List<OctantId> _visibleOctantsCache = new();

        /// <summary>
        /// Uses the <see cref="VisibilityTester"/> and <see cref="PointCloudDataHandler{TGpuData}"/> to update the visible meshes.
        /// Called every frame.
        /// </summary>
        /// <param name="fov">The camera's field of view.</param>
        /// <param name="viewportHeight">The viewport height.</param>
        /// <param name="renderFrustum">The camera's frustum.</param>
        /// <param name="camPos">The camera position in world coordinates.</param>
        /// /// <param name="modelMat">The model matrix of the SceneNode the PointCloud(Component) is part of.</param>
        public void Update(float fov, int viewportHeight, FrustumF renderFrustum, float3 camPos, float4x4 modelMat)
        {
            DataHandler.ProcessDisposeQueue();

            if (!LoadNewMeshes)
                return;

            if (!_doUpdate &&
                renderFrustum == VisibilityTester.RenderFrustum &&
                viewportHeight == VisibilityTester.ViewportHeight &&
                fov == VisibilityTester.Fov &&
                camPos == VisibilityTester.CamPos) return;

            VisibilityTester.RenderFrustum = renderFrustum;
            VisibilityTester.ViewportHeight = viewportHeight;
            VisibilityTester.Fov = fov;
            VisibilityTester.CamPos = camPos;
            VisibilityTester.Model = modelMat;

            VisibilityTester.Update();
            GpuDataToRender.Clear();

            var currentOctants = new List<OctantId>();

            foreach (var guid in VisibilityTester.VisibleNodes)
            {
                if (!guid.Valid) continue;

                var guidMeshes = DataHandler.GetGpuData(guid, () => !_visibleOctantsCache.Contains(guid));

                if (guidMeshes != null)
                    GpuDataToRender.AddRange(guidMeshes);

                currentOctants.Add(guid);
            }

            _visibleOctantsCache = new(currentOctants);
        }
    }
}