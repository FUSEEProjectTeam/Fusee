﻿using System;
using System.Text;
using WebAssembly;
using WebAssembly.Core;
using Array = System.Array;

namespace Fusee.Engine.Imp.Graphics.WebAsm
{
#pragma warning disable 1591
    public abstract class JSHandler : IDisposable
    {
        internal JSObject Handle { get; set; }

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

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            Handle.Dispose();
        }
    }

    public partial class WebGLActiveInfo : JSHandler
    {
    }

    public partial class WebGLContextAttributes : JSHandler
    {
    }

    public partial class WebGLObject : JSHandler
    {
    }

    public partial class WebGLRenderingContext : WebGLRenderingContextBase
    {
        public WebGLRenderingContext(JSObject canvas) 
            : base(canvas, "webgl")
        {
        }

        public WebGLRenderingContext(JSObject canvas, JSObject contextAttributes) 
            : base(canvas, "webgl", contextAttributes)
        {
        }
    }

    public abstract partial class WebGLRenderingContextBase : JSHandler
    {
        private const string WindowPropertyName = "WebGLRenderingContext";

        protected readonly JSObject gl;

        protected WebGLRenderingContextBase(
            JSObject canvas, 
            string contextType,
            string windowPropertyName = WindowPropertyName) 
            : this(canvas, contextType, null, windowPropertyName)
        {
        }

        protected WebGLRenderingContextBase(
            JSObject canvas, 
            string contextType, 
            JSObject contextAttributes, 
            string windowPropertyName = WindowPropertyName)
        {
            if (!CheckWindowPropertyExists(windowPropertyName))
            {
                throw new PlatformNotSupportedException(
                    $"The context '{contextType}' is not supported in this browser");
            }

            gl = (JSObject)canvas.Invoke("getContext", contextType, contextAttributes);
        }

        public static bool IsSupported => CheckWindowPropertyExists(WindowPropertyName);

        public static bool IsVerbosityEnabled { get; set; } =
#if DEBUG
            false; //(mr): disabled 
#else
            false;
#endif

        public ITypedArray CastNativeArray(object managedArray)
        {
            var arrayType = managedArray.GetType();
            ITypedArray array;

            // Here are listed some JavaScript array types:
            // https://github.com/mono/mono/blob/a7f5952c69ae76015ccaefd4dfa8be2274498a21/sdks/wasm/bindings-test.cs
            if (arrayType == typeof(byte[]))
            {
                array = Uint8Array.From((byte[])managedArray);
            }
            else if (arrayType == typeof(float[]))
            {
                array = Float32Array.From((float[])managedArray);
            }
            else if (arrayType == typeof(ushort[]))
            {
                array = Uint16Array.From((ushort[])managedArray);
            }
            else if (arrayType == typeof(uint[]))
            {
                array = Uint32Array.From((uint[])managedArray);
            }
            else
            {
                throw new NotImplementedException();
            }

            return array;
        }

        protected static bool CheckWindowPropertyExists(string property)
        {
            var window = (JSObject)Runtime.GetGlobalObject();
            var exists = window.GetObjectProperty(property) != null;

            return exists;
        }

        private void DisposeArrayTypes(object[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg is ITypedArray typedArray && typedArray != null)
                {
                    var disposable = (IDisposable)typedArray;
                    disposable.Dispose();
                }
                if (arg is WebAssembly.Core.Array jsArray && jsArray != null)
                {
                    var disposable = (IDisposable)jsArray;
                    disposable.Dispose();

                }
            }
        }

        protected object Invoke(string method, params object[] args)
        {
            var actualArgs = Translate(args);
            var result = gl.Invoke(method, actualArgs);
            DisposeArrayTypes(actualArgs);

            if (IsVerbosityEnabled)
            {
                var dump = new StringBuilder();
                dump.Append($"{method}(");

                for (var i = 0; i < args.Length; i++)
                {
                    var item = args[i];
                    dump.Append(Dump(item));

                    if (i < (args.Length - 1))
                    {
                        dump.Append(", ");
                    }
                }

                dump.Append($") = {Dump(result)}");
                Console.WriteLine(dump);
            }

            return result;
        }

        protected T Invoke<T>(string method, params object[] args)
            where T : JSHandler, new()
        {
            var rawResult = Invoke(method, args);

            var result = new T
            {
                Handle = (JSObject)rawResult
            };

            return result;
        }

        protected uint[] InvokeForIntToUintArray(string method, params object[] args)
        {
            var temp = InvokeForArray<int>(method, args);

            if (temp == null)
            {
                return null;
            }

            var result = new uint[temp.Length];

            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = (uint)temp[i];
            }

            return result;
        }

        protected T[] InvokeForArray<T>(string method, params object[] args)
        {
            using (var rawResult = (WebAssembly.Core.Array)Invoke(method, args))
            {
                return rawResult.ToArray(item => (T)item);
            }
        }

        protected T[] InvokeForJavaScriptArray<T>(string method, params object[] args)
            where T : JSHandler, new()
        {
            using (var rawResult = (WebAssembly.Core.Array)Invoke(method, args))
            {
                return rawResult.ToArray(item => new T { Handle = (JSObject)item });
            }
        }

        protected T InvokeForBasicType<T>(string method, params object[] args)
            where T : IConvertible
        {
            var result = Invoke(method, args);

            return (T)result;
        }

        private string Dump(object @object) => $"{@object ?? "null"} ({@object?.GetType()})";

        private object[] Translate(object[] args)
        {
            var actualArgs = new object[args.Length];

            for (int i = 0; i < actualArgs.Length; i++)
            {
                var arg = args[i];

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
                        arg = CastNativeArray(array);
                    else
                    {
                        // WebAssembly.Core.Array or Runtime should probably provide some type of
                        // helper functions for doing this.  I will put it on my todo list.
                        var argArray = new WebAssembly.Core.Array();
                        foreach(var item in (System.Array)arg)
                        {
                            argArray.Push(item);
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

    public partial class WebGLUniformLocation : JSHandler, IDisposable
    {
    }

    public partial class WebGL2RenderingContext : WebGL2RenderingContextBase
    {
        public WebGL2RenderingContext(JSObject canvas) 
            : base(canvas, "webgl2")
        { 
        }

        public WebGL2RenderingContext(JSObject canvas, JSObject contextAttributes) 
            : base(canvas, "webgl2", contextAttributes)
        {
        }
    }

    public abstract partial class WebGL2RenderingContextBase : WebGLRenderingContextBase
    {
        private const string WindowPropertyName = "WebGL2RenderingContext";

        protected WebGL2RenderingContextBase(
            JSObject canvas, 
            string contextType,
            string windowPropertyName = WindowPropertyName) 
            : this(canvas, contextType, null, windowPropertyName)
        {
        }

        protected WebGL2RenderingContextBase(
            JSObject canvas, 
            string contextType, 
            JSObject contextAttributes, 
            string windowPropertyName = WindowPropertyName) 
            : base(canvas, contextType, contextAttributes, windowPropertyName)
        {
        }

        public new static bool IsSupported => CheckWindowPropertyExists(WindowPropertyName);
    }

    public static class GLExtensions
    {
        public static WebGLBuffer CreateArrayBufferWithUsage(this WebGLRenderingContextBase gl, Array items, uint usage)
        {
            var arrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, arrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ARRAY_BUFFER, items, usage);

            return arrayBuffer;
        }

        public static WebGLBuffer CreateArrayBuffer(this WebGLRenderingContextBase gl, Array items)
        {
            var arrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, arrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ARRAY_BUFFER, items, WebGLRenderingContextBase.STATIC_DRAW);
            gl.BindBuffer(WebGLRenderingContextBase.ARRAY_BUFFER, null);

            return arrayBuffer;
        }

        public static WebGLBuffer CreateElementArrayBuffer(this WebGLRenderingContextBase gl, Array items)
        {
            var elementArrayBuffer = gl.CreateBuffer();
            gl.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, elementArrayBuffer);
            gl.BufferData(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, items, WebGLRenderingContextBase.STATIC_DRAW);
            gl.BindBuffer(WebGLRenderingContextBase.ELEMENT_ARRAY_BUFFER, null);

            return elementArrayBuffer;
        }

        public static WebGLProgram InitializeShaders(this WebGLRenderingContextBase gl, string vertexShaderCode, string fragmentShaderCode)
        {
            var shaderProgram = gl.CreateProgram();

            var vertexShader = GetShader(gl, vertexShaderCode, WebGLRenderingContextBase.VERTEX_SHADER);
            var fragmentShader = GetShader(gl, fragmentShaderCode, WebGLRenderingContextBase.FRAGMENT_SHADER);

            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);

            gl.LinkProgram(shaderProgram);

            gl.UseProgram(shaderProgram);

            return shaderProgram;
        }

        public static WebGLShader GetShader(this WebGLRenderingContextBase gl, string shaderSource, uint type)
        {
            var shader = gl.CreateShader(type);
            gl.ShaderSource(shader, shaderSource);
            gl.CompileShader(shader);

            var message = gl.GetShaderInfoLog(shader);
            if (message.Length > 0)
            {
                var msg = $"Shader Error: {message}";
                throw new Exception(msg);
            }

            return shader;
        }
    }

#pragma warning restore 1591
}
