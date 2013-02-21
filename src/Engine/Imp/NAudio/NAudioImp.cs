using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Fusee.Engine
{
    public class NAudioImp : IAudioImp
    {
        internal IWavePlayer WaveOutDevice;

        internal readonly IAudioStream[] AllStreams = new IAudioStream[128];
        internal int LoadedStreams;
    
        public void OpenDevice()
        {
            CloseDevice();
            WaveOutDevice = new DirectSoundOut();

            LoadedStreams = 0;
        }

        public void CloseDevice()
        {
            if (WaveOutDevice != null)
            {
                WaveOutDevice.Stop();
            }

            for (var x = 0; x < LoadedStreams; x++)
            {
                AllStreams[x].Dispose();
            }

            if (WaveOutDevice != null)
            {
                WaveOutDevice.Dispose();
                WaveOutDevice = null;
            }
        }

        public IAudioStream LoadFile(string fileName)
        {
            IAudioStream tmp = new AudioStream(fileName, WaveOutDevice);
            AllStreams[LoadedStreams] = tmp;
            LoadedStreams++;
            return tmp;
        }

        public void Play(IAudioStream stream)
        {
            if (stream != null)
            {
                WaveOutDevice.Init(((AudioStream) stream).MainOutputStream);
                WaveOutDevice.Play();
            }
        }

        public void Pause()
        {
            WaveOutDevice.Pause();
        }
    }
}
