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
            WaveOutDevice = new DirectSoundOut();
            WaveOutDevice.Init(MainOutputStream);
        }

        public void LoadFile(string fileName)
        {
            MainOutputStream = CreateInputStream("Assets/tetris.mp3");
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
