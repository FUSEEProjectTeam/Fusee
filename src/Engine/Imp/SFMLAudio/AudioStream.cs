using SFML.Audio;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
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

        public float Panning
        {
            get { return GetPanning(); }
            set { SetPanning(value); }
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

        public AudioStream(string fileName, SoundBuffer sndBuffer)
        {
            OutputBuffer = sndBuffer;
            _outputSound = new Sound(sndBuffer);

            Init(fileName, false);
        }

        public AudioStream(string fileName, bool streaming)
        {
            if (streaming)
                _outputStream = new Music(fileName);
            else
            {
                OutputBuffer = new SoundBuffer(fileName);
                _outputSound = new Sound(OutputBuffer);
            }

            Init(fileName, streaming);
        }

        private void Init(string fileName, bool streaming)
        {
            IsStream = streaming;
            FileName = fileName;
            Volume = 100;

            if (IsStream)
                _outputStream.MinDistance = 100;
            else
                _outputSound.MinDistance = 100;
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

        public void Play()
        {
            Play(Loop);
        }

        public void Play(bool loop)
        {
            if (IsStream)
            {
                _outputStream.Loop = loop;
                _outputStream.Play();
            }
            else
            {
                _outputSound.Loop = loop;
                _outputSound.Play();

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
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, 0);

            if (IsStream)
                _outputStream.Volume = maxVal;
            else
                _outputSound.Volume = maxVal;
        }

        internal float GetVolume()
        {
            return (float) System.Math.Round(IsStream ? _outputStream.Volume : _outputSound.Volume, 2);
        }

        internal void SetPanning(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, -100);

            var tmpPos = new Vector3F(maxVal, 0, 0);

            if (IsStream)
                _outputStream.Position = tmpPos;
            else
                _outputSound.Position = tmpPos;
        }

        internal float GetPanning()
        {
            return (float) System.Math.Round(IsStream ? _outputStream.Position.X : _outputSound.Position.X, 2);
        }
    }
}