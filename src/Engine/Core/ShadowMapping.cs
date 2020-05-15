using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusee.Engine.Core
{
    internal class ShadowParams
    {
        // For omni directional shadow mapping there will be six LightSpaceMatrices
        // to allow the creation of the cube map in one pass.
        public float4x4[] LightSpaceMatrices;
        public IWritableTexture[] ShadowMaps;
        public float2[] ClipPlanesForLightMat;
    }

    /// <summary>
    /// Saves a axis aligned bounding box in light space and the clipping planes of the associated sub-frustum.
    /// </summary>
    internal struct Cascade 
    {
        public AABBf Aabb;
        public float2 ClippingPlanes;        
    }

    /// <summary>
    /// Utility class for creating shadow maps.
    /// </summary>
    internal static class ShadowMapping
    {
        //For an explanation of Parallel Split Shadow Maps see: https://developer.nvidia.com/gpugems/GPUGems3/gpugems3_ch10.html and https://developer.download.nvidia.com/SDK/10.5/opengl/src/cascaded_shadow_maps/doc/cascaded_shadow_maps.pdf
        #region Parallel split shadow mapping

        /// <summary>
        /// Calculates the <see cref="Cascade"/> for each sub-frustum, used for cascaded shadow mapping.
        /// The algorithm, used to split the viewing frustum is "Parallel Split Shadow Maps"
        /// </summary>
        /// <param name="numberOfCascades">The number of times the viewing frustum is divided. A Cascade is created for each of the sub-frustum.</param>
        /// <param name="lightView">The lights view matrix.</param>
        /// <param name="lambda">A constant value, used to weight the logarithmic split with the uniform split of the viewing frustum.</param>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="width">The window width in px.</param>
        /// <param name="height">The window height in px.</param>
        /// <param name="fov">The field of view of the camera.</param>
        /// <param name="view">The view matrix.</param>        
        public static IEnumerable<Cascade> ParallelSplitCascades(int numberOfCascades, float4x4 lightView, float lambda, float zNear, float zFar, int width, int height, float fov, float4x4 view)
        {
            var frustumCorners = CascadeCornersWorldSpace(numberOfCascades, lambda, zNear, zFar, width, height, fov, view).ToList();
            foreach (Tuple<float4[], float2> tuple in frustumCorners)
            {
                var aabb = FrustumAABBLightSpace(lightView, tuple.Item1);
                yield return new Cascade { Aabb = aabb, ClippingPlanes = tuple.Item2 };
            }
        }
        
        /// <summary>
        /// Returns the clipping planes for each sub-frustum.
        /// </summary>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="numberOfCascades">The number of times the viewing frustum is divided. A Cascade is created for each of the sub-frustum.</param>
        /// <param name="lambda">A constant value, used to wight the logarithmic division with the logarithmic division of the viewing frustum.</param>        
        private static IEnumerable<float> GetClippingPlanesOfSplitFrustums(float zNear, float zFar, int numberOfCascades, float lambda)
        {
            for (int i = 0; i < numberOfCascades + 1; i++)
            {
                var splitOverNoOfSplits = i / (float)numberOfCascades;
                yield return SplitClipPlane(zNear, zFar, splitOverNoOfSplits, lambda);
            }
        }

        /// <summary>
        /// Uniform calculation of the i'th clipping plane.
        /// </summary>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="splitOverNoOfCascades">A fraction consisting of i and the number of cascades.</param>        
        private static float SplitClipPlaneUniform(float zNear, float zFar, float splitOverNoOfCascades)
        {
            return zNear + (zFar - zNear) * splitOverNoOfCascades;
        }

        /// <summary>
        /// Logarithmic calculation of the i'th clipping plane.
        /// </summary>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="splitOverNoOfCascades">A fraction consisting of i and the number of cascades.</param>        
        private static float SplitClipPlaneLog(float zNear, float zFar, float splitOverNoOfCascades)
        {
            return zNear * (float)System.Math.Pow((zFar / zNear), splitOverNoOfCascades);
        }

        /// <summary>
        /// Weighted calculation of the i'th clipping plane.
        /// </summary>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="splitOverNoOfCascades">A fraction consisting of i and the number of cascades.</param>   
        /// <param name="lambda">A constant value, used to weight the logarithmic split with the uniform split of the viewing frustum.</param>
        private static float SplitClipPlane(float zNear, float zFar, float splitOverNoOfCascades, float lambda)
        {
            return lambda * SplitClipPlaneLog(zNear, zFar, splitOverNoOfCascades) + (1 - lambda) * SplitClipPlaneUniform(zNear, zFar, splitOverNoOfCascades);
        }

        /// <summary>
        /// Calculates the axis aligned bounding box in light space.
        /// </summary>
        /// <param name="lightView">The view matrix of the light.</param>
        /// <param name="frustumCorners">The world space frustum corners of a cascade.</param>
        /// <returns></returns>
        private static AABBf FrustumAABBLightSpace(float4x4 lightView, float4[] frustumCorners)
        {
            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner = lightView * corner; //light space frustum corners
                frustumCorners[i] = corner;
            }

            var lightSpaceFrustumAABB = new AABBf(frustumCorners[0].xyz, frustumCorners[0].xyz);
            foreach (var p in frustumCorners)
            {
                lightSpaceFrustumAABB |= p.xyz;
            }

            return lightSpaceFrustumAABB;
        }

        /// <summary>
        /// Calculates the world space frustum corners from a projection (here: sub-frustum) and a view matrix.
        /// </summary>
        /// <param name="projectionMatrix">The projection matrix.</param>
        /// <param name="viewMatrix">The view matrix.</param>
        /// <returns></returns>
        private static float4[] GetWorldSpaceFrustumCorners(float4x4 projectionMatrix, float4x4 viewMatrix)
        {
            //1. Calculate the 8 corners of the view frustum in world space. This can be done by using the inverse view-projection matrix to transform the 8 corners of the NDC cube (which in OpenGL is [‒1, 1] along each axis).
            //2. Transform the frustum corners to a space aligned with the shadow map axes.This would commonly be the directional light object's local space. 
            //In fact, steps 1 and 2 can be done in one step by combining the inverse view-projection matrix of the camera with the inverse world matrix of the light.
            var invViewProjection = float4x4.Invert(projectionMatrix * viewMatrix);

            var frustumCorners = new float4[8];

            frustumCorners[0] = invViewProjection * new float4(-1, -1, -1, 1); //nbl
            frustumCorners[1] = invViewProjection * new float4(1, -1, -1, 1); //nbr 
            frustumCorners[2] = invViewProjection * new float4(-1, 1, -1, 1); //ntl  
            frustumCorners[3] = invViewProjection * new float4(1, 1, -1, 1); //ntr  
            frustumCorners[4] = invViewProjection * new float4(-1, -1, 1, 1); //fbl 
            frustumCorners[5] = invViewProjection * new float4(1, -1, 1, 1); //fbr 
            frustumCorners[6] = invViewProjection * new float4(-1, 1, 1, 1); //ftl  
            frustumCorners[7] = invViewProjection * new float4(1, 1, 1, 1); //ftr     

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                var corner = frustumCorners[i];
                corner /= corner.w; //world space frustum corners               
                frustumCorners[i] = corner;
            }

            return frustumCorners;
        }

        /// <summary>
        /// Returns a tuple of a projection matrix and the clipping planes for each sub-frustum.
        /// </summary>
        /// <param name="numberOfCascades">The number of times the viewing frustum is divided. A Cascade is created for each of the sub-frustum.</param>      
        /// <param name="lambda">A constant value, used to weight the logarithmic split with the uniform split of the viewing frustum.</param>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="width">The window width in px.</param>
        /// <param name="height">The window height in px.</param>
        /// <param name="fov">The field of view of the camera.</param>              
        private static IEnumerable<Tuple<float4x4, float2>> CascadesProjectionMatrices(int numberOfCascades, float lambda, float zNear, float zFar, int width, int height, float fov)
        {
            var clipPlanes = GetClippingPlanesOfSplitFrustums(zNear, zFar, numberOfCascades, lambda).ToList();
            
            for (int i = 0; i < clipPlanes.Count - 1; i++)
            {
                var cascadeNear = clipPlanes[i];
                var cascadeFar = clipPlanes[i + 1];

                //Subtract buffer value from cascades near plane to avoid artifacts from blending cascades.
                if (i > 0)
                {
                    float bufferPercent = 100 - System.Math.Max(85.0f - (5.0f * (i-1)), 50.0f); //The same function (max) must be used in the shader while blending the cascades.
                    var zPrecedingCascade = clipPlanes[i] - clipPlanes[i - 1];
                    var bufferLength = zPrecedingCascade / 100 * bufferPercent;
                    cascadeNear -= bufferLength;
                }

                var thisCascadesClipPlanes = new float2(cascadeNear, cascadeFar);                
                var aspect = (float)width / height;
                yield return new Tuple<float4x4, float2>(float4x4.CreatePerspectiveFieldOfView(fov * 2, aspect, cascadeNear, cascadeFar), thisCascadesClipPlanes);
            }
        }

        /// <summary>
        /// Returns a tuple of the world space frustum corners and the clipping planes for each sub-frustum.
        /// </summary>
        /// <param name="numberOfCascades">The number of times the viewing frustum is divided. A Cascade is created for each of the sub-frustum.</param>      
        /// <param name="lambda">A constant value, used to weight the logarithmic split with the uniform split of the viewing frustum.</param>
        /// <param name="zNear">The near clipping plane of the camera.</param>
        /// <param name="zFar">The far clipping plane of the camera.</param>
        /// <param name="width">The window width in px.</param>
        /// <param name="height">The window height in px.</param>
        /// <param name="fov">The field of view of the camera.</param>    
        /// <param name="view">The view matrix.</param> 
        private static IEnumerable<Tuple<float4[], float2>> CascadeCornersWorldSpace(int numberOfCascades, float lambda, float zNear, float zFar, int width, int height, float fov, float4x4 view)
        {
            var allSplitProjectionMatrices = CascadesProjectionMatrices(numberOfCascades, lambda, zNear, zFar, width, height, fov);

            foreach (Tuple<float4x4, float2> tuple in allSplitProjectionMatrices)
                yield return new Tuple<float4[], float2>(GetWorldSpaceFrustumCorners(tuple.Item1, view),tuple.Item2);
        }       

        #endregion

        /// <summary>
        /// Creates a orthographic projection matrix from a bounding box. This matrix is named "crop" matrix in the literature.
        /// </summary>
        /// <param name="aabb">The bounding box.</param>
        /// <returns></returns>
        public static float4x4 CreateOrthographic(AABBf aabb)
        {
            aabb.min.z = 0;
            var scaleX = 2.0f / (aabb.max.x - aabb.min.x);
            var scaleY = 2.0f / (aabb.max.y - aabb.min.y);
            var offsetX = -0.5f * (aabb.max.x + aabb.min.x) * scaleX;
            var offsetY = -0.5f * (aabb.max.y + aabb.min.y) * scaleY;
            var scaleZ = 1.0f / (aabb.max.z - aabb.min.z);
            var offsetZ = -aabb.min.z * scaleZ;

            return new float4x4(scaleX, 0, 0, offsetX,
                                0, scaleY, 0, offsetY,
                                0, 0, scaleZ, offsetZ,
                                0, 0, 0, 1);
        }
    }
}
