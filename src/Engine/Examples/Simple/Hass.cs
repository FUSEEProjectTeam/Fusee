using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.Simple
{
    public class Hass
    {
        private static Hass lastReader;

        private static Hass GetRecycled()
        {
            Hass protoReader = Hass.lastReader;
            Hass.lastReader = (Hass)null;
            return protoReader;
        }
    }
}
