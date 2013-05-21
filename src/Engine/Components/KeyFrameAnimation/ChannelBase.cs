using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusse.KeyFrameAnimation
{
    public class ChannelBase
    {
        protected virtual void DoTick(float time)
        {

        }

        public void SetTick(float time)
        {
            DoTick(time);
        }
    }
}
