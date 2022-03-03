using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop.Gizmos
{
    public enum MODE
    {
        LOCAL,
        WORLD
    };

    [Flags]
    public enum OPERATION : uint
    {
        TRANSLATE_X = (1u << 0),
        TRANSLATE_Y = (1u << 1),
        TRANSLATE_Z = (1u << 2),
        ROTATE_X = (1u << 3),
        ROTATE_Y = (1u << 4),
        ROTATE_Z = (1u << 5),
        ROTATE_SCREEN = (1u << 6),
        SCALE_X = (1u << 7),
        SCALE_Y = (1u << 8),
        SCALE_Z = (1u << 9),
        BOUNDS = (1u << 10),
        SCALE_XU = (1u << 11),
        SCALE_YU = (1u << 12),
        SCALE_ZU = (1u << 13),

        TRANSLATE = TRANSLATE_X | TRANSLATE_Y | TRANSLATE_Z,
        ROTATE = ROTATE_X | ROTATE_Y | ROTATE_Z | ROTATE_SCREEN,
        SCALE = SCALE_X | SCALE_Y | SCALE_Z,
        SCALEU = SCALE_XU | SCALE_YU | SCALE_ZU, // universal
        UNIVERSAL = TRANSLATE | ROTATE | SCALEU
    };

    public static class Manipulate
    {
        /// <summary>
        /// Allow axis to flip
        /// When true (default), the guizmo axis flip for better visibility
        /// When false, they always stay along the positive world/local axis
        /// </summary>
        public static bool AllowAxisFlip { get; set; }

        public static int SetID { get; set; }

        public static void DrawManipulate(
            float4x4 view,
            float4x4 projection,
            OPERATION operation, MODE mode,
            out float4x4 matrix,
            out float4x4 deltaMatrix,
            out float4x4 snap,
            out float4x4 localBounds,
            out float4x4 boundsSnap)
        {
            matrix = float4x4.Identity;
            deltaMatrix = float4x4.Identity;
            snap = float4x4.Identity;
            localBounds = float4x4.Identity;
            boundsSnap = float4x4.Identity;
        }

        /// <summary>
        /// Return true if cursor is over operation
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsOver(OPERATION op)
        {
            return false;
        }


        private static void SetGizmoSizeClipSpace(float value)
        {

        }
    }
}
