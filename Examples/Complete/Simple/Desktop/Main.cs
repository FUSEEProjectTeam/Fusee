using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Base.Imp.Desktop;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.Math.Core;
using Fusee.Serialization;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        private static int _vertexArray;
        private static int _vertexBuffer;
        private static int _vertexBufferSize;
        private static int _indexBuffer;
        private static int _indexBufferSize;

        private static int _windowWidth;
        private static int _windowHeight;

        private static bool _frameBegun;

        private static System.Numerics.Vector2 _scaleFactor = System.Numerics.Vector2.One;

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
        private static Dictionary<string, UniformFieldInfo> _uniformVarToLocation = new();

        struct UniformFieldInfo
        {
            public int Location;
            public string Name;
            public int Size;
            public ActiveUniformType Type;
        }


        private static void InitImGUI(object sender, InitEventArgs args)
        {
            _windowWidth = _renderCanvas._gameWindow.Size.X;
            _windowHeight = _renderCanvas._gameWindow.Size.Y;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            var io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.BackendFlags |= ImGuiBackendFlags.RendererHasViewports;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            CreateDeviceResources();

            SetPerFrameImGuiData(1f / 60f);

            ImGui.NewFrame();
            _frameBegun = true;

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

            GL.CreateFramebuffers(1, out _viewportFB);            // The texture we're going to render to
            GL.GenTextures(1, out _renderTexture);
            GL.GenRenderbuffers(1, out _depthRenderbuffer);
        }

        private static int _viewportFB;
        private static int _renderTexture;
        private static int _depthRenderbuffer;
        private static float2 _viewPortRes = new float2(3840, 2160);

        private unsafe static void RecreateFontDeviceTexture()
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
            SetPerFrameImGuiData(_renderCanvas.DeltaTimeUpdate);
            UpdateImGuiInput();

            _frameBegun = true;
            ImGui.NewFrame();
        }

        private static bool _viewportOpen = true;

        private static void UpdateRenderTexture(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _viewportFB);
            // "Bind" the newly created texture : all future texture functions will modify this texture
            GL.BindTexture(TextureTarget.Texture2D, _renderTexture);

            // Give an empty image to OpenGL ( the last "0" )
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Bgr, PixelType.UnsignedByte, new IntPtr());

            // Poor filtering. Needed !
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRenderbuffer);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, _renderTexture, 0);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == 0)
            {
                throw new Exception("Error Framebuffer!");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        static System.Numerics.Vector2 _min = new System.Numerics.Vector2(0, 0);
        static System.Numerics.Vector2 _max = new System.Numerics.Vector2(0, 0);
        static System.Numerics.Vector2 _size = new System.Numerics.Vector2(0, 0);
        static System.Numerics.Vector2 _pos = new System.Numerics.Vector2(0, 0);

        private static void RenderImGUI(object sender, RenderEventArgs args)
        {
            if (!app.IsLoaded) return;

            if (app.IsShuttingDown) return;

            Input.Instance.PreRender();
            Time.Instance.DeltaTimeIncrement = _renderCanvas.DeltaTime;

            if (_size.X != 0)
            {
                // pre-rendering

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                // Enable FB
                UpdateRenderTexture((int)_size.X, (int)_size.Y);
                // app.RC.Viewport((int)_pos.X, (int)(((_windowHeight - _size.Y)) - _pos.Y), (int)_size.X, (int)_size.Y);
                app.RC.Viewport(0, 0, (int)_size.X, (int)_size.Y);

                // rendering
                app.RenderAFrame();
            }

            // Disable FB
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, _windowWidth, _windowHeight);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            //////////////// --- IMGUI GOES HERE --- //////////////


            ImGui.Begin("Viewport", ref _viewportOpen,
                ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.DockNodeHost);

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

            GL.BindTexture(TextureTarget.Texture2D, _renderTexture);
            ImGui.Image(new IntPtr(_renderTexture), _size,
                new System.Numerics.Vector2(0, 1),
                new System.Numerics.Vector2(1, 0));

            ImGui.CaptureMouseFromApp();

            ImGui.EndChild();

            ImGui.End();

            ImGui.ShowDemoWindow();


            if (_frameBegun)
            {
                _frameBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            app.Present();

            Input.Instance.PostRender();
        }

        private unsafe static void RenderImDrawData(ImDrawDataPtr draw_data)
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
                ImDrawListPtr cmd_list = draw_data.CmdListsRange[i];

                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(vertexOffsetInVertices * Unsafe.SizeOf<ImDrawVert>()), cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _indexBuffer);
                GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)(indexOffsetInElements * sizeof(ushort)), cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
                indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
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
}