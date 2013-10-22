using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    /// <summary>
    /// The Time class provides all time information. It is accessible from everywhere by calling Time.instance.                          
    /// </summary>
    public class Time
    {
        #region Fields

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
        private double _realDeltaTime;

        internal double DeltaTimeIncrement
        {
            set
            {
                _deltaTime = value;

                _realDeltaTime = _deltaTime;
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

        /// <summary>
        /// Provides the average framerate of the last second (read only).
        /// </summary>
        public int FramePerSecondSmooth
        {
            get { return _framePerSecondSmooth; }
        }

        /// <summary>
        /// Provides the passed time since start of the application uneffected by TimeFlow (read only).
        /// </summary>
        public double RealTimeSinceStart
        {
            get { return _realTime;  }   
        }

        /// <summary>
        /// Provides the peek framerate, updated every frame (read only).
        /// </summary>
        public float FramePerSecond
        {
            get { return _unsmoothedFps; }
        }
        
        /// <summary>
        /// Provides the total number of rendered frames (read only).
        /// </summary>
        public long Frames
        {
            get { return _frameCount; }
        }

        /// <summary>
        /// Provides the DeltaTime that is effected by the TimeFlow (read only).
        /// </summary>
        public double DeltaTime
        {
            get { return _deltaTime; }
        }

        /// <summary>
        /// Provides the DeltaTime that is uneffected by the TimeFlow (read only).
        /// </summary>
        public double RealDeltaTime
        {
            get { return _realDeltaTime; }
        }        

        /// <summary>
        /// Provides the passed time since start of the application effected by TimeFlow (read only).
        /// </summary>
        public double TimeSinceStart
        {
            get { return _time; }
        }

        /// <summary>
        /// The TimeFlow modifies the speed of the time.
        /// </summary>
        /// <remarks>
        /// 0 the time stops.
        /// 1 normal time speed.
        /// Smaller then 1 time passes slower. 
        /// Bigger then 1 time passes faster.
        /// </remarks>
        public float TimeFlow
        {
            set
            {
                    _timeFlow = value;
            }

            get { return _timeFlow ; }
        }

        private Time()
        {
            _timeFlow = 1;
        }

        /// <summary>
        /// Provides the Instance of the Time Class.
        /// </summary>
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

        #endregion
    }
}
