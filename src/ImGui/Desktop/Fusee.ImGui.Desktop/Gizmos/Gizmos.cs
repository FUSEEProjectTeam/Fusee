using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop.Gizmos
{
    public static class Gizmos
    {
        /// <summary>
        /// Return true if mouse cursor is over any gizmo control (axis, plan or screen component)
        /// </summary>
        public static bool IsOver { get; internal set; }

        /// <summary>
        /// return true if mouse IsOver or if the gizmo is in moving state
        /// </summary>
        public static bool IsUsing { get; internal set; }

        public static ImGuiViewportPtr Viewport = ImGui.GetMainViewport();

        public static void SetRect(float x, float y, float width, float height)
        {

        }

        public static bool IsOrthographic { get; internal set; }


        internal static uint Vector2Col(Vector4 color)
        {
            uint col = 0;

            col += (uint)(color.X)       | byte.MinValue;
            col += (uint)(color.Y) << 8  | byte.MinValue;
            col += (uint)(color.Z) << 16 | byte.MinValue;
            col += (uint)(color.W) << 24 | byte.MinValue;

            return col;
        }

        internal static Vector2 WorldToPos(float3 worldPos, float4x4 mat)
        {
            return WorldToPos(worldPos, mat, Vector2.Zero, Vector2.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="mat"></param>
        /// <param name="position">float2.Zero for default value calculation</param>
        /// <param name="size">float2.Zero for default value calculation</param>
        /// <returns></returns>
        internal static Vector2 WorldToPos(float3 worldPos, float4x4 mat, Vector2 position, Vector2 size)
        {
            if (position == Vector2.Zero)
                position = new Vector2(Viewport.Pos.X, Viewport.Pos.Y);

            if (size == Vector2.Zero)
                size = new Vector2(Viewport.Size.X, Viewport.Size.Y);

            var trans = mat * new float4(worldPos.xyz, 1.0f); // TransformPt(worldPos.x, worldPos.y, worldPos.z, mat.ToArray());

            trans *= 0.5f / trans.w;
            trans += new float4(0.5f, 0.5f, 0, 0);
            trans.y = 1f - trans.y;
            trans.x *= size.X;
            trans.y *=  size.Y;
            trans.x += position.X;
            trans.y +=  position.Y;
            return new Vector2(trans.x, trans.y);
        }
    }
}
