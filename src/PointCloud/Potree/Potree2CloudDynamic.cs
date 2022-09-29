using Fusee.Base.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Fusee.PointCloud.Potree
{
    /// <summary>
    /// Non-point-type-specific implementation of Potree2 clouds.
    /// </summary>
    public class Potree2CloudDynamic : IPointCloudImp<Mesh>
    {
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
        public float3 Center => (float3)VisibilityTester.Octree.Root.Center;

        /// <summary>
        /// The size (longest edge) of the point clouds AABB / Octree root.
        /// </summary>
        public float3 Size => new((float)VisibilityTester.Octree.Root.Size);

        private readonly GetDynamicMeshes _getMeshes;
        private bool _doUpdate = true;

        /// <summary>
        /// Creates a new instance of type <see cref="PointCloud"/>
        /// </summary>
        public Potree2CloudDynamic(PointCloudDataHandlerBase<Mesh> dataHandler, IPointCloudOctree octree)
        {
            GpuDataToRender = new List<Mesh>();
            DataHandler = dataHandler;
            VisibilityTester = new VisibilityTester(octree, dataHandler.TriggerPointLoading);
            _getMeshes = dataHandler.GetGpuData;
        }

        /// <summary>
        /// Action that is run on every mesh that is loaded to be visible.
        /// </summary>
        public Action<Mesh> NewMeshAction;

        /// <summary>
        /// Uses the <see cref="VisibilityTester"/> and <see cref="PointCloudDataHandler{TGpuData, TPoint}"/> to update the visible meshes.
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

            var meshes = new List<Mesh>();

            foreach (var guid in VisibilityTester.VisibleNodes)
            {
                if (!guid.Valid) continue;

                var guidMeshes = _getMeshes(guid);

                if (guidMeshes == null) continue; //points for this octant aren't loaded yet.

                meshes.AddRange(guidMeshes);
            }

            if (NewMeshAction != null)
            {
                var newMeshes = meshes.Except(GpuDataToRender);

                if (newMeshes.Any())
                {
                    //Diagnostics.Debug($"New meshes {newMeshes.Count()}");

                    foreach (var mesh in newMeshes)
                    {
                        NewMeshAction(mesh);
                    }
                }
            }

            GpuDataToRender.Clear();
            GpuDataToRender.AddRange(meshes);
        }
    }
}