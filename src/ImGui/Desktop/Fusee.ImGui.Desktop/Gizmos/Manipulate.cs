using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Numerics;

namespace Fusee.DImGui.Desktop.Gizmos
{
    public static class Manipulate
    {

        public static bool DrawManipulate(
            float4x4 view,
            float4x4 projection,
            OPERATION operation, MODE mode,
            ref float4x4 matrix,
            ref float4x4 deltaMatrix,
            ref float[] snap,
            ref float[] localBounds,
            ref float[] boundsSnap)
        {
            matrix = float4x4.Identity;
            deltaMatrix = float4x4.Identity;

            // Scale is always local or matrix will be skewed when applying world scale or oriented matrix
            Gizmos.ComputeContext(view, projection, matrix, ((uint)(operation & OPERATION.SCALE)) == 1U ? MODE.LOCAL : mode);

            // set delta to identity
            if (deltaMatrix != null)
            {
                deltaMatrix = float4x4.Identity;
            }

            // behind camera

            var camSpacePosition = Gizmos.gContext.mMVP * float3.Zero;
            if (!Gizmos.gContext.mIsOrthographic && camSpacePosition.z < 0.001f)
            {
                return false;
            }

            // --
            var type = MOVETYPE.NONE;
            bool manipulated = false;
            if (Gizmos.gContext.mbEnable)
            {
                if (!Gizmos.gContext.mbUsingBounds)
                {
                    manipulated = Gizmos.HandleTranslation(ref matrix, ref deltaMatrix, operation, ref type, ref snap) ||
                                  Gizmos.HandleScale(ref matrix, ref deltaMatrix, operation, ref type, ref snap) ||
                                  Gizmos.HandleRotation(ref matrix, ref deltaMatrix, operation, ref type, ref snap);
                }
            }

            if (localBounds != null && !Gizmos.gContext.mbUsing)
            {
                Gizmos.HandleAndDrawLocalBounds(ref localBounds, matrix, ref boundsSnap, operation);
            }

            Gizmos.gContext.mOperation = operation;
            if (!Gizmos.gContext.mbUsingBounds)
            {
                Gizmos.DrawRotationGizmo(operation, type);
                Gizmos.DrawTranslationGizmo(operation, type);
                Gizmos.DrawScaleGizmo(operation, type);
                Gizmos.DrawScaleUniveralGizmo(operation, type);
            }
            return manipulated;
        }
    }

}
