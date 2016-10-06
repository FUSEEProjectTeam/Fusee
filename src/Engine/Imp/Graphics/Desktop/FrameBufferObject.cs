using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Path = Fusee.Base.Common.Path;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    /// <summary>
    /// Creates a simple FrameBufferObject
    /// </summary>
    public class FrameBufferObject : GameWindow
    {
        private const int FboWidth = 512;
        private const int FboHeight = 512;

        /// <summary>
        /// Creates a FrameBufferObject with a given TextureSize
        /// </summary>
        public FrameBufferObject()
        {
            // Create Color Texture
            uint depthRenderbuffer;
            uint colorTexture;
            uint fboHandle;


            GL.GenTextures(1, out colorTexture);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, FboWidth, FboHeight, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // test for GL Error here (might be unsupported format)

            GL.BindTexture(TextureTarget.Texture2D, 0);
                // prevent feedback, reading and writing to the same image is a bad idea

            // Create Depth Renderbuffer
            GL.Ext.GenRenderbuffers(1, out depthRenderbuffer);
            GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthRenderbuffer);
            GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage) All.DepthComponent32,
                FboWidth, FboHeight);

            // test for GL Error here (might be unsupported format)

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out fboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
                TextureTarget.Texture2D, colorTexture, 0);
            GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt,
                RenderbufferTarget.RenderbufferExt, depthRenderbuffer);

            // now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

            // since there's only 1 Color buffer attached this is not explicitly required
            GL.DrawBuffer((DrawBufferMode) FramebufferAttachment.ColorAttachment0Ext);

            GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
            GL.Viewport(0, 0, FboWidth, FboHeight);

            // render whatever your heart desires, when done ...

            GL.PopAttrib(); // restores GL.Viewport() parameters
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        /// <summary>
        /// Checks and prints the status of the FrameBufferObject.
        /// </summary>
        /// <returns></returns>
        public bool CheckFboStatus()
        {
            switch (GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
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