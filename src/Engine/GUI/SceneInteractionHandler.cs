using System.Diagnostics;
using System.Linq;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Needed for adding interactions/events to objects in the scene graph.
    /// Traverses the scene via a ScenePicker and invokes the necessary events.
    /// </summary>
    public class SceneInteractionHandler : SceneVisitor
    {
        //private static List<CodeComponent> _observables;
        private readonly ScenePicker _scenePicker;

        /// <summary>
        /// The View matrix for calculating the correct pick position.
        /// </summary>
        public float4x4 View;
        /// <summary>
        /// The projection matrix for calculating the correct pick position.
        /// </summary>
        public float4x4 Projection;

        public SceneInteractionHandler(SceneContainer scene)
        {
           _scenePicker = new ScenePicker(scene);
        }

        /// <summary>
        /// Picks at the mouse position and traverses the picked objects components.
        /// If a corresponding component is found the suitable visit method is called which invokes the event.
        /// </summary>
        /// <param name="mousePos">The current mouse position.</param>
        /// <param name="canvasWidth">Canvas width - needed to determine the mouse position in clip space.</param>
        /// <param name="canvasHeight">Canvas height - needed to determine the mouse position in clip space.</param>
        public void CheckForInteractiveObjects(float2 mousePos, int canvasWidth, int canvasHeight)
        {
            _scenePicker.View = View;
            _scenePicker.Projection = Projection;
            var pickPosClip = mousePos * new float2(2.0f / canvasWidth, -2.0f / canvasHeight) + new float2(-1, 1);
            var pickRes = _scenePicker.Pick(pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

            if (pickRes != null)
                Traverse(pickRes.Node);
        }

        [VisitMethod]
        public void InvokeInteraction(GUIButton btn)
        {
            btn.InvokeEvent();
        }
    }
}
 