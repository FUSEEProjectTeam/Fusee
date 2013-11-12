using System;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    public class GameController
    {
        // Das GamePad
        private Joystick joystick;

        // Status des GamePads
        private JoystickState state = new JoystickState();
        DirectInput input = new DirectInput();

        public GameController(DirectInput directInput, int number)
        {
            // Geräte suchen
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            if (devices.Count == 0 || devices[number] == null)
            {
                // Kein Gamepad vorhanden
                return;
            }

            // Gamepad erstellen
            joystick = new Joystick(directInput, devices[number].InstanceGuid);

            
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
