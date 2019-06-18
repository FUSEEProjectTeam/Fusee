using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Linq;
using LASlibNet;

namespace Fusee.Examples.LASReaderExample.Core
{
    public static class LAZtoSceneNode
    {
        public static PointShape Shape = PointShape.PARABOLID;
        public static ColorMode ColorMode = ColorMode.SINGLE;
        public static int Size = 20;
        public static float4 SingleColor = new float4(1, 1, 1, 1);
        public static int EdlNoOfNeighbourPx = 3;
        public static float EdlStrength = 0.5f;


        private static uint ColorToUInt(int r, int g, int b)
        {
            return (uint)((b << 16) | (g << 8) | (r << 0));
        }

        public static SceneNodeContainer FromLAZ(string fileName, ShaderEffect effect)
        {
            var lazReader = new LASReader(fileName);
            var points = lazReader.Points.ToList();

            var allPoints = points.Select(pt => new float4((float)pt.X, (float)pt.Z, (float)pt.Y, pt.intensity)).ToList();
            var allColors = points.Select(pt => new float3(pt.R / 256, pt.G / 256, pt.B / 256)).ToList();

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount).ToList();
            var allColorsSplitted = SplitList(allColors, maxVertCount).ToList();

            var returnNodeContainer = new SceneNodeContainer
            {
                Components = new List<SceneComponentContainer>
                {
                    new TransformComponent
                    {
                        Rotation = float3.Zero,
                        Scale = float3.One,
                        Translation = -allPoints[0].xyz
                    },
                    new ShaderEffectComponent
                    {
                        Effect = effect
                    }
                }
            };

            var maxIntensityVal = 1 << 12; // 12 bit

            for (int i = 0; i < allPointsSplitted.Count; i++)
            {
                var pointSplit = allPointsSplitted[i];
                var colorSplit = allColorsSplitted[i];

                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => pt.xyz).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = pointSplit.Select(pt => new float3(pt.w / maxIntensityVal, pt.w / maxIntensityVal, pt.w / maxIntensityVal)).ToArray(),
                    Colors = colorSplit.Select(pt => ColorToUInt((int)pt.r, (int)pt.g, (int)pt.b)).ToArray()
                };

                returnNodeContainer.Components.Add(currentMesh);
            }            

            return returnNodeContainer;
        }

        internal static ShaderEffect DepthPassEffect(float2 screenParams)
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

                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                
                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape}                
            });
        }

        internal static ShaderEffect StandardEffect(float2 screenParams, float2 clipPlaneDist)
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

                new EffectParameterDeclaration {Name = "ClipPlaneDist", Value = clipPlaneDist},
                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "Color", Value = SingleColor},

                new EffectParameterDeclaration {Name = "PointSize", Value = Size},
                new EffectParameterDeclaration {Name = "PointShape", Value = (int)Shape},
                new EffectParameterDeclaration {Name = "ColorMode", Value = (int)ColorMode},

                new EffectParameterDeclaration {Name = "Lighting", Value = (int)Lighting.EDL},
                new EffectParameterDeclaration{Name = "EDLStrength", Value = EdlStrength},
                new EffectParameterDeclaration{Name = "EDLNeighbourPixels", Value = EdlNoOfNeighbourPx},
                new EffectParameterDeclaration {Name = "SpecularStrength", Value = 0.5f},
                new EffectParameterDeclaration {Name = "Shininess", Value = 200f},                
                new EffectParameterDeclaration {Name = "SpecularColor", Value = new float4(1,1,1,1)},
                
            });
        }

        /// <summary>
        ///     Splits a long list to list chunks with given size
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="locations"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (var i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, M.Min(nSize, locations.Count - i));
            }
        }
    }
}
