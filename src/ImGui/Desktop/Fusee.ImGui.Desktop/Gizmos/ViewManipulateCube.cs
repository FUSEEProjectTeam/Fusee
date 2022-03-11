using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop.Gizmos
{

    //
    // Please note that this cubeview is patented by Autodesk : https://patents.google.com/patent/US7782319B2/en
    // It seems to be a defensive patent in the US. I don't think it will bring troubles using it as
    // other software are using the same mechanics. But just in case, you are now warned!
    //
    public static class ViewManipulateCube
    {
        private static float4 BuildPlan(float3 p_point1, float3 p_normal)
        {
            float4 res;
            var normal = p_normal.Normalize();
            res.w = float3.Dot(normal, p_point1);
            res.x = normal.x;
            res.y = normal.y;
            res.z = normal.z;
            return res;
        }

        private static void ComputeCameraRay(ref float3 rayOrigin, ref float3 rayDir, float2 position, float2 size)
        {
            // var io = ImGui.GetIO();
            //
            // float4x4 mViewProjInverse;
            // mViewProjInverse.Inverse(gContext.mViewMat * gContext.mProjectionMat);
            //
            // float mox = ((io.MousePos.X - position.x) / size.x) * 2f - 1f;
            // float moy = (1f - ((io.MousePos.Y - position.y) / size.y)) * 2f - 1f;
            //
            // float zNear = gContext.mReversed ? (1f - float.Epsilon) : 0f;
            // float zFar = gContext.mReversed ? 0f : (1f - float.Epsilon);
            //
            // rayOrigin.Transform(makeVect(mox, moy, zNear, 1f), mViewProjInverse);
            // rayOrigin *= 1.f / rayOrigin.w;
            // vec_t rayEnd;
            // rayEnd = new float4(mox, moy, zFar, 1f) * mViewProjInverse;
            // rayEnd *= 1f / rayEnd.w;
            // rayDir = Normalized(rayEnd - rayOrigin);
        }


        public static void DrawManipulateCube(float4x4 view, float length, Vector2 position, Vector2 size, uint backgroundColor)
        {
        //    bool isDraging = false;
        //    bool isClicking = false;
        //    bool isInside = false;
        //    var interpolationUp = float3.Zero;
        //    var interpolationDir = float3.Zero;
        //    int interpolationFrames = 0;
        //    float3 referenceUp = new float3(0f, 1f, 0f);

        //    var gContext = new Context();

        //    float4x4 svgView;
        //    float4x4 svgProjection;
        //    svgView = gContext.mViewMat;
        //    svgProjection = gContext.mProjectionMat;

        //    var io = ImGui.GetIO();
        //    gContext.mDrawList.AddRectFilled(position, position + size, backgroundColor);
        //    float4x4 viewInverse = view.Invert();

        //    float3 camTarget = viewInverse.v.position - viewInverse.v.dir * length;

        //    // view/projection matrices
        //    float distance = 3f;
        //    float4x4 cubeProjection;
        //    float4x4 cubeView;
        //    float fov = M.RadiansToDegrees(MathF.Acos(distance / (MathF.Sqrt(distance * distance + 3f)))) / MathF.Sqrt(2f);
        //    cubeProjection = float4x4.CreatePerspectiveFieldOfView(M.DegreesToRadians(fov), size.X / size.Y, 0.01f, 1000f);

        //    var dir = new float3(viewInverse[2, 0], viewInverse[2, 1], viewInverse[2, 2]);
        //    var up = new float3(viewInverse[1, 0], viewInverse[1, 1], viewInverse[1, 2]);
        //    var eye = dir * distance;
        //    var zero = new float3(0f, 0f, 0f);
        //    cubeView = float4x4.LookAt(eye, zero, up);


        //    // set context
        //    gContext.mViewMat = cubeView;
        //    gContext.mProjectionMat = cubeProjection;
        //    ComputeCameraRay(gContext.mRayOrigin, gContext.mRayVector, position, size);

        //    float4x4 res = cubeProjection * cubeView;

        //    // panels
        //    var panelPosition = new Vector2[9]
        //    {
        //        new Vector2(0.75f,0.75f),
        //        new Vector2(0.25f, 0.75f),
        //        new Vector2(0f, 0.75f),
        //        new Vector2(0.75f, 0.25f),
        //        new Vector2(0.25f, 0.25f),
        //        new Vector2(0f, 0.25f),
        //        new Vector2(0.75f, 0f),
        //        new Vector2(0.25f, 0f),
        //        new Vector2(0f, 0f)
        //    };

        //    var panelSize = new Vector2[9] {
        //        new Vector2(0.25f,0.25f),
        //        new Vector2(0.5f, 0.25f),
        //        new Vector2(0.25f, 0.25f),
        //        new Vector2(0.25f, 0.5f),
        //        new Vector2(0.5f, 0.5f),
        //        new Vector2(0.25f, 0.5f),
        //        new Vector2(0.25f, 0.25f),
        //        new Vector2(0.5f, 0.25f),
        //        new Vector2(0.25f, 0.25f)
        //    };

        //    // tag faces
        //    var boxes = new bool[27];
        //    var directionUnary = new float3[] { float3.UnitX, float3.UnitY, float3.UnitZ };
        //    var directionColor = new Vector4[3] { new Vector4(0xAA, 0, 0, 0xFF), new Vector4(0, 0xAA, 0, 0xFF), new Vector4(0, 0, 0xAA, 0XFF) };

        //    for (int iPass = 0; iPass < 2; iPass++)
        //    {
        //        for (int iFace = 0; iFace < 6; iFace++)
        //        {
        //            int normalIndex = (iFace % 3);
        //            int perpXIndex = (normalIndex + 1) % 3;
        //            int perpYIndex = (normalIndex + 2) % 3;
        //            float invert = (iFace > 2) ? -1f : 1f;
        //            float3 indexVectorX = directionUnary[perpXIndex] * invert;
        //            float3 indexVectorY = directionUnary[perpYIndex] * invert;
        //            float3 boxOrigin = directionUnary[normalIndex] * -invert - indexVectorX - indexVectorY;

        //            // plan local space
        //            float3 n = directionUnary[normalIndex] * invert;
        //            var viewSpaceNormal = n;
        //            var viewSpacePoint = n * 0.5f;
        //            viewSpaceNormal = viewSpaceNormal * cubeView;
        //            viewSpaceNormal.Normalize();
        //            viewSpacePoint = viewSpacePoint * cubeView;
        //            var viewSpaceFacePlan = BuildPlan(viewSpacePoint, viewSpaceNormal);

        //            // back face culling
        //            if (viewSpaceFacePlan.w > 0f)
        //            {
        //                continue;
        //            }

        //            var facePlan = BuildPlan(n * 0.5f, n);

        //            var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, facePlan);
        //            float3 posOnPlan = gContext.mRayOrigin + gContext.mRayVector * len - (n * 0.5f);

        //            var localx = float3.Dot(directionUnary[perpXIndex], posOnPlan) * invert + 0.5f;
        //            var localy = float3.Dot(directionUnary[perpYIndex], posOnPlan) * invert + 0.5f;

        //            // panels
        //            var dx = directionUnary[perpXIndex];
        //            var dy = directionUnary[perpYIndex];
        //            var origin = directionUnary[normalIndex] - dx - dy;
        //            for (int iPanel = 0; iPanel < 9; iPanel++)
        //            {
        //                var boxCoord = boxOrigin + indexVectorX * (float)(iPanel % 3) + indexVectorY * (float)(iPanel / 3) + new float3(1f, 1f, 1f);
        //                var p = panelPosition[iPanel] * 2f;
        //                var s = panelSize[iPanel] * 2f;
        //                var faceCoordsScreen = new Vector2[4];
        //                var panelPos = new float3[4]
        //                {
        //                    dx * p.X + dy * p.Y,
        //                    dx * p.X + dy * (p.Y + s.Y),
        //                    dx * (p.X + s.X) + dy * (p.Y + s.Y),
        //                    dx * (p.X + s.X) + dy * p.Y
        //                };

        //                for (var iCoord = 0; iCoord < 4; iCoord++)
        //                {
        //                    faceCoordsScreen[iCoord] = Gizmos.WorldToPos((panelPos[iCoord] + origin) * 0.5f * invert, res, position, size);
        //                }

        //                var panelCorners = new Vector2[2] { panelPosition[iPanel], panelPosition[iPanel] + panelSize[iPanel] };
        //                bool insidePanel = localx > panelCorners[0].X && localx < panelCorners[1].X && localy > panelCorners[0].Y && localy < panelCorners[1].Y;
        //                int boxCoordInt = (int)(boxCoord.x * 9f + boxCoord.y * 3f + boxCoord.z);
        //                Debug.Assert(boxCoordInt < 27);
        //                boxes[boxCoordInt] |= insidePanel && (!isDraging);

        //                // draw face with lighter color
        //                if (iPass == 1)
        //                {
        //                    gContext.mDrawList.AddConvexPolyFilled(ref faceCoordsScreen[0], 4, (Gizmos.Vector2Col(directionColor[normalIndex]) | Gizmos.Vector2Col(new Vector4(0x80, 0x80, 0x80, 0x80)) | (isInside ? Gizmos.Vector2Col(new Vector4(0x08, 0x08, 0x08, 0)) : 0)));
        //                    if (boxes[boxCoordInt])
        //                    {
        //                        gContext.mDrawList.AddConvexPolyFilled(ref faceCoordsScreen[0], 4, Gizmos.Vector2Col(new Vector4(0xF0, 0xA0, 0x60, 0x80)));

        //                        if (!io.MouseDown[0] && !isDraging && isClicking)
        //                        {
        //                            // apply new view direction
        //                            int cx = boxCoordInt / 9;
        //                            int cy = (boxCoordInt - cx * 9) / 3;
        //                            int cz = boxCoordInt % 3;
        //                            interpolationDir = new float3(1f - cx, 1f - cy, 1f - cz);
        //                            interpolationDir.Normalize();

        //                            if (MathF.Abs(float3.Dot(interpolationDir, referenceUp)) > 1.0f - 0.01f)
        //                            {
        //                                float3 right = viewInverse.v.right;
        //                                if (MathF.Abs(right.x) > MathF.Abs(right.z))
        //                                {
        //                                    right.z = 0f;
        //                                }
        //                                else
        //                                {
        //                                    right.x = 0f;
        //                                }
        //                                right.Normalize();
        //                                interpolationUp = float3.Cross(interpolationDir, right);
        //                                interpolationUp.Normalize();
        //                            }
        //                            else
        //                            {
        //                                interpolationUp = referenceUp;
        //                            }
        //                            interpolationFrames = 40;
        //                            isClicking = false;
        //                        }
        //                        if (io.MouseDown[0] && !isDraging)
        //                        {
        //                            isClicking = true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (interpolationFrames == 1)
        //    {
        //        interpolationFrames--;
        //        float3 newDir = viewInverse.v.dir;
        //        newDir = float3.Lerp(newDir, interpolationDir, 0.2f);
        //        newDir = newDir.Normalize();

        //        float3 newUp = viewInverse.v.up;
        //        newUp = float3.Lerp(newDir, interpolationUp, 0.3f);
        //        newUp.Normalize();
        //        newUp = interpolationUp;
        //        float3 newEye = camTarget + newDir * length;
        //        view = float4x4.LookAt(newEye, camTarget, newUp);
        //    }
        //    isInside = ImRect(position, position + size).Contains(io.MousePos);

        //    // drag view
        //    if (!isDraging && io.MouseDown[0] && isInside && (MathF.Abs(io.MouseDelta.X) > 0f || MathF.Abs(io.MouseDelta.Y) > 0f))
        //    {
        //        isDraging = true;
        //        isClicking = false;
        //    }
        //    else if (isDraging && !io.MouseDown[0])
        //    {
        //        isDraging = false;
        //    }

        //    if (isDraging)
        //    {
        //        float4x4 rx, ry, roll;

        //        rx.RotationAxis(referenceUp, -io.MouseDelta.X * 0.01f);
        //        ry.RotationAxis(viewInverse.v.right, -io.MouseDelta.Y * 0.01f);

        //        roll = rx * ry;

        //        float3 newDir = viewInverse.v.dir;
        //        newDir = newDir * roll;
        //        newDir.Normalize();

        //        // clamp
        //        float3 planDir = float3.Cross(viewInverse.v.right, referenceUp);
        //        planDir.y = 0f;
        //        planDir.Normalize();
        //        float dt = float3.Dot(planDir, newDir);
        //        if (dt < 0.0f)
        //        {
        //            newDir += planDir * dt;
        //            newDir.Normalize();
        //        }

        //        float3 newEye = camTarget + newDir * length;
        //        view = float4x4.LookAt(newEye, camTarget, referenceUp);
        //    }

        //    // restore view/projection because it was used to compute ray
        //    ComputeContext(svgView.m16, svgProjection.m16, gContext.mModelSource.m16, gContext.mMode);
        }
    }
}

