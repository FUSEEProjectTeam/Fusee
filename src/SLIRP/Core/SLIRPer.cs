using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.SLIRP.Common;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Fusee.SLIRP.Desktop;
using OpenTK.Windowing.Desktop;
using System.Data;
using OpenTK.Graphics.ES11;
using Fusee.SLIRP.DataTransformation;

namespace Fusee.SLIRP.Core
{
    [FuseeApplication(Name = "SLIRP Example", Description = " ")]
    public class SLIRPer
    {

        private string snapshotRoot = "C:\\Users\\Marc\\Daten\\00_SLIRP\\Snapshots";
        private DirectoryInfo snapshotCurDir;

        private const int height = 512;
        private const int width = 512;

        FrameHistorySnapshoter gbSnapshoter;
        private FrameHistoryController gbReader;
        private RenderCanvas renderer;

        IEncoder encoder;
        EncodingMeta encoderMeta;

        private int historyDepth = 75;

        int debugIndex = 0;
        int debugSnapshotIndex = 100;

        #region Init
        public void Init(RenderCanvas renderCanvas = null)
        {
            Console.WriteLine("Init SLIRPer " + (renderCanvas == null ? "new" : "with " + renderCanvas.ToString()));

            if (renderCanvas == null)
            {
                Console.WriteLine("Create Renderer");
                renderer = new SLIRPRenderer();
            }
            else
                renderer = renderCanvas;

            var cimp = (RenderCanvasImp)renderer.CanvasImplementor;

            //Render Context und Canvas für die Injection
            InitRenderCanvas();

            //create a frame history
            InitFrameHistory(cimp);

            //create the posibility to save the frame history to disc as images
            InitFrameHistorySnapshoter();

            //create encoder
            InitEncoding(cimp);

            //irgendwas mit Input
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(renderer.CanvasImplementor));
            Input.AddDriverImp(new Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(renderer.CanvasImplementor));

            //initialize render app
            InitAppAndRendering();

            renderer.EndOfFrame += OnEndOffFrame;

            renderer.Run();

        }

        #region Init Methods
        private void InitAppAndRendering()
        {
            Console.WriteLine("App initialize SLIRPer");
            renderer.InitApp();

            renderer.CanvasImplementor.DoInit();
            renderer.CanvasImplementor.DoResize(width, height);
            renderer.CanvasImplementor.DoUpdate();

            SpinWait.SpinUntil(() => renderer.IsLoaded);
        }

        private void InitEncoding(RenderCanvasImp cimp)
        {
            encoder = new SixLabsJPEGTransformer();
            encoderMeta = new SixLabsEncodingMeta(Configuration.Default, cimp.Width * cimp.Height);
            encoder.Init(encoderMeta);
        }

        private void InitFrameHistorySnapshoter()
        {
            string snapshotFolderName = "Testshots_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            snapshotCurDir = Directory.CreateDirectory(Path.Combine(snapshotRoot, snapshotFolderName));
            gbSnapshoter = new FrameHistorySnapshoter(gbReader, snapshotCurDir.FullName);
        }

        private void InitFrameHistory(RenderCanvasImp cimp)
        {
            Console.WriteLine("Create GraphicBufferController and pass CanvasImplementor.");
            gbReader = new FrameHistoryController(cimp, historyDepth, false);
        }

        private void InitRenderCanvas()
        {
            Console.WriteLine("Do Canvas and Context Stuff");
            var icon = AssetStorage.Get<ImageData>("FuseeIconTop32.png");
            renderer.CanvasImplementor = new Engine.Imp.Graphics.Desktop.RenderCanvasImp(icon);
            renderer.ContextImplementor = new Engine.Imp.Graphics.Desktop.RenderContextImp(renderer.CanvasImplementor);

            renderer.CanvasImplementor.PostRender += OnPostRender;
        }
        #endregion Init Methods

        #endregion Init

        public void OnEndOffFrame(object sender, EventArgs args)
        {
            Process();
        }

