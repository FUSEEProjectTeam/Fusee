using System;
using Fusee.Base.Common;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Math.Core;

namespace Fusee.Engine.GUI
{
    /// <summary>
    ///     The <see cref="GUIImage_Legacy" /> class provides functionality for loading and displaying 2D images.
    /// </summary>
    /// <remarks>
    ///     Images can be transparent *.png files, too.
    /// </remarks>
    public sealed class GUIImage_Legacy : GUIElement_Legacy
    {
        #region Private Fields

        private const string IMGPS = @"#version 300 es
                precision highp float;
  
  
            in vec2 vUV;
            in vec4 vColor;

            out vec4 fragColor;

            uniform sampler2D tex;

            void main(void) {
                if (vUV.x == -1.0)
                    fragColor = vColor;
                else
                    fragColor = texture(tex, vUV);
            }";

        private float4 _borderColor;
        private int _borderWidth;

        #endregion

        #region Public Fields

        /// <summary>
        ///     Gets or sets the width of the border.
        /// </summary>
        /// <value>
        ///     The width of the border.
        /// </value>
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                Dirty = true;
            }
        }

        /// <summary>
        ///     Gets or sets the color of the border.
        /// </summary>
        /// <value>
        ///     The color of the border.
        /// </value>
        public float4 BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Dirty = true;
            }
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIImage_Legacy" /> class.
        /// </summary>
        /// <param name="image">The image data containting the pixels.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="z">The z-index.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <remarks>
        ///     The z-index: lower values means further away. If two elements have the same z-index
        ///     then they are rendered according to their order in the <see cref="GUIHandler_Legacy" />.
        /// </remarks>
        public GUIImage_Legacy(IImageData image, int x, int y, int z, int width, int height)
            : base(String.Empty, null, x, y, z, width, height)
        {
            // settings
            ImgSrc = image;
            BorderWidth = 0;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GUIImage_Legacy" /> class.
        /// </summary>
        /// <param name="image">The image data containting the pixels.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public GUIImage_Legacy(IImageData image, int x, int y, int width, int height)
            : base(String.Empty, null, x, y, 0, width, height)
        {
            // settings
            ImgSrc = image;
            BorderWidth = 0;
        }

        protected override void CreateGUIShader()
        {
            GUIShader = new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = GUIVS,
                    PS = IMGPS,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        SourceBlend = Blend.SourceAlpha,
                        DestinationBlend = Blend.InverseSourceAlpha,
                        ZEnable = true
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "tex", Value = GUITexture},
                new EffectParameterDeclaration {Name = "guiXForm", Value = float4x4.Identity},
            });
        }


        protected internal override void AttachToContext(RenderContext rc)
        {
            if (RContext == rc) return;

            if (!ImgSrc.IsEmpty)
            {

                // (dd) old, currently no repeat option?
                //GUITexture = rc.CreateTexture(ImgSrc,
                //    M.IsPowerOfTwo(ImgSrc.Width) && M.IsPowerOfTwo(ImgSrc.Height));
                GUITexture = new Texture(ImgSrc);
                CreateGUIShader();
            }

            base.AttachToContext(rc);
        }

        protected override void CreateMesh()
        {
            SetRectangleMesh(BorderWidth, new float4(1, 1, 1, 1), BorderColor);

            var uvs = GUIMesh.UVs;

            uvs[0] = new float2(0, 0);
            uvs[1] = new float2(0, 1);
            uvs[2] = new float2(1, 0);
            uvs[3] = new float2(1, 1);

            if (BorderWidth > 0)
                for (var i = 4; i < 8; i++)
                    uvs[i] = new float2(-1, -1);

            GUIMesh.UVs = uvs;
        }
    }
}