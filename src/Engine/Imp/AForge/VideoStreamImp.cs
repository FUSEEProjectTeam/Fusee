using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Video;
using AForge.Video.FFMPEG;

namespace Fusee.Engine
{

    public class VideoStreamImp :IVideoStreamImp
    {
        private ImageData _nextFrame;

        public VideoStreamImp (VideoFileSource source)
        {
            source.Start();
            //source.FrameInterval = 5000;
            source.NewFrame +=NextFrame;
        }

        public void NextFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap _nextFrameBmp = (Bitmap)eventArgs.Frame;
            BitmapData bmpData = _nextFrameBmp.LockBits(new System.Drawing.Rectangle(0, 0, _nextFrameBmp.Width, _nextFrameBmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int strideAbs = (bmpData.Stride < 0) ? -bmpData.Stride : bmpData.Stride;
            int bytes = (strideAbs) * _nextFrameBmp.Height;

            _nextFrame = new ImageData
            {
                PixelData = new byte[bytes],
                Height = bmpData.Height,
                Width = bmpData.Width,
                Stride = bmpData.Stride
            };

            Marshal.Copy(bmpData.Scan0, _nextFrame.PixelData, 0, bytes);
            eventArgs.Frame.Dispose();
            _nextFrameBmp.UnlockBits(bmpData);


        }

        public ImageData GetCurrentFrame ()
        {
            return _nextFrame;
        }
    }
}
