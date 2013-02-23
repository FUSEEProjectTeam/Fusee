using System;
using NAudio.Wave;

namespace Fusee.Engine
{
    public class LoopStream : WaveStream
    {
        private WaveChannel32 _volumeStream;

        public enum PlaybackState
        {
            Stopped,
            Playing,
            Paused,
        }

        private void CreateInputStream(string fileName)
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

            _volumeStream = inputStream;
        }

        public LoopStream(string fileName)
        {
            CreateInputStream(fileName);
            Loop = true;
        }

        public void CloseStream()
        {
            if (_volumeStream != null)
                _volumeStream.Close();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // last state
            if (_state != PlaybackState.Playing)
            {
                if (_state == PlaybackState.Paused)
                    _volumeStream.Position = _currentPos;

                if (_state == PlaybackState.Stopped)
                    _volumeStream.Position = 0;

                // now playing again
                _state = PlaybackState.Playing;                
            }

            _currentPos = _volumeStream.Position;

            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _volumeStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (_volumeStream.Position > _volumeStream.Length)
                    if (Loop)
                        _volumeStream.Position = 0;

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }

        private long _currentPos;
        public bool Loop { get; set; }

        private PlaybackState _state;
        public PlaybackState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public override WaveFormat WaveFormat
        {
            get { return _volumeStream.WaveFormat; }
        }

        public override long Length
        {
            get { return _volumeStream.Length; }
        }

        public override long Position
        {
            get { return _volumeStream.Position; }
            set { _volumeStream.Position = value; }
        }

        public float Volume
        {
            get { return _volumeStream.Volume; }
            set { _volumeStream.Volume = value; }
        }
    }
}
