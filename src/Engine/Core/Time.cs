namespace Fusee.Engine.Core
{
    /// <summary>
    /// The Time class provides all time information. Time is a staticton (a singleton with an additional
    /// static interface). 
    /// </summary>
    /// <remarks>
    /// Use the input instance in cases where you actually need an 
    /// object to pass around (although there is no such use case in FUSEE code at all).
    /// Use the static access in all other cases to reduce typing Input.Instance
    /// over and over again. Use <code>using static Fusee.Engine.Core.Time</code> to
    /// directly access <see cref="FramePerSecond"/>, <see cref="DeltaTime"/> etc.
    /// without even typing a namespace or classname.
    /// </remarks>
    public class Time
    {
        #region Fields

        private static Time _instance;

        private float _deltaTime;
        private float _time;
        private float _timeScale;
        private long _frameCount;
        private float _realTime;
        private int _framesPerSecondAverage;
        private float _timeSecond;
        private int _framePerSecond;
        private float _unsmoothedFps;
        private float _realDeltaTime;

        internal float DeltaTimeIncrement
        {
            set
            {
                _deltaTime = value;

                _realDeltaTime = _deltaTime;
                _unsmoothedFps = (1 / _deltaTime);
                _timeSecond += value;
                _framePerSecond++;
                _realTime += _deltaTime;
                _frameCount++;
                _deltaTime *= _timeScale;
                _time += _deltaTime;

                if (_timeSecond >= 1)
                {
                    _framesPerSecondAverage = _framePerSecond;
                    _framePerSecond = 0;
                    _timeSecond = 0;
                }
            }
        }

        /// <summary>
        /// Provides the average framerate of the last second (read only).
        /// </summary>
        public int TimeFramesPerSecondAverage => _framesPerSecondAverage;
        /// <summary>
        /// Provides the average framerate of the last second (read only).
        /// </summary>
        public static int FramesPerSecondAverage => Instance.TimeFramesPerSecondAverage;

        /// <summary>
        /// Provides the passed time since start of the application uneffected by TimeScale (read only).
        /// </summary>
        public float TimeRealTimeSinceStart => _realTime;
        /// <summary>
        /// Provides the passed time since start of the application uneffected by TimeScale (read only).
        /// </summary>
        public static float RealTimeSinceStart => Instance.TimeRealTimeSinceStart;

        /// <summary>
        /// Provides the peek framerate, updated every frame (read only).
        /// </summary>
        public float TimeFramePerSecond => _unsmoothedFps;
        /// <summary>
        /// Provides the peek framerate, updated every frame (read only).
        /// </summary>
        public static float FramePerSecond => Instance.TimeFramePerSecond;

        /// <summary>
        /// Provides the total number of rendered frames (read only).
        /// </summary>
        public long TimeFrames => _frameCount;
        /// <summary>
        /// Provides the total number of rendered frames (read only).
        /// </summary>
        public static long Frames => Instance.TimeFrames;

        /// <summary>
        /// Provides the DeltaTime since the last frame in milliseconds that is effected by the TimeScale (read only).
        /// </summary>
        public float TimeDeltaTime => _deltaTime;
        /// <summary>
        /// Provides the DeltaTime since the last frame in milliseconds that is effected by the TimeScale (read only).
        /// </summary>
        public static float DeltaTime => Instance.TimeDeltaTime;

        /// <summary>
        /// Provides the DeltaTime since the last frame in milliseconds that is uneffected by the TimeScale (read only).
        /// </summary>
        public float TimeRealDeltaTimeMs => _realDeltaTime;
        /// <summary>
        /// Provides the DeltaTime since the last frame in milliseconds that is uneffected by the TimeScale (read only).
        /// </summary>
        public static float RealDeltaTimeMs => Instance.TimeRealDeltaTimeMs;

        /// <summary>
        /// Provides the passed time since start of the application effected by TimeScale (read only).
        /// </summary>
        public float InstTimeSinceStart => _time;
        /// <summary>
        /// Provides the passed time since start of the application effected by TimeScale (read only).
        /// </summary>
        public static float TimeSinceStart => Instance.InstTimeSinceStart;

        /// <summary>
        /// The TimeScale modifies the speed of the time.
        /// </summary>
        /// <remarks>
        /// 0 the time stops.
        /// 1 normal time speed.
        /// Smaller then 1 time passes slower. 
        /// Bigger then 1 time passes faster.
        /// </remarks>
        public float InstTimeScale
        {
            set { _timeScale = value; }
            get { return _timeScale ; }
        }

        /// <summary>
        /// The TimeScale modifies the speed of the time.
        /// </summary>
        /// <remarks>
        /// 0 the time stops.
        /// 1 normal time speed.
        /// Smaller then 1 time passes slower. 
        /// Bigger then 1 time passes faster.
        /// </remarks>
        public static float TimeScale
        {
            set { Instance.InstTimeScale = value; }
            get { return Instance.InstTimeScale; }
        }


        private Time()
        {
            _timeScale = 1;
        }

        /// <summary>
        /// Provides the Singleton Instance of the Time Class.
        /// </summary>
        public static Time Instance => _instance ?? (_instance = new Time());

        #endregion

        internal void Dispose()
        {
        }
    }
}
