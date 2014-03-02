using System;
using SlimDX.DirectInput;

namespace Fusee.Engine
{

    /// <summary>
    /// The SlimDX - specific implementation for the input devices.
    /// </summary>
    public class InputDeviceImp : IInputDeviceImp
    {
        
        private readonly DeviceInstance _controller;
        private Joystick _joystick;
        private JoystickState _state;
        private bool[] buttonsPressed;
        private float _deadZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDeviceImp"/> class.
        /// </summary>
        /// <param name="instance">The DeviceInstance.</param>
        public InputDeviceImp(DeviceInstance instance)
        {
            _controller = instance;
            _deadZone = 0.1f;
            buttonsPressed = new bool[100];
            _joystick = new Joystick(new DirectInput(), _controller.InstanceGuid);

            foreach (DeviceObjectInstance deviceObject in _joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    _joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);

            }

            _joystick.Acquire();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDeviceImp"/> class.
        /// </summary>
        public InputDeviceImp()
        {

        }

        /// <summary>
        /// Gets the current state of the input device. The state is used to poll the device.
        /// </summary>
        /// <returns>The state of the input device.</returns>
        public JoystickState GetState()
        {
            try
            {
                if (_joystick.Acquire().IsFailure || _joystick.Poll().IsFailure)
                {
                    _state = new JoystickState();
                    return _state;
                }

                _state = _joystick.GetCurrentState();

                return _state;
            }
            catch(DirectInputException)
            { }

            return new JoystickState();
        }

        /// <summary>
        /// Loop though all buttons on the gamepad an see which one is pressed
        /// </summary>
        /// <returns>The pressed button</returns>
        public int GetPressedButton()
        {
            int buttonIndex = -1;
            _state = GetState();
            var buttons = new bool[_state.GetButtons().Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (_state.IsPressed(i))
                {
                    buttonIndex = i;
                }
            }
            return buttonIndex;
        }

        /// <summary>
        /// Checks if the button is down.
        /// </summary>
        /// <param name="buttonIndex">The button to check.</param>
        /// <returns>True if the button is pressed and false if not.</returns>
        public bool IsButtonDown(int buttonIndex)
        {
            if (GetState().IsPressed(buttonIndex))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a specified button is held down for more than one frame.
        /// </summary>
        /// <param name="buttonIndex">The index of the button that is checked.</param>
        /// <returns>true if the button at the specified index is held down for more than one frame and false if not.</returns>
        public bool IsButtonPressed(int buttonIndex)
        {
            _state = GetState();

            if (_state.IsPressed(buttonIndex) && buttonsPressed[buttonIndex] == false)
            {
                buttonsPressed[buttonIndex] = true;
                return true;
            }

            if (_state.IsReleased(buttonIndex) && buttonsPressed[buttonIndex])
            {
                buttonsPressed[buttonIndex] = false;
            }
            return false;
        }

        /// <summary>
        /// Counts the buttons on the input device.
        /// </summary>
        /// <returns>The amount of buttons on the device.</returns>
        public int GetButtonCount()
        {
            return _joystick.Capabilities.ButtonCount;
        }

        /// <summary>
        /// Gets the value of the z-axis. On most gamepads the z-axis is either the right joystick or the triggers at the back.
        /// For wobbly joysticks a dead zone is required to avoid value changes if the joystick is not moved.
        /// </summary>
        /// <returns>The current value of the z-axis.</returns>
        public float GetZAxis()
        {
            float tmp = GetState().RotationZ / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        /// <summary>
        /// Gets the value of the y-axis. On most gamepads the y-axis is the horizontal axis of the left joystick.
        /// For wobbly joysticks a dead zone is required to avoid value changes if the joystick is not moved.
        /// </summary>
        /// <returns>The current value of the y-axis.</returns>
        public float GetYAxis()
        {
            float tmp = -GetState().Y / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        /// <summary>
        /// Gets the value of the x-axis. On most gamepads the x-axis the horizontal axis of the left joystick.
        /// For wobbly joysticks a dead zone is required to avoid value changes if the joystick is not moved.
        /// </summary>
        /// <returns>The current value of the x-axis.</returns>
        public float GetXAxis()
        {
            float tmp = GetState().X / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        /// <summary>
        /// Sets the Deadzone
        /// </summary>
        /// <param name="zone">The Deadzone.</param>
        public void SetDeadZone(float zone)
        {
            _deadZone = zone;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <returns>The category of the device</returns>
        public String GetCategory()
        {
            return "[Gamepad] "+  _controller.ProductName;
        }

        /// <summary>
        /// If the input device is no longer available, it is released.
        /// </summary>
        public void Release()
        {
            if (_joystick != null)
            {
                _joystick.Unacquire();
                _joystick.Dispose();
            }
            _joystick = null;
        }

        /// <summary>
        /// Gets a user-friendly product name of the device.
        /// </summary>
        /// <returns>The device name.</returns>
        public string GetName()
        {
            return _controller.ProductName;
        }


    }
}

