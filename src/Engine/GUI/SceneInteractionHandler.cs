using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using Fusee.Xene;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Gui
{
    /// <summary>
    /// Needed for adding interactions/events to objects in the scene graph.
    /// Traverses the scene via a ScenePicker and invokes the necessary events.
    /// </summary>
    public class SceneInteractionHandler : Visitor<SceneNode, SceneComponent>
    {
        //private static List<CodeComponent> _observables;
        private readonly ScenePicker _scenePicker;

        private SceneNode _pickRes;
        private SceneNode _pickResCache;


        /// <summary>
        /// Initializes a new instance of the <see cref="SceneInteractionHandler"/> class.
        /// </summary>
        /// <param name="scene">The scene the interaction handler belongs to.</param>
        /// <param name="prePassCameraResults">The <see cref="CameraResult"/> of the <see cref="PrePassVisitor.PrePassTraverse(SceneContainer)"/> operation.</param>
        public SceneInteractionHandler(SceneContainer scene, IEnumerable<CameraResult> prePassCameraResults)
        {
            IgnoreInactiveComponents = true;
            _scenePicker = new ScenePicker(scene, prePassCameraResults);
        }

        private static SceneNode FindLeafNodeInPickRes(SceneNode firstPickRes, IList<SceneNode> pickResults)
        {
            if (pickResults.Count == 1)
                return pickResults[0];

            if (firstPickRes.Children == null || firstPickRes.Children.Count == 0)
                return firstPickRes;

            foreach (var child in firstPickRes.Children)
            {
                if (pickResults.Contains(child))
                    return child;
                if (child.Children != null)
                {
                    var found = FindLeafNodeInPickRes(child, pickResults);
                    if (found != null && pickResults.Contains(found))
                        return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Picks at the mouse position and traverses the picked objects components.
        /// If a corresponding component is found the suitable visit method is called which invokes the event.
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext"/>, used for picking operations.</param>
        /// <param name="mousePos">The current mouse position.</param>
        /// <param name="canvasWidth">Canvas width - needed to determine the mouse position in clip space.</param>
        /// <param name="canvasHeight">Canvas height - needed to determine the mouse position in clip space.</param>
        public void CheckForInteractiveObjects(float2 mousePos, int canvasWidth, int canvasHeight)
        {
            var pickResults = _scenePicker.Pick(mousePos, canvasWidth, canvasHeight).ToList().OrderBy(pr => pr.ClipPos.z).ToList();
            var pickResNodes = pickResults.ConvertAll(x => x.Node);
            var firstPickRes = pickResults.FirstOrDefault();

            _pickRes = null;

            if (firstPickRes != null)
                _pickRes = FindLeafNodeInPickRes(firstPickRes?.Node, pickResNodes);

            if (_pickRes != _pickResCache)
                Traverse(_pickResCache);

            _pickResCache = _pickRes;

            if (_pickRes != null)
                Traverse(_pickRes);
        }

        /// <summary>
        /// Invokes an interaction on a given button.
        /// </summary>
        /// <param name="btn">The button to invoke an interaction on.</param>
        [VisitMethod]
        public void InvokeInteraction(GuiButton btn)
        {
            if (CurrentNode == _pickResCache && _pickResCache != _pickRes)
            {
                btn.IsMouseOver = false;
                btn.DetachEvents();
            }
            if (CurrentNode == _pickRes)
            {
                btn.IsMouseOver = true;
                btn.InvokeEvents();
            }
        }
    }
}