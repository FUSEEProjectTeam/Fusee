﻿
using Fusee.Engine.Common;
using Fusee.Engine.Imp.Graphics.Web;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    public static class DeferredShaderHelper
    {
        public static int CurrentRenderPass;

        public static ShaderEffect ShadowPassShaderEffect = null;
        public static ShaderEffect GBufferPassShaderEffect = null;
        public static ShaderEffect GBufferDrawPassShaderEffect = null;
        public static ITexture ShadowTexture = null;
        public static ITexture GBufferTexture = null;

        public static Mesh Quad = new FullscreenQuad();

        // ReSharper disable once InconsistentNaming
        public static float4x4 ShadowMapMVP { private set; get; } = float4x4.Identity;

        // ReSharper disable once InconsistentNaming
        public static void SetShadowMapMVP(float3 lightPosition, float3 coneDirection, float sceneScale, float4x4 currentView)
        {
            var lightPos = lightPosition;
            var lightCone = coneDirection;
            lightCone.Normalize();

            var target = lightCone;
            var depthViewMatrix = float4x4.LookAt(lightPos.x, lightPos.y, lightPos.z, target.x, target.y, target.z, 0, 1, 0);
            var scale = float4x4.CreateScale(sceneScale);
            var depthModelMatrix = scale;
            var projection = float4x4.CreateOrthographic(50, 50, -40f, 50f);
            var lightModelView = depthViewMatrix * depthModelMatrix;
            var invView = float4x4.Invert(currentView); // this is nescessary because we are calculating everything in ModelView Space 
                                                        // and with additional view, the shadow would move with camera!
            ShadowMapMVP = projection * lightModelView * invView;
        }

        public static string GlslVersion()
        {
           // return "#version 100";
            return "";
        }

        public static string OrtographicShadowMapMvVertexShader()
        {
            return GlslVersion() + @"
                attribute vec3 fuVertex;

                uniform mat4 LightMVP;
                uniform mat4 FUSEE_MV;

                void main()
                {
                    vec4 fuVertexMVSpace = FUSEE_MV * vec4(fuVertex,1.0);
                    gl_Position = LightMVP * fuVertexMVSpace;
                }";
        }

        public static string OrtographicShadowMapMvPixelShader()
        {
            return GlslVersion() + @"
            void main()
            {  
               // This is not necessary, only for debugging purposes                                            
               gl_FragColor = gl_FragCoord;
            }";
        }

        // TODO: Add SpecularIntensity & -Color into own texture
        public static string DeferredPassVertexShader()
        {
            return GlslVersion() + @"
                
                attribute vec3 fuVertex;
                attribute vec3 fuNormal;
                attribute vec2 fuUV;

                uniform mat4 FUSEE_MVP;
                uniform mat4 FUSEE_ITMV;
                uniform mat4 FUSEE_MV;
                uniform mat4 FUSEE_IMV;
                
                varying vec2 uv;
                varying vec3 normal;
                varying vec3 surfacePos;
                varying vec3 vViewDir;

                void main()
                {
                    normal =  normalize(mat3(FUSEE_ITMV) * fuNormal);
	                uv = fuUV;

                    vec3 viewPos = FUSEE_IMV[3].xyz;
                    vViewDir = fuVertex;

	                gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                    surfacePos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;

                }";
        }

        public static string DeferredPassPixelShader()
        {
            return GlslVersion() + @"
                #ifdef GL_ES
                    precision highp float
                #endif      
                   
                varying vec2 uv;
                varying vec3 normal;
                varying vec3 surfacePos;
                varying vec3 vViewDir;
       
                uniform vec3 DiffuseColor;
                uniform vec3 SpecularIntensity;

                uniform mat4 FUSEE_IMV;
                
            void main()
            {                                              
                // Store the fragment position vector in the first gbuffer texture
                gl_FragData[0] = vec4(surfacePos,1.0);
                // Also store the per-fragment normals into the gbuffer
                gl_FragData[1] = vec4(normal,1.0);
                // And the diffuse per-fragment color   
                // Store specular intensity in gAlbedoSpec's alpha component                         
                gl_FragData[2] = vec4(DiffuseColor, 1.0);
                gl_FragData[3] = vec4(vViewDir, 1.0);       

          }";
        }

        public static string DeferredDrawPassVertexShader()
        {
            return GlslVersion() + @"

                attribute vec3 fuVertex;
                attribute vec3 fuNormal;
                attribute vec2 fuUV;                
                
                varying vec2 uv; 

                uniform mat4 FUSEE_IMV;
                varying vec3 viewPos;

                void main()
                {
                    viewPos = FUSEE_IMV[3].xyz;
	                gl_Position = vec4(fuVertex, 1.0);
                    uv = fuUV;
                }";
        }

      
        public static string DeferredDrawPassPixelShader()
        {
            return GlslVersion() + @"
                #ifdef GL_ES
                    precision highp float
                #endif       
                
                varying vec2 uv;
                varying vec3 viewPos;
                
                uniform sampler2D gPosition;
                uniform sampler2D gNormal;
                uniform sampler2D gAlbedoSpec;
                uniform sampler2D gDepth;
                uniform sampler2D gViewDir;

                uniform vec3 lightPosition;


                vec3 diffuseLighting(vec3 N, vec3 L, vec3 color) {
                    float diffuseTerm = clamp(dot(N, L) / (length(L) * length(N)), 0.0, 1.0);
                    return (color * diffuseTerm);
                }
                
            vec3 specularLighting(vec3 N, vec3 L, vec3 V) {
                float specularTerm = 0.0;
                if(dot(N, L) > 0.0) {
                    vec3 H = normalize(L + V);
                    specularTerm = max(0.0, pow(dot(N, H), 2.0));
                }
               return (vec3(0.5,0.5,0.5));
            }
                

            void main()
            { 
                vec3 surfacePos = texture2D(gPosition, uv).xyz;
                vec3 normal = texture2D(gNormal, uv).xyz;
                vec3 albedo = texture2D(gAlbedoSpec, uv).xyz;
                float specularIntensity = texture2D(gPosition, uv).a;                 
                vec3 vViewDir = texture2D(gViewDir, uv).xyz;
                
                vViewDir = normalize(vViewDir - viewPos);

                vec3 L = normalize(lightPosition - surfacePos.xyz); 
                vec3 N = normal;
                vec3 V = normalize(vViewDir - surfacePos.xyz);

                vec3 result = vec3(0);

                result = diffuseLighting(N,L,albedo);
                //result = specularLighting(N, L, V);
                gl_FragColor = vec4(surfacePos, 1);

            }";
        }

   
        public static Mesh DeferredFullscreenQuad()
        {
            return Quad;
        }
    }

    internal class FullscreenQuad : Mesh
    {
        // This Mesh is a quad on screen and is used for displaying the second aka "normal" Renderpath during deferred rendering:
        public FullscreenQuad()
        {
            Vertices = new[]
            {
                // left, down, front vertex
                new float3(-1, -1, 0), // 0
                new float3(1, -1, 0), // 1
                new float3(1, 1, 0), // 2
                new float3(-1, 1, 0), // 3
            };
            Normals = new[]
            {
                // left, down, front vertex
                new float3(-1, 0, 0), // 0  - belongs to left
                new float3(0, -1, 0), // 1  - belongs to down
                new float3(0, 0, -1), // 2  - belongs to front

                // left, up, front vertex
                new float3(-1, 0, 0), // 6  - belongs to left
                new float3(0, 1, 0), // 7  - belongs to up
                new float3(0, 0, -1), // 8  - belongs to front

                // right, down, front vertex
                new float3(1, 0, 0), // 12 - belongs to right
                new float3(0, -1, 0), // 13 - belongs to down
                new float3(0, 0, -1), // 14 - belongs to front

                // right, up, front vertex
                new float3(1, 0, 0), // 18 - belongs to right
                new float3(0, 1, 0), // 19 - belongs to up
                new float3(0, 0, -1), // 20 - belongs to front
            };
            Triangles = new ushort[]
            {
                0, 1, 2,
                2, 3, 0
            };
            UVs = new[]
            {
                new float2(0, 0.0f), // bottom left
                new float2(1f, 0.0f), // bottom right
                new float2(1.0f, 1.0f), // top right
                new float2(0.0f, 1.0f) // top left
            };
        }
    }
}