// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Sound.Web
{
    /// <summary>
    /// This class is a container of all <see cref="AudioStreamImp" /> instances and regulates the properties of all Sounds.
    /// </summary>
    public class AudioImp : IAudioImp
    {
        [JSExternal]
        public AudioImp()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// opens an audio device.
        /// </summary>
        [JSExternal]
        public void OpenDevice()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Closes the audio device.
        /// </summary>
        [JSExternal]
        public void CloseDevice()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public IAudioStreamImp LoadFile(string fileName, bool streaming)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Stops the audio playback.
        /// </summary>
        [JSExternal]
        public void Stop()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sets the audio volume.
        /// </summary>
        /// <param name="val"></param>
        [JSExternal]
        public void SetVolume(float val)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Returns the audio volume.
        /// </summary>
        /// <returns></returns>
        [JSExternal]
        public float GetVolume()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Sets the audio direction (only in stero or surround).
        /// </summary>
        /// <param name="val"></param>
        [JSExternal]
        public void SetPanning(float val)
        {
            throw new System.NotImplementedException();
        }
    }
}
