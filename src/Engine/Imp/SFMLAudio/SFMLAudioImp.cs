namespace Fusee.Engine
{
    public class SFMLAudioImp : IAudioImp
    {
        private readonly IAudioStream[] _allStreams = new IAudioStream[128];
        private int _loadedStreams;

        private float _volume;

        public void OpenDevice()
        {
            _loadedStreams = 0;
            _volume = 100f;
        }

        public void CloseDevice()
        {
            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Dispose();
        }

        public IAudioStream LoadFile(string fileName, bool streaming)
        {
            IAudioStream tmp = new AudioStream(fileName, streaming, this);
            _allStreams[_loadedStreams] = tmp;
            _loadedStreams++;
            return tmp;
        }

        public void Play()
        {
            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Play();
        }

        public void Pause()
        {
            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Pause();
        }

        public void Stop()
        {
            for (var x = 0; x < _loadedStreams; x++)
                _allStreams[x].Stop();
        }

        public void Play(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream) stream).Play();
        }

        public void Pause(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream) stream).Pause();
        }

        public void Stop(IAudioStream stream)
        {
            if (stream != null)
                ((AudioStream) stream).Stop();
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
