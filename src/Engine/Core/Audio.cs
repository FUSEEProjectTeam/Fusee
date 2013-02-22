using JSIL.Meta;

namespace Fusee.Engine
{
    public class Audio
    {
        private readonly IAudioImp _audioImp;

        public Audio(IAudioImp audioImp)
        {
            _audioImp = audioImp;
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

        public void Play(IAudioStream stream)
        {
            _audioImp.Play(stream);
        }

        public void Pause()
        {
            _audioImp.Pause();
        }
    }
}
