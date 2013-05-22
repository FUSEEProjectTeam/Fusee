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