using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.GUI
{
    public class SceneInteractionHandler : SceneVisitor
    {

        //private static List<CodeComponent> _observables;
        private ScenePicker _scenePicker;

        public float4x4 View;
        public float4x4 Projection;

        public SceneInteractionHandler(SceneContainer _scene)
        {
           // _observables = new List<CodeComponent>();
            _scenePicker = new ScenePicker(_scene);
        }

        public void CheckForInteractableObjects( float2 pickPosClip)
        {
            _scenePicker.View = View;
            _scenePicker.Projection = Projection;
            var pickRes = _scenePicker.Pick(pickPosClip).ToList().OrderBy(pr => pr.ClipPos.z).FirstOrDefault();

            //foreach (var item in _scenePicker.Pick(pickPosClip).ToList())
            //{
            //    Debug.WriteLine(item.Node.Name);
            //}

            if (pickRes != null)
                Debug.WriteLine(pickRes.Node.Name);

            if (pickRes != null)
                Traverse(pickRes.Node);
        }

        /*public void Register(CodeComponent oberservable)
        {
            _observables.Add(oberservable);
        }*/

        [VisitMethod]
        public void InvokeInteraction(GUIButton btn)
        {
            btn.InvokeEvent();
        }
        

        //Iterate observables and call event if neccessary
        


    }
}
 