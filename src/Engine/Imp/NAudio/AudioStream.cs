using System;
using NAudio.Wave;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        internal NAudioImp Audio;

        internal WaveStream MainOutputStream { get; set; }
        internal WaveChannel32 VolumeStream;

        private WaveStream CreateInputStream(string fileName)
        {
            WaveChannel32 inputStream;
            if (fileName.EndsWith(".mp3"))
            {
                WaveStream mp3Reader = new Mp3FileReader(fileName);
                inputStream = new WaveChannel32(mp3Reader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }

            VolumeStream = inputStream;
            return VolumeStream;
        }

        public AudioStream(string fileName, NAudioImp audioCl)
        {
            Audio = audioCl;

            MainOutputStream = CreateInputStream(fileName);
        }

        public void Play()
        {
            Audio.Mixer.AddInputStream(VolumeStream);
        }

        public void Stop()
        {
            Audio.Mixer.RemoveInputStream(VolumeStream);
        }

        public void Dispose()
        {
            if (VolumeStream != null)
            {
                Audio.Mixer.RemoveInputStream(VolumeStream);

                VolumeStream.Close();
                VolumeStream = null;
            }

            if (MainOutputStream != null)
            {
                MainOutputStream.Close();
                MainOutputStream = null;
            }
        }
    }
}