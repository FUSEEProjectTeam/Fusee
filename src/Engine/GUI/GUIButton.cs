using System;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.GUI
{
    public class GUIButton : CodeComponent
    {
        public event OnClick OnClick;
        public event OnMouseEnter OnMouseEnter;
        public event OnMouseLeave OnMouseLeave;

        /*private SceneInteractionHandler _interactionHandler;

        public GUIButton(SceneInteractionHandler interactionHandler)
        {
            _interactionHandler = interactionHandler;
            _interactionHandler.Register(this);
        }*/


        public void InvokeEvent()
        {
            if (Input.Mouse.LeftButton)
            {
                OnClick?.Invoke();
            }
            //else if() ...
        }

    }


}