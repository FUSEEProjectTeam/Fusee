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

namespace Fusee.PointCloud.Core.Scene
{
    public class PointCloudPickResult : PickResult
    {
        public OctantId OctandID;

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

            var ray = new RayD(new double2(PickerState.PickPosClip.x, PickerState.PickPosClip.y), (double4x4)State.View, (double4x4)State.Projection);
            var tmpList = new List<PointCloudOctant>();
            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, ray, tmpList).OrderBy(x => x.ProjectedScreenSize);
            if (allHitBoxes != null && allHitBoxes.Any())
            {
                var mvp = State.Projection * State.View * State.Model;
                PickResult = new PointCloudPickResult
                {
                    Node = null,
                    Projection = State.Projection,
                    View = State.View,
                    Model = State.Model,
                    ClipPos = float4x4.TransformPerspective(mvp, (float3)allHitBoxes.ElementAt(0).Center),
                    OctandID = allHitBoxes.ElementAt(0).OctId,
                };
            }
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
