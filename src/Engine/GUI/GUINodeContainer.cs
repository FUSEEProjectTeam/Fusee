using System.Collections.Generic;
using System.Linq;
using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;
using Fusee.Xene;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Building block to create suitable hierarchies for using textures.
    /// </summary>
    public class TextureNodeContainer : SceneNodeContainer
    {
        /// <summary>
        /// Adds a CodeComponent to the TextureNodeContainer. 
        /// </summary>
        /// <param name="comp">The CodeComponent.</param>
        public void AddCodeComponent(CodeComponent comp)
        {
            Children.First().AddComponent(comp);
        }

        /// <summary>
        /// Creates a SceneNodeContainer with the proper components and children for rendering a nine sliced texture.
        /// </summary>
        /// <param name="name">Name of the SceneNodeContainer.</param>
        /// <param name="vs">The vertex shader you want to use.</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// /<param name="tex">Diffuse texture.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing canvas.</param>
        /// <param name="tiles">Defines the tiling of the inner rectangle of the texture. Use float2.one if you do not desire tiling.</param>
        /// <param name="borders">Defines the nine tiles of the texture. Order: left, right, top, bottom. Value is measured in percent from the respective edge of texture.</param>
        /// <param name="borderthickness">By default the border thickness is calculated relative to a unit plane. If you scale your object you may want to choose a higher value. 2 means a twice as thick border.</param>
        /// <returns></returns>
        public TextureNodeContainer(string name, string vs, string ps, Texture tex, MinMaxRect anchors, MinMaxRect offsets, float2 tiles, float4 borders, float borderthickness = 1)
        {
            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new RectTransformComponent
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets

                }
            };
            Children = new List<SceneNodeContainer>
            {
                new SceneNodeContainer
                {
                    Name = name + "_XForm",
                    Components = new List<SceneComponentContainer>
                    {
                        new XFormComponent
                        {
                            Name = name + "_XForm",
                        },
                        new ShaderEffectComponent
                        {
                            Effect = new ShaderEffect(new[]
                                {
                                    new EffectPassDeclaration
                                    {
                                        VS = vs,
                                        PS = ps,
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
            };
        }

        /// <summary>
        /// Creates a SceneNodeContainer with the proper components and children for rendering a nine sliced texture.
        /// </summary>
        /// <param name="name">Name of the SceneNodeContainer.</param>
        /// <param name="vs">The vertex shader you want to use.</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// /<param name="tex">Diffuse texture.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing canvas.</param>
        /// <returns></returns>
        public TextureNodeContainer(string name, string vs, string ps, Texture tex, MinMaxRect anchors,
            MinMaxRect offsets)
        {
            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new RectTransformComponent
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets

                }
            };
            Children = new List<SceneNodeContainer>
            {
                new SceneNodeContainer
                {
                    Name = name + "_XForm",
                    Components = new List<SceneComponentContainer>
                    {
                        new XFormComponent
                        {
                            Name = name + "_XForm",
                        },
                        new ShaderEffectComponent
                        {
                            Effect = new ShaderEffect(new[]
                                {
                                    new EffectPassDeclaration
                                    {
                                        VS = vs,
                                        PS = ps,
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
                                    new EffectParameterDeclaration {Name = "DiffuseMix", Value = 1f},
                                    new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                    new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                                })
                        },
                        new Plane()
                    }
                }
            };
        }
    }

    public class TextNodeContainer : SceneNodeContainer
    {
        public TextNodeContainer(string name, string vs, string ps, MinMaxRect anchors, MinMaxRect offsets, FontMap fontMap, float4 color)
        {

            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new RectTransformComponent
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets

                }
            };
            Children = new List<SceneNodeContainer>
                {
                    new SceneNodeContainer
                    {
                        Name = name + "_XForm",
                        Components = new List<SceneComponentContainer>
                        {
                            new XFormTextComponent
                            {
                                Name = name + "_XForm",
                            },
                            new ShaderEffectComponent
                            {
                                Effect = new ShaderEffect(new[]
                                    {
                                        new EffectPassDeclaration
                                        {
                                            VS = vs,
                                            PS = ps,
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
                                            Value = new Texture(fontMap.Image)
                                        },
                                        new EffectParameterDeclaration
                                            {Name = "DiffuseColor", Value = color},
                                        new EffectParameterDeclaration {Name = "DiffuseMix", Value = 0.0f},
                                        new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                                        new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                                    })
                            },
                            new GUIText(fontMap, "Hallo !")
                            {
                                Name = name + "textMesh"
                            }
                        }
                    }
            };
        }
    }
}
