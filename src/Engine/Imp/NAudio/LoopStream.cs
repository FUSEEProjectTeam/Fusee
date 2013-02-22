using System;
using NAudio.Wave;

namespace Fusee.Engine
{
    public class LoopStream : WaveStream
    {
        readonly WaveStream _sourceStream;
        internal WaveChannel32 VolumeStream { set; get; }

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
            return inputStream;
        }

        public LoopStream(string fileName)
        {
            _sourceStream = CreateInputStream(fileName);
            EnableLooping = true;
        }

        public bool EnableLooping { get; set; }

        public override WaveFormat WaveFormat
        {
            get { return _sourceStream.WaveFormat; }
        }

        public override long Length
        {
            get { return _sourceStream.Length; }
        }

        public override long Position
        {
            get { return _sourceStream.Position; }
            set { _sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);

                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0 || !EnableLooping)
                        break;

                    _sourceStream.Position = 0;
                }

                totalBytesRead += bytesRead;
            }

            return totalBytesRead;
        }
    }
}
