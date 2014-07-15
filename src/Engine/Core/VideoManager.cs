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

        public IVideoStreamImp LoadVideoFromFile (string filename)
        {
            return _videoManagerImp.CreateVideoStreamImpFromFile(filename);
        }

        public IVideoStreamImp LoadVideoFromCamera()
        {
            return _videoManagerImp.CreateVideoStreamImpFromCamera();
        }

        public static VideoManager Instance
        {
            get { return _instance ?? (_instance = new VideoManager()); }
        }
    }
}
