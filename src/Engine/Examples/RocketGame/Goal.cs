using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    class Goal : GameEntity
    {
        private bool _activated;
        private readonly float4 _inactiveColor = new float4(1,0,0,1);
        private readonly float4 _activeColor = new float4(0,1,0,1);


        public Goal(string meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
            : base(meshPath, rc, posX, posY, posZ, angX, angY, angZ)
        {
            SetShader(_inactiveColor, "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(10,10));
        }

        public Goal(string meshPath, RenderContext rc, float3 posxyz, float3 angxyz) : base(meshPath, rc, posxyz, angxyz)
        {
            SetShader(_inactiveColor, "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(10, 10));
        }

        public void SetActive()
        {
            if (!_activated)
            {
                SetShader(_activeColor, "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(10, 10));
                _activated = true;
            }
        }

        public void SetInactive()
        {
            if (_activated)
            {
                SetShader(_inactiveColor, "Assets/toon_generic_5_tex.png", new float4(0, 0, 0, 1), new float2(10, 10));
                _activated = false;
            }
        }
    }
}
