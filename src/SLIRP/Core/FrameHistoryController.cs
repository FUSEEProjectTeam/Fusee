using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.SLIRP.Common;

namespace Fusee.SLIRP.Core
{
    /// <summary>
    /// Stores a passed amount of <cref>BufferSet</cref>s. If the stored <cref>BufferSet</cref>s reach the capacity, the oldest is discarded
    /// and the new <cref>BufferSet</cref> can be stored.
    /// Can be used to run automatically on the render loop or by calling it manually per frame.
    /// </summary>
    public class FrameHistoryController
    {
        private int capacity;

        private StackBuffer<BufferSet> recordedFrames;
        private RenderCanvasImp canvasImp;

        public int Capacity { get => capacity; }

        public FrameHistoryController(RenderCanvasImp canvasImp, int capacity = 25, bool autoPostRender = true)
        {
            this.capacity = capacity;
            this.canvasImp = canvasImp;

            recordedFrames = new StackBuffer<BufferSet>(capacity);

            if(autoPostRender) 
                this.canvasImp.PostRender += OnPostRender;
        }

        public BufferSet PopLastBufferSet()
        {
            return recordedFrames.Pop();
        }

        public BufferSet PopBufferSet(int n)
        {
            return recordedFrames.Pop(n);
        }

        public BufferSet PeekLastBufferSet()
        {
            return recordedFrames.Peek();
        }

        /// <summary>
        /// Peek at a specific position "i" without altering the history.
        /// </summary>
        /// <param name="i">The position to peek at. 0 is the latest, "capacity"-1 the oldest.</param>
        /// <returns></returns>
        public BufferSet PeekBufferSetAt(int i)
        {
            return recordedFrames.PeekAt(i);
        }


        public void Destroy()
        {
            canvasImp.PostRender -= OnPostRender;
        }

        public void OnPostRender(object sender, PostRenderEventArgs args)
        {
            PushCurrentBuffers();
        }

        public void PushCurrentBuffers()
        {
            var frameBuffer = canvasImp.GetFrameBuffer(canvasImp.Width, canvasImp.Height);
            var depthBuffer = canvasImp.GetDepthBuffer(canvasImp.Width, canvasImp.Height);

            BufferSet newSet = new BufferSet(0, 0, canvasImp.Width, canvasImp.Height, frameBuffer, depthBuffer);

            recordedFrames.Push(newSet);
        }
    }
}