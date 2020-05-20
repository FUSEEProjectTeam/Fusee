﻿using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Used when positioning UI elements. Each entry corresponds to a commonly used anchor point setup.
    /// </summary>
    public enum AnchorPos
    {
        /// <summary>
        /// Anchors to the lower left corner of the parent element.
        /// </summary>
        DownDownLeft,     //Min = Max = 0,0

        /// <summary>
        /// Anchors to the lower right corner of the parent element.
        /// </summary>
        DownDownRight,    //Min = Max = 1,0

        /// <summary>
        /// Anchors to the upper left corner of the parent element.
        /// </summary>
        TopTopLeft,       //Min = Max = 0,1

        /// <summary>
        /// Anchors to the upper right corner of the parent element.
        /// </summary>
        TopTopRight,      //Min = Max = 1,1

        /// <summary>
        /// Stretches across all of the parent element.
        /// </summary>
        StretchAll,        //Min = 0, 0 and Max = 1, 1

        /// <summary>
        /// Stretches horizontally, but keeps its size vertically.
        /// </summary>
        StretchHorizontal, //Min = 0,0 and Max = 1,0

        /// <summary>
        /// Stretches vertically, but keeps its size horizontally.
        /// </summary>
        StretchVertical,   //Min = 0,0 and Max = 0,1

        /// <summary>
        /// Anchors to the middle of the parent element.
        /// </summary>
        Middle              //Min = Max = 0.5, 0.5
    }

    /// <summary>
    /// Contains convenience functions to position a UI element on its parent element.
    /// </summary>
    public static class UIElementPosition
    {
        /// <summary>
        /// Sets the anchor position in percent as a <see cref="MinMaxRect"/> depending on its <see cref="AnchorPos"/>
        /// </summary>
        /// <param name="anchorPos">The anchor point of the UI element.</param>
        /// <returns>The <see cref="MinMaxRect"/> containing the anchor position in percent.</returns>
        public static MinMaxRect GetAnchors(AnchorPos anchorPos)
        {
            switch (anchorPos)
            {
                case AnchorPos.DownDownLeft:
                    return new MinMaxRect()
                    {
                        Min = new float2(0, 0),
                        Max = new float2(0, 0)
                    };
                case AnchorPos.DownDownRight:
                    return new MinMaxRect()
                    {
                        Min = new float2(1, 0),
                        Max = new float2(1, 0)
                    };
                case AnchorPos.TopTopLeft:
                    return new MinMaxRect()
                    {
                        Min = new float2(0, 1),
                        Max = new float2(0, 1)
                    };
                case AnchorPos.TopTopRight:
                    return new MinMaxRect()
                    {
                        Min = new float2(1, 1),
                        Max = new float2(1, 1)
                    };
                case AnchorPos.StretchAll:
                    return new MinMaxRect()
                    {
                        Min = new float2(0, 0),
                        Max = new float2(1, 1)
                    };
                case AnchorPos.StretchHorizontal:
                    return new MinMaxRect()
                    {
                        Min = new float2(0, 0),
                        Max = new float2(1, 0)
                    };
                case AnchorPos.StretchVertical:
                    return new MinMaxRect()
                    {
                        Min = new float2(0, 0),
                        Max = new float2(0, 1)
                    };
                default:
                case AnchorPos.Middle:
                    return new MinMaxRect()
                    {
                        Min = new float2(0.5f, 0.5f),
                        Max = new float2(0.5f, 0.5f)
                    };
            }
        }

        /// <summary>
        /// Calculates the offset between an element and their parent element and therefore its size.
        /// </summary>
        /// <param name="anchorPos">The anchor point of the element.</param>
        /// <param name="posOnParent">The position on the parent element.</param>
        /// <param name="parentHeight">The height of the parent element.</param>
        /// <param name="parentWidth">The width of the parent element.</param>
        /// <param name="guiElementDim">The dimensions of the element along the x and y axis.</param>
        /// <returns></returns>
        public static MinMaxRect CalcOffsets(AnchorPos anchorPos, float2 posOnParent, float parentHeight, float parentWidth, float2 guiElementDim)
        {
            switch (anchorPos)
            {
                default:
                case AnchorPos.Middle:
                    var middle = new float2(parentWidth / 2f, parentHeight / 2f);
                    return new MinMaxRect
                    {
                        Min = posOnParent - middle,
                        Max = posOnParent - middle + guiElementDim
                    };

                case AnchorPos.StretchAll:
                    return new MinMaxRect
                    {
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), -(parentHeight - posOnParent.y - guiElementDim.y))
                    };
                case AnchorPos.StretchHorizontal:
                    return new MinMaxRect
                    {
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(-(parentWidth - (posOnParent.x + guiElementDim.x)), posOnParent.y + guiElementDim.y)
                    };
                case AnchorPos.StretchVertical:
                    return new MinMaxRect
                    {
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(posOnParent.x + guiElementDim.x, -(parentHeight - (posOnParent.y + guiElementDim.y)))
                    };
                case AnchorPos.DownDownLeft:
                    return new MinMaxRect
                    {
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(posOnParent.x + guiElementDim.x, posOnParent.y + guiElementDim.y)
                    };
                case AnchorPos.DownDownRight:
                    return new MinMaxRect
                    {
                        Min = new float2(-(parentWidth - posOnParent.x), posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), posOnParent.y + guiElementDim.y)
                    };
                case AnchorPos.TopTopLeft:
                    return new MinMaxRect
                    {
                        Min = new float2(posOnParent.x, -(parentHeight - posOnParent.y)),
                        Max = new float2(posOnParent.x + guiElementDim.x, -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
                case AnchorPos.TopTopRight:
                    return new MinMaxRect
                    {
                        Min = new float2(-(parentWidth - posOnParent.x), -(parentHeight - posOnParent.y)),
                        Max = new float2(-(parentWidth - guiElementDim.x - posOnParent.x), -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
            }
        }
    }

    /// <summary>
    /// Building block to create suitable hierarchies for creating a UI canvas.
    /// </summary>
    public class CanvasNode : SceneNode
    {
        /// <summary>
        /// Creates a SceneNode with the proper components and children for rendering a canvas.
        /// </summary>
        /// <param name="name">The name of the canvas.</param>
        /// <param name="canvasRenderMode">Choose in which mode you want to render this canvas.</param>
        /// <param name="size">The size of the canvas.</param>
        /// By default Scale in SCREEN mode is set to 0.1.
        public CanvasNode(string name, CanvasRenderMode canvasRenderMode, MinMaxRect size)
        {
            Name = name;
            Components = new List<SceneComponent>
            {
                new CanvasTransform(canvasRenderMode)
                {
                    Name = name + "_CanvasTransform",
                    Size = size
                },
                new XForm
                {
                    Name =  name + "_Canvas_XForm"
                }
            };
        }
    }


    /// <summary>
    /// Building block to create suitable hierarchies for using textures in the UI.
    /// </summary>
    public class TextureNode : SceneNode
    {
        /// <summary>
        /// Creates a SceneNode with the proper components and children for rendering a nine sliced texture.
        /// By default the border thickness is calculated relative to a unit plane. For a thicker border set the border thickness to the desired value, 2 means a twice as thick border.
        /// </summary>
        /// <param name="name">Name of the SceneNode.</param>
        /// <param name="vs">The vertex shader you want to use.</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// /<param name="tex">Diffuse texture.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
        /// <param name="tiles">Defines the tiling of the inner rectangle of the texture. Use float2.one if you do not desire tiling.</param>
        /// <param name="borders">Defines the nine tiles of the texture. Order: left, right, top, bottom. Value is measured in percent from the respective edge of texture.</param>
        /// <param name="borderThicknessBottom">Border thickness for the bottom border.</param>
        /// <param name="borderScaleFactor">Default value is 1. Set this to scale the border thickness if you use canvas render mode SCREEN.</param>
        /// <param name="borderThicknessLeft">Border thickness for the left border.</param>
        /// <param name="borderThicknessRight">Border thickness for the right border.</param>
        /// <param name="borderThicknessTop">Border thickness for the top border.</param>
        /// <returns></returns>
        public TextureNode(string name, string vs, string ps, Texture tex, MinMaxRect anchors,
            MinMaxRect offsets, float2 tiles, float4 borders, float borderThicknessLeft = 1, float borderThicknessRight = 1, float borderThicknessTop = 1, float borderThicknessBottom = 1, float borderScaleFactor = 1)
        {
            var borderThickness = new float4(borderThicknessLeft, borderThicknessRight, borderThicknessTop,
                borderThicknessBottom);
            Name = name;
            Components = new List<SceneComponent>
            {
                new RectTransform
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets
                },
                new XForm
                {
                    Name = name + "_XForm",
                },
               new ShaderEffect(new[]
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
                            new EffectParameterDeclaration {Name = "borderThickness", Value = borderThickness * borderScaleFactor},
                            new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_V", Value = float4x4.Identity},
                            new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity}
                        }),
                new NineSlicePlane()
            };
        }

        /// <summary>
        /// Creates a SceneNode with the proper components and children for rendering a nine sliced texture.
        /// </summary>
        /// <param name="name">Name of the SceneNode.</param>
        /// <param name="vs">The vertex shader you want to use.</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// /<param name="tex">Diffuse texture.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
        /// <returns></returns>
        public TextureNode(string name, string vs, string ps, Texture tex, MinMaxRect anchors,
            MinMaxRect offsets)
        {
            Name = name;
            Components = new List<SceneComponent>
            {
                new RectTransform
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets
                },
                new XForm
                {
                    Name = name + "_XForm",
                },
               new ShaderEffect(new[]
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
                        }),
                new Core.Plane()
            };
        }
    }

    /// <summary>
    /// Creates a SceneNode with the proper components and children for rendering text in the UI.
    /// </summary>
    public class TextNode : SceneNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextNode"/> class.
        /// </summary>
        /// <param name="text">The text you want to display.</param>
        /// <param name="name">The name of the SceneNode.</param>
        /// <param name="vs">The vertex shader you want to use..</param>
        /// <param name="ps">The pixel shader you want to use.</param>
        /// <param name="anchors">Anchors for the mesh. Influences the scaling of the object if the enclosing canvas is resized.</param>
        /// <param name="offsets">The offsets.</param>
        /// <param name="fontMap">Offsets for the mesh. Defines the position of the object relative to its enclosing UI element.</param>
        /// <param name="color">The color.</param>
        /// <param name="horizontalAlignment">The <see cref="HorizontalTextAlignment"/> defines the text's placement along the enclosing <see cref="MinMaxRect"/>'s x-axis.</param>
        /// <param name="verticalTextAlignment">The <see cref="HorizontalTextAlignment"/> defines the text's placement along the enclosing <see cref="MinMaxRect"/>'s y-axis.</param>
        public TextNode(string text, string name, string vs, string ps, MinMaxRect anchors, MinMaxRect offsets,
            FontMap fontMap, float4 color, HorizontalTextAlignment horizontalAlignment = HorizontalTextAlignment.Left, VerticalTextAlignment verticalTextAlignment = VerticalTextAlignment.Top)
        {
            var textMesh = new GUIText(fontMap, text, horizontalAlignment)
            {
                Name = name + "textMesh"
            };

            var xFormText = new XFormText
            {
                Width = textMesh.Width,
                Height = textMesh.Height,
                HorizontalAlignment = textMesh.HorizontalAlignment,
                VerticalAlignment = verticalTextAlignment
            };

            Name = name;
            Components = new List<SceneComponent>
            {
                new RectTransform
                {
                    Name = name + "_RectTransform",
                    Anchors = anchors,
                    Offsets = offsets
                },
                new XForm
                {
                    Name = name + "_XForm",
                }
            };

            Children = new ChildList()
            {
                new SceneNode()
                {
                    Components = new List<SceneComponent>()
                    {
                       xFormText,
                       new ShaderEffect(new[]
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
                                }),
                        textMesh
                     }
                }
            };
        }
    }
}