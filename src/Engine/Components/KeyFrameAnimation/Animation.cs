using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusse.KeyFrameAnimation
{
    public class Animation
    {
        private List<ChannelBase> _channels = new List<ChannelBase>();

        public Animation()
        {

        }

        public void AddChannel(ChannelBase baseChannel)
        {
            _channels.Add(baseChannel);
        }

        public void RemoveChannel(int channelPosition)
        {
            _channels.RemoveAt(channelPosition);
        }

        public void SetTick(float time)
        {
            foreach (var baseChannel in _channels)
            {
                baseChannel.SetTick(time);
            }
        }
    }
}
