// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Video.Web
{
    public class VideoManagerImp : IVideoManagerImp
    {
        [JSExternal]
        public IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IVideoStreamImp CreateVideoStreamImpFromCamera(int cameraIndex, bool useAudio)
        {
            throw new System.NotImplementedException();
        }
    }
}
