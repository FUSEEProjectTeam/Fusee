using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Linq;


namespace Fusee.Pointcloud.PointAccessorCollections
{
    /// <summary>
    /// Fore every point type: Define a Method that returns a Mesh.
    /// </summary>
    public static class MeshFromOocFile
    {    
        public static List<Mesh> GetMeshsForNode_Pos64(PointAccessor<Pos64> ptAccessor, List<Pos64> points)
        {
            var allPoints = new List<double3>();
            

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                
                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],
                    Colors = new uint[pointSplit.Count]
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64Col32IShort(PointAccessor<Pos64Col32IShort> ptAccessor, List<Pos64Col32IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allColors = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],
                    //Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt/4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };                

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64IShort(PointAccessor<Pos64IShort> ptAccessor, List<Pos64IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            
            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));                
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();            
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];                
                var intentsitySplit = allIntensitiesSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],                    
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64Col32(PointAccessor<Pos64Col32> ptAccessor, List<Pos64Col32> points)
        {
            var allPoints = new List<double3>();            
            var allColors = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();            

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];                ;

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray()                    
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64Nor32Col32IShort(PointAccessor<Pos64Nor32Col32IShort> ptAccessor, List<Pos64Nor32Col32IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allColors = new List<float3>();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();
            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    //Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64Nor32IShort(PointAccessor<Pos64Nor32IShort> ptAccessor, List<Pos64Nor32IShort> points)
        {
            var allPoints = new List<double3>();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var intentsitySplit = allIntensitiesSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    Colors = intentsitySplit.Select(pt => ColorToUInt((int)(pt / 4096f * 256), (int)(pt / 4096f * 256), (int)(pt / 4096f * 256))).ToArray(),
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        public static List<Mesh> GetMeshsForNode_Pos64Nor32Col32(PointAccessor<Pos64Nor32Col32> ptAccessor, List<Pos64Nor32Col32> points)
        {
            var allPoints = new List<double3>();
            var allColors = new List<float3>();
            var allNormals = new List<float3>();

            for (int i = 0; i < points.Count(); i++)
            {
                var pt = points[i];
                allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
                allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
                allNormals.Add(ptAccessor.GetNormalFloat3_32(ref pt));
            }

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allNormalsSplitted = SplitList(allNormals, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];
                var normalSplitt = allNormalsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = normalSplitt.Select(pt => new float3(pt.xyz)).ToArray(),
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r / 256, (int)pt.g / 256, (int)pt.b / 256)).ToArray()
                };

                allMeshes.Add(currentMesh);
            }

            return allMeshes;
        }

        private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (var i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, M.Min(nSize, locations.Count - i));
            }
        }

        private static uint ColorToUInt(int r, int g, int b)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0));
        }
    }
}
