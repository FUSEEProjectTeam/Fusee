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
        private int _framePerSecondSmooth;
        private float _timeSecond;
        private int _framePerSecond;
        private float _unsmoothedFps;

        internal double DeltaTimeIncrement
        {
            set
            {
                _deltaTime = value;

                _unsmoothedFps = (float)(1 / _deltaTime);
                _timeSecond += (float)value;
                _framePerSecond++;
                _realTime += _deltaTime;
                _frameCount++;
                _deltaTime *= _timeFlow;
                _time += _deltaTime;

                if (_timeSecond >= 1)
                {
                    _framePerSecondSmooth = _framePerSecond;
                    _framePerSecond = 0;
                    _timeSecond = 0;
                }
            }
        }

        public int FamePerSecondSmooth
        {
            get { return _framePerSecondSmooth; }
        }

        public double RealTimeSinceStart
        {
            get { return _realTime;  }   
        }

        public float FramePerSecond
        {
            get { return _unsmoothedFps; }
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
