using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Text;
using Fusee.PointCloud.Core;
using static Fusee.Engine.Core.ScenePicker;
using System.Linq;
using Microsoft.Extensions.Options;
using Fusee.PointCloud.Common;
using Fusee.Base.Core;
using Fusee.Structures;

namespace Fusee.PointCloud.Core.Scene
{
    public class PointCloudPickResult : PickResult
    {
        public OctantId OctandID;
        public PointCloudOctant Octant;
    }

    public class PointCloudPickerModule : IPickerModule
    {
        private PickerState State;
        private PointCloudOctree _octree;
        private IPointCloudImp<Mesh> _pcImp;

        public PickResult PickResult { get; set; }

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            if (!pointCloud.Active) return;

            var proj = (double4x4)State.CurrentCameraResult.Camera.GetProjectionMat(State.ScreenSize.x, State.ScreenSize.y, out _);
            var view = (double4x4)State.CurrentCameraResult.View;
            var ray = new RayD(new double2(State.PickPosClip.x, State.PickPosClip.y), view, proj);

            var tmpList = new List<PointCloudOctant>();
            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, ray, tmpList).ToList();

            if (allHitBoxes == null || allHitBoxes.Count == 0) return;
            foreach (var box in allHitBoxes)
                box.ComputeScreenProjectedSize(view.Translation(), State.ScreenSize.y, State.CurrentCameraResult.Camera.Fov, (float3)box.Center, new float3((float)box.Size));
            var octant = allHitBoxes.OrderBy(x => x.ProjectedScreenSize).ToList()[0];

            //foreach (var box in allHitBoxes)
            //    if ((float)box.Center.x - State.View.Translation().x < (float)octant.Center.x - State.View.Translation().x
            //       && (float)box.Center.y - State.View.Translation().y < (float)octant.Center.y - State.View.Translation().y
            //       && (float)box.Center.z - State.View.Translation().z < (float)octant.Center.z - State.View.Translation().z)
            //        octant = box;

            if (allHitBoxes != null && allHitBoxes.Any())
            {
                var mvp = (float4x4)proj * (float4x4)view * State.Model;
                PickResult = new PointCloudPickResult
                {
                    Node = null,
                    Projection = (float4x4)proj,
                    View = (float4x4)view,
                    Model = State.Model,
                    ClipPos = float4x4.TransformPerspective(mvp, (float3)allHitBoxes.ElementAt(0).Center),
                    OctandID = octant.OctId,
                    Octant = octant
                };
            }
        }

        private List<PointCloudOctant> PickOctantRecursively(PointCloudOctant node, RayD ray, List<PointCloudOctant> list)
        {
            if (node?.IsVisible == true && node.IntersectRay(ray))
            {
                list.Add(node);
            }

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
