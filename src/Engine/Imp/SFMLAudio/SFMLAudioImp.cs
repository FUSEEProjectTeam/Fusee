#define MP3Warning

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SFML.Audio;

namespace Fusee.Engine
{
    public class SFMLAudioImp : IAudioImp
    {
        private readonly List<AudioStream> _allStreams;

        private float _volume;

        public SFMLAudioImp()
        {
            _allStreams = new List<AudioStream>();

            Listener.Direction = new Vector3F(0, 0, -0.1f);
            Listener.Position = new Vector3F(0, 0, 0.1f);
        }

        public void OpenDevice()
        {
            _allStreams.Clear();
            _volume = 100f;
        }

        public void CloseDevice()
        {
            foreach (var audioStream in _allStreams)
               audioStream.Dispose();
        }

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
                                 ? new AudioStream(fileName, streaming, this)
                                 : new AudioStream(fileName, tmpSndBuffer, this);

            _allStreams.Add((AudioStream) tmpAudioStream);

            return tmpAudioStream;
        }

        public void Stop()
        {
            foreach (var audioStream in _allStreams)
                audioStream.Stop();
        }

        public void SetVolume(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, 0);

            _volume = maxVal;

            foreach (var audioStream in _allStreams)
                audioStream.SetGlobalVolume(maxVal);
        }

        public float GetVolume()
        {
            return _volume;
        }

        public void SetPanning(float val)
        {
            var maxVal = System.Math.Min(100, val);
            maxVal = System.Math.Max(maxVal, -100);

            foreach (var audioStream in _allStreams)
                audioStream.Panning = maxVal;
        }
    }
}
