using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{
    public class VideoStream
    {
        public IVideoStreamImp _imp;

        public ImageData GetCurrentFrame ()
        {
            return _imp.GetCurrentFrame();
        }

        public void Start()
        {
            _imp.Start();
        }

        public void Stop ()
        {
            _imp.Stop();
        }

    }
}
