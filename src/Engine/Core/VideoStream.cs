using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Fusee.Engine
{
    public class VideoStream
    {
        public IVideoStreamImp _imp;

        public ImageData GetCurrentFrame ()
        {
            return _imp.GetCurrentFrame();
        }

    }
}
