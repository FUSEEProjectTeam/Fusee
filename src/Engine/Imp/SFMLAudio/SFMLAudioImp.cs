#define MP3Warning

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SFML.Audio;

namespace Fusee.Engine
{
    /// <summary>
    /// This class is a container of all <see cref="AudioStream" /> instances and regulates the properties of all Sounds.
    /// </summary>
    public class SFMLAudioImp : IAudioImp
    {
        #region Fields
        private readonly List<AudioStream> _allStreams;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SFMLAudioImp"/> class.
        /// </summary>
        public SFMLAudioImp()
        {
            _allStreams = new List<AudioStream>();

            Listener.Direction = new Vector3F(0, 0, -0.1f);
            Listener.Position = new Vector3F(0, 0, 0.1f);
        }
        #endregion

        #region Members
        /// <summary>
        /// Opens the device. All <see cref="AudioStream" /> are reset. The GlobalVolume is set to 100(maximum).
        /// </summary>
        public void OpenDevice()
        {
            _allStreams.Clear();
            Listener.GlobalVolume = 100;
        }

        /// <summary>
        /// Closes the device. All <see cref="AudioStream" /> instances get disposed.
        /// </summary>
        public void CloseDevice()
        {
            foreach (var audioStream in _allStreams)
               audioStream.Dispose();
        }

        /// <summary>
        /// Loads a sound file from hard drive.
        /// </summary>
        /// <param name="fileName">Full path name of the file with datatype ending, e.g. "C\:sound.ogg". A path can be absolute or relative.</param>
        /// <param name="streaming">if set to <c>true</c> [streaming].</param>
        /// <returns>An IAudioStream instance is returned if the file path was correctly resolved.</returns>
        public IAudioStream LoadFile(string fileName, bool streaming)
        {
#if MP3Warning
            if (Path.GetExtension(fileName) == ".mp3")
                Debug.WriteLine(
                    "Warning: Using mp3 files requires a lot of memory and might require a license. Please consider using ogg files instead.");
#endif

            // sound already loaded?
            SoundBuffer tmpSndBuffer = null;

            if (!streaming)
                foreach (
                    var audioStream in
                        _allStreams.Where(audioStream => !audioStream.IsStream && audioStream.FileName == fileName))
                {
                    tmpSndBuffer = audioStream.OutputBuffer;
                    break;
                }

            IAudioStream tmpAudioStream = tmpSndBuffer == null
                                 ? new AudioStream(fileName, streaming)
                                 : new AudioStream(fileName, tmpSndBuffer);

            _allStreams.Add((AudioStream) tmpAudioStream);

            return tmpAudioStream;
        }

        /// <summary>
        /// Stops the playback of all <see cref="AudioStream" /> instances.
        /// </summary>
        public void Stop()
        {
            foreach (var audioStream in _allStreams)
                audioStream.Stop();
        }

        /// <summary>
        /// Sets the master volume of the listener.
        /// </summary>
        /// <param name="val">The value (0 - 100).</param>
        public void SetVolume(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, 0);

            Listener.GlobalVolume = maxVal;
        }

        /// <summary>
        /// Gets the master volume of the Listener.
        /// </summary>
        /// <returns>The volume as float with 2 digit precision.</returns>
        public float GetVolume()
        {
            return (float) System.Math.Round(Listener.GlobalVolume, 2);
        }

        /// <summary>
        /// Sets the panning of all <see cref="AudioStream" /> instances.
        /// </summary>
        /// <param name="val">The value(-100 - 100).</param>
        public void SetPanning(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, -100);

            foreach (var audioStream in _allStreams)
                audioStream.Panning = maxVal;
        }
        #endregion
    }
}
