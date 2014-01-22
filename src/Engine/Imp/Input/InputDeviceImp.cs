using System;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    public class InputDeviceImp : IInputDeviceImp
    {
        
        private readonly DeviceInstance _controller;
        private Joystick _joystick;
        private JoystickState _state;
        private bool[] buttonsPressed;
        private float _deadZone;

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


        public InputDeviceImp()
        {

        }

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

        public bool IsButtonDown(int buttonIndex)
        {
            if (GetState().IsPressed(buttonIndex))
            {
                return true;
            }

            return false;
        }

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

        public int GetButtonCount()
        {
            return _joystick.Capabilities.ButtonCount;
        }

        public float GetZAxis()
        {
            float tmp = GetState().RotationZ / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        public float GetYAxis()
        {
            float tmp = -GetState().Y / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        public float GetXAxis()
        {
            float tmp = GetState().X / 1000f;
            if (tmp > _deadZone)
                return tmp;
            if (tmp < -_deadZone)
                return tmp;
            return 0;
        }

        public void SetDeadZone(float zone)
        {
            _deadZone = zone;
        }

        public String GetCategory()
        {
            return "[Gamepad] "+  _controller.ProductName;
        }

        public void Release()
        {
            if (_joystick != null)
            {
                _joystick.Unacquire();
                _joystick.Dispose();
            }
            _joystick = null;
        }

        public string GetName()
        {
            return _controller.ProductName;
        }


    }
}

