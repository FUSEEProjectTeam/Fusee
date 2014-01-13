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
        
        private GameController _controller;


        public InputDeviceImp(GameController cont)
        {
            _controller = cont;
        }


        public InputDeviceImp()
        {

        }



        public bool IsButtonPressed(int buttonIndex)
        {
            return _controller.IsButtonPressed(buttonIndex);
        }

        public bool IsButtonDown(int buttonIndex)
        {
            return _controller.IsButtonDown(buttonIndex);

        }

        public float getAxis(string axis)
        {
            switch (axis)
            {
                case "horizontal":
                    return _controller.GetXAxis();
                case "vertical":
                    return _controller.GetYAxis();
                default:
                    return 0.0f;

            }
        }

        public string Name { get; private set; }

        public int GetPressedButton()
        {
            return _controller.GetPressedButton();
        }

        public int GetButtonCount()
        {
            return _controller.GetButtonCount();
        }

        public string GetCategory()
        {
            throw new NotImplementedException();
        }
    }
}