        public void Process()
        {
            //Console.WriteLine("Process SLIRPer");
            //Console.WriteLine("Start Render");
            //renderer.CanvasImplementor.DoRender();
            //Console.WriteLine("Rendered");
            //Console.WriteLine("Present  -->");
            //renderer.Present();
            // renderer.RenderAFrame();
            OnPostRender(this, null);

            TestEncoding();
        }

        private void TestEncoding()
        {
            debugIndex++;

            if (debugIndex > debugSnapshotIndex)
            {
                debugIndex = 0;

                Console.WriteLine("Save latest BufferSet");
                gbSnapshoter.SnapshotLastBufferSet();

                lock (gbReader)
                {
                    BufferSet latestSet = gbReader.PeekLastBufferSet();

                    Console.WriteLine("\nTest Encoding");

                    Console.WriteLine("Byte length: " + latestSet.frameBuffer.Length);

                    string dateTime = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
                    string filename = "RGB_" + dateTime + ".jpeg";
                    string realPath = Path.Combine(snapshotCurDir.FullName, filename);

                    Console.WriteLine("Save as jpeg");
                    var loadedImg = Image.LoadPixelData<Bgra32>(latestSet.frameBuffer, latestSet.width, latestSet.height);
                    loadedImg.SaveAsJpeg(realPath);

                    Console.WriteLine("Save raw image");
                    filename = "RawRGB_" + dateTime + ".raw";
                    realPath = Path.Combine(snapshotCurDir.FullName, filename);
                    using (FileStream fs = new FileStream(realPath, System.IO.FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(latestSet.frameBuffer, 0, latestSet.frameBuffer.Length);
                    }
                    Console.WriteLine("Raw image saved!");

                    Console.WriteLine("\nStart encoding");

                    using (MemoryStream dataStream = new MemoryStream())
                    {
                        Console.WriteLine("Benchmark: Start at " + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss"));
                        DateTime start = DateTime.Now;
                        encoder.Encode(latestSet.frameBuffer, dataStream, latestSet.width, latestSet.height);
                        DateTime end = DateTime.Now;
                        Console.WriteLine("Benchmark: End at " + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss"));
                        Console.WriteLine("Encoding finished!");

                        int duration = (end - start).Milliseconds;
                        Console.WriteLine("Encoding took " + duration + "ns");


                        Console.WriteLine("\nSave encoded image");
                        filename = "EncodedRGB_" + dateTime + ".jpeg";
                        realPath = Path.Combine(snapshotCurDir.FullName, filename);

                        dataStream.Seek(0, SeekOrigin.Begin);
                        using (FileStream fs = File.Create(realPath))
                        {
                            dataStream.CopyTo(fs);
                        }
                        dataStream.Seek(0, SeekOrigin.Begin);

                        Console.WriteLine("Encoded image saved!");


                        Console.WriteLine("\nTest Decoding");

                        Console.WriteLine("Start decoding");
                        Console.WriteLine("Benchmark: Start at " + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss"));
                        start = DateTime.Now;
                        byte[] decodedImg = encoder.Decode(dataStream, latestSet.width, latestSet.height);
                        end = DateTime.Now;
                        Console.WriteLine("Benchmark: End at " + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss"));
                        Console.WriteLine("Decoding finished!");

                        duration = (end - start).Milliseconds;
                        Console.WriteLine("Decoding took " + duration + "ns");

                        Console.WriteLine("Byte length: " + decodedImg.Length);

                        Console.WriteLine("Save decoded image");
                        filename = "DecodedRGB_" + dateTime + ".raw";
                        realPath = Path.Combine(snapshotCurDir.FullName, filename);
                        using (FileStream fs = new FileStream(realPath, System.IO.FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(decodedImg, 0, decodedImg.Length);
                        }
                        Console.WriteLine("Decoded image saved!");
                    }
                    Console.WriteLine("Finish Encoding Test");
                }
            }
        }

        public void OnPostRender(object sender, PostRenderEventArgs args)
        {
            //Console.WriteLine("OnPostRender SLIRPer");
            gbReader.PushCurrentBuffers();
            //BufferSet lastBufferSet = gbReader.PeekLastBufferSet();

        }

    }
}
