using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public static class Shader
    {
        public static ShaderEffect GetShaderEffect(RenderContext rc, float4 color)
        {
            EffectPassDeclaration[] epd = {new EffectPassDeclaration
                                        {
                                            VS = VsSimpleColor,
                                            PS = PsSimpleColor,
                                            StateSet = new RenderStateSet
                                                       {
                                                           AlphaBlendEnable = false,
                                                           ZEnable = true
                                                       }
                                        }};

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "color", Value = color}
            });

            shaderEffect.AttachToContext(rc);

            return shaderEffect;
        }

        public static ShaderEffect GetShaderEffect(RenderContext rc, ITexture iTexture)
        {
            EffectPassDeclaration[] epd = {new EffectPassDeclaration
                                        {
                                            VS = VsSimpleTexture,
                                            PS = PsSimpleTexture,
                                            StateSet = new RenderStateSet
                                                       {
                                                           AlphaBlendEnable = false,
                                                           ZEnable = true
                                                       }
                                        }};

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "texture1", Value = iTexture} 
            });

            shaderEffect.AttachToContext(rc);

            return shaderEffect;
        }

        private const string VsSimpleColor = @"
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;       
        
            varying vec3 vNormal;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
            }";

        private const string PsSimpleColor = @"
            #ifdef GL_ES
                precision highp float;
            #endif    
  
            uniform vec4 color;
            varying vec3 vNormal;

            void main()
            {             
                gl_FragColor = max(dot(vec3(0,0,1),normalize(vNormal)), 0.1) * color;
            }";

        private const string VsSimpleTexture = @"
            #ifdef GL_ES
                precision mediump float;
            #endif

            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;

            varying vec3 vNormal;
            varying vec2 vUV;

            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main(){
                vUV = fuUV;
                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
            }";

        private const string PsSimpleTexture = @"
            #ifdef GL_ES
                precision mediump float;
            #endif

            uniform sampler2D texture1;
            varying vec3 vNormal;
            varying vec2 vUV;

            void main(){
                gl_FragColor = max(dot(vec3(0,0,1),normalize(vNormal)), 0.2) * texture2D(texture1, vUV);
            }";

    };

}

