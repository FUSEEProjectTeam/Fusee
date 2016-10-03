using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Video;
using AForge.Video.FFMPEG;
using AForge.Video.DirectShow;
using Fusee.Base.Common;
using Fusee.Engine.Common;

namespace Fusee.Engine.Imp.Video.Desktop
{

    /// <summary>
    /// This class provides all fuctions to control the video playback and to obtain single images from the stream.
    /// </summary>
    public class VideoStreamImp : IVideoStreamImp
    {
        #region Fields
        private ImageData _nextFrame;
        private VideoFileSource _source;
        private VideoCaptureDevice _videoCaptureDevice;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoStreamImp"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="loopVideo">if set to <c>true</c> loop the video.</param>
        /// <param name="useAudio">if set to <c>true</c> use audio (if existing).</param>
        public VideoStreamImp(VideoFileSource source, bool loopVideo, bool useAudio)
        {
            _source = source;
            _source.NewFrame += NextFrame;
            if (loopVideo)
                _source.PlayingFinished += PlayingFinished;
            _source.VideoSourceError += VideoSourceError;
            _source.WaitForStop();
            _source.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoStreamImp"/> class.
        /// </summary>
        /// <param name="videoCaptureDevice">The video capture device.</param>
        /// <param name="useAudio">if set to <c>true</c> use audio (if existing).</param>
        public VideoStreamImp (VideoCaptureDevice videoCaptureDevice, bool useAudio)
        {
            _videoCaptureDevice = videoCaptureDevice;
            _videoCaptureDevice.NewFrame += NextFrame;
            _videoCaptureDevice.VideoSourceError += VideoSourceError;
            _videoCaptureDevice.Start();
        }
        #endregion

        #region Events

        /// <summary>
        /// This event is called every time a new frame is available.
        /// In this event the ImageData struct is updated with the PixelData from the current frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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

        /// <summary>
        /// This event is called when the stream finishes and loopAudio is set to true.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        public void PlayingFinished (object sender, ReasonToFinishPlaying reason)
        {  
            _source = new VideoFileSource(_source.Source);
            _source.NewFrame += NextFrame;
            _source.PlayingFinished += PlayingFinished;
            _source.Start();

        }

        /// <summary>
        /// This is event is called if an error with ther playback occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void VideoSourceError(object sender, VideoSourceErrorEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Description);
        }
        #endregion

        #region Members

        /// <summary>
        /// Gets the current video frame.
        /// </summary>
        /// <returns>An ImageData-struct containing the current video frame.</returns>
        public ImageData GetCurrentFrame()
        {
            return _nextFrame;
        }

        /// <summary>
        /// Stops the video playback.
        /// </summary>
        public void Stop()
        {
            _source.SignalToStop();
        }

        /// <summary>
        /// Starts the video playback.
        /// </summary>
        public void Start()
        {
            _source.Start();
        }
        #endregion
    }
}

