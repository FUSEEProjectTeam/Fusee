using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Examples.LASReaderExample.Core
{

    public enum PointShape
    {
        RECT = 0,
        CIRCLE = 1,
        CONE = 2,
        SPHERE = 3,
        PARABOLID = 4

    }

    public enum ColorMode
    {
        POINT,
        SINGLE,
        NORMAL,        
        WEIGHT
    }

    public enum Lighting
    {
        UNLIT = 0,
        DIFFUSE = 1,
        BLINN_PHONG = 2
    }
}
