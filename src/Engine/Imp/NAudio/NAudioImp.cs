using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace Fusee.Engine
{
    public class NAudioImp : IAudioImp
    {
        internal IWavePlayer WaveOutDevice;
        internal WaveMixerStream32 Mixer;

        internal readonly IAudioStream[] AllStreams = new IAudioStream[128];
        internal int LoadedStreams;
    
        public void OpenDevice()
        {
            CloseDevice();

            WaveOutDevice = new DirectSoundOut();

            Mixer = new WaveMixerStream32 {AutoStop = false};

            WaveOutDevice.Init(Mixer);
            WaveOutDevice.Play();

            LoadedStreams = 0;
        }

        public void CloseDevice()
        {
            if (WaveOutDevice != null)
            {
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
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

            if (Mixer != null)
            {
                Mixer.Dispose();
                Mixer = null;
            }    
        }

        public IAudioStream LoadFile(string fileName)
        {
            IAudioStream tmp = new AudioStream(fileName, this);
            AllStreams[LoadedStreams] = tmp;
            LoadedStreams++;
            return tmp;
        }

        public void Play(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream)stream).Play();
        }

        public void Pause()
        {
            WaveOutDevice.Pause();
        }
    }
}
