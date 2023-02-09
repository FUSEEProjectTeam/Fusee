using Fusee.SLIRP.Common;
using Fusee.SLIRP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.SLIRP.Desktop
{
    public class FrameHistorySnapshoter
    {
        string snapshotPath;

        FrameHistoryController gbReader;

        public FrameHistorySnapshoter(FrameHistoryController gbReader, string snapshotPath)
        {
            this.gbReader = gbReader;
            this.snapshotPath = snapshotPath;
        }

        public void SnapshotRandomFrameOfHistory()
        {
            Random rndmGen = new Random();

            int rndm = rndmGen.Next(0, gbReader.Capacity);

            Console.WriteLine("Save random BufferSet of the history at " + rndm);
            SnapshotBufferSetAt(rndm);
        }

        public void SnapshotFrameHistory()
        {
            Console.WriteLine("Save all BufferSets of the history");
            for (int i = 0; i < gbReader.Capacity; i++)
            {
                SnapshotBufferSetAt(i);
            }
        }

        public void SnapshotBufferSetAt(int i)
        {
            BufferSet latest = gbReader.PeekBufferSetAt(i);

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = "RGBframe_" + i + "_" + dateTime + ".png";
            SnapshotController.SaveSnapshotAsPng(latest.frameBuffer, latest.width, latest.height, snapshotPath, filename);
            filename = "Depth_" + i + "_" + dateTime + ".png";
            SnapshotController.SaveSnapshotAsPng(latest.depthBuffer, latest.width, latest.height, snapshotPath, filename);
        }

        public void SnapshotLastBufferSet()
        {
            BufferSet latest = gbReader.PeekLastBufferSet();

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string filename = "RGBframe_" + dateTime + ".png";
            SnapshotController.SaveSnapshotAsPng(latest.frameBuffer, latest.width, latest.height, snapshotPath, filename);
            filename = "Depth_" + dateTime + ".png";
            SnapshotController.SaveSnapshotAsPng(latest.depthBuffer, latest.width, latest.height, snapshotPath, filename);
        }
    }
}
