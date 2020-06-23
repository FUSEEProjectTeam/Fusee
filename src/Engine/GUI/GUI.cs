using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Math.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fusee.Engine.GUI
{
    /// <summary>
    /// Helper method for GUI creation
    /// </summary>
    public static class GUI
    {

        /// <summary>
        /// Creates the default Fusee GUI with the top left logo and a description text at the bottom
        /// </summary>
        /// <param name="width">Canvas width</param>
        /// <param name="height">Canvas height</param>
        /// <param name="descriptionText">The description text e.g.: "FUSEE Simple Example"</param>
        /// <param name="canvasRenderMode"></param>
        /// <param name="btnLogoEnter"></param>
        /// <param name="btnLogoExit"></param>
        /// <param name="btnLogoDown"></param>
        /// <returns>awaitable Task<SceneContainer></returns>
        public static async Task<SceneContainer> CreateDefaultGui(int width, int height, string descriptionText, CanvasRenderMode canvasRenderMode,
            CodeComponent.InteractionHandler btnLogoEnter,
            CodeComponent.InteractionHandler btnLogoExit,
            CodeComponent.InteractionHandler btnLogoDown
            )
        {
            string[] shader = await AssetStorage.GetAssetsAsync<string>(new string[] { "texture.vert", "texture.frag" }).ConfigureAwait(false);
            string vsTex = shader[0];
            string psTex = shader[1];

            float canvasWidth = width / 100f;
            float canvasHeight = height / 100f;

            GUIButton btnFuseeLogo = new GUIButton
            {
                Name = "Canvas_Button"
            };
            btnFuseeLogo.OnMouseEnter += btnLogoEnter;
            btnFuseeLogo.OnMouseExit += btnLogoExit;
            btnFuseeLogo.OnMouseDown += btnLogoDown;

            Texture guiFuseeLogo = new Texture(await AssetStorage.GetAsync<ImageData>("FuseeText.png").ConfigureAwait(false));
            TextureNode fuseeLogo = new TextureNode(
                "fuseeLogo",
                vsTex,
                psTex,
                //Set the albedo texture you want to use.
                guiFuseeLogo,
                //Define anchor points. They are given in percent, seen from the lower left corner, respectively to the width/height of the parent.
                //In this setup the element will stretch horizontally but stay the same vertically if the parent element is scaled.
                UIElementPosition.GetAnchors(AnchorPos.TopTopLeft),
                //Define Offset and therefor the size of the element.
                UIElementPosition.CalcOffsets(AnchorPos.TopTopLeft, new float2(0, canvasHeight - 0.5f), canvasHeight, canvasWidth, new float2(1.75f, 0.5f))
                );
            fuseeLogo.AddComponent(btnFuseeLogo);

            Font fontLato = await AssetStorage.GetAsync<Font>("Lato-Black.ttf").ConfigureAwait(false);
            FontMap guiLatoBlack = new FontMap(fontLato, 24);

            TextNode text = new TextNode(
                descriptionText,
                "ButtonText",
                vsTex,
                psTex,
                UIElementPosition.GetAnchors(AnchorPos.StretchHorizontal),
                UIElementPosition.CalcOffsets(AnchorPos.StretchHorizontal, new float2(canvasWidth / 2 - 4, 0), canvasHeight, canvasWidth, new float2(8, 1)),
                guiLatoBlack,
                ColorUint.Tofloat4(ColorUint.Greenery),
                HorizontalTextAlignment.Center,
                VerticalTextAlignment.Center);

            CanvasNode canvas = new CanvasNode(
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

            return new SceneContainer
            {
                Children = new List<SceneNode>
                {
                    //Add canvas.
                    canvas
                }
            };
        }


    }
}
