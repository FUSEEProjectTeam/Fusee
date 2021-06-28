using ProtoBuf;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NET5_0_OR_GREATER
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
#endif

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a 4x4 Matrix typically used in ComputerGraphics algorithms.
    /// </summary>
    /// <remarks>
    /// <para>
    /// float4x4 objects represent matrices used in column-vector-notation, that means
    /// when used in common matrix-vector-multiplication scenarios, the vector should
    /// be multiplied from the right to the matrix (M * v). Concatenations of matrices
    /// from left to right represent a transformation where the rightmost matrix is applied
    /// first and the leftmost matrix is applied last, for example in (M3 * M2 * M1) * v the vector
    /// v is first transformed by M1, then by M2 and finally by M3. The translation part of a 4x4
    /// matrix used in homogeneous coordinate calculations can be found in the leftmost column
    /// (M14 - x-translation, M24 - y-translation, M34 - z-translation).
    /// </para>
    /// <para>
    /// Note that although float4x4 objects represent matrices in COLUMN-vector-NOTATION as
    /// found in math books, the objects' contents is physically stored in ROW-major-ORDER, meaning that
    /// in physical memory, a float4x4's components are stored contiguously in the following order: first row (M11, M12, M13, M14),
    /// then second row (M21, M22, M21, M24), and so on. When exchanging matrix contents with libraries like
    /// graphics engines (OpenGL, Direct3D), physics engines, file formats, etc. make sure to convert to and
    /// from the given Matrix layout of the API you are exchanging data with.
    /// </para>
    /// <para>
    /// float4x4 contains convenience construction methods to create matrices commonly
    /// used in Computer Graphics. Most of these application matrices are handedness-agnostic, meaning
    /// that the resulting matrices can be used in both, left-handed and right-handed coordinate systems.
    /// This does not hold for LookAt and Projection matrices where the viewing direction plays a role. In
    /// left-handed coordinate systems the viewing direction is positive, meaning positions further away have
    /// bigger positive z-coordinates whereas in right-handed coordinate systems positions further away have smaller
    /// negative z-coordinates. By default, float4x4 will assume a left-handed coordinate system, but contains
    /// convenience construction methods to also create right-handed matrices if necessary. The right-handed versions
    /// of methods are postfixed with "RH".
    /// </para>
    /// </remarks>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct float4x4 : IEquatable<float4x4>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        [ProtoMember(1)]
        public float4 Row0;

        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        [ProtoMember(2)]
        public float4 Row1;

        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        [ProtoMember(3)]
        public float4 Row2;

        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        [ProtoMember(4)]
        public float4 Row3;

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static float4x4 Identity = new float4x4(float4.UnitX, float4.UnitY, float4.UnitZ, float4.UnitW);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static float4x4 Zero = new float4x4(float4.Zero, float4.Zero, float4.Zero, float4.Zero);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public float4x4(float4 row0, float4 row1, float4 row2, float4 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m00">First item of the first row of the matrix.</param>
        /// <param name="m01">Second item of the first row of the matrix.</param>
        /// <param name="m02">Third item of the first row of the matrix.</param>
        /// <param name="m03">Fourth item of the first row of the matrix.</param>
        /// <param name="m10">First item of the second row of the matrix.</param>
        /// <param name="m11">Second item of the second row of the matrix.</param>
        /// <param name="m12">Third item of the second row of the matrix.</param>
        /// <param name="m13">Fourth item of the second row of the matrix.</param>
        /// <param name="m20">First item of the third row of the matrix.</param>
        /// <param name="m21">Second item of the third row of the matrix.</param>
        /// <param name="m22">Third item of the third row of the matrix.</param>
        /// <param name="m23">First item of the third row of the matrix.</param>
        /// <param name="m30">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m31">Second item of the fourth row of the matrix.</param>
        /// <param name="m32">Third item of the fourth row of the matrix.</param>
        /// <param name="m33">Fourth item of the fourth row of the matrix.</param>
        public float4x4(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            Row0 = new float4(m00, m01, m02, m03);
            Row1 = new float4(m10, m11, m12, m13);
            Row2 = new float4(m20, m21, m22, m23);
            Row3 = new float4(m30, m31, m32, m33);
        }

        /// <summary>
        /// Constructs a new float4x4 by converting from a double4x4.
        /// </summary>
        /// <param name="d4x4">The double4x4 to copy components from.</param>
        public float4x4(double4x4 d4x4)
        {
            Row0 = (float4)d4x4.Row0;
            Row1 = (float4)d4x4.Row1;
            Row2 = (float4)d4x4.Row2;
            Row3 = (float4)d4x4.Row3;
        }

        /// <summary>
        /// Constructs a new float4x4 from a float3x3 by setting the 4th column and row to UnitW respectively.
        /// </summary>
        /// <param name="f3x3">The float3x3 matrix to copy components from.</param>
        public float4x4(float3x3 f3x3)
        {
            Row0 = new float4(f3x3.Row0, 0);
            Row1 = new float4(f3x3.Row1, 0);
            Row2 = new float4(f3x3.Row2, 0);
            Row3 = new float4(0, 0, 0, 1);
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public float Determinant => Row0.x * Row1.y * Row2.z * Row3.w - Row0.x * Row1.y * Row2.w * Row3.z + Row0.x * Row1.z * Row2.w * Row3.y -
                    Row0.x * Row1.z * Row2.y * Row3.w
                    + Row0.x * Row1.w * Row2.y * Row3.z - Row0.x * Row1.w * Row2.z * Row3.y - Row0.y * Row1.z * Row2.w * Row3.x +
                    Row0.y * Row1.z * Row2.x * Row3.w
                    - Row0.y * Row1.w * Row2.x * Row3.z + Row0.y * Row1.w * Row2.z * Row3.x - Row0.y * Row1.x * Row2.z * Row3.w +
                    Row0.y * Row1.x * Row2.w * Row3.z
                    + Row0.z * Row1.w * Row2.x * Row3.y - Row0.z * Row1.w * Row2.y * Row3.x + Row0.z * Row1.x * Row2.y * Row3.w -
                    Row0.z * Row1.x * Row2.w * Row3.y
                    + Row0.z * Row1.y * Row2.w * Row3.x - Row0.z * Row1.y * Row2.x * Row3.w - Row0.w * Row1.x * Row2.y * Row3.z +
                    Row0.w * Row1.x * Row2.z * Row3.y
                    - Row0.w * Row1.y * Row2.z * Row3.x + Row0.w * Row1.y * Row2.x * Row3.z - Row0.w * Row1.z * Row2.x * Row3.y +
                    Row0.w * Row1.z * Row2.y * Row3.x;

        /// <summary>
        /// Returns the trace of this matrix
        /// </summary>
        public float Trace => Row0.x + Row1.y + Row2.z + Row3.w;

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public float4 Column0
        {
            get => new float4(Row0.x, Row1.x, Row2.x, Row3.x);
            set { Row0.x = value.x; Row1.x = value.y; Row2.x = value.z; Row3.x = value.w; }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public float4 Column1
        {
            get => new float4(Row0.y, Row1.y, Row2.y, Row3.y);
            set { Row0.y = value.x; Row1.y = value.y; Row2.y = value.z; Row3.y = value.w; }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public float4 Column2
        {
            get => new float4(Row0.z, Row1.z, Row2.z, Row3.z);
            set { Row0.z = value.x; Row1.z = value.y; Row2.z = value.z; Row3.z = value.w; }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public float4 Column3
        {
            get => new float4(Row0.w, Row1.w, Row2.w, Row3.w);
            set { Row0.w = value.x; Row1.w = value.y; Row2.w = value.z; Row3.w = value.w; }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11
        {
            get => Row0.x;
            set => Row0.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12
        {
            get => Row0.y;
            set => Row0.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13
        {
            get => Row0.z;
            set => Row0.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 4 of this instance.
        /// </summary>
        public float M14
        {
            get => Row0.w;
            set => Row0.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21
        {
            get => Row1.x;
            set => Row1.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22
        {
            get => Row1.y;
            set => Row1.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23
        {
            get => Row1.z;
            set => Row1.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 4 of this instance.
        /// </summary>
        public float M24
        {
            get => Row1.w;
            set => Row1.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31
        {
            get => Row2.x;
            set => Row2.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32
        {
            get => Row2.y;
            set => Row2.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33
        {
            get => Row2.z;
            set => Row2.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 4 of this instance.
        /// </summary>
        public float M34
        {
            get => Row2.w;
            set => Row2.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 1 of this instance.
        /// </summary>
        public float M41
        {
            get => Row3.x;
            set => Row3.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 2 of this instance.
        /// </summary>
        public float M42
        {
            get => Row3.y;
            set => Row3.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 3 of this instance.
        /// </summary>
        public float M43
        {
            get => Row3.z;
            set => Row3.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 4 of this instance.
        /// </summary>
        public float M44
        {
            get => Row3.w;
            set => Row3.w = value;
        }

        /// <summary>
        /// Gets the offset part of the matrix as a <see cref="float3"/> instance.
        /// </summary>
        /// <remarks>
        /// The offset part of the matrix consists of the M14, M24 and M34 components (in row major order notation).
        /// </remarks>
        public float3 Offset => GetTranslation(this);

        #endregion Properties

        #region Instance

        #region this

        /// <summary>
        ///     Sets/Gets value from given index
        /// </summary>
        /// <param name="i">The ROW index</param>
        /// <param name="j">The COLUMN index</param>
        /// <returns></returns>
        public float this[int i, int j]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return Row0[j];

                    case 1:
                        return Row1[j];

                    case 2:
                        return Row2[j];

                    case 3:
                        return Row3[j];

                    default:
                        throw new ArgumentOutOfRangeException($"Index {i},{j} not eligible for a float4x4 type");
                }
            }
            set
            {
                switch (i)
                {
                    case 0:
                        Row0[j] = value;
                        break;

                    case 1:
                        Row1[j] = value;
                        break;

                    case 2:
                        Row2[j] = value;
                        break;

                    case 3:
                        Row3[j] = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Index {i},{j} not eligible for a float4x4 type");
                }
            }
        }

        #endregion this

        #region public Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public float4x4 Invert()
        {
            return Invert(this);
        }

        #endregion public Invert()

        #region public Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public float4x4 Transpose()
        {
            return Transpose(this);
        }

        #region float[] ToArray()

        /// <summary>
        /// Returns the matrix as float array.
        /// </summary>
        /// <returns></returns>
        public float[] ToArray()
        {
            return new[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        #endregion float[] ToArray()

        #endregion public Transpose()

        #region public Round()

        /// <summary>
        /// Rounds this instance to 6 digits (max float precision).
        /// </summary>
        public float4x4 Round()
        {
            return Round(this);
        }

        #endregion public Round()

        #region TRS Decomposition

        /// <summary>
        /// The translation component of this matrix.
        /// </summary>
        public float4x4 TranslationComponent()
        {
            return TranslationDecomposition(this);
        }

        /// <summary>
        /// The translation of this matrix.
        /// </summary>
        public float3 Translation()
        {
            return GetTranslation(this);
        }

        /// <summary>
        /// The rotation component of this matrix.
        /// </summary>
        public float4x4 RotationComponent()
        {
            return RotationDecomposition(this);
        }

        /// <summary>
        /// The scale component of this matrix.
        /// </summary>
        public float4x4 ScaleComponent()
        {
            return ScaleDecomposition(this);
        }

        /// <summary>
        /// The scale factors of this matrix.
        /// </summary>
        public float3 Scale()
        {
            return GetScale(this);
        }

        #endregion TRS Decomposition

        #endregion Instance

        #region Static

        #region CreateFromAxisAngle

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static float4x4 CreateFromAxisAngle(float3 axis, float angle)
        {
            var cos = (float)System.Math.Cos(-angle);
            var sin = (float)System.Math.Sin(-angle);
            var t = 1.0f - cos;

            axis = axis.Normalize();

            var result = new float4x4(t * axis.x * axis.x + cos, t * axis.x * axis.y + sin * axis.z, t * axis.x * axis.z - sin * axis.y, 0.0f,
                                  t * axis.x * axis.y - sin * axis.z, t * axis.y * axis.y + cos, t * axis.y * axis.z + sin * axis.x, 0.0f,
                                  t * axis.x * axis.z + sin * axis.y, t * axis.y * axis.z - sin * axis.x, t * axis.z * axis.z + cos, 0.0f,
                                  0.0f, 0.0f, 0.0f, 1.0f);

            return result;
        }

        #endregion CreateFromAxisAngle

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationX(float angle)
        {
            float4x4 result = new float4x4();

            var cos = (float)System.Math.Cos(angle);
            var sin = (float)System.Math.Sin(angle);

            result.Row0 = float4.UnitX;
            result.Row1 = new float4(0.0f, cos, -sin, 0.0f);
            result.Row2 = new float4(0.0f, sin, cos, 0.0f);
            result.Row3 = float4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationY(float angle)
        {
            float4x4 result = new float4x4();

            var cos = (float)System.Math.Cos(angle);
            var sin = (float)System.Math.Sin(angle);

            result.Row0 = new float4(cos, 0.0f, sin, 0.0f);
            result.Row1 = float4.UnitY;
            result.Row2 = new float4(-sin, 0.0f, cos, 0.0f);
            result.Row3 = float4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationZ(float angle)
        {
            float4x4 result = new float4x4();

            var cos = (float)System.Math.Cos(angle);
            var sin = (float)System.Math.Sin(angle);

            result.Row0 = new float4(cos, -sin, 0.0f, 0.0f);
            result.Row1 = new float4(sin, cos, 0.0f, 0.0f);
            result.Row2 = float4.UnitZ;
            result.Row3 = float4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xy">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationXY(float2 xy)
        {
            return CreateRotationXY(xy.x, xy.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="x">counter-clockwise angles in radians.</param>
        /// <param name="y">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationXY(float x, float y)
        {
            return CreateRotationY(y) * CreateRotationX(x);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and z-axis.
        /// </summary>
        /// <param name="yz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZY(float2 yz)
        {
            return CreateRotationZY(yz.x, yz.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="y">counter-clockwise angles in radians.</param>
        /// <param name="z">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZY(float y, float z)
        {
            return CreateRotationY(y) * CreateRotationZ(z);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZX(float2 xz)
        {
            return CreateRotationZX(xz.x, xz.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="x">counter-clockwise angles in radians.</param>
        /// <param name="z">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZX(float x, float z)
        {
            return CreateRotationX(x) * CreateRotationZ(z);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xyz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZXY(float3 xyz)
        {
            return CreateRotationZXY(xyz.x, xyz.y, xyz.z);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="x">counter-clockwise angles in radians.</param>
        /// <param name="y">counter-clockwise angles in radians.</param>
        /// <param name="z">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static float4x4 CreateRotationZXY(float x, float y, float z)
        {
            return CreateRotationY(y) * CreateRotationX(x) * CreateRotationZ(z);
        }

        #endregion CreateRotation[XYZ]

        #region CreateTranslation

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateTranslation(float x, float y, float z)
        {
            float4x4 result = Identity;

            result.M14 = x;
            result.M24 = y;
            result.M34 = z;

            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateTranslation(float3 vector)
        {
            float4x4 result = Identity;

            result.M14 = vector.x;
            result.M24 = vector.y;
            result.M34 = vector.z;

            return result;
        }

        #endregion CreateTranslation

        #region Rotation matrix to euler representation

        //see Blender mathutils and Graphic Gems IV p. 222-229
        private static void RotMatToEuler2(float[][] mat, ref float[] eul1, ref float[] eul2)
        {
            var axis = new float3(1, 0, 2); //for rotation order YXZ, see Blender mathutils and Graphic Gems IV for other configurations
            var parity = 1; //parity of axis permutation (even=0, odd=1) - 'n' in original code

            int i = (int)axis.x, j = (int)axis.y, k = (int)axis.z;
            var cy = System.Math.Sqrt(System.Math.Pow(mat[i][i], 2.0) + System.Math.Pow(mat[i][j], 2.0));

            var FLT_EPSILON = 1.192092896e-07F;

            if (cy > 16.0f * FLT_EPSILON)
            {
                eul1[i] = (float)System.Math.Atan2(mat[j][k], mat[k][k]);
                eul1[j] = (float)System.Math.Atan2(-mat[i][k], cy);
                eul1[k] = (float)System.Math.Atan2(mat[i][j], mat[i][i]);

                eul2[i] = (float)System.Math.Atan2(-mat[j][k], -mat[k][k]);
                eul2[j] = (float)System.Math.Atan2(-mat[i][k], -cy);
                eul2[k] = (float)System.Math.Atan2(-mat[i][j], -mat[i][i]);
            }
            else
            {
                eul1[i] = (float)System.Math.Atan2(-mat[k][j], mat[j][j]);
                eul1[j] = (float)System.Math.Atan2(-mat[i][k], cy);
                eul1[k] = 0;

                eul2[i] = (float)System.Math.Atan2(-mat[k][j], mat[j][j]);
                eul2[j] = (float)System.Math.Atan2(-mat[i][k], cy);
                eul2[k] = 0;
            }

            if (parity == 1)
            {
                for (var l = 0; l < eul1.Length; l++)
                {
                    eul1[l] *= -1;
                    eul1[l] *= -1;
                }
            }
        }

        //see Blender mathutils and Graphic Gems IV p. 222-229
        /// <summary>
        /// Returns the euler angles from a given rotation matrix.
        /// </summary>
        /// <param name="rotMat">The roation matrix.</param>
        public static float3 RotMatToEuler(float4x4 rotMat)
        {
            //Matrix is being handled as a multi-dimensional array to ensure that the rotation order can be changed easily in the future.
            var m = new[] { rotMat.Row0.ToArray(), rotMat.Row1.ToArray(), rotMat.Row2.ToArray(), rotMat.Row3.ToArray() };

            var eul1 = new float[3];
            var eul2 = new float[3];
            float d1, d2;

            RotMatToEuler2(m, ref eul1, ref eul2);

            d1 = System.Math.Abs(eul1[0]) + System.Math.Abs(eul1[1]) + System.Math.Abs(eul1[2]);
            d2 = System.Math.Abs(eul2[0]) + System.Math.Abs(eul2[1]) + System.Math.Abs(eul2[2]);

            /* return best, which is just the one with lowest values it in */
            return d1 > d2 ? new float3(eul2[0], eul2[1], eul2[2]) : new float3(eul1[0], eul1[1], eul1[2]);
        }

        #endregion Rotation matrix to euler representation

        #region CreateScale

        /// <summary>
        /// Creates a uniform scale matrix with the same scale value along all three dimensions.
        /// </summary>
        /// <param name="scale">The value to scale about x, y, and z.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateScale(float scale)
        {
            float4x4 result = Identity;

            result.M11 = scale;
            result.M22 = scale;
            result.M33 = scale;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="x">X scale.</param>
        /// <param name="y">Y scale.</param>
        /// <param name="z">Z scale.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateScale(float x, float y, float z)
        {
            float4x4 result = Identity;

            result.M11 = x;
            result.M22 = y;
            result.M33 = z;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="vector">The scale vector.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateScale(float3 vector)
        {
            float4x4 result = Identity;

            result.M11 = vector.x;
            result.M22 = vector.y;
            result.M33 = vector.z;

            return result;
        }

        #endregion CreateScale

        #region CreateOrthographic

        /// <summary>
        /// Creates a left handed orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateOrthographic(float width, float height, float zNear, float zFar)
        {
            return CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar);
        }

        #endregion CreateOrthographic

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates a right handed orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateOrthographicOffCenterRH(float left, float right, float bottom, float top, float zNear,
                                                         float zFar)
        {
            float4x4 result = new float4x4();

            float invRL = 1 / (right - left);
            float invTB = 1 / (top - bottom);
            float invFN = 1 / (zFar - zNear);

            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = -2 * invFN;

            result.M14 = -(right + left) * invRL;
            result.M24 = -(top + bottom) * invTB;
            result.M34 = -(zFar + zNear) * invFN;
            result.M44 = 1;

            return result;
        }

        /// <summary>
        /// Creates a left handed orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear,
                                                           float zFar)
        {
            float4x4 result = new float4x4();

            float invRL = 1 / (right - left);
            float invTB = 1 / (top - bottom);
            float invFN = 1 / (zFar - zNear);

            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = 2 * invFN;

            result.M14 = -(right + left) * invRL;
            result.M24 = -(top + bottom) * invTB;
            result.M34 = -(zFar + zNear) * invFN;
            result.M44 = 1;

            return result;
        }

        #endregion CreateOrthographicOffCenter

        #region CreatePerspectiveFieldOfView

        /// <summary>
        /// Creates a left handed perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static float4x4 CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar)
        {
            float4x4 result;

            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float yMax = zNear * (float)System.Math.Tan(0.5f * fovy);
            float yMin = -yMax;
            float xMin = yMin * aspect;
            float xMax = yMax * aspect;

            result = CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar);

            return result;
        }

        #endregion CreatePerspectiveFieldOfView

        #region CreatePerspectiveOffCenter

        /// <summary>
        /// Creates a right handed perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A right handed projection matrix that transforms camera space to raster space</returns>
        /// <remarks>Generates a matrix mapping a frustum shaped volume (the viewing frustum) to
        /// the unit cube (ranging from -1 to 1 in each dimension, also in z). The sign of the z-value will be
        /// flipped for vectors multiplied with this matrix. Given that the underlying rendering platform
        /// interprets z-values returned by the vertex shader to be in left-handed coordinates, where increasing
        /// z-values indicate locations further away from the view point (as BOTH, Direct3D AND OpenGL do), this
        /// type of matrix is widely called to be a "right handed" projection matrix as it assumes a right-handed
        /// camera coordinate system.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static float4x4 CreatePerspectiveOffCenterRH(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            float4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float x = (2.0f * zNear) / (right - left);
            float y = (2.0f * zNear) / (top - bottom);
            // Right handed
            float a = (right + left) / (right - left);
            float b = (top + bottom) / (top - bottom);
            float c = -(zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            result = new float4x4(x, 0, a, 0,
                                  0, y, b, 0,
                                  0, 0, c, d,
                                  0, 0, -1, 0);

            return result;
        }

        /// <summary>
        /// Creates an left handed perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static float4x4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            float4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float x = (2.0f * zNear) / (right - left);
            float y = (2.0f * zNear) / (top - bottom);
            // Left Handed
            float a = (left + right) / (left - right);
            float b = (top + bottom) / (bottom - top);
            float c = (zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            result = new float4x4(x, 0, a, 0,
                                  0, y, b, 0,
                                  0, 0, c, d,
                                  0, 0, 1, 0);

            return result;
        }

        #endregion CreatePerspectiveOffCenter

        #region Scale Functions

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="scale">Single scale factor for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static float4x4 Scale(float scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="scale">Scale factors for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static float4x4 Scale(float3 scale)
        {
            return Scale(scale.x, scale.y, scale.z);
        }

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>A scaling matrix</returns>
        public static float4x4 Scale(float x, float y, float z)
        {
            float4x4 result;
            result.Row0 = float4.UnitX * x;
            result.Row1 = float4.UnitY * y;
            result.Row2 = float4.UnitZ * z;
            result.Row3 = float4.UnitW;
            return result;
        }

        #endregion Scale Functions

        #region Camera Helper Functions

        /// <summary>
        /// Build a left handed world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4 that transforms world space to camera space</returns>
        public static float4x4 LookAt(float3 eye, float3 target, float3 up)
        {
            var z = float3.Normalize(target - eye);
            var x = float3.Normalize(float3.Cross(up, z));
            var y = float3.Cross(z, x);

            // Row order notation
            //return new float4x4(new float4(x.x, y.x, z.x, 0),
            //                    new float4(x.y, y.y, z.y, 0),
            //                    new float4(x.z, y.z, z.z, 0),
            //                    new float4(-float3.Dot(x, eye), -float3.Dot(y, eye), -float3.Dot(z, eye), 1));

            // Column order notation
            return new float4x4(x.x, x.y, x.z, -float3.Dot(x, eye),
                                y.x, y.y, y.z, -float3.Dot(y, eye),
                                z.x, z.y, z.z, -float3.Dot(z, eye),
                                0, 0, 0, 1);
        }

        /// <summary>
        /// Build a right handed world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A float4x4 that transforms world space to camera space</returns>
        public static float4x4 LookAtRH(float3 eye, float3 target, float3 up)
        {
            var z = float3.Normalize(eye - target);
            var x = float3.Normalize(float3.Cross(up, z));
            var y = float3.Cross(z, x);

            // Row order notation
            //return new float4x4(new float4(x.x, y.x, z.x, 0),
            //                    new float4(x.y, y.y, z.y, 0),
            //                    new float4(x.z, y.z, z.z, 0),
            //                    new float4(-float3.Dot(x, eye), -float3.Dot(y, eye), -float3.Dot(z, eye), 1));

            // Column order notation
            return new float4x4(x.x, x.y, x.z, -float3.Dot(x, eye),
                                y.x, y.y, y.z, -float3.Dot(y, eye),
                                z.x, z.y, z.z, -float3.Dot(z, eye),
                                0, 0, 0, 1);
        }

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eyeX">Eye (camera) position in world space</param>
        /// <param name="eyeY">Eye (camera) position in world space</param>
        /// <param name="eyeZ">Eye (camera) position in world space</param>
        /// <param name="targetX">Target position in world space</param>
        /// <param name="targetY">Target position in world space</param>
        /// <param name="targetZ">Target position in world space</param>
        /// <param name="upX">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upY">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <param name="upZ">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A float4x4 that transforms world space to camera space</returns>
        public static float4x4 LookAt(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ,
                                      float upX, float upY, float upZ)
        {
            return LookAt(new float3(eyeX, eyeY, eyeZ), new float3(targetX, targetY, targetZ), new float3(upX, upY, upZ));
        }

        #endregion Camera Helper Functions

        #region Elementary Arithmetic Functions

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The left operand of the addition.</param>
        /// <param name="right">The right operand of the addition.</param>
        /// <returns>A new instance that is the result of the addition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Add(in float4x4 left, in float4x4 right)
        {
#if NET5_0_OR_GREATER
            float4x4 result;

            if (Sse.IsSupported)
            {
                AddSse(in left, in right, out result);
            }
            else
            {
                Add(in left, in right, out result);
            }

            return result;
#else
            Add(in left, in right, out float4x4 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        private static unsafe void AddSse(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            Vector128<float> leftrow0;
            Vector128<float> leftrow1;
            Vector128<float> leftrow2;
            Vector128<float> leftrow3;

            fixed (float* m = &left.Row0.x)
            {
                leftrow0 = Sse.LoadVector128(m + 0);
                leftrow1 = Sse.LoadVector128(m + 4);
                leftrow2 = Sse.LoadVector128(m + 8);
                leftrow3 = Sse.LoadVector128(m + 12);
            }

            Vector128<float> rightrow0;
            Vector128<float> rightrow1;
            Vector128<float> rightrow2;
            Vector128<float> rightrow3;

            fixed (float* m = &right.Row0.x)
            {
                rightrow0 = Sse.LoadVector128(m + 0);
                rightrow1 = Sse.LoadVector128(m + 4);
                rightrow2 = Sse.LoadVector128(m + 8);
                rightrow3 = Sse.LoadVector128(m + 12);
            }

            var resultrow0 = Sse.Add(leftrow0, rightrow0);
            var resultrow1 = Sse.Add(leftrow1, rightrow1);
            var resultrow2 = Sse.Add(leftrow2, rightrow2);
            var resultrow3 = Sse.Add(leftrow3, rightrow3);

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.Row0.x)
            {
                Sse.Store(r + 0, resultrow0);
                Sse.Store(r + 4, resultrow1);
                Sse.Store(r + 8, resultrow2);
                Sse.Store(r + 12, resultrow3);
            }
        }
#endif

        private static void Add(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            result.Row0 = left.Row0 + right.Row0;
            result.Row1 = left.Row1 + right.Row1;
            result.Row2 = left.Row2 + right.Row2;
            result.Row3 = left.Row3 + right.Row3;
        }

        /// <summary>
        /// Subtracts the right instance from the left instance.
        /// </summary>
        /// <param name="left">The left operand of the subtraction.</param>
        /// <param name="right">The right operand of the subtraction.</param>
        /// <returns>A new instance that is the result of the subtraction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Subtract(in float4x4 left, in float4x4 right)
        {
#if NET5_0_OR_GREATER
            float4x4 result;

            if (Sse.IsSupported)
            {
                SubtractSse(in left, in right, out result);
            }
            else
            {
                Subtract(in left, in right, out result);
            }

            return result;
#else
            Subtract(in left, in right, out float4x4 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        private static unsafe void SubtractSse(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            Vector128<float> leftrow0;
            Vector128<float> leftrow1;
            Vector128<float> leftrow2;
            Vector128<float> leftrow3;

            fixed (float* m = &left.Row0.x)
            {
                leftrow0 = Sse.LoadVector128(m + 0);
                leftrow1 = Sse.LoadVector128(m + 4);
                leftrow2 = Sse.LoadVector128(m + 8);
                leftrow3 = Sse.LoadVector128(m + 12);
            }

            Vector128<float> rightrow0;
            Vector128<float> rightrow1;
            Vector128<float> rightrow2;
            Vector128<float> rightrow3;

            fixed (float* m = &right.Row0.x)
            {
                rightrow0 = Sse.LoadVector128(m + 0);
                rightrow1 = Sse.LoadVector128(m + 4);
                rightrow2 = Sse.LoadVector128(m + 8);
                rightrow3 = Sse.LoadVector128(m + 12);
            }

            var resultrow0 = Sse.Subtract(leftrow0, rightrow0);
            var resultrow1 = Sse.Subtract(leftrow1, rightrow1);
            var resultrow2 = Sse.Subtract(leftrow2, rightrow2);
            var resultrow3 = Sse.Subtract(leftrow3, rightrow3);

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.Row0.x)
            {
                Sse.Store(r + 0, resultrow0);
                Sse.Store(r + 4, resultrow1);
                Sse.Store(r + 8, resultrow2);
                Sse.Store(r + 12, resultrow3);
            }
        }
#endif

        private static void Subtract(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            result.Row0 = left.Row0 - right.Row0;
            result.Row1 = left.Row1 - right.Row1;
            result.Row2 = left.Row2 - right.Row2;
            result.Row3 = left.Row3 - right.Row3;
        }

        #endregion Elementary Arithmetic Functions

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static float4x4 Mult(in float4x4 left, in float4x4 right)
        {
#if NET5_0_OR_GREATER
            float4x4 result;

            if (Sse.IsSupported)
            {
                MultSse(in left, in right, out result);
            }
            else
            {
                Mult(in left, in right, out result);
            }

            return result;
#else
            Mult(in left, in right, out float4x4 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        private static unsafe void MultSse(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            Vector128<float> leftrow0;
            Vector128<float> leftrow1;
            Vector128<float> leftrow2;
            Vector128<float> leftrow3;

            fixed (float* m = &left.Row0.x)
            {
                leftrow0 = Sse.LoadVector128(m + 0);
                leftrow1 = Sse.LoadVector128(m + 4);
                leftrow2 = Sse.LoadVector128(m + 8);
                leftrow3 = Sse.LoadVector128(m + 12);
            }

            Vector128<float> rightrow0;
            Vector128<float> rightrow1;
            Vector128<float> rightrow2;
            Vector128<float> rightrow3;

            fixed (float* m = &right.Row0.x)
            {
                rightrow0 = Sse.LoadVector128(m + 0);
                rightrow1 = Sse.LoadVector128(m + 4);
                rightrow2 = Sse.LoadVector128(m + 8);
                rightrow3 = Sse.LoadVector128(m + 12);
            }

            var resultrow0 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0x00), rightrow0),
                                             Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0x55), rightrow1)),
                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0xAA), rightrow2),
                                             Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0xFF), rightrow3)));

            var resultrow1 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0x00), rightrow0),
                                             Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0x55), rightrow1)),
                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0xAA), rightrow2),
                                             Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0xFF), rightrow3)));

            var resultrow2 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0x00), rightrow0),
                                             Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0x55), rightrow1)),
                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0xAA), rightrow2),
                                             Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0xFF), rightrow3)));

            var resultrow3 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0x00), rightrow0),
                                             Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0x55), rightrow1)),
                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0xAA), rightrow2),
                                             Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0xFF), rightrow3)));

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.Row0.x)
            {
                Sse.Store(r + 0, resultrow0);
                Sse.Store(r + 4, resultrow1);
                Sse.Store(r + 8, resultrow2);
                Sse.Store(r + 12, resultrow3);
            }
        }
