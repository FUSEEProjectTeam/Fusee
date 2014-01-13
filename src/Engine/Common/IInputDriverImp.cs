using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IInputDriverImp
    {
        List<IInputDeviceImp> DeviceImps();
    }
}
