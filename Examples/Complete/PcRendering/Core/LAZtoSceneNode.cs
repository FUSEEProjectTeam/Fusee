using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.LASReader;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Fusee.Pointcloud.OoCFileReaderWriter;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Examples.PcRendering.Core
{

    internal class LAZPointType
    {
        public double3 Position;
        public float3 Color;
        public ushort Intensity;
        public int3 GridIndex;
    }

    internal static class LAZtoSceneNode
    {
        public static Lighting Lighting = Lighting.EDL;
        public static PointShape Shape = PointShape.CIRCLE;
        public static PointSizeMode PtMode = PointSizeMode.ADAPTIVE_SIZE;
        public static ColorMode ColorMode = ColorMode.SINGLE;
        public static int Size = 5;
        public static float4 SingleColor = new float4(0, 1, 1, 1);
        public static int EdlNoOfNeighbourPx = 3;
        public static float EdlStrength = 0.5f;


        private static uint ColorToUInt(int r, int g, int b)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0));
        }

        internal static SceneNodeContainer FromLAZ(string pathToPc, ShaderEffect effect)
        {
            var reader = new LASPointReader(pathToPc);
            var pointCnt = (MetaInfo)reader.MetaInfo;
            var pa = new PtRenderingAccessor();
            var points = new LAZPointType[(int)pointCnt.PointCnt];
            points = points.Select(pt => new LAZPointType()).ToArray();
           
            for (var i = 0; i <points.Length; i++)
                if (!reader.ReadNextPoint(ref points[i], pa)) break;

            var allPos = points.ToArray().Select(pt => new float3((float)pt.Position.x, (float)pt.Position.z, (float)pt.Position.y)).ToList();
            var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
            var allColors = points.ToArray().Select(pt => new float3(pt.Color.r / 256, pt.Color.g / 256, pt.Color.b / 256)).ToList();

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPos, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
            var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            var returnNodeContainer = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                {
                    new TransformComponent
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = -allPos[0].xyz
                    },
                    new ShaderEffectComponent
                    {
                        Effect = effect
                    }
                }
            };

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => pt.xyz).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r, (int)pt.g, (int)pt.b)).ToArray(),                    
                };

                returnNodeContainer.Components.Add(currentMesh);
            }            

            return returnNodeContainer;
        }

        //internal static SceneNodeContainer FromPointList<TPoint>(PointAccessor<TPoint> ptAccessor, List<TPoint> points, ShaderEffect effect)
        //{
        //    var allPoints = new List<double3>();

        //    //var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
        //    var allColors = new List<float3>();

        //    for (int i = 0; i < points.Count(); i++)
        //    {
        //        var pt = points[i];
        //        allPoints.Add(ptAccessor.GetPositionFloat3_64(ref pt));
        //        allColors.Add(ptAccessor.GetColorFloat3_32(ref pt));
        //    }

        //    var allMeshes = new List<Mesh>();

        //    var maxVertCount = ushort.MaxValue - 1;

        //    var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
        //    var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
        //    //var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

        //    var returnNodeContainer = new SceneNodeContainer
        //    {
        //        Name = "PointCloud",
        //        Components = new List<SceneComponentContainer>
        //        {
        //            new TransformComponent
        //            {
        //                Rotation = float3.Zero,
        //                Scale = float3.One,
        //                Translation = new float3(0,0,0)
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
        //            Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
        //            Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
        //            MeshType = (int)OpenGLPrimitiveType.POINT,
        //            Normals = new float3[pointSplit.Count],
        //            Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r, (int)pt.g, (int)pt.b)).ToArray(),
        //        };

        //        returnNodeContainer.Components.Add(currentMesh);
        //    }

        //    return returnNodeContainer;
        //}

        //internal static SceneNodeContainer FromPointList(List<LAZPointType> points, ShaderEffect effect)
        //{
        //    var allPoints = points.ToArray().Select(pt => new float3((float)pt.Position.x, (float)pt.Position.y, (float)pt.Position.z)).ToList();
        //    var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
        //    var allColors = points.ToArray().Select(pt => new float3(pt.Color.r / 256, pt.Color.g / 256, pt.Color.b / 256)).ToList();

        //    var allMeshes = new List<Mesh>();

        //    var maxVertCount = ushort.MaxValue - 1;

        //    var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
        //    var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();
        //    var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

        //    var returnNodeContainer = new SceneNodeContainer
        //    {
        //        Name = "PointCloud",
        //        Components = new List<SceneComponentContainer>
        //        {
        //            new TransformComponent
        //            {
        //                Rotation = float3.Zero,
        //                Scale = float3.One,
        //                Translation = new float3(0,0,0)
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

        internal static List<LAZPointType> ListFromLAZ(string pathToPc)
        {
            var reader = new LASPointReader(pathToPc);
            var pointCnt = (MetaInfo)reader.MetaInfo;
            var pa = new PtRenderingAccessor();
            var points = new LAZPointType[(int)pointCnt.PointCnt];
            points = points.Select(pt => new LAZPointType()).ToArray();

            for (var i = 0; i < points.Length; i++)
                if (!reader.ReadNextPoint(ref points[i], pa)) break;


            var firstPoint = points[0];

            for (int i = 0; i < points.Length; i++)
            {
                var pt = points[i];

                pt.Position -= firstPoint.Position;
                pt.Position = new double3(pt.Position.x, pt.Position.z, pt.Position.y);

                points[i] = pt;
            }

            return points.ToList();
        }

        internal static ShaderEffect DepthPassEffect(float2 screenParams, float initCamPosZ, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointCloud.vert"),
                    PS = AssetStorage.Get<string>("PointDepth.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},

                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},

                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape},
                new EffectParameterDeclaration {Name = "PointMode", Value = (int)PtMode},

                new EffectParameterDeclaration {Name = "OctantRes", Value = 0f},
                new EffectParameterDeclaration {Name = "OctantLevel", Value = 0},

                new EffectParameterDeclaration {Name = "OctreeTex", Value = octreeTex},
                new EffectParameterDeclaration {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new EffectParameterDeclaration {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new EffectParameterDeclaration {Name = "OctreeRootLength", Value = (float)octreeRootLength},
            });            
        }

        internal static ShaderEffect StandardEffect(float2 screenParams, float initCamPosZ, float2 clipPlaneDist, ITextureHandle depthTexHandle, Texture octreeTex, double3 octreeRootCenter, double octreeRootLength)
        {
            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointCloud.vert"),
                    PS = AssetStorage.Get<string>("PointCloud.frag"),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},

                new EffectParameterDeclaration {Name = "ClipPlaneDist", Value = clipPlaneDist},
                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "InitCamPosZ", Value = System.Math.Abs(initCamPosZ)},
                new EffectParameterDeclaration {Name = "Color", Value = SingleColor},

                new EffectParameterDeclaration {Name = "PointMode", Value = (int)PtMode},
                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape},
                new EffectParameterDeclaration {Name = "ColorMode", Value = (int)ColorMode},

                new EffectParameterDeclaration {Name = "Lighting", Value = (int)Lighting},
                new EffectParameterDeclaration{Name = "DepthTex", Value = depthTexHandle},
                new EffectParameterDeclaration{Name = "EDLStrength", Value = EdlStrength},
                new EffectParameterDeclaration{Name = "EDLNeighbourPixels", Value = EdlNoOfNeighbourPx},
                new EffectParameterDeclaration {Name = "SpecularStrength", Value = 0.5f},
                new EffectParameterDeclaration {Name = "Shininess", Value = 200f},                
                new EffectParameterDeclaration {Name = "SpecularColor", Value = new float4(1,1,1,1)},

                new EffectParameterDeclaration {Name = "OctantRes", Value = 0f},
                new EffectParameterDeclaration {Name = "OctantLevel", Value = 0},

                new EffectParameterDeclaration {Name = "OctreeTex", Value = octreeTex},
                new EffectParameterDeclaration {Name = "OctreeTexWidth", Value = octreeTex.Width}, //Used to access a specific pixel in the tex
                new EffectParameterDeclaration {Name = "OctreeRootCenter", Value = (float3)octreeRootCenter},
                new EffectParameterDeclaration {Name = "OctreeRootLength", Value = (float)octreeRootLength},
            });
        }

        /// <summary>
        ///     Splits a long list to list chunks with given size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locations"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
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
            //var allIntensities = points.ToArray().Select(pt => (float)pt.Intensity).ToList();
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
            //var allIntensitiesSplitted = SplitList(allIntensities, maxVertCount).ToList();

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => new float3(pt.xyz)).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = new float3[pointSplit.Count],
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r, (int)pt.g, (int)pt.b)).ToArray(),
                };

                allMeshes.Add(currentMesh);                
            }

            return allMeshes;
        }
    }

    
}
