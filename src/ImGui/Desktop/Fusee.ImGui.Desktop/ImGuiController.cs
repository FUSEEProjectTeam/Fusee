using Fusee.Base.Core;

using Fusee.Math.Core;
using ImGuiNET;
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
        private static int _vertexArray;
        private static int _vertexBuffer;
        private static int _vertexBufferSize;
        private static int _indexBuffer;
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

        internal static int ShaderProgram;
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
            ShaderProgram = GL.CreateProgram();

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

            GL.AttachShader(ShaderProgram, vertexShader);
            GL.AttachShader(ShaderProgram, pixelShader);

            GL.LinkProgram(ShaderProgram);

            GL.GetProgram(ShaderProgram, GetProgramParameterName.LinkStatus, out success);

            if (success == 0)
            {
                string Info = GL.GetProgramInfoLog(ShaderProgram);
                Diagnostics.Error($"GL.LinkProgram had info log:\n{Info}");
            }

            GL.DetachShader(ShaderProgram, vertexShader);
            GL.DetachShader(ShaderProgram, pixelShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(pixelShader);

            GL.GetProgram(ShaderProgram, GetProgramParameterName.ActiveUniforms, out int cnt);

            for (var i = 0; i < cnt; i++)
            {
                var name = GL.GetActiveUniform(ShaderProgram, i, out int Size, out ActiveUniformType Type);

                UniformFieldInfo FieldInfo;
                FieldInfo.Location = GL.GetUniformLocation(ShaderProgram, name);
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

            GL.UseProgram(ShaderProgram);

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