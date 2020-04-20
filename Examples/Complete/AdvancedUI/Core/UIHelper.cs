using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.GUI;
using Fusee.Math.Core;
using System.Collections.Generic;
using static Fusee.Examples.AdvancedUI.Core.UIHelper;
using System.Threading.Tasks;
using Fusee.Serialization.V1;
using HorizontalTextAlignment = Fusee.Engine.Core.Scene.HorizontalTextAlignment;
using VerticalTextAlignment = Fusee.Engine.Core.Scene.VerticalTextAlignment;
using System.Runtime.CompilerServices;

namespace Fusee.Examples.AdvancedUI.Core
{ 
    internal class UIHelper
    {
        internal List<string> DummySegmentationClasses = new List<string>()
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

        internal string VsTex;
        internal string PsTex;
        internal string VsNineSlice;
        internal string PsNineSlice;

        internal Font FontRaleway;
        internal FontMap RalewayFontMap;

        internal static float alphaInv = 0.5f;
        internal static float alphaVis = 1f;

        internal readonly float4 Green = new float4(0.14117f, 0.76078f, 0.48627f, alphaVis);
        internal readonly float4 Yellow = new float4(0.89411f, 0.63137f, 0.31372f, alphaVis);
        internal readonly float4 Gray = new float4(0.47843f, 0.52549f, 0.54901f, alphaVis);

        internal readonly float4 White = new float4(1, 1, 1, 1);

        private Texture _frameToCheck;
        private Texture _frameDiscarded;
        private Texture _frameRecognizedMLOrConfirmed;

        private Texture _iconToCheck;
        private Texture _iconDiscarded;
        private Texture _iconRecognizedML;
        private Texture _iconConfirmed;

        internal ShaderEffect GreenEffect;
        internal ShaderEffect YellowEffect;
        internal ShaderEffect GrayEffect;

        internal static ShaderEffect OccludedDummyEffect;

        private static UIHelper _instance;
        
        internal static async Task<UIHelper> Initialize()
        {
            if (_instance != null) return _instance;

            _instance = new UIHelper();
            await _instance.CreateAsyncs();

            return _instance;
        }
        
        private UIHelper()
        {

        }

        private async Task<bool> CreateAsyncs()
        {
            VsTex = await AssetStorage.GetAsync<string>("texture.vert");
            PsTex = await AssetStorage.GetAsync<string>("texture.frag");
            VsNineSlice = await AssetStorage.GetAsync<string>("nineSlice.vert");
            PsNineSlice = await AssetStorage.GetAsync<string>("nineSliceTile.frag");

            GreenEffect = ShaderCodeBuilder.MakeShaderEffect(Green, new float4(1, 1, 1, 1), 20, 0);
            YellowEffect = ShaderCodeBuilder.MakeShaderEffect(Yellow, new float4(1, 1, 1, 1), 20, 0);
            GrayEffect = ShaderCodeBuilder.MakeShaderEffect(Gray, new float4(1, 1, 1, 1), 20, 0);
            OccludedDummyEffect = ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), 20, 0);

            FontRaleway = await AssetStorage.GetAsync<Font>("Raleway-Regular.ttf");
            RalewayFontMap = new FontMap(FontRaleway, 24);

            _frameToCheck = new Texture(await AssetStorage.GetAsync<ImageData>("frame_yellow.png"));
            _frameDiscarded = new Texture(await AssetStorage.GetAsync<ImageData>("frame_gray.png"));
            _frameRecognizedMLOrConfirmed = new Texture(await AssetStorage.GetAsync<ImageData>("frame_green.png"));

            _iconToCheck = new Texture(await AssetStorage.GetAsync<ImageData>("lightbulb.png"));
            _iconDiscarded = new Texture(await AssetStorage.GetAsync<ImageData>("minus-oktagon.png"));
            _iconRecognizedML = new Texture(await AssetStorage.GetAsync<ImageData>("check-circle.png"));
            _iconConfirmed = new Texture(await AssetStorage.GetAsync<ImageData>("check-circle_filled.png"));

