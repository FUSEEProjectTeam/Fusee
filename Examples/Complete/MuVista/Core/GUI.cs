﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Gui;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
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

        public GUI(RenderCanvas rc, int width, int height, CanvasRenderMode canvasRenderMode, Transform mainCamTransform, Camera guiCam, out GuiButton zoomOut, out GuiButton zoomIn)
        {
            var canvasWidth = width / 100f;
            var canvasHeight = height / 100f;

            var fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                //Set the albedo texture you want to use.
                new Texture(AssetStorage.Get<ImageData>("FuseeText.png")),
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
            Action<CodeComponent> btnClickedAction = (btnFuseeLogo) => { BtnLogoDown(rc); };  
            Action<CodeComponent> btnExitAction = (btnFuseeLogo) => { BtnLogoExit(this); };
            Action<CodeComponent> btnEnterAction = (btnFuseeLogo) => { BtnLogoEnter(this); };
            _btnFuseeLogo.OnMouseEnter += btnEnterAction.Invoke;
            _btnFuseeLogo.OnMouseExit += btnExitAction.Invoke;
            _btnFuseeLogo.OnMouseDown += btnClickedAction.Invoke;

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
            /*
            _btnZoomOut = new GuiButton
            {
                Name = "Zoom_Out_Button"
            };

            _btnZoomIn = new GuiButton
            {
                Name = "Zoom_In_Button"
            };*/

            zoomOut = new GuiButton
            {
                Name = "Zoom_Out_Button"
            };

            zoomIn = new GuiButton
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
            zoomInNode.Components.Add(zoomIn);

            _zoomOutBtnPosition = new float2(canvasWidth - 1f, 0.4f);
            var zoomOutNode = TextureNode.Create(
                "ZoomOutLogo",
                new Texture(AssetStorage.Get<ImageData>("FuseeMinusIcon.png")),
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownRight, _zoomOutBtnPosition, canvasHeight, canvasWidth, new float2(0.5f, 0.5f)),
                float2.One
                );
            zoomOutNode.Components.Add(zoomOut);


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
                new SceneNode()
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, 0),
                                Rotation = float3.Zero,
                                Scale = float3.One
                            },
                            new Camera(canvasRenderMode == CanvasRenderMode.Screen ? ProjectionMethod.Orthographic : ProjectionMethod.Perspective, 0.01f, 500, M.PiOver4)
                            {
                                Active = true,
                                ClearColor = false
                            }
                        }
                    },
                    
                //Add canvas.
                canvas
            };

        }

        /*
        public static SceneContainer CreateDefaultUi(RenderCanvas rc, int width, int height, string title, out GuiButton zoomOut, out GuiButton zoomIn)
        {
            var canvasWidth = width / 100f;
            var canvasHeight = height / 100f;

            var btnFuseeLogo = new GuiButton
            {
                Name = "Canvas_Button"
            };

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = TextureNode.Create(
                "fuseeLogo",
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                GuiElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                GuiElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f)),
                float2.One
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            var fontLato = AssetStorage.Get<Font>("Lato-Black.ttf");
            var guiLatoBlack = new FontMap(fontLato, 18);

            var text = TextNode.Create(
                title,
                "AppTitle",
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            zoomOut = new GuiButton
            {
                Name = "Zoom_Out_Button"
            };

            zoomIn = new GuiButton
            {
                Name = "Zoom_In_Button"
            };


            var zoomInBtnPosition = new float2(canvasWidth - 1f, 1f);
            var zoomInNode = TextureNode.Create(
                "ZoomInLogo",
                new Texture(AssetStorage.Get<ImageData>("FuseePlusIcon.png")),
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownRight, zoomInBtnPosition, canvasHeight, canvasWidth, new float2(0.5f, 0.5f)),
                float2.One
                );
            zoomInNode.Components.Add(zoomIn);

            var zoomOutBtnPosition = new float2(canvasWidth - 1f, 0.4f);
            var zoomOutNode = TextureNode.Create(
                "ZoomOutLogo",
                new Texture(AssetStorage.Get<ImageData>("FuseeMinusIcon.png")),
                GuiElementPosition.GetAnchors(AnchorPos.DownDownRight),
                GuiElementPosition.CalcOffsets(AnchorPos.DownDownRight, zoomOutBtnPosition, canvasHeight, canvasWidth, new float2(0.5f, 0.5f)),
                float2.One
                );
            zoomOutNode.Components.Add(zoomOut);

            var canvas = new CanvasNode(
                "Canvas",
                CanvasRenderMode.Screen,
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

            var scene = new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    new SceneNode()
                    {
                        Name = "GuiCam",
                        Components = new List<SceneComponent>()
                        {
                            new Transform()
                            {
                                Translation = new float3(0, 0, 0),
                                Rotation = float3.Zero,
                                Scale = float3.One
                            },
                            new Camera(ProjectionMethod.Orthographic, 0.01f, 500, M.PiOver4)
                            {
                                Active = true,
                                ClearColor = false
                            }
                        }
                    },
                    canvas
                }
            };

            Action<CodeComponent> btnClickedAction = (btnFuseeLogo) => { DefaultGuiBtnClicked(rc); };
            Action<CodeComponent> btnExitAction = (btnFuseeLogo) => { DefaultGuiBtnExit(scene); };
            Action<CodeComponent> btnEnterAction = (btnFuseeLogo) => { DefaultGuiBtnEnter(scene); };
            btnFuseeLogo.OnMouseEnter += btnEnterAction.Invoke;
            btnFuseeLogo.OnMouseExit += btnExitAction.Invoke;
            btnFuseeLogo.OnMouseDown += btnClickedAction.Invoke;

            return scene;
        }
        */

        private static void BtnLogoEnter(SceneContainer guiContainer)
        {
            var effect = guiContainer.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        private static void BtnLogoExit(SceneContainer guiContainer)
        {
            var effect = guiContainer.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        private static void BtnLogoDown(RenderCanvas rc)
        {
            rc.OpenLink("http://fusee3d.org");
        }
        
        /*
        private static void DefaultGuiBtnEnter(SceneContainer guiContainer)
        {
            var effect = guiContainer.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, (float4)ColorUint.Black);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 0.8f);
        }

        private static void DefaultGuiBtnExit(SceneContainer guiContainer)
        {
            var effect = guiContainer.Children.FindNodes(node => node.Name == "fuseeLogo").First().GetComponent<Effect>();
            effect.SetFxParam(UniformNameDeclarations.Albedo, float4.One);
            effect.SetFxParam(UniformNameDeclarations.AlbedoMix, 1f);
        }

        private static void DefaultGuiBtnClicked(RenderCanvas rc)
        {
            rc.OpenLink("http://fusee3d.org");
        }
        */
    }
}
