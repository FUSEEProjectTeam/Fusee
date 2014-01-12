// Fusee uses the OpenAL library. See http://www.openal.org for details.
// OpenAL is used under the terms of the LGPL, version x.

namespace Fusee.Engine
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
    /// E.g. : IAudioStream myAudio1;
    ///
    ///        myAudio1 = Audio.Instance.LoadFile("Assets/Music.ogg"). 
    /// 
    ///        myAudio1.play();  
    /// </remarks>
    public class Audio
    {
        #region Fields
        private static Audio _instance;

        private IAudioImp _audioImp;

        internal IAudioImp AudioImp
        {
            set
            {
                _audioImp = value;
                _audioImp.OpenDevice();
            }
        }

        #endregion

        #region Members

        internal void CloseDevice()
        {
            _audioImp.CloseDevice();
        }

        /// <summary>
        /// Loads an audiofile.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="streaming"><c>true</c> if the audiofile shall be streamed; otherwise, <c>false</c>.</param>
        /// <returns>The audiofile as an <see cref="IAudioStream"/></returns>
        public IAudioStream LoadFile(string fileName, bool streaming = false)
        {
            return _audioImp.LoadFile(fileName, streaming);
        }

        /// <summary>
        /// Stops all <see cref="IAudioStream"/>s.
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
        /// Sets the panning of all <see cref="IAudioStream"/>s (-100 to +100)
        /// </summary>
        /// <param name="val">The value</param>
        public void SetPanning(float val)
        {
            _audioImp.SetPanning(val);
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
}