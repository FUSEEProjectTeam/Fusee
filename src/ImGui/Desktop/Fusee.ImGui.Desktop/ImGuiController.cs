using Fusee.Base.Core;
using Fusee.Math.Core;
using ImGuiNET;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Fusee.ImGuiDesktop
{
    public class ImGuiController
    {
        private static VertexArrayHandle _vertexArray;
        private static BufferHandle _vertexBuffer;
        private static int _vertexBufferSize;
        private static BufferHandle _indexBuffer;
        private static int _indexBufferSize;

        public int GameWindowWidth;
        public int GameWindowHeight;

        private static Vector2 _scaleFactor = Vector2.One;

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

        private static ProgramHandle _shaderProgram;
        private static readonly Dictionary<string, UniformFieldInfo> _uniformVarToLocation = new();

        public ImGuiController(int width, int height) => WindowResized(width, height);

        public void WindowResized(int width, int height)
        {
            GL.Viewport(0, 0, GameWindowWidth, GameWindowHeight);
            (GameWindowWidth, GameWindowHeight) = (width, height);
        }

        /// <summary>
        /// Init ImGui controller, set font size in px and font texture here
        /// </summary>
        /// <param name="fontSize">size in px</param>
        /// <param name="pathToFontTexture">path to texture (e. g. "Assets/Lato-Black.ttf")</param>
        public void InitImGUI(int fontSize = 14, string pathToFontTexture = "")
        {
            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            var io = ImGui.GetIO();
            if (pathToFontTexture != string.Empty)
            {
                io.Fonts.AddFontFromFileTTF(pathToFontTexture, fontSize);
            }
            else
            {
                io.Fonts.AddFontDefault();
            }

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

            CreateDeviceResources();
            SetPerFrameImGuiData(1f / 60f);
            ImGuiInputImp.InitImGuiInput();

            // TODO(mr): Let user decide
            if (File.Exists("Assets/ImGuiSettings.ini"))
                ImGui.LoadIniSettingsFromDisk("Assets/ImGuiSettings.ini");
        }

        private static void CreateDeviceResources()
        {
            _shaderProgram = GL.CreateProgram();

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, _vertexSource);
            GL.CompileShader(vertexShader);

            int success = -1;
            GL.GetShaderi(vertexShader, ShaderParameterName.CompileStatus, ref success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(vertexShader, out var info);
                Diagnostics.Error($"GL.CompileShader for shader '{vertexShader}' had info log:\n{info}");
            }

            var pixelShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShader, _fragmentSource);
            GL.CompileShader(pixelShader);

            GL.GetShaderi(pixelShader, ShaderParameterName.CompileStatus, ref success);
            if (success == 0)
            {
                GL.GetShaderInfoLog(pixelShader, out var info);
                Diagnostics.Error($"GL.CompileShader for shader '{pixelShader}' had info log:\n{info}");
            }

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, pixelShader);

            GL.LinkProgram(_shaderProgram);

            GL.GetProgrami(_shaderProgram, ProgramPropertyARB.LinkStatus, ref success);

            if (success == 0)
            {
                GL.GetProgramInfoLog(_shaderProgram, out var info);
                Diagnostics.Error($"GL.LinkProgram had info log:\n{info}");
            }

            GL.DetachShader(_shaderProgram, vertexShader);
            GL.DetachShader(_shaderProgram, pixelShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(pixelShader);

            int count = -1;
            GL.GetProgrami(_shaderProgram, ProgramPropertyARB.ActiveUniforms, ref count);

            for (uint i = 0; i < count; i++)
            {
                UniformType type = UniformType.Int;
                var size = 0;
                var length = 0;

                GL.GetActiveUniform(_shaderProgram, i, (int)ProgramInterfacePName.MaxNameLength, ref length, ref size, ref type, out var name);

                UniformFieldInfo FieldInfo;
                FieldInfo.Location = GL.GetUniformLocation(_shaderProgram, name);
                FieldInfo.Name = name;
                FieldInfo.Size = size;
                FieldInfo.Type = type;

                _uniformVarToLocation.Add(name, FieldInfo);
            }

            _vertexArray = GL.CreateVertexArray();

            _vertexBufferSize = 10000;
            _indexBufferSize = 2000;


            _vertexBuffer = GL.CreateBuffer();
            _indexBuffer = GL.CreateBuffer();

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
            GL.BufferData(BufferTargetARB.ArrayBuffer, _vertexBufferSize, IntPtr.Zero, BufferUsageARB.DynamicDraw);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTargetARB.ArrayBuffer, _indexBufferSize, IntPtr.Zero, BufferUsageARB.DynamicDraw);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(0));

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

        }

        private static unsafe void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            TextureHandle handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2d, handle);

            GL.TexImage2D(TextureTarget.Texture2d, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapR, (int)OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge);

            io.Fonts.SetTexID(new IntPtr(handle.Handle));

            io.Fonts.ClearTexData();
        }

        private void SetPerFrameImGuiData(float deltaSeconds)
        {

            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                GameWindowWidth / _scaleFactor.X,
                GameWindowHeight / _scaleFactor.Y);
            io.DisplayFramebufferScale = _scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        public void UpdateImGui(float DeltaTimeUpdate)
        {
            SetPerFrameImGuiData(DeltaTimeUpdate);
            ImGuiInputImp.UpdateImGuiInput();

            ImGui.NewFrame();
        }



        public void RenderImGui()
        {
            ImGui.Render();
            RenderImDrawData(ImGui.GetDrawData());
        }

        internal unsafe void RenderImDrawData(ImDrawDataPtr draw_data)
        {
            if (draw_data.NativePtr == IntPtr.Zero.ToPointer()) return;

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

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
                GL.BufferData(BufferTargetARB.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageARB.DynamicDraw);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(0));

                _vertexBufferSize = newSize;

                Diagnostics.Debug($"Resized vertex buffer to new size {_vertexBufferSize}");
            }

            uint totalIBSize = (uint)(draw_data.TotalIdxCount * sizeof(ushort));
            if (totalIBSize > _indexBufferSize)
            {
                int newSize = (int)M.Max(_indexBufferSize * 1.5f, totalIBSize);

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, _indexBuffer);
                GL.BufferData(BufferTargetARB.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageARB.DynamicDraw);
                GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(0));

                _indexBufferSize = newSize;

                Diagnostics.Debug($"Resized index buffer to new size {_indexBufferSize}");
            }


            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                var cmd_list = draw_data.CmdListsRange[i];

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vertexBuffer);
                GL.BufferSubData(BufferTargetARB.ArrayBuffer, (IntPtr)(vertexOffsetInVertices * Unsafe.SizeOf<ImDrawVert>()), cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);

                GL.BindBuffer(BufferTargetARB.ArrayBuffer, _indexBuffer);
                GL.BufferSubData(BufferTargetARB.ArrayBuffer, (IntPtr)(indexOffsetInElements * sizeof(ushort)), cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);

                vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
                indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
            }

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, new BufferHandle(0));

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
            GL.UniformMatrix4f(_uniformVarToLocation["projection_matrix"].Location, false, in mvp);
            GL.Uniform1i(_uniformVarToLocation["in_fontTexture"].Location, 0);

            GL.BindVertexArray(_vertexArray);

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationModeEXT.FuncAdd);
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
                        GL.BindTexture(TextureTarget.Texture2d, new TextureHandle((int)pcmd.TextureId));

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, GameWindowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

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