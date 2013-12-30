using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.RocketGame
{
    public class Shader
    {
        protected static string VsSimpleTexture = @"
            /* Copies incoming vertex color without change.
             * Applies the transformation matrix to vertex position.
             */

            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;

            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vUV = fuUV;
            }";

        protected static string PsSimpleTexture = @"
            /* Copies incoming fragment color without change. */
            #ifdef GL_ES
                precision highp float;
            #endif

            // The parameter required for the texturing process
            uniform sampler2D texture1;
            uniform vec4 vColor;
            varying vec3 vNormal;

            // The parameter holding the UV-Coordinates of the texture
            varying vec2 vUV;

            void main()
            {    
              // The most basic texturing function, expecting the above mentioned parameters   
              gl_FragColor = texture2D(texture1, vUV);        
            }";

        public static string GetVsSimpleTextureShader()
        {
            return VsSimpleTexture;
        }
        public static string GetPsSimpleTextureShader()
        {
            return PsSimpleTexture;
        }
    }
}
