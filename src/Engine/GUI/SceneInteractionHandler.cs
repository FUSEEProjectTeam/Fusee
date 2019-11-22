using System.Collections.Generic;
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

        private SceneNodeContainer _pickRes;
        private SceneNodeContainer _pickResCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneInteractionHandler"/> class.
        /// </summary>
        /// <param name="scene">The scene the interaction handler belongs to.</param>
        public SceneInteractionHandler(SceneContainer scene)
        {
            _scenePicker = new ScenePicker(scene);
        }

        private static SceneNodeContainer FindLeafNodeInPickRes(SceneNodeContainer firstPickRes, IList<SceneNodeContainer> pickResults)
        {
            if (pickResults.Count == 1)
                return pickResults[0];

            if (firstPickRes.Children == null)
                return firstPickRes;

            foreach (var child in firstPickRes.Children)
            {
                if (pickResults.Contains(child))
                    return child;
                if (child.Children != null)
                {
                    var found = FindLeafNodeInPickRes(child, pickResults);
                    if (found != null)
                        return found;
                }
            }

            return null; 
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
            
            var pickPosClip = mousePos * new float2(2.0f / canvasWidth, -2.0f / canvasHeight) + new float2(-1, 1);

            var pickResults = _scenePicker.Pick(pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).ToList();
            var pickResNodes = pickResults.Select(x => x.Node).ToList();
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
        public void InvokeInteraction(GUIButton btn)
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
