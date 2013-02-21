using System;
using NAudio.Wave;

namespace Fusee.Engine
{
    class AudioStream : IAudioStream
    {
        internal IWavePlayer WaveOutDevice;

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

        public AudioStream(string fileName, IWavePlayer waveOut)
        {
            MainOutputStream = CreateInputStream(fileName);
            WaveOutDevice = waveOut;
        }

        public void Play()
        {
            WaveOutDevice.Init(MainOutputStream);
            WaveOutDevice.Play();
        }

        public void Dispose()
        {
            if (MainOutputStream != null)
            {
                // this one really closes the file and ACM conversion
                VolumeStream.Close();
                VolumeStream = null;
                // this one does the metering stream
                MainOutputStream.Close();
                MainOutputStream = null;
            }
        }
    }
}