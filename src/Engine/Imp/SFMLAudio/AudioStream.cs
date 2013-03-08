using SFML.Audio;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        private readonly SFMLAudioImp _audio;

        internal SoundBuffer OutputBuffer { get; set; }
        internal string FileName { get; set; }

        private Sound _outputSound;
        private Music _outputStream;

        internal bool IsStream { get; set; }

        public float Volume
        {
            get { return GetVolume(); }
            set { SetVolume(value); }
        }

        public bool Loop
        {
            get { return IsStream ? _outputStream.Loop : _outputSound.Loop; }
            set
            {
                if (IsStream)
                    _outputStream.Loop = value;
                else
                    _outputSound.Loop = value;
            }
        }

        public AudioStream(string fileName, SoundBuffer sndBuffer, SFMLAudioImp audioCl)
        {
            _audio = audioCl;
            IsStream = false;

            OutputBuffer = sndBuffer;
            _outputSound = new Sound(sndBuffer);

            FileName = fileName;
        }

        public AudioStream(string fileName, bool streaming, SFMLAudioImp audioCl)
        {
            _audio = audioCl;
            IsStream = streaming;

            if (IsStream)
                _outputStream = new Music(fileName);
            else
            {
                OutputBuffer = new SoundBuffer(fileName);
                _outputSound = new Sound(OutputBuffer);
            }

            FileName = fileName;
        }

        public void Dispose()
        {
            if (_outputStream != null)
            {
                _outputStream.Dispose();
                _outputStream = null;
            }

            if (OutputBuffer != null)
            {
                OutputBuffer.Dispose();
                OutputBuffer = null;
            }

            if (_outputSound != null)
            {
                _outputSound.Dispose();
                _outputSound = null;
            }
        }

        public void Play(bool loop = true)
        {
            if (IsStream)
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
            if (IsStream)
                _outputStream.Pause();
            else
                _outputSound.Pause();
        }

        public void Stop()
        {
            if (IsStream)
                _outputStream.Stop();
            else
                _outputSound.Stop();
        }

        internal void SetVolume(float val)
        {
            var maxVal = System.Math.Min(_audio.GetVolume(), val);
            maxVal = System.Math.Max(maxVal, 0);

            if (IsStream)
                _outputStream.Volume = maxVal;
            else
                _outputSound.Volume = maxVal;
        }

        internal float GetVolume()
        {
            return IsStream ? _outputStream.Volume : _outputSound.Volume;
        }
    }
}