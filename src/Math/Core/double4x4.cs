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
    /// double4x4 objects represent matrices used in column-vector-notation, that means
    /// when used in common matrix-vector-multiplication scenarios, the vector should
    /// be multiplied from the right to the matrix (M * v). Concatenations of matrices
    /// from left to right represent a transformation where the rightmost matrix is applied
    /// first and the leftmost matrix is applied last, for example in (M3 * M2 * M1) * v the vector
    /// v is first transformed by M1, then by M2 and finally by M3. The translation part of a 4x4
    /// matrix used in homogeneous coordinate calculations can be found in the leftmost column
    /// (M14 - x-translation, M24 - y-translation, M34 - z-translation).
    /// </para>
    /// <para>
    /// Note that although double4x4 objects represent matrices in COLUMN-vector-NOTATION as
    /// found in math books, the objects' contents is physically stored in ROW-major-ORDER, meaning that
    /// in physical memory, a double4x4's components are stored contiguously in the following order: first row (M11, M12, M13, M14),
    /// then second row (M21, M22, M21, M24), and so on. When exchanging matrix contents with libraries like
    /// graphics engines (OpenGL, Direct3D), physics engines, file formats, etc. make sure to convert to and
    /// from the given Matrix layout of the API you are exchanging data with.
    /// </para>
    /// <para>
    /// double4x4 contains convenience construction methods to create matrices commonly
    /// used in Computer Graphics. Most of these application matrices are handedness-agnostic, meaning
    /// that the resulting matrices can be used in both, left-handed and right-handed coordinate systems.
    /// This does not hold for LookAt and Projection matrices where the viewing direction plays a role. In
    /// left-handed coordinate systems the viewing direction is positive, meaning positions further away have
    /// bigger positive z-coordinates whereas in right-handed coordinate systems positions further away have smaller
    /// negative z-coordinates. By default, double4x4 will assume a left-handed coordinate system, but contains
    /// convenience construction methods to also create right-handed matrices if necessary. The right-handed versions
    /// of methods are postfixed with "RH".
    /// </para>
    /// </remarks>
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct double4x4 : IEquatable<double4x4>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        [ProtoMember(1)]
        public double4 Row1;

        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        [ProtoMember(2)]
        public double4 Row2;

        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        [ProtoMember(3)]
        public double4 Row3;

        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        [ProtoMember(4)]
        public double4 Row4;

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static readonly double4x4 Identity = new(double4.UnitX, double4.UnitY, double4.UnitZ, double4.UnitW);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static readonly double4x4 Zero = new(double4.Zero, double4.Zero, double4.Zero, double4.Zero);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row1">Top row of the matrix</param>
        /// <param name="row2">Second row of the matrix</param>
        /// <param name="row3">Third row of the matrix</param>
        /// <param name="row4">Bottom row of the matrix</param>
        public double4x4(double4 row1, double4 row2, double4 row3, double4 row4)
        {
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
            Row4 = row4;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="m11">First item of the first row of the matrix.</param>
        /// <param name="m12">Second item of the first row of the matrix.</param>
        /// <param name="m13">Third item of the first row of the matrix.</param>
        /// <param name="m14">Fourth item of the first row of the matrix.</param>
        /// <param name="m21">First item of the second row of the matrix.</param>
        /// <param name="m22">Second item of the second row of the matrix.</param>
        /// <param name="m23">Third item of the second row of the matrix.</param>
        /// <param name="m24">Fourth item of the second row of the matrix.</param>
        /// <param name="m31">First item of the third row of the matrix.</param>
        /// <param name="m32">Second item of the third row of the matrix.</param>
        /// <param name="m33">Third item of the third row of the matrix.</param>
        /// <param name="m34">First item of the third row of the matrix.</param>
        /// <param name="m41">Fourth item of the fourth row of the matrix.</param>
        /// <param name="m42">Second item of the fourth row of the matrix.</param>
        /// <param name="m43">Third item of the fourth row of the matrix.</param>
        /// <param name="m44">Fourth item of the fourth row of the matrix.</param>
        public double4x4(
            double m11, double m12, double m13, double m14,
            double m21, double m22, double m23, double m24,
            double m31, double m32, double m33, double m34,
            double m41, double m42, double m43, double m44)
        {
            Row1 = new double4(m11, m12, m13, m14);
            Row2 = new double4(m21, m22, m23, m24);
            Row3 = new double4(m31, m32, m33, m34);
            Row4 = new double4(m41, m42, m43, m44);
        }

        /// <summary>
        /// Constructs a new double4x4 by converting from a double4x4.
        /// </summary>
        /// <param name="d4x4">The double4x4 to copy components from.</param>
        public double4x4(float4x4 d4x4)
        {
            Row1 = (double4)d4x4.Row1;
            Row2 = (double4)d4x4.Row2;
            Row3 = (double4)d4x4.Row3;
            Row4 = (double4)d4x4.Row4;
        }

        /// <summary>
        /// Constructs a new double4x4 from a double3x3 by setting the 4th column and row to UnitW respectively.
        /// </summary>
        /// <param name="f3x3">The double3x3 matrix to copy components from.</param>
        public double4x4(double3x3 f3x3)
        {
            Row1 = new double4(f3x3.Row1, 0);
            Row2 = new double4(f3x3.Row2, 0);
            Row3 = new double4(f3x3.Row3, 0);
            Row4 = new double4(0, 0, 0, 1);
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public double Determinant => Row1.x * Row2.y * Row3.z * Row4.w - Row1.x * Row2.y * Row3.w * Row4.z + Row1.x * Row2.z * Row3.w * Row4.y -
                    Row1.x * Row2.z * Row3.y * Row4.w
                    + Row1.x * Row2.w * Row3.y * Row4.z - Row1.x * Row2.w * Row3.z * Row4.y - Row1.y * Row2.z * Row3.w * Row4.x +
                    Row1.y * Row2.z * Row3.x * Row4.w
                    - Row1.y * Row2.w * Row3.x * Row4.z + Row1.y * Row2.w * Row3.z * Row4.x - Row1.y * Row2.x * Row3.z * Row4.w +
                    Row1.y * Row2.x * Row3.w * Row4.z
                    + Row1.z * Row2.w * Row3.x * Row4.y - Row1.z * Row2.w * Row3.y * Row4.x + Row1.z * Row2.x * Row3.y * Row4.w -
                    Row1.z * Row2.x * Row3.w * Row4.y
                    + Row1.z * Row2.y * Row3.w * Row4.x - Row1.z * Row2.y * Row3.x * Row4.w - Row1.w * Row2.x * Row3.y * Row4.z +
                    Row1.w * Row2.x * Row3.z * Row4.y
                    - Row1.w * Row2.y * Row3.z * Row4.x + Row1.w * Row2.y * Row3.x * Row4.z - Row1.w * Row2.z * Row3.x * Row4.y +
                    Row1.w * Row2.z * Row3.y * Row4.x;

        /// <summary>
        /// Returns the trace of this matrix
        /// </summary>
        public double Trace => Row1.x + Row2.y + Row3.z + Row4.w;

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public double4 Column1
        {
            get => new(Row1.x, Row2.x, Row3.x, Row4.x);
            set { Row1.x = value.x; Row2.x = value.y; Row3.x = value.z; Row4.x = value.w; }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public double4 Column2
        {
            get => new(Row1.y, Row2.y, Row3.y, Row4.y);
            set { Row1.y = value.x; Row2.y = value.y; Row3.y = value.z; Row4.y = value.w; }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public double4 Column3
        {
            get => new(Row1.z, Row2.z, Row3.z, Row4.z);
            set { Row1.z = value.x; Row2.z = value.y; Row3.z = value.z; Row4.z = value.w; }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public double4 Column4
        {
            get => new(Row1.w, Row2.w, Row3.w, Row4.w);
            set { Row1.w = value.x; Row2.w = value.y; Row3.w = value.z; Row4.w = value.w; }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 1 of this instance.
        /// </summary>
        public double M11
        {
            get => Row1.x;
            set => Row1.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 2 of this instance.
        /// </summary>
        public double M12
        {
            get => Row1.y;
            set => Row1.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 3 of this instance.
        /// </summary>
        public double M13
        {
            get => Row1.z;
            set => Row1.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 4 of this instance.
        /// </summary>
        public double M14
        {
            get => Row1.w;
            set => Row1.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 1 of this instance.
        /// </summary>
        public double M21
        {
            get => Row2.x;
            set => Row2.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 2 of this instance.
        /// </summary>
        public double M22
        {
            get => Row2.y;
            set => Row2.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 3 of this instance.
        /// </summary>
        public double M23
        {
            get => Row2.z;
            set => Row2.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 4 of this instance.
        /// </summary>
        public double M24
        {
            get => Row2.w;
            set => Row2.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 1 of this instance.
        /// </summary>
        public double M31
        {
            get => Row3.x;
            set => Row3.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 2 of this instance.
        /// </summary>
        public double M32
        {
            get => Row3.y;
            set => Row3.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 3 of this instance.
        /// </summary>
        public double M33
        {
            get => Row3.z;
            set => Row3.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 4 of this instance.
        /// </summary>
        public double M34
        {
            get => Row3.w;
            set => Row3.w = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 1 of this instance.
        /// </summary>
        public double M41
        {
            get => Row4.x;
            set => Row4.x = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 2 of this instance.
        /// </summary>
        public double M42
        {
            get => Row4.y;
            set => Row4.y = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 3 of this instance.
        /// </summary>
        public double M43
        {
            get => Row4.z;
            set => Row4.z = value;
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 4 of this instance.
        /// </summary>
        public double M44
        {
            get => Row4.w;
            set => Row4.w = value;
        }

        /// <summary>
        /// Gets the offset part of the matrix as a <see cref="double3"/> instance.
        /// </summary>
        /// <remarks>
        /// The offset part of the matrix consists of the M14, M24 and M34 components (in row major order notation).
        /// </remarks>
        public double3 Offset => GetTranslation(this);

        #endregion Properties

        #region Instance

        #region this

        /// <summary>
        ///     Sets/Gets value from given index
        /// </summary>
        /// <param name="i">The ROW index</param>
        /// <param name="j">The COLUMN index</param>
        /// <returns></returns>
        public double this[int i, int j]
        {
            get
            {
                return i switch
                {
                    0 => Row1[j],
                    1 => Row2[j],
                    2 => Row3[j],
                    3 => Row4[j],
                    _ => throw new ArgumentOutOfRangeException($"Index {i},{j} not eligible for a double4x4 type"),
                };
            }
            set
            {
                switch (i)
                {
                    case 0:
                        Row1[j] = value;
                        break;

                    case 1:
                        Row2[j] = value;
                        break;

                    case 2:
                        Row3[j] = value;
                        break;

                    case 3:
                        Row4[j] = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Index {i},{j} not eligible for a double4x4 type");
                }
            }
        }

        #endregion this

        #region public Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public double4x4 Invert()
        {
            return Invert(this);
        }

        #endregion public Invert()

        #region public Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public double4x4 Transpose()
        {
            return Transpose(this);
        }

        #region double[] ToArray()

        /// <summary>
        /// Returns the matrix as double array.
        /// </summary>
        /// <returns></returns>
        public double[] ToArray()
        {
            return new[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        #endregion double[] ToArray()

        #endregion public Transpose()

        #region public Round()

        /// <summary>
        /// Rounds this instance to 6 digits (max double precision).
        /// </summary>
        public double4x4 Round()
        {
            return Round(this);
        }

        #endregion public Round()

        #region TRS Decomposition

        /// <summary>
        /// The translation component of this matrix.
        /// </summary>
        public double4x4 TranslationComponent()
        {
            return TranslationDecomposition(this);
        }

        /// <summary>
        /// The translation of this matrix.
        /// </summary>
        public double3 Translation()
        {
            return GetTranslation(this);
        }

        /// <summary>
        /// The rotation component of this matrix.
        /// </summary>
        public double4x4 RotationComponent()
        {
            return RotationDecomposition(this);
        }

        /// <summary>
        /// The scale component of this matrix.
        /// </summary>
        public double4x4 ScaleComponent()
        {
            return ScaleDecomposition(this);
        }

        /// <summary>
        /// The scale factors of this matrix.
        /// </summary>
        public double3 Scale()
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
        public static double4x4 CreateFromAxisAngle(double3 axis, double angle)
        {
            var cos = System.Math.Cos(-angle);
            var sin = System.Math.Sin(-angle);
            var t = 1.0 - cos;

            axis = axis.Normalize();

            var result = new double4x4(t * axis.x * axis.x + cos, t * axis.x * axis.y + sin * axis.z, t * axis.x * axis.z - sin * axis.y, 0.0,
                                  t * axis.x * axis.y - sin * axis.z, t * axis.y * axis.y + cos, t * axis.y * axis.z + sin * axis.x, 0.0,
                                  t * axis.x * axis.z + sin * axis.y, t * axis.y * axis.z - sin * axis.x, t * axis.z * axis.z + cos, 0.0,
                                  0.0, 0.0, 0.0, 1.0);

            return result;
        }

        #endregion CreateFromAxisAngle

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateRotationX(double angle)
        {
            double4x4 result;

            var cos = System.Math.Cos(angle);
            var sin = System.Math.Sin(angle);

            result.Row1 = double4.UnitX;
            result.Row2 = new double4(0.0, cos, -sin, 0.0);
            result.Row3 = new double4(0.0, sin, cos, 0.0);
            result.Row4 = double4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateRotationY(double angle)
        {
            double4x4 result;

            var cos = System.Math.Cos(angle);
            var sin = System.Math.Sin(angle);

            result.Row1 = new double4(cos, 0.0, sin, 0.0);
            result.Row2 = double4.UnitY;
            result.Row3 = new double4(-sin, 0.0, cos, 0.0);
            result.Row4 = double4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateRotationZ(double angle)
        {
            double4x4 result;

            var cos = System.Math.Cos(angle);
            var sin = System.Math.Sin(angle);

            result.Row1 = new double4(cos, -sin, 0.0, 0.0);
            result.Row2 = new double4(sin, cos, 0.0, 0.0);
            result.Row3 = double4.UnitZ;
            result.Row4 = double4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xy">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationXY(double2 xy)
        {
            return CreateRotationXY(xy.x, xy.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="x">counter-clockwise angles in radians.</param>
        /// <param name="y">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationXY(double x, double y)
        {
            return CreateRotationY(y) * CreateRotationX(x);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and z-axis.
        /// </summary>
        /// <param name="yz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationZY(double2 yz)
        {
            return CreateRotationZY(yz.x, yz.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="y">counter-clockwise angles in radians.</param>
        /// <param name="z">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationZY(double y, double z)
        {
            return CreateRotationY(y) * CreateRotationZ(z);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationZX(double2 xz)
        {
            return CreateRotationZX(xz.x, xz.y);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="x">counter-clockwise angles in radians.</param>
        /// <param name="z">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationZX(double x, double z)
        {
            return CreateRotationX(x) * CreateRotationZ(z);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y and x-axis.
        /// </summary>
        /// <param name="xyz">counter-clockwise angles in radians.</param>
        /// <returns></returns>
        public static double4x4 CreateRotationZXY(double3 xyz)
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
        public static double4x4 CreateRotationZXY(double x, double y, double z)
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
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateTranslation(double x, double y, double z)
        {
            double4x4 result = Identity;

            result.M14 = x;
            result.M24 = y;
            result.M34 = z;

            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateTranslation(double3 vector)
        {
            double4x4 result = Identity;

            result.M14 = vector.x;
            result.M24 = vector.y;
            result.M34 = vector.z;

            return result;
        }

        #endregion CreateTranslation

        #region Rotation matrix to euler representation

        //see Blender mathutils and Graphic Gems IV p. 222-229
        private static void RotMatToEuler2(double[][] mat, ref double[] eul1, ref double[] eul2)
        {
            var axis = new double3(1, 0, 2); //for rotation order YXZ, see Blender mathutils and Graphic Gems IV for other configurations
            var parity = 1; //parity of axis permutation (even=0, odd=1) - 'n' in original code

            int i = (int)axis.x, j = (int)axis.y, k = (int)axis.z;
            var cy = System.Math.Sqrt(System.Math.Pow(mat[i][i], 2.0f) + System.Math.Pow(mat[i][j], 2.0f));

            //var FLT_EPSILON = 1.192092896e-07F;

            if (cy > 16.0 * M.EpsilonDouble)
            {
                eul1[i] = System.Math.Atan2(mat[j][k], mat[k][k]);
                eul1[j] = System.Math.Atan2(-mat[i][k], cy);
                eul1[k] = System.Math.Atan2(mat[i][j], mat[i][i]);

                eul2[i] = System.Math.Atan2(-mat[j][k], -mat[k][k]);
                eul2[j] = System.Math.Atan2(-mat[i][k], -cy);
                eul2[k] = System.Math.Atan2(-mat[i][j], -mat[i][i]);
            }
            else
            {
                eul1[i] = System.Math.Atan2(-mat[k][j], mat[j][j]);
                eul1[j] = System.Math.Atan2(-mat[i][k], cy);
                eul1[k] = 0;

                eul2[i] = System.Math.Atan2(-mat[k][j], mat[j][j]);
                eul2[j] = System.Math.Atan2(-mat[i][k], cy);
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
        /// <param name="rotMat">The rotation matrix.</param>
        public static double3 RotMatToEuler(double4x4 rotMat)
        {
            //Matrix is being handled as a multi-dimensional array to ensure that the rotation order can be changed easily in the future.
            var m = new[] { rotMat.Row1.ToArray(), rotMat.Row2.ToArray(), rotMat.Row3.ToArray(), rotMat.Row4.ToArray() };

            var eul1 = new double[3];
            var eul2 = new double[3];
            double d1, d2;

            RotMatToEuler2(m, ref eul1, ref eul2);

            d1 = System.Math.Abs(eul1[0]) + System.Math.Abs(eul1[1]) + System.Math.Abs(eul1[2]);
            d2 = System.Math.Abs(eul2[0]) + System.Math.Abs(eul2[1]) + System.Math.Abs(eul2[2]);

            /* return best, which is just the one with lowest values it in */
            return d1 > d2 ? new double3(eul2[0], eul2[1], eul2[2]) : new double3(eul1[0], eul1[1], eul1[2]);
        }

        #endregion Rotation matrix to euler representation

        #region CreateScale

        /// <summary>
        /// Creates a uniform scale matrix with the same scale value along all three dimensions.
        /// </summary>
        /// <param name="scale">The value to scale about x, y, and z.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateScale(double scale)
        {
            double4x4 result = Identity;

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
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateScale(double x, double y, double z)
        {
            double4x4 result = Identity;

            result.M11 = x;
            result.M22 = y;
            result.M33 = z;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="vector">The scale vector.</param>
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateScale(double3 vector)
        {
            double4x4 result = Identity;

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
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateOrthographic(double width, double height, double zNear, double zFar)
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
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateOrthographicOffCenterRH(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            double4x4 result = new();

            double invRL = 1 / (right - left);
            double invTB = 1 / (top - bottom);
            double invFN = 1 / (zFar - zNear);

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
        /// <returns>The resulting double4x4 instance.</returns>
        public static double4x4 CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            double4x4 result = new();

            double invRL = 1 / (right - left);
            double invTB = 1 / (top - bottom);
            double invFN = 1 / (zFar - zNear);

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
        /// <item>fovy is zero, less than zero or larger than System.Math.PI</item>
        /// <item>aspect is negative or zero</item>
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static double4x4 CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar)
        {
            double4x4 result;

            if (fovy <= 0 || fovy > System.Math.PI)
                throw new ArgumentOutOfRangeException(nameof(fovy));
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException(nameof(aspect));
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException(nameof(zNear));
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException(nameof(zFar));
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException(nameof(zNear));

            double yMax = zNear * System.Math.Tan(0.5f * fovy);
            double yMin = -yMax;
            double xMin = yMin * aspect;
            double xMax = yMax * aspect;

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
        public static double4x4 CreatePerspectiveOffCenterRH(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            double4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException(nameof(zNear));
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException(nameof(zFar));
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException(nameof(zNear));

            double x = (2.0 * zNear) / (right - left);
            double y = (2.0 * zNear) / (top - bottom);
            // Right handed
            double a = (right + left) / (right - left);
            double b = (top + bottom) / (top - bottom);
            double c = -(zFar + zNear) / (zFar - zNear);
            double d = -(2.0 * zFar * zNear) / (zFar - zNear);

            result = new double4x4(x, 0, a, 0,
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
        public static double4x4 CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            double4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException(nameof(zNear));
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException(nameof(zFar));
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException(nameof(zNear));

            double x = (2.0 * zNear) / (right - left);
            double y = (2.0 * zNear) / (top - bottom);
            // Left Handed
            double a = (left + right) / (left - right);
            double b = (top + bottom) / (bottom - top);
            double c = (zFar + zNear) / (zFar - zNear);
            double d = -(2.0 * zFar * zNear) / (zFar - zNear);

            result = new double4x4(x, 0, a, 0,
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
        public static double4x4 Scale(double scale)
        {
            return Scale(scale, scale, scale);
        }

        /// <summary>
        /// Build a scaling matrix
        /// </summary>
        /// <param name="scale">Scale factors for x,y and z axes</param>
        /// <returns>A scaling matrix</returns>
        public static double4x4 Scale(double3 scale)
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
        public static double4x4 Scale(double x, double y, double z)
        {
            double4x4 result;
            result.Row1 = double4.UnitX * x;
            result.Row2 = double4.UnitY * y;
            result.Row3 = double4.UnitZ * z;
            result.Row4 = double4.UnitW;
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
        public static double4x4 LookAt(double3 eye, double3 target, double3 up)
        {
            var z = double3.Normalize(target - eye);
            var x = double3.Normalize(double3.Cross(up, z));
            var y = double3.Cross(z, x);

            // Row order notation
            //return new double4x4(new double4(x.x, y.x, z.x, 0),
            //                    new double4(x.y, y.y, z.y, 0),
            //                    new double4(x.z, y.z, z.z, 0),
            //                    new double4(-double3.Dot(x, eye), -double3.Dot(y, eye), -double3.Dot(z, eye), 1));

            // Column order notation
            return new double4x4(x.x, x.y, x.z, -double3.Dot(x, eye),
                                y.x, y.y, y.z, -double3.Dot(y, eye),
                                z.x, z.y, z.z, -double3.Dot(z, eye),
                                0, 0, 0, 1);
        }

        /// <summary>
        /// Build a right handed world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A double4x4 that transforms world space to camera space</returns>
        public static double4x4 LookAtRH(double3 eye, double3 target, double3 up)
        {
            var z = double3.Normalize(eye - target);
            var x = double3.Normalize(double3.Cross(up, z));
            var y = double3.Cross(z, x);

            // Row order notation
            //return new double4x4(new double4(x.x, y.x, z.x, 0),
            //                    new double4(x.y, y.y, z.y, 0),
            //                    new double4(x.z, y.z, z.z, 0),
            //                    new double4(-double3.Dot(x, eye), -double3.Dot(y, eye), -double3.Dot(z, eye), 1));

            // Column order notation
            return new double4x4(x.x, x.y, x.z, -double3.Dot(x, eye),
                                y.x, y.y, y.z, -double3.Dot(y, eye),
                                z.x, z.y, z.z, -double3.Dot(z, eye),
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
        /// <returns>A double4x4 that transforms world space to camera space</returns>
        public static double4x4 LookAt(double eyeX, double eyeY, double eyeZ, double targetX, double targetY, double targetZ,
                                      double upX, double upY, double upZ)
        {
            return LookAt(new double3(eyeX, eyeY, eyeZ), new double3(targetX, targetY, targetZ), new double3(upX, upY, upZ));
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
        public static double4x4 Add(in double4x4 left, in double4x4 right)
        {
            //#if NET5_0_OR_GREATER
            //            double4x4 result;

            //            if (Sse.IsSupported)
            //            {
            //                AddSse(in left, in right, out result);
            //            }
            //            else
            //            {
            //                Add(in left, in right, out result);
            //            }

            //            return result;
            //#else
            Add(in left, in right, out double4x4 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void AddSse(in double4x4 left, in double4x4 right, out double4x4 result)
        //        {
        //            Vector128<double> leftrow0;
        //            Vector128<double> leftrow1;
        //            Vector128<double> leftrow2;
        //            Vector128<double> leftrow3;

        //            fixed (double* m = &left.Row1.x)
        //            {
        //                leftrow0 = Sse.LoadVector128(m + 0);
        //                leftrow1 = Sse.LoadVector128(m + 4);
        //                leftrow2 = Sse.LoadVector128(m + 8);
        //                leftrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            Vector128<double> rightrow0;
        //            Vector128<double> rightrow1;
        //            Vector128<double> rightrow2;
        //            Vector128<double> rightrow3;

        //            fixed (double* m = &right.Row1.x)
        //            {
        //                rightrow0 = Sse.LoadVector128(m + 0);
        //                rightrow1 = Sse.LoadVector128(m + 4);
        //                rightrow2 = Sse.LoadVector128(m + 8);
        //                rightrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            var resultrow0 = Sse.Add(leftrow0, rightrow0);
        //            var resultrow1 = Sse.Add(leftrow1, rightrow1);
        //            var resultrow2 = Sse.Add(leftrow2, rightrow2);
        //            var resultrow3 = Sse.Add(leftrow3, rightrow3);

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.Row1.x)
        //            {
        //                Sse.Store(r + 0, resultrow0);
        //                Sse.Store(r + 4, resultrow1);
        //                Sse.Store(r + 8, resultrow2);
        //                Sse.Store(r + 12, resultrow3);
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add(in double4x4 left, in double4x4 right, out double4x4 result)
        {
            result.Row1 = left.Row1 + right.Row1;
            result.Row2 = left.Row2 + right.Row2;
            result.Row3 = left.Row3 + right.Row3;
            result.Row4 = left.Row4 + right.Row4;
        }

        /// <summary>
        /// Subtracts the right instance from the left instance.
        /// </summary>
        /// <param name="left">The left operand of the subtraction.</param>
        /// <param name="right">The right operand of the subtraction.</param>
        /// <returns>A new instance that is the result of the subtraction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 Subtract(in double4x4 left, in double4x4 right)
        {
            //#if NET5_0_OR_GREATER
            //            double4x4 result;

            //            if (Sse.IsSupported)
            //            {
            //                SubtractSse(in left, in right, out result);
            //            }
            //            else
            //            {
            //                Subtract(in left, in right, out result);
            //            }

            //            return result;
            //#else
            Subtract(in left, in right, out double4x4 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void SubtractSse(in double4x4 left, in double4x4 right, out double4x4 result)
        //        {
        //            Vector128<double> leftrow0;
        //            Vector128<double> leftrow1;
        //            Vector128<double> leftrow2;
        //            Vector128<double> leftrow3;

        //            fixed (double* m = &left.Row1.x)
        //            {
        //                leftrow0 = Sse.LoadVector128(m + 0);
        //                leftrow1 = Sse.LoadVector128(m + 4);
        //                leftrow2 = Sse.LoadVector128(m + 8);
        //                leftrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            Vector128<double> rightrow0;
        //            Vector128<double> rightrow1;
        //            Vector128<double> rightrow2;
        //            Vector128<double> rightrow3;

        //            fixed (double* m = &right.Row1.x)
        //            {
        //                rightrow0 = Sse.LoadVector128(m + 0);
        //                rightrow1 = Sse.LoadVector128(m + 4);
        //                rightrow2 = Sse.LoadVector128(m + 8);
        //                rightrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            var resultrow0 = Sse.Subtract(leftrow0, rightrow0);
        //            var resultrow1 = Sse.Subtract(leftrow1, rightrow1);
        //            var resultrow2 = Sse.Subtract(leftrow2, rightrow2);
        //            var resultrow3 = Sse.Subtract(leftrow3, rightrow3);

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.Row1.x)
        //            {
        //                Sse.Store(r + 0, resultrow0);
        //                Sse.Store(r + 4, resultrow1);
        //                Sse.Store(r + 8, resultrow2);
        //                Sse.Store(r + 12, resultrow3);
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Subtract(in double4x4 left, in double4x4 right, out double4x4 result)
        {
            result.Row1 = left.Row1 - right.Row1;
            result.Row2 = left.Row2 - right.Row2;
            result.Row3 = left.Row3 - right.Row3;
            result.Row4 = left.Row4 - right.Row4;
        }

        #endregion Elementary Arithmetic Functions

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 Mult(in double4x4 left, in double4x4 right)
        {
            //#if NET5_0_OR_GREATER
            //            double4x4 result;

            //            if (Sse.IsSupported)
            //            {
            //                MultSse(in left, in right, out result);
            //            }
            //            else
            //            {
            //                Mult(in left, in right, out result);
            //            }

            //            return result;
            //#else
            Mult(in left, in right, out double4x4 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void MultSse(in double4x4 left, in double4x4 right, out double4x4 result)
        //        {
        //            Vector128<double> leftrow0;
        //            Vector128<double> leftrow1;
        //            Vector128<double> leftrow2;
        //            Vector128<double> leftrow3;

        //            fixed (double* m = &left.Row1.x)
        //            {
        //                leftrow0 = Sse.LoadVector128(m + 0);
        //                leftrow1 = Sse.LoadVector128(m + 4);
        //                leftrow2 = Sse.LoadVector128(m + 8);
        //                leftrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            Vector128<double> rightrow0;
        //            Vector128<double> rightrow1;
        //            Vector128<double> rightrow2;
        //            Vector128<double> rightrow3;

        //            fixed (double* m = &right.Row1.x)
        //            {
        //                rightrow0 = Sse.LoadVector128(m + 0);
        //                rightrow1 = Sse.LoadVector128(m + 4);
        //                rightrow2 = Sse.LoadVector128(m + 8);
        //                rightrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            var resultrow0 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0x00), rightrow0),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0x55), rightrow1)),
        //                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0xAA), rightrow2),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow0, leftrow0, 0xFF), rightrow3)));

        //            var resultrow1 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0x00), rightrow0),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0x55), rightrow1)),
        //                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0xAA), rightrow2),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow1, leftrow1, 0xFF), rightrow3)));

        //            var resultrow2 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0x00), rightrow0),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0x55), rightrow1)),
        //                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0xAA), rightrow2),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow2, leftrow2, 0xFF), rightrow3)));

        //            var resultrow3 = Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0x00), rightrow0),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0x55), rightrow1)),
        //                                     Sse.Add(Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0xAA), rightrow2),
        //                                             Sse.Multiply(Sse.Shuffle(leftrow3, leftrow3, 0xFF), rightrow3)));

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.Row1.x)
        //            {
        //                Sse.Store(r + 0, resultrow0);
        //                Sse.Store(r + 4, resultrow1);
        //                Sse.Store(r + 8, resultrow2);
        //                Sse.Store(r + 12, resultrow3);
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Mult(in double4x4 left, in double4x4 right, out double4x4 result)
        {
            double leftM11 = left.Row1.x;
            double leftM12 = left.Row1.y;
            double leftM13 = left.Row1.z;
            double leftM14 = left.Row1.w;
            double leftM21 = left.Row2.x;
            double leftM22 = left.Row2.y;
            double leftM23 = left.Row2.z;
            double leftM24 = left.Row2.w;
            double leftM31 = left.Row3.x;
            double leftM32 = left.Row3.y;
            double leftM33 = left.Row3.z;
            double leftM34 = left.Row3.w;
            double leftM41 = left.Row4.x;
            double leftM42 = left.Row4.y;
            double leftM43 = left.Row4.z;
            double leftM44 = left.Row4.w;
            double rightM11 = right.Row1.x;
            double rightM12 = right.Row1.y;
            double rightM13 = right.Row1.z;
            double rightM14 = right.Row1.w;
            double rightM21 = right.Row2.x;
            double rightM22 = right.Row2.y;
            double rightM23 = right.Row2.z;
            double rightM24 = right.Row2.w;
            double rightM31 = right.Row3.x;
            double rightM32 = right.Row3.y;
            double rightM33 = right.Row3.z;
            double rightM34 = right.Row3.w;
            double rightM41 = right.Row4.x;
            double rightM42 = right.Row4.y;
            double rightM43 = right.Row4.z;
            double rightM44 = right.Row4.w;

            result.Row1.x = (leftM11 * rightM11) + (leftM12 * rightM21) + (leftM13 * rightM31) + (leftM14 * rightM41);
            result.Row1.y = (leftM11 * rightM12) + (leftM12 * rightM22) + (leftM13 * rightM32) + (leftM14 * rightM42);
            result.Row1.z = (leftM11 * rightM13) + (leftM12 * rightM23) + (leftM13 * rightM33) + (leftM14 * rightM43);
            result.Row1.w = (leftM11 * rightM14) + (leftM12 * rightM24) + (leftM13 * rightM34) + (leftM14 * rightM44);
            result.Row2.x = (leftM21 * rightM11) + (leftM22 * rightM21) + (leftM23 * rightM31) + (leftM24 * rightM41);
            result.Row2.y = (leftM21 * rightM12) + (leftM22 * rightM22) + (leftM23 * rightM32) + (leftM24 * rightM42);
            result.Row2.z = (leftM21 * rightM13) + (leftM22 * rightM23) + (leftM23 * rightM33) + (leftM24 * rightM43);
            result.Row2.w = (leftM21 * rightM14) + (leftM22 * rightM24) + (leftM23 * rightM34) + (leftM24 * rightM44);
            result.Row3.x = (leftM31 * rightM11) + (leftM32 * rightM21) + (leftM33 * rightM31) + (leftM34 * rightM41);
            result.Row3.y = (leftM31 * rightM12) + (leftM32 * rightM22) + (leftM33 * rightM32) + (leftM34 * rightM42);
            result.Row3.z = (leftM31 * rightM13) + (leftM32 * rightM23) + (leftM33 * rightM33) + (leftM34 * rightM43);
            result.Row3.w = (leftM31 * rightM14) + (leftM32 * rightM24) + (leftM33 * rightM34) + (leftM34 * rightM44);
            result.Row4.x = (leftM41 * rightM11) + (leftM42 * rightM21) + (leftM43 * rightM31) + (leftM44 * rightM41);
            result.Row4.y = (leftM41 * rightM12) + (leftM42 * rightM22) + (leftM43 * rightM32) + (leftM44 * rightM42);
            result.Row4.z = (leftM41 * rightM13) + (leftM42 * rightM23) + (leftM43 * rightM33) + (leftM44 * rightM43);
            result.Row4.w = (leftM41 * rightM14) + (leftM42 * rightM24) + (leftM43 * rightM34) + (leftM44 * rightM44);
        }

        #endregion Multiply Functions

        #region Invert Functions

        /// <summary>
        /// Checks if this matrix is invertible.
        /// </summary>
        /// <param name="mat">The matrix.</param>       
        public static bool IsInvertable(double4x4 mat)
        {
            return mat.Determinant != 0;
        }

        /// <summary>
        /// Checks if this matrix is invertible.
        /// </summary>
        /// <param name="mat">The matrix.</param>
        /// <param name="det">The determinant of the matrix.</param>       
        public static bool IsInvertable(double4x4 mat, out double det)
        {
            det = mat.Determinant;
            return det != 0;
        }

        /// <summary>
        /// Calculate the inverse of the given matrix.
        /// If you are unsure whether the matrix is invertible, check it with IsInvertable() first.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 Invert(in double4x4 matrix)
        {
            if (matrix == Identity || matrix == Zero) return matrix;

            //#if NET5_0_OR_GREATER
            //            double4x4 result;

            //            if (Sse3.IsSupported)
            //            {
            //                InvertSse3(in matrix, out result);
            //            }
            //            else
            //            {
            //                Invert(in matrix, out result);
            //            }

            //            return result;
            //#else
            Invert(in matrix, out double4x4 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void InvertSse3(in double4x4 matrix, out double4x4 result)
        //        {
        //            // Original derivation and implementation can be found here:
        //            // https://lxjk.github.io/2017/09/03/Fast-4x4-Matrix-Inverse-with-SSE-SIMD-Explained.html
        //            // found via OpenTK

        //            Vector128<double> row0;
        //            Vector128<double> row1;
        //            Vector128<double> row2;
        //            Vector128<double> row3;

        //            fixed (double* m = &matrix.Row1.x)
        //            {
        //                row0 = Sse.LoadVector128(m);
        //                row1 = Sse.LoadVector128(m + 4);
        //                row2 = Sse.LoadVector128(m + 8);
        //                row3 = Sse.LoadVector128(m + 12);
        //            }

        //            var A = Sse.MoveLowToHigh(row0, row1);
        //            var B = Sse.MoveHighToLow(row1, row0);
        //            var C = Sse.MoveLowToHigh(row2, row3);
        //            var D = Sse.MoveHighToLow(row3, row2);

        //            const byte Shuffle_0202 = 0b1000_1000;
        //            const byte Shuffle_1313 = 0b1101_1101;

        //            var detSub = Sse.Subtract(
        //                Sse.Multiply(
        //                    Sse.Shuffle(row0, row2, Shuffle_0202),
        //                    Sse.Shuffle(row1, row3, Shuffle_1313)),
        //                Sse.Multiply(
        //                    Sse.Shuffle(row0, row2, Shuffle_1313),
        //                    Sse.Shuffle(row1, row3, Shuffle_0202)));

        //            const byte Shuffle_0000 = 0b0000_0000;
        //            const byte Shuffle_1111 = 0b0101_0101;
        //            const byte Shuffle_2222 = 0b1010_1010;
        //            const byte Shuffle_3333 = 0b1111_1111;

        //            var detA = Sse2.Shuffle(detSub.AsInt32(), Shuffle_0000).AsSingle();
        //            var detB = Sse2.Shuffle(detSub.AsInt32(), Shuffle_1111).AsSingle();
        //            var detC = Sse2.Shuffle(detSub.AsInt32(), Shuffle_2222).AsSingle();
        //            var detD = Sse2.Shuffle(detSub.AsInt32(), Shuffle_3333).AsSingle();

        //            const byte Shuffle_3300 = 0b0000_1111;
        //            const byte Shuffle_1122 = 0b1010_0101;
        //            const byte Shuffle_2301 = 0b0100_1110;

        //            var D_C = Sse.Subtract(
        //                Sse.Multiply(Sse2.Shuffle(D.AsInt32(), Shuffle_3300).AsSingle(), C),
        //                Sse.Multiply(
        //                    Sse2.Shuffle(D.AsInt32(), Shuffle_1122).AsSingle(),
        //                    Sse2.Shuffle(C.AsInt32(), Shuffle_2301).AsSingle()));

        //            var A_B = Sse.Subtract(
        //                Sse.Multiply(Sse2.Shuffle(A.AsInt32(), Shuffle_3300).AsSingle(), B),
        //                Sse.Multiply(
        //                    Sse2.Shuffle(A.AsInt32(), Shuffle_1122).AsSingle(),
        //                    Sse2.Shuffle(B.AsInt32(), Shuffle_2301).AsSingle()));

        //            const byte Shuffle_0303 = 0b1100_1100;
        //            const byte Shuffle_1032 = 0b1011_0001;
        //            const byte Shuffle_2121 = 0b0110_0110;

        //            var X_ = Sse.Subtract(
        //                Sse.Multiply(detD, A),
        //                Sse.Add(
        //                    Sse.Multiply(B, Sse2.Shuffle(D_C.AsInt32(), Shuffle_0303).AsSingle()),
        //                    Sse.Multiply(
        //                        Sse2.Shuffle(B.AsInt32(), Shuffle_1032).AsSingle(),
        //                        Sse2.Shuffle(D_C.AsInt32(), Shuffle_2121).AsSingle())));

        //            var W_ = Sse.Subtract(
        //                Sse.Multiply(detA, D),
        //                Sse.Add(
        //                    Sse.Multiply(C, Sse2.Shuffle(A_B.AsInt32(), Shuffle_0303).AsSingle()),
        //                    Sse.Multiply(
        //                        Sse2.Shuffle(C.AsInt32(), Shuffle_1032).AsSingle(),
        //                        Sse2.Shuffle(A_B.AsInt32(), Shuffle_2121).AsSingle())));

        //            var detM = Sse.Multiply(detA, detD);

        //            const byte Shuffle_3030 = 0b0011_0011;

        //            var Y_ = Sse.Subtract(
        //                Sse.Multiply(detB, C),
        //                Sse.Subtract(
        //                    Sse.Multiply(D, Sse2.Shuffle(A_B.AsInt32(), Shuffle_3030).AsSingle()),
        //                    Sse.Multiply(
        //                        Sse2.Shuffle(D.AsInt32(), Shuffle_1032).AsSingle(),
        //                        Sse2.Shuffle(A_B.AsInt32(), Shuffle_2121).AsSingle())));

        //            var Z_ = Sse.Subtract(
        //                Sse.Multiply(detC, B),
        //                Sse.Subtract(
        //                    Sse.Multiply(A, Sse2.Shuffle(D_C.AsInt32(), Shuffle_3030).AsSingle()),
        //                    Sse.Multiply(
        //                        Sse2.Shuffle(A.AsInt32(), Shuffle_1032).AsSingle(),
        //                        Sse2.Shuffle(D_C.AsInt32(), Shuffle_2121).AsSingle())));

        //            detM = Sse.Add(detM, Sse.Multiply(detB, detC));

        //            const byte Shuffle_0213 = 0b1101_1000;

        //            var tr = Sse.Multiply(A_B, Sse2.Shuffle(D_C.AsInt32(), Shuffle_0213).AsSingle());
        //            tr = Sse3.HorizontalAdd(tr, tr);
        //            tr = Sse3.HorizontalAdd(tr, tr);

        //            detM = Sse.Subtract(detM, tr);

        //            if (System.Math.Abs(detM.GetElement(0)) < double.Epsilon)
        //            {
        //                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
        //            }

        //            var adjSignMask = Vector128.Create(1.0f, -1.0f, -1.0f, 1.0f);

        //            var rDetM = Sse.Divide(adjSignMask, detM);

        //            X_ = Sse.Multiply(X_, rDetM);
        //            Y_ = Sse.Multiply(Y_, rDetM);
        //            Z_ = Sse.Multiply(Z_, rDetM);
        //            W_ = Sse.Multiply(W_, rDetM);

        //            const byte Shuffle_3131 = 0b0111_0111;
        //            const byte Shuffle_2020 = 0b0010_0010;

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.Row1.x)
        //            {
        //                Sse.Store(r + 0, Sse.Shuffle(X_, Y_, Shuffle_3131));
        //                Sse.Store(r + 4, Sse.Shuffle(X_, Y_, Shuffle_2020));
        //                Sse.Store(r + 8, Sse.Shuffle(Z_, W_, Shuffle_3131));
        //                Sse.Store(r + 12, Sse.Shuffle(Z_, W_, Shuffle_2020));
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Invert(in double4x4 matrix, out double4x4 result)
        {
            // Original implementation can be found here:
            // https://github.com/dotnet/runtime/blob/79ae74f5ca5c8a6fe3a48935e85bd7374959c570/src/libraries/System.Private.CoreLib/src/System/Numerics/Matrix4x4.cs#L1556
            // found via OpenTK

            var mat = matrix;

            double a = mat.M11, b = mat.M21, c = mat.M31, d = mat.M41;
            double e = mat.M12, f = mat.M22, g = mat.M32, h = mat.M42;
            double i = mat.M13, j = mat.M23, k = mat.M33, l = mat.M43;
            double m = mat.M14, n = mat.M24, o = mat.M34, p = mat.M44;

            double kp_lo = k * p - l * o;
            double jp_ln = j * p - l * n;
            double jo_kn = j * o - k * n;
            double ip_lm = i * p - l * m;
            double io_km = i * o - k * m;
            double in_jm = i * n - j * m;

            double a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
            double a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            double a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
            double a14 = -(e * jo_kn - f * io_km + g * in_jm);

            double det = a * a11 + b * a12 + c * a13 + d * a14;

            if (System.Math.Abs(det) < double.Epsilon)
            {
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            }

            double invDet = 1.0f / det;

            result.Row1 = new double4(a11, a12, a13, a14) * invDet;

            result.Row2 = new double4(
                -(b * kp_lo - c * jp_ln + d * jo_kn),
                +(a * kp_lo - c * ip_lm + d * io_km),
                -(a * jp_ln - b * ip_lm + d * in_jm),
                +(a * jo_kn - b * io_km + c * in_jm)) * invDet;

            double gp_ho = g * p - h * o;
            double fp_hn = f * p - h * n;
            double fo_gn = f * o - g * n;
            double ep_hm = e * p - h * m;
            double eo_gm = e * o - g * m;
            double en_fm = e * n - f * m;

            result.Row3 = new double4(
                +(b * gp_ho - c * fp_hn + d * fo_gn),
                -(a * gp_ho - c * ep_hm + d * eo_gm),
                +(a * fp_hn - b * ep_hm + d * en_fm),
                -(a * fo_gn - b * eo_gm + c * en_fm)) * invDet;

            double gl_hk = g * l - h * k;
            double fl_hj = f * l - h * j;
            double fk_gj = f * k - g * j;
            double el_hi = e * l - h * i;
            double ek_gi = e * k - g * i;
            double ej_fi = e * j - f * i;

            result.Row4 = new double4(
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 Transpose(double4x4 matrix)
        {
            //#if NET5_0_OR_GREATER
            //            double4x4 result;

            //            if (Sse.IsSupported)
            //            {
            //                TransposeSse(in matrix, out result);
            //            }
            //            else
            //            {
            //                Transpose(in matrix, out result);
            //            }

            //            return result;
            //#else
            Transpose(in matrix, out double4x4 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void TransposeSse(in double4x4 matrix, out double4x4 result)
        //        {
        //            Vector128<double> row0;
        //            Vector128<double> row1;
        //            Vector128<double> row2;
        //            Vector128<double> row3;

        //            fixed (double* m = &matrix.Row1.x)
        //            {
        //                row0 = Sse.LoadVector128(m + 0);
        //                row1 = Sse.LoadVector128(m + 4);
        //                row2 = Sse.LoadVector128(m + 8);
        //                row3 = Sse.LoadVector128(m + 12);
        //            }

        //            var l12 = Sse.UnpackLow(row0, row1);
        //            var l34 = Sse.UnpackLow(row2, row3);
        //            var h12 = Sse.UnpackHigh(row0, row1);
        //            var h34 = Sse.UnpackHigh(row2, row3);

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.Row1.x)
        //            {
        //                Sse.Store(r + 0, Sse.MoveLowToHigh(l12, l34));
        //                Sse.Store(r + 4, Sse.MoveHighToLow(l34, l12));
        //                Sse.Store(r + 8, Sse.MoveLowToHigh(h12, h34));
        //                Sse.Store(r + 12,Sse.MoveHighToLow(h34, h12));
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Transpose(in double4x4 matrix, out double4x4 result)
        {
            double m11 = matrix.Row1.x;
            double m12 = matrix.Row1.y;
            double m13 = matrix.Row1.z;
            double m14 = matrix.Row1.w;
            double m21 = matrix.Row2.x;
            double m22 = matrix.Row2.y;
            double m23 = matrix.Row2.z;
            double m24 = matrix.Row2.w;
            double m31 = matrix.Row3.x;
            double m32 = matrix.Row3.y;
            double m33 = matrix.Row3.z;
            double m34 = matrix.Row3.w;
            double m41 = matrix.Row4.x;
            double m42 = matrix.Row4.y;
            double m43 = matrix.Row4.z;
            double m44 = matrix.Row4.w;

            result = new double4x4()
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
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 Transform(double4x4 matrix, double4 vector)
        {
            return new double4(
                (matrix.M11 * vector.x) + (matrix.M12 * vector.y) + (matrix.M13 * vector.z) + (matrix.M14 * vector.w),
                (matrix.M21 * vector.x) + (matrix.M22 * vector.y) + (matrix.M23 * vector.z) + (matrix.M24 * vector.w),
                (matrix.M31 * vector.x) + (matrix.M32 * vector.y) + (matrix.M33 * vector.z) + (matrix.M34 * vector.w),
                (matrix.M41 * vector.x) + (matrix.M42 * vector.y) + (matrix.M43 * vector.z) + (matrix.M44 * vector.w));
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 TransformPremult(double4 vector, double4x4 matrix)
        {
            return new double4(
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
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double3"/> instance.</param>
        /// <returns>A new <see cref="double3"/> instance containing the result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 Transform(double4x4 matrix, double3 vector)
        {
            //#if NET5_0_OR_GREATER
            //            double3 result;

            //            if (Sse.IsSupported)
            //            {
            //                TransformSse(in matrix, in vector, out result);
            //            }
            //            else
            //            {
            //                Transform(in matrix, in vector, out result);
            //            }

            //            return result;
            //#else
            Transform(in matrix, in vector, out double3 result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        private static unsafe void TransformSse(in double4x4 matrix, in double3 vector, out double3 result)
        //        {
        //            Vector128<double> row0;
        //            Vector128<double> row1;
        //            Vector128<double> row2;
        //            Vector128<double> row3;

        //            fixed (double* m = &matrix.Row1.x)
        //            {
        //                row0 = Sse.LoadVector128(m + 0);
        //                row1 = Sse.LoadVector128(m + 4);
        //                row2 = Sse.LoadVector128(m + 8);
        //                row3 = Sse.LoadVector128(m + 12);
        //            }

        //            var l12 = Sse.UnpackLow(row0, row1);
        //            var l34 = Sse.UnpackLow(row2, row3);
        //            var h12 = Sse.UnpackHigh(row0, row1);
        //            var h34 = Sse.UnpackHigh(row2, row3);

        //            var col0 = Sse.MoveLowToHigh(l12, l34);
        //            var col1 = Sse.MoveHighToLow(l34, l12);
        //            var col2 = Sse.MoveLowToHigh(h12, h34);
        //            var col3 = Sse.MoveHighToLow(h34, h12);

        //            Vector128<double> vec;

        //            fixed (double* m = &vector.x)
        //            {
        //                vec = Sse.LoadVector128(m);
        //            }

        //            const byte Shuffle_0000 = 0x00;
        //            const byte Shuffle_1111 = 0x55;
        //            const byte Shuffle_2222 = 0xAA;
        //            const byte Shuffle_3333 = 0xFF;

        //            var vX = Sse.Shuffle(vec, vec, Shuffle_0000);
        //            var vY = Sse.Shuffle(vec, vec, Shuffle_1111);
        //            var vZ = Sse.Shuffle(vec, vec, Shuffle_2222);

        //            var res = Sse.Divide(Sse.Add(Sse.Add(Sse.Multiply(Sse.MoveLowToHigh(l12, l34), vX),
        //                                                 Sse.Multiply(Sse.MoveHighToLow(l34, l12), vY)),
        //                                         Sse.Add(Sse.Multiply(Sse.MoveLowToHigh(h12, h34), vZ),
        //                                                 Sse.MoveHighToLow(h34, h12))),
        //                                 Sse.Add(Sse.Add(Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_0000), vX),
        //                                                 Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_1111), vY)),
        //                                         Sse.Add(Sse.Multiply(Sse.Shuffle(row3, row3, Shuffle_2222), vZ),
        //                                                 Sse.Shuffle(row3, row3, Shuffle_3333))));

        //            Unsafe.SkipInit(out result);

        //            fixed (double* r = &result.x)
        //            {
        //                Sse.Store(r + 0, res);
        //            }
        //        }
        //#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Transform(in double4x4 matrix, in double3 vector, out double3 result)
        {
            var mat = matrix;
            var vec = vector;

            double w = (mat.M41 * vec.x) + (mat.M42 * vec.y) + (mat.M43 * vec.z) + mat.M44;

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
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double3"/> instance.</param>
        /// <returns>A new <see cref="double3"/> instance containing the result.</returns>
        public static double3 TransformPremult(double3 vector, double4x4 matrix)
        {
            double w = (matrix.M14 * vector.x) + (matrix.M24 * vector.y) + (matrix.M34 * vector.z) + matrix.M44;
            return new double3(
                ((matrix.M11 * vector.x) + (matrix.M21 * vector.y) + (matrix.M31 * vector.z) + matrix.M41) / w,
                ((matrix.M12 * vector.x) + (matrix.M22 * vector.y) + (matrix.M32 * vector.z) + matrix.M42) / w,
                ((matrix.M13 * vector.x) + (matrix.M23 * vector.y) + (matrix.M33 * vector.z) + matrix.M43) / w);
        }

        /// <summary>
        /// Transforms a given 3D vector by a matrix, and projects the resulting double4 back to a double3.
        /// </summary>
        /// <param name="mat">The desired transformation matrix.</param>
        /// <param name="vec">The given vector.</param>
        /// <returns>The transformed vector.</returns>
        public static double3 TransformPerspective(double4x4 mat, double3 vec)
        {
            var v = new double4(vec, 1.0);
            v = mat * v;
            double3 result = new();

            if (v.w > M.EpsilonDouble)
            {
                result.x = v.x / v.w;
                result.y = v.y / v.w;
                result.z = v.z / v.w;
            }
            else
            {
                result = double3.Zero;
            }

            return result;
        }

        /// <summary>
        /// Transforms the given vector by the given matrix and applies a perspective division.
        /// </summary>
        /// <param name="mat">The desired transformation.</param>
        /// <param name="vec">The given vector.</param>
        /// <returns>The transformed vector.</returns>
        public static double4 TransformPerspective(double4x4 mat, double4 vec)
        {
            double4 tmp = mat * vec;
            return tmp /= tmp.w;
        }

        #endregion Transform

        #region TRS Decomposition

        /// <summary>
        /// Calculates translation of the given double4x4 matrix and returns it as a double3 vector.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double3 GetTranslation(double4x4 mat)
        {
            return new double3(mat.M14, mat.M24, mat.M34);
        }

        /// <summary>
        /// Calculates and returns only the translation component of the given double4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double4x4 TranslationDecomposition(double4x4 mat)
        {
            var translationVec = GetTranslation(mat);
            var translationMtx = double4x4.Identity;

            translationMtx.M14 = translationVec.x;
            translationMtx.M24 = translationVec.y;
            translationMtx.M34 = translationVec.z;

            return translationMtx;
        }

        /// <summary>
        /// Calculates and returns the rotation component of the given double4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double4x4 RotationDecomposition(double4x4 mat)
        {
            var scalevector = GetScale(mat);
            var rotationMtx = Identity;

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
        /// Calculates the scale factor of the given double4x4 and returns it as a double3 vector.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double3 GetScale(double4x4 mat)
        {
            var scale = double3.One;

            scale.x = mat.Column1.Length;
            scale.y = mat.Column2.Length;
            scale.z = mat.Column3.Length;

            return scale;
        }

        /// <summary>
        /// Calculates and returns the scale component of the given double4x4 matrix.
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static double4x4 ScaleDecomposition(double4x4 mat)
        {
            var scalevector = GetScale(mat);
            var scaleMtx = double4x4.Identity;

            scaleMtx.M11 = scalevector.x;
            scaleMtx.M22 = scalevector.y;
            scaleMtx.M33 = scalevector.z;

            return scaleMtx;
        }

        #endregion TRS Decomposition

        #region Round

        /// <summary>
        /// Rounds the given matrix to 6 digits (max double precision).
        /// </summary>
        /// <param name="mat">The matrix to round.</param>
        /// <returns>The rounded matrix.</returns>
        public static double4x4 Round(double4x4 mat)
        {
            return new double4x4(double4.Round(mat.Row1),
                                double4.Round(mat.Row2),
                                double4.Round(mat.Row3),
                                double4.Round(mat.Row4));
        }

        #endregion Round

        #endregion Static

        #region Operators

        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new double4x4 which holds the result of the multiplication</returns>
        public static double4x4 operator +(double4x4 left, double4x4 right)
        {
            return Add(in left, in right);
        }

        /// <summary>
        /// Matrix subtraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new double2x2 which holds the result of the multiplication</returns>
        public static double4x4 operator -(double4x4 left, double4x4 right)
        {
            return Subtract(in left, in right);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static double4x4 operator *(double4x4 left, double4x4 right)
        {
            return Mult(in left, in right);
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(double4x4 left, double4x4 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(double4x4 left, double4x4 right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via matrix*vector (post-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 operator *(double4x4 matrix, double4 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (pre-multiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 operator *(double4 vector, double4x4 matrix)
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
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double3"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double3 operator *(double4x4 matrix, double3 vector)
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
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double3"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double3 operator *(double3 vector, double4x4 matrix)
        {
            return TransformPremult(vector, matrix);
        }

        /// <summary>
        /// Explicit cast operator to cast a double4x4 into a double4x4 value.
        /// </summary>
        /// <param name="d4x4">The double4x4 value to cast.</param>
        /// <returns>A double4x4 value.</returns>
        public static explicit operator double4x4(float4x4 d4x4)
        {
            return new double4x4(d4x4);
        }

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current double4x4.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return ConvertToString(null);
        }

        /// <summary>
        /// Returns a System.String that represents the current double4x4.
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

            return string.Format(provider, "{0}\n{1}\n{2}\n{3}", Row1.ToString(provider), Row2.ToString(provider), Row3.ToString(provider), Row4.ToString(provider));
        }

        #endregion public override string ToString()

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode() ^ Row4.GetHashCode();
        }

        #endregion public override int GetHashCode()

        #region public override bool Equals(object obj)

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare the result.</param>
        /// <returns>True if the instances are equal; false otherwise.</returns>
        public override readonly bool Equals(object? obj)
        {
            if (obj is not double4x4)
                return false;

            return Equals((double4x4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #endregion Public Members

        #region IEquatable<double4x4> Members

        /// <summary>
        /// Checks whether row three (the projection part) of the matrix is equal to (0, 0, 0, 1). If this is the case the matrix is affine.
        /// </summary>       
        public bool IsAffine =>
                // Column order notation
                (Row4 == double4.UnitW);

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">A matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public readonly bool Equals(double4x4 other)
        {
            //#if NET5_0_OR_GREATER
            //            bool result;

            //            if (Sse.IsSupported)
            //            {
            //                EqualsSse(in other, out result);
            //            }
            //            else
            //            {
            //                Equals(in other, out result);
            //            }

            //            return result;
            //#else
            Equals(in other, out bool result);

            return result;
            //#endif
        }

        //#if NET5_0_OR_GREATER
        //        private readonly unsafe void EqualsSse(in double4x4 other, out bool result)
        //        {

        //            Vector128<double> thisrow0;
        //            Vector128<double> thisrow1;
        //            Vector128<double> thisrow2;
        //            Vector128<double> thisrow3;

        //            fixed (double* m = &this.Row1.x)
        //            {
        //                thisrow0 = Sse.LoadVector128(m);
        //                thisrow1 = Sse.LoadVector128(m + 4);
        //                thisrow2 = Sse.LoadVector128(m + 8);
        //                thisrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            Vector128<double> otherrow0;
        //            Vector128<double> otherrow1;
        //            Vector128<double> otherrow2;
        //            Vector128<double> otherrow3;

        //            fixed (double* m = &other.Row1.x)
        //            {
        //                otherrow0 = Sse.LoadVector128(m);
        //                otherrow1 = Sse.LoadVector128(m + 4);
        //                otherrow2 = Sse.LoadVector128(m + 8);
        //                otherrow3 = Sse.LoadVector128(m + 12);
        //            }

        //            result = false;

        //            var e = Vector128.Create(M.EpsilonDouble);

        //            var r = Sse.And(Sse.And(Sse.CompareLessThan(Sse.Subtract(Sse.Max(thisrow0, otherrow0), Sse.Min(thisrow0, otherrow0)), e),
        //                                    Sse.CompareLessThan(Sse.Subtract(Sse.Max(thisrow1, otherrow1), Sse.Min(thisrow1, otherrow1)), e)),
        //                            Sse.And(Sse.CompareLessThan(Sse.Subtract(Sse.Max(thisrow2, otherrow2), Sse.Min(thisrow2, otherrow2)), e),
        //                                    Sse.CompareLessThan(Sse.Subtract(Sse.Max(thisrow3, otherrow3), Sse.Min(thisrow3, otherrow3)), e)));

        //            if (double.IsNaN(r.GetElement(0)) && double.IsNaN(r.GetElement(1)) && double.IsNaN(r.GetElement(2)) && double.IsNaN(r.GetElement(3)))
        //                result = true;
        //        }
        //#endif

        private readonly void Equals(in double4x4 other, out bool result)
        {
            result = Row1 == other.Row1 &&
                     Row2 == other.Row2 &&
                     Row3 == other.Row3 &&
                     Row4 == other.Row4;
        }

        #endregion IEquatable<double4x4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double4x4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double4x4> ParseConverter { get; set; } = (x => double4x4.Parse(x));

        /// <summary>
        /// Parses a string into a double4x4.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static double4x4 Parse(string source, IFormatProvider? provider = null)
        {
            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            char separator = M.GetNumericListSeparator(provider);

            string[] strings = source.Split(new char[] { separator, '(', ')', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (strings.Length != 16)
                throw new FormatException("String parse for double4x4 did not result in exactly 16 items.");

            double[] doubles = new double[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                try
                {
                    doubles[i] = double.Parse(strings[i], provider);
                }
                catch
                {
                    throw new FormatException();
                }
            }

            return new double4x4(doubles[0], doubles[1], doubles[2], doubles[3], doubles[4], doubles[5], doubles[6], doubles[7], doubles[8], doubles[9], doubles[10], doubles[11], doubles[12], doubles[13], doubles[14], doubles[15]);
        }
    }
}