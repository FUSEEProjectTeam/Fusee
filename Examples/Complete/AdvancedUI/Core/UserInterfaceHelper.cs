using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Examples.AdvancedUI.Core
{
    internal static class UserInterfaceHelper
    {
        internal static List<string> DummySegmentationClasses = new()
        {
            "orangutan",
            "banana",
            "monkey",
            "jungle",
            "coconut",
            "chimp",
            "gorilla",
            "lemur",
            "liana",
            "gibbon"
        };

        internal static float CanvasWidthInit;
        internal static float CanvasHeightInit;

        internal static float2 AnnotationDim = new(3f, 0.5f);
        internal static float4 AnnotationBorderThickness = new(6, 0.5f, 0.5f, 0.5f);

        internal static Font FontRaleway = AssetStorage.Get<Font>("Raleway-Regular.ttf");
        internal static FontMap RalewayFontMap = new(FontRaleway, 24);

        internal static float alphaInv = 0.5f;
        internal static float alphaVis = 1f;

        internal static readonly float4 Green = new float4(0.14117f, 0.76078f, 0.48627f, alphaVis).LinearColorFromSRgb();
        internal static readonly float4 Yellow = new float4(0.89411f, 0.63137f, 0.31372f, alphaVis).LinearColorFromSRgb();
        internal static readonly float4 Gray = new float4(0.47843f, 0.52549f, 0.54901f, alphaVis).LinearColorFromSRgb();

        internal static readonly float4 White = (float4)ColorUint.White;

        private static readonly Texture _frameToCheck = new(AssetStorage.Get<ImageData>("frame_yellow.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _frameDiscarded = new(AssetStorage.Get<ImageData>("frame_gray.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _frameRecognizedMLOrConfirmed = new(AssetStorage.Get<ImageData>("frame_green.png"), false, TextureFilterMode.Linear);

        private static readonly Texture _iconToCheck = new(AssetStorage.Get<ImageData>("lightbulb.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconDiscarded = new(AssetStorage.Get<ImageData>("minus-oktagon.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconRecognizedML = new(AssetStorage.Get<ImageData>("check-circle.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconConfirmed = new(AssetStorage.Get<ImageData>("check-circle_filled.png"), false, TextureFilterMode.Linear);

        internal static readonly SurfaceEffectBase GreenEffect = MakeEffect.FromDiffuseSpecular(Green, float4.Zero);
        internal static readonly SurfaceEffectBase YellowEffect = MakeEffect.FromDiffuseSpecular(Yellow, float4.Zero);
        internal static readonly SurfaceEffectBase GrayEffect = MakeEffect.FromDiffuseSpecular(Gray, float4.Zero);

        internal static readonly SurfaceEffectBase OccludedDummyEffect = MakeEffect.FromDiffuseSpecular((float4)ColorUint.White, float4.Zero);

        private static readonly float _circleThickness = 0.04f;
        internal static float LineThickness = 0.02f;

        public static float AnnotationDistToLeftOrRightEdge = 1;

        internal enum MatColor
        {
            Green,
            Yellow,
            Gray,
            White
        }

        internal enum AnnotationKind
        {
            ToCheck,
            Discarded,
            RecognizedML,
            Confirmed
        }

        internal static void CreateAndAddCircleAnnotationAndLine(SceneNode parentUiElement, AnnotationKind annotationKind, float2 circleDim, float2 annotationPos, float borderScaleFactor, string text)
        {
            SceneNode container = new()
            {
                Name = "Container"
            };

            switch (annotationKind)
            {
                case AnnotationKind.ToCheck:
                    container.Children.Add(CreateCircle(circleDim, MatColor.Yellow));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconToCheck, _frameToCheck));
                    container.Children.Add(CreateLine(MatColor.Yellow));
                    break;

                case AnnotationKind.Discarded:
                    container.Children.Add(CreateCircle(circleDim, MatColor.Gray));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconDiscarded, _frameDiscarded));
                    container.Children.Add(CreateLine(MatColor.Gray));
                    break;

                case AnnotationKind.RecognizedML:
                    container.Children.Add(CreateCircle(circleDim, MatColor.Green));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconRecognizedML, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(CreateLine(MatColor.Green));
                    break;

                case AnnotationKind.Confirmed:
                    container.Children.Add(CreateCircle(circleDim, MatColor.Green));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconConfirmed, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(CreateLine(MatColor.Green));
                    break;
            }
            parentUiElement.Children.Add(container);
        }

        private static SceneNode CreateAnnotation(float2 pos, float borderScaleFactor, string text, Texture iconTex, Texture frameTex)
        {
            TextureNode icon = new(
                "icon",
                iconTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                GuiElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.07f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(0.35f, 0.35f)),
                float2.One
            );

            TextNode annotationText = new(
                text,
                "annotation text",
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                GuiElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.5f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(2.5f, 0.35f)),
                RalewayFontMap,
                (float4)ColorUint.Black,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            TextureNode annotation = new(
                "Annotation",
                frameTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownLeft, pos, CanvasHeightInit, CanvasWidthInit,
                    AnnotationDim),
                new float2(1, 1),
                new float4(0.09f, 0.09f, 0.09f, 0.09f),
                AnnotationBorderThickness.x, AnnotationBorderThickness.y, AnnotationBorderThickness.z,
                AnnotationBorderThickness.w,
                borderScaleFactor

            );
            annotation.Children.Add(annotationText);
            annotation.Children.Add(icon);

            return annotation;
        }

        private static SceneNode CreateCircle(float2 circleDim, MatColor color)
        {
            float4 col;

            string nameSuffix;

            switch (color)
            {
                default:
                case MatColor.White:
                    col = White;
                    nameSuffix = "white";
                    break;

                case MatColor.Green:
                    col = Green;
                    nameSuffix = "green";
                    break;

                case MatColor.Yellow:
                    col = Yellow;
                    nameSuffix = "yellow";
                    break;

                case MatColor.Gray:
                    col = Gray;
                    nameSuffix = "gray";
                    break;
            }

            return new SceneNode
            {
                Name = "Circle_" + nameSuffix,
                Components = new List<SceneComponent>
                {
                    new RectTransform
                    {
                        Name = "circle" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = GuiElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0,0), CanvasHeightInit, CanvasWidthInit, circleDim),
                    },
                    new XForm
                    {
                        Name = "circle" + "_XForm",
                    },
                    MakeEffect.FromDiffuseSpecular(col, float4.Zero),
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        private static SceneNode CreateLine(MatColor color)
        {
            var col = color switch
            {
                MatColor.Green => Green,
                MatColor.Yellow => Yellow,
                MatColor.Gray => Gray,
                _ => White,
            };
            return new SceneNode()
            {
                Name = "line",
                Components = new List<SceneComponent>
                {
                    new RectTransform
                    {
                        Name = "line" + "_RectTransform",
                        Anchors = new MinMaxRect
                        {
                            Min = new float2(0.5f, 0.5f),
                            Max = new float2(0.5f, 0.5f)
                        },
                        Offsets = GuiElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XForm
                    {
                        Name = "line" + "_XForm",
                    },
                    MakeEffect.FromDiffuseSpecular(col, float4.Zero),
                }
            };
        }

        internal static void SetDiffuseAlphaInShaderEffect(this SurfaceEffect effect, float alpha)
        {
            float4 color = effect.SurfaceInput.Albedo;
            color.w = alpha;
            effect.SurfaceInput.Albedo = color;
        }

        internal static bool DoesAnnotationIntersectWithAnnotation(float2 firstAnnotation, float2 secondAnnotation, float2 intersectionBuffer)
        {
            return firstAnnotation.x + intersectionBuffer.x + AnnotationDim.x > secondAnnotation.x - intersectionBuffer.x &&
                   firstAnnotation.x - intersectionBuffer.x < secondAnnotation.x + intersectionBuffer.x + AnnotationDim.x &&
                   firstAnnotation.y + intersectionBuffer.y + AnnotationDim.y > secondAnnotation.y - intersectionBuffer.y &&
                   firstAnnotation.y - intersectionBuffer.y < secondAnnotation.y + AnnotationDim.y + intersectionBuffer.y;
        }
    }
}