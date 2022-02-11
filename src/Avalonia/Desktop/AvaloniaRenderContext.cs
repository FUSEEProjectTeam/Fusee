using Fusee.Engine.Common;
using Fusee.Engine.Core;
using Fusee.Engine.Core.ShaderShards;
using Fusee.Engine.Imp.Graphics.Desktop;
using Fusee.Math.Core;

namespace Fusee.Avalonia.Desktop
{
    /// <summary>
    /// Custom RenderContextImp, which derives from <see cref="Fusee.Engine.Imp.Graphics.Desktop.RenderContextImp"/>
    /// Just for naming purposes and possible future overrides / functionality
    /// </summary>
    public class AvaloniaRenderContextImp : RenderContextImp
    {
        public AvaloniaRenderContextImp(IRenderCanvasImp renderCanvas) : base(renderCanvas)
        {
        }
    }

    /// <summary>
    /// As Avalonia uses a right handed coordinate system for all OpenGL calls and Fusee uses a left handed system, everything is drawn the
    /// wrong way-mirrored around the x-axis. We fix this by multiplying the Fusee render result after MVP-projection with a scale of (1, -1, 1)
    /// and thereby flipping the image in manner that fits to Avalonia's assumption of a right handed system.
    /// Attention: One must therefor set the <see cref="GL.CullFace"/> to "Front" which is done inside <see cref="AvaloniaRenderCanvasImp"/>
    /// </summary>
    public class AvaloniaRenderContext : RenderContext
    {
        public AvaloniaRenderContext(IRenderContextImp rci) : base(rci)
        {
        }

        /// <summary>
        /// The projection matrix used by the rendering pipeline.
        /// !!Attention: This methods contains the scale factor (1, -1, 1) for Avalonia!!
        /// </summary>
        /// <value>
        /// The 4x4 projection matrix applied to view coordinates yielding clip space coordinates.
        /// </value>
        /// <remarks>
        /// View coordinates are the result of the ModelView matrix multiplied to the geometry (<see cref="RenderContext.ModelView"/>).
        /// The coordinate system of the view space has its origin in the camera center with the z axis aligned to the viewing direction, and the x- and
        /// y axes aligned to the viewing plane. Still, no projection from 3d space to the viewing plane has been performed. This is done by multiplying
        /// view coordinate geometry with the projection matrix. Typically, the projection matrix either performs a parallel projection or a perspective
        /// projection.
        /// </remarks>
        public override float4x4 Projection
        {
            get => _projection;
            set
            {
                if (_projection == value) return;

                // Update matrix
                _projection = value;

                // Calculate frustum planes without Avalonia flip!
                RenderFrustum.CalculateFrustumPlanes(_projection * View);

                // apply the flip
                _projection = float4x4.Scale(1, -1, 1) * _projection;

                // Invalidate derived matrices
                _modelViewProjectionOk = false;
                _invProjectionOk = false;
                _invTransProjectionOk = false;
                _transProjectionOk = false;

                SetGlobalEffectParam(UniformNameDeclarations.ProjectionHash, _projection);
                SetGlobalEffectParam(UniformNameDeclarations.ModelViewProjectionHash, ModelViewProjection);
                SetGlobalEffectParam(UniformNameDeclarations.IProjectionHash, InvProjection);
                SetGlobalEffectParam(UniformNameDeclarations.ITProjectionHash, InvTransProjection);
                SetGlobalEffectParam(UniformNameDeclarations.TProjectionHash, TransProjection);


                SetGlobalEffectParam(UniformNameDeclarations.ClippingPlanesHash, CalculateClippingPlanesFromProjection());
            }
        }
    }
}
