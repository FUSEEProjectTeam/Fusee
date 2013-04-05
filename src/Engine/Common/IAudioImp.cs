#pragma warning disable 1591 //disables the warning about missing XML-comments

namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();
        void CloseDevice();

        IAudioStream LoadFile(string fileName, bool streaming);

        void Stop();

        void SetVolume(float val);
        float GetVolume();

        void SetPanning(float val);
    }
}

#pragma warning restore 1591