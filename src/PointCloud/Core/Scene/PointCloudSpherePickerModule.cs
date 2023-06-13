using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Xene;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Fusee.Engine.Core.ScenePicker;
using static Fusee.PointCloud.Core.Scene.PointCloudPickerModule;

namespace Fusee.PointCloud.Core.Scene
{
    /// <summary>
    /// Point cloud picker module. Inject to pick <see cref="PointCloudComponent"/>s via <see cref="Sphere"/>
    /// </summary>

    public class PointCloudSpherePickerModule : IPickerModule
    {
        private PickerState? _state;
        private readonly PointCloudOctree? _octree;
        private readonly IPointCloudImp<Mesh, VisualizationPoint>? _pcImp;

        /// <summary>
        /// The picked meshs with triangle idx (pt)
        /// </summary>
        public List<PickResult> PickResults { get; set; } = new();

        public double3 SpherePosition { get; set; }
        public double3 SphereScale { get; set; }

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            PickResults.Clear();
            if (!pointCloud.Active) return;

            Guard.IsNotNull(_pcImp);
            Guard.IsNotNull(_octree);
            Guard.IsNotNull(_state);

            var invSphereModelMatrix = (double4x4.CreateScale(SphereScale) * double4x4.CreateTranslation(SpherePosition)).Invert();

            var tmpList = new List<PointCloudOctant>();
            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, invSphereModelMatrix, tmpList).ToList();

            if (allHitBoxes == null || allHitBoxes.Count == 0) return;

            var currentRes = new ConcurrentBag<MinPickValue>();

            Parallel.ForEach(_pcImp.GpuDataToRender, (mesh) =>
            {
                foreach (var box in allHitBoxes)
                {
                    if (!mesh.BoundingBox.Intersects(new AABBf((float3)box.Min, (float3)box.Max))) continue;

                    var currentMin = new MinPickValue
                    {
                        Distance = float2.One * float.MaxValue
                    };

                    Guard.IsNotNull(mesh.Vertices);

                    for (var i = 0; i < mesh.Vertices.Length; i++)
                    {
                        //var dist = SphereRayIntersection((float3)rayD.Origin, (float3)rayD.Direction, mesh.Vertices[i], _pointSpacing * 0.5f);
                        //if (dist.x < 0 || dist.y < 0) continue;

                        //if (dist.x <= currentMin.Distance.x && dist.y <= currentMin.Distance.y)
                        //{
                        //    currentMin.Distance = dist;
                        //    currentMin.Mesh = mesh;
                        //    currentMin.VertIdx = i;
                        //    currentMin.OctantId = box.OctId;
                        //    //break; // <- check if break after first result is enough even for sparse point clouds
                        //}
                    }

                    if (currentMin.Mesh == null) continue;
                    currentRes.Add(currentMin);
                }
            });

            if (currentRes == null || currentRes.IsEmpty) return;


            var mvp = proj * view * _state.Model;

            foreach (var r in currentRes)
            {
                var pickRes = new PointCloudPickResult
                {
                    Node = null,
                    Projection = proj,
                    View = view,
                    Model = _state.Model,
                    ClipPos = float4x4.TransformPerspective(mvp, r.Mesh.Vertices[r.VertIdx]),
                    DistanceToRay = r.Distance,
                    Mesh = r.Mesh,
                    VertIdx = r.VertIdx,
                    OctantId = r.OctantId
                };

                PickResults.Add(pickRes);
            }
        }

        /// <summary>
        /// Pick octant via Sphere/AABB intersection
        /// </summary>
        /// <param name="node"></param>
        /// <param name="invSphereModelMatrix"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<PointCloudOctant> PickOctantRecursively(PointCloudOctant node, double4x4 invSphereModelMatrix, List<PointCloudOctant> list)
        {
            list.Add(node);
            if (node.Children[0] != null)
            {
                foreach (var child in node.Children.Cast<PointCloudOctant>())
                {
                    if (child?.IsVisible == true && child.InsideOrIntersecting(invSphereModelMatrix))
                    {
                        PickOctantRecursively(child, invSphereModelMatrix, list);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Set the current <see cref="PickerState"/> from external (this is done automatically by the <see cref="Visitor{TNode, TComponent}.Traverse(TNode)"/> method.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(PickerState state)
        {
            _state = state;
        }

        /// <summary>
        /// Inject this to a <see cref="ScenePicker"/> to be able to pick <see cref="PointCloudComponent"/>s.
        /// The actual point and <see cref="PointCloudOctree"/> data needs to be present a priori, however it's type is polymorph, therefore we need to inject those data, too.
        /// </summary>
        /// <param name="octree">The <see cref="PointCloudOctree"/> of the <see cref="IPointCloudImp{TGpuData, TPoint}"/>.</param>
        /// <param name="pcImp">The <see cref="IPointCloudImp{TGpuData, TPoint}"/>, needs to be of type <see cref="Mesh"/></param>
        public PointCloudSpherePickerModule(IPointCloudOctree octree, IPointCloudImp<Mesh, VisualizationPoint>? pcImp)
        {
            if (pcImp == null)
                Diagnostics.Warn("No per point picking possible, no PointCloud<Mesh> type loaded");

            _octree = (PointCloudOctree)octree;
            _pcImp = pcImp;
        }
    }
}
