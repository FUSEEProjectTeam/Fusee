using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Engine.Core.ShaderShards
{

    public static class LightingShard
    {
        ///The maximal number of lights we can render when using the forward pipeline.
        public const int NumberOfLightsForward = 8;

        public static string LightStructDeclaration()
        {
            var lightStruct = @"
            struct Light 
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
            ";
            return lightStruct + $"uniform Light allLights[{NumberOfLightsForward}];";

        }
    }
}
