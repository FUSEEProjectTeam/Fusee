using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using Fusee.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        private static readonly Texture _frameToCheck = new Texture(AssetStorage.Get<ImageData>("frame_yellow.png"));
        private static readonly Texture _frameDiscarded = new Texture(AssetStorage.Get<ImageData>("frame_gray.png"));
        private static readonly Texture _frameRecognizedMLOrConfirmed = new Texture(AssetStorage.Get<ImageData>("frame_green.png"));

        private static readonly Texture _iconToCheck = new Texture(AssetStorage.Get<ImageData>("lightbulb.png"));
        private static readonly Texture _iconDiscarded = new Texture(AssetStorage.Get<ImageData>("minus-oktagon.png"));
        private static readonly Texture _iconRecognizedML = new Texture(AssetStorage.Get<ImageData>("check-circle.png"));
        private static readonly Texture _iconConfirmed = new Texture(AssetStorage.Get<ImageData>("check-circle_filled.png"));

        internal static ShaderEffect GreenEffect;
        internal static ShaderEffect YellowEffect;
        internal static ShaderEffect GrayEffect;

        internal static ShaderEffect OccludedDummyEffect;

        static UIHelper()
        {
            CreateAsyncs();
        }

        private static async void CreateAsyncs()
        {
            GreenEffect = await ShaderCodeBuilder.MakeShaderEffect(Green, new float4(1, 1, 1, 1), 20, 0);
            YellowEffect = await ShaderCodeBuilder.MakeShaderEffect(Yellow, new float4(1, 1, 1, 1), 20, 0);
            GrayEffect = await ShaderCodeBuilder.MakeShaderEffect(Gray, new float4(1, 1, 1, 1), 20, 0);
            OccludedDummyEffect = await ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), 20, 0);
        }

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

        internal enum AnnotationKind
        {
            TO_CHECK,
            DISCARDED,
            RECOGNIZED_ML,
            CONFIRMED
        }

        internal static async void CreateAndAddCircleAnnotationAndLine(SceneNodeContainer parentUiElement, AnnotationKind annotationKind, float2 circleDim, float2 annotationPos, float textSize, float borderScaleFactor, string text)
        {
            var container = new SceneNodeContainer
            {
                Name = "Container"
            };

            switch (annotationKind)
            {
                case AnnotationKind.TO_CHECK:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.YELLOW));
                    container.Children.Add(CreateAnnotation(annotationPos, textSize, borderScaleFactor, text, _iconToCheck, _frameToCheck, textSizeModifier));
                    container.Children.Add(await CreateLine(MatColor.YELLOW));
                    break;

                case AnnotationKind.DISCARDED:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.GRAY));
                    container.Children.Add(CreateAnnotation(annotationPos, textSize, borderScaleFactor, text, _iconDiscarded, _frameDiscarded, textSizeModifier));
                    container.Children.Add(await CreateLine(MatColor.GRAY));
                    break;

                case AnnotationKind.RECOGNIZED_ML:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.GREEN));
                    container.Children.Add(CreateAnnotation(annotationPos, textSize, borderScaleFactor, text, _iconRecognizedML, _frameRecognizedMLOrConfirmed, textSizeModifier));
                    container.Children.Add(await CreateLine(MatColor.GREEN));
                    break;

                case AnnotationKind.CONFIRMED:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.GREEN));
                    container.Children.Add(CreateAnnotation(annotationPos, textSize, borderScaleFactor, text, _iconConfirmed, _frameRecognizedMLOrConfirmed, textSizeModifier));
                    container.Children.Add(await CreateLine(MatColor.GREEN));
                    break;
            }
            parentUiElement.Children.Add(container);
        }

        private static SceneNodeContainer CreateAnnotation(float2 pos, float borderScaleFactor, string text, Texture iconTex, Texture frameTex)
        {
            var icon = new TextureNodeContainer(
                "icon",
                VsTex,
                PsTex,
                iconTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(1, 1)
                },
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.07f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(0.35f, 0.35f))
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
                UIElementPosition.CalcOffsets(AnchorPos.STRETCH_ALL, new float2(0.5f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(2.5f, 0.35f)),
                RalewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black),
                HorizontalTextAlignment.CENTER,
                VerticalTextAlignment.CENTER);

            var annotation = new TextureNodeContainer(
                "Annotation",
                VsNineSlice,
                PsNineSlice,
                frameTex,
                new MinMaxRect
                {
                    Min = new float2(0, 0),
                    Max = new float2(0, 0)
                },
                UIElementPosition.CalcOffsets(AnchorPos.DOWN_DOWN_LEFT, pos, CanvasHeightInit, CanvasWidthInit,
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

        private static async Task<SceneNodeContainer> CreateCircle(float2 circleDim, MatColor color)
        {
            float4 col;

            string nameSuffix;

            switch (color)
            {
                default:
                case MatColor.WHITE:
                    col = White;
                    nameSuffix = "white";
                    break;

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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, circleDim),
                    },
                    new XFormComponent
                    {
                        Name = "circle" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = await ShaderCodeBuilder.MakeShaderEffect(col, new float4(1,1,1,1), 20, 0)
                    },
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        private static async Task<SceneNodeContainer> CreateLine(MatColor color)
        {
            float4 col;

            switch (color)
            {
                default:
                case MatColor.WHITE:
                    col = White;
                    break;

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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XFormComponent
                    {
                        Name = "line" + "_XForm",
                    },
                    new ShaderEffectComponent()
                    {
                        Effect = await ShaderCodeBuilder.MakeShaderEffect(col, new float4(1, 1, 1,1), 20, 0)
                    }
                }
            };
        }

        internal static ShaderEffect GetShaderEffectFromMatColor(MatColor col)
        {
            switch (col)
            {
                default:
                case MatColor.WHITE:
                    return OccludedDummyEffect;

                case MatColor.GREEN:
                    return GreenEffect;

                case MatColor.YELLOW:
                    return YellowEffect;

                case MatColor.GRAY:
                    return GrayEffect;
            }
        }

        internal static void SetDiffuseAlphaInShaderEffect(this ShaderEffect effect, float alpha)
        {
            var color = (float4)effect.GetEffectParam(UniformNameDeclarations.DiffuseColor);
            color.w = alpha;
            effect.SetEffectParam(UniformNameDeclarations.DiffuseColor, color);
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