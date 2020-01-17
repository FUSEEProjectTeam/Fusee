// Fusee uses the OpenAL library. See http://www.openal.org for details.
// OpenAL is used under the terms of the LGPL, version x.

using Fusee.Base.Core;
using Fusee.Engine.Imp.Sound.Common;

namespace Fusee.Engine.Imp.Sound.Core
{
    /// <summary>
    /// The Audio class provides all audio functionality. It is accessible from everywhere.                          
    /// </summary>
    /// <remarks>
    /// Supported file formats are: mp3, ogg, wav, flac, aiff, au, raw, paf, svx, nist, voc,
    /// ircam, w64, mat4, mat5 pvf, htk, sds, avr, sd2, caf, wve, mpc2k, rf64
    /// 
    /// Audio can be either buffered completely or be played as a stream.
    /// 
    /// E.g. : IAudioStreamImp myAudio1;
    ///
    ///        myAudio1 = Audio.Instance.LoadFile("AssetStorage/Music.ogg"). 
    /// 
    ///        myAudio1.play();  
    /// </remarks>
    public class Audio
    {
        #region Fields
        private static Audio _instance;

        private IAudioImp _audioImp;

        public IAudioImp AudioImp
        {
            set
            {
                if (value == null)
                {
                    Diagnostics.Warn("WARNING: No Audio implementation set. To enable Audio functionality inject an appropriate implementation of IAudioImp in your platform specific application main module.");
                    _audioImp = new DummyAudioImp();
                }
                else
                {
                    _audioImp = value;
                    _audioImp.OpenDevice();
                }
            }
        }

        #endregion

        #region Members

        public void CloseDevice()
        {
            _audioImp.CloseDevice();
        }

        /// <summary>
        /// Loads an audiofile.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="streaming"><c>true</c> if the audiofile shall be streamed; otherwise, <c>false</c>.</param>
        /// <returns>The audiofile as an <see cref="IAudioStreamImp"/></returns>
        public IAudioStreamImp LoadFile(string fileName, bool streaming = false)
        {
            return _audioImp.LoadFile(fileName, streaming);
        }

        /// <summary>
        /// Stops all <see cref="IAudioStreamImp"/>s.
        /// </summary>
        public void Stop()
        {
            _audioImp.Stop();
        }

        /// <summary>
        /// Sets the main volume (0 to 100)
        /// </summary>
        /// <param name="val">The value</param>
        public void SetVolume(float val)
        {
            _audioImp.SetVolume(val);
        }

        /// <summary>
        /// Gets the main volume.
        /// </summary>
        /// <returns>The main value (0 to 100)</returns>
        public float GetVolume()
        {
            return _audioImp.GetVolume();
        }

        /// <summary>
        /// Sets the panning of all <see cref="IAudioStreamImp"/>s (-100 to +100)
        /// </summary>
        /// <param name="val">The value</param>
        public void SetPanning(float val)
        {
            _audioImp.SetPanning(val);
        }

        public void Dispose()
        {
            _instance = null;
        }

        /// <summary>
        /// Provides the Instance of the Audio Class.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Audio Instance
        {
            get { return _instance ?? (_instance = new Audio()); }
        }

        #endregion
    }

    /// <summary>
    /// Dummy implementation with no functionality
    /// </summary>
    internal class DummyAudioImp : IAudioImp
    {
        /// <summary>
        /// Opens the device.
        /// </summary>
        public void OpenDevice()
        {
        }

        /// <summary>
        /// Closes the device.
        /// </summary>
        public void CloseDevice()
        {
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="streaming">if set to <c>true</c> [streaming].</param>
        /// <returns></returns>
        public IAudioStreamImp LoadFile(string fileName, bool streaming)
        {
            return new DummyAudioStreamImp();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// Sets the volume.
        /// </summary>
        /// <param name="volume">The volume.</param>
        public void SetVolume(float volume)
        {
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <returns></returns>
        public float GetVolume()
        {
            return 0;
        }

        /// <summary>
        /// Sets the panning.
        /// </summary>
        /// <param name="val">The value.</param>
        public void SetPanning(float val)
        {
        }
    }

    /// <summary>
    /// Dummy audio stream implementation with no functionality.
    /// </summary>
    internal class DummyAudioStreamImp : IAudioStreamImp
    {
        /// <summary>
        /// Gets and sets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        public float Volume { get; set; }
        /// <summary>
        /// Gets and sets a value indicating whether this <see cref="DummyAudioStreamImp"/> is loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }
        /// <summary>
        /// Gets and sets the panning.
        /// </summary>
        /// <value>
        /// The panning.
        /// </value>
        public float Panning { get; set; }
        /// <summary>
        /// Plays this stream.
        /// </summary>
        public void Play()
        {
        }

        /// <summary>
        /// Plays this stream as a loop.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void Play(bool loop)
        {
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
        }
    }
}