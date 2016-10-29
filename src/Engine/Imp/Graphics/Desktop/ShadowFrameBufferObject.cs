using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using Fusee.Base.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Path = Fusee.Base.Common.Path;
using TextureTarget2d = OpenTK.Graphics.ES20.TextureTarget2d;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Creates a simple ShadowFrameBufferObject
    /// </summary>
    public class ShadowFrameBufferObject
    {
        // Create handles
        private int _shadowTextureHandle;
        private int _fboHandle;
        public ShaderProgramImp ShadowShader { private set; get; }

        private const string ShadowMapVs = @"

                attribute vec3 fuVertex;
                attribute vec2 fuUV;
                attribute vec3 fuNormal;

                uniform mat4 FUSEE_MVP;

                varying vec2 uv;

                void main()
                {
                    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                    uv = fuUV;
                }";

        private const string ShadowMapPs = @"
                #ifdef GL_ES
                    precision highp float
                #endif

            varying vec2 uv;

            uniform sampler2D texture;

            void main()
            {
                float Depth = texture2D(texture, uv).x;
                Depth = 1.0 - (1.0 - Depth) * 25.0;
                gl_FragColor = vec4(Depth);
            }";

        /// <summary>
        /// Creates a ShadowFrameBufferObject with a given TextureSize
        /// </summary>
        public ShadowFrameBufferObject(int width, int height, RenderContextImp rc)
        {
            Init(width, height);
            ShadowShader = (ShaderProgramImp) rc.CreateShader(ShadowMapVs, ShadowMapPs);

            //-------------------------
            /*
            GL.Viewport(0, 0, 256, 256);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0.0, 256.0, 0.0, 256.0, -1.0, 1.0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            //-------------------------
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            //-------------------------
            //**************************
            //RenderATriangle, {0.0, 0.0}, {256.0, 0.0}, {256.0, 256.0}
            //Read http://www.opengl.org/wiki/VBO_-_just_examples

            //-------------------------
            //pixels 0, 1, 2 should be white
            //pixel 4 should be black
            //----------------
            //Bind 0, which means render to back buffer
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); */

        }

        private void Init(int width, int height)
        {
            try
            {

                GL.GenTextures(1, out _shadowTextureHandle);
                GL.BindTexture(TextureTarget.Texture2D, _shadowTextureHandle);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent, width, height, 0,
                    PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);

                // test for GL Error here (might be unsupported format)

                // GL.BindTexture(TextureTarget.Texture2D, 0);
                // prevent feedback, reading and writing to the same image is a bad idea

                // test for GL Error here (might be unsupported format)

                // Create a FBO and attach the textures 
                GL.GenFramebuffers(1, out _fboHandle);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fboHandle);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment,
                    TextureTarget.Texture2D, _shadowTextureHandle, 0);

                // now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

                // Disable writes to the color buffer
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);

                // TODO: Add usage of shadowmap shader

            }
            catch (Exception e)
            {
                var error = GL.GetError();
                throw new Exception($"Error: {error}, Exception: {e}, FBO: {CheckFboStatus()}");
            }


        }

        /// <summary>
        /// Convenience method for framebufferbinding
        /// </summary>
        public void BindForWriting()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fboHandle);
        }

        /// <summary>
        /// Convenience method for binding the texture created by the framebuffer
        /// </summary>
        /// <param name="textureUnit"></param>
        public void BindForReading(TextureUnit textureUnit)
        {
            GL.ActiveTexture(textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, _shadowTextureHandle);
        }

        /// <summary>
        /// Checks and prints the status of the ShadowFrameBufferObject.
        /// </summary>
        /// <returns></returns>
        public bool CheckFboStatus()
        {
            switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Trace.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
                        return true;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Trace.WriteLine("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Trace.WriteLine("FBO: There are no attachments.");
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         Trace.WriteLine("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Trace.WriteLine("FBO: Attachments are of different size. All attachments must have the same width and height.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Trace.WriteLine("FBO: The color attachments have different format. All color attachments must have the same format.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Trace.WriteLine("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Trace.WriteLine("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Trace.WriteLine("FBO: This particular FBO configuration is not supported by the implementation.");
                        break;
                    }
                default:
                    {
                        Trace.WriteLine("FBO: Status unknown. (yes, this is really bad.)");
                        break;
                    }
            }
            return false;
        }

    }
}