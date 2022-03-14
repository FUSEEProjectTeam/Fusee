using Fusee.Base.Core;
using Fusee.DImGui.Desktop.Gizmos;
using Fusee.Engine.Core;
using Fusee.Examples.Simple.Core;
using Fusee.Math.Core;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Fusee.DImGui.Desktop
{
    internal class ImGuiController
    {
        private static int _vertexArray;
        private static int _vertexBuffer;
        private static int _vertexBufferSize;
        private static int _indexBuffer;
        private static int _indexBufferSize;

        private static int _gameWindowWidth;
        private static int _gameWindowHeight;

        private static bool _dockspaceOpen;

        private static Vector2 _scaleFactor = System.Numerics.Vector2.One;

        private static readonly string _vertexSource = @"#version 330 core
                                                uniform mat4 projection_matrix;
                                                layout(location = 0) in vec2 in_model;
                                                layout(location = 1) in vec2 in_texCoord;
                                                layout(location = 2) in vec4 in_color;
                                                out vec4 color;
                                                out vec2 texCoord;
                                                void main()
                                                {
                                                    gl_Position = projection_matrix * vec4(in_model, 0, 1);
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

        private static int _depthRenderbuffer;
        private static int _texture4ImGui;

        public int ViewportFramebuffer { get; private set; }
        public int IntermediateFrameBuffer { get; private set; }
        public int ViewportRenderTexture { get; private set; }


        public Vector2 FuseeViewportMin { get; private set; } = new(0, 0);
        public Vector2 FuseeViewportMax { get; private set; } = new(0, 0);
        public Vector2 FuseeViewportSize { get; private set; } = new(0, 0);
        public Vector2 FuseeViewportPos { get; private set; } = new(0, 0);

        private static GameWindow? _gameWindow;

        internal ImGuiController(GameWindow gw) => (_gameWindow) = (gw);

        public void InitImGUI()
        {
            _gameWindowWidth = _gameWindow.Size.X;
            _gameWindowHeight = _gameWindow.Size.Y;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            //io.Fonts.AddFontFromFileTTF("Assets/Lato-Black.ttf", 14);
            io.Fonts.AddFontDefault();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            FuseeGUI.SetImGuiDesign();

            CreateDeviceResources();
            SetPerFrameImGuiData(1f / 60f);

        }

        private void CreateDeviceResources()
        {
            _shaderProgram = GL.CreateProgram();

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, _vertexSource);
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string Info = GL.GetShaderInfoLog(vertexShader);
                Diagnostics.Error($"GL.CompileShader for shader '{vertexShader}' had info log:\n{Info}");
            }

            var pixelShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShader, _fragmentSource);
            GL.CompileShader(pixelShader);

            GL.GetShader(pixelShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string Info = GL.GetShaderInfoLog(pixelShader);
                Diagnostics.Error($"GL.CompileShader for shader '{pixelShader}' had info log:\n{Info}");
            }

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, pixelShader);

            GL.LinkProgram(_shaderProgram);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out success);

            if (success == 0)
            {
                string Info = GL.GetProgramInfoLog(_shaderProgram);
                Diagnostics.Error($"GL.LinkProgram had info log:\n{Info}");
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
            GL.CreateFramebuffers(1, out int fb);
            ViewportFramebuffer = fb;
            GL.GenTextures(1, out int renderTex);
            ViewportRenderTexture = renderTex;
            GL.GenRenderbuffers(1, out _depthRenderbuffer);

            // Set up texture to blt to
            _texture4ImGui = GL.GenTexture();
            GL.CreateFramebuffers(1, out int iFb);
            IntermediateFrameBuffer = iFb;
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

        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            _gameWindowHeight = _gameWindow.Size.Y;
            _gameWindowWidth = _gameWindow.Size.X;
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                _gameWindowWidth / _scaleFactor.X,
                _gameWindowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        public void UpdateImGui(float DeltaTimeUpdate)
        {
            SetPerFrameImGuiData(DeltaTimeUpdate);
            UpdateImGuiInput();

            ImGui.NewFrame();
        }

        private static void UpdateImGuiInput()
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseDown[0] = Input.Mouse.LeftButton;
            io.MouseDown[1] = Input.Mouse.MiddleButton;
            io.MouseDown[2] = Input.Mouse.RightButton;

            io.MousePos = new Vector2(Input.Mouse.X, Input.Mouse.Y);

            io.MouseWheel = Input.Mouse.Wheel;
            io.MouseWheelH = 0;
        }


        private float[] snap = new float[] { 1, 1, 1, 1 };
        private float[] bounds = new float[] { -0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f };
        private float[] boundsSnap = new float[] { 0.1f, 0.1f, 0.1f, 0.1f };

        private float4x4 resMat = float4x4.Identity;
        private float4x4 deltaMat = float4x4.Identity;

        private bool useSnap = false;
        private bool boundSizing = false;
        private bool boundSizingSnap = false;

        public void RenderImGui()
        {
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
            FuseeGUI.DrawMainMenuBar();

            ImGui.Begin("Viewport",
                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoCollapse);


            var parentMin = ImGui.GetWindowContentRegionMin();
            var parentMax = ImGui.GetWindowContentRegionMax();
            var size = parentMax - parentMin;

            // Using a Child allow to fill all the space of the window.
            // It also allows customization
            ImGui.BeginChild("GameRender", size, true, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove);

            var rc = Simple.RenderContext;
            var pos = ImGui.GetWindowPos();
            Grid.DrawGrid(rc.View, rc.Projection, float4x4.Identity, 100f, size, pos);


            // var viewManipulateRight = ImGui.GetWindowPos().X + size.X - 128;
            // var viewManipulateTop = ImGui.GetWindowPos().Y;

            //ViewManipulateCube.DrawManipulateCube(rc.View, rc.Projection.M22,
            //    new Vector2(viewManipulateRight, viewManipulateTop),
            //    new Vector2(128, 128), 0x10101010);



            MODE mCurrentGizmoMode = MODE.LOCAL;


            Gizmos.Gizmos.SetRect(pos.X, pos.Y, size.X, size.Y);



            if (Simple._rocketScene != null)
            {
                var aabb = new AABBCalculator(Simple._rocketScene);

                var min = aabb.GetBox().Value.min;
                var max = aabb.GetBox().Value.max;


                bounds = new float[] { min.x, min.y, min.z, max.x, max.y, max.z };
            }

            Manipulate.DrawManipulate(rc.View, rc.Projection, OPERATION.TRANSLATE, mCurrentGizmoMode, ref resMat,
               ref deltaMat, ref snap, ref bounds, ref boundsSnap);


            //if (MathF.Abs(resMat.Determinant) > float.Epsilon)
            //    rc.Model = resMat;

            FuseeViewportMin = ImGui.GetWindowContentRegionMin();
            FuseeViewportMax = ImGui.GetWindowContentRegionMax();
            FuseeViewportSize = FuseeViewportMax - FuseeViewportMin;
            FuseeViewportPos = ImGui.GetWindowPos();

            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            ImGui.Image(new IntPtr(_texture4ImGui), FuseeViewportSize,
                new Vector2(0, 1),
                new Vector2(1, 0));



            ImGui.EndChild();
            ImGui.End();

            FuseeGUI.DrawGUI();

            ImGui.Render();

            RenderImDrawData(ImGui.GetDrawData());

            _gameWindow?.SwapBuffers();
        }

        public void UpdateRenderTexture(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, IntermediateFrameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, _texture4ImGui);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, new IntPtr());
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _texture4ImGui, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Enable(EnableCap.Multisample);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ViewportFramebuffer);
            GL.BindTexture(TextureTarget.Texture2DMultisample, ViewportRenderTexture);

            GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, 4, PixelInternalFormat.Rgba, width, height, true);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, 4, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depthRenderbuffer);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2DMultisample, ViewportRenderTexture, 0);

            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) == 0)
            {
                throw new Exception("Error Framebuffer!");
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        internal static unsafe void RenderImDrawData(ImDrawDataPtr draw_data)
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

                Diagnostics.Debug($"Resized vertex buffer to new size {_vertexBufferSize}");
            }

            uint totalIBSize = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > _indexBufferSize)
            {
                int newSize = (int)M.Max(_indexBufferSize * 1.5f, totalIBSize);

                GL.BindBuffer(BufferTarget.ArrayBuffer, _indexBuffer);
                GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                _indexBufferSize = newSize;

                Diagnostics.Debug($"Resized index buffer to new size {_indexBufferSize}");
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

            // Column order notation
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
                        GL.Scissor((int)clip.X, _gameWindowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

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

            draw_data.Clear();
        }


    }
}
