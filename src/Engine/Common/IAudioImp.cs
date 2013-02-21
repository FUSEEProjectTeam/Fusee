using JSIL.Meta;

namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();

        [JSIgnore]
        void CloseDevice();

        void LoadFile(string fileName);

        void Play();

        void Pause();
    }
}
