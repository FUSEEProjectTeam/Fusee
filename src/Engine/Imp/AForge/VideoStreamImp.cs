using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Video;
using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;

namespace Fusee.Engine
{

    public class VideoStreamImp : IVideoStreamImp
    {
        #region Fields
        private ImageData _nextFrame;
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;
        #endregion

        #region Constructors
        public VideoStreamImp(VideoFileSource source)
        {
            _source = source;
            _source.NewFrame += NextFrame;
            _source.PlayingFinished += PlayingFinished;
            _source.Start();
        }

        public VideoStreamImp (VideoCaptureDevice videoCaptureDevice)
        {
            _videoCaptureDevice = videoCaptureDevice;
            _videoCaptureDevice.NewFrame += NextFrame;
            _videoCaptureDevice.Start();
        }
        #endregion

        #region Events

        public void NextFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap nextFrameBmp = (Bitmap)eventArgs.Frame;
            nextFrameBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData bmpData = nextFrameBmp.LockBits(new System.Drawing.Rectangle(0, 0, nextFrameBmp.Width, nextFrameBmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * nextFrameBmp.Height;

            _nextFrame = new ImageData
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                PixelFormat = ImagePixelFormat.RGB,
                Stride = bmpData.Stride
            };
            
            Marshal.Copy(bmpData.Scan0, _nextFrame.PixelData, 0, bytes);
            nextFrameBmp.UnlockBits(bmpData);
        }

        public void PlayingFinished (object sender, ReasonToFinishPlaying reason)
        {
            _source.SignalToStop();
            _source.Start();
            _source.NewFrame += NextFrame;
        }
        #endregion

        //TODO:Property
        #region Members
        public ImageData GetCurrentFrame()
        {
            return _nextFrame;
        }

        public void Stop()
        {
            _source.SignalToStop();
        }

        public void Start()
        {
            _source.Start();
        }
        #endregion
    }
}
