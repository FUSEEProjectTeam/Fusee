﻿using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.Math.Core;
using Fusee.Serialization;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Numerics;
using ImVec4 = System.Numerics.Vector4;
using System.Drawing;
using Font = Fusee.Base.Core.Font;
using static System.Net.Mime.MediaTypeNames;


namespace Fusee.Examples.Simple.Desktop
{
    public class Simple
    {
        private static Core.Simple app;
        private static RenderCanvasImp _renderCanvas;
        private static RenderContextImp _renderCtx;

        public static void Main()
        {
            // Inject Fusee.Engine.Base InjectMe dependencies
            IO.IOImp = new Fusee.Base.Imp.Desktop.IOImp();

            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("Assets");
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(Font),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return await Task.FromResult(new Font { _fontImp = new FontImp((Stream)storage) });
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return new Font { _fontImp = new FontImp((Stream)storage) };
                    },
                    Checker = id => Path.GetExtension(id).Contains("ttf", System.StringComparison.OrdinalIgnoreCase)
                });
            fap.RegisterTypeHandler(
                new AssetHandler
                {
                    ReturnedType = typeof(SceneContainer),
                    DecoderAsync = async (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return await FusSceneConverter.ConvertFromAsync(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Decoder = (string id, object storage) =>
                    {
                        if (!Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)) return null;
                        return FusSceneConverter.ConvertFrom(ProtoBuf.Serializer.Deserialize<FusFile>((Stream)storage), id);
                    },
                    Checker = id => Path.GetExtension(id).Contains("fus", System.StringComparison.OrdinalIgnoreCase)
                });

            AssetStorage.RegisterProvider(fap);


            app = new Core.Simple();

            // Inject Fusee.Engine InjectMe dependencies (hard coded)
            System.Drawing.Icon appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            app.CanvasImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasImp(appIcon);
            app.ContextImplementor = new Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp(app.CanvasImplementor);
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.RenderCanvasInputDriverImp(app.CanvasImplementor));
            Input.AddDriverImp(new Fusee.Engine.Imp.Graphics.Desktop.WindowsTouchInputDriverImp(app.CanvasImplementor));


            _renderCanvas = ((Engine.Imp.Graphics.Desktop.RenderCanvasImp)app.CanvasImplementor);
            _renderCtx = ((Engine.Imp.Graphics.Desktop.RenderContextImp)app.ContextImplementor);

            app.InitAppCustomRenderMethod();

            _renderCanvas.Init += InitImGUI;
            _renderCanvas.Update += UpdateImGUI;
            _renderCanvas.Render += RenderImGUI;


            // Start the app
            app.Run();
        }

        #region IMGUI

        private struct UniformFieldInfo
        {
            public int Location;
            public string Name;
            public int Size;
            public ActiveUniformType Type;
        }

        private static int _vertexArray;
        private static int _vertexBuffer;
        private static int _vertexBufferSize;
        private static int _indexBuffer;
        private static int _indexBufferSize;

        private static int _windowWidth;
        private static int _windowHeight;

        private static Vector2 _scaleFactor = System.Numerics.Vector2.One;

        private static readonly string _vertexSource = @"#version 330 core
                                                uniform mat4 projection_matrix;
                                                layout(location = 0) in vec2 in_position;
                                                layout(location = 1) in vec2 in_texCoord;
                                                layout(location = 2) in vec4 in_color;
                                                out vec4 color;
                                                out vec2 texCoord;
                                                void main()
                                                {
                                                    gl_Position = projection_matrix * vec4(in_position, 0, 1);
                                                    color = in_color;
                                                    texCoord = in_texCoord;
                                                }";
        private static readonly string _fragmentSource = @"#version 330 core
                                                uniform sampler2D in_fontTexture;
                                                in vec4 color;
                                                in vec2 texCoord;
                                                out vec4 outputColor;
                                                void main()
                                                {
                                                    outputColor = color * texture(in_fontTexture, texCoord);
                                                }";

        private static int _shaderProgram;
        private static readonly Dictionary<string, UniformFieldInfo> _uniformVarToLocation = new();
        private static int _viewportFB;
        private static int _renderTexture;
        private static int _depthRenderbuffer;
        private static int _texture4ImGui;
        private static int _intermediateFBO;
        private static Vector2 _min = new(0, 0);
        private static Vector2 _max = new(0, 0);
        private static Vector2 _size = new(0, 0);
        private static Vector2 _pos = new(0, 0);

        private static bool _initialized = false;

        private static void InitImGUI(object sender, InitEventArgs args)
        {
            _windowWidth = _renderCanvas._gameWindow.Size.X;
            _windowHeight = _renderCanvas._gameWindow.Size.Y;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            io.Fonts.AddFontFromFileTTF("Assets/Lato-Black.ttf", 14);


            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;


            var style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 2.0f;             // Radius of window corners rounding. Set to 0.0f to have rectangular windows
            style.ScrollbarRounding = 3.0f;             // Radius of grab corners rounding for scrollbar
            style.GrabRounding = 2.0f;             // Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
            style.AntiAliasedLines = true;
            style.AntiAliasedFill = true;
            style.WindowRounding = 2;
            style.ChildRounding = 2;
            style.ScrollbarSize = 16;
            style.ScrollbarRounding = 3;
            style.GrabRounding = 2;
            style.ItemSpacing.X = 10;
            style.ItemSpacing.Y = 4;
            style.IndentSpacing = 22;
            style.FramePadding.X = 6;
            style.FramePadding.Y = 4;
            style.Alpha = 1.0f;
            style.FrameRounding = 3.0f;

            colors[(int)ImGuiCol.Text] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.60f, 0.60f, 0.60f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
            //color(int)s[ImGuiCol_ChildWindowBg]         = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.93f, 0.93f, 0.93f, 0.98f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.71f, 0.71f, 0.71f, 0.08f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.04f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.71f, 0.71f, 0.71f, 0.55f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.94f, 0.94f, 0.94f, 0.55f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.71f, 0.78f, 0.69f, 0.98f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.85f, 0.85f, 0.85f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.82f, 0.78f, 0.78f, 0.51f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.78f, 0.78f, 0.78f, 1.00f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.86f, 0.86f, 0.86f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.20f, 0.25f, 0.30f, 0.61f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.90f, 0.90f, 0.90f, 0.30f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.92f, 0.92f, 0.92f, 0.78f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.184f, 0.407f, 0.193f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.71f, 0.78f, 0.69f, 0.40f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.725f, 0.805f, 0.702f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.793f, 0.900f, 0.836f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.71f, 0.78f, 0.69f, 0.31f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.71f, 0.78f, 0.69f, 0.80f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.71f, 0.78f, 0.69f, 1.00f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.14f, 0.44f, 0.80f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.14f, 0.44f, 0.80f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(1.00f, 1.00f, 1.00f, 0.00f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.26f, 0.59f, 0.98f, 0.45f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.78f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.39f, 0.39f, 0.39f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            //colors[(int)ImGuiCol.ModalWindowDarkening] = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            colors[(int)ImGuiCol.NavHighlight] = colors[(int)ImGuiCol.HeaderHovered];
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(0.70f, 0.70f, 0.70f, 0.70f);

            CreateDeviceResources();

            SetPerFrameImGuiData(1f / 60f);

            _initialized = true;

        }

        private static void CreateDeviceResources()
        {
            _shaderProgram = GL.CreateProgram();

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, _vertexSource);
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string Info = GL.GetShaderInfoLog(vertexShader);
                Debug.WriteLine($"GL.CompileShader for shader '{vertexShader}' had info log:\n{Info}");
            }

            var pixelShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShader, _fragmentSource);
            GL.CompileShader(pixelShader);

            GL.GetShader(pixelShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string Info = GL.GetShaderInfoLog(pixelShader);
                Debug.WriteLine($"GL.CompileShader for shader '{pixelShader}' had info log:\n{Info}");
            }

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, pixelShader);

            GL.LinkProgram(_shaderProgram);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out success);

            if (success == 0)
            {
                string Info = GL.GetProgramInfoLog(_shaderProgram);
                Debug.WriteLine($"GL.LinkProgram had info log:\n{Info}");
            }

            GL.DetachShader(_shaderProgram, vertexShader);
            GL.DetachShader(_shaderProgram, pixelShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(pixelShader);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.ActiveUniforms, out int cnt);

            for (var i = 0; i < cnt; i++)
            {
                var name = GL.GetActiveUniform(_shaderProgram, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo FieldInfo;
                FieldInfo.Location = GL.GetUniformLocation(_shaderProgram, name);
                FieldInfo.Name = name;
                FieldInfo.Size = Size;
                FieldInfo.Type = Type;

                _uniformVarToLocation.Add(name, FieldInfo);
            }

            GL.CreateVertexArrays(1, out _vertexArray);

            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;

            GL.CreateBuffers(1, out _vertexBuffer);
            GL.CreateBuffers(1, out _indexBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            RecreateFontDeviceTexture();

            GL.VertexArrayVertexBuffer(_vertexArray, 0, _vertexBuffer, IntPtr.Zero, Unsafe.SizeOf<ImDrawVert>());
            GL.VertexArrayElementBuffer(_vertexArray, _indexBuffer);

            GL.EnableVertexArrayAttrib(_vertexArray, 0);
            GL.VertexArrayAttribBinding(_vertexArray, 0, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 0, 2, VertexAttribType.Float, false, 0);

            GL.EnableVertexArrayAttrib(_vertexArray, 1);
            GL.VertexArrayAttribBinding(_vertexArray, 1, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 1, 2, VertexAttribType.Float, false, 8);

            GL.EnableVertexArrayAttrib(_vertexArray, 2);
            GL.VertexArrayAttribBinding(_vertexArray, 2, 0);
            GL.VertexArrayAttribFormat(_vertexArray, 2, 4, VertexAttribType.UnsignedByte, true, 16);

            if (GL.GetError() != 0)
            {
                throw new Exception($"OpenGL Error {GL.GetError()}");
            }

            // FRAMEBUFFER STUFF
            GL.CreateFramebuffers(1, out _viewportFB);
            GL.GenTextures(1, out _renderTexture);
            GL.GenRenderbuffers(1, out _depthRenderbuffer);

            // Set up texture to blt to
            _texture4ImGui = GL.GenTexture();
            GL.CreateFramebuffers(1, out _intermediateFBO);

        }

        private static unsafe void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);

            io.Fonts.SetTexID(new IntPtr(id));

            io.Fonts.ClearTexData();
        }

        private static void SetPerFrameImGuiData(float deltaSeconds)
        {
            _windowHeight = _renderCanvas._gameWindow.Size.Y;
            _windowWidth = _renderCanvas._gameWindow.Size.X;
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                _windowWidth / _scaleFactor.X,
                _windowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        private static void UpdateImGuiInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseDown[0] = Input.Mouse.LeftButton;
            io.MouseDown[1] = Input.Mouse.MiddleButton;
            io.MouseDown[2] = Input.Mouse.RightButton;

            io.MousePos = new System.Numerics.Vector2(Input.Mouse.X, Input.Mouse.Y);

            io.MouseWheel = Input.Mouse.Wheel;
            io.MouseWheelH = 0;
        }

        private static void UpdateImGUI(object sender, RenderEventArgs args)
        {
            if (!app.IsLoaded) return;
            if (!_initialized) return;

            SetPerFrameImGuiData(_renderCanvas.DeltaTimeUpdate);
            UpdateImGuiInput();

            ImGui.NewFrame();
        }

        private static void UpdateRenderTexture(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _intermediateFBO);
            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture4ImGui, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.Multisample);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _viewportFB);
            // "Bind" the newly created texture : all future texture functions will modify this texture
            GL.BindTexture(TextureTarget.Texture2DMultisample, _renderTexture);

            // Give an empty image to OpenGL ( the last "0" )
            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 4, PixelInternalFormat.Rgb, width, height, true);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRenderbuffer);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, _renderTexture, 0);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == 0)
            {
                throw new Exception("Error Framebuffer!");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        private static bool _dockspaceOpen = true;
        private static readonly bool _viewportOpen = true;
        private static bool _render = true;

        private static void RenderImGUI(object sender, RenderEventArgs args)
        {
            if (!app.IsLoaded) return;

            if (app.IsShuttingDown) return;

            Input.Instance.PreRender();
            Time.Instance.DeltaTimeIncrement = _renderCanvas.DeltaTime;



            #region FuseeRender

            if (_size.X != 0)
            {
                //GL.ClearColor(0.9f, 0.9f, 0.9f, 1);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                app.RC.Viewport(0, 0, (int)_size.X, (int)_size.Y);

                // Enable FB
                if (_render)
                {
                    UpdateRenderTexture((int)_size.X, (int)_size.Y);
                    // app.RC.Viewport((int)_pos.X, (int)(((_windowHeight - _size.Y)) - _pos.Y), (int)_size.X, (int)_size.Y);
                    app.RenderAFrame();

                    // after rendering, blt result into _texture4ImGui
                    GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _viewportFB);
                    GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _intermediateFBO);
                    GL.BlitFramebuffer(0, 0, (int)_size.X, (int)_size.Y, 0, 0, (int)_size.X, (int)_size.Y, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);
                }
            }

            // Disable FB
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Viewport(0, 0, _windowWidth, _windowHeight);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            #endregion
            //////////////// --- IMGUI GOES HERE --- //////////////

            // Set Window flags for Dockspace
            var wndDockspaceFlags =
                    ImGuiWindowFlags.NoDocking
                    | ImGuiWindowFlags.NoTitleBar
                    | ImGuiWindowFlags.NoCollapse
                    | ImGuiWindowFlags.NoResize
                    | ImGuiWindowFlags.NoMove
                    | ImGuiWindowFlags.NoBringToFrontOnFocus
                    | ImGuiWindowFlags.NoFocusOnAppearing;

            var dockspaceFlags = ImGuiDockNodeFlags.PassthruCentralNode /*| ImGuiDockNodeFlags.AutoHideTabBar*/;

            var viewport = ImGui.GetMainViewport();

            // Set the parent window's position, size, and viewport to match that of the main viewport. This is so the parent window
            // completely covers the main viewport, giving it a "full-screen" feel.
            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(viewport.WorkSize);
            ImGui.SetNextWindowViewport(viewport.ID);

            // Set the parent window's styles to match that of the main viewport:
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f); // No corner rounding on the window
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f); // No border around the window
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0.0f, 0.0f));

            // Create Dockspace
            ImGui.Begin("DockSpace", ref _dockspaceOpen, wndDockspaceFlags);

            var dockspace_id = ImGui.GetID("DockSpace");
            ImGui.DockSpace(dockspace_id, new Vector2(0.0f, 0.0f), dockspaceFlags);

            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();

            // Titlebar
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Menu"))
                {
                    if (ImGui.MenuItem("Open"))
                    {

                    }
                    if (ImGui.MenuItem("Exit"))
                    {
                        Environment.Exit(0);
                    }
                    ImGui.EndMenu();
                }
            }
            ImGui.EndMainMenuBar();

            ImGui.Begin("Viewport",
                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);

            if (ImGui.Button(_render ? "Pause" : "Continue"))
            {
                _render = !_render;
            }

            var parentMin = ImGui.GetWindowContentRegionMin();
            var parentMax = ImGui.GetWindowContentRegionMax();
            var size = parentMax - parentMin;

            // Using a Child allow to fill all the space of the window.
            // It also allows customization

            ImGui.BeginChild("GameRender", size, true, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

            _min = ImGui.GetWindowContentRegionMin();
            _max = ImGui.GetWindowContentRegionMax();
            _size = _max - _min;
            _pos = ImGui.GetWindowPos();

            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            ImGui.Image(new IntPtr(_texture4ImGui), _size,
                new Vector2(0, 1),
                new Vector2(1, 0));


            app.RC.ViewportXStart = (int)_pos.X;
            app.RC.ViewportYStart = (int)((_windowHeight - _size.Y) - _pos.Y);

            ImGui.CaptureMouseFromApp();

            ImGui.EndChild();
            ImGui.End();


            ImGui.Begin("Settings");
            ImGui.Text("Fusee PointCloud Rendering");
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:0.00} ms/frame ({ImGui.GetIO().Framerate:0} FPS)");
            ImGui.NewLine();
            ImGui.Button("Open File");
            ImGui.SameLine();
            ImGui.Button("Reset Camera");
            ImGui.SameLine();
            ImGui.Button("Show Octree");

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Visibility");
            ImGui.InputFloat("Threshold", ref _threshold);
            ImGui.SliderFloat("Min. Projection Size Modifier", ref _minProj, 0f, 1f);
            ImGui.EndGroup();


            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Lighting");
            ImGui.SliderInt("EDL Neighbor Px", ref _edlNeighbour, 0, 5);
            ImGui.SliderFloat("EDL Strength", ref _edlStrength, -1f, 5f);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Shape");
            ImGui.Combo("PointShape", ref _currentPtShape, new string[] { "Paraboloid", "Box", "Square" }, 3);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Point Size Method");
            ImGui.Combo("Point Size Method", ref _currentPtSizeMethod, new string[] { "FixedPixelSize", "Adaptive", "Third" }, 3);
            ImGui.SliderFloat("Point Size", ref _ptSize, 0.2f, 20f);
            ImGui.EndGroup();

            ImGui.NewLine();
            ImGui.Spacing();
            ImGui.BeginGroup();
            ImGui.Text("Color Mode");
            if (ImGui.ColorButton("Toggle Color Picker", _ptColor))
            {
                _colorPickerOpen = !_colorPickerOpen;

            }
            if (_colorPickerOpen)
            {
                ImGui.Begin("Color Picker", ref _colorPickerOpen, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.ColorPicker4("Color", ref _ptColor);
                ImGui.End();
            }

            ImGui.EndGroup();

            ImGui.BeginGroup();

            ImGui.SliderAngle("Colorpicker", ref color);

            ImGui.EndGroup();

            ImGui.End();

            // ImGui.ShowDemoWindow();



            ImGui.Render();

            RenderImDrawData(ImGui.GetDrawData());



            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            app.Present();

            Input.Instance.PostRender();
        }

        private static ImDrawListPtr _drawList;

        private static float[] _view;
        private static float[] _model;
        private static float[] _proj;

        private static float color;

        private static float _threshold;
        private static int _edlNeighbour;
        private static float _edlStrength;
        private static int _currentPtShape;
        private static int _currentPtSizeMethod;
        private static float _ptSize;
        private static float _minProj;
        private static Vector4 _ptColor;
        private static bool _colorPickerOpen = false;


        private static unsafe void RenderImDrawData(ImDrawDataPtr draw_data)
        {

            uint vertexOffsetInVertices = 0;
            uint indexOffsetInElements = 0;

            if (draw_data.CmdListsCount == 0)
            {
                return;
            }

            uint totalVBSize = (uint)(draw_data.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
            if (totalVBSize > _vertexBufferSize)
            {
                int newSize = (int)M.Max(_vertexBufferSize * 1.5f, totalVBSize);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                _vertexBufferSize = newSize;

                Console.WriteLine($"Resized vertex buffer to new size {_vertexBufferSize}");
            }

            uint totalIBSize = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > _indexBufferSize)
            {
                int newSize = (int)M.Max(_indexBufferSize * 1.5f, totalIBSize);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _indexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                _indexBufferSize = newSize;

                Console.WriteLine($"Resized index buffer to new size {_indexBufferSize}");
            }


            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                var cmd_list = draw_data.CmdListsRange[i];

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(vertexOffsetInVertices * Unsafe.SizeOf<ImDrawVert>()), cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _indexBuffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(indexOffsetInElements * sizeof(ushort)), cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
                indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
            }

            // render Imguizmo




            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            OpenTK.Mathematics.Matrix4 mvp = OpenTK.Mathematics.Matrix4.CreateOrthographicOffCenter(
               0.0f,
               io.DisplaySize.X,
               io.DisplaySize.Y,
               0.0f,
               -1.0f,
               1.0f);

            GL.UseProgram(_shaderProgram);
            GL.UniformMatrix4(_uniformVarToLocation["projection_matrix"].Location, false, ref mvp);
            GL.Uniform1(_uniformVarToLocation["in_fontTexture"].Location, 0);

            GL.BindVertexArray(_vertexArray);

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);


            // Render command lists
            int vtx_offset = 0;
            int idx_offset = 0;
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[n];
                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, _windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

                        GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);
                    }

                    idx_offset += (int)pcmd.ElemCount;
                }
                vtx_offset += cmd_list.VtxBuffer.Size;
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        #endregion

    }


    //public class FilePicker
    //{
    //    private const string FilePickerID = "###FilePicker";
    //    private static readonly Dictionary<object, FilePicker> s_filePickers = new Dictionary<object, FilePicker>();
    //    private static readonly Vector2 DefaultFilePickerSize = new Vector2(600, 400);

    //    public string CurrentFolder { get; set; }
    //    public string SelectedFile { get; set; }

    //    public static FilePicker GetFilePicker(object o, string startingPath)
    //    {
    //        if (File.Exists(startingPath))
    //        {
    //            startingPath = new FileInfo(startingPath).DirectoryName;
    //        }
    //        else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
    //        {
    //            startingPath = Application.Instance.ProjectContext.GetAssetRootPath();
    //            if (string.IsNullOrEmpty(startingPath))
    //            {
    //                startingPath = AppContext.BaseDirectory;
    //            }
    //        }

    //        if (!s_filePickers.TryGetValue(o, out FilePicker fp))
    //        {
    //            fp = new FilePicker();
    //            fp.CurrentFolder = startingPath;
    //            s_filePickers.Add(o, fp);
    //        }

    //        return fp;
    //    }

    //    public bool Draw(ref string selected)
    //    {
    //        string label = null;
    //        if (selected != null)
    //        {
    //            if (Util.TryGetFileInfo(selected, out FileInfo realFile))
    //            {
    //                label = realFile.Name;
    //            }
    //            else
    //            {
    //                label = "<Select File>";
    //            }
    //        }
    //        if (ImGui.Button(label))
    //        {
    //            ImGui.OpenPopup(FilePickerID);
    //        }

    //        bool result = false;
    //        ImGui.SetNextWindowSize(DefaultFilePickerSize, Condition.FirstUseEver);
    //        if (ImGui.BeginPopupModal(FilePickerID, WindowFlags.NoTitleBar))
    //        {
    //            result = DrawFolder(ref selected, true);
    //            ImGui.EndPopup();
    //        }

    //        return result;
    //    }

    //    private bool DrawFolder(ref string selected, bool returnOnSelection = false)
    //    {
    //        ImGui.Text("Current Folder: " + CurrentFolder);
    //        bool result = false;

    //        if (ImGui.BeginChildFrame(1, new Vector2(0, 600), WindowFlags.Default))
    //        {
    //            DirectoryInfo di = new DirectoryInfo(CurrentFolder);
    //            if (di.Exists)
    //            {
    //                if (di.Parent != null)
    //                {
    //                    ImGui.PushStyleColor(ColorTarget.Text, RgbaFloat.Yellow.ToVector4());
    //                    if (ImGui.Selectable("../", false, SelectableFlags.DontClosePopups))
    //                    {
    //                        CurrentFolder = di.Parent.FullName;
    //                    }
    //                    ImGui.PopStyleColor();
    //                }
    //                foreach (var fse in Directory.EnumerateFileSystemEntries(di.FullName))
    //                {
    //                    if (Directory.Exists(fse))
    //                    {
    //                        string name = Path.GetFileName(fse);
    //                        ImGui.PushStyleColor(ColorTarget.Text, RgbaFloat.Yellow.ToVector4());
    //                        if (ImGui.Selectable(name + "/", false, SelectableFlags.DontClosePopups))
    //                        {
    //                            CurrentFolder = fse;
    //                        }
    //                        ImGui.PopStyleColor();
    //                    }
    //                    else
    //                    {
    //                        string name = Path.GetFileName(fse);
    //                        bool isSelected = SelectedFile == fse;
    //                        if (ImGui.Selectable(name, isSelected, SelectableFlags.DontClosePopups))
    //                        {
    //                            SelectedFile = fse;
    //                            if (returnOnSelection)
    //                            {
    //                                result = true;
    //                                selected = SelectedFile;
    //                            }
    //                        }
    //                        if (ImGui.IsMouseDoubleClicked(0))
    //                        {
    //                            result = true;
    //                            selected = SelectedFile;
    //                            ImGui.CloseCurrentPopup();
    //                        }
    //                    }
    //                }
    //            }

    //        }
    //        ImGui.EndChildFrame();


    //        if (ImGui.Button("Cancel"))
    //        {
    //            result = false;
    //            ImGui.CloseCurrentPopup();
    //        }

    //        if (SelectedFile != null)
    //        {
    //            ImGui.SameLine();
    //            if (ImGui.Button("Open"))
    //            {
    //                result = true;
    //                selected = SelectedFile;
    //                ImGui.CloseCurrentPopup();
    //            }
    //        }

    //        return result;
    //    }
    //}
}

