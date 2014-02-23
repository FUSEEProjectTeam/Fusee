using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Fusee.Engine
{
    public enum DeviceCategory
    {
        Mouse,
        Keyboard,
        GameController,
        Touch,
        Kinect,
    }

    public enum ControllerButton
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,

        R1,
        R2,
        L1,
        L2,
        //...

        FirstUserButton,
    }

    /// <summary>
    ///     The Input class takes care of all inputs. It is accessible from everywhere inside a Fusee project.
    ///     E.g. : Input.Instance.IsButtonDown(MouseButtons.Left);
    /// </summary>
    public class Input
    {
        #region Fields

        private static Input _instance;

        private IInputImp _inputImp;

        internal IInputImp InputImp
        {
            set
            {
                _inputImp = value;

                _inputImp.KeyDown += KeyDown;
                _inputImp.KeyUp += KeyUp;

                _inputImp.MouseButtonDown += ButtonDown;
                _inputImp.MouseButtonUp += ButtonUp;
                _inputImp.MouseMove += MouseMove;

                _axes = new float[(int) InputAxis.LastAxis];
                _axesPreviousAbsolute = new float[(int) InputAxis.LastAxis];

                _keys = new HashSet<int>();
                _keysUp = new HashSet<int>();
                _keysDown = new HashSet<int>();

                _buttonsPressed = new HashSet<int>();
            }
        }

        public event EventHandler<MouseEventArgs> OnMouseButtonDown;
        public event EventHandler<MouseEventArgs> OnMouseButtonUp;
        public event EventHandler<MouseEventArgs> OnMouseMove;

        private float[] _axes;
        private float[] _axesPreviousAbsolute;

        private HashSet<int> _keys;
        private HashSet<int> _keysUp;
        private HashSet<int> _keysDown;

        private HashSet<int> _buttonsPressed;

        /// <summary>
        ///     Gets or sets a value indicating whether to fix mouse at center.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the mouse is fixed at center; otherwise, <c>false</c>.
        /// </value>
        public bool FixMouseAtCenter { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the cursor is visible.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the cursor is visible; otherwise, <c>false</c>.
        /// </value>
        public bool CursorVisible
        {
            get { return _inputImp.CursorVisible; }
            set { _inputImp.CursorVisible = value; }
        }

        #endregion

        #region Members

        private void KeyDown(object sender, KeyEventArgs kea)
        {
            if (!_keys.Contains((int) kea.KeyCode))
            {
                _keys.Add((int) kea.KeyCode);

                if (!_keysDown.Contains((int) kea.KeyCode))
                    _keysDown.Add((int) kea.KeyCode);
            }
        }

        private void KeyUp(object sender, KeyEventArgs kea)
        {
            if (_keys.Contains((int) kea.KeyCode))
                _keys.Remove((int) kea.KeyCode);

            if (!_keysUp.Contains((int) kea.KeyCode))
                _keysUp.Add((int) kea.KeyCode);
        }

        private void ButtonDown(object sender, MouseEventArgs mea)
        {
            if (OnMouseButtonDown != null)
                OnMouseButtonDown(this, mea);

            if (!_buttonsPressed.Contains((int) mea.Button))
                _buttonsPressed.Add((int) mea.Button);
        }

        private void ButtonUp(object sender, MouseEventArgs mea)
        {
            if (OnMouseButtonUp != null)
                OnMouseButtonUp(this, mea);

            if (_buttonsPressed.Contains((int) mea.Button))
                _buttonsPressed.Remove((int) mea.Button);
        }

        private void MouseMove(object sender, MouseEventArgs mea)
        {
            if (OnMouseMove != null)
                OnMouseMove(this, mea);
        }

        /// <summary>
        ///     Returns the scalar value for the given axis. Typically these values are used as velocities.
        /// </summary>
        /// <param name="axis">The axis for which the value is to be returned.</param>
        /// <returns>
        ///     The current deflection of the given axis.
        /// </returns>
        public float GetAxis(InputAxis axis)
        {
            return _axes[(int) axis];
        }

        /// <summary>
        ///     Sets the mouse position.
        /// </summary>
        /// <param name="pos">A <see cref="Point" /> with x and y values.</param>
        public void SetMousePos(Point pos)
        {
            _inputImp.SetMousePos(pos);
        }

        /// <summary>
        ///     Gets the mouse position.
        /// </summary>
        /// <returns>A <see cref="Point" /> with x and y values.</returns>
        public Point GetMousePos()
        {
            return _inputImp.GetMousePos();
        }

        /// <summary>
        ///     Check if a given key is pressed during the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the key is pressed. Otherwise false.
        /// </returns>
        public bool IsKey(KeyCodes key)
        {
            return _keys.Contains((int) key);
        }

        /// <summary>
        ///     Check if the user started pressing a key in the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the user started pressing the key in the current frame. Otherwise false.
        /// </returns>
        public bool IsKeyDown(KeyCodes key)
        {
            return _keysDown.Contains((int) key);
        }

        /// <summary>
        ///     Check if the user stopped pressing a key in the current frames.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>
        ///     true if the user stopped pressing the key in the current frame. Otherwise false.
        /// </returns>
        public bool IsKeyUp(KeyCodes key)
        {
            return _keysUp.Contains((int) key);
        }

        /// <summary>
        ///     Check if a given mouse button is pressed during the current frame.
        /// </summary>
        /// <param name="button">the mousebutton to check.</param>
        /// <returns>
        ///     True if the mousebutton is pressed. Otherwise false.
        /// </returns>
        public bool IsButton(MouseButtons button)
        {
            return _buttonsPressed.Contains((int) button);
        }

        /*
        /// <summary>
        /// Called when [button down] event is triggered. 
        /// Occurs when a Mouse button was pressed down once.
        /// </summary>
        /// <param name="button">The mousebutton.</param>
        /// <returns>True, if mousebutton was pressed down once. Otherwise false.</returns>

            public bool OnButtonDown(MouseButtons button)
            {
                // not implemented
                return false;
            }
        */

        /*
        /// <summary>
        ///     Called when [button up] event is triggered.
        ///     Occurs when a Mouse button was released.
        /// </summary>
        /// <param name="button">The mousebutton.</param>
        /// <returns>True, if mousebutton was released. Otherwise false.</returns>

        public bool OnButtonUp(MouseButtons button)
        {
            // not implemented
            return false;
        }
        */

        internal void OnUpdateFrame()
        {
            var p = _inputImp.GetMousePos();

            float currX = p.x;
            float currY = p.y;
            float currR = _inputImp.GetMouseWheelPos();

            const float deltaFix = 0.005f;

            _axes[(int) InputAxis.MouseX] = (currX - _axesPreviousAbsolute[(int) InputAxis.MouseX])*deltaFix;
            _axes[(int) InputAxis.MouseY] = (currY - _axesPreviousAbsolute[(int) InputAxis.MouseY])*deltaFix;
            _axes[(int) InputAxis.MouseWheel] = (currR - _axesPreviousAbsolute[(int) InputAxis.MouseWheel])*deltaFix;

            // Fix to Center
            if (FixMouseAtCenter)
            {
                p = _inputImp.SetMouseToCenter();

                currX = p.x;
                currY = p.y;
            }

            _axesPreviousAbsolute[(int) InputAxis.MouseX] = currX;
            _axesPreviousAbsolute[(int) InputAxis.MouseY] = currY;
            _axesPreviousAbsolute[(int) InputAxis.MouseWheel] = currR;
        }

        internal void OnLateUpdate()
        {
            _keysDown.Clear();
            _keysUp.Clear();
        }

        /// <summary>
        ///     Provides the Instance of the Input Class.
        /// </summary>
        public static Input Instance
        {
            get { return _instance ?? (_instance = new Input()); }
        }

        #endregion

        #region InputDevices

        /// <summary>
        ///     Input devices like gamepads are managed here.
        /// </summary>
        public Collection<InputDevice> Devices = new Collection<InputDevice>();

        private IInputDriverImp _inputDriverImp;

        /// <summary>
        ///     All connected input devices are added to <see cref="Devices" /> - List and the names and indices of
        ///     the devices are printed to the debugging - console.
        /// </summary>
        public void InitializeDevices()
        {
            List<IInputDeviceImp> tmp = _inputDriverImp.DeviceImps();
            foreach (var inputDevice in tmp)
            {
                Devices.Add(new InputDevice(inputDevice));
            }
        }

        /// <summary>
        ///     Checks if a device at the specified index exists and returns it if it exists.
        /// </summary>
        /// <param name="deviceIndex">The index at <see cref="Devices" /></param>
        /// <returns>The device at the specified index </returns>
        public InputDevice GetDevice(int deviceIndex)
        {
            try
            {
                return Devices[deviceIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("Can not find Input Device with Device-Index " + deviceIndex + "!");
            }
        }

        /// <summary>
        ///     Counts the devices.
        /// </summary>
        /// <returns>The amount of devices</returns>
        public int CountDevices()
        {
            return Devices.Count;
        }

        internal IInputDriverImp InputDriverImp
        {
            set { _inputDriverImp = value; }
        }

        #endregion
    }
}