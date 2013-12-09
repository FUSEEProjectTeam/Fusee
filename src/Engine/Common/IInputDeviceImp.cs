using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public interface IInputDeviceImp
    {

        List<IInputDeviceImp> getDevicesByCategory();

        float getAxis(string axis);


    }
}
