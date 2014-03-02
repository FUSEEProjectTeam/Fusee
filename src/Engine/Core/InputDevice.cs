using System;

namespace Fusee.Engine
{
    /// <summary>
    /// Represents one instance of an input device other than keyboard or mouse
    /// </summary>
    public class InputDevice
    {
        public enum Axis
        {
            Horizontal,
            Vertical,
            Z
        }

        private readonly IInputDeviceImp _inputDeviceImp;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevice"/> class.
        /// </summary>
        /// <param name="inputDeviceImp">The input device imp.</param>
        public InputDevice(IInputDeviceImp inputDeviceImp)
        {
            _inputDeviceImp = inputDeviceImp;
        }

        public InputDevice()
        {

        }

        /// <summary>
        /// Gets the current value of one axis (i.e. joystick or trigger).
        /// </summary>
        /// <param name="axis">Specifies the desired axis, can be "horizontal", "vertical" or "z".</param>
        /// <returns>
        /// The current value (between -1.0 and +1.0) for the specified axis.
        /// </returns>
        public float GetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return _inputDeviceImp.GetXAxis();

                case Axis.Vertical:
                    return _inputDeviceImp.GetYAxis();

                case Axis.Z:
                    return _inputDeviceImp.GetZAxis();

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Gets the name of the instance 
        /// </summary>
        /// <returns>The product name of the queried input device.</returns>
        public string Name()
        {
            return _inputDeviceImp.GetName();
        }

        /// <summary>
        /// Gets the index of the currently pressed button on the input device.
        /// </summary>
        /// <returns>The index of the currently pressed button</returns>
        public int GetPressedButton()
        {
            return _inputDeviceImp.GetPressedButton();
        }

        /// <summary>
        /// Checks if a specified button is pressed in the current frame on the input device.
        /// </summary>
        /// <param name="buttonIndex">The index of the button that is checked.</param>
        /// <returns>True if the button at the specified index is pressed in the current frame and false if not.</returns>
        public bool IsButtonDown(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonDown(buttonIndex);
        }

        /// <summary>
        /// Checks if a specified button is held down for more than one frame.
        /// </summary>
        /// <param name="buttonIndex">The index of the button that is checked.</param>
        /// <returns>True if the button at the specified index is held down for more than one frame and false if not.</returns>
        public bool IsButtonPressed(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonPressed(buttonIndex);
        }

        /// <summary>
        /// Counts the buttons on the input device.
        /// </summary>
        /// <returns>The amount of buttons on the device.</returns>
        public int GetButtonCount()
        {
            return _inputDeviceImp.GetButtonCount();
        }

        /// <summary>
        /// Gets the category of the input device.
        /// </summary>
        /// <returns>The name of the Device Category.</returns>
        public String GetCategory()
        {
            return _inputDeviceImp.GetCategory();
        }
    }
}
