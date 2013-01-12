using System;
using Fusee.Engine;
using Fusee.Math;

namespace Examples.CubeAndTiles
{
    public class RollingCube
    {
        // vars
        private readonly float3 _cubeColor;
        private readonly Mesh _cubeMesh;
        private readonly Level _curLevel;

        private static float[] _rotateYX;
        private static int[] _curDirXY;
        private static Quaternion _orientQuat;

        private float _posZ;
        private float _veloZ;
        private float _curBright;

        internal int[] PosCurXY { get; private set; }
        internal int[] PosLastXY { get; private set; }
        internal CubeStates State { get; private set; }

        // const
        private const float PiHalf = (float) Math.PI/2.0f;
        internal const int CubeSize = 200;
        private const float CubeSpeed = 4.0f;

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

            PosCurXY = new int[2];
            PosLastXY = new int[2];

            _rotateYX = new float[2];
            _curDirXY = new int[2];

            _orientQuat = Quaternion.Identity;

            _posZ = 0;
            _veloZ = 0.0f;
            _curBright = 0.0f;

            ResetCube(0, 0);
        }

        // methods
        public void ResetCube(int x, int y)
        {
            State = CubeStates.CsLoading;

            PosCurXY[0] = x;
            PosCurXY[1] = y;

            for (var i = 0; i < 1; i++)
            {
                _rotateYX[i] = 0.0f;
                _curDirXY[i] = 0;
            }

            _orientQuat = Quaternion.Identity;

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
            if (_curDirXY[0] + _curDirXY[1] == 0)
            {
                PosLastXY[0] = PosCurXY[0];
                PosLastXY[1] = PosCurXY[1];

                _curDirXY[0] = dirX;
                _curDirXY[1] = dirY;

                return true;
            }

            _curLevel.SetDeadField(PosLastXY[0], PosLastXY[1]);

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
                if (_curDirXY[i] == 0) continue;

                // rotate and check if target reached
                _rotateYX[i] += CubeSpeed*_curLevel.LvlDeltaTime;
                if (_rotateYX[i] < PiHalf) continue;

                PosCurXY[i] += _curDirXY[i];

                // rotation with quaterions
                float3 rotVektor;

                if (i == 0)
                    rotVektor = new float3(-_curDirXY[0] * PiHalf, 0, 0);
                else
                    rotVektor = new float3(0, 0, _curDirXY[1] * PiHalf);

                _orientQuat *= Quaternion.EulerToQuaternion(rotVektor);
                _orientQuat.Normalize();

                // reset
                _rotateYX[i] = 0;
                _curDirXY[i] = 0;

                // check if special/dead field
                _curLevel.CheckField(PosCurXY);
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
            var mtxObjRot = float4x4.CreateRotationY(_rotateYX[0]*_curDirXY[0])*
                            float4x4.CreateRotationX(-_rotateYX[1]*_curDirXY[1]);

            var mtxObjOrientRot = Quaternion.QuaternionToMatrix(_orientQuat);

            // cube position
            var mtxObjPos = float4x4.CreateTranslation(PosCurXY[0]*CubeSize, PosCurXY[1]*CubeSize,
                                                       _posZ*CubeSize);

            var arAxis = float4x4.CreateTranslation(-100 * _curDirXY[0], -100 * _curDirXY[1], 100);
            var invArAxis = float4x4.CreateTranslation(100 * _curDirXY[0], 100 * _curDirXY[1], -100);

            // set modelview and color of cube
            _curLevel.RContext.ModelView = _curLevel.AddCameraTrans(mtxObjOrientRot * arAxis * mtxObjRot * invArAxis * mtxObjPos);
            _curLevel.RContext.SetShaderParam(_curLevel.VColorObj, new float4(_cubeColor, _curBright));

            // render
            _curLevel.RContext.Render(_cubeMesh);
        }
    }
}