#endif

        private static void Mult(in float4x4 left, in float4x4 right, out float4x4 result)
        {
            float leftM11 = left.Row0.x;
            float leftM12 = left.Row0.y;
            float leftM13 = left.Row0.z;
            float leftM14 = left.Row0.w;
            float leftM21 = left.Row1.x;
            float leftM22 = left.Row1.y;
            float leftM23 = left.Row1.z;
            float leftM24 = left.Row1.w;
            float leftM31 = left.Row2.x;
            float leftM32 = left.Row2.y;
            float leftM33 = left.Row2.z;
            float leftM34 = left.Row2.w;
            float leftM41 = left.Row3.x;
            float leftM42 = left.Row3.y;
            float leftM43 = left.Row3.z;
            float leftM44 = left.Row3.w;
            float rightM11 = right.Row0.x;
            float rightM12 = right.Row0.y;
            float rightM13 = right.Row0.z;
            float rightM14 = right.Row0.w;
            float rightM21 = right.Row1.x;
            float rightM22 = right.Row1.y;
            float rightM23 = right.Row1.z;
            float rightM24 = right.Row1.w;
            float rightM31 = right.Row2.x;
            float rightM32 = right.Row2.y;
            float rightM33 = right.Row2.z;
            float rightM34 = right.Row2.w;
            float rightM41 = right.Row3.x;
            float rightM42 = right.Row3.y;
            float rightM43 = right.Row3.z;
            float rightM44 = right.Row3.w;

            result.Row0.x = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31) + (leftM14 * rightM41);
            result.Row0.y = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32) + (leftM14 * rightM42);
            result.Row0.z = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33) + (leftM14 * rightM43);
            result.Row0.w = (leftM11 * rightM14) + (leftM12 * rightM24) + (leftM13 * rightM34) + (leftM14 * rightM44);
            result.Row1.x = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31) + (leftM24 * rightM41);
            result.Row1.y = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32) + (leftM24 * rightM42);
            result.Row1.z = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33) + (leftM24 * rightM43);
            result.Row1.w = (leftM21 * rightM14) + (leftM22 * rightM24) + (leftM23 * rightM34) + (leftM24 * rightM44);
            result.Row2.x = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31) + (leftM34 * rightM41);
            result.Row2.y = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32) + (leftM34 * rightM42);
            result.Row2.z = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33) + (leftM34 * rightM43);
            result.Row2.w = (leftM31 * rightM14) + (leftM32 * rightM24) + (leftM33 * rightM34) + (leftM34 * rightM44);
            result.Row3.x = (leftM41 * rightM11) + (leftM42 * rightM21) + (leftM43 * rightM31) + (leftM44 * rightM41);
            result.Row3.y = (leftM41 * rightM12) + (leftM42 * rightM22) + (leftM43 * rightM32) + (leftM44 * rightM42);
            result.Row3.z = (leftM41 * rightM13) + (leftM42 * rightM23) + (leftM43 * rightM33) + (leftM44 * rightM43);
            result.Row3.w = (leftM41 * rightM14) + (leftM42 * rightM24) + (leftM43 * rightM34) + (leftM44 * rightM44);
        }

        #endregion Multiply Functions

        #region Invert Functions

        /// <summary>
        /// Checks if this matrix is invertible.
        /// </summary>
        /// <param name="mat">The matrix.</param>       
        public static bool IsInvertable(float4x4 mat)
        {
            return mat.Determinant != 0f;
        }

        /// <summary>
        /// Checks if this matrix is invertible.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="det">The determinant of the matrix.</param>       
        public static bool IsInvertable(float4x4 mat, out float det)
        {
            det = mat.Determinant;
            return det != 0f;
        }

        /// <summary>
        /// Calculate the inverse of the given matrix.
        /// If you are unsure whether the matrix is invertible, check it with IsInvertable() first.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix.</returns>
        public static float4x4 Invert(in float4x4 matrix)
        {
#if NET5_0_OR_GREATER
            float4x4 result;

            if (Sse3.IsSupported)
            {
                InvertSse3(in matrix, out result);
            }
            else
            {
                Invert(in matrix, out result);
            }

            return result;
#else
            Invert(in matrix, out float4x4 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void InvertSse3(in float4x4 matrix, out float4x4 result)
        {
            // Original derivation and implementation can be found here:
            // https://lxjk.github.io/2017/09/03/Fast-4x4-Matrix-Inverse-with-SSE-SIMD-Explained.html
            // found via OpenTK

            Vector128<float> row0;
            Vector128<float> row1;
            Vector128<float> row2;
            Vector128<float> row3;

            fixed (float* m = &matrix.Row0.x)
            {
                row0 = Sse.LoadVector128(m);
                row1 = Sse.LoadVector128(m + 4);
                row2 = Sse.LoadVector128(m + 8);
                row3 = Sse.LoadVector128(m + 12);
            }

            var A = Sse.MoveLowToHigh(row0, row1);
            var B = Sse.MoveHighToLow(row1, row0);
            var C = Sse.MoveLowToHigh(row2, row3);
            var D = Sse.MoveHighToLow(row3, row2);

            const byte Shuffle_0202 = 0b1000_1000;
            const byte Shuffle_1313 = 0b1101_1101;

            var detSub = Sse.Subtract(
                Sse.Multiply(
                    Sse.Shuffle(row0, row2, Shuffle_0202),
                    Sse.Shuffle(row1, row3, Shuffle_1313)),
                Sse.Multiply(
                    Sse.Shuffle(row0, row2, Shuffle_1313),
                    Sse.Shuffle(row1, row3, Shuffle_0202)));

            const byte Shuffle_0000 = 0b0000_0000;
            const byte Shuffle_1111 = 0b0101_0101;
            const byte Shuffle_2222 = 0b1010_1010;
            const byte Shuffle_3333 = 0b1111_1111;

            var detA = Sse2.Shuffle(detSub.AsInt32(), Shuffle_0000).AsSingle();
            var detB = Sse2.Shuffle(detSub.AsInt32(), Shuffle_1111).AsSingle();
            var detC = Sse2.Shuffle(detSub.AsInt32(), Shuffle_2222).AsSingle();
            var detD = Sse2.Shuffle(detSub.AsInt32(), Shuffle_3333).AsSingle();

            const byte Shuffle_3300 = 0b0000_1111;
            const byte Shuffle_1122 = 0b1010_0101;
            const byte Shuffle_2301 = 0b0100_1110;

            var D_C = Sse.Subtract(
                Sse.Multiply(Sse2.Shuffle(D.AsInt32(), Shuffle_3300).AsSingle(), C),
                Sse.Multiply(
                    Sse2.Shuffle(D.AsInt32(), Shuffle_1122).AsSingle(),
                    Sse2.Shuffle(C.AsInt32(), Shuffle_2301).AsSingle()));

            var A_B = Sse.Subtract(
                Sse.Multiply(Sse2.Shuffle(A.AsInt32(), Shuffle_3300).AsSingle(), B),
                Sse.Multiply(
                    Sse2.Shuffle(A.AsInt32(), Shuffle_1122).AsSingle(),
                    Sse2.Shuffle(B.AsInt32(), Shuffle_2301).AsSingle()));

            const byte Shuffle_0303 = 0b1100_1100;
            const byte Shuffle_1032 = 0b1011_0001;
            const byte Shuffle_2121 = 0b0110_0110;

            var X_ = Sse.Subtract(
                Sse.Multiply(detD, A),
                Sse.Add(
                    Sse.Multiply(B, Sse2.Shuffle(D_C.AsInt32(), Shuffle_0303).AsSingle()),
                    Sse.Multiply(
                        Sse2.Shuffle(B.AsInt32(), Shuffle_1032).AsSingle(),
                        Sse2.Shuffle(D_C.AsInt32(), Shuffle_2121).AsSingle())));

            var W_ = Sse.Subtract(
                Sse.Multiply(detA, D),
                Sse.Add(
                    Sse.Multiply(C, Sse2.Shuffle(A_B.AsInt32(), Shuffle_0303).AsSingle()),
                    Sse.Multiply(
                        Sse2.Shuffle(C.AsInt32(), Shuffle_1032).AsSingle(),
                        Sse2.Shuffle(A_B.AsInt32(), Shuffle_2121).AsSingle())));

            var detM = Sse.Multiply(detA, detD);

            const byte Shuffle_3030 = 0b0011_0011;

            var Y_ = Sse.Subtract(
                Sse.Multiply(detB, C),
                Sse.Subtract(
                    Sse.Multiply(D, Sse2.Shuffle(A_B.AsInt32(), Shuffle_3030).AsSingle()),
                    Sse.Multiply(
                        Sse2.Shuffle(D.AsInt32(), Shuffle_1032).AsSingle(),
                        Sse2.Shuffle(A_B.AsInt32(), Shuffle_2121).AsSingle())));

            var Z_ = Sse.Subtract(
                Sse.Multiply(detC, B),
                Sse.Subtract(
                    Sse.Multiply(A, Sse2.Shuffle(D_C.AsInt32(), Shuffle_3030).AsSingle()),
                    Sse.Multiply(
                        Sse2.Shuffle(A.AsInt32(), Shuffle_1032).AsSingle(),
                        Sse2.Shuffle(D_C.AsInt32(), Shuffle_2121).AsSingle())));

            detM = Sse.Add(detM, Sse.Multiply(detB, detC));

            const byte Shuffle_0213 = 0b1101_1000;

            var tr = Sse.Multiply(A_B, Sse2.Shuffle(D_C.AsInt32(), Shuffle_0213).AsSingle());
            tr = Sse3.HorizontalAdd(tr, tr);
            tr = Sse3.HorizontalAdd(tr, tr);

            detM = Sse.Subtract(detM, tr);

            if (MathF.Abs(detM.GetElement(0)) < float.Epsilon)
            {
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            }

            var adjSignMask = Vector128.Create(1.0f, -1.0f, -1.0f, 1.0f);

            var rDetM = Sse.Divide(adjSignMask, detM);

            X_ = Sse.Multiply(X_, rDetM);
            Y_ = Sse.Multiply(Y_, rDetM);
            Z_ = Sse.Multiply(Z_, rDetM);
            W_ = Sse.Multiply(W_, rDetM);

            const byte Shuffle_3131 = 0b0111_0111;
            const byte Shuffle_2020 = 0b0010_0010;

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.Row0.x)
            {
                Sse.Store(r + 0, Sse.Shuffle(X_, Y_, Shuffle_3131));
                Sse.Store(r + 4, Sse.Shuffle(X_, Y_, Shuffle_2020));
                Sse.Store(r + 8, Sse.Shuffle(Z_, W_, Shuffle_3131));
                Sse.Store(r + 12, Sse.Shuffle(Z_, W_, Shuffle_2020));
            }
        }
#endif
        private static void Invert(in float4x4 matrix, out float4x4 result)
        {
            // Original implementation can be found here:
            // https://github.com/dotnet/runtime/blob/79ae74f5ca5c8a6fe3a48935e85bd7374959c570/src/libraries/System.Private.CoreLib/src/System/Numerics/Matrix4x4.cs#L1556
            // found via OpenTK

            var mat = matrix;

            float a = mat.M11, b = mat.M21, c = mat.M31, d = mat.M41;
            float e = mat.M12, f = mat.M22, g = mat.M32, h = mat.M42;
            float i = mat.M13, j = mat.M23, k = mat.M33, l = mat.M43;
            float m = mat.M14, n = mat.M24, o = mat.M34, p = mat.M44;

            float kp_lo = k * p - l * o;
            float jp_ln = j * p - l * n;
            float jo_kn = j * o - k * n;
            float ip_lm = i * p - l * m;
            float io_km = i * o - k * m;
            float in_jm = i * n - j * m;

            float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
            float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
            float a14 = -(e * jo_kn - f * io_km + g * in_jm);

            float det = a * a11 + b * a12 + c * a13 + d * a14;

            if (MathF.Abs(det) < float.Epsilon)
            {
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            }

            float invDet = 1.0f / det;

            result.Row0 = new float4(a11, a12, a13, a14) * invDet;

            result.Row1 = new float4(
                -(b * kp_lo - c * jp_ln + d * jo_kn),
                +(a * kp_lo - c * ip_lm + d * io_km),
                -(a * jp_ln - b * ip_lm + d * in_jm),
                +(a * jo_kn - b * io_km + c * in_jm)) * invDet;

            float gp_ho = g * p - h * o;
            float fp_hn = f * p - h * n;
            float fo_gn = f * o - g * n;
            float ep_hm = e * p - h * m;
            float eo_gm = e * o - g * m;
            float en_fm = e * n - f * m;

            result.Row2 = new float4(
                +(b * gp_ho - c * fp_hn + d * fo_gn),
                -(a * gp_ho - c * ep_hm + d * eo_gm),
                +(a * fp_hn - b * ep_hm + d * en_fm),
                -(a * fo_gn - b * eo_gm + c * en_fm)) * invDet;

            float gl_hk = g * l - h * k;
            float fl_hj = f * l - h * j;
            float fk_gj = f * k - g * j;
            float el_hi = e * l - h * i;
            float ek_gi = e * k - g * i;
            float ej_fi = e * j - f * i;

            result.Row3 = new float4(
                -(b * gl_hk - c * fl_hj + d * fk_gj),
                +(a * gl_hk - c * el_hi + d * ek_gi),
                -(a * fl_hj - b * el_hi + d * ej_fi),
                +(a * fk_gj - b * ek_gi + c * ej_fi)) * invDet;
        }

        #endregion Invert Functions

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="matrix">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        /// 
        public static float4x4 Transpose(float4x4 matrix)
        {
#if NET5_0_OR_GREATER
            float4x4 result;

            if (Sse.IsSupported)
            {
                TransposeSse(in matrix, out result);
            }
            else
            {
                Transpose(in matrix, out result);
            }

            return result;
#else
            Transpose(in matrix, out float4x4 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        private static unsafe void TransposeSse(in float4x4 matrix, out float4x4 result)
        {
            Vector128<float> row0;
            Vector128<float> row1;
            Vector128<float> row2;
            Vector128<float> row3;

            fixed (float* m = &matrix.Row0.x)
            {
                row0 = Sse.LoadVector128(m + 0);
                row1 = Sse.LoadVector128(m + 4);
                row2 = Sse.LoadVector128(m + 8);
                row3 = Sse.LoadVector128(m + 12);
            }

            var l12 = Sse.UnpackLow(row0, row1);
            var l34 = Sse.UnpackLow(row2, row3);
            var h12 = Sse.UnpackHigh(row0, row1);
            var h34 = Sse.UnpackHigh(row2, row3);

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.Row0.x)
            {
                Sse.Store(r + 0, Sse.MoveLowToHigh(l12, l34));
                Sse.Store(r + 4, Sse.MoveHighToLow(l34, l12));
                Sse.Store(r + 8, Sse.MoveLowToHigh(h12, h34));
                Sse.Store(r + 12,Sse.MoveHighToLow(h34, h12));
            }
        }
#endif

        private static void Transpose(in float4x4 matrix, out float4x4 result)
        {
            float m11 = matrix.Row0.x;
            float m12 = matrix.Row0.y;
            float m13 = matrix.Row0.z;
            float m14 = matrix.Row0.w;
            float m21 = matrix.Row1.x;
            float m22 = matrix.Row1.y;
            float m23 = matrix.Row1.z;
            float m24 = matrix.Row1.w;
            float m31 = matrix.Row2.x;
            float m32 = matrix.Row2.y;
            float m33 = matrix.Row2.z;
            float m34 = matrix.Row2.w;
            float m41 = matrix.Row3.x;
            float m42 = matrix.Row3.y;
            float m43 = matrix.Row3.z;
            float m44 = matrix.Row3.w;

            result = new float4x4()
            {
                M11 = m11,
                M12 = m21,
                M13 = m31,
                M14 = m41,
                M21 = m12,
                M22 = m22,
                M23 = m32,
                M24 = m42,
                M31 = m13,
                M32 = m23,
                M33 = m33,
                M34 = m43,
                M41 = m14,
                M42 = m24,
                M43 = m34,
                M44 = m44
            };
        }

        #endregion Transpose

        #region Transform        

        /// <summary>
        /// Transforms a given vector by a matrix via matrix*vector (Postmultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 Transform(float4x4 matrix, float4 vector)
        {
            return new float4(
                (matrix.M11 * vector.x) + (matrix.M12 * vector.y) + (matrix.M13 * vector.z) + (matrix.M14 * vector.w),
                (matrix.M21 * vector.x) + (matrix.M22 * vector.y) + (matrix.M23 * vector.z) + (matrix.M24 * vector.w),
                (matrix.M31 * vector.x) + (matrix.M32 * vector.y) + (matrix.M33 * vector.z) + (matrix.M34 * vector.w),
                (matrix.M41 * vector.x) + (matrix.M42 * vector.y) + (matrix.M43 * vector.z) + (matrix.M44 * vector.w));
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 TransformPremult(float4 vector, float4x4 matrix)
        {
            return new float4(
                (matrix.M11 * vector.x) + (matrix.M21 * vector.y) + (matrix.M31 * vector.z) + (matrix.M41 * vector.w),
                (matrix.M12 * vector.x) + (matrix.M22 * vector.y) + (matrix.M32 * vector.z) + (matrix.M42 * vector.w),
                (matrix.M13 * vector.x) + (matrix.M23 * vector.y) + (matrix.M33 * vector.z) + (matrix.M43 * vector.w),
                (matrix.M14 * vector.x) + (matrix.M24 * vector.y) + (matrix.M34 * vector.z) + (matrix.M44 * vector.w));
        }

        /// <summary>
        /// Transforms a given 3D vector by a matrix using perspective division via matrix*vector (Postmultiplication of the vector).
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float3"/> instance containing the result.</returns>
        public static float3 Transform(float4x4 matrix, float3 vector)
        {
#if NET5_0_OR_GREATER
            float3 result;

            if (Sse.IsSupported)
            {
                TransformSse(in matrix, in vector, out result);
            }
            else
            {
                Transform(in matrix, in vector, out result);
            }

            return result;
#else
            Transform(in matrix, in vector, out float3 result);

            return result;
#endif
        }

#if NET5_0_OR_GREATER
        private static unsafe void TransformSse(in float4x4 matrix, in float3 vector, out float3 result)
        {
            Vector128<float> row0;
            Vector128<float> row1;
            Vector128<float> row2;
            Vector128<float> row3;

            fixed (float* m = &matrix.Row0.x)
            {
                row0 = Sse.LoadVector128(m + 0);
                row1 = Sse.LoadVector128(m + 4);
                row2 = Sse.LoadVector128(m + 8);
                row3 = Sse.LoadVector128(m + 12);
            }

            var l12 = Sse.UnpackLow(row0, row1);
            var l34 = Sse.UnpackLow(row2, row3);
            var h12 = Sse.UnpackHigh(row0, row1);
            var h34 = Sse.UnpackHigh(row2, row3);

            var col0 = Sse.MoveLowToHigh(l12, l34);
            var col1 = Sse.MoveHighToLow(l34, l12);
            var col2 = Sse.MoveLowToHigh(h12, h34);
            var col3 = Sse.MoveHighToLow(h34, h12);

            Vector128<float> vec;

            fixed (float* m = &vector.x)
            {
                vec = Sse.LoadVector128(m);
            }

            const byte Shuffle_0000 = 0x00;
            const byte Shuffle_1111 = 0x55;
            const byte Shuffle_2222 = 0xAA;
            const byte Shuffle_3333 = 0xFF;

            var vX = Sse.Shuffle(vec, vec, Shuffle_0000);
            var vY = Sse.Shuffle(vec, vec, Shuffle_1111);
            var vZ = Sse.Shuffle(vec, vec, Shuffle_2222);

            var res = Sse.Divide(Sse.Add(Sse.Add(Sse.Multiply(Sse.MoveLowToHigh(l12, l34), vX),
                                                 Sse.Multiply(Sse.MoveHighToLow(l34, l12), vY)),
                                         Sse.Add(Sse.Multiply(Sse.MoveLowToHigh(h12, h34), vZ),
                                                 Sse.MoveHighToLow(h34, h12))),
                                 Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_0000), vX),
                                                 Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_1111), vY)),
                                         Sse.Add(Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_2222), vZ),
                                                 Sse.Shuffle(row3, row3, Shuffle_3333))));

            Unsafe.SkipInit(out result);

            fixed (float* r = &result.x)
            {
                Sse.Store(r + 0, res);
            }
        }
