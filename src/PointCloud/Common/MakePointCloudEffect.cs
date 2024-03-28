using Fusee.Base.Core;
using Fusee.Engine.Core;
using Fusee.Engine.Core.Effects;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.PointCloud.Common
{
    /// <summary>
    /// Static class that contains methods that create <see cref="Effect"/>s for rendering point clouds. 
    /// </summary>
    public static class MakePointCloudEffect
    {
        /// <summary>
        /// <see cref="ShaderEffect"/> for rendering the depth-only pass. The result is used for EDL lighting. Only needed when using forward rendering.
        /// </summary>
        /// <param name="size">The point size.</param>
        /// <param name="pointSizeMode">The <see cref="PointSizeMode"/>.</param>
        /// <param name="shape">The <see cref="PointShape"/>.</param>
        /// <returns></returns>
        public static ShaderEffect ForDepthPass(float size, PointSizeMode pointSizeMode, PointShape shape)
        {
            return new ShaderEffect(

            new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float> {Name = UniformNameDeclarations.PointSize, Value = size},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointShape, Value = (int)shape},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSizeMode, Value = (int)pointSizeMode},
            },
            new RenderStateSet
            {
                AlphaBlendEnable = true,
                ZEnable = true,
            },
            AssetStorage.Get<string>("PointCloud.vert"),
            AssetStorage.Get<string>("PointDepth.frag"))
            {
                Active = false
            };
        }

        /// <summary>
        /// <see cref="ShaderEffect"/> for rendering the depth-only pass. The result is used for EDL lighting. Only needed when using forward rendering.
        /// </summary>
        /// <param name="size">The point size.</param>
        /// <param name="pointSizeMode">The <see cref="PointSizeMode"/>.</param>
        /// <param name="shape">The <see cref="PointShape"/>.</param>
        /// <returns></returns>
        public static ShaderEffect ForDepthPassInstanced(int size, PointSizeMode pointSizeMode, PointShape shape)
        {
            return new ShaderEffect(

            new List<IFxParamDeclaration>
            {
                new FxParamDeclaration<float> {Name = UniformNameDeclarations.PointSize, Value = size},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointShape, Value = (int)shape},
                new FxParamDeclaration<int> {Name = UniformNameDeclarations.PointSizeMode, Value = (int)pointSizeMode},
            },
            new RenderStateSet
            {
                AlphaBlendEnable = true,
                ZEnable = true,
            },
            AssetStorage.Get<string>("PointCloudInstanced.vert"),
            AssetStorage.Get<string>("PointDepthInstanced.frag"));
        }

        /// <summary>
        /// The <see cref="SurfaceEffectPointCloud"/> used for point cloud rendering.
        /// </summary>
        /// <param name="size">The point size.</param>
        /// <param name="pointSizeMode">The <see cref="PointSizeMode"/>.</param>
        /// <param name="shape">The <see cref="PointShape"/>.</param>
        /// <param name="colorMode">The <see cref="ColorMode"/>.</param>
        /// <param name="edlStrength">The strength of the EDL lighting.</param>
        /// <param name="edlNeigbourPx">Number of pixels, used in the EDL lighting calculation.</param>
        /// <returns></returns>
        public static SurfaceEffectPointCloud ForColorPass(float size, ColorMode colorMode, PointSizeMode pointSizeMode, PointShape shape, float edlStrength, int edlNeigbourPx)
        {
            var fx = new SurfaceEffectPointCloud
            {
                PointSize = size,
                ColorMode = (int)colorMode,
                PointShape = (int)shape,
                DepthTex = null,
                EDLStrength = edlStrength,
                EDLNeighbourPixels = edlNeigbourPx,
                PointSizeMode = (int)pointSizeMode
            };
            fx.SurfaceInput.Albedo = new float4(0.5f, 0.5f, 0.5f, 1.0f);
            return fx;
        }

        /// <summary>
        /// The <see cref="SurfaceEffectPointCloud"/> used for point cloud rendering.
        /// </summary>
        /// <param name="size">The point size.</param>
        /// <param name="pointSizeMode">The <see cref="PointSizeMode"/>.</param>
        /// <param name="shape">The <see cref="PointShape"/>.</param>
        /// <param name="colorMode">The <see cref="ColorMode"/>.</param>
        /// <param name="edlStrength">The strength of the EDL lighting.</param>
        /// <param name="edlNeigbourPx">Number of pixels, used in the EDL lighting calculation.</param>
        /// <returns></returns>
        public static SurfaceEffectPointCloud ForColorPassInstanced(float size, ColorMode colorMode, PointSizeMode pointSizeMode, PointShape shape, float edlStrength, int edlNeigbourPx)
        {
            var fx = new SurfaceEffectPointCloud(null, true)
            {
                PointSize = size,
                ColorMode = (int)colorMode,
                PointShape = (int)shape,
                DepthTex = null,
                EDLStrength = edlStrength,
                EDLNeighbourPixels = edlNeigbourPx,
                PointSizeMode = (int)pointSizeMode
            };
            fx.SurfaceInput.Albedo = new float4(0.5f, 0.5f, 0.5f, 1.0f);
            return fx;
        }
    }
}