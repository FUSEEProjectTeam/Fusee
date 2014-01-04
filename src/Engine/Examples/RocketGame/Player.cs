using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{

    public class Player : GameEntity
    {

        public Player(String meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0) : base(meshPath, rc, posX, posY, posZ, angX, angY, angZ)
        {

        }

        public Player(String meshPath, RenderContext rc, float3 posxyz, float3 angxyz) : base(meshPath, rc, posxyz, angxyz)
        {
        }

        public void Move()
        {
            Rotation.x = Input.Instance.GetAxis(InputAxis.MouseX);
            Rotation.y = Input.Instance.GetAxis(InputAxis.MouseY);


            if (Input.Instance.IsKeyDown(KeyCodes.W))
            {
                Speed += 0.1f;
            }

            if (Input.Instance.IsKeyDown(KeyCodes.S))
            {
                Speed -= 0.1f;
            }

            Speed = Clamp(Speed, 0.0f, 5.0f);

            var oldPos3 = new float3(Position.Row3);

            Position *= float4x4.CreateTranslation(-oldPos3) *
                         float4x4.CreateFromAxisAngle(NRotYV, -Rotation.x) *
                         float4x4.CreateFromAxisAngle(NRotXV, -Rotation.y) *
                         float4x4.CreateTranslation(oldPos3) *
                         float4x4.CreateTranslation(NRotZV * -Speed);

            UpdateNVectors();

        }

        public float4x4 GetCamMatrix()
        {

            return float4x4.LookAt(Position.M41 + (NRotZV.x * 1000), Position.M42 + (NRotZV.y * 1000), Position.M43 + (NRotZV.z * 1000),
                                   Position.M41, Position.M42, Position.M43,
                                   Position.M21, Position.M22, Position.M23)
                                   * float4x4.CreateTranslation(0, -300, 0);
        }

        // PRIVATE move to static class later

        private static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
