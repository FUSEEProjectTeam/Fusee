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

        private static float[] _rotateYx;
        private static int[] _curDirXy;

        // const
        private const float PiHalf = (float) Math.PI / 2.0f;
        private const int CubeSize = 200;
        private const float CubeSpeed = 3.0f;

        // constructor
        public RollingCube(Level curLevel)
        {
            _curLevel = curLevel;
            _cubeMesh = _curLevel.GlobalCubeMesh;
            _cubeColor = new float4(0.5f, 0.15f, 0.17f, 1.0f);

            PosCurXy = new int[2];
            PosLastXy = new int[2];

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
                _rotateYx[i] = 0.0f;
                _curDirXy[i] = 0;
            }
        }

        public bool MoveCube(sbyte dirX, sbyte dirY)
        {
            if (_curDirXy[0] + _curDirXy[1] == 0)
            {
                PosLastXy[0] = PosCurXy[0];
                PosLastXy[1] = PosCurXy[1];

                _curDirXy[0] = dirX;
                _curDirXy[1] = dirY;

                return true;
            }

            return false;   
        }

        private void AnimCube()
        {
            // 1st: moving in x direction
            // 2nd: moving in y direction
            for (var i = 0; i <= 1; i++)
            {
                if (_curDirXy[i] == 0) continue;

                // rotate and check if target reached
                _rotateYx[i] += CubeSpeed * _curLevel.LvlDeltaTime;
                if (_rotateYx[i] < PiHalf) continue;

                PosCurXy[i] += _curDirXy[i];

                _rotateYx[i] = 0;
                _curDirXy[i] = 0;

                _curLevel.CheckField(PosLastXy, PosCurXy);
            }
        }

        public void RenderCube()
        {
            // anim cube and get translations
            AnimCube();

            // set cube translation
            var mtxObjRot = float4x4.CreateRotationY(_rotateYx[0]*_curDirXy[0])*
                            float4x4.CreateRotationX(-_rotateYx[1]*_curDirXy[1]);

            var mtxObjPos = float4x4.CreateTranslation(PosCurXy[0]*CubeSize, PosCurXy[1]*CubeSize,
                                                       CubeSize/2.0f + 15);

            var arAxis = float4x4.CreateTranslation(-100*_curDirXy[0], -100*_curDirXy[1], 100);
            var invArAxis = float4x4.CreateTranslation(100*_curDirXy[0], 100*_curDirXy[1], -100);

            // set modelview and color of cube
            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(arAxis*mtxObjRot*invArAxis*mtxObjPos);
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, _cubeColor);

            // render
            _curLevel.RContext.Render(_cubeMesh);
        }
    }
}