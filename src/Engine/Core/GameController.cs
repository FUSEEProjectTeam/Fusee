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
        

        // Status des GamePads
        
        

        public GameController(DeviceInstance device)
        {
            DirectInput directInput = new DirectInput();
             state = new JoystickState();
            
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

        public bool isButtonDown(int buttonIndex)
        {
            state = this.GetState();

           
                if (state.IsPressed(buttonIndex))
                {

                    return true;
                }
            return false;
        }

        public bool IsButtonPressed(int buttonIndex)
        {
            state = this.GetState();


            if (state.IsPressed(buttonIndex) && buttonsPressed[buttonIndex] == false)
                {
                    buttonsPressed[buttonIndex] = true;
                    return true;
                }

            if (state.IsReleased(buttonIndex) && buttonsPressed[buttonIndex] == true)
                {
                    buttonsPressed[buttonIndex] = false;
                }
            return false;
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
