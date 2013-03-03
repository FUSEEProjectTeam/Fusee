using SFML.Audio;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        private readonly SFMLAudioImp _audio;

        private SoundBuffer _outputBuffer;
        private Sound _outputSound;
        private Music _outputStream;

        private readonly bool _isStream;

        public float Volume
        {
            get { return GetVolume(); }
            set { SetVolume(value); }
        }

        public bool Loop
        {
            get { return _isStream ? _outputStream.Loop : _outputSound.Loop; }
            set
            {
                if (_isStream)
                    _outputStream.Loop = value;
                else
                    _outputSound.Loop = value;
            }
        }

        public AudioStream(string fileName, bool streaming, SFMLAudioImp audioCl)
        {
            _audio = audioCl;
            _isStream = streaming;

            if (_isStream)
                _outputStream = new Music(fileName);
            else
            {
                _outputBuffer = new SoundBuffer(fileName);
                _outputSound = new Sound(_outputBuffer);
            }
        }

        public void Dispose()
        {
            if (_outputStream != null)
            {
                _outputStream.Dispose();
                _outputStream = null;
            }

            if (_outputBuffer != null)
            {
                _outputBuffer.Dispose();
                _outputBuffer = null;
            }

            if (_outputSound != null)
            {
                _outputSound.Dispose();
                _outputSound = null;
            }
        }

        public void Play(bool loop = true)
        {
            if (_isStream)
            {
                _outputStream.Play();
                _outputStream.Loop = loop;
            }
            else
            {
                _outputSound.Play();
                _outputSound.Loop = loop;
            }
        }

        public void Pause()
        {
            if (_isStream)
                _outputStream.Pause();
            else
                _outputSound.Pause();
        }

        public void Stop()
        {
            if (_isStream)
                _outputStream.Stop();
            else
                _outputSound.Stop();
        }

        internal void SetVolume(float val)
        {
            var maxVal = System.Math.Min(_audio.GetVolume(), val);
            maxVal = System.Math.Max(maxVal, 0);

            if (_isStream)
                _outputStream.Volume = maxVal;
            else
                _outputSound.Volume = maxVal;
        }

        internal float GetVolume()
        {
            return _isStream ? _outputStream.Volume : _outputSound.Volume;
        }
    }
}