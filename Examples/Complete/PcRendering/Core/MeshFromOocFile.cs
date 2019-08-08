using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Linq;


namespace Fusee.Examples.PcRendering.Core
{

    internal static class MeshFromOocFile
    {
        //internal static SceneNodeContainer ToSceneNodeContainer(string pathToPc, ShaderEffect effect)
        //{
        //    var reader = new LASPointReader(pathToPc);
        //    var pointCnt = (MetaInfo)reader.MetaInfo;
        //    var pa = new PtRenderingAccessor();
        //    var points = new LAZPointType[(int)pointCnt.PointCnt];
        //    points = points.Select(pt => new LAZPointType()).ToArray();

        //    for (var i = 0; i < points.Length; i++)
        //        if (!reader.ReadNextPoint(ref points[i], pa)) break;

        //    var allPos = points.ToArray().Select(pt => new float3((float)pt.Position.x, (float)pt.Position.z, (float)pt.Position.y)).ToList();
        //    var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
        //    var allColors = points.ToArray().Select(pt => new float3(pt.Color.r / 256, pt.Color.g / 256, pt.Color.b / 256)).ToList();

        //    var allMeshes = new List<Mesh>();

        //    var maxVertCount = ushort.MaxValue - 1;

        //    var allPointsSplitted = SplitList(allPos, maxVertCount).ToList();
        //    var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
        //    var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

        //    var returnNodeContainer = new SceneNodeContainer
        //    {
        //        Components = new List<SceneComponentContainer>
        //        {
        //            new TransformComponent
        //            {
        //                Rotation = float3.Zero,
        //                Scale = float3.One,
        //                Translation = -allPos[0].xyz
        //            },
        //            new ShaderEffectComponent
        //            {
        //                Effect = effect
        //            }
        //        }
        //    };

        //    for (int i = 0; i < allPointsSplitted.Count; i++)
        //    {
        //        var pointSplit = allPointsSplitted[i];
        //        var colorSplit = allColorsSplitted[i];

        //        var currentMesh = new Mesh
        //        {
        //            Vertices = pointSplit.Select(pt => pt.xyz).ToArray(),
        //            Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
        //            MeshType = (int)OpenGLPrimitiveType.POINT,
        //            Normals = new float3[pointSplit.Count],
        //            Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r, (int)pt.g, (int)pt.b)).ToArray(),
        //        };

        //        returnNodeContainer.Components.Add(currentMesh);
        //    }

        //    return returnNodeContainer;
        //}

        //internal static List<LAZPointType> ToList(string pathToPc)
        //{
        //    var reader = new LASPointReader(pathToPc);
        //    var pointCnt = (MetaInfo)reader.MetaInfo;
        //    var pa = new PtRenderingAccessor();
        //    var points = new LAZPointType[(int)pointCnt.PointCnt];
        //    points = points.Select(pt => new LAZPointType()).ToArray();

        //    for (var i = 0; i < points.Length; i++)
        //        if (!reader.ReadNextPoint(ref points[i], pa)) break;


        //    var firstPoint = points[0];

        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        var pt = points[i];                
        //        pt.Position -= firstPoint.Position;
        //        pt.Position = new double3(pt.Position.x, pt.Position.z, pt.Position.y);

        //        points[i] = pt;
        //    }

        //    reader.Dispose();
        //    return points.ToList();
        //}

        /// <summary>
        /// Splits a long list to list chunks with given size.
        /// </summary> 
        internal static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (var i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, M.Min(nSize, locations.Count - i));
            }
        }    
               
        internal static List<Mesh> GetMeshsForNode(PointAccessor<LAZPointType> ptAccessor, List<LAZPointType> points)
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

        private static uint ColorToUInt(int r, int g, int b)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0));
        }
    }
}
