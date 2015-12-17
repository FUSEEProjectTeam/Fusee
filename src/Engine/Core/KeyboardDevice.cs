using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.Keyboard"/>.
    /// Defines convenience methods to access the keyboard buttons.
    /// </summary>
    public class KeyboardDevice : InputDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The platform dependent connector to the underlying physical device.</param>
        public KeyboardDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
        }

        /// <summary>
        /// Retrieves the current button state for the specified key.
        /// </summary>
        /// <value>
        /// true if the button is pressed; otherwise false.
        /// </value>
        /// <param name="key">The key to check.</param>
        public bool this[KeyCodes key] => GetButton((int)key);
    }
}
