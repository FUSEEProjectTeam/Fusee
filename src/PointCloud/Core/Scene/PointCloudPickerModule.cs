using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.Xene;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Fusee.Engine.Core.ScenePicker;
using System.Linq;
using Microsoft.Extensions.Options;
using Fusee.PointCloud.Common;
using Fusee.Base.Core;
using Fusee.Structures;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;

namespace Fusee.PointCloud.Core.Scene
{
    public class PointCloudPickResult : PickResult
    {
        public Mesh Mesh;
        public int VertIdx;
        public OctantId OctantId;
    }

    public class PointCloudPickerModule : IPickerModule
    {
        private PickerState State;
        private PointCloudOctree _octree;
        private IPointCloudImp<Mesh> _pcImp;

        public PickResult PickResult { get; set; }

        internal struct MinPickValue
        {
            internal float2 Distance;
            internal Mesh Mesh;
            internal int VertIdx;
            internal OctantId OctantId;
        }

        private Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            sw.Start();
            if (!pointCloud.Active) return;

            var proj = State.CurrentCameraResult.Camera.GetProjectionMat(State.ScreenSize.x, State.ScreenSize.y, out _);
            var view = State.CurrentCameraResult.View;
            var rayD = new RayD(new double2(State.PickPosClip.x, State.PickPosClip.y), (double4x4)view, (double4x4)proj);

            Diagnostics.Info($"Setup took: {sw.ElapsedTicks}");
            sw.Restart();

            var tmpList = new List<PointCloudOctant>();
            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, rayD, tmpList).ToList();

            Diagnostics.Info($"PickOctantRecursively took: {sw.ElapsedTicks}");
            sw.Restart();


            if (allHitBoxes == null || allHitBoxes.Count == 0) return;

            var currentRes = new ConcurrentBag<MinPickValue>();

            Parallel.ForEach(_pcImp.GpuDataToRender, (mesh) =>
            //foreach(var mesh in _pcImp.GpuDataToRender)
            {
                foreach (var box in allHitBoxes)
                {
                    if (!mesh.BoundingBox.Intersects(new AABBf((float3)box.Min, (float3)box.Max))) continue;

                    var currentMin = new MinPickValue
                    {
                        Distance = float2.One * float.MaxValue
                    };

                    for (var i = 0; i < mesh.Vertices.Length; i++)
                    {
                        var dist = SphereRayIntersection((float3)rayD.Origin, (float3)rayD.Direction, mesh.Vertices[i], 0.017f); // spacing * 0.1?
                        if (dist == float2.One * -1) continue;

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

            Diagnostics.Info($"foreach took: {sw.ElapsedTicks}");
            sw.Restart();

            if (currentRes == null || currentRes.Count == 0) return;

            var minElement = currentRes.First();

            foreach (var r in currentRes)
            {
                if (r.Distance.x < minElement.Distance.x && r.Distance.y < minElement.Distance.y)
                {
                    minElement = r;
                }
            }

            Diagnostics.Info($"Min element took: {sw.ElapsedTicks}");
            sw.Restart();

            var mvp = proj * view * State.Model;
            PickResult = new PointCloudPickResult
            {
                Node = null,
                Projection = proj,
                View = view,
                Model = State.Model,
                ClipPos = float4x4.TransformPerspective(mvp, minElement.Mesh.Vertices[minElement.VertIdx]),
                Mesh = minElement.Mesh,
                VertIdx = minElement.VertIdx,
                OctantId = minElement.OctantId
            };

        }


        // sphere of size ra centered at point ce
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        float2 SphereRayIntersection(float3 ro, float3 rd, float3 ce, float ra)
        {
            var oc = ro - ce;
            float b = float3.Dot(oc, rd);
            var qc = oc - b * rd;
            float h = ra * ra - float3.Dot(qc, qc);
            if (h < 0.0) new float2(-1.0f); // no intersection
            h = MathF.Sqrt(h);
            if (float.IsNaN(h)) return new float2(-1.0f);
            return new float2(-b - h, -b + h);
        }

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

        public PointCloudPickerModule(IPointCloudOctree octree, IPointCloudImp<Mesh> pcImp)
        {
            if (pcImp == null)
                Diagnostics.Warn("No per point picking possible, no PointCloud<Mesh> type loaded");

            _octree = (PointCloudOctree)octree;
            _pcImp = pcImp;
        }

        public void SetState(PickerState state)
        {
            State = state;
        }
    }
}