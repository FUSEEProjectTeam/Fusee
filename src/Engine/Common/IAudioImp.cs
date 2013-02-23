using JSIL.Meta;

namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();
        void CloseDevice();

        IAudioStream LoadFile(string fileName);

        void Play();
        void Pause();
        void Stop();

        void Play(IAudioStream stream);
        void Pause(IAudioStream stream);
        void Stop(IAudioStream stream);

        void SetVolume(float val);
        float GetVolume();
    }
}
