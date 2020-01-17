using Fusee.Engine.Imp.Sound.Common;
using SFML.Audio;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Engine.Imp.Sound.Desktop
{
    /// <summary>
    /// This class is a container of all <see cref="AudioStreamImp" /> instances and regulates the properties of all Sounds.
    /// </summary>
    public class AudioImp : IAudioImp
    {
        #region Fields

        private readonly List<AudioStreamImp> _allStreams;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioImp"/> class.
        /// </summary>
        public AudioImp()
        {
            _allStreams = new List<AudioStreamImp>();

            Listener.Direction = new Vector3f(0, 0, -0.1f);
            Listener.Position = new Vector3f(0, 0, 0.1f);
        }

        #endregion Constructors

        #region Members

        /// <summary>
        /// Opens the device. All <see cref="AudioStreamImp" /> are reset. The GlobalVolume is set to 100(maximum).
        /// </summary>
        public void OpenDevice()
        {
            _allStreams.Clear();
            Listener.GlobalVolume = 100;
        }

        /// <summary>
        /// Closes the device. All <see cref="AudioStreamImp" /> instances get disposed.
        /// </summary>
        public void CloseDevice()
        {
            foreach (var audioStream in _allStreams)
                audioStream.Dispose();
        }

        /// <summary>
        /// Loads an audio file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="streaming"></param>
        /// <returns></returns>
        public IAudioStreamImp LoadFile(string fileName, bool streaming)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Loads a sound file from hard drive.
        ///// </summary>
        ///// <param name="fileName">Full path name of the file with datatype ending, e.g. "C\:sound.ogg". A path can be absolute or relative.</param>
        ///// <param name="streaming">if set to <c>true</c> [streaming].</param>
        ///// <returns>An IAudioStreamImp instance is returned if the file path was correctly resolved.</returns>
        //public IAudioStreamImp LoadFile(string fileName, bool streaming)
        //{
        //    if (Path.GetExtension(fileName) == ".mp3")
        //        throw new Exception("MP3 files are not supported. Please consider using OGG files instead.");

        //    // sound already loaded?
        //    SoundBuffer tmpSndBuffer = null;

        //    if (!streaming)
        //        foreach (
        //            var audioStream in
        //                _allStreams.Where(audioStream => !audioStream.IsStream && audioStream.FileName == fileName))
        //        {
        //            tmpSndBuffer = audioStream.OutputBuffer;
        //            break;
        //        }

        //    IAudioStreamImp tmpAudioStreamImp = tmpSndBuffer == null
        //                         ? new AudioStreamImp(fileName, streaming)
        //                         : new AudioStreamImp(fileName, tmpSndBuffer);

        //    _allStreams.Add((AudioStreamImp) tmpAudioStreamImp);

        //    return tmpAudioStreamImp;
        //}

        /// <summary>
        /// Open a sound file from the given stream.
        /// </summary>
        /// <returns>An IAudioStreamImp instance is returned if the file path was correctly resolved.</returns>
        public IAudioStreamImp OpenFromStream(string id, Stream stream, bool streaming)
        {
            // Sound already loaded?
            SoundBuffer tmpSndBuffer = null;

            if (!streaming)
                foreach (
                    var audioStream in
                        _allStreams.Where(audioStream => !audioStream.IsStream && audioStream.FileName == id))
                {
                    tmpSndBuffer = audioStream.OutputBuffer;
                    break;
                }

            IAudioStreamImp tmpAudioStreamImp = tmpSndBuffer == null
                                 ? new AudioStreamImp(id, streaming)
                                 : new AudioStreamImp(id, tmpSndBuffer);

            _allStreams.Add((AudioStreamImp)tmpAudioStreamImp);

            return tmpAudioStreamImp;
        }

        /// <summary>
        /// Stops the playback of all <see cref="AudioStreamImp" /> instances.
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
            return (float)System.Math.Round(Listener.GlobalVolume, 2);
        }

        /// <summary>
        /// Sets the panning of all <see cref="AudioStreamImp" /> instances.
        /// </summary>
        /// <param name="val">The value(-100 - 100).</param>
        public void SetPanning(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, -100);

            foreach (var audioStream in _allStreams)
                audioStream.Panning = maxVal;
        }

        #endregion Members
    }
}