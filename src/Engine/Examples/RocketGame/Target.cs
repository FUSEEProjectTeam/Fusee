using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    class Target : GameEntity
    {
        private bool _activated;
        private readonly float4 _inactiveColor = new float4(1, 0, 0, 1);
        private readonly float4 _activeColor = new float4(0, 1, 0, 1);
        private readonly float4 _inactiveBorderColor = new float4(0, 0, 0, 1);
        private readonly float4 _activeBorderColor = new float4(0, 0, 0, 1);
        private readonly float2 _inactiveBorderWidth = new float2(10, 10);
        private readonly float2 _activeBorderWidth = new float2(10, 10);

        public Target(string meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
            : base(meshPath, rc, posX, posY, posZ, angX, angY, angZ)
        {
            SetShader(_inactiveColor, "Assets/toon_generic_5_tex.png", _inactiveBorderColor, _inactiveBorderWidth);
        }

        public bool GetStatus()
        {
            return _activated;
        }

        public void SetActive()
        {
            if (!_activated)
            {
                SetShader(_activeColor, "Assets/toon_generic_5_tex.png", _activeBorderColor, _activeBorderWidth);
                _activated = true;
            }
        }

        public void SetInactive()
        {
            if (_activated)
            {
                SetShader(_inactiveColor, "Assets/toon_generic_5_tex.png", _inactiveBorderColor, _inactiveBorderWidth);
                _activated = false;
            }
        }
    }
}
