using System.Collections.Generic;
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

        internal static readonly float3 Green = new float3(0.14117f, 0.76078f, 0.48627f);
        internal static readonly float3 Yellow = new float3(0.89411f, 0.63137f, 0.31372f);
        internal static readonly float3 Gray = new float3(0.47843f, 0.52549f, 0.54901f);
        internal static readonly float3 White = new float3(1, 1, 1);

        internal static readonly ShaderEffect GreenEffect = ShaderCodeBuilder.MakeShaderEffect(Green, new float3(1, 1, 1), 20, 0);
        internal static readonly ShaderEffect YellowEffect = ShaderCodeBuilder.MakeShaderEffect(Yellow, new float3(1, 1, 1), 20, 0);
        internal static readonly ShaderEffect GrayEffect = ShaderCodeBuilder.MakeShaderEffect(Gray, new float3(1, 1, 1), 20, 0);

        internal static readonly ShaderEffect OccludedDummyEffect = ShaderCodeBuilder.MakeShaderEffect(new float3(1, 1, 1), new float3(1, 1, 1), 20, 0);
        
        private static float _circleThickness = 0.04f;
        internal static float LineThickness = 0.02f;

        public static float AnnotationDistToLeftOrRightEdge = 1;

        internal enum MatColor
        {
            GREEN,
            YELLOW,
            GRAY,
            WHITE
        }

        internal enum AnnotationPos
        {
            LEFT,
            RIGHT
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

            string nameSuffix;

            switch (color)
            {
                default:
                case MatColor.GREEN:
                    col = Green;
                    nameSuffix = "green";
                    break;
                case MatColor.YELLOW:
                    col = Yellow;
                    nameSuffix = "yellow";
                    break;
                case MatColor.GRAY:
                    col = Gray;
                    nameSuffix = "gray";
                    break;
            }

            return new SceneNodeContainer
            {
                Name = "Circle_" + nameSuffix,
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

        internal static SceneNodeContainer CreateLine(MatColor color)
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


            return new SceneNodeContainer()
            {
                Name = "line",
                Components = new List<SceneComponentContainer>
                {
                    new RectTransformComponent
                    {
                        Name = "line" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XFormComponent
                    {
                        Name = "line" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = ShaderCodeBuilder.MakeShaderEffect(col, new float3(1, 1, 1), 20, 0)
                    }
                }
            };
        }

        internal static ShaderEffect GetShaderEffectFromMatColor(MatColor col)
        {
            switch (col)
            {
                default:
                case MatColor.GREEN:
                    return GreenEffect;
                    
                case MatColor.YELLOW:
                    return YellowEffect;
                    
                case MatColor.GRAY:
                    return GrayEffect;

                case MatColor.WHITE:
                    return OccludedDummyEffect;

            }
        }

        internal static bool DoesAnnotationIntersectWithAnnotation(float2 thisAnnotationCanvasPos, float2 annotationCanvasPos)
        {
            //AnnotationCanvasPos equals lower left corner.

            var thisMin = thisAnnotationCanvasPos.y;
            var thisMax = thisAnnotationCanvasPos.y + AnnotationDim.y;

            var min = annotationCanvasPos.y;
            var max = annotationCanvasPos.y + AnnotationDim.y;

            if (((thisMax>min && thisMax < max) && annotationCanvasPos.x == thisAnnotationCanvasPos.x) ||
                ((thisMin < max && thisMin > min) && annotationCanvasPos.x == thisAnnotationCanvasPos.x))
                return true;
            return false;

            //if ((thisAnnotationCanvasPos.y + AnnotationDim.y > annotationCanvasPos.y ||
            //    thisAnnotationCanvasPos.y < annotationCanvasPos.y + AnnotationDim.y) &&
            //    annotationCanvasPos.x >= thisAnnotationCanvasPos.x &&
            //    annotationCanvasPos.x <= thisAnnotationCanvasPos.x + AnnotationDim.x)
            //    return true;
            //return false;
        }
    }
}
