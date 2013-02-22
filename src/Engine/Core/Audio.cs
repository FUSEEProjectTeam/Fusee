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
    }
}
