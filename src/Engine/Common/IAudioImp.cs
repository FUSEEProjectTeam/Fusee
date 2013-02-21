using JSIL.Meta;

namespace Fusee.Engine
{
    public interface IAudioImp
    {
        void OpenDevice();

        [JSIgnore]
        void CloseDevice();

        IAudioStream LoadFile(string fileName);

        void Play(IAudioStream stream);

        void Pause();
    }
}
