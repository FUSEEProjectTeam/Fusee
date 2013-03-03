#pragma warning disable 1591 //disables the warning about missing XML-comments

namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();
        void CloseDevice();

        IAudioStream LoadFile(string fileName, bool streaming);

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

#pragma warning restore 1591