// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Input.Web
{
    public class InputDeviceImp : IInputDeviceImp
    {
        [JSExternal]
        public float GetXAxis()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetYAxis()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetZAxis()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public string GetName()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public int GetPressedButton()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool IsButtonDown(int button)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool IsButtonPressed(int button)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public int GetButtonCount()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public string GetCategory()
        {
            throw new System.NotImplementedException();
        }
    }
}

