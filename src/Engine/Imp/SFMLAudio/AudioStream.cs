using SFML.Audio;

namespace Fusee.Engine
{
    /// <summary>
    /// This class is the implementation for Audio playback. It uses the SFML library to handle the sound playback. 
    /// </summary>
    class AudioStream : IAudioStream
    {
        #region Fields
        internal SoundBuffer OutputBuffer { get; set; }
        internal string FileName { get; set; }

        private Sound _outputSound;
        private Music _outputStream;

        internal bool IsStream { get; set; }

        /// <summary>
        /// Gets or sets the volume of this <see cref="IAudioStream" /> (0 - 100).
        /// </summary>
        /// <value>
        /// The volume of this <see cref="IAudioStream" /> (0 - 100).
        /// </value>
        public float Volume
        {
            get { return GetVolume(); }
            set { SetVolume(value); }
        }

        /// <summary>
        /// Gets or sets the panning of this <see cref="IAudioStream" /> (-100 to +100).
        /// </summary>
        /// <value>
        /// The panning of this <see cref="IAudioStream" /> (-100 to +100).
        /// </value>
        public float Panning
        {
            get { return GetPanning(); }
            set { SetPanning(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IAudioStream" /> shall be looped.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="IAudioStream" /> shall be looped; otherwise, <c>false</c>.
        /// </value>
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="sndBuffer">The SND buffer.</param>
        public AudioStream(string fileName, SoundBuffer sndBuffer)
        {
            OutputBuffer = sndBuffer;
            _outputSound = new Sound(sndBuffer);

            Init(fileName, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioStream"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="streaming">if set to <c>true</c> [streaming].</param>
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
        #endregion

        #region Members
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
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

        /// <summary>
        /// Plays this <see cref="IAudioStream" />.
        /// </summary>
        public void Play()
        {
            Play(Loop);
        }

        /// <summary>
        /// Plays this <see cref="IAudioStream" />.
        /// </summary>
        /// <param name="loop"><c>true</c> if this <see cref="IAudioStream" /> shall be looped; otherwise, <c>false</c>.</param>
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

        /// <summary>
        /// Pauses this <see cref="IAudioStream" />.
        /// </summary>
        public void Pause()
        {
            if (IsStream)
                _outputStream.Pause();
            else
                _outputSound.Pause();
        }
        #endregion

        #region Internal Members

        /// <summary>
        /// Stops this <see cref="IAudioStream" />.
        /// </summary>
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
        #endregion
    }
}