using System;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        private readonly NAudioImp _audio;
        private LoopStream _mainOutputStream;

        public float Volume
        {
            get { return GetVolume(); }
            set { SetVolume(value); }
        }

        public bool Loop
        {
            get { return _mainOutputStream.Loop; }
            set { _mainOutputStream.Loop = value; }
        }

        public AudioStream(string fileName, NAudioImp audioCl)
        {
            _audio = audioCl;

            _mainOutputStream = new LoopStream(fileName)
                                    {
                                        Volume = _audio.GetVolume(),
                                        State = LoopStream.PlaybackState.Stopped
                                    };
        }

        public void Play(bool loop = true)
        {
            _mainOutputStream.Loop = loop;
            _audio.Mixer.AddInputStream(_mainOutputStream);
        }

        public void Pause()
        {
            _audio.Mixer.RemoveInputStream(_mainOutputStream);
            _mainOutputStream.State = LoopStream.PlaybackState.Paused;
        }

        public void Stop()
        {
            _audio.Mixer.RemoveInputStream(_mainOutputStream);
            _mainOutputStream.State = LoopStream.PlaybackState.Stopped;
        }

        public void Dispose()
        {
            if (_mainOutputStream != null)
            {
                _audio.Mixer.RemoveInputStream(_mainOutputStream);

                _mainOutputStream.CloseStream();
                _mainOutputStream.Close();
                _mainOutputStream = null;
            }
        }

        internal void SetVolume(float val)
        {
            var maxVal = System.Math.Min(_audio.GetVolume(), val);
            maxVal = System.Math.Max(maxVal, 0);

            _mainOutputStream.Volume = maxVal;
        }

        internal float GetVolume()
        {
            return _mainOutputStream.Volume;
        }
    }
}