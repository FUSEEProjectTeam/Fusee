using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.MyTestGame
{
    public class RollingCube
    {
        // vars
        private readonly float3 _cubeColor;
        private readonly Mesh _cubeMesh;
        private readonly Level _curLevel;

        private static float[] _rotateYx;
        private static int[] _curDirXy;

        private float _posZ;
        private float _veloZ;
        private float _curBright;

        internal int[] PosCurXy { get; private set; }
        internal int[] PosLastXy { get; private set; }
        internal CubeStates State { get; private set; }

        // const
        private const float PiHalf = (float) Math.PI/2.0f;
        private const int CubeSize = 200;
        private const float CubeSpeed = 5.0f;

        public enum CubeStates
        {
            CsLoading,
            CsAlive,
            CsWinning,
            CsWon,
            CsDying,
            CsDead
        }

        // constructor
        public RollingCube(Level curLevel)
        {
            _curLevel = curLevel;
            _cubeMesh = _curLevel.GlobalCubeMesh;
            _cubeColor = new float3(0.5f, 0.15f, 0.17f);

            PosCurXy = new int[2];
            PosLastXy = new int[2];

            _rotateYx = new float[2];
            _curDirXy = new int[2];

            _posZ = 0;
            _veloZ = 0.0f;
            _curBright = 0.0f;

            ResetCube(0, 0);
        }

        // methods
        public void ResetCube(int x, int y)
        {
            State = CubeStates.CsLoading;

            PosCurXy[0] = x;
            PosCurXy[1] = y;

            for (var i = 0; i < 1; i++)
            {
                _rotateYx[i] = 0.0f;
                _curDirXy[i] = 0;
            }

            _posZ = 2;
            _veloZ = -1.0f;
            _curBright = 0;
        }

        public void DeadCube()
        {
            if (State != CubeStates.CsDying)
            {
                State = CubeStates.CsDying;

                _posZ = 0;
                _veloZ = -0.2f;
            }
        }

        public void WinningCube()
        {
            if (State != CubeStates.CsWon)
            {
                State = CubeStates.CsWinning;

                _posZ = 0;
                _veloZ = +0.2f;
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

            _curLevel.SetDeadField(PosLastXy[0], PosLastXy[1]);

            return false;
        }

        private void LoadAnimation()
        {
            if (State != CubeStates.CsLoading) return;
           
            _veloZ = Math.Min(-0.01f, -_posZ / 10.0f);
            _posZ += _veloZ;

            _curBright = 1 - (_posZ/2);

            if (_posZ < 0.01f)
            {
                _posZ = 0;
                _veloZ = 0;
                _curBright = 1.0f;

                State = CubeStates.CsAlive;
            }
        }

        private void WinningAnimation()
        {
            if (State != CubeStates.CsWinning) return;

            if (_curBright > 0.0f)
            {
                _posZ += _veloZ;
                _curBright -= .05f;
            }
            else
                State = CubeStates.CsWon;            
        }

        private void DeadAnimation()
        {
            if (State != CubeStates.CsDying) return;

            if (_curBright > 0.0f)
            {
                _posZ += _veloZ;
                _curBright -= .05f;
            }
            else
                State = CubeStates.CsDead;
        }

        private void AnimCube()
        {
            // 1st: moving in x direction
            // 2nd: moving in y direction
            for (var i = 0; i <= 1; i++)
            {
                if (_curDirXy[i] == 0) continue;

                // rotate and check if target reached
                _rotateYx[i] += CubeSpeed*_curLevel.LvlDeltaTime;
                if (_rotateYx[i] < PiHalf) continue;

                PosCurXy[i] += _curDirXy[i];

                _rotateYx[i] = 0;
                _curDirXy[i] = 0;

                _curLevel.CheckField(PosCurXy);
            }
        }

        public void RenderCube()
        {
            // anim cube while loading, dying, winning, moving
            LoadAnimation();
            DeadAnimation();
            WinningAnimation();

            AnimCube();

            // set cube translation
            var mtxObjRot = float4x4.CreateRotationY(_rotateYx[0]*_curDirXy[0])*
                            float4x4.CreateRotationX(-_rotateYx[1]*_curDirXy[1]);

            var mtxObjPos = float4x4.CreateTranslation(PosCurXy[0]*CubeSize, PosCurXy[1]*CubeSize,
                                                       _posZ*CubeSize + (CubeSize/2.0f + 15));

            var arAxis = float4x4.CreateTranslation(-100*_curDirXy[0], -100*_curDirXy[1], 100);
            var invArAxis = float4x4.CreateTranslation(100*_curDirXy[0], 100*_curDirXy[1], -100);

            // set modelview and color of cube
            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(arAxis*mtxObjRot*invArAxis*mtxObjPos);

            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(_cubeColor, _curBright));

            // render
            _curLevel.RContext.Render(_cubeMesh);
        }
    }
}