using System;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    public class GameController
    {
        // Das GamePad
        private Joystick joystick;
        private JoystickState state;
        private bool[] buttonsPressed;
        private float deadZone;
        private int buttonCount;
        
        

        // Status des GamePads
        
        

        public GameController(DeviceInstance device)
        {
            DirectInput directInput = new DirectInput();
            //buttonCount = direct
             state = new JoystickState();
            deadZone = 0.1f;
            // Geräte suchen

            
            buttonsPressed = new bool[100];
            

            // Gamepad erstellen
            joystick = new Joystick(directInput, device.InstanceGuid);
            

            
            // Den Zahlenbereich der Achsen auf -1000 bis 1000 setzen
            foreach (DeviceObjectInstance deviceObject in joystick.GetObjects())
            {
                if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                    joystick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-1000, 1000);

            }

            joystick.Acquire();
        }

        public JoystickState GetState()
        {
            if (joystick.Acquire().IsFailure || joystick.Poll().IsFailure)
            {
                // Wenn das GamePad nicht erreichbar ist, leeren Status zurückgeben.
                state = new JoystickState();
                return state;
            }

            state = joystick.GetCurrentState();

            return state;
        }

        public int GetPressedButton()
        {
            int buttonIndex = -1;
            state = GetState();
            bool[] buttons = new bool[state.GetButtons().Length];
            for (int i = 0; i < buttons.Length; i++)
            {
                if (state.IsPressed(i))
                {
                    buttonIndex = i;
                }
            }
            return buttonIndex;
        }

        public bool IsButtonDown(int buttonIndex)
        {
            state = GetState();

           
                if (state.IsPressed(buttonIndex))
                {

                    return true;
                }
            return false;
        }

        public bool IsButtonPressed(int buttonIndex)
        {
            state = GetState();


            if (state.IsPressed(buttonIndex) && buttonsPressed[buttonIndex] == false)
                {
                    buttonsPressed[buttonIndex] = true;
                    return true;
                }

            if (state.IsReleased(buttonIndex) && buttonsPressed[buttonIndex])
                {
                    buttonsPressed[buttonIndex] = false;
                }
            return false;
        }

        public int GetButtonCount()
        {
            return joystick.Capabilities.ButtonCount;
        }

        public float GetZAxis()
        {
            float _tmp = GetState().Z / 1000f;
            if (_tmp > deadZone)
                return _tmp;
            if (_tmp < -deadZone)
                return _tmp;
            return 0;
        }

        public float GetYAxis()
        {
            float _tmp = -GetState().Y / 1000f;
            if (_tmp > deadZone)
                return _tmp;
            if (_tmp < -deadZone)
                return _tmp;
            return 0;
        }

        public float GetXAxis()
        {
          float  _tmp = GetState().X / 1000f;
          if (_tmp > deadZone)
              return _tmp;
          if (_tmp < -deadZone)
              return _tmp;
            return 0;
        }

        public void SetDeadZone (float zone)
        {
            deadZone = zone;
        }

        public void Release()
        {
            if (joystick != null)
            {
                joystick.Unacquire();
                joystick.Dispose();
            }
            joystick = null;
        }
    }
}
