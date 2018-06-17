using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Base.Core;

namespace Fusee.Engine.Core.GUI
{
    public class GUINodes
    {
        /// <summary>
        /// Creates a SceneNodeContainer with the propper components and children for rendering a nine sliced texture.
        /// </summary>
        /// <param name="name">Name of the SceneNodeContainer.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaleing of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing canvas.</param>
        /// <param name="tex">Diffuse texture</param>
        /// <param name="tiles">Defines the tiling of the inner rectangle of the texture. Use float2.one if you do not desire tiling.</param>
        /// <param name="borders">Defines the nine tiles of the texture. Order: left, right, top, bottom. Value is messured in percent from the respective edge of texture.</param>
        /// <param name="borderthickness">By default the border thickness is calculated relative to a unit plane. If you scale your object you may want to choose a higher value. 2 means a twice as thick border.</param>
        /// <returns></returns>
        public static  SceneNodeContainer NineSliceNode(string name, MinMaxRect anchors, MinMaxRect offsets, Texture tex, float2 tiles, float4 borders, float borderthickness = 1f)
        {          
            return new SceneNodeContainer
            {
                Name = name,
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = name+"_RectTransform",
                        Anchors = anchors,
                        Offsets = offsets

                    }
                },
                Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = name+"_XForm",
                        Components = new List<SceneComponentContainer>
                        {
                            new XFormComponent
                            {
                                Name = name+"_XForm",
                            },
                            new ShaderEffectComponent
                            {
                                Effect = new ShaderEffect(new[]
                                    {
                                        new EffectPassDeclaration
                                        {
                                            VS = AssetStorage.Get<string>("nineSlice.vert"),
                                            PS = AssetStorage.Get<string>("nineSliceTile.frag"),
                                            StateSet = new RenderStateSet
                                            {
                                                AlphaBlendEnable = true,
                                                SourceBlend = Blend.SourceAlpha,
                                                DestinationBlend = Blend.InverseSourceAlpha,
                                                BlendOperation = BlendOperation.Add,
                                                ZEnable = false
                                            }
                                        }
                                    },
                                    new[]
                                    {
                                        new EffectParameterDeclaration
                                        {
                                            Name = "DiffuseTexture",
                                            Value = tex
                                        },
                                        new EffectParameterDeclaration {Name = "DiffuseColor", Value = float4.One},
                                        new EffectParameterDeclaration {Name = "Tile", Value = tiles},
                                        new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                        new EffectParameterDeclaration
                                        {
                                            Name = "borders",
                                            Value = borders
                                        },
                                        new EffectParameterDeclaration {Name = "borderThickness", Value = borderthickness},
                                        new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                        new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                                        new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                                        new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                                    })
                            },
                            new NineSlicePlane()
                        }
                    }
                }
            };
        }
    }
}
