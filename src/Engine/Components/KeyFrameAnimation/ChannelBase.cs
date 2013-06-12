using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusse.KeyFrameAnimation
{
    public class ChannelBase
    {
        private float _time;

        public float Time { get { return _time; } set { _time = value; } }

        protected virtual void DoTick(float time)
        {
            //if (TimeChanged != null)
            //{
            //    TValue currentValue = GetValueAt(time);
            //    TimeChanged(currentValue);
            //}
        }
        protected virtual void DemandTime()
        {

        }


        public void SetTick(float time)
        {
            DoTick(time);
        }
        public void NeedTime()
        {
            DemandTime();
        }

    }
}
