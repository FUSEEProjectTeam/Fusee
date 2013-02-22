using System;
using NAudio.Wave;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        private readonly NAudioImp _audio;

        private LoopStream _mainOutputStream;
        private WaveChannel32 _volumeStream;

        private int _pausePosition;

        public float Volume
        {
            get { return GetVolume(); }
            set { SetVolume(value); }
        }

        public bool Loop { get; set; }

        public AudioStream(string fileName, NAudioImp audioCl)
        {
            _audio = audioCl;

            _mainOutputStream = new LoopStream(fileName);
            
            _volumeStream = _mainOutputStream.VolumeStream;
            _volumeStream.Volume = _audio.GetVolume();
        }

        public void Play()
        {
            _audio.Mixer.AddInputStream(_volumeStream);
        }

        public void Pause()
        {
            _audio.Mixer.RemoveInputStream(_volumeStream);
        }

        public void Stop()
        {
            _audio.Mixer.RemoveInputStream(_volumeStream);
            _audio.Mixer.Position = 0;
            _mainOutputStream.Position = 0;
            _volumeStream.Position = 0;
            _audio.Mixer.AddInputStream(_volumeStream);
        }

        public void Dispose()
        {
            if (_volumeStream != null)
            {
                _audio.Mixer.RemoveInputStream(_volumeStream);

                _volumeStream.Close();
                _volumeStream = null;
            }

            if (_mainOutputStream != null)
            {
                _mainOutputStream.Close();
                _mainOutputStream = null;
            }
        }

        internal void SetVolume(float val)
        {
            var maxVal = Math.Min(_audio.GetVolume(), val);
            maxVal = Math.Max(maxVal, 0);

            _volumeStream.Volume = maxVal;
        }

        internal float GetVolume()
        {
            return _volumeStream.Volume;
        }

        
    }
}