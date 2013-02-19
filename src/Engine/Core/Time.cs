using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class Time
    {
        private static Time _instance;
        private double _deltaTime;
        private double _time;
        private float _timeFlow;
        private long _frameCount;
        private double _realTime;


        public double RealTimeSinceStart
        {
            get { return _realTime;  }   
        }

        public int FramePerSecond
        {

          get{ return (int)(1 / _deltaTime); }

        }

        public long Frames
        {
            get { return _frameCount; }
        }

        public double DeltaTime
        {
            get { return _deltaTime; }
        }

        public double TimeSinceStart
        {
            get { return _time; }
        }

        internal double DeltaTimeIncrement
        {
            set 
            {
                _deltaTime = value;
                _realTime += _deltaTime;
                _frameCount++;
                _deltaTime *= _timeFlow;
                _time += _deltaTime;
            }
        }

        public float TimeFlow
        {
            set { _timeFlow = value; }
        }

        private Time()
        {
            _timeFlow = 1;
        }

        public static Time Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Time();
                }
                return _instance;
            }
        }
    }
}
