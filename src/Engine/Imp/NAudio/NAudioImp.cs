using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Fusee.Engine
{
    public class NAudioImp : IAudioImp
    {
        internal IWavePlayer WaveOutDevice;
        internal WaveStream MainOutputStream;
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

        public void OpenDevice()
        {
            CloseDevice();
            WaveOutDevice = new DirectSoundOut();
        }

        public void CloseDevice()
        {
            if (WaveOutDevice != null)
            {
                WaveOutDevice.Stop();
            }

            if (MainOutputStream != null)
            {
                // this one really closes the file and ACM conversion
                VolumeStream.Close();
                VolumeStream = null;
                // this one does the metering stream
                MainOutputStream.Close();
                MainOutputStream = null;
            }

            if (WaveOutDevice != null)
            {
                WaveOutDevice.Dispose();
                WaveOutDevice = null;
            }
        }

        public void LoadFile(string fileName)
        {
            MainOutputStream = CreateInputStream("Assets/tetris.mp3");
            WaveOutDevice.Init(MainOutputStream);
        }

        public void Play()
        {
            WaveOutDevice.Play();
        }

        public void Pause()
        {
            WaveOutDevice.Pause();
        }
    }
}
