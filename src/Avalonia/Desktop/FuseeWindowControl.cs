using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Threading;
using Fusee.Math.Core;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Code based in parts upon https://github.com/SamboyCoding/OpenTKAvalonia, but heavily modified for our use-case(s)
/// </summary>
namespace Fusee.Avalonia.Desktop
{
    /// <summary>
    /// Wrapper to expose GetProcAddress from Avalonia in a manner that OpenTK can consume.
    /// </summary>
    internal class AvaloniaTkContext : IBindingsContext
    {
        private readonly GlInterface _glInterface;
        public AvaloniaTkContext(GlInterface glInterface) => (_glInterface) = (glInterface);
        public IntPtr GetProcAddress(string procName) => _glInterface.GetProcAddress(procName);
    }


    /// <summary>
    /// This class can be bound inside any Avalonia *.axaml.cs files and represents the window where Fusee will render to
    /// </summary>
    public class FuseeWindowControl : OpenGlControlBase
    {
        public PixelSize GetPixelSize()
        {
            var value = _pxSizeMethod?.Invoke(this, null);
            if (value is not null)
            {
                return (PixelSize)value;
            }

            throw new("Unable to retrieve PixelSize() from framebuffer");
        }

        /// <summary>
        /// OnRender is called once a frame to draw to the control.
        /// You can do anything you want here, but make sure you undo any configuration changes after, or you may get weirdness with other controls.
        /// </summary>
        public EventHandler<EventArgs>? OnRender;

        /// <summary>
        /// OnInit is called once when the control is first created.
        /// At this point, the GL bindings are initialized and you can invoke GL functions.
        /// You could use this function to load and compile shaders, load textures, allocate buffers, etc.
        /// </summary>
        public EventHandler<EventArgs>? OnInit;

        /// <summary>
        /// OnTeardown is called once when the control is destroyed.
        /// Though GL bindings are still valid, as OpenTK provides no way to clear them, you should not invoke GL functions after this function finishes executing.
        /// At best, they will do nothing, at worst, something could go wrong.
        /// You should use this function as a last chance to clean up any GL resources you have allocated - delete buffers, vertex arrays, programs, and textures.
        /// </summary>
        public EventHandler<EventArgs>? OnTeardown;

        /// <summary>
        /// Called whenever this control is resized
        /// For viewport adaption
        /// </summary>
        public EventHandler<PixelSize>? OnResize;

        private readonly MethodInfo _pxSizeMethod = typeof(OpenGlControlBase).GetMethod("GetPixelSize", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new("Unable to find PixelSize() method");
        private readonly FieldInfo _depthBufferField = typeof(OpenGlControlBase).GetField("_depthBuffer", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new("Unable to find _depthBuffer field");

        private AvaloniaTkContext? _avaloniaTkContext;

        /// <summary>
        /// Initialize a new Fusee window
        /// </summary>
        public FuseeWindowControl()
        {
            LayoutUpdated += (sender, args) =>
            {
               OnResize?.Invoke(this, GetPixelSize());
            };
        }


        protected sealed override void OnOpenGlRender(GlInterface gl, int fb)
        {
            //Set up the aspect ratio so shapes aren't stretched.
            //As avalonia is using this opengl instance internally to render the entire window, stuff gets messy, so we workaround that here
            //to provide a good experience to the user.
            var oldViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, oldViewport);
            GL.Viewport(0, 0, (int)Bounds.Width, (int)Bounds.Height);

            //Tell our subclass to render
            OnRender?.Invoke(this, EventArgs.Empty);

            //Reset viewport after our fix above
            GL.Viewport(oldViewport[0], oldViewport[1], oldViewport[2], oldViewport[3]);

            //Workaround for avalonia issue #6488, set active texture back to 0
            GL.ActiveTexture(TextureUnit.Texture0);

            //Schedule next UI update with avalonia
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Background);
        }

        protected sealed override void OnOpenGlInit(GlInterface gl, int fb)
        {
            //Initialize the OpenTK<->Avalonia Bridge
            _avaloniaTkContext = new(gl);
            GL.LoadBindings(_avaloniaTkContext);

            //Invoke the subclass' init function
            OnInit?.Invoke(this, EventArgs.Empty);
        }

        //Simply call the subclass' teardown function
        protected sealed override void OnOpenGlDeinit(GlInterface gl, int fb)
        {
            OnTeardown?.Invoke(this, EventArgs.Empty);

            //Workaround an Avalonia issue where the depth buffer ID is not cleared
            //which causes depth problems (see https://github.com/SamboyCoding/OpenTKAvalonia/issues/1)
            _depthBufferField.SetValue(this, 0);
        }
    }
}
