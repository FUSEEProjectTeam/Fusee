using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Static helper class which conains methods to create predefined GUIs.s
    /// </summary>
    public static class FuseeGuiHelper
    {
        /// <summary>
        /// Creates a <see cref="SceneContainer"/> which conatins the default Fusee-UI (Logo and app title)
        /// </summary>
        /// <param name="rc">The <see cref="RenderCanvas"/>.</param>
        /// <param name="canvasRenderMode">The <see cref="CanvasRenderMode"/> which is used to render the GUI.</param>
        /// <param name="title">The app title.</param>
        /// <returns></returns>
        public static SceneContainer CreateDefaultGui(RenderCanvas rc, CanvasRenderMode canvasRenderMode, string title)
        {
            var canvasWidth = rc.Width / 100f;
            var canvasHeight = rc.Height / 100f;

            var btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };

            var guiFuseeLogo = new Texture(AssetStorage.Get<ImageData>("FuseeText.png"));
            var fuseeLogo = new TextureNode(
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
            var guiLatoBlack = new FontMap(fontLato, 24);

            var text = new TextNode(
                title,
                "AppTitle",
                GuiElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                GuiElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                (float4)ColorUint.Greenery,
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

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
                    text
                }
            };

            var scene = new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
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
    }
}