using Fusee.Engine.Core.ShaderShards;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.Effects
{
    internal static class DeferredShaders
    {
        /// <summary>
        /// Ready to use FXAA fragment shader
        /// </summary>
        public static string FXAAFrag
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine("#define ITERATIONS 14");
                sb.AppendLine($"#define {UniformNameDeclarations.LightedSceneTexture} Albedo");
                sb.AppendLine("#define EDGE_THRESHOLD_MIN 0.0625");
                sb.AppendLine("#define EDGE_THRESHOLD_MAX 0.125");
                sb.AppendLine("#define SUBPIXEL_QUALITY 0.75");
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));

                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Sampler2D, UniformNameDeclarations.LightedSceneTexture));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Vec2, UniformNameDeclarations.ViewportPx));

                sb.AppendLine(GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.ColorOut));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Float, "rgb2luma", new string[] { "vec3 rgb" }, new List<string>
                {
                    $"return rgb.y * (0.587/0.299) + rgb.x; //sqrt(dot(rgb, vec3(0.299, 0.587, 0.114)));",
                }));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Float, "QUALITY", new string[] { "int i" }, new List<string>
                {
                   "switch(i)" ,
                   "{",
                    "case 8:  " ,
                    "    return 1.5;" ,
                    "case 9:  " ,
                    "case 10: " ,
                    "case 11: " ,
                    "case 12: " ,
                    "    return 2.0;" ,
                    "case 13: " ,
                    "    return 4.0;" ,
                    "case 14: " ,
                    "    return 8.0;" ,
                    "}"
                }));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    $"// ------ FXAA calculation ------ //",
                    $"",
                    $"// ---- 0. Detecting where to apply FXAA",
                    $"",
                    $"vec2 inverseScreenSize = vec2(1.0/{UniformNameDeclarations.ViewportPx}.x, 1.0/{UniformNameDeclarations.ViewportPx}.y);",
                    $"vec3 colorCenter = texture({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}).rgb;",
                    $"",
                    $"// Luma at the current fragment",
                    $"float lumaCenter = rgb2luma(colorCenter);",
                    $"",
                    $"// Luma at the four direct neighbours of the current fragment.",
                    $"float lumaDown = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(0, -1)).rgb);",
                    $"float lumaUp = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(0, 1)).rgb);",
                    $"float lumaLeft = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(-1, 0)).rgb);",
                    $"float lumaRight = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(1, 0)).rgb);",
                    $"",
                    $"// Find the maximum and minimum luma around the current fragment.",
                    $"float lumaMin = min(lumaCenter, min(min(lumaDown, lumaUp), min(lumaLeft, lumaRight)));",
                    $"float lumaMax = max(lumaCenter, max(max(lumaDown, lumaUp), max(lumaLeft, lumaRight)));",
                    $"",
                    $"// Compute the delta.",
                    $"float lumaRange = lumaMax - lumaMin;",
                    $"",
                    $"// If the luma variation is lower that a threshold (or if we are in a really dark area), we are not on an edge, don't perform any AA.",
                    $"if (lumaRange < max(EDGE_THRESHOLD_MIN, lumaMax * EDGE_THRESHOLD_MAX))",
                    "{",$"    oColor = vec4(colorCenter, 1.0);",
                    $"    return;",
                    "}",
                    $"",
                    $"// ---- 1. Choosing Edge direction (vertical or horizontal)",
                    $"",
                    $"// Query the 4 remaining corners lumas.",
                    $"float lumaDownLeft = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(-1, -1)).rgb);",
                    $"float lumaUpRight = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(1, 1)).rgb);",
                    $"float lumaUpLeft = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(-1, 1)).rgb);",
                    $"float lumaDownRight = rgb2luma(textureOffset({UniformNameDeclarations.LightedSceneTexture}, {VaryingNameDeclarations.TextureCoordinates}, ivec2(1, -1)).rgb);",
                    $"     ",
                    $"// Combine the four edges lumas (using intermediary variables for future computations with the same values).",
                    $"float lumaDownUp = lumaDown + lumaUp;",
                    $"float lumaLeftRight = lumaLeft + lumaRight;",
                    $"     ",
                    $"// Same for corners    ",
                    $"float lumaLeftCorners = lumaDownLeft + lumaUpLeft; ",
                    $"float lumaDownCorners = lumaDownLeft + lumaDownRight;",
                    $"float lumaRightCorners = lumaDownRight + lumaUpRight;",
                    $"float lumaUpCorners = lumaUpRight + lumaUpLeft;",
                    $"     ",
                    $"// Compute an estimation of the gradient along the horizontal and vertical axis.",
                    $"float edgeHorizontal = abs(-2.0 * lumaLeft + lumaLeftCorners) + abs(-2.0 * lumaCenter + lumaDownUp) * 2.0 + abs(-2.0 * lumaRight + lumaRightCorners);",
                    $"float edgeVertical = abs(-2.0 * lumaUp + lumaUpCorners) + abs(-2.0 * lumaCenter + lumaLeftRight) * 2.0 + abs(-2.0 * lumaDown + lumaDownCorners); ",
                    $"     ",
                    $"// Is the local edge horizontal or vertical ?  ",
                    $"bool isHorizontal = (edgeHorizontal >= edgeVertical);",
                    $"     ",
                    $"// ---- 2. Estimating gradient and choosing edge direction (current pixel is not necessarily exactly on the edge).",
                    $"     ",
                    $"// Select the two neighboring texels lumas in the opposite direction to the local edge.",
                    $"float luma1 = isHorizontal ? lumaDown : lumaLeft;",
                    $"float luma2 = isHorizontal ? lumaUp : lumaRight; ",
                    $"// Compute gradients in this direction.",
                    $"float gradient1 = luma1 - lumaCenter;",
                    $"float gradient2 = luma2 - lumaCenter;",
                    $"     ",
                    $"// Which direction is the steepest ?     ",
                    $"bool is1Steepest = abs(gradient1) >= abs(gradient2); ",
                    $"     ",
                    $"// Gradient in the corresponding direction, normalized. ",
                    $"float gradientScaled = 0.25 * max(abs(gradient1), abs(gradient2)); ",
                    $"     ",
                    $"// Choose the step size (one pixel) according to the edge direction.",
                    $"float stepLength = isHorizontal ? inverseScreenSize.y : inverseScreenSize.x; ",
                    $"     ",
                    $"// Average luma in the correct direction.",
                    $"float lumaLocalAverage = 0.0;",
                    $"     ",
                    $"if (is1Steepest) ",
                    "{     ",
                    $"    // Switch the direction  ",
                    $"    stepLength = -stepLength;",
                    $"    lumaLocalAverage = 0.5 * (luma1 + lumaCenter); ",
                    "}     ",
                    $"else ",
                    "{     ",
                    $"    lumaLocalAverage = 0.5 * (luma2 + lumaCenter); ",
                    "}     ",
                    $"     ",
                    $"// Shift UV in the correct direction by half a pixel.",
                    $"vec2 currentUv = {VaryingNameDeclarations.TextureCoordinates}; ",
                    $"if (isHorizontal)",
                    "{     ",
                    $"    currentUv.y += stepLength * 0.5; ",
                    "}     ",
                    $"else ",
                    "{     ",
                    $"    currentUv.x += stepLength * 0.5; ",
                    "}     ",
                    $"     ",
                    $"// ---- 3. Exploration along the main axis of the edge. ",
                    $"     ",
                    $"// Compute offset (for each iteration step) in the right direction. ",
                    $"vec2 offset = isHorizontal ? vec2(inverseScreenSize.x, 0.0) : vec2(0.0, inverseScreenSize.y);",
                    $"// Compute UVs to explore on each side of the edge, orthogonally. The QUALITY allows us to step faster. ",
                    $"vec2 uv1 = currentUv - offset; ",
                    $"vec2 uv2 = currentUv + offset; ",
                    $"     ",
                    $"// Read the lumas at both current extremities of the exploration segment, and compute the delta wrt to the local average luma.",
                    $"float lumaEnd1 = rgb2luma(texture({UniformNameDeclarations.LightedSceneTexture}, uv1).rgb);",
                    $"float lumaEnd2 = rgb2luma(texture({UniformNameDeclarations.LightedSceneTexture}, uv2).rgb);",
                    $"lumaEnd1 -= lumaLocalAverage;",
                    $"lumaEnd2 -= lumaLocalAverage;",
                    $"     ",
                    $"// If the luma deltas at the current extremities are larger than the local gradient, we have reached the side of the edge.",
                    $"bool reached1 = abs(lumaEnd1) >= gradientScaled; ",
                    $"bool reached2 = abs(lumaEnd2) >= gradientScaled; ",
                    $"bool reachedBoth = reached1 && reached2; ",
                    $"     ",
                    $"// If the side is not reached, we continue to explore in this direction.",
                    $"if (!reached1)   ",
                    "{     ",
                    $"    uv1 -= offset; ",
                    "}     ",
                    $"if (!reached2)   ",
                    "{     ",
                    $"    uv2 += offset; ",
                    "}     ",
                    $"     ",
                    $"// ---- 4. Iterating - keep iterating until both extremities of the edge are reached, or until the maximum number of iterations (14) is reached.",
                    $"// If both sides have not been reached, continue to explore.",
                    $"if (!reachedBoth)",
                    "{     ",
                    $"     ",
                    $"    for (int i = 2; i < ITERATIONS; i++) ",
                    "    { ",
                    $"  // If needed, read luma in 1st direction, compute delta. ",
                    $"  if (!reached1) ",
                    "  {   ",
                    $"lumaEnd1 = rgb2luma(texture({UniformNameDeclarations.LightedSceneTexture}, uv1).rgb);",
                    $"lumaEnd1 = lumaEnd1 - lumaLocalAverage;",
                    "  }   ",
                    $"  // If needed, read luma in opposite direction, compute delta.",
                    $"  if (!reached2) ",
                    "  {   ",
                    $"lumaEnd2 = rgb2luma(texture({UniformNameDeclarations.LightedSceneTexture}, uv2).rgb);",
                    $"lumaEnd2 = lumaEnd2 - lumaLocalAverage;",
                    "  }   ",
                    $"  // If the luma deltas at the current extremities is larger than the local gradient, we have reached the side of the edge.",
                    $"  reached1 = abs(lumaEnd1) >= gradientScaled;",
                    $"  reached2 = abs(lumaEnd2) >= gradientScaled;",
                    $"  reachedBoth = reached1 && reached2;",
                    $"     ",
                    $"  // If the side is not reached, we continue to explore in this direction, with a variable quality.",
                    $"  if (!reached1) ",
                    "  {   ",
                    $"uv1 -= offset * QUALITY(i);",
                    "  }   ",
                    $"  if (!reached2) ",
                    "  {   ",
                    $"uv2 += offset * QUALITY(i);",
                    "  }   ",
                    $"     ",
                    $"  // If both sides have been reached, stop the exploration.",
                    "  if (reachedBoth) { break; } ",
                    "    } ",
                    "}     ",
                    $"     ",
                    $"// ---- 5. Estimating offset.",
                    $"     ",
                    $"// Compute the distances to each extremity of the edge. ",
                    $"float distance1 = isHorizontal ? ({VaryingNameDeclarations.TextureCoordinates}.x - uv1.x) : ({VaryingNameDeclarations.TextureCoordinates}.y - uv1.y);",
                    $"float distance2 = isHorizontal ? (uv2.x - {VaryingNameDeclarations.TextureCoordinates}.x) : (uv2.y - {VaryingNameDeclarations.TextureCoordinates}.y);",
                    $"     ",
                    $"// In which direction is the extremity of the edge closer ?",
                    $"bool isDirection1 = distance1 < distance2; ",
                    $"float distanceFinal = min(distance1, distance2); ",
                    $"     ",
                    $"// Length of the edge. ",
                    $"float edgeThickness = (distance1 + distance2); ",
                    $"     ",
                    $"// UV offset: read in the direction of the closest side of the edge.",
                    $"float pixelOffset = -distanceFinal / edgeThickness + 0.5;",
                    $"     ",
                    $"// Is the luma at center smaller than the local average ?  ",
                    $"bool isLumaCenterSmaller = lumaCenter < lumaLocalAverage;",
                    $"     ",
                    $"// If the luma at center is smaller than at its neighbour, the delta luma at each end should be positive (same variation).",
                    $"// (in the direction of the closer side of the edge.)",
                    $"bool correctVariation = ((isDirection1 ? lumaEnd1 : lumaEnd2) < 0.0) != isLumaCenterSmaller; ",
                    $"     ",
                    $"// If the luma variation is incorrect, do not offset.",
                    $"float finalOffset = correctVariation ? pixelOffset : 0.0;",
                    $"     ",
                    $"// ---- 5. Subpixel antialiasing   ",
                    $"     ",
                    $"// Sub-pixel shifting  ",
                    $"// Full weighted average of the luma over the 3x3 neighborhood.",
                    $"float lumaAverage = (1.0 / 12.0) * (2.0 * (lumaDownUp + lumaLeftRight) + lumaLeftCorners + lumaRightCorners);",
                    $"// Ratio of the delta between the global average and the center luma, over the luma range in the 3x3 neighborhood.",
                    $"float subPixelOffset1 = clamp(abs(lumaAverage - lumaCenter) / lumaRange, 0.0, 1.0);",
                    $"float subPixelOffset2 = (-2.0 * subPixelOffset1 + 3.0) * subPixelOffset1 * subPixelOffset1;",
                    $"// Compute a sub-pixel offset based on this delta.",
                    $"float subPixelOffsetFinal = subPixelOffset2 * subPixelOffset2 * SUBPIXEL_QUALITY;",
                    $"     ",
                    $"// Pick the biggest of the two offsets.",
                    $"finalOffset = max(finalOffset, subPixelOffsetFinal); ",
                    $"     ",
                    $"// ---- 6. Final read  ",
                    $"// Compute the final UV coordinates.",
                    $"vec2 finalUv = {VaryingNameDeclarations.TextureCoordinates}; ",
                    $"     ",
                    $"if (isHorizontal)",
                    "{     ",
                    $"    finalUv.y += finalOffset * stepLength; ",
                    "}     ",
                    $"else ",
                    "{     ",
                    $"    finalUv.x += finalOffset * stepLength; ",
                    "}     ",
                    $"     ",
                    $"// Read the color at the new UV coordinates, and use it.",
                    $"vec4 finalColor = texture({UniformNameDeclarations.LightedSceneTexture}, finalUv);",
                    $"{VaryingNameDeclarations.ColorOut} = finalColor; ",
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use deferred vertex shader
        /// </summary>
        public static string DeferredVert
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Vertex));
                sb.AppendLine(GLSL.CreateOut(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    $"{VaryingNameDeclarations.TextureCoordinates} = {UniformNameDeclarations.Vertex}.xy * 2.0 * 0.5 + 0.5;",
                    $"gl_Position = vec4({UniformNameDeclarations.Vertex}.xy * 2.0, 0.0, 1.0);"
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use shadow cube map geometry shader
        /// </summary>
        public static string ShadowCubeMapGeom
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version440Core);
                sb.AppendLine("layout (triangles) in;");
                sb.AppendLine("layout(triangle_strip, max_vertices = 18) out; ");

                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.LightSpaceMatrices6));

                sb.AppendLine(GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.FragPos));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    "for (int face = 0; face < 6; face++)   ",
                    "{  ",
                    "    gl_Layer = face; // built-in variable that specifies to which face we render.",
                    "    for (int i = 0; i < 3; ++i) // for each triangle's vertices",
                    "    {    ",
                    "  FragPos = gl_in[i].gl_Position;",
                    $" gl_Position = LightSpaceMatrices[face] * {VaryingNameDeclarations.FragPos};",
                    "  EmitVertex();",
                    "    }    ",
                    "    EndPrimitive();",
                    "}  "
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use shadow cube map geometry shader
        /// </summary>
        public static string ShadowCubeMapPointPrimitiveGeom
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version440Core);
                sb.AppendLine("layout (points) in;");
                sb.AppendLine("layout (points, max_vertices=6) out;");

                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.LightSpaceMatrices6));

                sb.AppendLine(GLSL.CreateOut(GLSL.Type.Vec4, VaryingNameDeclarations.FragPos));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                "for(int face = 0; face < 6; face++)",
                "{",
                "   gl_Layer = face; // built-in variable that specifies to which face we render.",
                "",
                "   FragPos = gl_in[0].gl_Position;",
                "   gl_Position = LightSpaceMatrices[face] * FragPos;",
                "   EmitVertex();",
                "",
                "  EndPrimitive();",
                "}"
                }));

                return sb.ToString();
            }
        }

        public static string ShadowMapVert
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.Model));
                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Vertex));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.LightSpaceMatrix));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    $"gl_Position = {UniformNameDeclarations.LightSpaceMatrix} * {UniformNameDeclarations.Model} * vec4({UniformNameDeclarations.Vertex}, 1.0);",
                }));

                return sb.ToString();
            }
        }

        public static string ShadowMapFrag
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine("#extension GL_ARB_explicit_uniform_location : enable");
                sb.AppendLine(Header.EsPrecisionHighpFloat);
                sb.AppendLine("layout (location = 0) out vec4 Depth;");

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    $"float d = gl_FragCoord.z;",
                    "Depth = vec4(d, d, d, 1.0);"
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use shadow cube map vertices shader
        /// </summary>
        public static string ShadowCubeMapVert
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);

                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.Model));
                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec3, UniformNameDeclarations.Vertex));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    $"gl_Position = {UniformNameDeclarations.Model} * vec4({UniformNameDeclarations.Vertex}, 1.0);",
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use shadow cube map fragment shader
        /// </summary>
        public static string ShadowCubeMapFrag
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec4, VaryingNameDeclarations.FragPos));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Vec2, UniformNameDeclarations.LightMatClipPlanes));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Vec3, UniformNameDeclarations.LightPos));

                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                    "// get distance between fragment and light source",
                    $"float lightDistance = length({VaryingNameDeclarations.FragPos}.xyz - {UniformNameDeclarations.LightPos});",
                    "",
                    "// map to [0;1] range by dividing by far_plane",
                    $"lightDistance = lightDistance / {UniformNameDeclarations.LightMatClipPlanes}.y;",
                    "",
                    "// write this as modified depth",
                    "gl_FragDepth = lightDistance;"
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use simple blur shader
        /// </summary>
        public static string BlurFrag
        {
            get
            {
                // TODO(MR,SBu): check for naming!

                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine("#define KERNEL_SIZE_HALF 2");

                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Sampler2D, "InputTex"));
                sb.AppendLine("layout (location = 0) out vec4 oBlurred;");


                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                "vec2 texelSize = 1.0 / vec2(textureSize(InputTex, 0)); ",
                "vec3 result = vec3(0.0, 0.0, 0.0); ",
                " ",
                "for (int x = -KERNEL_SIZE_HALF; x < KERNEL_SIZE_HALF; ++x)",
                "{",
                "   for (int y = -KERNEL_SIZE_HALF; y < KERNEL_SIZE_HALF; ++y)",
                "   {   ",
                " vec2 offset = vec2(float(x), float(y)) * texelSize; ",
                $" result += texture(InputTex, {VaryingNameDeclarations.TextureCoordinates} + offset).rgb;",
                "   }   ",
                "}",
                " ",
                "float kernelSize = float(KERNEL_SIZE_HALF) * 2.0;",
                "result = result / (kernelSize * kernelSize); ",
                " ",
                "oBlurred = vec4(result, 1.0);"
                }));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Ready to use SSAO shader
        /// </summary>
        public static string SSAOFrag
        {
            get
            {
                // TODO(MR,SBu): check for naming!

                var sb = new StringBuilder();
                sb.AppendLine(Header.Version300Es);
                sb.AppendLine("#define KERNEL_LENGTH 64");
                sb.AppendLine(Header.EsPrecisionHighpFloat);

                sb.AppendLine(GLSL.CreateIn(GLSL.Type.Vec2, VaryingNameDeclarations.TextureCoordinates));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.IVec2, UniformNameDeclarations.ViewportPx));
                sb.AppendLine("uniform vec3[KERNEL_LENGTH] SSAOKernel;");
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Sampler2D, "Position"));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Sampler2D, "Normal"));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Sampler2D, "NoiseTex"));
                sb.AppendLine(GLSL.CreateUniform(GLSL.Type.Mat4, UniformNameDeclarations.Projection));

                sb.AppendLine("out vec4 Ssao;");


                sb.AppendLine(GLSL.CreateMethod(GLSL.Type.Void, "main", Array.Empty<string>(), new List<string>
                {
                  $"vec3 Normal = texture(Normal, {VaryingNameDeclarations.TextureCoordinates}).rgb;",
                  "",
                  "if (Normal.x == 0.0 && Normal.y == 0.0 && Normal.z == 0.0)",
                  "    discard;",
                  "",
                  $"vec3 FragPos = texture(Position, {VaryingNameDeclarations.TextureCoordinates}).xyz;",
                  "",
                  "float radius = 5.0;",
                  "float occlusion = 0.0;",
                  "float bias = 0.005;",
                  $"vec2 noiseScale = vec2(float({UniformNameDeclarations.ViewportPx}.x) * 0.25, float({UniformNameDeclarations.ViewportPx}.y) * 0.25);",
                  $"vec3 randomVec = texture(NoiseTex, {VaryingNameDeclarations.TextureCoordinates} * noiseScale).xyz;",
                  "vec3 tangent = normalize(randomVec - Normal * dot(randomVec, Normal)); ",
                  "vec3 bitangent = cross(Normal, tangent); ",
                  "mat3 tbn = mat3(tangent, bitangent, Normal); ",
                  "     ",
                  "for (int i = 0; i < KERNEL_LENGTH; ++i)  ",
                  "{    ",
                  "    // get sample position:  ",
                  "    vec3 sampleVal = tbn * SSAOKernel[i];",
                  "    sampleVal = sampleVal * radius + FragPos.xyz;",
                  "     ",
                  "    // project sample position:    ",
                  "    vec4 offset = vec4(sampleVal, 1.0);",
                  "    offset = FUSEE_P * offset; ",
                  "    offset.xy /= offset.w; ",
                  "    offset.xy = offset.xy * 0.5 + 0.5; ",
                  "     ",
                  "    // get sample depth:     ",
                  "    // ----- EXPENSIVE TEXTURE LOOKUP - graphics card workload goes up and frame rate goes down the nearer the camera is to the model.",
                  "    // keyword: dependent texture look up, see also: https://stackoverflow.com/questions/31682173/strange-performance-behaviour-with-ssao-algorithm-using-opengl-and-glsl    ",
                  "    float sampleDepth = texture(Position, offset.xy).z;",
                  "     ",
                  "    // range check & accumulate:   ",
                  "    float rangeCheck = smoothstep(0.0, 1.0, radius / abs(FragPos.z - sampleDepth));",
                  "    occlusion += (sampleDepth <= sampleVal.z + bias ? 1.0 : 0.0) * rangeCheck; ",
                  "}    ",
                  "     ",
                  "occlusion = clamp(1.0 - (occlusion / float(KERNEL_LENGTH)), 0.0, 1.0); ",
                  "     ",
                  "Ssao = vec4(occlusion, occlusion, occlusion, 1.0); "
                }));

                return sb.ToString();
            }
        }

    }
}