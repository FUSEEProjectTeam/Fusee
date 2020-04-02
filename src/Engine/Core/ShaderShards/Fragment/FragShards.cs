using System;
using System.Collections.Generic;

namespace Fusee.Engine.Core.ShaderShards.Fragment
{
    public static class FragShards
    {
        public static readonly string ChangeSurfFrag = "ChangeSurfFrag";
        public static string GetChangeSurfFragMethod(LightingSetup setup, List<string> methodBody, Type inputType)
        {
            return GLSL.CreateMethod(SurfaceOut.GetLightingSetupShards(setup).Name, ChangeSurfFrag, new string[] { $"{inputType.Name} IN" }, methodBody);
        }
    }
}
