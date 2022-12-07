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

namespace Fusee.PointCloud.Core.Scene
{
    public class PointCloudPickerModule : IPickerModule
    {
        private PickerState State;
        private PointCloudOctree _octree;

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            Console.WriteLine($"MousePos: {State.PickPosClip}");

            var ray = new RayD(new double2(State.PickPosClip.x, State.PickPosClip.y), (double4x4)State.View, (double4x4)State.Projection);
            var tmpList = new List<PointCloudOctant>();

            var allHitBoxes = PickOctantRecursively((PointCloudOctant)_octree.Root, ray, tmpList);

            Console.WriteLine(allHitBoxes.Count > 0);

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

        public PointCloudPickerModule(Common.IPointCloudOctree octree)
        {
            _octree = (PointCloudOctree)octree;
        }

        public void SetState(PickerState state)
        {
            State = state;
        }
    }
}
