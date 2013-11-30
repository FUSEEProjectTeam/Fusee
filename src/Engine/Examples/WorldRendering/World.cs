using System;
using System.Collections.Generic;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.WorldRendering
{
    internal class World
    {
        private readonly RenderContext _rc;
        private readonly List<Object> _objects;

        private float _globalPosX;
        private float _globalPosZ;
        private float _globalAngleX;

        private readonly float _globalPosY;

        private readonly float _speed;
        private readonly float _rotationSpeed;
        private readonly float _rotationSpeedM;

        public World(RenderContext renderCon)
        {
            _rc = renderCon;

            _objects = new List<Object>();

            _globalPosX = 0;
            _globalPosY = 0;
            _globalPosZ = 0;
            _globalAngleX = 0;

            _speed = 7;
            _rotationSpeedM = 15;
            _rotationSpeed = 0.015f;
        }

        public void AddObject(Geometry geo, ShaderMaterial m, int posX, int posY, int posZ)
        {
            _objects.Add(new Object(geo, m, posX, posY, posZ));
        }

        public void RenderWorld()
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
            {
                foreach (var t in _objects)
                {
                    t.SetAngleX(t.GetAngleX() + _rotationSpeedM*Input.Instance.GetAxis(InputAxis.MouseX));
                    t.SetAngleY(t.GetAngleY() + _rotationSpeedM*Input.Instance.GetAxis(InputAxis.MouseY));
                }
            }

            if (Input.Instance.IsKey(KeyCodes.W))
            {
                _globalPosX += _speed*(float) Math.Sin(_globalAngleX);
                _globalPosZ += _speed*(float) Math.Cos(_globalAngleX);
            }

            if (Input.Instance.IsKey(KeyCodes.S))
            {
                _globalPosX -= _speed*(float) Math.Sin(_globalAngleX);
                _globalPosZ -= _speed*(float) Math.Cos(_globalAngleX);
            }

            if (Input.Instance.IsKey(KeyCodes.A))
            {
                _globalPosX += _speed*(float) Math.Cos(_globalAngleX);
                _globalPosZ -= _speed*(float) Math.Sin(_globalAngleX);
            }

            if (Input.Instance.IsKey(KeyCodes.D))
            {
                _globalPosX -= _speed*(float) Math.Cos(_globalAngleX);
                _globalPosZ += _speed*(float) Math.Sin(_globalAngleX);
            }

            if (Input.Instance.IsKey(KeyCodes.Left))
                _globalAngleX += _rotationSpeed;

            if (Input.Instance.IsKey(KeyCodes.Right))
                _globalAngleX -= _rotationSpeed;

            ////// render all objects
            foreach (var t in _objects)
            {
                _rc.SetShader(t.GetShader(_rc));

                // colh
                //var mtxRot = float4x4.CreateRotationY(t.GetAngleX())*float4x4.CreateRotationX(-t.GetAngleY());
                //var mtxTrans = float4x4.CreateTranslation(t.GetPosX() - _globalPosX, t.GetPosY() + _globalPosY,
                //                                          t.GetPosZ() - _globalPosZ);
                //var mtxLook = float4x4.LookAt(0, 0, 0, (float) Math.Sin(_globalAngleX), 0,
                //                              (float) Math.Cos(_globalAngleX), 0, 1, 0);

                //_rc.ModelView = mtxRot*mtxTrans*mtxLook;

                var mtxRot = float4x4.CreateRotationX(t.GetAngleY()) * float4x4.CreateRotationY(-t.GetAngleX());
                var mtxTrans = float4x4.CreateTranslation(t.GetPosX() - _globalPosX, t.GetPosY() + _globalPosY,
                                                          t.GetPosZ() + _globalPosZ);
                var mtxLook = float4x4.LookAt(0, 0, 0, (float)Math.Sin(-_globalAngleX), 0,
                                              (float)Math.Cos(-_globalAngleX), 0, 1, 0);

                _rc.ModelView = mtxLook * mtxTrans * mtxRot;
                
                
                _rc.Render(t.GetMesh());
            }
        }
    }
}