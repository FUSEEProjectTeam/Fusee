using Fusee.Engine;
using Fusee.Math;

namespace Examples.FuseeExampleApp1
{
    public class FuseeExampleApp1 : RenderCanvas
    {

        protected string Vs = @"
             #version 120

            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec2 fuUV;

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            
            

            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                vUV = fuUV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                // vColor = vec4(fuNormal * 0.5 + 0.5, 1.0);
                // vec4 norm4 = FUSEE_MVP * vec4(fuNormal, 0.0);
                // vNormal = norm4.xyz;
                vNormal = mat3(FUSEE_ITMV) * fuNormal;
            }";

        protected string Ps = @"
             #version 120

            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 vColor;
            varying vec3 vNormal;

            //Variablen für Texturen im Pixelshader
            uniform sampler2D texture_planet;
            varying vec2 vUV;
                        


            void main()
            {
                

                gl_FragColor = texture2D(texture_planet, vUV) ; //vColor * dot(vNormal, vec3(0, 0, 1));
            }";
        public override void Init()
        {
            // is called on startup
        }

        public override void RenderAFrame()
        {
            // is called once a frame
        }

        public override void Resize()
        {
            // is called when the window is resized
            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width / (float)Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);
        }

        public static void Main()
        {
            var app = new FuseeExampleApp1();
            app.Run();
        }

    }
}
