// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Sound.Web
{
    class AudioStreamImp : IAudioStreamImp
    {
        [JSExternal]
        public AudioStreamImp(string file)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public float Volume { get; set; }
        [JSExternal]
        public bool Loop { get; set; }
        [JSExternal]
        public float Panning { get; set; }
        [JSExternal]
        public void Play()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Play(bool loop)
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void Pause()
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