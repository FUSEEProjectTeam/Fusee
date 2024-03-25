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
        public float DistanceToRay;
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
            internal float Distance;
            internal Mesh Mesh;
            internal int VertIdx;
            internal OctantId OctantId;
        }

        /// <summary>
        /// Picks visible points.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void PickPointCloud(PointCloudComponent pointCloud)
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

            var results = new ConcurrentBag<MinPickValue>();

            Parallel.ForEach(_pcImp.GpuDataToRender, (mesh) =>
            {
                foreach (var box in allHitBoxes)
                {
                    var minDistBox = float.MaxValue;

                    if (!mesh.BoundingBox.Intersects(new AABBf((float3)box.Min, (float3)box.Max))) continue;

                    Guard.IsNotNull(mesh.Vertices);
                    var levelDependentSpacing = (_pointSpacing * MathF.Pow(2, box.Level)) / 2f;

                    for (var i = 0; i < mesh.Vertices.Length; i++)
                    {
                        var dist = CalculateDistance(mesh.Vertices[i], (float3)rayD.Origin, (float3)rayD.Direction);
                        if (dist < 0) continue;

                        if (dist <= levelDependentSpacing && dist < minDistBox)
                        {
                            minDistBox = dist;
                            results.Add(new MinPickValue()
                            {
                                Distance = dist,
                                Mesh = mesh,
                                VertIdx = i,
                                OctantId = box.OctId
                            });
                        }
                    }
                }

            });

            var mvp = proj * view * _state.Model;

            foreach (var res in results)
            {
                Guard.IsNotNull(res.Mesh.Vertices);
                var pickRes = new PointCloudPickResult
                {
                    Node = null,
                    Projection = proj,
                    View = view,
                    Model = _state.Model,
                    ClipPos = float4x4.TransformPerspective(mvp, res.Mesh.Vertices[res.VertIdx]),
                    DistanceToRay = res.Distance,
                    Mesh = res.Mesh,
                    VertIdx = res.VertIdx,
                    OctantId = res.OctantId
                };

                PickResults.Add(pickRes);
            }
        }

        private static float CalculateDistance(float3 point, float3 rayOrigin, float3 rayDirection)
        {
            if (float3.Dot(point - rayOrigin, rayDirection) < 0) //point is behind the ray's origin
                return -1;
            return float3.Cross(rayDirection, point - rayOrigin).Length;
        }

        /// <summary>
        /// Calculates the intersection distance between a ray and a sphere.
        /// </summary>
        /// <param name="rayOrigin"><see cref="RayF.Origin"/></param>
        /// <param name="rayDirection"><see cref="RayF.Direction"/></param>
        /// <param name="sphereCenter">Center point of sphere/point</param>
        /// <param name="sphereRad">Radius of sphere with center point of <paramref name="sphereCenter"/></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static float2 SphereRayIntersection(float3 rayOrigin, float3 rayDirection, float3 sphereCenter, float sphereRad)
        {
            var oc = rayOrigin - sphereCenter;
            var b = float3.Dot(oc, rayDirection);
            var c = float3.Dot(oc, oc) - sphereRad * sphereRad;
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

        private List<PointCloudOctant> PickOctantIterative(PointCloudOctant node, RayD ray, List<PointCloudOctant> list)
        {
            var stack = new Stack<PointCloudOctant>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current?.IsVisible == true && current.IntersectRay(ray))
                {
                    list.Add(current);
                }

                if (current == null)
                    continue;

                foreach (var child in current.Children)
                {
                    if (child != null)
                        stack.Push((PointCloudOctant)child);
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