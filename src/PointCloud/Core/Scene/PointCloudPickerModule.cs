using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Text;
using static Fusee.Engine.Core.ScenePicker;

namespace Fusee.PointCloud.Core.Scene
{
    public class PointCloudPickerModule : IPickerModule
    {
        private PickerState State;

        public Func<PickerState> GetPickerState { get; set; }

        /// <summary>
        /// Determines visible points of a point cloud (using the components <see cref="VisibilityTester"/>) and renders them.
        /// </summary>
        /// <param name="pointCloud">The point cloud component.</param>
        [VisitMethod]
        public void RenderPointCloud(PointCloudComponent pointCloud)
        {
            State = GetPickerState?.Invoke();
            Console.WriteLine($"MousePos: {State.PickPosClip}");
        }

        public PointCloudPickerModule()
        {

        }

        public void SetState(PickerState state)
        {
            State = state;
        }
    }
}
