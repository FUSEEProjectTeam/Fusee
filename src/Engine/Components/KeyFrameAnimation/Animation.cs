using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine;
using Fusee.KeyFrameAnimation;
using Fusee.Xirkit;

namespace Fusse.KeyFrameAnimation
{
    public class Animation
    {
        private List<ChannelBase> _channels = new List<ChannelBase>();
        private Circuit _animHandler = new Circuit();

        private float _time;
        private float _maxDuration;
        private int _animMode;
        private bool _direction;

        public List<ChannelBase> ChannelBaseList { get { return _channels; } }
        public int AnimationMode { get { return _animMode; } set { _animMode = value; } }

        public Animation(int animMode = 0)
        {
            _time = 0;
            _animMode = animMode;
            _direction = true;
        }

        public void AddChannel(ChannelBase baseChannel)
        {

            _channels.Add(baseChannel);

        }

        public void AddAnimation<TValue>(Channel<TValue> channel, Object channelObject, String field)
        {
            channel.NeedTime();

            if (_maxDuration == null || _maxDuration < channel.Time)
            {
                _maxDuration = channel.Time;
            }

            _channels.Add(channel);
            Node channelNode = new Node(channel);
            Node channelObjektNode = new Node(channelObject);
            //channelObjektNode.Attach(field, channelNode, "Value");
            channelNode.Attach("Value", channelObjektNode, field);
            _animHandler.AddRoot(channelNode);
            _animHandler.AddNode(channelObjektNode);
            _animHandler.Execute();
        }


        public void RemoveChannel(int channelPosition)
        {
            _channels.RemoveAt(channelPosition);
            //TODO Implement in Xircit functionality to remove nodes
        }

        public void SetTick(float time)
        {
            foreach (var baseChannel in _channels)
            {
                baseChannel.SetTick(time);
            }
        }

        public void Animate(float time)
        {


            switch (_animMode)
            {
                case 0:
                    if (_time >= _maxDuration)
                    {
                        _time = 0;
                    }
                    break;
                case 1:
                    if (_time + time > _maxDuration || _time + time < 0)
                    {
                        _direction = !_direction;
                    }
                    break;
                default:
                    if (_time >= _maxDuration)
                    {
                        _time = 0;
                    }
                    break;
            }

            if (_direction)
            {
                _time += time;
            }
            else
            {
                _time -= time;
            }




            foreach (var baseChannel in _channels)
            {
                baseChannel.SetTick(_time);
            }

            _animHandler.Execute();
        }

        public void Animate()
        {

            float time = (float)Time.Instance.DeltaTime;
            switch (_animMode)
            {
                case 0:
                    if (_time >= _maxDuration)
                    {
                        _time = 0;
                    }
                    break;
                case 1:
                    if (_time + time > _maxDuration || _time + time < 0)
                    {
                        _direction = !_direction;
                    }
                    break;
                default:
                    if (_time >= _maxDuration)
                    {
                        _time = 0;
                    }
                    break;
            }

            if (_direction)
            {
                _time += time;
            }
            else
            {
                _time -= time;
            }


            foreach (var baseChannel in _channels)
            {
                baseChannel.SetTick(_time);
            }
            _animHandler.Execute();
        }

        public void DeleteAnimation(int pos)
        {
            _animHandler.DeleteRoot(pos);
            _animHandler.DeleteNode(pos);
            _channels.RemoveAt(pos);
        }
    }
}
