using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Linq;
using FontMap = Fusee.Engine.Core.FontMap;

namespace Fusee.Examples.MuVista.Core
{
    public class GUI : SceneContainer
    {
        public GuiButton _btnZoomOut;
        public GuiButton _btnZoomIn;
        private GuiButton _btnFuseeLogo;

        public float2 _zoomInBtnPosition;
        public float2 _zoomOutBtnPosition;

        public float2 _miniMapBtnPosition;

        private FontMap _fontMap;



        public GUI(int width, int height, CanvasRenderMode canvasRenderMode, Transform mainCamTransform, Camera guiCam)
        {
            /*
            var vsTex = AssetStorage.Get<string>("texture.vert");
            var psTex = AssetStorage.Get<string>("texture.frag");
            var psText = AssetStorage.Get<string>("text.frag");
            var vsNineSlice = AssetStorage.Get<string>("nineSlice.vert");
            var psNineSlice = AssetStorage.Get<string>("nineSliceTile.frag");
            */

            var canvasWidth = width / 100f;
            var canvasHeight = height / 100f;

            var fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                //Set the albedo texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("FuseeText.png"), false, TextureFilterMode.Linear),
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(_btnFuseeLogo);

            _btnFuseeLogo = new GuiButton
            {
                Name = "Logo_Button"
            };
            _btnFuseeLogo.OnMouseEnter += BtnLogoEnter;
            _btnFuseeLogo.OnMouseExit += BtnLogoExit;
            _btnFuseeLogo.OnMouseDown += BtnLogoDown;

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            _fontMap = new FontMap(fontLato, 24);

            var text = TextNode.Create(
                "FUSEE Spherical Image Viewer",
                "ButtonText",
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                _fontMap,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            _btnZoomOut = new GuiButton
            {
                Name = "Zoom_Out_Button"
            };

            _btnZoomIn = new GuiButton
            {
                Name = "Zoom_In_Button"
            };


            _zoomInBtnPosition = new float2(canvasWidth - 1f, 1f);
            var zoomInNode = TextureNode.Create(
                "ZoomInLogo",
                new Texture(AssetStorage.Get<ImageData>("FuseePlusIcon.png")),
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownRight, _zoomInBtnPosition, canvasHeight, canvasWidth, new float2(0.5f, 0.5f)),
                float2.One
                );
            zoomInNode.Components.Add(_btnZoomIn);

            _zoomOutBtnPosition = new float2(canvasWidth - 1f, 0.4f);
            var zoomOutNode = TextureNode.Create(
                "ZoomOutLogo",
                new Texture(AssetStorage.Get<ImageData>("FuseeMinusIcon.png")),
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownRight, _zoomOutBtnPosition, canvasHeight, canvasWidth, new float2(0.5f, 0.5f)),
                float2.One
                );
            zoomOutNode.Components.Add(_btnZoomOut);


            var canvas = new CanvasNode(
                "Canvas",
                canvasRenderMode,
                new MinMaxRect
                {
                    Min = new float2(-canvasWidth / 2, -canvasHeight / 2f),
                    Max = new float2(canvasWidth / 2, canvasHeight / 2f)
                })
            {
                Children = new ChildList()
                {
                    //Simple Texture Node, contains the fusee logo.
                    fuseeLogo,
                    text,
                    zoomInNode,
                    zoomOutNode
                }
            };

            Children = new List<SceneNode>
                {
                    new SceneNode
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                    {
                        mainCamTransform,
                        guiCam
                    }
                },
                    
                //Add canvas.
                canvas
            };

        }

        public void BtnLogoEnter(CodeComponent sender)
        {
            var effect = this.Children.FindNodesWhereComponent(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        public void BtnLogoExit(CodeComponent sender)
        {
            var effect = this.Children.FindNodesWhereComponent(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        public void BtnLogoDown(CodeComponent sender)
        {
            //OpenLink("http://fusee3d.org");
        }
    }
}
