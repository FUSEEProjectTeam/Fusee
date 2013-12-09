using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusee.Engine;
using SlimDX.DirectInput;

namespace Fusee.Engine
{
    public class InputDeviceImp : IInputDeviceImp
    {
        public static List<GameController> _devices = new List<GameController>();
        private GameController _controller;


            public static List<IInputDeviceImp> getDevicesByCategory(){
                CreateDevices();
                List<IInputDeviceImp> _list = new List<IInputDeviceImp>();
                foreach (GameController controller in _devices)
                {
                    _list.Add(new InputDeviceImp(controller));
                }
                return _list;
            }


        // Status des GamePads

        public static void CreateDevices()
        {
            DirectInput directInput = new DirectInput();
            var devices = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);

            foreach (DeviceInstance deviceInstance in devices)
            {
                _devices.Add(new GameController(deviceInstance));
            }
        }

        public InputDeviceImp(GameController cont)
        {
            _controller = cont;
        }

        public bool isButtonPressed()
        {
            return _controller.IsButtonPressed(0);
        }


    }
}

