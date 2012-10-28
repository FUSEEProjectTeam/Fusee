using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class RollingCube
    {
        // vars
        private readonly Level _curLevel;

        private readonly Mesh _cubeMesh;
        private readonly float4 _cubeColor;

        internal int[] PosCurXy { get; private set; }
        internal int[] PosLastXy { get; private set; }

        private static int[] _orientYx;
        private static float[] _rotateYx;
        private static int[] _curDirXy;

        // const
        private const float PiHalf = (float) Math.PI / 2.0f;
        private const int CubeSize = 200;
      
        // structs
        private struct Translation2D
        {
            internal float[] PosXy;
            internal float[] RotateYx;
        }

        // constructor
        public RollingCube(Level curLevel)
        {
            _curLevel = curLevel;
            _cubeMesh = MeshReader.LoadMesh("SampleObj/Cube.obj.model");
            _cubeColor = new float4(0.5f, 0.15f, 0.17f, 1.0f);

            PosCurXy = new int[2];
            PosLastXy = new int[2];

            _orientYx = new int[2];
            _rotateYx = new float[2];
            _curDirXy = new int[2];

            ResetCube(0, 0);
        }

        // methods
        public void ResetCube(int x, int y)
        {
            PosCurXy[0] = x;
            PosCurXy[1] = y;

            for (int i = 0; i < 1; i++)
            {
                _orientYx[i] = 0;
                _rotateYx[i] = 0.0f;
                _curDirXy[i] = 0;
            }
        }

        public bool MoveCube(sbyte dirX, sbyte dirY)
        {
            if (_curDirXy[0] + _curDirXy[1] == 0)
            {
                PosLastXy = PosCurXy;

                _curDirXy[0] = dirX;
                _curDirXy[1] = dirY;

                for (int i = 0; i <= 1; i++)
                {
                    _orientYx[i] += _curDirXy[i];
                    _orientYx[i] = (int)(_orientYx[i] - Math.Floor(_orientYx[i] / 4.0) * 4);
                    _rotateYx[i] = (_orientYx[i] - _curDirXy[i])*PiHalf;
                }

                return true;
            }

            return false;   
        }

        private Translation2D AnimCube()
        {
            var trans = new Translation2D
                            {PosXy = new float[2] {PosCurXy[0], PosCurXy[1]}, RotateYx = new float[] {0, 0}};

            var rotateTargYx = new[] { _orientYx[0] * PiHalf, _orientYx[1] * PiHalf };

            for (var i = 0; i <= 1; i++)
            {
                if (_curDirXy[i] == 0) continue;

                _rotateYx[i] += _curDirXy[i] * 2.0f * _curLevel.DeltaTime;

                float progr = 1 - (Math.Abs(rotateTargYx[i] - _rotateYx[i]) / PiHalf);
                float curPos = progr * _curDirXy[i];

                trans.PosXy[i] = PosCurXy[i] + curPos;

                // rotation target reached?
                if ((rotateTargYx[i] - _rotateYx[i]) * _curDirXy[i] < 0)
                {
                    _rotateYx[i] = _orientYx[i] * PiHalf;

                    PosCurXy[i] += _curDirXy[i];
                    trans.PosXy[i] = PosCurXy[i];

                    _curDirXy[i] = 0;
                    _curLevel.CheckField(PosLastXy, PosCurXy);
                }

                trans.RotateYx[i] = _rotateYx[i];
            }

            return trans;
        }

        public void RenderCube(float4x4 camPosition, float4x4 camTranslation, float4x4 objectOrientation, float4x4 mtxRot)
        {
            // anim cube and get translations
            var transCube = AnimCube();

            // set cube translation
            var mtxObjRot = float4x4.CreateRotationY(transCube.RotateYx[0])*
                            float4x4.CreateRotationX(-transCube.RotateYx[1]);

            var mtxObjPos = float4x4.CreateTranslation(transCube.PosXy[0] * CubeSize, transCube.PosXy[1] * CubeSize, 110);

            // set modelview and color of cube
            _curLevel.RContext.ModelView = mtxObjRot * mtxObjPos * camTranslation * mtxRot * camPosition;
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, _cubeColor);

            // render
            _curLevel.RContext.Render(_cubeMesh);
        }
    }
}