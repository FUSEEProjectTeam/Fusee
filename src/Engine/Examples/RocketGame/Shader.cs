using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public static class Shader
    {
        public static ShaderEffect GetShaderEffect(RenderContext rc, float4 color)
        {
            EffectPassDeclaration[] epd =
            {
                new EffectPassDeclaration
                {
                    VS = VsSimpleColor,
                    PS = PsSimpleColor,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                }
            };

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "color", Value = color}
            });

            shaderEffect.AttachToContext(rc);

            return shaderEffect;
        }

        public static ShaderEffect GetShaderEffect(RenderContext rc, ITexture iTexture)
        {
            EffectPassDeclaration[] epd =
            {
                new EffectPassDeclaration
                {
                    VS = VsSimpleTexture,
                    PS = PsSimpleTexture,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                }
            };

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "texture1", Value = iTexture} 
            });

            shaderEffect.AttachToContext(rc);

            return shaderEffect;
        }
        public static ShaderEffect GetShaderEffect(RenderContext rc, float4 baseColor, ITexture colorMapTexture, float4 lineColor, float2 lineWidth)
        {
            EffectPassDeclaration[] epd =
            {
                new EffectPassDeclaration
                {
                    VS = VsSimpleToonPass1,
                    PS = PsSimpleToonPass1,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                },
                new EffectPassDeclaration
                {
                    VS = VsSimpleToonPass2,
                    PS = PsSimpleToonPass2,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                }
            };

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "uLineColor", Value = lineColor},
                new EffectParameterDeclaration {Name = "texture1", Value = colorMapTexture},
                new EffectParameterDeclaration {Name = "uLineWidth", Value = lineWidth},
                new EffectParameterDeclaration {Name = "color", Value = baseColor} 
            });

            shaderEffect.AttachToContext(rc);

            return shaderEffect;
        }

        public static ShaderEffect GetShaderEffect(RenderContext rc, ITexture baseTexture, ITexture colorMapTexture, float4 lineColor, float2 lineWidth)
        {
            EffectPassDeclaration[] epd =
            {
                new EffectPassDeclaration
                {
                    VS = VsSimpleToonPass1,
                    PS = PsSimpleToonPass1,
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                },
                new EffectPassDeclaration
                {
                    VS = VsSimpleToonPass2,
                    PS = PsTextureToonPass2,  //The only difference to the previous shader definition
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true
                    }
                }
            };

            var shaderEffect = new ShaderEffect(epd, new[]
            {
                new EffectParameterDeclaration {Name = "uLineColor", Value = lineColor},
                new EffectParameterDeclaration {Name = "texture1", Value = colorMapTexture},
                new EffectParameterDeclaration {Name = "uLineWidth", Value = lineWidth},
                new EffectParameterDeclaration {Name = "texture2", Value = baseTexture} 
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

        private const string VsSimpleToonPass1 = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec4 vColor;
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            uniform vec2 uLineWidth;

            void main()
            {
                vNormal = mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal;
                vNormal = normalize(vNormal);
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) ) + vec4(uLineWidth * vNormal.xy, 0, 0) + vec4(0, 0, 0.06, 0);
                vUV = fuUV;
            }";

        private const string PsSimpleToonPass1 = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform vec4 uLineColor;
            varying vec3 vNormal;

            void main()
            {
                gl_FragColor = uLineColor;
            }";

        private const string VsSimpleToonPass2 = @"
            attribute vec4 fuColor;
            attribute vec3 fuVertex;
            attribute vec3 fuNormal;
            attribute vec2 fuUV;
                    
            varying vec3 vNormal;
            varying vec2 vUV;
        
            uniform mat4 FUSEE_MVP;
            uniform mat4 FUSEE_ITMV;

            void main()
            {
                gl_Position = (FUSEE_MVP * vec4(fuVertex, 1.0) );
                vNormal = normalize(mat3(FUSEE_ITMV[0].xyz, FUSEE_ITMV[1].xyz, FUSEE_ITMV[2].xyz) * fuNormal);
                vUV = fuUV;
            }";

        private const string PsSimpleToonPass2 = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D texture1;
            uniform vec4 color;

            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                gl_FragColor = vec4(texture2D(texture1, vNormal.xy * 0.5 + vec2(0.5, 0.5)).rgb * color.rgb, 0.85);
            }";

        private const string PsTextureToonPass2 = @"
            #ifdef GL_ES
                precision highp float;
            #endif
        
            uniform sampler2D texture1;
            uniform sampler2D texture2;

            varying vec3 vNormal;
            varying vec2 vUV;

            void main()
            {
                gl_FragColor = vec4(texture2D(texture1, vNormal.xy * 0.5 + vec2(0.5, 0.5)).rgb * texture2D(texture2, vUV).rgb, 0.85);
            }";
    };

}