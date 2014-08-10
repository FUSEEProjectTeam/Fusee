#pragma warning disable 0649

using Fusee.Math;

namespace CrossSL.Meta
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable UnusedAutoPropertyAccessor.Local

    public abstract partial class xSLShader
    {
        #region VERTEX/FRAGMENT SHADER VARIABLES

        // vertex/fragment shader / attribute variables (RO)

        [VertexShader, FragmentShader]
        protected float4 xslColor { get; private set; }

        [VertexShader, FragmentShader]
        protected float4 xslSecondaryColor { get; private set; }

        // vertex/fragment shader / varying output/input (RW)

        [VertexShader, FragmentShader]
        protected float4[] TexCoord { get; set; }

        [VertexShader, FragmentShader]
        protected float xslFogFragCoord { get; set; }

        #endregion

        #region VERTEX SHADER VARIABLES

        // vertex shader / output variables (RW)

        [VertexShader, Mandatory]
        protected float4 xslPosition { get; set; }

        [VertexShader]
        protected float xslPointSize { get; set; }

        [VertexShader]
        protected float4 xslClipVertex { get; set; }

        // vertex shader / attribute variables (RO)

        [VertexShader]
        protected float4 xslVertex { get; private set; }

        [VertexShader]
        protected float3 xslNormal { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord0 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord1 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord2 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord3 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord4 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord5 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord6 { get; private set; }

        [VertexShader]
        protected float4 xslMultiTexCoord7 { get; private set; }

        [VertexShader]
        protected float xslFogCoord { get; private set; }

        // vertex shader / varying output (RW)

        [VertexShader]
        protected float4 xslFrontColor { get; set; }

        [VertexShader]
        protected float4 xslBackColor { get; set; }

        [VertexShader]
        protected float4 xslFrontSecondaryColor { get; set; }

        [VertexShader]
        protected float4 xslBackSecondaryColor { get; set; }

        #endregion

        #region FRAGMENT SHADER VARIABLES

        // fragment shader / output variables (RW)

        [FragmentShader, Mandatory]
        protected float4 xslFragColor { get; set; }

        [FragmentShader]
        protected float4[] xslFragData { get; set; }

        [FragmentShader]
        protected float xslFragDepth { get; set; }

        // fragment shader / input variables (RO)

        [FragmentShader]
        protected float4 xslFragCoord { get; private set; }

        [FragmentShader]
        protected bool xslFrontFacing { get; private set; }

        #endregion

        #region BUILT-IN CONSTANTS

        [VertexShader, FragmentShader]
        protected int xslMaxVertexUniformComponents { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxFragmentUniformComponents { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxVertexAttribs { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxVaryingFloats { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxDrawBuffers { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxTextureCoords { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxTextureUnits { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxTextureImageUnits { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxVertexTextureImageUnits { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxCombinedTextureImageUnits { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxLights { get; private set; }

        [VertexShader, FragmentShader]
        protected int xslMaxClipPlanes { get; private set; }

        #endregion

        #region BUILT-IN UNIFORMS

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewMatrix { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewProjectionMatrix { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslProjectionMatrix { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4[] xslTextureMatrix { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewMatrixInverse { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewProjectionMatrixInverse { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslProjectionMatrixInverse { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4[] xslTextureMatrixInverse { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewMatrixTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewProjectionMatrixTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslProjectionMatrixTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4[] xslTextureMatrixTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewMatrixInverseTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslModelViewProjectionMatrixInverseTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslProjectionMatrixInverseTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4[] xslTextureMatrixInverseTranspose { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslNormalMatrix { get; private set; }

        [VertexShader, FragmentShader]
        protected float4x4 xslNormalScale { get; private set; }

        protected struct xslDepthRangeParameters
        {
            internal float Near;
            internal float Far;
            internal float Diff;
        }

        [VertexShader, FragmentShader]
        protected xslDepthRangeParameters xslDepthRange { get; private set; }

        protected class xslFogParameters
        {
            internal float4 Color;
            internal float Density;
            internal float Start;
            internal float End;
            internal float Scale;
        }

        [VertexShader, FragmentShader]
        protected xslFogParameters xslFog { get; private set; }

        protected class xslLightSourceParameters
        {
            internal float4 Ambient;
            internal float4 Diffuse;
            internal float4 Specular;
            internal float4 Position;
            internal float4 HalfVector;
            internal float3 SpotDirection;
            internal float SpotExponent;
            internal float SpotCutoff;
            internal float SpotCosCutoff;
            internal float ConstantAttenuation;
            internal float LinearAttentuation;
            internal float QuadraticAttenuation;
        }

        [VertexShader, FragmentShader]
        protected xslLightSourceParameters[] xslLightSource { get; private set; }

        protected class xslLightModelParameters
        {
            internal float4 Ambient;
        }

        [VertexShader, FragmentShader]
        protected xslLightModelParameters xslLightModel { get; private set; }

        protected class xslLightModelProducts
        {
            internal float4 SceneColor;
        }

        [VertexShader, FragmentShader]
        protected xslLightModelProducts xslFrontLightModelProduct { get; private set; }

        [VertexShader, FragmentShader]
        protected xslLightModelProducts xslBackLightModelProduct { get; private set; }

        protected class xslLightProducts
        {
            internal float4 Ambient;
            internal float4 Diffuse;
            internal float4 Specular;
        }

        [VertexShader, FragmentShader]
        protected xslLightProducts[] xslFrontLightProduct { get; private set; }

        [VertexShader, FragmentShader]
        protected xslLightProducts[] xslBackLightProduct { get; private set; }

        protected class xslMaterialParameters
        {
            internal float4 Emission;
            internal float4 Ambient;
            internal float4 Diffuse;
            internal float4 Specular;
            internal float Shininess;
        }

        [VertexShader, FragmentShader]
        protected xslMaterialParameters xslFrontMaterial { get; private set; }

        [VertexShader, FragmentShader]
        protected xslMaterialParameters xslBackMaterial { get; private set; }

        protected struct xslPointParameters
        {
            internal float Size;
            internal float SizeMin;
            internal float SizeMax;
            internal float FadeThresholdSize;
            internal float DistanceConstantAttenuation;
            internal float DistanceLinearAttenuation;
            internal float DistanceQuadraticAttenuation;
        }

        [VertexShader, FragmentShader]
        protected xslPointParameters xslPoint { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslTextureEnvColor { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslClipPlane { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslEyePlaneS { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslEyePlaneT { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslEyePlaneR { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslEyePlaneQ { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslObjectPlaneS { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslObjectPlaneT { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslObjectPlaneR { get; private set; }

        [VertexShader, FragmentShader]
        protected float4[] xslObjectPlaneQ { get; private set; }

        #endregion
    }

    // ReSharper restore UnusedAutoPropertyAccessor.Local
    // ReSharper restore InconsistentNaming
}

#pragma warning restore 0649