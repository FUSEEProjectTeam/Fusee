using Fusee.Engine.Common;
using Fusee.Engine.Core;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Code component that creates a button from an object in the scene graph.
    /// </summary>
    public class GUIButton : CodeComponent
    {
        /// <summary>
        ///     Occurs when mouse button is pressed on this button.
        /// </summary>
        public event InteractionHandler OnMouseDown;

        /// <summary>
        ///     Occurs when mouse button is released on this button.
        /// </summary>
        public event InteractionHandler OnMouseUp;

        /// <summary>
        ///     Occurs when the mouse cursor is over this button.
        /// </summary>
        public event InteractionHandler OnMouseOver;

        /// <summary>
        ///     Occurs when the mouse cursor enters this button.
        /// </summary>
        public event InteractionHandler OnMouseEnter;

        /// <summary>
        ///     Occurs when the mouse cursor exits this button.
        /// </summary>
        public event InteractionHandler OnMouseExit;

        private bool _isAttached;

        /// <summary>
        /// Creates a new instance of the code component that creates a button from an object in the scene graph.
        /// </summary>
        public GUIButton()
        {
            _isAttached = false;
        }
        
        internal void InvokeEvents()
        {
            OnMouseOver?.Invoke(this);

            if (!_isAttached)
            {
                Input.Mouse.ButtonValueChanged += OnMouse;
                Input.Touch.ButtonValueChanged += OnMouse; //ToDo: Define OnTouch()
                Input.Mouse.AxisValueChanged += OnAxisChanged;

                OnMouseEnter?.Invoke(this);
            }
            _isAttached = true;
        }

        internal void DetachEvents()
        {
            Input.Mouse.ButtonValueChanged -= OnMouse;
            Input.Touch.ButtonValueChanged -= OnMouse; //ToDo: Define OnTouch()
            Input.Mouse.AxisValueChanged -= OnAxisChanged;

            OnMouseExit?.Invoke(this);

            _isAttached = false;
        }

        private void OnMouse(object sender, ButtonValueChangedArgs bvca)
        {
                if (bvca.Pressed)
                    OnMouseDown?.Invoke(this);
                else
                    OnMouseUp?.Invoke(this);
        }

        private void OnAxisChanged(object sender, AxisValueChangedArgs avca)
        {
            //if (IsMouseOver)
            //{
            //    if (!_mouseEnterButton) return;
            //    _mouseEnterButton = false;

            //    if (OnMouseEnter == null) return;
            //    OnMouseEnter?.Invoke(this);
            //}
        }
    }
}