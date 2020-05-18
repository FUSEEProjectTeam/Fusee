using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.Keyboard"/>.
    /// Defines convenience methods to access the keyboard buttons.
    /// </summary>
    public class KeyboardDevice : InputDevice
    {
        private readonly int _leftRightAxis;
        private readonly int _upDownAxis;
        private readonly int _adAxis;
        private readonly int _wsAxis;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The platform dependent connector to the underlying physical device.</param>
        public KeyboardDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
            _leftRightAxis = RegisterTwoButtonAxis((int) KeyCodes.Left, (int) KeyCodes.Right, AxisDirection.X).Id;
            _upDownAxis = RegisterTwoButtonAxis((int)KeyCodes.Down, (int)KeyCodes.Up, AxisDirection.Y).Id;
            _adAxis = RegisterTwoButtonAxis((int)KeyCodes.A, (int)KeyCodes.D, AxisDirection.X).Id;
            _wsAxis = RegisterTwoButtonAxis((int)KeyCodes.S, (int)KeyCodes.W, AxisDirection.Y).Id;
        }

        /// <summary>
        /// Retrieves the current button state for the specified key.
        /// </summary>
        /// <value>
        /// true if the button is pressed; otherwise false.
        /// </value>
        /// <param name="key">The key to check.</param>
        public bool GetKey(KeyCodes key) => GetButton((int)key);

        /// <summary>
        /// Determines whether the specified key was pressed during the current frame.
        /// The result is true only for one single frame even if the key is still pressed 
        /// in subsequent frames.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was pressed during the current frame, false if the key
        /// was not pressed during the current frame - even if the key was not released yet.
        /// </returns>
        public bool IsKeyDown(KeyCodes key) => IsButtonDown((int)key);

        /// <summary>
        /// Determines whether the specified key was released during the current frame.
        /// The result is true only for one single frame even if the key is still up
        /// in subsequent frames.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the key was released during the current frame, false if the key
        /// was not released during the current frame - even if the key currently is released.
        /// </returns>
        public bool IsKeyUp(KeyCodes key) => IsButtonUp((int)key);

        /// <summary>
        /// Gets the value at the (calculated) axis controlled with the Up/Down cursor keys.
        /// </summary>
        /// <returns>The  axis' value in the range between [-1, 1]</returns>
        public float UpDownAxis => GetAxis(_upDownAxis);
        /// <summary>
        /// Gets the value at the (calculated) axis controlled with the Left/Right cursor keys.
        /// </summary>
        /// <returns>The  axis' value in the range between [-1, 1]</returns>
        public float LeftRightAxis => GetAxis(_leftRightAxis);

        /// <summary>
        /// Gets the value at the (calculated) axis controlled with the A and the D key.
        /// This is the horizontal component of the four key WASD left-handed game steering paradigm
        /// </summary>
        /// <returns>The  axis' value in the range between [-1, 1]</returns>
        // ReSharper disable once InconsistentNaming
        public float ADAxis => GetAxis(_adAxis);

        /// <summary>
        /// Gets the value at the (calculated) axis controlled with the A and the D key.
        /// This is the vertical component of the four key WASD left-handed game steering paradigm
        /// </summary>
        /// <returns>The  axis' value in the range between [-1, 1]</returns>
        // ReSharper disable once InconsistentNaming
        public float WSAxis => GetAxis(_wsAxis);
    }
}
