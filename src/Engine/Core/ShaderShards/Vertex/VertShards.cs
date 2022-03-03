using Fusee.Engine.Core.Effects;
using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Vertex
{
    /// <summary>
    /// Contains pre-defined Shader Shards. 
    /// Those are the content of the Vertex Shader's method that lets us change values of the "out"-struct that is used for the position calculation.    
    /// </summary>
    public static class VertShards
    {
        /// <summary>
        /// Returns a default method body for the vertex shaders "ChangeSurf" method.
        /// </summary>
        /// <param name="surfInput">The surface input class. Needed to receive the shading model.</param>
        /// <returns></returns>
        internal static List<string> SurfOutBody(SurfaceEffectInput surfInput)
        {
            var res = new List<string>();
            switch (surfInput.ShadingModel)
            {
                case ShadingModel.Edl:
                case ShadingModel.Unlit:
                    res.Add("OUT.position = fuVertex;");
                    break;
                case ShadingModel.DiffuseSpecular:
                case ShadingModel.DiffuseOnly:
                case ShadingModel.Glossy:
                case ShadingModel.BRDF:
                    res.Add(@"
                    OUT.position = fuVertex;
                    OUT.normal = fuNormal;"
                    );
                    break;
                default:
                    throw new ArgumentException("Invalid ShadingModel!");
            }
            return res;
        }

        /// <summary>
        /// Returns a default method body for the vertex shaders "ChangeSurf" method.
        /// </summary>
        internal static List<string> SurfOutBody(ShadingModel shadingModel)
        {
            var res = new List<string>();
            switch (shadingModel)
            {
                case ShadingModel.Edl:
                case ShadingModel.Unlit:
                    res.Add("OUT.position = fuVertex;");
                    break;
                case ShadingModel.DiffuseSpecular:
                case ShadingModel.DiffuseOnly:
                case ShadingModel.Glossy:
                case ShadingModel.BRDF:
                    res.Add(@"
                    OUT.position = fuVertex;
                    OUT.normal = fuNormal;"
                    );
                    break;
                default:
                    throw new ArgumentException("Invalid ShadingModel!");
            }
            return res;
        }
    }
}