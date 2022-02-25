using Fusee.Base.Core;
using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.Primitives;
using Fusee.Engine.Core.Scene;
using Fusee.Engine.Core.ShaderShards.Fragment;
using Fusee.Math.Core;
using System.Threading.Tasks;
using static Fusee.Engine.Core.Input;
using static Fusee.Engine.Core.Time;

namespace Fusee.Examples.RenderContextOnly.Core
{
    [FuseeApplication(Name = "FUSEE Simple Example", Description = "A very simple example.")]
    public class RenderContextOnly : RenderCanvas
    {
        // angle variables
        private static float _angleHorz, _angleVert, _angleVelHorz, _angleVelVert;

        private const float RotationSpeed = 7;
        private const float Damping = 0.8f;

        private const float ZNear = 1f;
        private const float ZFar = 1000;
        private readonly float _fovy = M.PiOver4;

        private bool _keys;

        private Light _light;

        private Mesh _rocketWhite;
        private Mesh _rocketGreen;
        private Mesh _rocketGray;

        private float4x4 _modelMatWhite;
        private float4x4 _modelMatGreen;
        private float4x4 _modelMatGray;

        private SurfaceEffect _whiteFx;
        private SurfaceEffect _greenFx;
        private SurfaceEffect _grayFx;

        private async Task Load()
        {

        }

        public override async Task InitAsync()
        {
            await Load();
            await base.InitAsync();
        }

        // Init is called on startup.
        public override void Init()
        {
            RC.ClearColor = float4.One;

            _light = new Light
            {
                IsCastingShadows = false,
                Type = Base.Common.LightType.Parallel,
                Color = float4.One,
                Strength = 1
            };
            UpdateShaderParamForLight(_light);

            _rocketWhite = Rocket.MeshWhite();
            _modelMatWhite = Rocket.TransformWhite().Matrix;
            _whiteFx = Rocket.ShaderWhite();

            _rocketGreen = Rocket.MeshGreen();
            _modelMatGreen = Rocket.TransformGreen().Matrix;
            _greenFx = Rocket.ShaderGreen();

            _rocketGray = Rocket.MeshGrey();
            _modelMatGray = Rocket.TransformGrey().Matrix;
            _grayFx = Rocket.ShaderGrey();
        }

        public override void Update()
        {
            // Mouse and keyboard movement
            if (Keyboard.LeftRightAxis != 0 || Keyboard.UpDownAxis != 0)
            {
                _keys = true;
            }

            if (Mouse.LeftButton)
            {
                _keys = false;
                _angleVelHorz = -RotationSpeed * Mouse.XVel * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = -RotationSpeed * Mouse.YVel * DeltaTimeUpdate * 0.0005f;
            }
            else if (Touch != null && Touch.GetTouchActive(TouchPoints.Touchpoint_0))
            {
                _keys = false;
                var touchVel = Touch.GetVelocity(TouchPoints.Touchpoint_0);
                _angleVelHorz = -RotationSpeed * touchVel.x * DeltaTimeUpdate * 0.0005f;
                _angleVelVert = -RotationSpeed * touchVel.y * DeltaTimeUpdate * 0.0005f;
            }
            else
            {
                if (_keys)
                {
                    _angleVelHorz = -RotationSpeed * Keyboard.LeftRightAxis * DeltaTimeUpdate;
                    _angleVelVert = -RotationSpeed * Keyboard.UpDownAxis * DeltaTimeUpdate;
                }
                else
                {
                    var curDamp = (float)System.Math.Exp(-Damping * DeltaTimeUpdate);
                    _angleVelHorz *= curDamp;
                    _angleVelVert *= curDamp;
                }
            }

            _angleHorz += _angleVelHorz;
            _angleVert += _angleVelVert;
        }


        // RenderAFrame is called once a frame
        public override void RenderAFrame()
        {
            // Clear the backbuffer
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);
            RC.Viewport(0, 0, Width, Height);

            // Create the camera matrix and set it as the current ModelView transformation
            var mtxRot = float4x4.CreateRotationX(_angleVert) * float4x4.CreateRotationY(_angleHorz);
            var mtxCam = float4x4.LookAt(0, 2, -10, 0, 2, 0, 0, 1, 0);

            RC.View = mtxCam * mtxRot;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(_fovy, (float)Width / Height, ZNear, ZFar);

            //If a parameter or the transformation of the light has changed:
            //UpdateShaderParamForLight(_light);

            RC.Model = _modelMatWhite;
            RC.SetEffect(_whiteFx);
            RC.Render(_rocketWhite);

            RC.Model = _modelMatGreen;
            RC.SetEffect(_greenFx);
            RC.Render(_rocketGreen);

            RC.Model = _modelMatGray;
            RC.SetEffect(_grayFx);
            RC.Render(_rocketGray);

            // Swap buffers: Show the contents of the backbuffer (containing the currently rendered frame) on the front buffer.
            Present();
        }

        private void UpdateShaderParamForLight(Light light)
        {
            var strength = light.Strength;

            if (strength > 1.0 || strength < 0.0)
            {
                strength = M.Clamp(light.Strength, 0.0f, 1.0f);
                Diagnostics.Warn("Strength of the light will be clamped between 0 and 1.");
            }

            var lightParamStrings = new LightParamStrings(0);

            // Set parameters in view/camera space.
            RC.SetGlobalEffectParam(lightParamStrings.PositionViewSpace.GetHashCode(), float3.Zero);
            RC.SetGlobalEffectParam(lightParamStrings.Intensities.GetHashCode(), light.Color);
            RC.SetGlobalEffectParam(lightParamStrings.MaxDistance.GetHashCode(), light.MaxDistance);
            RC.SetGlobalEffectParam(lightParamStrings.Strength.GetHashCode(), strength);
            RC.SetGlobalEffectParam(lightParamStrings.OuterAngle.GetHashCode(), M.DegreesToRadians(light.OuterConeAngle));
            RC.SetGlobalEffectParam(lightParamStrings.InnerAngle.GetHashCode(), M.DegreesToRadians(light.InnerConeAngle));
            RC.SetGlobalEffectParam(lightParamStrings.Direction.GetHashCode(), float3.UnitZ);
            RC.SetGlobalEffectParam(lightParamStrings.LightType.GetHashCode(), (int)light.Type);
            RC.SetGlobalEffectParam(lightParamStrings.IsActive.GetHashCode(), light.Active ? 1 : 0);
        }
    }
}