#endif

        private static void Transform(in float4x4 matrix, in float3 vector, out float3 result)
        {
            var mat = matrix;
            var vec = vector;

            float w = (mat.M41 * vec.x) + (mat.M42 * vec.y) + (mat.M43 * vec.z) + mat.M44;

            result.x = ((mat.M11 * vec.x) + (mat.M12 * vec.y) + (mat.M13 * vec.z) + mat.M14) / w;
            result.y = ((mat.M21 * vec.x) + (mat.M22 * vec.y) + (mat.M23 * vec.z) + mat.M24) / w;
            result.z = ((mat.M31 * vec.x) + (mat.M32 * vec.y) + (mat.M33 * vec.z) + mat.M34) / w;
        }

        /// <summary>
        /// Transforms a given 3D vector by a matrix using perspective division via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float3"/> instance containing the result.</returns>
        public static float3 TransformPremult(float3 vector, float4x4 matrix)
        {
            float w = (matrix.M14 * vector.x) + (matrix.M24 * vector.y) + (matrix.M34 * vector.z) + matrix.M44;
            return new float3(
                ((matrix.M11 * vector.x) + (matrix.M21 * vector.y) + (matrix.M31 * vector.z) + matrix.M41) / w,
                ((matrix.M12 * vector.x) + (matrix.M22 * vector.y) + (matrix.M32 * vector.z) + matrix.M42) / w,
                ((matrix.M13 * vector.x) + (matrix.M23 * vector.y) + (matrix.M33 * vector.z) + matrix.M43) / w);
        }

        /// <summary>
        /// Transforms a given 3D vector by a matrix, and projects the resulting float4 back to a float3.
        /// </summary>
        /// <param name="mat">The desired transformation matrix.</param>
        /// <param name="vec">The given vector.</param>
        /// <returns>The transformed vector.</returns>
        public static float3 TransformPerspective(float4x4 mat, float3 vec)
        {
            var v = new float4(vec, 1.0f);
            v = mat * v;
            float3 result = new float3();

            if (v.w > M.EpsilonFloat)
            {
                result.x = v.x / v.w;
                result.y = v.y / v.w;
                result.z = v.z / v.w;
            }
            else
            {
                result = float3.Zero;
            }

            return result;
        }

        /// <summary>
        /// Transforms the given vector by the given matrix and applies a perspective division.
        /// </summary>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="vec">The given vector.</param>
        /// <returns>The transformed vector.</returns>
        public static float4 TransformPerspective(float4x4 mat, float4 vec)
        {
            float4 tmp = mat * vec;
            return tmp /= tmp.w;
        }

        #endregion Transform

        #region TRS Decomposition

        /// <summary>
        /// Calculates translation of the given float4x4 matrix and returns it as a float3 vector.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static float3 GetTranslation(float4x4 mat)
        {
            return new float3(mat.M14, mat.M24, mat.M34);
        }

        /// <summary>
        /// Calculates and returns only the translation component of the given float4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static float4x4 TranslationDecomposition(float4x4 mat)
        {
            var translationVec = GetTranslation(mat);
            var translationMtx = float4x4.Identity;

            translationMtx.M14 = translationVec.x;
            translationMtx.M24 = translationVec.y;
            translationMtx.M34 = translationVec.z;

            return translationMtx;
        }

        /// <summary>
        /// Calculates and returns the rotation component of the given float4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static float4x4 RotationDecomposition(float4x4 mat)
        {
            var scalevector = GetScale(mat);
            var rotationMtx = float4x4.Identity;

            rotationMtx.M11 = mat.M11 / scalevector.x;
            rotationMtx.M21 = mat.M21 / scalevector.x;
            rotationMtx.M31 = mat.M31 / scalevector.x;

            rotationMtx.M12 = mat.M12 / scalevector.y;
            rotationMtx.M22 = mat.M22 / scalevector.y;
            rotationMtx.M32 = mat.M32 / scalevector.y;

            rotationMtx.M13 = mat.M13 / scalevector.z;
            rotationMtx.M23 = mat.M23 / scalevector.z;
            rotationMtx.M33 = mat.M33 / scalevector.z;

            return rotationMtx;
        }

        /// <summary>
        /// Calculates the scale factor of the given float4x4 and returns it as a float3 vector.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static float3 GetScale(float4x4 mat)
        {
            var scale = float3.One;

            scale.x = mat.Column0.Length;
            scale.y = mat.Column1.Length;
            scale.z = mat.Column2.Length;

            return scale;
        }

        /// <summary>
        /// Calculates and returns the scale component of the given float4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static float4x4 ScaleDecomposition(float4x4 mat)
        {
            var scalevector = GetScale(mat);
            var scaleMtx = float4x4.Identity;

            scaleMtx.M11 = scalevector.x;
            scaleMtx.M22 = scalevector.y;
            scaleMtx.M33 = scalevector.z;

            return scaleMtx;
        }

        #endregion TRS Decomposition

        #region Round

        /// <summary>
        /// Rounds the given matrix to 6 digits (max float precision).
        /// </summary>
        /// <param name="mat">The matrix to round.</param>
        /// <returns>The rounded matrix.</returns>
        public static float4x4 Round(float4x4 mat)
        {
            return new float4x4(float4.Round(mat.Row0),
                                float4.Round(mat.Row1),
                                float4.Round(mat.Row2),
                                float4.Round(mat.Row3));
        }

        #endregion Round

        #endregion Static

        #region Operators

        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float4x4 which holds the result of the multiplication</returns>
        public static float4x4 operator +(float4x4 left, float4x4 right)
        {
            return Add(in left, in right);
        }

        /// <summary>
        /// Matrix subtraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float2x2 which holds the result of the multiplication</returns>
        public static float4x4 operator -(float4x4 left, float4x4 right)
        {
            return Subtract(in left, in right);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static float4x4 operator *(float4x4 left, float4x4 right)
        {
            return Mult(in left, in right);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(float4x4 left, float4x4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(float4x4 left, float4x4 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 operator *(float4x4 matrix, float4 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (pre-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 operator *(float4 vector, float4x4 matrix)
        {
            return TransformPremult(vector, matrix);
        }

        /// <summary>
        /// Transforms a given three dimensional vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float4x4 matrix, float3 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        /// Transforms a given three dimensional vector by a matrix via vector*matrix (pre-multiplication of the vector).
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float3 vector, float4x4 matrix)
        {
            return TransformPremult(vector, matrix);
        }

        /// <summary>
        /// Explicit cast operator to cast a double4x4 into a float4x4 value.
        /// </summary>
        /// <param name="d4x4">The double4x4 value to cast.</param>
        /// <returns>A float4x4 value.</returns>
        public static explicit operator float4x4(double4x4 d4x4)
        {
            return new float4x4(d4x4);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current float4x4.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current float4x4.
        /// </summary>
        /// <param name="provider">Provides information about a specific culture.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(provider);
        }

        internal string ConvertToString(IFormatProvider? provider)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            return string.Format(provider, "{0}\n{1}\n{2}\n{3}", Row0.ToString(provider), Row1.ToString(provider), Row2.ToString(provider), Row3.ToString(provider));
        }

        #endregion public override string ToString()

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion public override int GetHashCode()

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare tresult.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is float4x4))
                return false;

            return Equals((float4x4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #endregion Public Members

        #region IEquatable<float4x4> Members

        /// <summary>
        /// Checks whether row three (the projection part) of the matrix is equal to (0, 0, 0, 1). If this is the case the matrix is affine.
        /// </summary>       
        public bool IsAffine =>
                // Column order notation
                (Row3 == float4.UnitW);

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">A matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(float4x4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion IEquatable<float4x4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a float4x4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, float4x4> ParseConverter { get; set; } = (x => float4x4.Parse(x));

        /// <summary>
        /// Parses a string into a float4x4.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static float4x4 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 16)
                throw new FormatException("String parse for float4x4 did not result in exactly 16 items.");

            float[] floats = new float[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    floats[i] = float.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new float4x4(floats[0], floats[1], floats[2], floats[3], floats[4], floats[5], floats[6], floats[7], floats[8], floats[9], floats[10], floats[11], floats[12], floats[13], floats[14], floats[15]);
        }
    }
}