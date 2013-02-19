namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();

        void LoadFile(string fileName);

        void Init();

        void Play();

        void Pause();
    }
}
