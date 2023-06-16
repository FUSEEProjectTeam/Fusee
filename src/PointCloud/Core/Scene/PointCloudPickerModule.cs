using CommunityToolkit.Diagnostics;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.Xene;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Fusee.Engine.Core.ScenePicker;

namespace Fusee.PointCloud.Core.Scene
{
    /// <summary>
    /// Result of a point cloud pick operation
    /// </summary>
    public class PointCloudPickResult : PickResult
    {
        /// <summary>
        /// The point mesh.
        /// </summary>
        public Mesh? Mesh;
        /// <summary>
        /// The index of the hit vertex, in this case the point index.
        /// </summary>
        public int VertIdx;
        /// <summary>
        /// The <see cref="OctantId"/> of the <see cref="PointCloudOctant"/>in which the found point lies.
        /// </summary>
        public OctantId OctantId;

        /// <summary>
        /// The distance between ray (origin: mouse position) and hit result (point)
        /// </summary>
        public float2 DistanceToRay;
    }

    /// <summary>
    /// Point cloud picker module. Inject to pick <see cref="PointCloudComponent"/>s
    /// </summary>
    public class PointCloudPickerModule : IPickerModule
    {
        private PickerState? _state;
        private readonly PointCloudOctree? _octree;
        private readonly IPointCloudImp<Mesh, VisualizationPoint>? _pcImp;
        private readonly float _pointSpacing;

        /// <summary>
        /// The pick result after picking.
        /// </summary>
        public List<PickResult> PickResults { get; set; } = new();

        internal struct MinPickValue
        {
            internal float2 Distance;
            internal Mesh Mesh;
            internal int VertIdx;
            internal OctantId OctantId;
        }

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

            if (float.IsInfinity(_state.PickPosClip.x) || float.IsInfinity(_state.PickPosClip.y))
                return;

            var proj = _state.CurrentCameraResult.Camera.GetProjectionMat(_state.ScreenSize.x, _state.ScreenSize.y, out _);
            var view = _state.CurrentCameraResult.View;
            var rayD = new RayD(new double2(_state.PickPosClip.x, _state.PickPosClip.y), (double4x4)view, (double4x4)proj);

            var tmpList = new List<PointCloudOctant>();
            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, rayD, tmpList).ToList();

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
                        var dist = SphereRayIntersection((float3)rayD.Origin, (float3)rayD.Direction, mesh.Vertices[i], _pointSpacing * 0.5f);
                        if (dist.x < 0 || dist.y < 0) continue;

                        if (dist.x <= currentMin.Distance.x && dist.y <= currentMin.Distance.y)
                        {
                            currentMin.Distance = dist;
                            currentMin.Mesh = mesh;
                            currentMin.VertIdx = i;
                            currentMin.OctantId = box.OctId;
                            //break; // <- check if break after first result is enough even for sparse point clouds
                        }
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
        /// Calculates the intersection distance between a ray and a sphere.
        /// </summary>
        /// <param name="ro"><see cref="RayF.Origin"/></param>
        /// <param name="rd"><see cref="RayF.Direction"/></param>
        /// <param name="ce">Center point of sphere/point</param>
        /// <param name="ra">Radius of sphere with center point of <paramref name="ce"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float2 SphereRayIntersection(float3 ro, float3 rd, float3 ce, float ra)
        {
            var oc = ro - ce;
            var b = float3.Dot(oc, rd);
            var c = float3.Dot(oc, oc) - ra * ra;
            var h = b * b - c;
            if (h < 0.0f) return new float2(-1.0f); // no intersection
            h = MathF.Sqrt(h);
            if (float.IsNaN(h) || float.IsInfinity(h)) return new float2(-1.0f);
            return new float2(-b - h, -b + h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<PointCloudOctant> PickOctantRecursively(PointCloudOctant node, RayD ray, List<PointCloudOctant> list)
        {
            list.Add(node);
            if (node.Children[0] != null)
            {
                foreach (var child in node.Children.Cast<PointCloudOctant>())
                {
                    if (child?.IsVisible == true && child.IntersectRay(ray))
                    {
                        PickOctantRecursively(child, ray, list);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Inject this to a <see cref="ScenePicker"/> to be able to pick <see cref="PointCloudComponent"/>s.
        /// The actual point and <see cref="PointCloudOctree"/> data needs to be present a priori, however it's type is polymorph, therefore we need to inject those data, too.
        /// </summary>
        /// <param name="octree">The <see cref="PointCloudOctree"/> of the <see cref="IPointCloudImp{TGpuData, TPoint}"/>.</param>
        /// <param name="pcImp">The <see cref="IPointCloudImp{TGpuData, TPoint}"/>, needs to be of type <see cref="Mesh"/></param>
        /// <param name="pointSpacing">The spacing between points. For Potree use the metadata spacing component * 0.1f <br/> e. g. Spacing = 2.18f, pass 0.218f to this ctor.</param>
        public PointCloudPickerModule(IPointCloudOctree octree, IPointCloudImp<Mesh, VisualizationPoint> pcImp, float pointSpacing)
        {
            if (pcImp == null)
                Diagnostics.Warn("No per point picking possible, no PointCloud<Mesh> type loaded");

            _octree = (PointCloudOctree)octree;
            _pcImp = pcImp;
            _pointSpacing = pointSpacing;
        }

        /// <summary>
        /// Set the current <see cref="PickerState"/> from external (this is done automatically by the <see cref="Visitor{TNode, TComponent}.Traverse(TNode)"/> method.
        /// </summary>
        /// <param name="state"></param>
        public void SetState(PickerState state)
        {
            _state = state;
        }
    }
}