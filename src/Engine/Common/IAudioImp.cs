namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();

        void LoadFile(string fileName);

        void Play();

        void Pause();
    }
}
