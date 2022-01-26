using Fusee.Math.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using System.Collections.Generic;

namespace Fusee.Engine.Core.Scene
{
    public class LasCloud : IPointCloudImp
    {
        public List<GpuMesh> MeshesToRender { get; set; }

        public PointCloudFileType FileType => PointCloudFileType.Las;

        public float3 Center { get; private set; }

        public float3 Size { get; private set; }

        public LasCloud()
        {
            
        }

        public void CreateMeshes(IPointReader reader, CreateGpuMesh createGpuMesh)
        {
            MeshesToRender = GetMeshsFromLasFile(reader, createGpuMesh, out var box);
            Center = box.Center;
            Size = box.Size;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="createGpuMesh"></param>
        /// <param name="box"></param>
        /// <param name="doExchangeYZ"></param>
        /// <returns></returns>
        private List<GpuMesh> GetMeshsFromLasFile(IPointReader reader, CreateGpuMesh createGpuMesh, out AABBf box, bool doExchangeYZ = false)
        {
            var pointCnt = reader.MetaInfo.PointCount;
            int maxVertCount = ushort.MaxValue - 1;
            var noOfMeshes = (int)System.Math.Ceiling((float)pointCnt / maxVertCount);
            var meshCnt = 0;
            var meshes = new List<GpuMesh>();
            box = new();

            var ptAccessor = new PosD3ColF3InUsLblBAccessor();
            IPointCloudPoint[] points = new IPointCloudPoint[maxVertCount];
            for (int i = 0; i < pointCnt; i += maxVertCount)
            {
                int numberOfPointsInMesh;
                if (noOfMeshes == 1)
                    numberOfPointsInMesh = points.Length;
                else if (noOfMeshes == meshCnt + 1)
                    numberOfPointsInMesh = (int)(pointCnt - maxVertCount * meshCnt);
                else
                    numberOfPointsInMesh = maxVertCount;
                points = reader.ReadNPoints(numberOfPointsInMesh, ptAccessor);

                GpuMesh mesh = MeshFromPointCloudPoints.GetMesh(ptAccessor, points, doExchangeYZ, float3.Zero, createGpuMesh);
                if (i == 0)
                    box = mesh.BoundingBox;
                else
                    box |= mesh.BoundingBox;

                meshes.Add(mesh);
                meshCnt++;
            }
            reader.Dispose();
            return meshes;

        }
    }
}
