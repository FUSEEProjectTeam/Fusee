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

            GreenEffect = await ShaderCodeBuilder.MakeShaderEffect(Green, new float4(1, 1, 1, 1), 20, 0);
            YellowEffect = await ShaderCodeBuilder.MakeShaderEffect(Yellow, new float4(1, 1, 1, 1), 20, 0);
            GrayEffect = await ShaderCodeBuilder.MakeShaderEffect(Gray, new float4(1, 1, 1, 1), 20, 0);
            OccludedDummyEffect = await ShaderCodeBuilder.MakeShaderEffect(new float4(1, 1, 1, 1), new float4(1, 1, 1, 1), 20, 0);

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

        internal async void CreateAndAddCircleAnnotationAndLine(SceneNode parentUiElement, AnnotationKind annotationKind, float2 circleDim, float2 annotationPos, float borderScaleFactor, string text)
        {

            var container = new SceneNode
            {
                Name = "Container"
            };

            switch (annotationKind)
            {
                case AnnotationKind.ToCheck:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.Yellow));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconToCheck, _frameToCheck));
                    container.Children.Add(await CreateLine(MatColor.Yellow));
                    break;

                case AnnotationKind.Discarded:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.Gray));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconDiscarded, _frameDiscarded));
                    container.Children.Add(await CreateLine(MatColor.Gray));
                    break;

                case AnnotationKind.RecognizedML:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.Green));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconRecognizedML, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(await CreateLine(MatColor.Green));
                    break;

                case AnnotationKind.Confirmed:
                    container.Children.Add(await CreateCircle(circleDim, MatColor.Green));
                    container.Children.Add(CreateAnnotation(annotationPos, borderScaleFactor, text, _iconConfirmed, _frameRecognizedMLOrConfirmed));
                    container.Children.Add(await CreateLine(MatColor.Green));
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
                UIElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.07f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(0.35f, 0.35f))
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
                UIElementPosition.CalcOffsets(AnchorPos.StretchAll, new float2(0.5f, 0.07f), AnnotationDim.y, AnnotationDim.x, new float2(2.5f, 0.35f)),
                RalewayFontMap,
                ColorUint.Tofloat4(ColorUint.Black),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

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

        private async Task<SceneNode> CreateCircle(float2 circleDim, MatColor color)
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
                    await ShaderCodeBuilder.MakeShaderEffect(col, new float4(1,1,1,1), 20, 0),
                    new Circle(false, 30,100,_circleThickness)
                }
            };
        }

        private async Task<SceneNode> CreateLine(MatColor color)
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
                        Offsets = UIElementPosition.CalcOffsets(AnchorPos.Middle, new float2(0,0), CanvasHeightInit, CanvasWidthInit, new float2(CanvasWidthInit,CanvasHeightInit)),
                    },
                    new XForm
                    {
                        Name = "line" + "_XForm",
                    },
                    await ShaderCodeBuilder.MakeShaderEffect(col, new float4(1, 1, 1,1), 20, 0)
                }
            };
        }

        internal ShaderEffect GetShaderEffectFromMatColor(MatColor col)
        {
            return col switch
            {
                MatColor.Green => GreenEffect,
                MatColor.Yellow => YellowEffect,
                MatColor.Gray => GrayEffect,
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