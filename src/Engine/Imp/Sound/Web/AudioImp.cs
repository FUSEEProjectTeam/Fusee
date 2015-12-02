// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Sound.Web
{
    /// <summary>
    /// This class is a container of all <see cref="AudioStream" /> instances and regulates the properties of all Sounds.
    /// </summary>
    public class AudioImp : IAudioImp
    {
        [JSExternal]
        public AudioImp()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void OpenDevice()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void CloseDevice()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IAudioStream LoadFile(string fileName, bool streaming)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Stop()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetVolume(float val)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float GetVolume()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void SetPanning(float val)
        {
            throw new System.NotImplementedException();
        }
    }
}
