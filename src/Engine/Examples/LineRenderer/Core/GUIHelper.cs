using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Examples.LineRenderer.Core
{
    internal static class GuiHelper
    {
        internal const float CanvasWidthInit = 16;
        internal const float CanvasHeightInit = 9;

        internal static float2 AnnotationDim = new float2(3f, 0.5f);
        internal static float4 AnnotationBorderThickness = new float4(6, 0.5f, 0.5f, 0.5f);

        internal static string VsTex = AssetStorage.Get<string>("texture.vert");
        internal static string PsTex = AssetStorage.Get<string>("texture.frag");
        internal static string VsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
        internal static string PsNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");

        internal static Font FontRaleway = AssetStorage.Get<Font>("Raleway-Regular.ttf");
        internal static FontMap RalewayFontMap = new FontMap(FontRaleway, 12);

        private static readonly float3 Green = new float3(0.14117f, 0.76078f, 0.48627f);
        private static readonly float3 Yellow = new float3(0.89411f, 0.63137f, 0.31372f);
        private static readonly float3 Gray = new float3(0.47843f, 0.52549f, 0.54901f);

        private static float _circleThickness = 0.04f;

        internal enum MatColor
        {
            GREEN,
            YELLOW,
            GRAY
        }


        public enum AnchorPos
        {
            DOWN_DOWN_LEFT,     //Min = Max = 0,0
            DOWN_DOWN_RIGHT,    //Min = Max = 0,1
            TOP_TOP_LEFT,       //Min = Max = 0,1
            TOP_TOP_RIGHT,      //Min = Max = 1,1
            STRETCH_ALL,        //Min 0, 0 and Max 1, 1
            MIDDLE              //Min = Max = 0.5, 0.5
        }

        public static MinMaxRect CalcOffsets(AnchorPos anchorPos, float2 posOnParent, float parentHeight, float parentWidth, float2 guiElementDim)
        {
            switch (anchorPos)
            {
                default:
                case GuiHelper.AnchorPos.MIDDLE:
                    var middle = new float2(parentWidth / 2f, parentHeight / 2f);
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0.5,0.5 and Max 0.5,0.5!!!
                        Min = posOnParent - middle,
                        Max = posOnParent - middle + guiElementDim
                    };

                case GuiHelper.AnchorPos.STRETCH_ALL:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,0 and Max 1,1!!!
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), -(parentHeight - posOnParent.y - guiElementDim.y))
                    };
                case GuiHelper.AnchorPos.DOWN_DOWN_LEFT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,0 and Max 0,0!!!
                        Min = new float2(posOnParent.x, posOnParent.y),
                        Max = new float2(posOnParent.x + guiElementDim.x, posOnParent.y + guiElementDim.y)
                    };
                case GuiHelper.AnchorPos.DOWN_DOWN_RIGHT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 1,0 and Max 1,0!!!
                        Min = new float2(-(parentWidth - posOnParent.x), posOnParent.y),
                        Max = new float2(-(parentWidth - posOnParent.x - guiElementDim.x), posOnParent.y + guiElementDim.y)
                    };
                case GuiHelper.AnchorPos.TOP_TOP_LEFT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 0,1 and Max 0,1!!!
                        Min = new float2(posOnParent.x, -(parentHeight - posOnParent.y)),
                        Max = new float2(posOnParent.x + guiElementDim.x, -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
                case GuiHelper.AnchorPos.TOP_TOP_RIGHT:
                    return new MinMaxRect
                    {
                        //only for the anchors Min 1,1 and Max 1,1!!!
                        Min = new float2(-(parentWidth - posOnParent.x), -(parentHeight - posOnParent.y)),
                        Max = new float2(-(parentWidth - guiElementDim.x - posOnParent.x), -(parentHeight - guiElementDim.y - posOnParent.y))
                    };
            }
        }

        internal static SceneNodeContainer CreateAnnotation(float2 pos, float textSize, float borderScaleFactor, string text, string iconFileName, string frameFileName, float textSizeAdaptor = 1)
        {
            var icon = new TextureNodeContainer(
                "icon",
                VsTex,
                PsTex,
                new Texture(AssetStorage.Get<ImageData>(iconFileName)),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(0.35f, 0.35f))
            );

            var annotationText = new TextNodeContainer(
                text,
                "annotation text",
                VsTex,
                PsTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(2.5f, 0.35f)),
                RalewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black), textSize * textSizeAdaptor);


            return new TextureNodeContainer(
                "Annotation",
                VsNineSlice,
                PsNineSlice,
                new Texture(AssetStorage.Get<ImageData>(frameFileName)),
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, pos, CanvasHeightInit, CanvasWidthInit, AnnotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                AnnotationBorderThickness.x, AnnotationBorderThickness.y, AnnotationBorderThickness.z, AnnotationBorderThickness.w,
                borderScaleFactor

            )
            {
                Children = new List<SceneNodeContainer>
                {
                    annotationText,
                    icon
                }
            };
        }

        internal static SceneNodeContainer CreateCircle(float2 circleDim, MatColor color)
        {
            float3 col;

            switch (color)
            {
                default:
                case MatColor.GREEN:
                    col = Green;
                    break;
                case MatColor.YELLOW:
                    col = Yellow;
                    break;
                case MatColor.GRAY:
                    col = Gray;
                    break;
            }

           return new SceneNodeContainer
            {
                Name = "Circle",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, circleDim),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(col, new float3(1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        //TODO: scale points from rect range to -0.5 0.5 in Line constructor
        //internal static float3 CalcLinePoint(CanvasRenderMode crm, float2 canvasPos, float canvasWidth, float canvasHeight, float canvasScaleFactor, float2 resizeScaleFactor)
        //{
        //    var scaledPos = canvasPos;
        //    var pos = new float3(scaledPos.x / canvasWidth, scaledPos.y / canvasHeight, 0); //range 0,1
        //    pos.x = (pos.x * 2 - 1) / 2; //range -1,1
        //    pos.y = (pos.y * 2 - 1) / 2; //range -1,1

        //    if (crm == CanvasRenderMode.SCREEN)
        //    {
        //        //negate scaling in scene renderer
        //        pos.x /= canvasWidth / canvasScaleFactor;
        //        pos.y /= canvasHeight / canvasScaleFactor;
        //    }
        //    else
        //    {
        //        //negate scaling in scene renderer
        //        pos.x /= canvasWidth;
        //        pos.y /= canvasHeight;
        //    }

        //    return pos;
        //}
    }
}
