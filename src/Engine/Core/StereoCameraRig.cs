using System;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// A S3D camera rig. 
    /// </summary>
    public class StereoCameraRig : Stereo3D
    {
        private float4x4 _leftFrustum, _rightFrustum;
        public float FocalLength { get; private set; }

        public float Iod
        {
            get { return Stereo3DParams.EyeDistance; }
            set { Stereo3DParams.EyeDistance = value; }
        }

        public StereoCameraRig(Stereo3DMode mode, int width, int height, float iod = 6.5f) : base(mode, width, height)
        {
            Iod = iod;
        }


        /// <summary>
        /// Extends base. Prepare(Stereo3DEye eye) by setting projrction matrix whether left or right eye is rendered
        /// </summary>
        /// <param name="eye"></param>
        public override void Prepare(Stereo3DEye eye)
        {
            _rc.Projection = (eye == Stereo3DEye.Left) ? _leftFrustum : _rightFrustum;
            base.Prepare(eye);
        }

        //LookAt3D - Frustum Shift
        public override float4x4 LookAt3D(Stereo3DEye eye, float3 eyeV, float3 target, float3 up)
        {
            //shifttranslation
            var x = (eye == Stereo3DEye.Left)
                ? eyeV.x - Iod / 2
                : eyeV.x + Iod / 2;

            var newEye = new float3(x, eyeV.y, eyeV.z);
            var newTarget = new float3(x, target.y, target.z);

            return float4x4.LookAt(newEye, newTarget, up);
        }


        /// <summary>
        /// Calculating left/right frustum/projection matrix
        /// </summary>
        /// <param name="rc">The <see cref="RenderContext" /></param>
        /// <param name = "fovy" > Angle of the field of view in the y direction(in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="focalLength">Deistance to Camera where parallax is zero (zero parallax plane)</param>
        public void SetFrustums(RenderContext rc, float fovy, float aspectRatio, float zNear, float zFar, float focalLength)
        {
            FocalLength = focalLength;
            _leftFrustum = ViewFrustumShifted(fovy, aspectRatio, zNear, zFar, focalLength, true);
            _rightFrustum = ViewFrustumShifted(fovy, aspectRatio, zNear, zFar, focalLength, false);
            rc.Projection = _leftFrustum;
        }

        /// <summary>
        ///     Creates a left handed perspective projection matrix when using SteroCameraRig -> Frustrum shift / Off-axis.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="focalLength">distance to convergence plane. </param>
        /// <param name="lefteye">defines if frustum is created for left odr right camera</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     Thrown under the following conditions:
        ///     <list type="bullet">
        ///         <item>fovy is zero, less than zero or larger than Math.PI</item>
        ///         <item>aspect is negative or zero</item>
        ///         <item>zNear is negative or zero</item>
        ///         <item>zFar is negative or zero</item>
        ///         <item>zNear is larger than zFar</item>
        ///     </list>
        /// </exception>
        /// More information on frustum shift and stereoscopic rendering in general can be found here: http://paulbourke.net/stereographics/stereorender/
        private float4x4 ViewFrustumShifted(float fovy, float aspect, float zNear, float zFar, float focalLength,
            bool lefteye)
        {
            //Allgemein
            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");


            var top = (float)System.Math.Tan(fovy * 0.5f) * zNear;
            var bottom = -top;

            var shiftLr = lefteye ? -1 : 1;
            var shiftOffset = (Iod * 0.5f) * (zNear / focalLength);
            var left = -aspect * top + shiftOffset * shiftLr;
            var right = aspect * top + shiftOffset * shiftLr;

            float4x4 result;
            float4x4.CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
    }
}
