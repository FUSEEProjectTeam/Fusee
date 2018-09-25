using System.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Engine.Core;

namespace Fusee.Engine.GUI
{
    public class GUIButton : CodeComponent
    {
        public event OnMouseOver OnMouseOver;
        public event OnMouseEnter OnMouseEnter;
        public event OnMouseLeave OnMouseLeave;
        public event OnMouseDown OnMouseDown;
        public event OnMouseUp OnMouseUp;

        public void InvokeEvent()
        {
            OnMouseOver?.Invoke();
            Input.Mouse.ButtonValueChanged += OnMouse;
            Input.Touch.ButtonValueChanged += OnMouse; //ToDo: Define OnTouch()

            Input.Mouse.AxisValueChanged += OnAxisChanged;
        }

        private void OnMouse(object sender, ButtonValueChangedArgs bvca)
        {
                if (bvca.Pressed)
                    OnMouseDown?.Invoke();
                else
                    OnMouseUp?.Invoke();
        }

        private void OnAxisChanged(object sender, AxisValueChangedArgs avca)
        {
            //tbd
        }
    }
}