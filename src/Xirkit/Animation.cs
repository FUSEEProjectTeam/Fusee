﻿using System;
using System.Collections.Generic;
// using Fusee.Engine;


namespace Fusee.Xirkit
{
    /// <summary>
    /// The Animation Class is capable of storing and handling different types of Channels.
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The _channels List stores all Animation Channels that are derived from the ChannelBase Class.
        /// </summary>
        private List<ChannelBase> _channels = new List<ChannelBase>();
        /// <summary>
        /// The _anim handler is a Xirkit Circuit connecting the calculated values to the corresponding Object.
        /// </summary>
        private Circuit _animHandler = new Circuit();

        /// <summary>
        /// The _time is the actual time in this Animation.
        /// </summary>
        private float _time;
        /// <summary>
        /// The Duration of the Animation
        /// </summary>
        private float _maxDuration;
        /// <summary>
        /// The _anim mode determines whether the animation runs in PingPong or Loop mode.
        /// </summary>
        private int _animMode;
        /// <summary>
        /// The _direction is the actual direction the animation is playing (forward and backward is possible)
        /// </summary>
        private bool _direction;

        /// <summary>
        /// Gets the channel base list.
        /// </summary>
        public List<ChannelBase> ChannelBaseList { get { return _channels; } }
        /// <summary>
        /// Gets and sets the animation mode.
        /// </summary>
        /// <value>
        /// The animation mode.
        /// </value>
        public int AnimationMode { get { return _animMode; } set { _animMode = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Animation"/> class.
        /// </summary>
        /// <param name="animMode">The parameter sets the AnimationMode</param>
        public Animation(int animMode = 0)
        {
            _time = 0;
            _animMode = animMode;
            _direction = true;
        }

        /// <summary>
        /// Adds a channel to this Animation. This channel will be handled by this Animation.
        /// </summary>
        /// <param name="baseChannel">The base channel that will be handled by this Animation.</param>
        public void AddChannel(ChannelBase baseChannel)
        {

            _channels.Add(baseChannel);

        }

        /// <summary>
        /// Adds the Channel to this Animation and adds it to the _animHandler.
        /// </summary>
        /// <typeparam name="TValue">The Type of the channel.</typeparam>
        /// <param name="channel">The channel that will be added.</param>
        /// <param name="channelObject">The Object that will be wired with the channel.</param>
        /// <param name="field">The full name of the Field or Property of the Object that will be handled by this Animation.</param>
        public void AddAnimation<TValue>(Channel<TValue> channel, object channelObject, string field)
        {
            // Adjust overall animation duration so that the newly added channel's duration is not shortened
            if (_maxDuration < channel.Duration)
                _maxDuration = channel.Duration;

            _channels.Add(channel);
            channel.SetTick(_time);
            Node channelNode = new Node(channel);
            Node channelObjektNode = new Node(channelObject);
            channelNode.Attach("Value", channelObjektNode, field);
            _animHandler.AddRoot(channelNode);
            _animHandler.AddNode(channelObjektNode);
            _animHandler.Execute();
        }


        /// <summary>
        /// Removes a channel from the _channelList.
        /// </summary>
        /// <param name="channelPosition">The position of the channel that has to be removed.</param>
        public void RemoveChannel(int channelPosition)
        {
            _channels.RemoveAt(channelPosition);
        }

        /// <summary>
        /// Sets the time in all channels.
        /// </summary>
        /// <param name="time">The time that will be set.</param>
        public void SetTick(float time)
        {
            foreach (var baseChannel in _channels)
            {
                baseChannel.SetTick(time);
            }
        }


        /// <summary>
        /// Animates all channels. The time is set by the user.
        /// </summary>
        /// <param name="time">The time that the user set's.</param>
        public void Animate(float time)
        {
            // DEBUG QUATERNION LERP
            // float time = 0.1f; 
            //  float time = (float)Time.Instance.DeltaTime;
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

        /// <summary>
        /// Deletes the animation and removes the Nodes from the AnimHandler
        /// </summary>
        /// <param name="pos">The position.</param>
        public void DeleteAnimation(int pos)
        {
            throw new NotImplementedException("This method is defunct since a user has no idea of what to specify at pos");
            //_animHandler.DeleteRoot(pos);
            //_animHandler.DeleteNode(pos);
            //_channels.RemoveAt(pos);
        }
    }
}
