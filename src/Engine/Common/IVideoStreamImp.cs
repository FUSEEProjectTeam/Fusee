using System.Drawing;

namespace Fusee.Engine
{
    public interface IVideoStreamImp
    {
        ImageData GetCurrentFrame();
        void Start();
        void Stop();
        
    }
}
