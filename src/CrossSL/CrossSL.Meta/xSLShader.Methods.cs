using System;
using Fusee.Math;
using Fusee.Math.Core;

namespace CrossSL.Meta
{
    // ReSharper disable InconsistentNaming

    public abstract partial class xSLShader
    {
        // SHADER MAIN
        public abstract void VertexShader();
        public abstract void FragmentShader();

        // BUILT-IN FUNCTIONS

        #region TEXTURE LOOKUP FUNCTIONS

        [Mapping("texture1D")]
        protected float4 Texture1D(sampler1D sampler, float coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1D"), FragmentShader]
        protected float4 Texture1D(sampler1D sampler, float coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProj")]
        protected float4 Texture1DProj(sampler1D sampler, float2 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProj"), FragmentShader]
        protected float4 Texture1DProj(sampler1D sampler, float2 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProj")]
        protected float4 Texture1DProj(sampler1D sampler, float4 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProj"), FragmentShader]
        protected float4 Texture1DProj(sampler1D sampler, float4 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2D")]
        protected float4 Texture2D(sampler2D sampler, float2 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2D"), FragmentShader]
        protected float4 Texture2D(sampler2D sampler, float2 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProj")]
        protected float4 Texture2DProj(sampler2D sampler, float3 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProj"), FragmentShader]
        protected float4 Texture2DProj(sampler2D sampler, float3 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProj")]
        protected float4 Texture2DProj(sampler2D sampler, float4 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProj"), FragmentShader]
        protected float4 Texture2DProj(sampler2D sampler, float4 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture3D")]
        protected float4 Texture3D(sampler3D sampler, float3 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture3D"), FragmentShader]
        protected float4 Texture3D(sampler3D sampler, float3 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture3DProj")]
        protected float4 Texture3DProj(sampler3D sampler, float4 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture3DProj"), FragmentShader]
        protected float4 Texture3DProj(sampler3D sampler, float4 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("textureCube")]
        protected float4 TextureCube(samplerCube sampler, float3 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("textureCube"), FragmentShader]
        protected float4 TextureCube(samplerCube sampler, float3 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1D")]
        protected float4 Shadow1D(sampler1DShadow sampler, float3 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1D"), FragmentShader]
        protected float4 Shadow1D(sampler1DShadow sampler, float3 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2D")]
        protected float4 Shadow2D(sampler2DShadow sampler, float3 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2D"), FragmentShader]
        protected float4 Shadow2D(sampler2DShadow sampler, float3 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1DProj")]
        protected float4 Shadow1DProj(sampler1DShadow sampler, float4 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1DProj"), FragmentShader]
        protected float4 Shadow1DProj(sampler1DShadow sampler, float4 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2DProj")]
        protected float4 Shadow2DProj(sampler2DShadow sampler, float4 coord)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2DProj"), FragmentShader]
        protected float4 Shadow2DProj(sampler2DShadow sampler, float4 coord, float bias)
        {
            return new float4(1, 1, 1, 1);
        }

        #endregion

        #region TEXTURE LOOKUP FUNCTIONS WITH LOD

        [Mapping("texture1DLod"), VertexShader]
        protected float4 Texture1DLod(sampler1D sampler, float coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProjLod"), VertexShader]
        protected float4 Texture1DProjLod(sampler1D sampler, float2 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture1DProjLod"), VertexShader]
        protected float4 Texture1DProjLod(sampler1D sampler, float4 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DLod"), VertexShader]
        protected float4 Texture2DLod(sampler2D sampler, float2 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProjLod"), VertexShader]
        protected float4 Texture2DProjLod(sampler2D sampler, float3 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture2DProjLod"), VertexShader]
        protected float4 Texture2DProjLod(sampler2D sampler, float4 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("texture3DProjLod"), VertexShader]
        protected float4 Texture3DProjLod(sampler3D sampler, float4 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("textureCubeLod"), VertexShader]
        protected float4 TextureCubeLod(samplerCube sampler, float3 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1DLod"), VertexShader]
        protected float4 Shadow1DLod(sampler1DShadow sampler, float3 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2DLod"), VertexShader]
        protected float4 Shadow2DLod(sampler2DShadow sampler, float3 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow1DProjLod"), VertexShader]
        protected float4 Shadow1DProjLod(sampler1DShadow sampler, float4 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        [Mapping("shadow2DProjLod"), VertexShader]
        protected float4 Shadow2DProjLod(sampler2DShadow sampler, float4 coord, float lod)
        {
            return new float4(1, 1, 1, 1);
        }

        #endregion

        #region NOISE FUNCTIONS

        [Mapping("noise1")]
        protected float Noise1(float val)
        {
            return new Random().Next();
        }

        [Mapping("noise1")]
        protected float Noise1(float2 val)
        {
            return new Random().Next();
        }

        [Mapping("noise1")]
        protected float Noise1(float3 val)
        {
            return new Random().Next();
        }

        [Mapping("noise1")]
        protected float Noise1(float4 val)
        {
            return new Random().Next();
        }

        [Mapping("noise2")]
        protected float2 Noise2(float val)
        {
            var rng = new Random();
            return new float2(rng.Next(), rng.Next());
        }

        [Mapping("noise2")]
        protected float2 Noise2(float2 val)
        {
            var rng = new Random();
            return new float2(rng.Next(), rng.Next());
        }

        [Mapping("noise2")]
        protected float2 Noise2(float3 val)
        {
            var rng = new Random();
            return new float2(rng.Next(), rng.Next());
        }

        [Mapping("noise2")]
        protected float2 Noise2(float4 val)
        {
            var rng = new Random();
            return new float2(rng.Next(), rng.Next());
        }

        [Mapping("noise3")]
        protected float3 Noise3(float val)
        {
            var rng = new Random();
            return new float3(rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise3")]
        protected float3 Noise3(float2 val)
        {
            var rng = new Random();
            return new float3(rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise3")]
        protected float3 Noise3(float3 val)
        {
            var rng = new Random();
            return new float3(rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise3")]
        protected float3 Noise3(float4 val)
        {
            var rng = new Random();
            return new float3(rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise4")]
        protected float4 Noise4(float val)
        {
            var rng = new Random();
            return new float4(rng.Next(), rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise4")]
        protected float4 Noise4(float2 val)
        {
            var rng = new Random();
            return new float4(rng.Next(), rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise4")]
        protected float4 Noise4(float3 val)
        {
            var rng = new Random();
            return new float4(rng.Next(), rng.Next(), rng.Next(), rng.Next());
        }

        [Mapping("noise4")]
        protected float4 Noise4(float4 val)
        {
            var rng = new Random();
            return new float4(rng.Next(), rng.Next(), rng.Next(), rng.Next());
        }

        #endregion
    }

    // ReSharper restore InconsistentNaming
}