// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System.Collections.Generic;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Input.Web
{
    public class InputDriverImp : IInputDriverImp
    {
        [JSExternal]
        public List<IInputDeviceImp> DeviceImps()
        {
            throw new System.NotImplementedException();
        }
    }
}
