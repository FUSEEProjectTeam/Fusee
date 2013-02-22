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
