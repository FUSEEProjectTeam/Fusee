using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fusee.DImGui.Desktop.Gizmos
{

    [Flags]
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

    [Flags]
    public enum MOVETYPE : int
    {
        NONE = 0,
        MOVE_X,
        MOVE_Y,
        MOVE_Z,
        MOVE_YZ,
        MOVE_ZX,
        MOVE_XY,
        MOVE_SCREEN,
        ROTATE_X,
        ROTATE_Y,
        ROTATE_Z,
        ROTATE_SCREEN,
        SCALE_X,
        SCALE_Y,
        SCALE_Z,
        SCALE_XYZ
    }

    public class Context
    {
        public Context() => (mbUsing, mbEnable, mbUsingBounds) = (false, true, false);

        public ImDrawListPtr mDrawList => ImGui.GetWindowDrawList();

        public MODE mMode;
        public float4x4 mViewMat;
        public float4x4 mProjectionMat;
        public float4x4 mModel;
        public float4x4 mModelLocal; // orthonormalized model
        public float4x4 mModelInverse;
        public float4x4 mModelSource;
        public float4x4 mModelSourceInverse;
        public float4x4 mMVP;
        public float4x4 mMVPLocal; // MVP with full model matrix whereas mMVP's model matrix might only be translation in case of World space edition
        public float4x4 mViewProjection;
        public float3 mModelScaleOrigin;
        public float4 mCameraEye;
        public float4 mCameraRight;
        public float4 mCameraDir;
        public float4 mCameraUp;
        public float4 mRayOrigin;
        public float4 mRayVector;
        public float mRadiusSquareCenter;
        public Vector2 mScreenSquareCenter;
        public Vector2 mScreenSquareMin;
        public Vector2 mScreenSquareMax;
        public float mScreenFactor;
        public float4 mRelativeOrigin;
        public bool mbUsing;
        public bool mbEnable;
        public bool mbMouseOver;
        public bool mReversed; // reversed projection matrix

        // translation
        public float4 mTranslationPlan;
        public float4 mTranslationPlanOrigin;
        public float4 mMatrixOrigin;
        public float4 mTranslationLastDelta;

        // rotation
        public float4 mRotationVectorSource;
        public float mRotationAngle;
        public float mRotationAngleOrigin;

        //float4 mWorldToLocalAxis;

        // scale
        public float4 mScale;
        public float4 mScaleValueOrigin;
        public float4 mScaleLast;
        public float mSaveMousePosx;

        // save axis factor when using gizmo
        public bool[] mBelowAxisLimit = new bool[3];
        public bool[] mBelowPlaneLimit = new bool[3];
        public float[] mAxisFactor = new float[3];

        // bounds stretching
        public float4 mBoundsPivot;
        public float4 mBoundsAnchor;
        public float4 mBoundsPlan;
        public float4 mBoundsLocalPivot;
        public int mBoundsBestAxis;
        public int[] mBoundsAxis = new int[2];
        public bool mbUsingBounds;
        public float4x4 mBoundsMatrix;

        //
        public int mCurrentOperation;
        public float mX = 0f;
        public float mY = 0f;
        public float mWidth = 0f;
        public float mHeight = 0f;
        public float mXMax = 0f;
        public float mYMax = 0f;
        public float mDisplayRatio = 1f;
        public bool mIsOrthographic = false;
        public int mActualID = -1;
        public int mEditingID = -1;
        public OPERATION mOperation = 0;
        public bool mAllowAxisFlip = true;
        public float mGizmoSizeClipSpace = 0.1f;
    }

    public static class Gizmos
    {
        public static ImGuiViewportPtr Viewport = ImGui.GetMainViewport();

        internal static uint Vector2Col(Vector4 color)
        {
            uint col = 0;

            col += (uint)(color.X) | byte.MinValue;
            col += (uint)(color.Y) << 8 | byte.MinValue;
            col += (uint)(color.Z) << 16 | byte.MinValue;
            col += (uint)(color.W) << 24 | byte.MinValue;

            return col;
        }

        internal static Vector2 WorldToPos(float3 worldPos, float4x4 mat)
        {
            return WorldToPos(worldPos, mat, Vector2.Zero, Vector2.Zero);
        }

        //internal static Vector2 WorldToPos(float4 worldPos, float4x4 mat)
        //{
        //    return WorldToPos(worldPos, mat, Vector2.Zero, Vector2.Zero);
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="mat"></param>
        /// <param name="position">float2.Zero for default value calculation</param>
        /// <param name="size">float2.Zero for default value calculation</param>
        /// <returns></returns>
        //internal static Vector2 WorldToPos(float4 worldPos, float4x4 mat, Vector2 position, Vector2 size)
        //{
        //    if (position == Vector2.Zero)
        //        position = new Vector2(Viewport.Pos.X, Viewport.Pos.Y);
        //
        //    if (size == Vector2.Zero)
        //        size = new Vector2(Viewport.Size.X, Viewport.Size.Y);
        //
        //    var trans = mat * worldPos;
        //
        //    trans *= 0.5f / trans.w;
        //    trans += new float4(0.5f, 0.5f, 0, 0);
        //    trans.y = 1f - trans.y;
        //    trans.x *= size.X;
        //    trans.y *= size.Y;
        //    trans.x += position.X;
        //    trans.y += position.Y;
        //    return new Vector2(trans.x, trans.y);
        //}

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
            trans.y *= size.Y;
            trans.x += position.X;
            trans.y += position.Y;
            return new Vector2(trans.x, trans.y);
        }



        public static bool IsTranslateType(MOVETYPE type)
        {
            return type >= MOVETYPE.MOVE_X && type <= MOVETYPE.MOVE_SCREEN;
        }

        public static bool IsTranslateType(int type)
        {
            return type >= (int)MOVETYPE.MOVE_X && type <= (int)MOVETYPE.MOVE_SCREEN;
        }

        public static bool IsRotateType(MOVETYPE type)
        {
            return type >= MOVETYPE.ROTATE_X && type <= MOVETYPE.ROTATE_SCREEN;
        }

        public static bool IsScaleType(MOVETYPE type)
        {
            return type >= MOVETYPE.SCALE_X && type <= MOVETYPE.SCALE_XYZ;
        }

        // Matches MOVETYPE.MOVE_AB order
        public static readonly OPERATION[] TRANSLATE_PLANS = new OPERATION[3] { OPERATION.TRANSLATE_Y | OPERATION.TRANSLATE_Z, OPERATION.TRANSLATE_X | OPERATION.TRANSLATE_Z, OPERATION.TRANSLATE_X | OPERATION.TRANSLATE_Y };
        public static Context gContext = null;
        public static readonly float4[] directionUnary = new float4[3] { new float4(1f, 0f, 0f, 1.0f), new float4(0f, 1f, 0f, 1.0f), new float4(0f, 0f, 1f, 1.0f) };
        public static readonly Vector4[] directionColor = new Vector4[3] { new Vector4(0xAA, 0, 0, 0xFF), new Vector4(0, 0xAA, 0, 0xFF), new Vector4(0, 0, 0xAA, 0XFF) };

        // Alpha: 100%: FF, 87%: DE, 70%: B3, 54%: 8A, 50%: 80, 38%: 61, 12%: 1F
        public static readonly Vector4[] planeColor = new Vector4[3] { new Vector4(0xAA, 0, 0, 0x61), new Vector4(0, 0xAA, 0, 0x61), new Vector4(0, 0, 0xAA, 0x61) };
        public static Vector4 selectionColor = new Vector4(0xFF, 0x80, 0x10, 0x8A);
        public static Vector4 inactiveColor = new Vector4(0x99, 0x99, 0x99, 0x99);
        public static Vector4 translationLineColor = new Vector4(0xAA, 0xAA, 0xAA, 0xAA);
        public static readonly string[] translationInfoMask = new string[] { "X : {0:N3}", "Y : {0:N3}", "Z : {0:N3}", "Y : {0:N3} Z : {1:000}", "X : {0:N3} Z : {1:000}", "X : {0:N3} Y : {1:000}", "X : {0:N3} Y : {1:000} Z : {2:000}" };
        public static readonly string[] scaleInfoMask = new string[] { "X : {0:N2}", "Y : {0:N2}", "Z : {0:N2}", "XYZ : {0:N2}" };
        public static readonly string[] rotationInfoMask = new string[] { "X : {0:N2} deg {1:00} rad", "Y :{0:N2} deg {1:00} rad", "Z : {0:N2} deg {1:00} rad", "Screen : {0:N2} deg {1:00} rad" };
        public static readonly int[] translationInfoIndex = new int[] { 0, 0, 0, 1, 0, 0, 2, 0, 0, 1, 2, 0, 0, 2, 0, 0, 1, 0, 0, 1, 2 };
        public static readonly float quadMin = 0.5f;
        public static readonly float quadMax = 0.8f;
        public static readonly float[] quadUV = new float[8] { quadMin, quadMin, quadMin, quadMax, quadMax, quadMax, quadMax, quadMin };
        public static readonly int halfCircleSegmentCount = 64;
        public static readonly float snapTension = 0.5f;

        public static readonly float rotationDisplayFactor = 1.2f;
        public static readonly float screenRotateSize = 0.06f;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        //static int GetMoveType(OPERATION op, float4* gizmoHitProportion);
        //static int GetRotateType(OPERATION op);
        //static int GetScaleType(OPERATION op);

        public static void ComputeCameraRay(ref float4 rayOrigin, ref float4 rayDir)
        {
            ComputeCameraRay(ref rayOrigin, ref rayDir, Vector2.Zero, Vector2.Zero);
        }

        public static void ComputeCameraRay(ref float4 rayOrigin, ref float4 rayDir, Vector2 position, Vector2 size)
        {
            if (position.X == 0 && position.Y == 0)
                position = new Vector2(gContext.mX, gContext.mY);

            if (size.X == 0 && size.Y == 0)
                size = new Vector2(gContext.mWidth, gContext.mHeight);

            var io = ImGui.GetIO();

            var mViewProjInverse = (gContext.mProjectionMat * gContext.mViewMat).Invert();

            float mox = ((io.MousePos.X - position.X) / size.X) * 2f - 1f;
            float moy = (1f - ((io.MousePos.Y - position.Y) / size.Y)) * 2f - 1f;

            float zNear = gContext.mReversed ? (1f - float.Epsilon) : 0f;
            float zFar = gContext.mReversed ? 0f : (1f - float.Epsilon);

            rayOrigin = mViewProjInverse * new float4(mox, moy, zNear, 1f);
            rayOrigin *= 1f / rayOrigin.w;

            var rayEnd = mViewProjInverse * new float4(mox, moy, zFar, 1f);
            rayEnd *= 1f / rayEnd.w;
            rayDir = (rayEnd - rayOrigin).Normalize();
        }

        public static float GetSegmentLengthClipSpace(float4 start, float4 end, bool localCoordinates = false)
        {
            float4 startOfSegment = start;
            float4x4 mvp = localCoordinates ? gContext.mMVPLocal : gContext.mMVP;
            startOfSegment = mvp * startOfSegment;
            if (MathF.Abs(startOfSegment.w) > float.Epsilon) // check for axis aligned with camera direction
            {
                startOfSegment *= 1f / startOfSegment.w;
            }

            float4 endOfSegment = end;
            endOfSegment = mvp * endOfSegment;
            if (MathF.Abs(endOfSegment.w) > float.Epsilon) // check for axis aligned with camera direction
            {
                endOfSegment *= 1f / endOfSegment.w;
            }

            float4 clipSpaceAxis = endOfSegment - startOfSegment;
            clipSpaceAxis.y /= gContext.mDisplayRatio;
            float segmentLengthInClipSpace = MathF.Sqrt(clipSpaceAxis.x * clipSpaceAxis.x + clipSpaceAxis.y * clipSpaceAxis.y);
            return segmentLengthInClipSpace;
        }

        public static float GetParallelogram(float4 ptO, float4 ptA, float4 ptB)
        {
            var pts = new float4[] { ptO, ptA, ptB };
            for (var i = 0; i < pts.Length; i++)
            {
                pts[i] = gContext.mMVP * pts[i];

                if (MathF.Abs(pts[i].w) > float.Epsilon) // check for axis aligned with camera direction
                {
                    pts[i] *= 1f / pts[i].w;
                }
            }
            var segA = pts[1] - pts[0];
            var segB = pts[2] - pts[0];
            segA.y /= gContext.mDisplayRatio;
            segB.y /= gContext.mDisplayRatio;
            var segAOrtho = new float4(-segA.y, segA.x, 0, 0);
            segAOrtho.Normalize();
            float dt = float3.Dot(segAOrtho.xyz, segB.xyz);
            float surface = MathF.Sqrt(segA.x * segA.x + segA.y * segA.y) * MathF.Abs(dt);
            return surface;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PointOnSegment(Vector2 point, Vector2 vertPos1, Vector2 vertPos2)
        {
            var c = point - vertPos1;
            var V = (vertPos2 - vertPos1);
            V = Vector2.Normalize(V);
            var d = (vertPos2 - vertPos1).Length();
            var t = Vector2.Dot(V, c);

            if (t < 0f)
            {
                return vertPos1;
            }

            if (t > d)
            {
                return vertPos2;
            }

            return vertPos1 + V * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 PointOnSegment(float3 point, float3 vertPos1, float3 vertPos2)
        {
            var c = point - vertPos1;
            var V = (vertPos2 - vertPos1).Normalize();
            var d = (vertPos2 - vertPos1).Length;
            var t = float3.Dot(V, c);

            if (t < 0f)
            {
                return vertPos1;
            }

            if (t > d)
            {
                return vertPos2;
            }

            return vertPos1 + V * t;
        }

        public static float IntersectRayPlane(float4 rOrigin, float4 rVector, float4 plan)
        {
            var numer = float3.Dot(plan.xyz, rOrigin.xyz) - plan.w;
            var denom = float3.Dot(plan.xyz, rVector.xyz);

            if (MathF.Abs(denom) < float.Epsilon)  // normal is orthogonal to vector, cant intersect
            {
                return -1.0f;
            }

            return -(numer / denom);
        }

        public static float DistanceToPlane(float4 point, float4 plan)
        {
            return float3.Dot(plan.xyz, point.xyz) + plan.w;
        }

        public static bool IsInContextRect(Vector2 p)
        {
            return IsWithin(p.X, gContext.mX, gContext.mXMax) && IsWithin(p.Y, gContext.mY, gContext.mYMax);
        }

        public static bool IsWithin(float x, float y, float z) { return (x >= y) && (x <= z); }

        public static bool IsHoveringWindow()
        {
            return ImGui.IsWindowHovered();
        }

        public static void SetRect(float x, float y, float width, float height)
        {
            if (gContext == null) return;

            gContext.mX = x;
            gContext.mY = y;
            gContext.mWidth = width;
            gContext.mHeight = height;
            gContext.mXMax = gContext.mX + gContext.mWidth;
            gContext.mYMax = gContext.mY + gContext.mXMax;
            gContext.mDisplayRatio = width / height;
        }

        public static void SetOrthographic(bool isOrthographic)
        {
            gContext.mIsOrthographic = isOrthographic;
        }

        public static bool IsUsing()
        {
            return gContext.mbUsing || gContext.mbUsingBounds;
        }

        public static bool IsOver()
        {
            var noop = float3.Zero;
            return (Intersects(gContext.mOperation, OPERATION.TRANSLATE) && GetMoveType(gContext.mOperation, ref noop) != MOVETYPE.NONE) ||
               (Intersects(gContext.mOperation, OPERATION.ROTATE) && GetRotateType(gContext.mOperation) != MOVETYPE.NONE) ||
               (Intersects(gContext.mOperation, OPERATION.SCALE) && GetScaleType(gContext.mOperation) != MOVETYPE.NONE) || IsUsing();
        }

        public static bool IsOver(OPERATION op)
        {
            if (IsUsing())
            {
                return true;
            }
            if (Intersects(op, OPERATION.SCALE) && GetScaleType(op) != MOVETYPE.NONE)
            {
                return true;
            }
            if (Intersects(op, OPERATION.ROTATE) && GetRotateType(op) != MOVETYPE.NONE)
            {
                return true;
            }
            var noop = float3.Zero;
            if (Intersects(op, OPERATION.TRANSLATE) && GetMoveType(op, ref noop) != MOVETYPE.NONE)
            {
                return true;
            }
            return false;
        }

        public static void Enable(bool enable)
        {
            gContext.mbEnable = enable;
            if (!enable)
            {
                gContext.mbUsing = false;
                gContext.mbUsingBounds = false;
            }
        }

        public static float4 Right(this float4x4 mat)
        {
            return mat.Row1;
        }

        public static float4 Up(this float4x4 mat)
        {
            return mat.Row2;
        }

        public static float4 Dir(this float4x4 mat)
        {
            return mat.Row3;
        }

        public static float4 Position(this float4x4 mat)
        {
            return mat.Row4;
        }

        public static void OrthoNormalize(this ref float4x4 mat)
        {
            // right, up, dir, position;

            mat.Row1 = mat.Row1.Normalize();
            mat.Row2 = mat.Row2.Normalize();
            mat.Row3 = mat.Row3.Normalize();
        }

        public static void ComputeContext(float4x4 view, float4x4 projection, float4x4 matrix, MODE mode)
        {
            if(gContext == null)
                gContext = new();

            gContext.mMode = mode;
            gContext.mViewMat = view;
            gContext.mProjectionMat = projection;
            gContext.mbMouseOver = IsHoveringWindow();

            gContext.mModelLocal = matrix;
            gContext.mModelLocal.OrthoNormalize();

            if (mode == MODE.LOCAL)
            {
                gContext.mModel = gContext.mModelLocal;
            }
            else
            {
                gContext.mModel = float4x4.Identity;
                gContext.mModel.Row4 = matrix.Position();
            }
            gContext.mModelSource = matrix;
            gContext.mModelScaleOrigin = new float3(gContext.mModelSource.Right().xyz.Length, gContext.mModelSource.Up().xyz.Length, gContext.mModelSource.Dir().xyz.Length);

            gContext.mModelInverse = gContext.mModel.Invert();
            gContext.mModelSourceInverse = gContext.mModelSource.Invert();
            gContext.mViewProjection = gContext.mProjectionMat * gContext.mViewMat;
            gContext.mMVP = gContext.mViewProjection * gContext.mModel;
            gContext.mMVPLocal = gContext.mViewProjection * gContext.mModelLocal;

            var viewInverse = gContext.mViewMat.Invert();
            gContext.mCameraDir = viewInverse.Dir();
            gContext.mCameraEye = viewInverse.Position();
            gContext.mCameraRight = viewInverse.Right();
            gContext.mCameraUp = viewInverse.Up();

            // projection reverse
            var nearPos = gContext.mProjectionMat * new float4(0, 0, 1f, 1f);
            var farPos = gContext.mProjectionMat * new float4(0, 0, 2f, 1f);

            gContext.mReversed = (nearPos.z / nearPos.w) > (farPos.z / farPos.w);

            // compute scale from the size of camera right vector projected on screen at the matrix position
            //var pointRight = viewInverse.Right();
            //pointRight = gContext.mViewProjection * pointRight;
            //gContext.mScreenFactor = gContext.mGizmoSizeClipSpace / (pointRight.x / pointRight.w - gContext.mMVP.Position().x / gContext.mMVP.Position().w);

            var rightViewInverse = viewInverse.Right();
            rightViewInverse = gContext.mModelInverse * rightViewInverse;
            float rightLength = GetSegmentLengthClipSpace(float4.Zero, rightViewInverse);
            gContext.mScreenFactor = gContext.mGizmoSizeClipSpace / rightLength;

            var centerSSpace = Gizmos.WorldToPos(new float3(0f, 0f, 0f), gContext.mMVP);
            gContext.mScreenSquareCenter = centerSSpace;
            gContext.mScreenSquareMin = new Vector2(centerSSpace.X - 10f, centerSSpace.Y - 10f);
            gContext.mScreenSquareMax = new Vector2(centerSSpace.X + 10f, centerSSpace.Y + 10f);

            ComputeCameraRay(ref gContext.mRayOrigin, ref gContext.mRayVector);
        }

        public static void ComputeColors(ref Vector4[] colors, MOVETYPE type, OPERATION operation)
        {
            if (gContext.mbEnable)
            {
                switch (operation)
                {
                    case OPERATION.TRANSLATE:
                        colors[0] = (type == MOVETYPE.MOVE_SCREEN) ? selectionColor : new Vector4(255);
                        for (int i = 0; i < 3; i++)
                        {
                            colors[i + 1] = (type == (MOVETYPE.MOVE_X + i)) ? selectionColor : directionColor[i];
                            colors[i + 4] = (type == (MOVETYPE.MOVE_YZ + i)) ? selectionColor : planeColor[i];
                            colors[i + 4] = (type == MOVETYPE.MOVE_SCREEN) ? selectionColor : colors[i + 4];
                        }
                        break;
                    case OPERATION.ROTATE:
                        colors[0] = (type == MOVETYPE.ROTATE_SCREEN) ? selectionColor : new Vector4(255);
                        for (int i = 0; i < 3; i++)
                        {
                            colors[i + 1] = (type == (MOVETYPE.ROTATE_X + i)) ? selectionColor : directionColor[i];
                        }
                        break;
                    case OPERATION.SCALEU:
                    case OPERATION.SCALE:
                        colors[0] = (type == MOVETYPE.SCALE_XYZ) ? selectionColor : new Vector4(255);
                        for (int i = 0; i < 3; i++)
                        {
                            colors[i + 1] = (type == (MOVETYPE.SCALE_X + i)) ? selectionColor : directionColor[i];
                        }
                        break;
                    // note: this internal function is only called with three possible values for operation
                    default:
                        break;
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    colors[i] = inactiveColor;
                }
            }
        }

        public static void ComputeTripodAxisAndVisibility(int axisIndex, ref float4 dirAxis, ref float4 dirPlaneX, ref float4 dirPlaneY, ref bool belowAxisLimit, ref bool belowPlaneLimit, bool localCoordinates = false)
        {
            dirAxis = directionUnary[axisIndex];
            dirPlaneX = directionUnary[(axisIndex + 1) % 3];
            dirPlaneY = directionUnary[(axisIndex + 2) % 3];

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
            {
                // when using, use stored factors so the gizmo doesn't flip when we translate
                belowAxisLimit = gContext.mBelowAxisLimit[axisIndex];
                belowPlaneLimit = gContext.mBelowPlaneLimit[axisIndex];

                dirAxis *= gContext.mAxisFactor[axisIndex];
                dirPlaneX *= gContext.mAxisFactor[(axisIndex + 1) % 3];
                dirPlaneY *= gContext.mAxisFactor[(axisIndex + 2) % 3];
            }
            else
            {
                // new method
                float lenDir = GetSegmentLengthClipSpace(float4.UnitW, dirAxis, localCoordinates);
                float lenDirMinus = GetSegmentLengthClipSpace(float4.UnitW, -dirAxis, localCoordinates);

                float lenDirPlaneX = GetSegmentLengthClipSpace(float4.UnitW, dirPlaneX, localCoordinates);
                float lenDirMinusPlaneX = GetSegmentLengthClipSpace(float4.UnitW, -dirPlaneX, localCoordinates);

                float lenDirPlaneY = GetSegmentLengthClipSpace(float4.UnitW, dirPlaneY, localCoordinates);
                float lenDirMinusPlaneY = GetSegmentLengthClipSpace(float4.UnitW, -dirPlaneY, localCoordinates);

                // For readability
                bool allowFlip = gContext.mAllowAxisFlip;
                float mulAxis = (allowFlip && lenDir < lenDirMinus && MathF.Abs(lenDir - lenDirMinus) > float.Epsilon) ? -1f : 1f;
                float mulAxisX = (allowFlip && lenDirPlaneX < lenDirMinusPlaneX && MathF.Abs(lenDirPlaneX - lenDirMinusPlaneX) > float.Epsilon) ? -1f : 1f;
                float mulAxisY = (allowFlip && lenDirPlaneY < lenDirMinusPlaneY && MathF.Abs(lenDirPlaneY - lenDirMinusPlaneY) > float.Epsilon) ? -1f : 1f;
                dirAxis *= mulAxis;
                dirPlaneX *= mulAxisX;
                dirPlaneY *= mulAxisY;

                // for axis
                float axisLengthInClipSpace = GetSegmentLengthClipSpace(float4.UnitW, dirAxis * gContext.mScreenFactor, localCoordinates);

                float paraSurf = GetParallelogram(float4.UnitW, dirPlaneX * gContext.mScreenFactor, dirPlaneY * gContext.mScreenFactor);
                belowPlaneLimit = (paraSurf > 0.0025f);
                belowAxisLimit = (axisLengthInClipSpace > 0.02f);

                // and store values
                gContext.mAxisFactor[axisIndex] = mulAxis;
                gContext.mAxisFactor[(axisIndex + 1) % 3] = mulAxisX;
                gContext.mAxisFactor[(axisIndex + 2) % 3] = mulAxisY;
                gContext.mBelowAxisLimit[axisIndex] = belowAxisLimit;
                gContext.mBelowPlaneLimit[axisIndex] = belowPlaneLimit;
            }
        }

        public static void ComputeSnap(ref float value, float snap)
        {
            if (snap <= float.Epsilon)
            {
                return;
            }

            float modulo = value % snap;
            float moduloRatio = MathF.Abs(modulo) / snap;
            if (moduloRatio < snapTension)
            {
                value -= modulo;
            }
            else if (moduloRatio > (1f - snapTension))
            {
                value = value - modulo + snap * ((value < 0f) ? -1f : 1f);
            }
        }

        public static void ComputeSnap(ref float3 value, float[] snap)
        {
            for (int i = 0; i < 3; i++)
            {
                var valCpy = value[i];
                ComputeSnap(ref valCpy, snap[i]);
                value[i] = valCpy;
            }
        }

        public static void ComputeSnap(ref float4 value, float[] snap)
        {
            for (int i = 0; i < 4; i++)
            {
                var valCpy = value[i];
                ComputeSnap(ref valCpy, snap[i]);
                value[i] = valCpy;
            }
        }

        public static float ComputeAngleOnPlan()
        {
            var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
            var localPos = (gContext.mRayOrigin + gContext.mRayVector * len - gContext.mModel.Position()).Normalize();

            var perpendicularVector = float3.Cross(gContext.mRotationVectorSource.xyz, gContext.mTranslationPlan.xyz);
            perpendicularVector.Normalize();
            float acosAngle = M.Clamp(float3.Dot(localPos.xyz, gContext.mRotationVectorSource.xyz), -1f, 1f);
            float angle = MathF.Acos(acosAngle);
            angle *= (float3.Dot(localPos.xyz, perpendicularVector) < 0f) ? 1f : -1f;
            return angle;
        }

        public static void DrawRotationGizmo(OPERATION op, MOVETYPE type)
        {
            if (!Intersects(op, OPERATION.ROTATE))
            {
                return;
            }
            var drawList = gContext.mDrawList;

            // colors
            var colors = new Vector4[7];
            ComputeColors(ref colors, type, OPERATION.ROTATE);

            float4 cameraToModelNormalized;

            if (gContext.mIsOrthographic)
            {
                var viewInverse = gContext.mViewMat.Invert();
                cameraToModelNormalized = viewInverse.Dir();
            }
            else
            {
                cameraToModelNormalized = (gContext.mModel.Position() - gContext.mCameraEye).Normalize();
            }

            cameraToModelNormalized = gContext.mModelInverse * cameraToModelNormalized;

            gContext.mRadiusSquareCenter = screenRotateSize * gContext.mHeight;

            bool hasRSC = Intersects(op, OPERATION.ROTATE_SCREEN);
            for (int axis = 0; axis < 3; axis++)
            {
                if (!Intersects(op, ((uint)OPERATION.ROTATE_Z >> axis)))
                {
                    continue;
                }
                bool usingAxis = (gContext.mbUsing && type == MOVETYPE.ROTATE_Z - axis);
                int circleMul = (hasRSC && !usingAxis) ? 1 : 2;

                var circlePos = new List<Vector2>();
                for (var i = 0; i < circleMul * halfCircleSegmentCount + 1; i++)
                {
                    circlePos.Add(new Vector2(0, 0));
                }

                float angleStart = MathF.Atan2(cameraToModelNormalized[(4 - axis) % 3], cameraToModelNormalized[(3 - axis) % 3]) + M.Pi * 0.5f;

                for (int i = 0; i < circleMul * halfCircleSegmentCount + 1; i++)
                {
                    float ng = angleStart + circleMul * M.Pi * (i / (float)halfCircleSegmentCount);
                    var axisPos = new float3(MathF.Cos(ng), MathF.Sin(ng), 0f);
                    var pos = new float3(axisPos[axis], axisPos[(axis + 1) % 3], axisPos[(axis + 2) % 3]) * gContext.mScreenFactor * rotationDisplayFactor;
                    circlePos[i] = Gizmos.WorldToPos(pos.xyz, gContext.mMVP);
                }
                if (!gContext.mbUsing || usingAxis)
                {
                    drawList.AddPolyline(ref circlePos.ToArray()[0], circleMul * halfCircleSegmentCount + 1, Gizmos.Vector2Col(colors[3 - axis]), ImDrawFlags.Closed, 2);
                }

                float radiusAxis = MathF.Sqrt((Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection) - circlePos[0]).LengthSquared());
                if (radiusAxis > gContext.mRadiusSquareCenter)
                {
                    gContext.mRadiusSquareCenter = radiusAxis;
                }
            }
            if (hasRSC && (!gContext.mbUsing || type == MOVETYPE.ROTATE_SCREEN))
            {
                drawList.AddCircle(Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection), gContext.mRadiusSquareCenter, Gizmos.Vector2Col(colors[0]), 64, 3f);
            }

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsRotateType(type))
            {
                var circlePos = new Vector2[halfCircleSegmentCount + 1];

                circlePos[0] = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);
                for (var i = 1; i < halfCircleSegmentCount; i++)
                {
                    float ng = gContext.mRotationAngle * ((i - 1) / (float)(halfCircleSegmentCount - 1));

                    var rotateVectorMatrix = float4x4.CreateFromAxisAngle(gContext.mTranslationPlan.xyz, ng);
                    var pos = rotateVectorMatrix * gContext.mRotationVectorSource;
                    pos *= gContext.mScreenFactor * rotationDisplayFactor;
                    circlePos[i] = Gizmos.WorldToPos(pos.xyz + gContext.mModel.Position().xyz, gContext.mViewProjection);
                }
                drawList.AddConvexPolyFilled(ref circlePos[0], halfCircleSegmentCount, Gizmos.Vector2Col(new Vector4(0xFF, 0x80, 0x10, 0x80)));
                drawList.AddPolyline(ref circlePos[0], halfCircleSegmentCount, Gizmos.Vector2Col(new Vector4(0xFF, 0x80, 0x10, 0xFF)), ImDrawFlags.Closed, 2);

                var destinationPosOnScreen = circlePos[1];
                var tmps = string.Format(rotationInfoMask[type - MOVETYPE.ROTATE_X], (gContext.mRotationAngle / M.Pi) * 180f, gContext.mRotationAngle);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 15, destinationPosOnScreen.Y + 15), Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)), tmps);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 14, destinationPosOnScreen.Y + 14), Gizmos.Vector2Col(new Vector4(255)), tmps);
            }
        }

        public static void DrawHatchedAxis(float4 axis)
        {
            for (int j = 1; j < 10; j++)
            {
                var baseSSpace2 = Gizmos.WorldToPos(axis.xyz * 0.05f * (j * 2) * gContext.mScreenFactor, gContext.mMVP);
                var worldDirSSpace2 = Gizmos.WorldToPos(axis.xyz * 0.05f * (j * 2 + 1) * gContext.mScreenFactor, gContext.mMVP);
                gContext.mDrawList.AddLine(baseSSpace2, worldDirSSpace2, Gizmos.Vector2Col(new Vector4(0, 0, 0, 0x80)), 6f);
            }
        }

        public static void DrawScaleGizmo(OPERATION op, MOVETYPE type)
        {
            var drawList = gContext.mDrawList;

            if (!Intersects(op, OPERATION.SCALE))
            {
                return;
            }

            // colors
            var colors = new Vector4[7];
            ComputeColors(ref colors, type, OPERATION.SCALE);

            // draw
            var scaleDisplay = new float4(1f, 1f, 1f, 0f);

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
            {
                scaleDisplay = gContext.mScale;
            }

            for (var i = 0; i < 3; i++)
            {
                if (!Intersects(op, ((uint)OPERATION.SCALE_X << i)))
                {
                    continue;
                }
                var usingAxis = (gContext.mbUsing && type == MOVETYPE.SCALE_X + i);
                if (!gContext.mbUsing || usingAxis)
                {
                    var dirPlaneX = float4.Zero;
                    var dirPlaneY = float4.Zero;
                    var dirAxis = float4.Zero;
                    bool belowAxisLimit = false;
                    bool belowPlaneLimit = false;
                    ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit, true);

                    // draw axis
                    if (belowAxisLimit)
                    {
                        bool hasTranslateOnAxis = Contains(op, ((uint)OPERATION.TRANSLATE_X << i));
                        float markerScale = hasTranslateOnAxis ? 1.4f : 1.0f;
                        var baseSSpace = Gizmos.WorldToPos(dirAxis.xyz * 0.1f * gContext.mScreenFactor, gContext.mMVP);
                        var worldDirSSpaceNoScale = Gizmos.WorldToPos(dirAxis.xyz * markerScale * gContext.mScreenFactor, gContext.mMVP);
                        var worldDirSSpace = Gizmos.WorldToPos((dirAxis.xyz * markerScale * scaleDisplay[i]) * gContext.mScreenFactor, gContext.mMVP);

                        if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
                        {
                            drawList.AddLine(baseSSpace, worldDirSSpaceNoScale, Gizmos.Vector2Col(new Vector4(0x40, 0x40, 0x40, 0xFF)), 3f);
                            drawList.AddCircleFilled(worldDirSSpaceNoScale, 6f, Gizmos.Vector2Col(new Vector4(0x40, 0x40, 0x40, 0xFF)));
                        }

                        if (!hasTranslateOnAxis || gContext.mbUsing)
                        {
                            drawList.AddLine(baseSSpace, worldDirSSpace, Gizmos.Vector2Col(colors[i + 1]), 3f);
                        }
                        drawList.AddCircleFilled(worldDirSSpace, 6f, Gizmos.Vector2Col(colors[i + 1]));

                        if (gContext.mAxisFactor[i] < 0f)
                        {
                            DrawHatchedAxis(dirAxis * scaleDisplay[i]);
                        }
                    }
                }
            }

            // draw screen cirle
            drawList.AddCircleFilled(gContext.mScreenSquareCenter, 6f, Gizmos.Vector2Col(colors[0]), 32);

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsScaleType(type))
            {
                //ImVec2 sourcePosOnScreen = worldToPos(gContext.mMatrixOrigin, gContext.mViewProjection);
                var destinationPosOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);
                /*vec_t dif(destinationPosOnScreen.x - sourcePosOnScreen.x, destinationPosOnScreen.y - sourcePosOnScreen.y);
                dif.Normalize();
                dif *= 5f;
                drawList->AddCircle(sourcePosOnScreen, 6f, translationLineColor);
                drawList->AddCircle(destinationPosOnScreen, 6f, translationLineColor);
                drawList->AddLine(ImVec2(sourcePosOnScreen.x + dif.x, sourcePosOnScreen.y + dif.y), ImVec2(destinationPosOnScreen.x - dif.x, destinationPosOnScreen.y - dif.y), translationLineColor, 2f);
                */
                //vec_t deltaInfo = gContext.mModel.v.position - gContext.mMatrixOrigin;
                int componentInfoIndex = (type - MOVETYPE.SCALE_X) * 3;
                var tmps = string.Format(scaleInfoMask[type - MOVETYPE.SCALE_X], scaleDisplay[translationInfoIndex[componentInfoIndex]]);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 15, destinationPosOnScreen.Y + 15), Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)), tmps);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 14, destinationPosOnScreen.Y + 14), Gizmos.Vector2Col(new Vector4(255)), tmps);
            }
        }

        public static void DrawScaleUniveralGizmo(OPERATION op, MOVETYPE type)
        {
            var drawList = gContext.mDrawList;

            if (!Intersects(op, OPERATION.SCALEU))
            {
                return;
            }

            // colors
            var colors = new Vector4[7];
            ComputeColors(ref colors, type, OPERATION.SCALEU);

            // draw
            var scaleDisplay = new float4(1f, 1f, 1f, 1f);

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
            {
                scaleDisplay = gContext.mScale;
            }

            for (var i = 0; i < 3; i++)
            {
                if (!Intersects(op, ((uint)OPERATION.SCALE_XU << i)))
                {
                    continue;
                }
                bool usingAxis = (gContext.mbUsing && type == MOVETYPE.SCALE_X + i);
                if (!gContext.mbUsing || usingAxis)
                {
                    var dirPlaneX = float4.Zero;
                    var dirPlaneY = float4.Zero;
                    var dirAxis = float4.Zero;
                    bool belowAxisLimit = false;
                    bool belowPlaneLimit = false;

                    ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit, true);

                    // draw axis
                    if (belowAxisLimit)
                    {
                        bool hasTranslateOnAxis = Contains(op, ((uint)OPERATION.TRANSLATE_X << i));
                        float markerScale = hasTranslateOnAxis ? 1.4f : 1.0f;
                        var baseSSpace = Gizmos.WorldToPos(dirAxis.xyz * 0.1f * gContext.mScreenFactor, gContext.mMVPLocal);
                        //ImVec2 worldDirSSpaceNoScale = worldToPos(dirAxis * markerScale * gContext.mScreenFactor, gContext.mMVP);
                        var worldDirSSpace = Gizmos.WorldToPos((dirAxis.xyz * markerScale * scaleDisplay[i]) * gContext.mScreenFactor, gContext.mMVPLocal);

                        /*if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
                        {
                           drawList->AddLine(baseSSpace, worldDirSSpaceNoScale, IM_COL32(0x40, 0x40, 0x40, 0xFF), 3f);
                           drawList->AddCircleFilled(worldDirSSpaceNoScale, 6f, IM_COL32(0x40, 0x40, 0x40, 0xFF));
                        }
                        /*
                        if (!hasTranslateOnAxis || gContext.mbUsing)
                        {
                           drawList->AddLine(baseSSpace, worldDirSSpace, colors[i + 1], 3f);
                        }
                        */
                        drawList.AddCircleFilled(worldDirSSpace, 12f, Gizmos.Vector2Col(colors[i + 1]));
                    }
                }
            }

            // draw screen cirle
            drawList.AddCircle(gContext.mScreenSquareCenter, 20f, Gizmos.Vector2Col(colors[0]), 32, 3f);

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsScaleType(type))
            {
                //ImVec2 sourcePosOnScreen = worldToPos(gContext.mMatrixOrigin, gContext.mViewProjection);
                var destinationPosOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);
                /*vec_t dif(destinationPosOnScreen.x - sourcePosOnScreen.x, destinationPosOnScreen.y - sourcePosOnScreen.y);
                dif.Normalize();
                dif *= 5f;
                drawList->AddCircle(sourcePosOnScreen, 6f, translationLineColor);
                drawList->AddCircle(destinationPosOnScreen, 6f, translationLineColor);
                drawList->AddLine(ImVec2(sourcePosOnScreen.x + dif.x, sourcePosOnScreen.y + dif.y), ImVec2(destinationPosOnScreen.x - dif.x, destinationPosOnScreen.y - dif.y), translationLineColor, 2f);
                */

                //vec_t deltaInfo = gContext.mModel.v.position - gContext.mMatrixOrigin;
                int componentInfoIndex = (type - MOVETYPE.SCALE_X) * 3;
                var tmps = string.Format(scaleInfoMask[type - MOVETYPE.SCALE_X], scaleDisplay[translationInfoIndex[componentInfoIndex]]);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 15, destinationPosOnScreen.Y + 15), Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)), tmps);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 14, destinationPosOnScreen.Y + 14), Gizmos.Vector2Col(new Vector4(255)), tmps);
            }
        }

        public static float4 BuildPlan(float4 p_point1, float4 p_normal)
        {
            var res = float4.Zero;
            var normal = p_normal.Normalize();
            res.w = float3.Dot(normal.xyz, p_point1.xyz);
            res.x = normal.x;
            res.y = normal.y;
            res.z = normal.z;
            return res;
        }

        public static void DrawTranslationGizmo(OPERATION op, MOVETYPE type)
        {
            var drawList = gContext.mDrawList;

            if (!Intersects(op, OPERATION.TRANSLATE))
            {
                return;
            }

            // colors
            var colors = new Vector4[7];
            ComputeColors(ref colors, type, OPERATION.TRANSLATE);

            var origin = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);

            // draw
            bool belowAxisLimit = false;
            bool belowPlaneLimit = false;
            for (var i = 0; i < 3; ++i)
            {
                var dirPlaneX = float4.Zero;
                var dirPlaneY = float4.Zero;
                var dirAxis = float4.Zero;

                ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit);

                if (!gContext.mbUsing || (gContext.mbUsing && type == MOVETYPE.MOVE_X + i))
                {
                    // draw axis
                    if (belowAxisLimit && Intersects(op, ((uint)OPERATION.TRANSLATE_X << i)))
                    {
                        var baseSSpace = Gizmos.WorldToPos(dirAxis.xyz * 0.1f * gContext.mScreenFactor, gContext.mMVP);
                        var worldDirSSpace = Gizmos.WorldToPos(dirAxis.xyz * gContext.mScreenFactor, gContext.mMVP);

                        drawList.AddLine(baseSSpace, worldDirSSpace, Gizmos.Vector2Col(colors[i + 1]), 3f);

                        // ArRow head begin
                        var dir = (origin - worldDirSSpace);

                        float d = MathF.Sqrt((dir.LengthSquared()));
                        dir /= d; // Normalize
                        dir *= 6.0f;

                        var ortogonalDir = new Vector2(dir.Y, -dir.X); // Perpendicular vector
                        var a = (worldDirSSpace + dir);
                        drawList.AddTriangleFilled(worldDirSSpace - dir, a + ortogonalDir, a - ortogonalDir, Gizmos.Vector2Col(colors[i + 1]));
                        // ArRow head end

                        if (gContext.mAxisFactor[i] < 0f)
                        {
                            DrawHatchedAxis(dirAxis);
                        }
                    }
                }
                // draw plane
                if (!gContext.mbUsing || (gContext.mbUsing && type == MOVETYPE.MOVE_YZ + i))
                {
                    if (belowPlaneLimit && Contains(op, TRANSLATE_PLANS[i]))
                    {
                        var screenQuadPts = new Vector2[4];
                        for (int j = 0; j < 4; ++j)
                        {
                            var cornerWorldPos = (dirPlaneX * quadUV[j * 2] + dirPlaneY * quadUV[j * 2 + 1]) * gContext.mScreenFactor;
                            screenQuadPts[j] = Gizmos.WorldToPos(cornerWorldPos.xyz, gContext.mMVP);
                        }
                        drawList.AddPolyline(ref screenQuadPts[0], 4, Gizmos.Vector2Col(directionColor[i]), ImDrawFlags.Closed, 1.0f);
                        drawList.AddConvexPolyFilled(ref screenQuadPts[0], 4, Gizmos.Vector2Col(colors[i + 4]));
                    }
                }
            }

            drawList.AddCircleFilled(gContext.mScreenSquareCenter, 6f, Gizmos.Vector2Col(colors[0]), 32);

            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsTranslateType(type))
            {
                var sourcePosOnScreen = Gizmos.WorldToPos(gContext.mMatrixOrigin.xyz, gContext.mViewProjection);
                var destinationPosOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);
                var dif = new float4(destinationPosOnScreen.X - sourcePosOnScreen.X, destinationPosOnScreen.Y - sourcePosOnScreen.Y, 0, 0);
                dif.Normalize();
                dif *= 5f;
                drawList.AddCircle(sourcePosOnScreen, 6f, Gizmos.Vector2Col(translationLineColor));
                drawList.AddCircle(destinationPosOnScreen, 6f, Gizmos.Vector2Col(translationLineColor));
                drawList.AddLine(new Vector2(sourcePosOnScreen.X + dif.x, sourcePosOnScreen.Y + dif.y), new Vector2(destinationPosOnScreen.X - dif.x, destinationPosOnScreen.Y - dif.y), Gizmos.Vector2Col(translationLineColor), 2f);

                var deltaInfo = gContext.mModel.Position() - gContext.mMatrixOrigin;
                int componentInfoIndex = (type - MOVETYPE.MOVE_X) * 3;
                var tmps = string.Format(translationInfoMask[type - MOVETYPE.MOVE_X], deltaInfo[translationInfoIndex[componentInfoIndex]], deltaInfo[translationInfoIndex[componentInfoIndex + 1]], deltaInfo[translationInfoIndex[componentInfoIndex + 2]]);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 15, destinationPosOnScreen.Y + 15), Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)), tmps);
                drawList.AddText(new Vector2(destinationPosOnScreen.X + 14, destinationPosOnScreen.Y + 14), Gizmos.Vector2Col(new Vector4(255)), tmps);
            }
        }

        public static bool CanActivate()
        {
            return (ImGui.IsMouseClicked(0) && !ImGui.IsAnyItemHovered() && !ImGui.IsAnyItemActive());
        }

        public static void HandleAndDrawLocalBounds(ref float[] bounds, ref float4x4 matrix, ref float[] snapValues, OPERATION operation)
        {
            var io = ImGui.GetIO();
            var drawList = gContext.mDrawList;

            // compute best projection axis
            var axesWorldDirections = new float4[3];
            var bestAxisWorldDirection = float4.Zero;
            var axes = new int[3];
            var numAxes = 1;
            axes[0] = gContext.mBoundsBestAxis;
            int bestAxis = axes[0];
            if (!gContext.mbUsingBounds)
            {
                numAxes = 0;
                float bestDot = 0f;
                for (var i = 0; i < 3; i++)
                {
                    var dirPlaneNormalWorld = (gContext.mModelSource * directionUnary[i]).Normalize();


                    float dt = MathF.Abs(float3.Dot((gContext.mCameraEye.xyz - gContext.mModelSource.Position().xyz).Normalize(), dirPlaneNormalWorld.xyz));
                    if (dt >= bestDot)
                    {
                        bestDot = dt;
                        bestAxis = i;
                        bestAxisWorldDirection = dirPlaneNormalWorld;
                    }

                    if (dt >= 0.1f)
                    {
                        axes[numAxes] = i;
                        axesWorldDirections[numAxes] = dirPlaneNormalWorld;
                        ++numAxes;
                    }
                }
            }

            if (numAxes == 0)
            {
                axes[0] = bestAxis;
                axesWorldDirections[0] = bestAxisWorldDirection;
                numAxes = 1;
            }

            else if (bestAxis != axes[0])
            {
                var bestIndex = 0;
                for (var i = 0; i < numAxes; i++)
                {
                    if (axes[i] == bestAxis)
                    {
                        bestIndex = i;
                        break;
                    }
                }
                int tempAxis = axes[0];
                axes[0] = axes[bestIndex];
                axes[bestIndex] = tempAxis;
                var tempDirection = axesWorldDirections[0];
                axesWorldDirections[0] = axesWorldDirections[bestIndex];
                axesWorldDirections[bestIndex] = tempDirection;
            }

            for (var axisIndex = 0; axisIndex < numAxes; ++axisIndex)
            {
                bestAxis = axes[axisIndex];
                bestAxisWorldDirection = axesWorldDirections[axisIndex];

                // corners
                var aabb = new float4[4];

                int secondAxis = (bestAxis + 1) % 3;
                int thirdAxis = (bestAxis + 2) % 3;

                for (int i = 0; i < 4; i++)
                {
                    aabb[i][3] = aabb[i][bestAxis] = 0f;
                    aabb[i][secondAxis] = bounds[secondAxis + 3 * (i >> 1)];
                    aabb[i][thirdAxis] = bounds[thirdAxis + 3 * ((i >> 1) ^ (i & 1))];
                }

                // draw bounds
                var anchorAlpha = gContext.mbEnable ? Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)) : Gizmos.Vector2Col(new Vector4(0, 0, 0, 0x80));

                var boundsMVP = gContext.mViewProjection * gContext.mModelSource;
                for (int i = 0; i < 4; i++)
                {
                    var worldBound1 = Gizmos.WorldToPos(aabb[i].xyz, boundsMVP);
                    var worldBound2 = Gizmos.WorldToPos(aabb[(i + 1) % 4].xyz, boundsMVP);
                    if (!IsInContextRect(worldBound1) || !IsInContextRect(worldBound2))
                    {
                        continue;
                    }
                    float boundDistance = MathF.Sqrt((worldBound1 - worldBound2).LengthSquared());
                    int stepCount = (int)(boundDistance / 10f);
                    stepCount = M.Min(stepCount, 1000);
                    float stepLength = 1f / stepCount;
                    for (int j = 0; j < stepCount; j++)
                    {
                        float t1 = j * stepLength;
                        float t2 = j * stepLength + stepLength * 0.5f;
                        var worldBoundSS1 = Vector2.Lerp(worldBound1, worldBound2, t1);
                        var worldBoundSS2 = Vector2.Lerp(worldBound1, worldBound2, t2);
                        //drawList->AddLine(worldBoundSS1, worldBoundSS2, IM_COL32(0, 0, 0, 0) + anchorAlpha, 3.f);
                        drawList.AddLine(worldBoundSS1, worldBoundSS2, Gizmos.Vector2Col(new Vector4(0xAA, 0xAA, 0xAA, 0)) + anchorAlpha, 2f);
                    }
                    var midPoint = (aabb[i] + aabb[(i + 1) % 4]) * 0.5f;
                    var midBound = Gizmos.WorldToPos(midPoint.xyz, boundsMVP);
                    var AnchorBigRadius = 8f;
                    var AnchorSmallRadius = 6f;
                    bool overBigAnchor = (worldBound1 - io.MousePos).LengthSquared() <= (AnchorBigRadius * AnchorBigRadius);
                    bool overSmallAnchor = (midBound - io.MousePos).LengthSquared() <= (AnchorBigRadius * AnchorBigRadius);

                    var type = MOVETYPE.NONE;
                    var gizmoHitProportion = new float3();

                    if (Intersects(operation, OPERATION.TRANSLATE))
                    {
                        type = GetMoveType(operation, ref gizmoHitProportion);
                    }
                    if (Intersects(operation, OPERATION.ROTATE) && type == MOVETYPE.NONE)
                    {
                        type = GetRotateType(operation);
                    }
                    if (Intersects(operation, OPERATION.SCALE) && type == MOVETYPE.NONE)
                    {
                        type = GetScaleType(operation);
                    }

                    if (type != MOVETYPE.NONE)
                    {
                        overBigAnchor = false;
                        overSmallAnchor = false;
                    }

                    var bigAnchorColor = overBigAnchor ? Gizmos.Vector2Col(selectionColor) : (Gizmos.Vector2Col(new Vector4(0xAA, 0xAA, 0xAA, anchorAlpha)));
                    var smallAnchorColor = overSmallAnchor ? Gizmos.Vector2Col(selectionColor) : (Gizmos.Vector2Col((new Vector4(0xAA, 0xAA, 0xAA, anchorAlpha))));

                    drawList.AddCircleFilled(worldBound1, AnchorBigRadius, Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)));
                    drawList.AddCircleFilled(worldBound1, AnchorBigRadius - 1.2f, bigAnchorColor);

                    drawList.AddCircleFilled(midBound, AnchorSmallRadius, Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)));
                    drawList.AddCircleFilled(midBound, AnchorSmallRadius - 1.2f, smallAnchorColor);
                    int oppositeIndex = (i + 2) % 4;
                    // big anchor on corners
                    if (!gContext.mbUsingBounds && gContext.mbEnable && overBigAnchor && CanActivate())
                    {
                        gContext.mBoundsPivot = gContext.mModelSource * aabb[(i + 2) % 4];
                        gContext.mBoundsAnchor = gContext.mModelSource * aabb[i];
                        gContext.mBoundsPlan = BuildPlan(gContext.mBoundsAnchor, bestAxisWorldDirection);
                        gContext.mBoundsBestAxis = bestAxis;
                        gContext.mBoundsAxis[0] = secondAxis;
                        gContext.mBoundsAxis[1] = thirdAxis;

                        gContext.mBoundsLocalPivot = float4.Zero;
                        gContext.mBoundsLocalPivot[secondAxis] = aabb[oppositeIndex][secondAxis];
                        gContext.mBoundsLocalPivot[thirdAxis] = aabb[oppositeIndex][thirdAxis];

                        gContext.mbUsingBounds = true;
                        gContext.mEditingID = gContext.mActualID;
                        gContext.mBoundsMatrix = gContext.mModelSource;
                    }
                    // small anchor on middle of segment
                    if (!gContext.mbUsingBounds && gContext.mbEnable && overSmallAnchor && CanActivate())
                    {
                        var midPointOpposite = (aabb[(i + 2) % 4] + aabb[(i + 3) % 4]) * 0.5f;
                        gContext.mBoundsPivot = gContext.mModelSource * midPointOpposite;
                        gContext.mBoundsAnchor = gContext.mModelSource * midPoint;
                        gContext.mBoundsPlan = BuildPlan(gContext.mBoundsAnchor, bestAxisWorldDirection);
                        gContext.mBoundsBestAxis = bestAxis;
                        var indices = new int[] { secondAxis, thirdAxis };
                        gContext.mBoundsAxis[0] = indices[i % 2];
                        gContext.mBoundsAxis[1] = -1;

                        gContext.mBoundsLocalPivot = float4.Zero;
                        gContext.mBoundsLocalPivot[gContext.mBoundsAxis[0]] = aabb[oppositeIndex][indices[i % 2]];// bounds[gContext.mBoundsAxis[0]] * (((i + 1) & 2) ? 1.f : -1.f);

                        gContext.mbUsingBounds = true;
                        gContext.mEditingID = gContext.mActualID;
                        gContext.mBoundsMatrix = gContext.mModelSource;
                    }
                }

                if (gContext.mbUsingBounds && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID))
                {
                    var scale = float4x4.Identity;

                    // compute projected mouse position on plan
                    float len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mBoundsPlan);
                    var newPos = gContext.mRayOrigin + gContext.mRayVector * len;

                    // compute a reference and delta vectors base on mouse move

                    var deltaVector = new float4(
                        MathF.Abs(newPos.x - gContext.mBoundsPivot.x),
                        MathF.Abs(newPos.y - gContext.mBoundsPivot.y),
                        MathF.Abs(newPos.z - gContext.mBoundsPivot.z),
                        MathF.Abs(newPos.w - gContext.mBoundsPivot.w));
                    var referenceVector = new float4(
                        MathF.Abs(gContext.mBoundsAnchor.x - gContext.mBoundsPivot.x),
                        MathF.Abs(gContext.mBoundsAnchor.y - gContext.mBoundsPivot.y),
                        MathF.Abs(gContext.mBoundsAnchor.z - gContext.mBoundsPivot.z),
                        MathF.Abs(gContext.mBoundsAnchor.w - gContext.mBoundsPivot.w));

                    // for 1 or 2 axes, compute a ratio that's used for scale and snap it based on resulting length
                    for (int i = 0; i < 2; i++)
                    {
                        int axisIndex1 = gContext.mBoundsAxis[i];
                        if (axisIndex1 == -1)
                        {
                            continue;
                        }

                        float ratioAxis = 1f;
                        float4 axisDir = float4.Zero;
                        switch (axisIndex1)
                        {
                            case 0:
                                axisDir = new float4(MathF.Abs(
                                    gContext.mBoundsMatrix.Row1.x),
                                    MathF.Abs(gContext.mBoundsMatrix.Row1.y),
                                    MathF.Abs(gContext.mBoundsMatrix.Row1.z),
                                    MathF.Abs(gContext.mBoundsMatrix.Row1.w));
                                break;
                            case 1:
                                axisDir = new float4(MathF.Abs(
                                    gContext.mBoundsMatrix.Row2.x),
                                    MathF.Abs(gContext.mBoundsMatrix.Row2.y),
                                    MathF.Abs(gContext.mBoundsMatrix.Row2.z),
                                    MathF.Abs(gContext.mBoundsMatrix.Row2.w));
                                break;
                            case 2:
                                axisDir = new float4(MathF.Abs(
                                    gContext.mBoundsMatrix.Row3.x),
                                    MathF.Abs(gContext.mBoundsMatrix.Row3.y),
                                    MathF.Abs(gContext.mBoundsMatrix.Row3.z),
                                    MathF.Abs(gContext.mBoundsMatrix.Row3.w));
                                break;
                            case 3:
                                axisDir = new float4(MathF.Abs(
                                    gContext.mBoundsMatrix.Row4.x),
                                    MathF.Abs(gContext.mBoundsMatrix.Row4.y),
                                    MathF.Abs(gContext.mBoundsMatrix.Row4.z),
                                    MathF.Abs(gContext.mBoundsMatrix.Row4.w));
                                break;
                        }


                        float dtAxis = float3.Dot(axisDir.xyz, referenceVector.xyz);
                        float boundSize = bounds[axisIndex1 + 3] - bounds[axisIndex1];
                        if (dtAxis > float.Epsilon)
                        {
                            ratioAxis = float3.Dot(axisDir.xyz, deltaVector.xyz) / dtAxis;
                        }

                        if (snapValues != null)
                        {
                            float length = boundSize * ratioAxis;
                            ComputeSnap(ref length, snapValues[axisIndex1]);
                            if (boundSize > float.Epsilon)
                            {
                                ratioAxis = length / boundSize;
                            }
                        }

                        switch (axisIndex1)
                        {
                            case 0:
                                scale.Row1 *= ratioAxis;
                                break;
                            case 1:
                                scale.Row2 *= ratioAxis;
                                break;
                            case 2:
                                scale.Row3 *= ratioAxis;
                                break;
                            case 3:
                                scale.Row4 *= ratioAxis;
                                break;
                        }
                    }

                    // transform matrix
                    var preScale = float4x4.Identity;
                    preScale.Row4 = -gContext.mBoundsLocalPivot;

                    var postScale = float4x4.Identity;
                    preScale.Row4 = gContext.mBoundsLocalPivot;
                    var res = gContext.mBoundsMatrix * postScale * scale * preScale;
                    matrix = res;

                    // info text
                    var destinationPosOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz, gContext.mViewProjection);
                    var tmps = string.Format("X: {0:N2} Y: {0:N2} Z: {0:N2}"
                       , (bounds[3] - bounds[0]) * gContext.mBoundsMatrix.Row1.Length * scale.Row1.Length
                       , (bounds[4] - bounds[1]) * gContext.mBoundsMatrix.Row2.Length * scale.Row2.Length
                       , (bounds[5] - bounds[2]) * gContext.mBoundsMatrix.Row3.Length * scale.Row3.Length
                    );
                    drawList.AddText(new Vector2(destinationPosOnScreen.X + 15, destinationPosOnScreen.Y + 15), Gizmos.Vector2Col(new Vector4(0, 0, 0, 255)), tmps);
                    drawList.AddText(new Vector2(destinationPosOnScreen.X + 14, destinationPosOnScreen.Y + 14), Gizmos.Vector2Col(new Vector4(255)), tmps);
                }

                if (!io.MouseDown[0])
                {
                    gContext.mbUsingBounds = false;
                    gContext.mEditingID = -1;
                }
                if (gContext.mbUsingBounds)
                {
                    break;
                }
            }
        }

        public static MOVETYPE GetScaleType(OPERATION op)
        {
            if (gContext.mbUsing)
            {
                return MOVETYPE.NONE;
            }
            var io = ImGui.GetIO();
            var type = MOVETYPE.NONE;

            // screen
            if (io.MousePos.X >= gContext.mScreenSquareMin.X && io.MousePos.X <= gContext.mScreenSquareMax.X &&
               io.MousePos.Y >= gContext.mScreenSquareMin.Y && io.MousePos.Y <= gContext.mScreenSquareMax.Y &&
               Contains(op, OPERATION.SCALE))
            {
                type = MOVETYPE.SCALE_XYZ;
            }

            // compute
            for (var i = 0; i < 3 && type == MOVETYPE.NONE; i++)
            {
                if (!Intersects(op, ((uint)MOVETYPE.SCALE_X << i)))
                {
                    continue;
                }
                var dirPlaneX = float4.Zero;
                var dirPlaneY = float4.Zero;
                var dirAxis = float4.Zero;
                bool belowAxisLimit = false;
                bool belowPlaneLimit = false;

                ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit, true);
                dirAxis = gContext.mModelLocal * dirAxis;
                dirPlaneX = gContext.mModelLocal * dirPlaneX;
                dirPlaneY = gContext.mModelLocal * dirPlaneY;

                float len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, BuildPlan(gContext.mModelLocal.Position(), dirAxis));
                var posOnPlan = gContext.mRayOrigin + gContext.mRayVector * len;

                var startOffset = Contains(op, ((uint)OPERATION.TRANSLATE_X << i)) ? 1.0f : 0.1f;
                var endOffset = Contains(op, ((uint)OPERATION.TRANSLATE_X << i)) ? 1.4f : 1.0f;
                var posOnPlanScreen = Gizmos.WorldToPos(posOnPlan.xyz, gContext.mViewProjection);
                var axisStartOnScreen = Gizmos.WorldToPos(gContext.mModelLocal.Position().xyz + dirAxis.xyz * gContext.mScreenFactor * startOffset, gContext.mViewProjection);
                var axisEndOnScreen = Gizmos.WorldToPos(gContext.mModelLocal.Position().xyz + dirAxis.xyz * gContext.mScreenFactor * endOffset, gContext.mViewProjection);

                var closestPointOnAxis = PointOnSegment(posOnPlanScreen, axisStartOnScreen, axisEndOnScreen);

                if ((closestPointOnAxis - posOnPlanScreen).Length() < 12f) // pixel size
                {
                    type = MOVETYPE.SCALE_X + i;
                }
            }

            // universal

            var deltaScreen = new float4(io.MousePos.X - gContext.mScreenSquareCenter.X, io.MousePos.Y - gContext.mScreenSquareCenter.Y, 0f, 0f);
            float dist = deltaScreen.Length;
            if (Contains(op, OPERATION.SCALEU) && dist >= 17.0f && dist < 23.0f)
            {
                type = MOVETYPE.SCALE_XYZ;
            }

            for (var i = 0; i < 3 && type == MOVETYPE.NONE; i++)
            {
                if (!Intersects(op, ((uint)OPERATION.SCALE_XU << i)))
                {
                    continue;
                }

                var dirPlaneX = float4.Zero;
                var dirPlaneY = float4.Zero;
                var dirAxis = float4.Zero;
                bool belowAxisLimit = false;
                bool belowPlaneLimit = false;

                ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit, true);

                // draw axis
                if (belowAxisLimit)
                {
                    bool hasTranslateOnAxis = Contains(op, ((uint)OPERATION.TRANSLATE_X << i));
                    float markerScale = hasTranslateOnAxis ? 1.4f : 1.0f;
                    var baseSSpace = Gizmos.WorldToPos(dirAxis.xyz * 0.1f * gContext.mScreenFactor, gContext.mMVPLocal);
                    //ImVec2 worldDirSSpaceNoScale = worldToPos(dirAxis * markerScale * gContext.mScreenFactor, gContext.mMVP);
                    var worldDirSSpace = Gizmos.WorldToPos((dirAxis.xyz * markerScale) * gContext.mScreenFactor, gContext.mMVPLocal);

                    float distance = MathF.Sqrt((worldDirSSpace - io.MousePos).LengthSquared());
                    if (distance < 12f)
                    {
                        type = MOVETYPE.SCALE_X + i;
                    }
                }
            }
            return type;
        }

        public static bool Intersects(OPERATION lhs, OPERATION rhs)
        {
            return (lhs & rhs) != 0;
        }

        public static bool Intersects(OPERATION lhs, uint rhs)
        {
            return ((uint)lhs & rhs) != 0;
        }

        public static bool Intersects(uint lhs, uint rhs)
        {
            return (lhs & rhs) != 0;
        }

        // True if lhs contains rhs
        public static bool Contains(OPERATION lhs, OPERATION rhs)
        {
            return (lhs & rhs) == rhs;
        }

        public static bool Contains(OPERATION lhs, uint rhs)
        {
            return ((uint)lhs & rhs) == rhs;
        }

        public static bool Contains(uint lhs, uint rhs)
        {
            return (lhs & rhs) == rhs;
        }

        public static MOVETYPE GetRotateType(OPERATION op)
        {
            if (gContext.mbUsing)
            {
                return MOVETYPE.NONE;
            }
            var io = ImGui.GetIO();
            var type = MOVETYPE.NONE;

            var deltaScreen = new float4(io.MousePos.X - gContext.mScreenSquareCenter.X, io.MousePos.Y - gContext.mScreenSquareCenter.Y, 0f, 0f);
            float dist = deltaScreen.Length;
            if (Intersects(op, OPERATION.ROTATE_SCREEN) && dist >= (gContext.mRadiusSquareCenter - 4.0f) && dist < (gContext.mRadiusSquareCenter + 4.0f))
            {
                type = MOVETYPE.ROTATE_SCREEN;
            }

            var planNormals = new float4[] { gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir() };

            var modelViewPos = gContext.mViewMat * gContext.mModel.Position();

            for (var i = 0; i < 3 && type == MOVETYPE.NONE; i++)
            {
                if (!Intersects(op, ((uint)OPERATION.ROTATE_X << i)))
                {
                    continue;
                }
                // pickup plan
                var pickupPlan = BuildPlan(gContext.mModel.Position(), planNormals[i]);

                var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, pickupPlan);
                var intersectWorldPos = gContext.mRayOrigin + gContext.mRayVector * len;
                var intersectViewPos = gContext.mViewMat * intersectWorldPos;

                if (MathF.Abs(modelViewPos.z) - MathF.Abs(intersectViewPos.z) < -float.Epsilon)
                {
                    continue;
                }

                var localPos = intersectWorldPos - gContext.mModel.Position();
                var idealPosOnCircle = localPos.Normalize();
                idealPosOnCircle = gContext.mModelInverse * idealPosOnCircle;
                var idealPosOnCircleScreen = Gizmos.WorldToPos(idealPosOnCircle.xyz * rotationDisplayFactor * gContext.mScreenFactor, gContext.mMVP);

                //gContext.mDrawList->AddCircle(idealPosOnCircleScreen, 5.f, IM_COL32_WHITE);
                var distanceOnScreen = idealPosOnCircleScreen - io.MousePos;

                var distance = (distanceOnScreen).Length();
                if (distance < 8f) // pixel size
                {
                    type = MOVETYPE.ROTATE_X + i;
                }
            }

            return type;
        }

        public static MOVETYPE GetMoveType(OPERATION op, ref float3 gizmoHitProportion)
        {
            if (!Intersects(op, OPERATION.TRANSLATE) || gContext.mbUsing || !gContext.mbMouseOver)
            {
                return MOVETYPE.NONE;
            }
            var io = ImGui.GetIO();
            var type = MOVETYPE.NONE;

            // screen
            if (io.MousePos.X >= gContext.mScreenSquareMin.X && io.MousePos.X <= gContext.mScreenSquareMax.X &&
               io.MousePos.Y >= gContext.mScreenSquareMin.Y && io.MousePos.Y <= gContext.mScreenSquareMax.Y &&
               Contains(op, OPERATION.TRANSLATE))
            {
                type = MOVETYPE.MOVE_SCREEN;
            }

            var screenCoord = (io.MousePos - new Vector2(gContext.mX, gContext.mY));

            // compute
            for (var i = 0; i < 3 && type == MOVETYPE.NONE; i++)
            {
                bool belowAxisLimit = false;
                bool belowPlaneLimit = false;

                var dirPlaneX = float4.Zero;
                var dirPlaneY = float4.Zero;
                var dirAxis = float4.Zero;
                ComputeTripodAxisAndVisibility(i, ref dirAxis, ref dirPlaneX, ref dirPlaneY, ref belowAxisLimit, ref belowPlaneLimit);
                dirAxis = gContext.mModel * dirAxis;
                dirPlaneX = gContext.mModel * dirPlaneX;
                dirPlaneY = gContext.mModel * dirPlaneY;

                var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, BuildPlan(gContext.mModel.Position(), dirAxis));
                var posOnPlan = gContext.mRayOrigin + gContext.mRayVector * len;

                var axisStartOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz + dirAxis.xyz * gContext.mScreenFactor * 0.1f, gContext.mViewProjection) - new Vector2(gContext.mX, gContext.mY);
                var axisEndOnScreen = Gizmos.WorldToPos(gContext.mModel.Position().xyz + dirAxis.xyz * gContext.mScreenFactor, gContext.mViewProjection) - new Vector2(gContext.mX, gContext.mY);

                var closestPointOnAxis = PointOnSegment(screenCoord, axisStartOnScreen, axisEndOnScreen);
                if ((closestPointOnAxis - screenCoord).Length() < 12f && Intersects(op, ((uint)OPERATION.TRANSLATE_X << i))) // pixel size
                {
                    type = MOVETYPE.MOVE_X + i;
                }

                var dx = float3.Dot(dirPlaneX.xyz, ((posOnPlan.xyz - gContext.mModel.Position().xyz) * (1f / gContext.mScreenFactor)));
                var dy = float3.Dot(dirPlaneY.xyz, ((posOnPlan.xyz - gContext.mModel.Position().xyz) * (1f / gContext.mScreenFactor)));
                if (belowPlaneLimit && dx >= quadUV[0] && dx <= quadUV[4] && dy >= quadUV[1] && dy <= quadUV[3] && Contains(op, TRANSLATE_PLANS[i]))
                {
                    type = MOVETYPE.MOVE_YZ + i;
                }

                if (gizmoHitProportion != float3.Zero)
                {
                    gizmoHitProportion = new float3(dx, dy, 0f);
                }
            }
            return type;
        }

        public static bool HandleTranslation(ref float4x4 matrix, ref float4x4 deltaMatrix, OPERATION op, ref MOVETYPE type, ref float[] snap)
        {
            if (!Intersects(op, OPERATION.TRANSLATE) || type != MOVETYPE.NONE)
            {
                return false;
            }
            var io = ImGui.GetIO();
            bool applyRotationLocaly = gContext.mMode == MODE.LOCAL || type == MOVETYPE.MOVE_SCREEN;
            bool modified = false;

            // move
            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsTranslateType(gContext.mCurrentOperation))
            {
                ImGui.CaptureMouseFromApp();
                var signedLength = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
                var len = MathF.Abs(signedLength); // near plan
                var newPos = gContext.mRayOrigin + gContext.mRayVector * len;

                // compute delta
                var newOrigin = newPos - gContext.mRelativeOrigin * gContext.mScreenFactor;
                var delta = newOrigin - gContext.mModel.Position();

                // 1 axis constraint
                if (gContext.mCurrentOperation >= (int)MOVETYPE.MOVE_X && gContext.mCurrentOperation <= (int)MOVETYPE.MOVE_Z)
                {
                    int axisIndex = gContext.mCurrentOperation - (int)MOVETYPE.MOVE_X;
                    var axisValue = float4.Zero;
                    switch (axisIndex)
                    {
                        case 0:
                            axisValue = gContext.mModel.Row1;
                            break;
                        case 1:
                            axisValue = gContext.mModel.Row2;
                            break;
                        case 2:
                            axisValue = gContext.mModel.Row3;
                            break;
                        case 3:
                            axisValue = gContext.mModel.Row4;
                            break;
                    }
                    float lengthOnAxis = float3.Dot(axisValue.xyz, delta.xyz);
                    delta = axisValue * lengthOnAxis;
                }

                // snap
                if (snap != null)
                {
                    var cumulativeDelta = gContext.mModel.Position() + delta - gContext.mMatrixOrigin;
                    if (applyRotationLocaly)
                    {
                        var modelSourceNormalized = gContext.mModelSource;
                        modelSourceNormalized.OrthoNormalize();
                        var modelSourceNormalizedInverse = modelSourceNormalized.Invert();
                        cumulativeDelta = modelSourceNormalizedInverse * cumulativeDelta;
                        ComputeSnap(ref cumulativeDelta, snap);
                        cumulativeDelta = modelSourceNormalized * cumulativeDelta;
                    }
                    else
                    {
                        ComputeSnap(ref cumulativeDelta, snap);
                    }
                    delta = gContext.mMatrixOrigin + cumulativeDelta - gContext.mModel.Position();

                }

                if (delta != gContext.mTranslationLastDelta)
                {
                    modified = true;
                }
                gContext.mTranslationLastDelta = delta;

                // compute matrix & delta
                var deltaMatrixTranslation = float4x4.Identity;
                deltaMatrixTranslation.Row4.xyz = delta.xyz;

                if (deltaMatrix != null)
                {
                    deltaMatrix = deltaMatrixTranslation;
                }

                var res = deltaMatrixTranslation * gContext.mModelSource;
                matrix = res;

                if (!io.MouseDown[0])
                {
                    gContext.mbUsing = false;
                }

                type = (MOVETYPE)gContext.mCurrentOperation;
            }
            else
            {
                // find new possible way to move
                var gizmoHitProportion = float3.Zero;
                type = GetMoveType(op, ref gizmoHitProportion);
                if (type != MOVETYPE.NONE)
                {
                    ImGui.CaptureMouseFromApp();
                }
                if (CanActivate() && type != MOVETYPE.NONE)
                {
                    gContext.mbUsing = true;
                    gContext.mEditingID = gContext.mActualID;
                    gContext.mCurrentOperation = (int)type;
                    var movePlanNormal = new float4[] { gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir(),
               gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir(),  -gContext.mCameraDir };

                    var cameraToModelNormalized = (gContext.mModel.Position().xyz - gContext.mCameraEye.xyz).Normalize();
                    for (var i = 0; i < 3; i++)
                    {
                        var orthoVector = float3.Cross(movePlanNormal[i].xyz, cameraToModelNormalized.xyz);
                        movePlanNormal[i] = new float4(float3.Cross(movePlanNormal[i].xyz, orthoVector), 0);
                        movePlanNormal[i].Normalize();
                    }
                    // pickup plan
                    gContext.mTranslationPlan = BuildPlan(gContext.mModel.Position(), movePlanNormal[type - MOVETYPE.MOVE_X]);
                    var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
                    gContext.mTranslationPlanOrigin = gContext.mRayOrigin + gContext.mRayVector * len;
                    gContext.mMatrixOrigin = gContext.mModel.Position();

                    gContext.mRelativeOrigin = (gContext.mTranslationPlanOrigin - gContext.mModel.Position()) * (1f / gContext.mScreenFactor);
                }
            }
            return modified;
        }

        public static bool HandleScale(ref float4x4 matrix, ref float4x4 deltaMatrix, OPERATION op, ref MOVETYPE type, ref float[] snap)
        {
            if ((!Intersects(op, OPERATION.SCALE) && !Intersects(op, OPERATION.SCALEU)) || type != MOVETYPE.NONE || !gContext.mbMouseOver)
            {
                return false;
            }
            var io = ImGui.GetIO();
            var modified = false;

            if (!gContext.mbUsing)
            {
                // find new possible way to scale
                type = GetScaleType(op);
                if (type != MOVETYPE.NONE)
                {
                    ImGui.CaptureMouseFromApp();
                }
                if (CanActivate() && type != MOVETYPE.NONE)
                {
                    gContext.mbUsing = true;
                    gContext.mEditingID = gContext.mActualID;
                    gContext.mCurrentOperation = (int)type;
                    var movePlanNormal = new float4[] { gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir(),
               gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir(),  -gContext.mCameraDir };
                    // pickup plan

                    gContext.mTranslationPlan = BuildPlan(gContext.mModel.Position(), movePlanNormal[type - MOVETYPE.SCALE_X]);
                    var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
                    gContext.mTranslationPlanOrigin = gContext.mRayOrigin + gContext.mRayVector * len;
                    gContext.mMatrixOrigin = gContext.mModel.Position();
                    gContext.mScale = float4.One;
                    gContext.mRelativeOrigin = (gContext.mTranslationPlanOrigin - gContext.mModel.Position()) * (1f / gContext.mScreenFactor);
                    gContext.mScaleValueOrigin = new float4(gContext.mModelSource.Right().Length, gContext.mModelSource.Up().Length, gContext.mModelSource.Dir().Length, 1);
                    gContext.mSaveMousePosx = io.MousePos.X;
                }
            }
            // scale
            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsScaleType((MOVETYPE)gContext.mCurrentOperation))
            {
                ImGui.CaptureMouseFromApp();
                var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
                var newPos = gContext.mRayOrigin + gContext.mRayVector * len;
                var newOrigin = newPos - gContext.mRelativeOrigin * gContext.mScreenFactor;
                var delta = newOrigin - gContext.mModelLocal.Position();

                // 1 axis constraint
                if (gContext.mCurrentOperation >= (int)MOVETYPE.SCALE_X && gContext.mCurrentOperation <= (int)MOVETYPE.SCALE_Z)
                {
                    int axisIndex = gContext.mCurrentOperation - (int)MOVETYPE.SCALE_X;

                    float4 axisValue = float4.Zero;

                    switch (axisIndex)
                    {
                        case 0:
                            axisValue = gContext.mModelLocal.Row1;
                            break;
                        case 1:
                            axisValue = gContext.mModelLocal.Row2;
                            break;
                        case 2:
                            axisValue = gContext.mModelLocal.Row3;
                            break;
                        case 3:
                            axisValue = gContext.mModelLocal.Row4;
                            break;
                    }

                    var lengthOnAxis = float3.Dot(axisValue.xyz, delta.xyz);
                    delta = axisValue * lengthOnAxis;

                    var baseVector = gContext.mTranslationPlanOrigin - gContext.mModelLocal.Position();
                    float ratio = float3.Dot(axisValue.xyz, baseVector.xyz + delta.xyz) / float3.Dot(axisValue.xyz, baseVector.xyz);

                    gContext.mScale[axisIndex] = M.Max(ratio, 0.001f);
                }
                else
                {
                    float scaleDelta = (io.MousePos.X - gContext.mSaveMousePosx) * 0.01f;
                    gContext.mScale = new float4(M.Max(1f + scaleDelta, 0.001f));
                }

                // snap
                if (snap != null)
                {
                    var scaleSnap = new float[] { snap[0], snap[0], snap[0], snap[0] };
                    ComputeSnap(ref gContext.mScale, scaleSnap);
                }

                // no 0 allowed
                for (int i = 0; i < 3; i++)
                    gContext.mScale[i] = M.Max(gContext.mScale[i], 0.001f);

                if (gContext.mScaleLast != gContext.mScale)
                {
                    modified = true;
                }
                gContext.mScaleLast = gContext.mScale;

                // compute matrix & delta
                var deltaMatrixScale = float4x4.CreateScale(gContext.mScale.xyz * gContext.mScaleValueOrigin.xyz);

                var res = gContext.mModelLocal * deltaMatrixScale;
                matrix = res;

                if (deltaMatrix != null)
                {
                    var deltaScale = gContext.mScale * gContext.mScaleValueOrigin;

                    var originalScaleDivider = float4.Zero;
                    originalScaleDivider.x = 1 / gContext.mModelScaleOrigin.x;
                    originalScaleDivider.y = 1 / gContext.mModelScaleOrigin.y;
                    originalScaleDivider.z = 1 / gContext.mModelScaleOrigin.z;

                    deltaScale = deltaScale * originalScaleDivider;

                    deltaMatrixScale = float4x4.CreateScale(deltaScale.xyz);
                    deltaMatrix = deltaMatrixScale;
                }

                if (!io.MouseDown[0])
                {
                    gContext.mbUsing = false;
                    gContext.mScale = float4.One;
                }

                type = (MOVETYPE)gContext.mCurrentOperation;
            }
            return modified;
        }

        public static bool HandleRotation(ref float4x4 matrix, ref float4x4 deltaMatrix, OPERATION op, ref MOVETYPE type, ref float[] snap)
        {
            if (!Intersects(op, OPERATION.ROTATE) || type != MOVETYPE.NONE || !gContext.mbMouseOver)
            {
                return false;
            }
            var io = ImGui.GetIO();
            bool applyRotationLocaly = gContext.mMode == MODE.LOCAL;
            bool modified = false;

            if (!gContext.mbUsing)
            {
                type = GetRotateType(op);

                if (type != MOVETYPE.NONE)
                {
                    ImGui.CaptureMouseFromApp();
                }

                if (type == MOVETYPE.ROTATE_SCREEN)
                {
                    applyRotationLocaly = true;
                }

                if (CanActivate() && type != MOVETYPE.NONE)
                {
                    gContext.mbUsing = true;
                    gContext.mEditingID = gContext.mActualID;
                    gContext.mCurrentOperation = (int)type;
                    var rotatePlanNormal = new float4[] { gContext.mModel.Right(), gContext.mModel.Up(), gContext.mModel.Dir(), -gContext.mCameraDir };
                    // pickup plan
                    if (applyRotationLocaly)
                    {
                        gContext.mTranslationPlan = BuildPlan(gContext.mModel.Position(), rotatePlanNormal[type - MOVETYPE.ROTATE_X]);
                    }
                    else
                    {
                        gContext.mTranslationPlan = BuildPlan(gContext.mModelSource.Position(), directionUnary[type - MOVETYPE.ROTATE_X]);
                    }

                    var len = IntersectRayPlane(gContext.mRayOrigin, gContext.mRayVector, gContext.mTranslationPlan);
                    var localPos = gContext.mRayOrigin + gContext.mRayVector * len - gContext.mModel.Position();
                    gContext.mRotationVectorSource = localPos.Normalize();
                    gContext.mRotationAngleOrigin = ComputeAngleOnPlan();
                }
            }

            // rotation
            if (gContext.mbUsing && (gContext.mActualID == -1 || gContext.mActualID == gContext.mEditingID) && IsRotateType((MOVETYPE)gContext.mCurrentOperation))
            {
                ImGui.CaptureMouseFromApp();
                gContext.mRotationAngle = ComputeAngleOnPlan();
                if (snap != null)
                {
                    float snapInRadian = M.DegreesToRadians(snap[0]);
                    ComputeSnap(ref gContext.mRotationAngle, snapInRadian);
                }

                var rotationAxisLocalSpace = gContext.mModelInverse * new float4(gContext.mTranslationPlan.x, gContext.mTranslationPlan.y, gContext.mTranslationPlan.z, 0f);
                rotationAxisLocalSpace = rotationAxisLocalSpace.Normalize();


                var deltaRotation = float4x4.CreateFromAxisAngle(rotationAxisLocalSpace.xyz, gContext.mRotationAngle - gContext.mRotationAngleOrigin);
                if (gContext.mRotationAngle != gContext.mRotationAngleOrigin)
                {
                    modified = true;
                }
                gContext.mRotationAngleOrigin = gContext.mRotationAngle;


                var scaleOrigin = float4x4.CreateScale(gContext.mModelScaleOrigin);

                if (applyRotationLocaly)
                {
                    matrix = gContext.mModelLocal * deltaRotation * scaleOrigin;
                }
                else
                {
                    var res = gContext.mModelSource;
                    res.Row4 = float4.Zero;

                    matrix = deltaRotation * res;
                    matrix.Row4 = gContext.mModelSource.Position();
                }

                if (deltaMatrix != null)
                {
                    deltaMatrix = gContext.mModel * deltaRotation * gContext.mModelInverse;
                }

                if (!io.MouseDown[0])
                {
                    gContext.mbUsing = false;
                    gContext.mEditingID = -1;
                }
                type = (MOVETYPE)gContext.mCurrentOperation;
            }
            return modified;
        }
    }
}


