using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    public class Player : GameEntity
    {
        private float3 _rotation;
        private float3 _rotationSpeed = new float3(1, 1, 1);
        private float _speed;
        private float _speedModifier = 1;
        private float SpeedMax = 30f;

        public Player(String meshPath, RenderContext rc, float posX = 0, float posY = 0, float posZ = 0, float angX = 0, float angY = 0, float angZ = 0)
            : base(meshPath, rc, posX, posY, posZ, angX, angY, angZ)
        {
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public void Move()
        {
            if (Input.Instance.IsKey(KeyCodes.D))
                _rotation.x = _rotationSpeed.x * (float)Time.Instance.DeltaTime;
            else if (Input.Instance.IsKey(KeyCodes.A))
                _rotation.x = -_rotationSpeed.x * (float)Time.Instance.DeltaTime;
            else
                _rotation.x = 0;

            if (Input.Instance.IsKey(KeyCodes.W))
                _rotation.y = _rotationSpeed.y * (float)Time.Instance.DeltaTime;
            else if (Input.Instance.IsKey(KeyCodes.S))
                _rotation.y = -_rotationSpeed.x * (float)Time.Instance.DeltaTime;
            else
                _rotation.y = 0;

            if (Input.Instance.IsKey(KeyCodes.E))
                _rotation.z = _rotationSpeed.z * (float)Time.Instance.DeltaTime;
            else if (Input.Instance.IsKey(KeyCodes.Q))
                _rotation.z = -_rotationSpeed.z * (float)Time.Instance.DeltaTime;
            else
                _rotation.z = 0;

            if (Input.Instance.IsKey(KeyCodes.Up) || Input.Instance.IsKey(KeyCodes.Space))
            {
                _speed += _speedModifier * (float)Time.Instance.DeltaTime;
            }
            else
            {
                _speed -= _speedModifier * (float)Time.Instance.DeltaTime;
            }

            _speed = Clamp(_speed, 0.0f, SpeedMax);

            Position *= float4x4.CreateTranslation(-Position.Row3.xyz) *
                        float4x4.CreateFromAxisAngle(float3.Normalize(Position.Row1.xyz), -_rotation.x) *
                        float4x4.CreateFromAxisAngle(float3.Normalize(Position.Row0.xyz), -_rotation.y) *
                        float4x4.CreateFromAxisAngle(float3.Normalize(Position.Row2.xyz), -_rotation.z) *
                        float4x4.CreateTranslation(Position.Row3.xyz) *
                        float4x4.CreateTranslation(float3.Normalize(Position.Row2.xyz) * -_speed);
        }

        public float4x4 GetCamMatrix()
        {
            return float4x4.LookAt(Position.M41 + (float3.Normalize(Position.Row2.xyz).x),
                                   Position.M42 + (float3.Normalize(Position.Row2.xyz).y),
                                   Position.M43 + (float3.Normalize(Position.Row2.xyz).z),
                                   Position.M41,
                                   Position.M42,
                                   Position.M43,
                                   Position.M21,
                                   Position.M22,
                                   Position.M23)
                                   * float4x4.CreateTranslation(0, -300, -1000);
        }

        private static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