            return true;
        }

        private static readonly float _circleThickness = 0.04f;
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

        internal void CreateAndAddCircleAnnotationAndLine(SceneNode parentUiElement, AnnotationKind annotationKind, float2 circleDim, float2 annotationPos, float borderScaleFactor, string text)
        {

            var container = new SceneNode
            {
                Name = "Container"
            };

            switch (annotationKind)
            {
                case AnnotationKind.TO_CHECK:
                    container.Children.Add(CreateCircle(circleDim, MatColor.YELLOW));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconToCheck, _frameToCheck));
                    container.Children.Add(CreateLine(MatColor.YELLOW));
                    break;

                case AnnotationKind.DISCARDED:
                    container.Children.Add(CreateCircle(circleDim, MatColor.GRAY));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconDiscarded, _frameDiscarded));
                    container.Children.Add(CreateLine(MatColor.GRAY));
                    break;

                case AnnotationKind.RECOGNIZED_ML:
                    container.Children.Add(CreateCircle(circleDim, MatColor.GREEN));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconRecognizedML, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(CreateLine(MatColor.GREEN));
                    break;

                case AnnotationKind.CONFIRMED:
                    container.Children.Add(CreateCircle(circleDim, MatColor.GREEN));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconConfirmed, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(CreateLine(MatColor.GREEN));
                    break;
            }
            parentUiElement.Children.Add(container);
        }

        private SceneNode CreateAnnotation(float2 pos, float borderScaleFactor, string text, Texture iconTex, Texture frameTex)
        {
            var icon = new TextureNode(
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

            var annotationText = new TextNode(
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

            var annotation = new TextureNode(
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

        private SceneNode CreateCircle(float2 circleDim, MatColor color)
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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, circleDim),
                    },
                    new XForm
                    {
                        Name = "circle" + "_XForm",
                    },
                    ShaderCodeBuilder.MakeShaderEffect(col, new float4(1,1,1,1), 20, 0),
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        private SceneNode CreateLine(MatColor color)
        {
            var col = color switch
            {
                MatColor.GREEN => Green,
                MatColor.YELLOW => Yellow,
                MatColor.GRAY => Gray,
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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.MIDDLE, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XForm
                    {
                        Name = "line" + "_XForm",
                    },
                    ShaderCodeBuilder.MakeShaderEffect(col, new float4(1, 1, 1,1), 20, 0)
                }
            };
        }

        internal ShaderEffect GetShaderEffectFromMatColor(MatColor col)
        {
            return col switch
            {
                MatColor.GREEN => GreenEffect,
                MatColor.YELLOW => YellowEffect,
                MatColor.GRAY => GrayEffect,
                _ => OccludedDummyEffect,
            };
        }

        internal bool DoesAnnotationIntersectWithAnnotation(float2 firstAnnotation, float2 secondAnnotation, float2 intersectionBuffer)
        {
            return firstAnnotation.x + intersectionBuffer.x + AnnotationDim.x > secondAnnotation.x - intersectionBuffer.x &&
                   firstAnnotation.x - intersectionBuffer.x < secondAnnotation.x + intersectionBuffer.x + AnnotationDim.x &&
                   firstAnnotation.y + intersectionBuffer.y + AnnotationDim.y > secondAnnotation.y - intersectionBuffer.y &&
                   firstAnnotation.y - intersectionBuffer.y < secondAnnotation.y + AnnotationDim.y + intersectionBuffer.y;
        }
    }

    internal static class UIHelperExtensions
    {
        internal static void SetDiffuseAlphaInShaderEffect(this ShaderEffect effect, float alpha)
        {
            var color = (float4)effect.GetEffectParam(UniformNameDeclarations.AlbedoColor);
            color.w = alpha;
            effect.SetEffectParam(UniformNameDeclarations.AlbedoColor, color);
        }
    }
}