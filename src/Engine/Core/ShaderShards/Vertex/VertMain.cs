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
                $"newVertex = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.x)] * vec4(vertPos, 1.0) ) * {UniformNameDeclarations.Instance.BoneWeight}.x ;",
                $"newVertex = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.y)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.Instance.BoneWeight}.y + newVertex;",
                $"newVertex = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.z)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.Instance.BoneWeight}.z + newVertex;",
                $"newVertex = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.w)] * vec4(vertPos, 1.0)) * {UniformNameDeclarations.Instance.BoneWeight}.w + newVertex;",
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
                $"newNormal = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.x)] * vec4(normal, 0.0)) * {UniformNameDeclarations.Instance.BoneWeight}.x;",
                $"newNormal = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.y)] * vec4(normal, 0.0)) * {UniformNameDeclarations.Instance.BoneWeight}.y + newNormal;",
                $"newNormal = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.z)] * vec4(normal, 0.0)) * {UniformNameDeclarations.Instance.BoneWeight}.z + newNormal;",
                $"newNormal = ({UniformNameDeclarations.Instance.Bones}[int({UniformNameDeclarations.Instance.BoneIndex}.w)] * vec4(normal, 0.0)) * {UniformNameDeclarations.Instance.BoneWeight}.w + newNormal;",
                "return newNormal;"
            };

            return GLSL.CreateMethod(GLSL.Type.Vec4, "TransformNormalByBone", new string[] { GLSL.CreateVar(GLSL.Type.Vec3, "normal") }, methodBody);
        }

        /// <summary>
        /// Creates the main method for the vertex shader, used in forward rendering.
        /// </summary>
        /// <returns></returns>
        public static string VertexMain(LightingSetupFlags setup, bool doRenderPoints)
        {
            var vertMainBody = new List<string>
            {
                $"{SurfaceOut.Instance.SurfOutVaryingName} = {SurfaceOut.Instance.ChangeSurfVert}();",
                $"vec4 changedVert = {SurfaceOut.Instance.SurfOutVaryingName}.{SurfaceOut.Instance.Pos.Item2};",
                $"{SurfaceOut.Instance.SurfOutVaryingName}.{SurfaceOut.Instance.Pos.Item2} = {UniformNameDeclarations.Instance.ModelView} * {SurfaceOut.Instance.SurfOutVaryingName}.{SurfaceOut.Instance.Pos.Item2};",
            };

            if (!setup.HasFlag(LightingSetupFlags.Unlit))
            {
                vertMainBody.Add($"{SurfaceOut.Instance.SurfOutVaryingName}.{SurfaceOut.Instance.Normal.Item2} = normalize(vec3({ UniformNameDeclarations.Instance.ITModelView}* vec4({SurfaceOut.Instance.SurfOutVaryingName}.normal, 0.0)));");
            }

            if (setup.HasFlag(LightingSetupFlags.AlbedoTex) || setup.HasFlag(LightingSetupFlags.NormalMap))
                vertMainBody.Add($"{VaryingNameDeclarations.TextureCoordinates} = {UniformNameDeclarations.Instance.TextureCoordinates};");

            if (setup.HasFlag(LightingSetupFlags.NormalMap))
            {
                vertMainBody.Add($"vec3 T = normalize(vec3({ UniformNameDeclarations.Instance.ITModelView} * vec4({ UniformNameDeclarations.Instance.Tangent}.xyz, 0.0)));");
                vertMainBody.Add($"vec3 B = normalize(vec3({ UniformNameDeclarations.Instance.ITModelView} * vec4({ UniformNameDeclarations.Instance.Bitangent}.xyz, 0.0)));");

                vertMainBody.Add($"TBN = mat3(T,B,{SurfaceOut.Instance.SurfOutVaryingName}.{SurfaceOut.Instance.Normal.Item2});");
            }

            vertMainBody.Add($"gl_Position = {UniformNameDeclarations.Instance.ModelViewProjection} * changedVert;");

            if (doRenderPoints)
                vertMainBody.Add($"gl_PointSize = float({UniformNameDeclarations.Instance.PointSize});");

            //TODO: needed when bone animation is working (again)
            //vertMainBody.Add(effectProps.MeshProbs.HasWeightMap
            //? $"gl_Position = {UniformNameDeclarations.Instance.ModelViewProjection} * vec4(vec3(newVertex), 1.0);"
            //: $"gl_Position = {UniformNameDeclarations.Instance.ModelViewProjection} * vec4({UniformNameDeclarations.Instance.Vertex}, 1.0);"

            return GLSL.MainMethod(vertMainBody);
        }
    }
}