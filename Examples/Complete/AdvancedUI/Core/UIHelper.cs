using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Examples.AdvancedUI.Core
{
    internal static class UIHelper
    {
        internal static List<string> DummySegmentationClasses = new List<string>()
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

        internal static float2 AnnotationDim = new float2(3f, 0.5f);
        internal static float4 AnnotationBorderThickness = new float4(6, 0.5f, 0.5f, 0.5f);

        internal static string VsTex = AssetStorage.Get<string>("texture.vert");
        internal static string PsTex = AssetStorage.Get<string>("texture.frag");
        internal static string PsText = AssetStorage.Get<string>("text.frag");
        internal static string VsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
        internal static string PsNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");

        internal static Font FontRaleway = AssetStorage.Get<Font>("Raleway-Regular.ttf");
        internal static FontMap RalewayFontMap = new FontMap(FontRaleway, 24);

        internal static float alphaInv = 0.5f;
        internal static float alphaVis = 1f;

        internal static readonly float4 Green = new float4(0.14117f, 0.76078f, 0.48627f, alphaVis);
        internal static readonly float4 Yellow = new float4(0.89411f, 0.63137f, 0.31372f, alphaVis);
        internal static readonly float4 Gray = new float4(0.47843f, 0.52549f, 0.54901f, alphaVis);

        internal static readonly float4 White = new float4(1, 1, 1, 1);

        private static readonly Texture _frameToCheck = new Texture(AssetStorage.Get<ImageData>("frame_yellow.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _frameDiscarded = new Texture(AssetStorage.Get<ImageData>("frame_gray.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _frameRecognizedMLOrConfirmed = new Texture(AssetStorage.Get<ImageData>("frame_green.png"), false, TextureFilterMode.Linear);

        private static readonly Texture _iconToCheck = new Texture(AssetStorage.Get<ImageData>("lightbulb.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconDiscarded = new Texture(AssetStorage.Get<ImageData>("minus-oktagon.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconRecognizedML = new Texture(AssetStorage.Get<ImageData>("check-circle.png"), false, TextureFilterMode.Linear);
        private static readonly Texture _iconConfirmed = new Texture(AssetStorage.Get<ImageData>("check-circle_filled.png"), false, TextureFilterMode.Linear);

        internal static readonly SurfaceEffect GreenEffect = MakeEffect.FromDiffuseSpecular(Green, float4.Zero, 20f, 0f);
        internal static readonly SurfaceEffect YellowEffect = MakeEffect.FromDiffuseSpecular(Yellow, float4.Zero, 20f, 0f);
        internal static readonly SurfaceEffect GrayEffect = MakeEffect.FromDiffuseSpecular(Gray, float4.Zero, 20f, 0f);

        internal static readonly SurfaceEffect OccludedDummyEffect = MakeEffect.FromDiffuseSpecular(new float4(1, 1, 1, 1), float4.Zero, 20, 0);

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
            SceneNode container = new SceneNode
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
            TextureNode icon = new TextureNode(
                "icon",
                VsTex,
                PsTex,
                iconTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                UIElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.07f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(0.35f, 0.35f)),
                float2.One
            );

            TextNode annotationText = new TextNode(
                text,
                "annotation text",
                VsTex,
                PsText,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                UIElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.5f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(2.5f, 0.35f)),
                RalewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            TextureNode annotation = new TextureNode(
                "Annotation",
                VsNineSlice,
                PsNineSlice,
                frameTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                UIElementPosition.CalcOffsets(AnchorPos.DownDownLeft, pos, CanvasHeightInit, CanvasWidthInit,
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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0,0), CanvasHeightInit, CanvasWidthInit, circleDim),
                    },
                    new XForm
                    {
                        Name = "circle" + "_XForm",
                    },
                    MakeEffect.FromDiffuseSpecular(col, float4.Zero, 20, 0),
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        private static SceneNode CreateLine(MatColor color)
        {
            float4 col;

            switch (color)
            {
                default:
                case MatColor.White:
                    col = White;
                    break;

                case MatColor.Green:
                    col = Green;
                    break;

                case MatColor.Yellow:
                    col = Yellow;
                    break;

                case MatColor.Gray:
                    col = Gray;
                    break;
            }

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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XForm
                    {
                        Name = "line" + "_XForm",
                    },
                    MakeEffect.FromDiffuseSpecular(col, float4.Zero, 20, 0),
                }
            };
        }

        internal static void SetDiffuseAlphaInShaderEffect(this DefaultSurfaceEffect effect, float alpha)
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