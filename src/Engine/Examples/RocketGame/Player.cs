using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class Player : GameEntity
    {
        public Player(String meshPath, ShaderMaterial material, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0) : base(meshPath, material, rc, posX, posY, posZ, angX, angY, angZ)
        {
        }

        public Player(String meshPath, ShaderMaterial material, RenderContext rc, float3 posxyz, float3 angxyz) : base(meshPath, material, rc, posxyz, angxyz)
        {
        }

        public float4x4 GetCamMatrix()
        {
            //Change to inherit movement
            return float4x4.CreateTranslation(0, 0, 1000) * float4x4.LookAt(0, 0, 0, (float)Math.Sin(0), 0, (float)Math.Cos(0), 0, 1, 0);
        }
    }
}
