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
        public static SceneNodeContainer FromLAZ(string fileName, ShaderEffect effect)
        {
            var lazReader = new LASReader(fileName);
            var allPoints = lazReader.Points.Select(pt => new float4((float)pt.X, (float)pt.Z, (float)pt.Y, pt.intensity)).ToList();

            var allMeshes = new List<Mesh>();

            var maxVertCount = ushort.MaxValue - 1;

            var allPointsSplitted = SplitList(allPoints, maxVertCount);
           
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

            foreach (var pointSplit in allPointsSplitted)
            {
                var currentMesh = new Mesh
                {
                    Vertices = pointSplit.Select(pt => pt.xyz).ToArray(),
                    Triangles = Enumerable.Range(0, pointSplit.Count).Select(num => (ushort)num).ToArray(),
                    MeshType = (int)OpenGLPrimitiveType.POINT,
                    Normals = pointSplit.Select(pt => new float3(pt.w / maxIntensityVal, pt.w / maxIntensityVal, pt.w / maxIntensityVal)).ToArray()
                };

                returnNodeContainer.Components.Add(currentMesh);
            }

            return returnNodeContainer;
        }

        internal static ShaderEffect StandardEffect(float2 screenParams)
        {
            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = AssetStorage.Get<string>("PointVertexShader.vert"),
                    PS = AssetStorage.Get<string>("PointPixelShader.frag"),
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
                new EffectParameterDeclaration {Name = "PointSize", Value = 100},
                new EffectParameterDeclaration {Name = "ScreenParams", Value = screenParams},

                new EffectParameterDeclaration {Name = "PointShape", Value = (int)PointShape.PARABOLID},
                new EffectParameterDeclaration {Name = "ColorMode", Value = (int)ColorMode.DEPTH},
                new EffectParameterDeclaration {Name = "Lighting", Value = (int)Lighting.UNLIT},
                new EffectParameterDeclaration {Name = "SpecularStrength", Value = 0.5f},
                new EffectParameterDeclaration {Name = "Shininess", Value = 200f},
                new EffectParameterDeclaration {Name = "Color", Value = new float4(0,0,1,1)},
                new EffectParameterDeclaration {Name = "SecularColor", Value = new float4(1,1,1,1)},
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
