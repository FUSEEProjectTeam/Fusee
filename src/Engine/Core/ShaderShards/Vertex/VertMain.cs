using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Collection of shader code strings, describing the Main method of a vertex shader.
    /// </summary>
    public static class VertMain
    {
        /// <summary>
        /// Returns a GLSL method that transforms a vertex by a bone.
        /// Returns the transformed vertex.
        /// Parameters: vertex position (vec3).
        /// </summary>
        public static string TransformPosByBone()
        {
            var methodBody = new List<string>()
            {
                "vec4 newVertex;",
                $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.x)] * vec4(vertPos, 1.0) ) * {UniformNameDeclarations.BoneWeight}.x ;",
                $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.y)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.BoneWeight}.y + newVertex;",
                $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.z)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.BoneWeight}.z + newVertex;",
                $"newVertex = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.w)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.BoneWeight}.w + newVertex;",
                "return newVertex;"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec4, "TransformPosByBone", new string[] { GLSL.CreateVar(GLSL.Type.Vec3, "vertPos") }, methodBody);
        }

        /// <summary>
        /// Returns a GLSL method that transforms a normal by a bone.
        /// Returns the transformed normal.
        /// Parameters: normal (vec3).
        /// </summary>
        public static string TransformNormalByBone()
        {
            var methodBody = new List<string>()
            {
                "vec4 newNormal;",
                $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.x)] * vec4(normal, 0.0)) * {UniformNameDeclarations.BoneWeight}.x;",
                $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.y)] * vec4(normal, 0.0)) * {UniformNameDeclarations.BoneWeight}.y + newNormal;",
                $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.z)] * vec4(normal, 0.0)) * {UniformNameDeclarations.BoneWeight}.z + newNormal;",
                $"newNormal = ({UniformNameDeclarations.Bones}[int({UniformNameDeclarations.BoneIndex}.w)] * vec4(normal, 0.0)) * {UniformNameDeclarations.BoneWeight}.w + newNormal;",
                "return newNormal;"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec4, "TransformNormalByBone", new string[] { GLSL.CreateVar(GLSL.Type.Vec3, "normal") }, methodBody);
        }

        /// <summary>
        /// Creates the main method for the vertex shader, used in forward rendering.
        /// </summary>
        /// <returns></returns>
        public static string VertexMain(ShadingModel shadingModel, TextureSetup texSetup, bool doRenderPoints)
        {
            var vertMainBody = new List<string>
            {
                $"{SurfaceOut.SurfOutVaryingName} = {SurfaceOut.ChangeSurfVert}();",
                $"vec3 changedVert = {SurfaceOut.SurfOutVaryingName}.{SurfaceOut.Pos.Item2};",
                $"{SurfaceOut.SurfOutVaryingName}.{SurfaceOut.Pos.Item2} = ({UniformNameDeclarations.ModelView} * vec4({SurfaceOut.SurfOutVaryingName}.{SurfaceOut.Pos.Item2}, 1.0)).xyz;",
            };

            if (shadingModel != (ShadingModel.Unlit) && shadingModel != (ShadingModel.Edl))
            {
                vertMainBody.Add($"{SurfaceOut.SurfOutVaryingName}.{SurfaceOut.Normal.Item2} = normalize(vec3({ UniformNameDeclarations.ITModelView}* vec4({SurfaceOut.SurfOutVaryingName}.normal, 0.0)));");
            }

            if (texSetup.HasFlag(TextureSetup.AlbedoTex) || texSetup.HasFlag(TextureSetup.NormalMap))
                vertMainBody.Add($"{VaryingNameDeclarations.TextureCoordinates} = {UniformNameDeclarations.TextureCoordinates};");

            if (texSetup.HasFlag(TextureSetup.NormalMap))
            {
                vertMainBody.Add($"vec3 T = normalize(vec3({ UniformNameDeclarations.ITModelView} * vec4({ UniformNameDeclarations.Tangent}.xyz, 0.0)));");
                vertMainBody.Add($"vec3 B = normalize(vec3({ UniformNameDeclarations.ITModelView} * vec4({ UniformNameDeclarations.Bitangent}.xyz, 0.0)));");

                vertMainBody.Add($"TBN = mat3(T,B,{SurfaceOut.SurfOutVaryingName}.{SurfaceOut.Normal.Item2});");
            }

            vertMainBody.Add($"gl_Position = {UniformNameDeclarations.ModelViewProjection} * vec4(changedVert, 1.0);");
            vertMainBody.Add($"{VaryingNameDeclarations.Color} = {UniformNameDeclarations.VertexColor};");

            if (doRenderPoints)
                vertMainBody.Add($"gl_PointSize = float({UniformNameDeclarations.PointSize});");

            //TODO: needed when bone animation is working (again)
            //vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
            //? $"gl_Position = {UniformNameDeclarations.ModelViewProjection} * vec4(vec3(newVertex), 1.0);"
            //: $"gl_Position = {UniformNameDeclarations.ModelViewProjection} * vec4({UniformNameDeclarations.Vertex}, 1.0);"

            return GLSL.MainMethod(vertMainBody);
        }
    }
}