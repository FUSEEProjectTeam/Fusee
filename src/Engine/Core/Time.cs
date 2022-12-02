﻿using System;

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
    /// directly access <see cref="FramesPerSecond"/>, <see cref="DeltaTime"/> etc.
    /// without even typing a namespace or class name.
    /// </remarks>
    public class Time : IDisposable
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

        private float _deltaTimeUpdate;

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

        internal float DeltaTimeUpdateIncrement
        {
            set
            {
                _deltaTimeUpdate = value;
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
        /// Provides the passed time since start of the application unaffected by TimeScale (read only).
        /// </summary>
        public float TimeRealTimeSinceStart => _realTime;
        /// <summary>
        /// Provides the passed time since start of the application unaffected by TimeScale (read only).
        /// </summary>
        public static float RealTimeSinceStart => Instance.TimeRealTimeSinceStart;

        /// <summary>
        /// Provides the peek framerate, updated every frame (read only).
        /// </summary>
        public float TimeFramePerSecond => _unsmoothedFps;
        /// <summary>
        /// Provides the peek framerate, updated every frame (read only).
        /// </summary>
        public static float FramesPerSecond => Instance.TimeFramePerSecond;

        /// <summary>
        /// Provides the total number of rendered frames (read only).
        /// </summary>
        public long TimeFrames => _frameCount;
        /// <summary>
        /// Provides the total number of rendered frames (read only).
        /// </summary>
        public static long Frames => Instance.TimeFrames;

        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.RenderAFrame"/> loop, since the last frame in seconds that is effected by the TimeScale (read only).
        /// </summary>
        public float TimeDeltaTime => _deltaTime;
        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.RenderAFrame"/> loop, since the last frame in seconds that is effected by the TimeScale (read only).
        /// </summary>
        public static float DeltaTime => Instance.TimeDeltaTime;

        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.RenderAFrame"/> loop, since the last frame in seconds that is unaffected by the TimeScale (read only).
        /// </summary>
        public float TimeRealDeltaTime => _realDeltaTime;
        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.RenderAFrame"/> loop, since the last frame in seconds that is unaffected by the TimeScale (read only).
        /// </summary>
        public static float RealDeltaTime => Instance.TimeRealDeltaTime;

        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.Update"/> loop, since the last frame in seconds that is not effected by the TimeScale (read only).
        /// </summary>
        public float TimeDeltaTimeUpdate => _deltaTimeUpdate;
        /// <summary>
        /// Provides the DeltaTime, for the <see cref="RenderCanvas.Update"/> loop, since the last frame in seconds that is not effected by the TimeScale (read only).
        /// </summary>
        public static float DeltaTimeUpdate => Instance.TimeDeltaTimeUpdate;

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
            get { return _timeScale; }
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
        public static Time Instance => _instance ??= new Time();

        #endregion

        #region IDisposable Support

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _instance = null;
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~Time()
        {
            Dispose(false);
        }

        #endregion
    }
}