using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.EgoPerspective
{
    class World
    {
        private RenderContext RC;
        private Input In;
        private List<Object> objects;

        private float _globalPosX;
        private float globalPosY;
        private float _globalPosZ;
        private float _globalAngleX;
        private float _globalAngleY;
        private float _globalAngleZ;
        private readonly float _speed;
        private readonly float _rotationSpeed;
        private readonly float _rotationSpeedM;



        public World(RenderContext renderCon, Input input)
        {
            RC = renderCon;
            In = input;
            objects = new List<Object>();
            _globalPosX = 0;
            globalPosY = 0;
            _globalPosZ = 0;
            _globalAngleX = 0;
            _globalAngleY = 0;
            _globalAngleZ = 0;
            _speed = 7;
            _rotationSpeedM = 15;
            _rotationSpeed = 0.015f;

        }





        public void addObject(Geometry geo, ShaderMaterial m, int posX, int posY, int posZ)
        {
            objects.Add(new Object(geo, m, posX, posY, posZ));
        }


        public void RenderWorld(float drehungHorz)
        {

            if (In.IsButtonDown(MouseButtons.Left))
            {
                foreach (Object t in objects)
                {
                    t.SetAngleX(t.GetAngleX()+_rotationSpeedM*In.GetAxis(InputAxis.MouseX)); // *deltatime
                    t.SetAngleY(t.GetAngleY()+_rotationSpeedM*In.GetAxis(InputAxis.MouseY));
                }
            }
            if (In.IsKeyDown(KeyCodes.W))
            {
                _globalPosX += _speed*(float) Math.Sin(_globalAngleX);
                _globalPosZ += _speed*(float) Math.Cos(_globalAngleX);
            }
            if (In.IsKeyDown(KeyCodes.S))
            {
                _globalPosX -= _speed*(float) Math.Sin(_globalAngleX);
                _globalPosZ -= _speed*(float) Math.Cos(_globalAngleX);
            }
            if (In.IsKeyDown(KeyCodes.A))
            {
                _globalPosX += _speed*(float) Math.Cos(_globalAngleX);
                _globalPosZ -= _speed*(float) Math.Sin(_globalAngleX);
            }
            if (In.IsKeyDown(KeyCodes.D))
            {
                _globalPosX -= _speed*(float) Math.Cos(_globalAngleX);
                _globalPosZ += _speed*(float) Math.Sin(_globalAngleX);
            }
            if (In.IsKeyDown(KeyCodes.Left))
            {
                _globalAngleX += _rotationSpeed;
            }
            if (In.IsKeyDown(KeyCodes.Right))
            {
                _globalAngleX -= _rotationSpeed;
            }

            ////// render all objects
            foreach (Object t in objects)
            {
                //RC.SetShader(t.GetShader());
                float4x4 mtxRot = float4x4.CreateRotationY(t.GetAngleX()) * float4x4.CreateRotationX(-t.GetAngleY());
                float4x4 mtxCam = float4x4.LookAt(0, 200, 400, 0, 50, 0, 0, 1, 0);
                float4x4 mtxTrans = float4x4.CreateTranslation(t.GetPosX() - _globalPosX, t.GetPosY() + globalPosY,
                                                          t.GetPosZ() - _globalPosZ);
                float4x4 mtxLook = float4x4.LookAt(0, 0, 0, (float) Math.Sin(_globalAngleX), 0,
                                                (float) Math.Cos(_globalAngleX), 0, 1, 0);
                RC.ModelView = mtxRot  *mtxTrans * mtxLook;
                RC.Render(t.GetMesh());
            }


        }


    }

}