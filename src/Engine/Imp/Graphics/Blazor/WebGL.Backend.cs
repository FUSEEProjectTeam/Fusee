using Fusee.Base.Imp.Blazor;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;


namespace Fusee.Engine.Imp.Graphics.Blazor
{
#pragma warning disable 1591
    public abstract class JSHandler : IDisposable
    {
        internal IJSObjectReference Handle { get; set; }

        public bool IsDisposed { get; private set; }

        ~JSHandler()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            await Handle.DisposeAsync();
        }
    }

    public partial class WebGLContextAttributes : JSHandler
    {
        public WebGLContextAttributes(IJSObjectReference canvas)
        {
            Handle = canvas;
        }
    }

    public partial class WebGLObject : JSHandler
    {
    }

    public partial class WebGLRenderingContext : WebGLRenderingContextBase
    {
        public WebGLRenderingContext(IJSObjectReference canvas, IJSRuntime runtime)
            : base(canvas, runtime, "webgl")
        {
        }

        public WebGLRenderingContext(IJSObjectReference canvas, IJSRuntime runtime, WebGLContextAttributes contextAttributes)
            : base(canvas, runtime, "webgl", contextAttributes)
        {
        }
    }

    public abstract partial class WebGLRenderingContextBase : JSHandler
    {
        private const string WindowPropertyName = "WebGLRenderingContext";

        protected readonly IJSObjectReference gl;
        protected readonly IJSRuntime runtime;

        protected WebGLRenderingContextBase(
            IJSObjectReference canvas,
            IJSRuntime runtime,
            string contextType,
            string windowPropertyName = WindowPropertyName)
            : this(canvas, runtime, contextType, null, windowPropertyName)
        {
        }

        protected WebGLRenderingContextBase(
            IJSObjectReference canvas,
            IJSRuntime runtime,
            string contextType,
            WebGLContextAttributes contextAttributes,
            string windowPropertyName = WindowPropertyName)
        {
            this.runtime = runtime;

            gl = ((IJSInProcessRuntime)BlazorExtensions.Runtime).Invoke<IJSObjectReference>("generateCtx", contextAttributes);
        }

        public static bool IsVerbosityEnabled { get; set; } = false;

        protected bool CheckWindowPropertyExists(string property)
        {
            IJSObjectReference window = runtime.GetGlobalObject<IJSObjectReference>("window");
            object exists = ((IJSInProcessObjectReference)window).GetObjectProperty<object>(property);

            return exists != null;
        }

        protected void Invoke(string method, params object[] args)
        {
            object[] actualArgs = Translate(args);
            ((IJSInProcessObjectReference)gl).InvokeVoid(method, actualArgs);
        }

        protected T Invoke<T>(string method, params object[] args)
            where T : JSHandler, new()
        {
            object[] actualArgs = Translate(args);
            IJSObjectReference rawResult = ((IJSInProcessObjectReference)gl).Invoke<IJSObjectReference>(method, actualArgs);

            return new T
            {
                Handle = rawResult
            };
        }

        protected uint[] InvokeForIntToUintArray(string method, params object[] args)
        {
            int[] temp = InvokeForArray<int>(method, args);

            if (temp == null)
            {
                return null;
            }

            uint[] result = new uint[temp.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = (uint)temp[i];
            }

            return result;
        }

        // TODO: Make unmarshalled with custom javascript function
        protected T[] InvokeForArray<T>(string method, params object[] args)
        {
            Array rawResult = ((IJSInProcessObjectReference)gl).Invoke<Array>(method, args);
            return rawResult.ToArray(item => (T)item);

        }

        // TODO: Make unmarshalled with custom javascript function
        protected T[] InvokeForJavaScriptArray<T>(string method, params object[] args)
            where T : JSHandler, new()
        {
            IJSUnmarshalledObjectReference[] rawResult = ((IJSInProcessObjectReference)gl).Invoke<IJSUnmarshalledObjectReference[]>(method, args);
            return rawResult.ToArray(item => new T { Handle = (IJSObjectReference)item });
        }

        protected T InvokeForBasicType<T>(string method, params object[] args)
            where T : IConvertible
        {
            object[] actualArgs = Translate(args);
            T result = ((IJSInProcessObjectReference)gl).Invoke<T>(method, actualArgs);

            return result;
        }

        private string Dump(object @object)
        {
            return $"{@object ?? "null"} ({@object?.GetType()})";
        }

        private object[] Translate(object[] args)
        {
            object[] actualArgs = new object[args.Length];

            for (int i = 0; i < actualArgs.Length; i++)
            {
                object arg = args[i];

                if (arg == null)
                {
                    actualArgs[i] = null;
                    continue;
                }

                if (arg is JSHandler jsHandler)
                {
                    arg = jsHandler.Handle;
                }
                else if (arg is System.Array array)
                {
                    if (((System.Array)arg).GetType().GetElementType().IsPrimitive)
                    {
                        arg = array;
                    }
                    else
                    {
                        List<object> argArray = new();
                        foreach (object item in (System.Array)arg)
                        {
                            argArray.Add(item);
                        }
                        arg = argArray;
                    }
                }

                actualArgs[i] = arg;
            }

            return actualArgs;
        }
    }

    public partial class WebGLShaderPrecisionFormat : JSHandler
    {
    }

    public partial class WebGLUniformLocation : JSHandler
    {
    }

    public partial class WebGL2RenderingContext : WebGL2RenderingContextBase
    {
        public WebGL2RenderingContext(IJSObjectReference canvas, IJSRuntime runtime)
            : base(canvas, runtime, "webgl2")
        {
        }

        public WebGL2RenderingContext(IJSObjectReference context, IJSObjectReference canvas, IJSRuntime runtime)
           : base(canvas, runtime, "webgl2")
        {
            Handle = context;
        }

        public WebGL2RenderingContext(IJSObjectReference canvas, IJSRuntime runtime, WebGLContextAttributes contextAttributes)
            : base(canvas, runtime, "webgl2", contextAttributes)
        {
        }
    }

    public abstract partial class WebGL2RenderingContextBase : WebGLRenderingContextBase
    {
        private const string WindowPropertyName = "WebGL2RenderingContext";

        protected WebGL2RenderingContextBase(
            IJSObjectReference canvas,
            IJSRuntime runtime,
            string contextType,
            string windowPropertyName = WindowPropertyName)
            : this(canvas, runtime, contextType, null, windowPropertyName)
        {
        }

        protected WebGL2RenderingContextBase(
            IJSObjectReference canvas,
            IJSRuntime runtime,
            string contextType,
            WebGLContextAttributes contextAttributes,
            string windowPropertyName = WindowPropertyName)
            : base(canvas, runtime, contextType, contextAttributes, windowPropertyName)
        {
        }

        public bool IsSupported => CheckWindowPropertyExists(WindowPropertyName);

        public void TexImage2D(
         uint target,
         int level,
         int internalformat,
         int width,
         int height,
         int border,
         uint format,
         uint type,
         IntPtr source)
        {
            //// TODO(MR): managed to native via javscript (implement & test)
            ////using var nativeArray = Uint8Array.From(source);
            GLTexImage2D(target, level, internalformat, width, height, border, format, type, source);

        }



        //public void TexImage2D(
        //  uint target,
        //  int level,
        //  int internalformat,
        //  int width,
        //  int height,
        //  int border,
        //  uint format,
        //  uint type,
        //  ReadOnlySpan<byte> source)
        //{
        //  //// TODO(MR): managed to native via javscript (implement & test)
        //  ////using var nativeArray = Uint8Array.From(source);
        //  GLTexImage2D(target, level, internalformat, width, height, border, format, type, source.ToArray());

        //}

        public void TexImage2D(
          uint target,
          int level,
          int internalformat,
          int width,
          int height,
          int border,
          uint format,
          uint type,
          Array source)
        {
            GLTexImage2D(target, level, internalformat, width, height, border, format, type, source);

        }

        public void TexImage3D(
            uint target,
            int level,
            int internalformat,
            int width,
            int height,
            int depth,
            int border,
            uint format,
            uint type,
            Array source)
        {
            GLTexImage3D(target, level, internalformat, width, height, depth, border, format, type, source);
        }

        public void TexSubImage3D(
            uint target,
            int level,
            int xoffset,
            int yoffset,
            int zoffset,
            int width,
            int height,
            int depth,
            uint format,
            uint type,
            Array source)
        {
            // TODO(MR): managed to native via javscript (implement & test)
            //using var nativeArray = Uint8Array.From(source);
            TexSubImage3D(
                target,
                level,
                xoffset,
                yoffset,
                zoffset,
                width,
                height,
                depth,
                format,
                type,
                source);
        }

        public void TexSubImage2D(
            uint target,
            int level,
            int xoffset,
            int yoffset,
            int width,
            int height,
            uint format,
            uint type,
            Array source)
        {
            // TODO(MR): managed to native via javscript (implement & test)
            //using var nativeArray = Uint8Array.From(source);
            GLTexSubImage2D(
                target,
                level,
                xoffset,
                yoffset,
                width,
                height,
                format,
                type,
                source);
        }
    }

#pragma warning restore 1591
}