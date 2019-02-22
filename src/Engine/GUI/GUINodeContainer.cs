using System.Collections.Generic;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Building block to create suitable hierarchies for creating a UI canvas.
    /// </summary>
    public class CanvasNodeContainer : SceneNodeContainer
    {
        /// <summary>
        /// Creates a SceneNodeContainer with the proper components and children for rendering a canvas.
        /// </summary>
        /// <param name="name">The name of the canvas.</param>
        /// <param name="canvasRenderMode">Choose in which mode you want to render this canvas.</param>
        /// <param name="size">The size of the canvas.</param>
        /// <param name="scale">Scale factor for the user given offsets that define the sizes if the canvas' child elements. This becomes important when rendering in SCREEN mode.
        /// By default Scale in SCREEN mode is set to 0.1.</param>
        public CanvasNodeContainer(string name, CanvasRenderMode canvasRenderMode, MinMaxRect size, float scale = 0.1f)
        {
            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new CanvasTransformComponent(canvasRenderMode, scale)
                {
                    Name = name + "_CanvasTransform",
                    Size = size
                },
                new XFormComponent
                {
                    Name =  name + "_Canvas_XForm"
                }
            };
        }
    }
    
    
    /// <summary>
    /// Building block to create suitable hierarchies for using textures in the UI.
    /// </summary>
    public class TextureNodeContainer : SceneNodeContainer
    {
        /// <summary>
        /// Creates a SceneNodeContainer with the proper components and children for rendering a nine sliced texture.
        /// </summary>
        /// <param name="name">Name of the SceneNodeContainer.</param>
        /// <param name="vs">The vertex shader you want to use.</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// /<param name="tex">Diffuse texture.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
        /// <param name="tiles">Defines the tiling of the inner rectangle of the texture. Use float2.one if you do not desire tiling.</param>
        /// <param name="borders">Defines the nine tiles of the texture. Order: left, right, top, bottom. Value is measured in percent from the respective edge of texture.</param>
        /// <param name="borderthickness">By default the border thickness is calculated relative to a unit plane. If you scale your object you may want to choose a higher value. 2 means a twice as thick border.</param>
        /// <param name="borderScaleFactor">Default value is 1. Set this to scale the border thickness if you use canvas render mode SCREEN.</param>
        /// <returns></returns>
        public TextureNodeContainer(string name, string vs, string ps, Texture tex, MinMaxRect anchors,
            MinMaxRect offsets, float2 tiles, float4 borders, float borderthicknessLeft = 1, float borderthicknessRight = 1, float borderthicknessTop = 1, float borderthicknessBottom = 1, float borderScaleFactor = 1)
        {
            var borderthickness = new float4(borderthicknessLeft, borderthicknessRight, borderthicknessTop,
                borderthicknessBottom);
            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new RectTransformComponent
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets
                },
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
                            new EffectParameterDeclaration {Name = "borderThickness", Value = borderThickness},
                            new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                        })
                },
                new NineSlicePlane()
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
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
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
                },
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
            };
        }
    }

    /// <summary>
    /// Creates a SceneNodeContainer with the proper components and children for rendering text in the UI.
    /// </summary>
    public class TextNodeContainer : SceneNodeContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextNodeContainer"/> class.
        /// </summary>
        /// <param name="text">The text you want to disply.</param>
        /// <param name="name">The name of the SceneNodeContainer.</param>
        /// <param name="vs">The vertex shader you want to use..</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">The offsets.</param>
        /// <param name="fontMap">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
        /// <param name="color">The color.</param>
        /// <param name="textScaleFactor">By default a text has the with of 1 fusee unit. Set this to adapt the text size.</param>
        public TextNodeContainer(string text, string name, string vs, string ps, MinMaxRect anchors, MinMaxRect offsets,
            FontMap fontMap, float4 color, float textScaleFactor = 1)
        {
            Name = name;
            Components = new List<SceneComponentContainer>
            {
                new RectTransformComponent
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets
                },
                new XFormComponent
                {
                    Name = name + "_XForm",
                }
            };

            Children = new List<SceneNodeContainer>()
            {
                new SceneNodeContainer()
                {
                    Components = new List<SceneComponentContainer>()
                    {
                        new XFormTextComponent(textScaleFactor),
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
                        new GUIText(fontMap, text)
                        {
                            Name = name + "textMesh"
                        }
                    }
                }
            };

        }
    }
}