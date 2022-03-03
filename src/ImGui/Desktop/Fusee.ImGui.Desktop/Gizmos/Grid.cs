using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop.Gizmos
{
    public static class Grid
    {
        public static void DrawGrid(float4x4 view, float4x4 projection, float4x4 model, float gridSize, Vector2 size, Vector2 pos)
        {
            var VP = projection * view;
            var frustum = new FrustumF();
            frustum.CalculateFrustumPlanes(VP);

            var allPlanes = new PlaneF[] {
               frustum.Near.Normalize(),
               frustum.Far.Normalize(),
               frustum.Left.Normalize(),
               frustum.Right.Normalize(),
               frustum.Top.Normalize(),
               frustum.Bottom.Normalize()
               };

            var res = VP * model;

            for (var f = -gridSize; f <= gridSize; f += 1f)
            {
                for (int dir = 0; dir < 2; dir++)
                {
                    var ptA = new float3(dir != 0 ? -gridSize : f, 0f, dir != 0 ? f : -gridSize);
                    var ptB = new float3(dir != 0 ? gridSize : f, 0f, dir != 0 ? f : gridSize);
                    bool visible = true;
                    for (int i = 0; i < allPlanes.Length; i++)
                    {
                        var dA = allPlanes[i].SignedDistanceFromPoint(ptA);
                        var dB = allPlanes[i].SignedDistanceFromPoint(ptB);
                        if (dA > 0f && dB > 0f)
                        {
                            visible = false;
                            break;
                        }
                        if (dA < 0f && dB < 0f)
                        {
                            continue;
                        }
                        if (dA > 0f)
                        {
                            float len = MathF.Abs(dA - dB);
                            float t = MathF.Abs(dA) / len;
                            ptA = float3.Lerp(ptA, ptB, t);
                        }
                        if (dB > 0f)
                        {
                            float len = MathF.Abs(dB - dA);
                            float t = MathF.Abs(dB) / len;
                            ptB = float3.Lerp(ptB, ptA, t);
                        }
                    }

                    if (visible)
                    {
                        var col = new Vector4(0x80, 0x80, 0x80, 0xFF);
                        col = MathF.Abs(f) % 10f < float.Epsilon ? new Vector4(0x90, 0x90, 0x90, 0xFF) : col;
                        col = MathF.Abs(f) < float.Epsilon ? new Vector4(0x40, 0x40, 0x40, 0xFF) : col;

                        float thickness = 1f;
                        thickness = MathF.Abs(f) % 10f < float.Epsilon ? 1.5f : thickness;
                        thickness = MathF.Abs(f) < float.Epsilon ? 2.3f : thickness;

                        var lineStart = Gizmos.WorldToPos(ptA, res, pos, size);
                        var lineEnd = Gizmos.WorldToPos(ptB, res, pos, size);

                        ImGui.GetWindowDrawList().AddLine(lineStart, lineEnd, Gizmos.Vector2Col(col), thickness);
                    }

                }

            }
        }
    }
}
