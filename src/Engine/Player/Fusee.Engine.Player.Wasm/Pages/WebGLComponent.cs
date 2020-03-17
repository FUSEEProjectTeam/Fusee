using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.WebGL;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace Fusee.Engine.Player.Wasm.Pages
{


    public class WebGLComponent : ComponentBase
    {

        private IJSRuntime _js;

        [Inject]
        public IJSRuntime JS { get => _js;
            set {
                _js = value;
                
            }
        }
         

        public int Width { get; set; }

        public int Height { get; set; }

        private WebGLContext _context;

        protected BECanvasComponent _canvasReference;

        private const string VS_SOURCE = "attribute vec3 aPos;" +
                                         "attribute vec3 aColor;" +
                                         "varying vec3 vColor;" +

                                         "void main() {" +
                                            "gl_Position = vec4(aPos, 1.0);" +
                                            "vColor = aColor;" +
                                         "}";

        private const string FS_SOURCE = "precision mediump float;" +
                                         "varying vec3 vColor;" +

                                         "void main() {" +
                                            "gl_FragColor = vec4(vColor, 1.0);" +
                                         "}";


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Console.WriteLine("Render called");
           
            _context = await _canvasReference.CreateWebGLAsync(new WebGLContextAttributes
            {
                PowerPreference = WebGLContextAttributes.POWER_PREFERENCE_HIGH_PERFORMANCE
            });

            await _context.ClearColorAsync(0, 0, 0, 1);
            await _context.ClearAsync(BufferBits.COLOR_BUFFER_BIT);

            var program = await InitProgramAsync(_context, VS_SOURCE, FS_SOURCE);

            var vertexBuffer = await _context.CreateBufferAsync();
            await _context.BindBufferAsync(BufferType.ARRAY_BUFFER, vertexBuffer);

            var vertices = new[]
            {
                -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
                0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
                0.0f,  0.5f, 0.0f, 0.0f, 0.0f, 1.0f
            };
            await _context.BufferDataAsync(BufferType.ARRAY_BUFFER, vertices, BufferUsageHint.STATIC_DRAW);

            await _context.VertexAttribPointerAsync(0, 3, DataType.FLOAT, false, 6 * sizeof(float), 0);
            await _context.VertexAttribPointerAsync(1, 3, DataType.FLOAT, false, 6 * sizeof(float), 3 * sizeof(float));
            await _context.EnableVertexAttribArrayAsync(0);
            await _context.EnableVertexAttribArrayAsync(1);

            await _context.UseProgramAsync(program);
            await _context.DrawArraysAsync(Primitive.TRIANGLES, 0, 3);
        }

        private void Loop()
        {

        }

        private async Task<WebGLProgram> InitProgramAsync(WebGLContext gl, string vsSource, string fsSource)
        {
            var vertexShader = await LoadShaderAsync(gl, ShaderType.VERTEX_SHADER, vsSource);
            var fragmentShader = await LoadShaderAsync(gl, ShaderType.FRAGMENT_SHADER, fsSource);

            var program = await gl.CreateProgramAsync();
            await gl.AttachShaderAsync(program, vertexShader);
            await gl.AttachShaderAsync(program, fragmentShader);
            await gl.LinkProgramAsync(program);

            await gl.DeleteShaderAsync(vertexShader);
            await gl.DeleteShaderAsync(fragmentShader);

            if (!await gl.GetProgramParameterAsync<bool>(program, ProgramParameter.LINK_STATUS))
            {
                string info = await gl.GetProgramInfoLogAsync(program);
                throw new Exception("An error occured while linking the program: " + info);
            }

            return program;
        }

        private async Task<WebGLShader> LoadShaderAsync(WebGLContext gl, ShaderType type, string source)
        {
            var shader = await gl.CreateShaderAsync(type);

            await gl.ShaderSourceAsync(shader, source);
            await gl.CompileShaderAsync(shader);

            if (!await gl.GetShaderParameterAsync<bool>(shader, ShaderParameter.COMPILE_STATUS))
            {
                string info = await gl.GetShaderInfoLogAsync(shader);
                await gl.DeleteShaderAsync(shader);
                throw new Exception("An error occured while compiling the shader: " + info);
            }
            return shader;
        }
    }
}
