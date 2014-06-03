using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class VideoManager
    {
        private static VideoManager _instance;
        private IVideoManagerImp _videoManagerImp;

        internal IVideoManagerImp VideoManagerImp
        {
            set { _videoManagerImp = value; }
        }

        public VideoStream LoadVideoFromFile (string filename)
        {
            var source = new VideoStream();
            source._imp = _videoManagerImp.CreateVideoStreamImpFromFile(filename);
            return source;
        }

        public VideoStream LoadVideoFromCamera()
        {
            var source = new VideoStream();
            source._imp = _videoManagerImp.CreateVideoStreamImpFromCamera();
            return source;
        }

        public static VideoManager Instance
        {
            get { return _instance ?? (_instance = new VideoManager()); }
        }
    }
}
