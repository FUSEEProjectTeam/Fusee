// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Base.Common;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Video.Web
{
    public class VideoStreamImp : IVideoStreamImp
    {
        [JSExternal]
        public ImageData GetCurrentFrame()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Start()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}

