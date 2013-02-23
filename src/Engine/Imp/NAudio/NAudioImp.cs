using NAudio.Wave;

namespace Fusee.Engine
{
    public class NAudioImp : IAudioImp
    {
        internal IWavePlayer WaveOutDevice;
        internal WaveMixerStream32 Mixer;

        private readonly IAudioStream[] _allStreams = new IAudioStream[128];
        private int _loadedStreams;
        private float _volume;

        public void OpenDevice()
        {
            CloseDevice();

            WaveOutDevice = new DirectSoundOut();

            Mixer = new WaveMixerStream32 {AutoStop = false};
                                        
            WaveOutDevice.Init(Mixer);
            WaveOutDevice.Play();

            _loadedStreams = 0;
            _volume = 1.0f;
        }

        public void CloseDevice()
        {
            if (WaveOutDevice != null)
                if (WaveOutDevice.PlaybackState == PlaybackState.Playing)
                    WaveOutDevice.Stop();

            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Dispose();

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
            _allStreams[_loadedStreams] = tmp;
            _loadedStreams++;
            return tmp;
        }

        public void Play()
        {
            if (WaveOutDevice == null) return;

            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Play();
        }

        public void Pause()
        {
            if (WaveOutDevice == null) return;

            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Pause();
        }

        public void Stop()
        {
            if (WaveOutDevice == null) return;

            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Stop();
        }

        public void Play(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream)stream).Play();
        }

        public void Pause(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream)stream).Pause();
        }

        public void Stop(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream)stream).Stop();
        }

        public void SetVolume(float val)
        {
            var compr = _volume / val;

            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Volume *= compr;

            _volume = val;            
        }

        public float GetVolume()
        {
            return _volume;
        }
    }
}
