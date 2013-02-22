using JSIL.Meta;

namespace Fusee.Engine
{
    public class Audio
    {
        private static Audio _instance;

        private IAudioImp _audioImp;

        internal IAudioImp AudioImp
        {
            set
            {
                _audioImp = value;
            }
        }

        public void OpenDevice()
        {
            _audioImp.OpenDevice();
        }

        [JSIgnore]
        public void CloseDevice()
        {
            _audioImp.CloseDevice();
        }

        public IAudioStream LoadFile(string fileName)
        {
            return _audioImp.LoadFile(fileName);
        }

        public void Play()
        {
            _audioImp.Play();
        }

        public void Pause()
        {
            _audioImp.Pause();
        }

        public void Stop()
        {
            _audioImp.Stop();
        }

        public void Play(IAudioStream stream)
        {
            _audioImp.Play(stream);
        }

        public void Pause(IAudioStream stream)
        {
            _audioImp.Pause(stream);
        }

        public void Stop(IAudioStream stream)
        {
            _audioImp.Stop(stream);
        }

        public void SetVolume(float val)
        {
            _audioImp.SetVolume(val);
        }

        public float GetVolume()
        {
            return _audioImp.GetVolume();
        }

        /// <summary>
        /// Provides the Instance of the Audio Class.
        /// </summary>
        public static Audio Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Audio();
                }
                return _instance;
            }
        }
    }
}
