using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Fusee.Base.Common;
using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Engine.Core.ShaderShards.Vertex;
using Fusee.Math.Core;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Compiler for ShaderCode. Takes a MaterialComponent, evaluates input parameters and creates pixel and vertex shader
    /// </summary>
    public static class ShaderCodeBuilder
    {
        #region CreateVertexShader

        public static string CreateVertexShader(WeightComponent wc, ShaderEffectProps effectProps)
        {
            var vertexShader = new List<string>
            {
                HeaderShard.Version(),
                VertPropertiesShard.Uniforms(effectProps),
                VertPropertiesShard.InAndOutParams(wc, effectProps),
            };

            // Main
            AddVertexMain(effectProps, vertexShader);            

            return string.Join("\n", vertexShader);
        }

        private static void AddVertexMain(ShaderEffectProps effectProps, List<string> vertexShader)
        {
            // Main
            vertexShader.Add("void main() {");

            vertexShader.Add("gl_PointSize = 10.0;");

            if (effectProps.MeshProbs.HasNormals && effectProps.MeshProbs.HasWeightMap)
            {
                vertexShader.Add("vec4 newVertex;");
                vertexShader.Add("vec4 newNormal;");
                vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuVertex, 1.0) ) * fuBoneWeight.x ;");
                vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.x)] * vec4(fuNormal, 0.0)) * fuBoneWeight.x;");
                vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuVertex, 1.0)) * fuBoneWeight.y + newVertex;");
                vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.y)] * vec4(fuNormal, 0.0)) * fuBoneWeight.y + newNormal;");
                vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuVertex, 1.0)) * fuBoneWeight.z + newVertex;");

                vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.z)] * vec4(fuNormal, 0.0)) * fuBoneWeight.z + newNormal;");
                vertexShader.Add(
                    "newVertex = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuVertex, 1.0)) * fuBoneWeight.w + newVertex;");
                vertexShader.Add(
                    "newNormal = (FUSEE_BONES[int(fuBoneIndex.w)] * vec4(fuNormal, 0.0)) * fuBoneWeight.w + newNormal;");

                // At this point the normal is in World space - transform back to model space                
                vertexShader.Add("vNormal = mat3(FUSEE_ITMV) * newNormal.xyz;");
            }

            if (effectProps.MatProbs.HasSpecular)
            {
                vertexShader.Add("vec3 vCamPos = FUSEE_IMV[3].xyz;");

                vertexShader.Add(effectProps.MeshProbs.HasWeightMap
                    ? "vViewDir = normalize(vCamPos - vec3(newVertex));"
                    : "vViewDir = normalize(vCamPos - fuVertex);");
            }

            if (effectProps.MeshProbs.HasUVs)
                vertexShader.Add("vUV = fuUV;");

            if (effectProps.MeshProbs.HasNormals && !effectProps.MeshProbs.HasWeightMap)
                vertexShader.Add("vNormal = normalize(mat3(FUSEE_ITMV) * fuNormal);");

            vertexShader.Add("vViewPos = (FUSEE_MV * vec4(fuVertex, 1.0)).xyz;");

            if (effectProps.MeshProbs.HasTangents && effectProps.MeshProbs.HasBiTangents)
            {
                vertexShader.Add($"vT = {UniformNameDeclarations.TangentAttribName};");
                vertexShader.Add($"vB = {UniformNameDeclarations.BitangentAttribName};");
            }

            vertexShader.Add(effectProps.MeshProbs.HasWeightMap
            ? "gl_Position = FUSEE_MVP * vec4(vec3(newVertex), 1.0);"
            : "gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);");

            // End of main
            vertexShader.Add("}");
        }

        #endregion

        #region CreatePixelShader        

        public static string CreatePixelShader(MaterialComponent mc, ShaderEffectProps effectProps, LightingCalculationMethod lightingCalculationMethod)
        {
            var pixelShader = new List<string>
            {
                HeaderShard.Version(),
                HeaderShard.EsPrecision(),

                FragPropertiesShard.InParams(effectProps),
                FragPropertiesShard.FuseeUniforms(effectProps),
                FragPropertiesShard.MatPropsUniforms(effectProps),
                FragPropertiesShard.ColorOut(),

                LightingShard.LightStructDeclaration(),                     
            };

            //---- LIGHTING ---//
            if (effectProps.MatProbs.HasApplyLightString)
            {
                pixelShader.Add((mc as MaterialLightComponent)?.ApplyLightString);
            }
            else
            {
                //Adds methods to the PS that calculate the single light components (ambient, diffuse, specular)
                switch (effectProps.MatType)
                {
                    case MaterialType.Material:
                    case MaterialType.MaterialLightComponent:
                        pixelShader.Add(LightingShard.AmbientLightMethod());
                        if (effectProps.MatProbs.HasDiffuse)
                            pixelShader.Add(LightingShard.DiffuseLightMethod(effectProps));
                        if (effectProps.MatProbs.HasSpecular)
                            pixelShader.Add(LightingShard.SpecularLightMethod());
                        break;
                    case MaterialType.MaterialPbrComponent:
                        if (lightingCalculationMethod != LightingCalculationMethod.ADVANCED)
                        {
                            pixelShader.Add(LightingShard.AmbientLightMethod());
                            if (effectProps.MatProbs.HasDiffuse)
                                pixelShader.Add(LightingShard.DiffuseLightMethod(effectProps));
                            if (effectProps.MatProbs.HasSpecular)
                                pixelShader.Add(LightingShard.SpecularLightMethod());
                        }
                        else
                        {
                            pixelShader.Add(LightingShard.AmbientLightMethod());
                            if (effectProps.MatProbs.HasDiffuse)
                                pixelShader.Add(LightingShard.DiffuseLightMethod(effectProps));
                            if (effectProps.MatProbs.HasSpecular)
                                pixelShader.Add(LightingShard.PbrSpecularLightMethod((MaterialPBRComponent)mc));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Material Type unknown or incorrect: {effectProps.MatType}");
                }

                pixelShader.Add(LightingShard.ApplyLightMethod(mc, effectProps));
            }
            //--------------------------------------//

            //Calculates the lighting for all lights by using the above method
            AddPixelMainMethod(effectProps, pixelShader);

            return string.Join("\n", pixelShader);
        }

        private static void AddPixelMainMethod(ShaderEffectProps effectProps, List<string> pixelShader)
        {
            string fragColorAlpha = effectProps.MatProbs.HasDiffuse ? $"{UniformNameDeclarations.DiffuseColorName}.w" : "1.0";

            var methodBody = new List<string>
            {
                "vec4 result = ambientLighting(0.2);", //ambient component
                $"for(int i = 0; i < {LightingShard.NumberOfLightsForward};i++)",
                "{",
                "if(allLights[i].isActive == 0) continue;",
                "vec3 currentPosition = allLights[i].position;",
                "vec4 currentIntensities = allLights[i].intensities;",
                "vec3 currentConeDirection = allLights[i].direction;",
                "float currentAttenuation = allLights[i].maxDistance;",
                "float currentStrength = allLights[i].strength;",
                "float currentOuterConeAngle = allLights[i].outerConeAngle;",
                "float currentInnerConeAngle = allLights[i].innerConeAngle;",
                "int currentLightType = allLights[i].lightType; ",
                "result += ApplyLight(currentPosition, currentIntensities, currentConeDirection, ",
                "currentAttenuation, currentStrength, currentOuterConeAngle, currentInnerConeAngle, currentLightType);",
                "}",

                 effectProps.MatProbs.HasDiffuseTexture ? $"oFragmentColor = result;" : $"oFragmentColor = vec4(result.rgb, {UniformNameDeclarations.DiffuseColorName}.w);",

            };

            pixelShader.Add(GLSL.CreateMethod(GLSL.Type.Void, "main",
                new[] { "" }, methodBody));
        }

        #endregion

        #region Deferred

        /// <summary>
        /// FXAA shader relies on the luminosity of the pixels read from the texture.
        /// It is a weighted sum of the red, green and blue components that takes into account the sensibility of our eyes to each wavelength range.
        /// </summary>
        /// <returns></returns>
        private static string RGBLuma()
        {
            return @"
            float rgb2luma(vec3 rgb)
            {
                return rgb.y * (0.587/0.299) + rgb.x; //sqrt(dot(rgb, vec3(0.299, 0.587, 0.114)));
            }
            ";
        }

        private static string Quality()
        {
            return @"
            float QUALITY(int i)
            {
                switch(i)
                {
                    case 8:
                        return 1.5;
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                        return 2.0;
                    case 13:
                        return 4.0;
                    case 14:
                        return 8.0;
                }
            }
            ";
        }

        /// <summary>
        /// If rendered with FXAA we'll need an additional (final) pass, that takes the lighted scene, rendered to a texture, as input.
        /// </summary>
        /// <param name="srcRenderTarget">RenderTarget, that contains a single texture in the Albedo/Specular channel, that contains the lighted scene.</param>
        /// <param name="screenParams">The width and height of the screen.</param>       
        // see: http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf
        // http://blog.simonrodriguez.fr/articles/30-07-2016_implementing_fxaa.html
        public static ShaderEffect FXAARenderTargetEffect(RenderTarget srcRenderTarget, float2 screenParams)
        {
            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append(HeaderShard.EsPrecision());
            frag.Append($"#define LIGHTED_SCENE_TEX {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)}\n");
            frag.Append($"#define EDGE_THRESHOLD_MIN 0.0625\n");
            frag.Append($"#define EDGE_THRESHOLD_MAX 0.125\n");
            frag.Append($"#define ITERATIONS 14\n");
            frag.Append($"#define SUBPIXEL_QUALITY 0.125\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform sampler2D LIGHTED_SCENE_TEX;\n");
            frag.Append($"uniform vec2 ScreenParams;\n");

            frag.Append($"out vec4 oColor;\n");

            frag.Append(RGBLuma());

            frag.Append(Quality());

            frag.Append("void main() {");

            frag.Append(@"
                        
            // ------ FXAA calculation ------ //

            // ---- 0. Detecting where to apply FXAA

            vec2 inverseScreenSize = vec2(1.0/ScreenParams.x, 1.0/ScreenParams.y);
            vec3 colorCenter = texture(LIGHTED_SCENE_TEX, vTexCoords).rgb;

            // Luma at the current fragment
            float lumaCenter = rgb2luma(colorCenter);

            // Luma at the four direct neighbours of the current fragment.
            float lumaDown = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(0,-1)).rgb);
            float lumaUp = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(0,1)).rgb);
            float lumaLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,0)).rgb);
            float lumaRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,0)).rgb);

            // Find the maximum and minimum luma around the current fragment.
            float lumaMin = min(lumaCenter,min(min(lumaDown,lumaUp),min(lumaLeft,lumaRight)));
            float lumaMax = max(lumaCenter,max(max(lumaDown,lumaUp),max(lumaLeft,lumaRight)));

            // Compute the delta.
            float lumaRange = lumaMax - lumaMin;

            // If the luma variation is lower that a threshold (or if we are in a really dark area), we are not on an edge, don't perform any AA.
            if(lumaRange < max(EDGE_THRESHOLD_MIN, lumaMax * EDGE_THRESHOLD_MAX)) 
            {
                oColor = vec4(colorCenter, 1.0);
                return;
            }
            
            // ---- 1. Choosing Edge direction (vertical or horizontal)

            // Query the 4 remaining corners lumas.
            float lumaDownLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,-1)).rgb);
            float lumaUpRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,1)).rgb);
            float lumaUpLeft = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(-1,1)).rgb);
            float lumaDownRight = rgb2luma(textureOffset(LIGHTED_SCENE_TEX, vTexCoords, ivec2(1,-1)).rgb);

            // Combine the four edges lumas (using intermediary variables for future computations with the same values).
            float lumaDownUp = lumaDown + lumaUp;
            float lumaLeftRight = lumaLeft + lumaRight;

            // Same for corners
            float lumaLeftCorners = lumaDownLeft + lumaUpLeft;
            float lumaDownCorners = lumaDownLeft + lumaDownRight;
            float lumaRightCorners = lumaDownRight + lumaUpRight;
            float lumaUpCorners = lumaUpRight + lumaUpLeft;

            // Compute an estimation of the gradient along the horizontal and vertical axis.
            float edgeHorizontal =  abs(-2.0 * lumaLeft + lumaLeftCorners)  + abs(-2.0 * lumaCenter + lumaDownUp ) * 2.0    + abs(-2.0 * lumaRight + lumaRightCorners);
            float edgeVertical =    abs(-2.0 * lumaUp + lumaUpCorners)      + abs(-2.0 * lumaCenter + lumaLeftRight) * 2.0  + abs(-2.0 * lumaDown + lumaDownCorners);

            // Is the local edge horizontal or vertical ?
            bool isHorizontal = (edgeHorizontal >= edgeVertical);
            
            // ---- 2. Estimating gradient and choosing edge direction (current pixel is not necessarily exactly on the edge).

            // Select the two neighboring texels lumas in the opposite direction to the local edge.
            float luma1 = isHorizontal ? lumaDown : lumaLeft;
            float luma2 = isHorizontal ? lumaUp : lumaRight;
            // Compute gradients in this direction.
            float gradient1 = luma1 - lumaCenter;
            float gradient2 = luma2 - lumaCenter;

            // Which direction is the steepest ?
            bool is1Steepest = abs(gradient1) >= abs(gradient2);

            // Gradient in the corresponding direction, normalized.
            float gradientScaled = 0.25*max(abs(gradient1),abs(gradient2));

            // Choose the step size (one pixel) according to the edge direction.
            float stepLength = isHorizontal ? inverseScreenSize.y : inverseScreenSize.x;

            // Average luma in the correct direction.
            float lumaLocalAverage = 0.0;

            if(is1Steepest)
            {
                // Switch the direction
                stepLength = - stepLength;
                lumaLocalAverage = 0.5*(luma1 + lumaCenter);
            } 
            else 
            {
                lumaLocalAverage = 0.5*(luma2 + lumaCenter);
            }

            // Shift UV in the correct direction by half a pixel.
            vec2 currentUv = vTexCoords;
            if(isHorizontal)
            {
                currentUv.y += stepLength * 0.5;
            } 
            else 
            {
                currentUv.x += stepLength * 0.5;
            }

            // ---- 3. Exploration along the main axis of the edge.

            // Compute offset (for each iteration step) in the right direction.
            vec2 offset = isHorizontal ? vec2(inverseScreenSize.x,0.0) : vec2(0.0,inverseScreenSize.y);
            // Compute UVs to explore on each side of the edge, orthogonally. The QUALITY allows us to step faster.
            vec2 uv1 = currentUv - offset;
            vec2 uv2 = currentUv + offset;

            // Read the lumas at both current extremities of the exploration segment, and compute the delta wrt to the local average luma.
            float lumaEnd1 = rgb2luma(texture(LIGHTED_SCENE_TEX,uv1).rgb);
            float lumaEnd2 = rgb2luma(texture(LIGHTED_SCENE_TEX,uv2).rgb);
            lumaEnd1 -= lumaLocalAverage;
            lumaEnd2 -= lumaLocalAverage;

            // If the luma deltas at the current extremities are larger than the local gradient, we have reached the side of the edge.
            bool reached1 = abs(lumaEnd1) >= gradientScaled;
            bool reached2 = abs(lumaEnd2) >= gradientScaled;
            bool reachedBoth = reached1 && reached2;

            // If the side is not reached, we continue to explore in this direction.
            if(!reached1){
                uv1 -= offset;
            }
            if(!reached2){
                uv2 += offset;
            }   

            // ---- 4. Iterating - keep iterating until both extremities of the edge are reached, or until the maximum number of iterations (12) is reached.
            // If both sides have not been reached, continue to explore.
            if(!reachedBoth){

                for(int i = 2; i < ITERATIONS; i++){
                    // If needed, read luma in 1st direction, compute delta.
                    if(!reached1){
                        lumaEnd1 = rgb2luma(texture(LIGHTED_SCENE_TEX, uv1).rgb);
                        lumaEnd1 = lumaEnd1 - lumaLocalAverage;
                    }
                    // If needed, read luma in opposite direction, compute delta.
                    if(!reached2){
                        lumaEnd2 = rgb2luma(texture(LIGHTED_SCENE_TEX, uv2).rgb);
                        lumaEnd2 = lumaEnd2 - lumaLocalAverage;
                    }
                    // If the luma deltas at the current extremities is larger than the local gradient, we have reached the side of the edge.
                    reached1 = abs(lumaEnd1) >= gradientScaled;
                    reached2 = abs(lumaEnd2) >= gradientScaled;
                    reachedBoth = reached1 && reached2;

                    // If the side is not reached, we continue to explore in this direction, with a variable quality.
                    if(!reached1){
                        uv1 -= offset * QUALITY(i);
                    }
                    if(!reached2){
                        uv2 += offset * QUALITY(i);
                    }

                    // If both sides have been reached, stop the exploration.
                    if(reachedBoth){ break;}
                }
            }

            // ---- 5. Estimating offset.

            // Compute the distances to each extremity of the edge.
            float distance1 = isHorizontal ? (vTexCoords.x - uv1.x) : (vTexCoords.y - uv1.y);
            float distance2 = isHorizontal ? (uv2.x - vTexCoords.x) : (uv2.y - vTexCoords.y);

            // In which direction is the extremity of the edge closer ?
            bool isDirection1 = distance1 < distance2;
            float distanceFinal = min(distance1, distance2);

            // Length of the edge.
            float edgeThickness = (distance1 + distance2);

            // UV offset: read in the direction of the closest side of the edge.
            float pixelOffset = - distanceFinal / edgeThickness + 0.5;
            
            // Is the luma at center smaller than the local average ?
            bool isLumaCenterSmaller = lumaCenter < lumaLocalAverage;

            // If the luma at center is smaller than at its neighbour, the delta luma at each end should be positive (same variation).
            // (in the direction of the closer side of the edge.)
            bool correctVariation = ((isDirection1 ? lumaEnd1 : lumaEnd2) < 0.0) != isLumaCenterSmaller;

            // If the luma variation is incorrect, do not offset.
            float finalOffset = correctVariation ? pixelOffset : 0.0;

            // ---- 5. Subpixel antialiasing

            // Sub-pixel shifting
            // Full weighted average of the luma over the 3x3 neighborhood.
            float lumaAverage = (1.0/12.0) * (2.0 * (lumaDownUp + lumaLeftRight) + lumaLeftCorners + lumaRightCorners);
            // Ratio of the delta between the global average and the center luma, over the luma range in the 3x3 neighborhood.
            float subPixelOffset1 = clamp(abs(lumaAverage - lumaCenter)/lumaRange,0.0,1.0);
            float subPixelOffset2 = (-2.0 * subPixelOffset1 + 3.0) * subPixelOffset1 * subPixelOffset1;
            // Compute a sub-pixel offset based on this delta.
            float subPixelOffsetFinal = subPixelOffset2 * subPixelOffset2 * SUBPIXEL_QUALITY;

            // Pick the biggest of the two offsets.
            finalOffset = max(finalOffset,subPixelOffsetFinal);

            // ---- 6. Final read
            // Compute the final UV coordinates.
            vec2 finalUv = vTexCoords;
            if(isHorizontal)
            {
                finalUv.y += finalOffset * stepLength;
            } 
            else 
            {
                finalUv.x += finalOffset * stepLength;
            }

            // Read the color at the new UV coordinates, and use it.
            vec4 finalColor = texture(LIGHTED_SCENE_TEX, finalUv);
            oColor = finalColor;
            
            ");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},
                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
            });

        }

        /// <summary>
        /// Creates a blurred ssao texture, to hide rectangular artifacts originating from the noise texture;
        /// </summary>
        /// <param name="ssaoRenderTex">The non blurred ssao texture.</param>        
        public static ShaderEffect SSAORenderTargetBlurEffect(WritableTexture ssaoRenderTex)
        {
            float blurKernelSize;
            switch (ssaoRenderTex.Width)
            {
                case (int)TexRes.LOW_RES:
                    blurKernelSize = 2.0f;
                    break;
                default:
                case (int)TexRes.MID_RES:
                    blurKernelSize = 4.0f;
                    break;
                case (int)TexRes.HIGH_RES:
                    blurKernelSize = 8.0f;
                    break;
            }

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append(HeaderShard.EsPrecision());
            frag.Append($"#define SSAO_INPUT_TEX {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)}\n");
            frag.Append($"#define KERNEL_SIZE {blurKernelSize.ToString("0.0", CultureInfo.InvariantCulture)}\n");
            frag.Append($"#define KERNEL_SIZE_HALF {(blurKernelSize * 0.5)}\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform sampler2D SSAO_INPUT_TEX;\n");


            frag.Append($"out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)};\n");

            frag.Append("void main() {");

            frag.Append(@"
            vec2 texelSize = 1.0 / vec2(textureSize(SSAO_INPUT_TEX, 0));
            float result = 0.0;
            for (int x = -KERNEL_SIZE_HALF; x < KERNEL_SIZE_HALF; ++x) 
            {
                for (int y = -KERNEL_SIZE_HALF; y < KERNEL_SIZE_HALF; ++y) 
                {
                    vec2 offset = vec2(float(x), float(y)) * texelSize;
                    result += texture(SSAO_INPUT_TEX, vTexCoords + offset).r;
                }
            }

            result = result / (KERNEL_SIZE * KERNEL_SIZE);
            
            ");

            frag.Append($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)} = vec4(result, result, result, 1.0);");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = ssaoRenderTex},

            });

        }

        /// <summary>
        /// Shader effect for the ssao pass.
        /// </summary>        
        /// <param name="geomPassRenderTarget">RenderTarget filled in the previous geometry pass.</param>
        /// <param name="kernelLength">SSAO kernel size.</param>
        /// <param name="screenParams">Width and Height of the screen.</param>        
        public static ShaderEffect SSAORenderTargetTextureEffect(RenderTarget geomPassRenderTarget, int kernelLength, float2 screenParams)
        {
            var ssaoKernel = SSAOHelper.CreateKernel(kernelLength);
            var ssaoNoiseTex = SSAOHelper.CreateNoiseTex(16);

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();
            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"

            in vec3 fuVertex;
            out vec2 vTexCoords;

            ");

            vert.Append(@"
            void main() 
            {
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append(HeaderShard.EsPrecision());
            frag.Append($"#define KERNEL_LENGTH {kernelLength}\n");

            frag.Append($"in vec2 vTexCoords;\n");

            frag.Append($"uniform vec2 ScreenParams;\n");
            frag.Append($"uniform vec3[KERNEL_LENGTH] SSAOKernel;\n");
            frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_POSITION)};\n");
            frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_NORMAL)};\n");
            frag.Append($"uniform sampler2D NoiseTex;\n");
            frag.Append($"uniform mat4 FUSEE_P;\n");

            frag.Append($"out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)};\n");

            frag.Append("void main() {");
            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");

            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)
                discard;
            ");

            frag.AppendLine($"vec3 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords).xyz;");

            //SSAO
            //-------------------------------------- -
            frag.Append(@"
            float radius = 5.0;
            float occlusion = 0.0;
            float bias = 0.005;
            ");

            frag.AppendLine($"vec2 noiseScale = vec2(ScreenParams.x * 0.25, ScreenParams.y * 0.25);");
            frag.AppendLine($"vec3 randomVec = texture(NoiseTex, vTexCoords * noiseScale).xyz;");

            frag.AppendLine($"vec3 tangent = normalize(randomVec - Normal * dot(randomVec, Normal));");
            frag.AppendLine($"vec3 bitangent = cross(Normal, tangent);");
            frag.AppendLine($"mat3 tbn = mat3(tangent, bitangent, Normal);");

            frag.Append(@"

            for (int i = 0; i < KERNEL_LENGTH; ++i) 
            {
             // get sample position:
             vec3 sampleVal = tbn * SSAOKernel[i];
             sampleVal = sampleVal * radius + FragPos.xyz;

             // project sample position:
             vec4 offset = vec4(sampleVal, 1.0);
             offset = FUSEE_P * offset;		
             offset.xy /= offset.w;
             offset.xy = offset.xy * 0.5 + 0.5;

             // get sample depth:
             // ----- EXPENSIVE TEXTURE LOOKUP - graphics card workload goes up and frame rate goes down the nearer the camera is to the model.
             // keyword: dependent texture look up, see also: https://stackoverflow.com/questions/31682173/strange-performance-behaviour-with-ssao-algorithm-using-opengl-and-glsl
            ");

            frag.AppendLine($"float sampleDepth = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, offset.xy).z;");
            frag.Append(@"           

             // range check & accumulate:
             float rangeCheck = smoothstep(0.0, 1.0, radius / abs(FragPos.z - sampleDepth));
             occlusion += (sampleDepth <= sampleVal.z + bias ? 1.0 : 0.0) * rangeCheck;
            }

            occlusion = clamp(1.0 - (occlusion / float(KERNEL_LENGTH)), 0.0, 1.0);           

            ");

            frag.Append($"{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_SSAO)} = vec4(occlusion, occlusion, occlusion, 1.0);");

            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = geomPassRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},

                new EffectParameterDeclaration { Name = "ScreenParams", Value = screenParams},
                new EffectParameterDeclaration {Name = "SSAOKernel[0]", Value = ssaoKernel},
                new EffectParameterDeclaration {Name = "NoiseTex", Value = ssaoNoiseTex},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
            });

        }

        /// <summary>
        /// ShaderEffect for rendering into the textures given in a RenderTarget (Geometry Pass).
        /// </summary>
        /// <param name="rt">The RenderTarget</param>
        /// <param name="diffuseMix">Constant for mixing a single albedo color with a color read from a texture.</param>
        /// <param name="diffuseTex">The texture, containing diffuse colors.</param>
        /// <returns></returns>
        public static ShaderEffect GBufferTextureEffect(RenderTarget rt, float diffuseMix, Texture diffuseTex = null)
        {
            var textures = rt.RenderTextures;

            //------------ vertex shader ------------------//
            var vert = new StringBuilder();

            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"
                uniform mat4 FUSEE_M;
                uniform mat4 FUSEE_MV;
                uniform mat4 FUSEE_MVP;
                uniform mat4 FUSEE_ITM;
                uniform mat4 FUSEE_ITMV;
                uniform vec4 DiffuseColor;

                in vec3 fuVertex;
                in vec3 fuNormal;
                in vec4 fuColor;
                in vec2 fuUV;
                
                out vec4 vPos;
                out vec3 vNormal;
                out vec4 vColor;    
                out vec2 vUv;

                ");

            vert.Append(@"
                void main() 
                {
                    vPos = FUSEE_MV * vec4(fuVertex.xyz, 1.0);
                    vNormal = (FUSEE_ITMV * vec4(fuNormal, 0.0)).xyz;
                    vColor = DiffuseColor;
                    vUv = fuUV;

                    gl_Position = FUSEE_MVP * vec4(fuVertex, 1.0);

                }");

            //--------- Fragment shader ----------- //
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append(HeaderShard.EsPrecision());

            frag.Append(FragPropertiesShard.GBufferOut(rt));

            frag.Append(@"
                in vec4 vPos;
                in vec3 vNormal;
                in vec4 vColor;
                in vec2 vUv;"
            );

            if (diffuseTex != null)
            {
                frag.Append(@"
                uniform sampler2D DiffuseTexture;
                uniform float DiffuseMix;"
                );
            }

            frag.AppendLine("void main() {");

            for (int i = 0; i < textures.Length; i++)
            {
                var tex = textures[i];
                if (tex == null) continue;

                switch (i)
                {
                    case 0: //POSITION
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(vPos.xyz, vPos.w);");
                        break;
                    case 1: //ALBEDO_SPECULAR
                        if (diffuseTex != null)
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(mix(vColor.xyz, texture(DiffuseTexture, vUv).xyz, DiffuseMix), vColor.a);");
                        else
                            frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vColor;");
                        break;
                    case 2: //NORMAL
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(normalize(vNormal.xyz), 1.0);");
                        break;
                    case 3: //DEPTH
                        frag.AppendLine($"{Enum.GetName(typeof(RenderTargetTextureTypes), i)} = vec4(gl_FragCoord.z, gl_FragCoord.z, gl_FragCoord.z, 1.0);");
                        break;
                }
            }
            frag.Append("}");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration {Name = "FUSEE_ITMV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_ITM", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration {Name = "DiffuseColor", Value = new float4(1.0f, 0, 1.0f, 1.0f)},
                new EffectParameterDeclaration {Name = "DiffuseTexture", Value = diffuseTex},
                new EffectParameterDeclaration {Name = "DiffuseMix", Value = diffuseMix},
            });

        }

        private static string DeferredLightingVS()
        {
            var vert = new StringBuilder();
            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"
                
            uniform mat4 FUSEE_MVP;                       
            in vec3 fuVertex;
            out vec2 vTexCoords;           

            ");

            vert.Append(@"
            void main() 
            {                
                vTexCoords = fuVertex.xy * 2.0 * 0.5 + 0.5;
                gl_Position = vec4(fuVertex.xy * 2.0, 0.0 ,1.0);

            }");

            return vert.ToString();
        }        

        private static string DeferredLightingFS(LightComponent lc)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecision());

            for (int i = 0; i < Enum.GetNames(typeof(RenderTargetTextureTypes)).Length; i++)
            {
                frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
            }

            frag.Append(@"struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };
            uniform Light light;
            ");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                    frag.Append($"uniform sampler2D ShadowMap;\n");
                else
                    frag.Append("uniform samplerCube ShadowCubeMap;\n");
            }

            frag.Append("uniform mat4x4 LightSpaceMatrix;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(ShadowCalculation());
            else
                frag.Append(ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);                        

                        break;
                    }
                    //Parallel
                    case 1:
                        lightDir = -light.direction;
                        break;
                    //Spot
                    case 2:
                    {                           
                        //point component
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);
                        
                        //cone component
                        vec3 coneDir = light.direction;
                        float lightToSurfaceAngleCos = dot(coneDir, -lightDir); 
                        
                        float epsilon = cos(light.innerConeAngle) - cos(light.outerConeAngle);
                        float t = (lightToSurfaceAngleCos - cos(light.outerConeAngle)) / epsilon;
                        attenuation *= clamp(t, 0.0, 1.0);
                        break;
                    }
                    case 3:
                        lightDir = -light.direction;
                        break;
                }
                ");

            if (lc.IsCastingShadows)
            {
                if (lc.Type != LightType.Point)
                {
                    frag.Append(@"
                    // shadow                
                    if (light.isCastingShadows == 1)
                    {
                        vec4 posInLightSpace = (LightSpaceMatrix * FUSEE_IV) * FragPos;
                        shadow = ShadowCalculation(ShadowMap, posInLightSpace, Normal, lightDir, light.bias, 1.0);                    
                    }                
                    ");
                }
                else
                {
                    frag.Append(@"
                    // shadow       
                    if (light.isCastingShadows == 1)
                    {
                        shadow = ShadowCalculationCubeMap(ShadowCubeMap, (FUSEE_IV * FragPos).xyz, light.positionWorldSpace, light.maxDistance, Normal, lightDir, light.bias, 2.0);
                    }
                    ");
                }
            }

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static string DeferredLightingFSCascaded(LightComponent lc, int numberOfCascades)
        {
            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecision());

            for (int i = 0; i < Enum.GetNames(typeof(RenderTargetTextureTypes)).Length; i++)
            {
                frag.Append($"uniform sampler2D {Enum.GetName(typeof(RenderTargetTextureTypes), i)};\n");
            }

            frag.Append(@"struct Light 
            {
                vec3 position;
                vec3 positionWorldSpace;
                vec4 intensities;
                vec3 direction;
                vec3 directionWorldSpace;
                float maxDistance;
                float strength;
                float outerConeAngle;
                float innerConeAngle;
                int lightType;
                int isActive;
                int isCastingShadows;
                float bias;
            };
            uniform Light light;
            ");

            frag.Append("uniform mat4 FUSEE_IV;\n");
            frag.Append("uniform mat4 FUSEE_V;\n");
            frag.Append("uniform mat4 FUSEE_MV;\n");
            frag.Append("uniform mat4 FUSEE_ITV;\n");
           
            frag.Append($"uniform sampler2D[{numberOfCascades}] ShadowMaps;\n");
            frag.Append($"uniform vec2[{numberOfCascades}] ClipPlanes;\n");            

            frag.Append($"uniform mat4x4[{numberOfCascades}] LightSpaceMatrices;\n");
            frag.Append("uniform int PassNo;\n");
            frag.Append("uniform int SsaoOn;\n");

            frag.Append("uniform vec4 BackgroundColor;\n");

            frag.Append($"in vec2 vTexCoords;\n");
            frag.Append($"layout (location = {0}) out vec4 o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)};\n");

            //Shadow calculation
            //-------------------------------------- 
            if (lc.Type != LightType.Point)
                frag.Append(ShadowCalculation());
            else
                frag.Append(ShadowCalculationCubeMap());

            frag.Append(@"void main()
            {
            ");

            frag.AppendLine($"vec3 Normal = texture({RenderTargetTextureTypes.G_NORMAL.ToString()}, vTexCoords).rgb;");
            //Do not do calculations for the background - is there a smarter way (stencil buffer)?
            //---------------------------------------
            frag.Append(@"
            if(Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)      
            {
            ");

            frag.AppendLine($"  o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = BackgroundColor;");
            frag.AppendLine(@"  return;
            }
            ");

            frag.AppendLine($"vec4 FragPos = texture({RenderTargetTextureTypes.G_POSITION.ToString()}, vTexCoords);");
            frag.AppendLine($"vec3 DiffuseColor = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).rgb;");
            frag.AppendLine($"float SpecularStrength = texture({RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString()}, vTexCoords).a;");
            frag.AppendLine($"vec3 Occlusion = texture({RenderTargetTextureTypes.G_SSAO.ToString()}, vTexCoords).rgb;");

            //Lighting calculation
            //-------------------------
            frag.Append(@"
            // then calculate lighting as usual
            vec3 lighting = vec3(0,0,0);

            if(PassNo == 0)
            {
                vec3 ambient = vec3(0.2 * DiffuseColor);

                if(SsaoOn == 1)
                    ambient *= Occlusion;

                lighting += ambient;
            }

            vec3 camPos = FUSEE_IV[3].xyz;
            vec3 viewDir = normalize(-FragPos.xyz);

           
            if(light.isActive == 1)
            {
                float shadow = 0.0;

                vec3 lightColor = light.intensities.xyz;
                vec3 lightPosition = light.position;
                vec3 lightDir = normalize(lightPosition - FragPos.xyz);                

                //attenuation
                float attenuation = 1.0;
                switch(light.lightType)
                {
                    //Point
                    case 0:
                    {
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);                        

                        break;
                    }
                    //Parallel
                    case 1:
                        lightDir = -light.direction;
                        break;
                    //Spot
                    case 2:
                    {                           
                        //point component
                        float distanceToLight = length(lightPosition - FragPos.xyz); 
                        float distance = pow(distanceToLight/light.maxDistance, 2.0);
                        attenuation = (clamp(1.0 - pow(distance,2.0), 0.0, 1.0)) / (pow(distance,2.0) + 1.0);
                        
                        //cone component
                        vec3 coneDir = light.direction;
                        float lightToSurfaceAngleCos = dot(coneDir, -lightDir); 
                        
                        float epsilon = cos(light.innerConeAngle) - cos(light.outerConeAngle);
                        float t = (lightToSurfaceAngleCos - cos(light.outerConeAngle)) / epsilon;
                        attenuation *= clamp(t, 0.0, 1.0);
                        break;
                    }
                    case 3:
                        lightDir = -light.direction;
                        break;
                }
                ");

            if (lc.IsCastingShadows)
            {
                //TODO: iterate clip planes and choose shadow map for this frag. Use this shadow map in ShadowCalculation()  

                frag.Append(@"
                int thisFragmentsFirstCascade = -1;
                int thisFragmentsSecondCascade = -1;
                float fragDepth = FragPos.z;
                
                ");

                frag.AppendLine($"int numberOfCascades = {numberOfCascades};");
                frag.Append($"for (int i = 0; i < numberOfCascades; i++)\n");
                frag.Append(
                @"{
                    vec2 cp1 = ClipPlanes[i];
                    if(fragDepth < cp1.y)
                    {                        
                        thisFragmentsFirstCascade = i;  
                        if(i + 1 <= numberOfCascades - 1)
                        {
                            vec2 cp2 = ClipPlanes[i+1];
                            if(fragDepth < cp2.y)                                                 
                                thisFragmentsSecondCascade = i+1;
                        }
                        break;
                    }                    
                }
                ");

                frag.Append(@"
                // shadow                
                if (light.isCastingShadows == 1)
                {
                    vec4 posInLightSpace1 = (LightSpaceMatrices[thisFragmentsFirstCascade] * FUSEE_IV) * FragPos;
                    float shadow1 = ShadowCalculation(ShadowMaps[thisFragmentsFirstCascade], posInLightSpace1, Normal, lightDir,  light.bias, 1.0);                   

                    //blend cascades to avoid hard cuts between them
                    if(thisFragmentsSecondCascade != -1)
                    {  
                        float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade-1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps        

                        vec4 posInLightSpace2 = (LightSpaceMatrices[thisFragmentsSecondCascade] * FUSEE_IV) * FragPos;
                        float shadow2 = ShadowCalculation(ShadowMaps[thisFragmentsSecondCascade], posInLightSpace2, Normal, lightDir, light.bias, 1.0);    
                        float z = ClipPlanes[thisFragmentsFirstCascade].y - ClipPlanes[thisFragmentsFirstCascade].x;
                        float percent = (100.0/z * (fragDepth - ClipPlanes[thisFragmentsFirstCascade].x));
                        float percentNormalized = (percent - blendStartPercent) / (100.0 - blendStartPercent);

                        if(percent >= blendStartPercent)
                            shadow = mix(shadow1, shadow2, percentNormalized);
                        else
                            shadow = shadow1;
                    }
                    else                    
                        shadow = shadow1;
                }                              
                ");               
            }

            frag.Append(@"
                // diffuse 
                vec3 diffuse = max(dot(Normal, lightDir), 0.0) * DiffuseColor * lightColor;
                lighting += (1.0 - shadow) * (diffuse * attenuation * light.strength);
            
                // specular
                vec3 reflectDir = reflect(-lightDir, Normal);  
                float spec = pow(max(dot(viewDir, reflectDir), 0.0), 100.0);
                vec3 specular = SpecularStrength * spec * lightColor;
                lighting += (1.0 - shadow) * (specular * attenuation * light.strength);


                //vec3 cascadeColor1 = vec3(0.0,0.0,0.0);
                //vec3 cascadeColor2 = vec3(0.0,0.0,0.0);
                //vec3 cascadeColor = vec3(1.0,1.0,1.0);
                
                //if(thisFragmentsFirstCascade == 0)
                //    cascadeColor1 = vec3(1,0.3f,0.3f);
                //else if(thisFragmentsFirstCascade == 1)
                //     cascadeColor1 = vec3(0.3f,1,0.3f);
                //else if(thisFragmentsFirstCascade == 2)
                //    cascadeColor1 = vec3(0.3f,0.3f,1);
                //else if(thisFragmentsFirstCascade == 3)
                //    cascadeColor1 = vec3(1,1,0.3f);
                //else if(thisFragmentsFirstCascade == 4)
                //    cascadeColor1 = vec3(1,0.3,1);
                //else if(thisFragmentsFirstCascade == 5)
                //    cascadeColor1 = vec3(1,0.3f,1);                
                
                //if(thisFragmentsSecondCascade == 0)
                //    cascadeColor2 = vec3(1,0.3f,0.3f);
                //else if(thisFragmentsSecondCascade == 1)
                //     cascadeColor2 = vec3(0.3f,1,0.3f);
                //else if(thisFragmentsSecondCascade == 2)
                //    cascadeColor2 = vec3(0.3f,0.3f,1);
                //else if(thisFragmentsSecondCascade == 3)
                //    cascadeColor2 = vec3(1,1,0.3f);
                //else if(thisFragmentsSecondCascade == 4)
                //    cascadeColor2 = vec3(1,0.3,1);
                //else if(thisFragmentsSecondCascade == 5)
                //    cascadeColor2 = vec3(1,0.3f,1);

                //if(thisFragmentsSecondCascade != -1)
                //{
                //    float blendStartPercent = max(85.0 - (5.0 * float(thisFragmentsFirstCascade -1)), 50.0); //the farther away the cascade, the earlier we blend the shadow maps   
                //    float z = ClipPlanes[thisFragmentsFirstCascade].y;
                //    float percent = (100.0/z * fragDepth);
                //    float percentNormalized = (percent - blendStartPercent) / (100.0 - blendStartPercent);
                //    if(percent >= blendStartPercent)
                //        cascadeColor = mix(cascadeColor1, cascadeColor2, percentNormalized);
                //    else
                //        cascadeColor = cascadeColor1;
                //}
                //else
                //{
                //    cascadeColor = cascadeColor1;
                //}
                //lighting *= cascadeColor;
            }              
            ");

            frag.AppendLine($"o{Enum.GetName(typeof(RenderTargetTextureTypes), RenderTargetTextureTypes.G_ALBEDO_SPECULAR)} = vec4(lighting, 1.0);");

            frag.Append("}");

            return frag.ToString();
        }

        private static List<EffectParameterDeclaration> DeferredLightingEffectParams(RenderTarget srcRenderTarget, float4 backgroundColor)
        {
            return new List<EffectParameterDeclaration>()
            {
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_POSITION.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_POSITION]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_NORMAL.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_NORMAL]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_ALBEDO_SPECULAR.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_ALBEDO_SPECULAR]},
                new EffectParameterDeclaration { Name = RenderTargetTextureTypes.G_SSAO.ToString(), Value = srcRenderTarget.RenderTextures[(int)RenderTargetTextureTypes.G_SSAO]},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_IV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_ITV", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_P", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "light.position", Value = new float3(0, 0, -1.0f)},
                new EffectParameterDeclaration { Name = "light.positionWorldSpace", Value = new float3(0, 0, -1.0f)},
                new EffectParameterDeclaration { Name = "light.intensities", Value = float4.Zero},
                new EffectParameterDeclaration { Name = "light.maxDistance", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.strength", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.outerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.innerConeAngle", Value = 0.0f},
                new EffectParameterDeclaration { Name = "light.direction", Value = float3.Zero},
                new EffectParameterDeclaration { Name = "light.lightType", Value = 1},
                new EffectParameterDeclaration { Name = "light.isActive", Value = 1},
                new EffectParameterDeclaration { Name = "light.isCastingShadows", Value = 0},
                new EffectParameterDeclaration { Name = "light.bias", Value = 0.0f},
                new EffectParameterDeclaration { Name = "PassNo", Value = 0},
                new EffectParameterDeclaration { Name = "BackgroundColor", Value = backgroundColor},
                new EffectParameterDeclaration { Name = "SsaoOn", Value = 1},
            };
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture shadowMap, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMap", Value = shadowMap });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightingVS(),
                    PS = DeferredLightingFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass. Shadow is calculated with cascaded shadow maps.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMaps">The cascaded shadow maps.</param>
        /// <param name="clipPlanes">The clip planes of the frustums. Each frustum is associated with one shadow map.</param>
        /// <param name="numberOfCascades">The number of sub-frustums, used for cascaded shadow mapping.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>            
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableTexture[] shadowMaps, float2[] clipPlanes, int numberOfCascades,float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = new float4x4[] { } });
            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowMaps[0]", Value = shadowMaps });
            effectParams.Add(new EffectParameterDeclaration { Name = "ClipPlanes[0]", Value = clipPlanes });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightingVS(),
                    PS = DeferredLightingFSCascaded(lc, numberOfCascades),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>
        /// <param name="shadowMap">The shadow map.</param>
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        /// <returns></returns>
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, WritableCubeMap shadowMap, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            effectParams.Add(new EffectParameterDeclaration { Name = "ShadowCubeMap", Value = shadowMap });

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightingVS(),
                    PS = DeferredLightingFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that performs the lighting calculation according to the textures from the Geometry Pass.
        /// </summary> 
        /// <param name="srcRenderTarget">The source render target.</param>
        /// <param name="lc">The light component.</param>  
        /// <param name="backgroundColor">Sets the background color. Could be replaced with a texture or other sky color calculations in the future.</param>       
        public static ShaderEffect DeferredLightingPassEffect(RenderTarget srcRenderTarget, LightComponent lc, float4 backgroundColor)
        {
            var effectParams = DeferredLightingEffectParams(srcRenderTarget, backgroundColor);

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = DeferredLightingVS(),
                    PS = DeferredLightingFS(lc),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = true,
                        ZEnable = true,
                        BlendOperation = BlendOperation.Add,
                        SourceBlend = Blend.One,
                        DestinationBlend = Blend.One,
                        ZFunc = Compare.LessEqual,


                    }
                }
            },
            effectParams.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowCubeMapEffect(float4x4[] lightSpaceMatrices)
        {
            // Vertex shader ------------------------------
            var vert = new StringBuilder();
            vert.AppendLine("#version 330 core");

            vert.Append(@"                
            
            uniform mat4 FUSEE_M;              
            in vec3 fuVertex; 
            ");

            vert.Append(@"
            void main() 
            {                
                gl_Position = FUSEE_M * vec4(fuVertex, 1.0);               

            }");

            //Geometry shader ------------------------------
            var geom = new StringBuilder();
            geom.AppendLine("#version 330 core");
            geom.Append(@"
                layout (triangles) in;
                layout (triangle_strip, max_vertices=18) out;

                uniform mat4 LightSpaceMatrices[6];

                out vec4 FragPos;

                void main()
                {
                    for(int face = 0; face < 6; face++)
                    {
                        gl_Layer = face; // built-in variable that specifies to which face we render.
                        for(int i = 0; i < 3; ++i) // for each triangle's vertices
                        {
                            FragPos = gl_in[i].gl_Position;
                            gl_Position = LightSpaceMatrices[face] * FragPos;
                            EmitVertex();
                        }    
                        EndPrimitive();
                    }
                }  

            ");

            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append("#version 330 core\n");

            frag.Append("in vec4 FragPos;\n");
            frag.Append("uniform vec2 LightMatClipPlanes;\n");
            frag.Append("uniform vec3 LightPos;\n");

            frag.Append(@"
            void main()
            {
                // get distance between fragment and light source
                float lightDistance = length(FragPos.xyz - LightPos);
    
                // map to [0;1] range by dividing by far_plane
                lightDistance = lightDistance / LightMatClipPlanes.y;
    
                // write this as modified depth                
                gl_FragDepth = lightDistance;
            }  
                
            ");

            var effectParamDecls = new List<EffectParameterDeclaration>
            {
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "FUSEE_V", Value = float4x4.Identity },
                new EffectParameterDeclaration { Name = "LightMatClipPlanes", Value = float2.One },
                new EffectParameterDeclaration { Name = "LightPos", Value = float3.One },
                new EffectParameterDeclaration { Name = $"LightSpaceMatrices[0]", Value = lightSpaceMatrices }
            };

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    GS = geom.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                        CullMode = Cull.Clockwise,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            effectParamDecls.ToArray());
        }

        /// <summary>
        /// ShaderEffect that renders the depth map from a lights point of view - this depth map is used as a shadow map.
        /// </summary>
        /// <returns></returns>
        public static ShaderEffect ShadowMapEffect()
        {
            // Vertex shader ------------------------------
            var vert = new StringBuilder();
            vert.Append(HeaderShard.Version());
            vert.Append(HeaderShard.EsPrecision());

            vert.Append(@"
                
            uniform mat4 LightSpaceMatrix;
            uniform mat4 FUSEE_M;              
            in vec3 fuVertex; 
            ");

            vert.Append(@"
            void main() 
            {                
                gl_Position = LightSpaceMatrix* FUSEE_M * vec4(fuVertex, 1.0);               

            }");

            // Fragment shader ------------------------------
            var frag = new StringBuilder();
            frag.Append(HeaderShard.Version());
            frag.Append("#extension GL_ARB_explicit_uniform_location : enable\n");
            frag.Append(HeaderShard.EsPrecision());

            frag.Append($"layout (location = {0}) out vec4 {Enum.GetName(typeof(RenderTargetTextureTypes), (int)RenderTargetTextureTypes.G_DEPTH)};\n");            
            frag.Append("uniform int LightType;\n");

            frag.Append(@"void main()
            {  
                float d = gl_FragCoord.z;
                
            ");
            frag.AppendLine($" {Enum.GetName(typeof(RenderTargetTextureTypes), (int)RenderTargetTextureTypes.G_DEPTH)} = vec4(d, d, d, 1.0);\n");
            frag.Append(@"}
            ");

            return new ShaderEffect(new[]
            {
                new EffectPassDeclaration
                {
                    VS = vert.ToString(),
                    PS = frag.ToString(),
                    StateSet = new RenderStateSet
                    {
                        AlphaBlendEnable = false,
                        ZEnable = true,
                        CullMode = Cull.Clockwise,
                        ZFunc = Compare.LessEqual,
                    }
                }
            },
            new[]
            {
                new EffectParameterDeclaration { Name = "FUSEE_M", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "FUSEE_MVP", Value = float4x4.Identity},
                new EffectParameterDeclaration { Name = "LightSpaceMatrix", Value = float4x4.Identity},                
                new EffectParameterDeclaration { Name = "LightType", Value = 0},
            });
        }

        private static string ShadowCalculation()
        {
            return @"
                
            float ShadowCalculation(sampler2D shadowMap, vec4 fragPosLightSpace, vec3 normal, vec3 lightDir, float bias, float pcfKernelHalfSize)
            {
                float shadow = 0.0;                
                int pcfLoop = int(pcfKernelHalfSize);
                float pcfKernelSize = pcfKernelHalfSize + pcfKernelHalfSize + 1.0;
                pcfKernelSize *= pcfKernelSize;

                // perform perspective divide
                vec4 projCoords = fragPosLightSpace / fragPosLightSpace.w;
                projCoords = projCoords * 0.5 + 0.5; 
                //float closestDepth = texture(shadowMap, projCoords.xy).r;
                float currentDepth = projCoords.z;  

                float thisBias = max(bias * (1.0 - dot(normal, lightDir)), bias/100.0);
            
                vec2 texelSize = 1.0 / vec2(textureSize(shadowMap, 0));
                
                //use this for using sampler2DShadow (automatic PCF) instead of sampler2D
                //float depth = texture(shadowMap, projCoords.xyz).r; 
                //shadow += (currentDepth - thisBias) > depth ? 1.0 : 0.0;
                
                for(int x = -pcfLoop; x <= pcfLoop; ++x)
                {
                    for(int y = -pcfLoop; y <= pcfLoop; ++y)
                    {
                        float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
                        shadow += (currentDepth - thisBias) > pcfDepth ? 1.0 : 0.0;        
                    }    
                }
                shadow /= pcfKernelSize;

                return shadow;
            }

            ";
        }

        private static string ShadowCalculationCubeMap()
        {
            return @"
                
            float ShadowCalculationCubeMap(samplerCube shadowMap, vec3 fragPos, vec3 lightPos, float farPlane, vec3 normal, vec3 lightDir, float bias, float pcfKernelHalfSize)
            {               
                float pcfKernelSize = pcfKernelHalfSize + pcfKernelHalfSize + 1.0;
                pcfKernelSize *= pcfKernelSize;

                vec3 sampleOffsetDirections[20] = vec3[]
                (
                   vec3( pcfKernelHalfSize,  pcfKernelHalfSize,  pcfKernelHalfSize), vec3( pcfKernelHalfSize, -pcfKernelHalfSize,  pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize,  pcfKernelHalfSize), vec3(-pcfKernelHalfSize,  pcfKernelHalfSize,  pcfKernelHalfSize), 
                   vec3( pcfKernelHalfSize,  pcfKernelHalfSize, -pcfKernelHalfSize), vec3( pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize,  pcfKernelHalfSize, -pcfKernelHalfSize),
                   vec3( pcfKernelHalfSize,  pcfKernelHalfSize,  0), vec3( pcfKernelHalfSize, -pcfKernelHalfSize,  0), vec3(-pcfKernelHalfSize, -pcfKernelHalfSize,  0), vec3(-pcfKernelHalfSize,  pcfKernelHalfSize,  0),
                   vec3( pcfKernelHalfSize,  0,  pcfKernelHalfSize), vec3(-pcfKernelHalfSize,  0,  pcfKernelHalfSize), vec3( pcfKernelHalfSize,  0, -pcfKernelHalfSize), vec3(-pcfKernelHalfSize,  0, -pcfKernelHalfSize),
                   vec3( 0,  pcfKernelHalfSize,  pcfKernelHalfSize), vec3( 0, -pcfKernelHalfSize,  pcfKernelHalfSize), vec3( 0, -pcfKernelHalfSize, -pcfKernelHalfSize), vec3( 0,  pcfKernelHalfSize, -pcfKernelHalfSize)
                );

                // get vector between fragment position and light position
                vec3 fragToLight = (fragPos - lightPos) * -1.0;                
                // now get current linear depth as the length between the fragment and light position
                float currentDepth = length(fragToLight);

                float shadow = 0.0;
                float thisBias   = max(bias * (1.0 - dot(normal, lightDir)), bias * 0.01);//0.15;
                int samples = 20;
                vec3 camPos = FUSEE_IV[3].xyz;
                float viewDistance = length(camPos - fragPos);
                    
                float diskRadius = 0.5; //(1.0 + (viewDistance / farPlane)) / pcfKernelSize;
                for(int i = 0; i < samples; ++i)
                { 
                    float closestDepth = texture(shadowMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;
                    closestDepth *= farPlane;   // Undo mapping [0;1]
                    if(currentDepth - thisBias > closestDepth)
                        shadow += 1.0;
                }
                shadow /= float(samples);
                return shadow;
            }

            ";
        }

        #endregion

        #region Make ShaderEffect
        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };
            return MakeShaderEffectFromMatComp(temp);
        }

        /// <summary>
        ///     Builds a simple shader effect with diffuse and specular color.
        /// </summary>
        /// <param name="diffuseColor">The diffuse color the resulting effect.</param>
        /// <param name="specularColor">The specular color for the resulting effect.</param>
        /// <param name="shininess">The resulting effect's shininess.</param>
        /// <param name="texName">Name of the texture you want to use.</param>
        /// <param name="diffuseMix">Determines how much the diffuse color and the color from the texture are mixed.</param>
        /// <param name="specularIntensity">The resulting effects specular intensity.</param>
        /// <returns>A ShaderEffect ready to use as a component in scene graphs.</returns>
        public static ShaderEffect MakeShaderEffect(float4 diffuseColor, float4 specularColor, float shininess, string texName, float diffuseMix, float specularIntensity = 0.5f)
        {
            MaterialComponent temp = new MaterialComponent
            {
                Diffuse = new MatChannelContainer
                {
                    Color = diffuseColor,
                    Texture = texName,
                    Mix = diffuseMix
                },
                Specular = new SpecularChannelContainer
                {
                    Color = specularColor,
                    Shininess = shininess,
                    Intensity = specularIntensity,
                }
            };
            return MakeShaderEffectFromMatComp(temp);
        }

        /// <summary> 
        /// Creates a ShaderEffectComponent from a MaterialComponent 
        /// </summary> 
        /// <param name="mc">The MaterialComponent</param> 
        /// <param name="wc">Only pass over a WeightComponent if you use bone animations in the current node (usage: pass currentNode.GetWeights())</param>        
        /// <returns></returns> 
        /// <exception cref="Exception"></exception> 
        public static ShaderEffect MakeShaderEffectFromMatComp(MaterialComponent mc, WeightComponent wc = null)
        {
            string vs = "";
            string ps = "";

            var effectProps = ShaderShardUtil.CollectEffectProps(null, mc, wc);

            //TODO: LightingCalculationMethod does not seem to have an effect right now.. see ShaderCodeBuilder constructor.
            if (mc.GetType() == typeof(MaterialLightComponent))
            {
                if (mc is MaterialLightComponent lightMat) 
                { 
                    vs = CreateVertexShader(wc, effectProps); 
                    ps = CreatePixelShader(lightMat, effectProps, LightingCalculationMethod.SIMPLE); 
                }
            }
            else if (mc.GetType() == typeof(MaterialPBRComponent))
            {
                if (mc is MaterialPBRComponent pbrMaterial) 
                {
                    vs = CreateVertexShader(wc, effectProps);
                    ps = CreatePixelShader(pbrMaterial, effectProps, LightingCalculationMethod.SIMPLE);
                }
            }
            else
            {
                vs = CreateVertexShader(wc, effectProps);
                ps = CreatePixelShader(mc, effectProps, LightingCalculationMethod.SIMPLE);
            }

            var effectParameters = AssembleEffectParamers(mc);

            if (vs == string.Empty || ps == string.Empty) throw new Exception("Material could not be evaluated or be built!");

            var ret = new ShaderEffect(new[]
                {
                    new EffectPassDeclaration
                    {
                        VS = vs, 
                        //VS = VsBones, 
                        PS = ps,
                        StateSet = new RenderStateSet
                        {
                            ZEnable = true,
                            AlphaBlendEnable = true,
                            SourceBlend = Blend.SourceAlpha,
                            DestinationBlend = Blend.InverseSourceAlpha,
                            BlendOperation = BlendOperation.Add,
                        }
                    }
                },
                effectParameters
            );
            
            return ret;
        }

        private static IEnumerable<EffectParameterDeclaration> AssembleEffectParamers(MaterialComponent mc)
        {
            var effectParameters = new List<EffectParameterDeclaration>();

            if (mc.HasDiffuse)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.DiffuseColorName,
                    Value = mc.Diffuse.Color
                });
                if (mc.Diffuse.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.DiffuseMixName,
                        Value = mc.Diffuse.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.DiffuseTextureName,
                        Value = LoadTexture(mc.Diffuse.Texture)
                    });
                }
            }

            if (mc.HasSpecular)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.SpecularColorName,
                    Value = mc.Specular.Color
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.SpecularShininessName,
                    Value = mc.Specular.Shininess
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.SpecularIntensityName,
                    Value = mc.Specular.Intensity
                });
                if (mc.Specular.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularMixName,
                        Value = mc.Specular.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.SpecularTextureName,
                        Value = LoadTexture(mc.Specular.Texture)
                    });
                }
            }

            if (mc.HasEmissive)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.EmissiveColorName,
                    Value = mc.Emissive.Color
                });
                if (mc.Emissive.Texture != null)
                {
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveMixName,
                        Value = mc.Emissive.Mix
                    });
                    effectParameters.Add(new EffectParameterDeclaration
                    {
                        Name = UniformNameDeclarations.EmissiveTextureName,
                        Value = LoadTexture(mc.Emissive.Texture)
                    });
                }
            }

            if (mc.HasBump)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.BumpIntensityName,
                    Value = mc.Bump.Intensity
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = UniformNameDeclarations.BumpTextureName,
                    Value = LoadTexture(mc.Bump.Texture)
                });
            }

            for (int i = 0; i < LightingShard.NumberOfLightsForward; i++)
            {
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].position",
                    Value = new float3(0, 0, -1.0f)
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].intensities",
                    Value = float4.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].maxDistance",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].strength",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].outerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].innerConeAngle",
                    Value = 0.0f
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].direction",
                    Value = float3.Zero
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].lightType",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isActive",
                    Value = 1
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].isCastingShadows",
                    Value = 0
                });
                effectParameters.Add(new EffectParameterDeclaration
                {
                    Name = "allLights[" + i + "].bias",
                    Value = 0f
                });
            }

            // FUSEE_ PARAMS
            // TODO: Just add the necessary ones!
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_M",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_MVP",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_ITMV",
                Value = float4x4.Identity
            });

            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_IMV",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_V",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_P",
                Value = float4x4.Identity
            });
            effectParameters.Add(new EffectParameterDeclaration
            {
                Name = "FUSEE_BONES",
                Value = new[] { float4x4.Identity }
            });

            return effectParameters;
        }

        private static Texture LoadTexture(string path)
        {
            var image = AssetStorage.Get<ImageData>(path);
            if (image != null)
                return new Texture(image);

            image = AssetStorage.Get<ImageData>("DefaultTexture.png");
            if (image != null)
                return new Texture(image);

            return new Texture(new ImageData());
        }

        #endregion        

    }
}