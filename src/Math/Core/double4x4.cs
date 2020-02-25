using ProtoBuf;
using System;
using System.Runtime.InteropServices;

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
    /// matrix used in homogeneus coordinate calculations can be found in the the leftmost column
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
    /// bigger positive z-coordinates wheras in right-handed coordinate systems positions further away have smaller
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
        public double4 Row0;

        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        [ProtoMember(2)]
        public double4 Row1;

        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        [ProtoMember(3)]
        public double4 Row2;

        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        [ProtoMember(4)]
        public double4 Row3;

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static double4x4 Identity = new double4x4(double4.UnitX, double4.UnitY, double4.UnitZ, double4.UnitW);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static double4x4 Zero = new double4x4(double4.Zero, double4.Zero, double4.Zero, double4.Zero);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="row0">Top row of the matrix</param>
        /// <param name="row1">Second row of the matrix</param>
        /// <param name="row2">Third row of the matrix</param>
        /// <param name="row3">Bottom row of the matrix</param>
        public double4x4(double4 row0, double4 row1, double4 row2, double4 row3)
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
        public double4x4(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            Row0 = new double4(m00, m01, m02, m03);
            Row1 = new double4(m10, m11, m12, m13);
            Row2 = new double4(m20, m21, m22, m23);
            Row3 = new double4(m30, m31, m32, m33);
        }

        #endregion Constructors

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public double Determinant
        {
            get
            {
                return
                    Row0.x * Row1.y * Row2.z * Row3.w - Row0.x * Row1.y * Row2.w * Row3.z + Row0.x * Row1.z * Row2.w * Row3.y -
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
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public double4 Column0
        {
            get { return new double4(Row0.x, Row1.x, Row2.x, Row3.x); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public double4 Column1
        {
            get { return new double4(Row0.y, Row1.y, Row2.y, Row3.y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public double4 Column2
        {
            get { return new double4(Row0.z, Row1.z, Row2.z, Row3.z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public double4 Column3
        {
            get { return new double4(Row0.w, Row1.w, Row2.w, Row3.w); }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 1 of this instance.
        /// </summary>
        public double M11
        {
            get { return Row0.x; }
            set { Row0.x = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 2 of this instance.
        /// </summary>
        public double M12
        {
            get { return Row0.y; }
            set { Row0.y = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 3 of this instance.
        /// </summary>
        public double M13
        {
            get { return Row0.z; }
            set { Row0.z = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 1, column 4 of this instance.
        /// </summary>
        public double M14
        {
            get { return Row0.w; }
            set { Row0.w = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 1 of this instance.
        /// </summary>
        public double M21
        {
            get { return Row1.x; }
            set { Row1.x = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 2 of this instance.
        /// </summary>
        public double M22
        {
            get { return Row1.y; }
            set { Row1.y = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 3 of this instance.
        /// </summary>
        public double M23
        {
            get { return Row1.z; }
            set { Row1.z = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 2, column 4 of this instance.
        /// </summary>
        public double M24
        {
            get { return Row1.w; }
            set { Row1.w = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 1 of this instance.
        /// </summary>
        public double M31
        {
            get { return Row2.x; }
            set { Row2.x = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 2 of this instance.
        /// </summary>
        public double M32
        {
            get { return Row2.y; }
            set { Row2.y = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 3 of this instance.
        /// </summary>
        public double M33
        {
            get { return Row2.z; }
            set { Row2.z = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 3, column 4 of this instance.
        /// </summary>
        public double M34
        {
            get { return Row2.w; }
            set { Row2.w = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 1 of this instance.
        /// </summary>
        public double M41
        {
            get { return Row3.x; }
            set { Row3.x = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 2 of this instance.
        /// </summary>
        public double M42
        {
            get { return Row3.y; }
            set { Row3.y = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 3 of this instance.
        /// </summary>
        public double M43
        {
            get { return Row3.z; }
            set { Row3.z = value; }
        }

        /// <summary>
        /// Gets and sets the value at row 4, column 4 of this instance.
        /// </summary>
        public double M44
        {
            get { return Row3.w; }
            set { Row3.w = value; }
        }

        /// <summary>
        /// Gets the offset part of the matrix as a <see cref="double3"/> instance.
        /// </summary>
        /// <remarks>
        /// The offset part of the matrix consists of the M14, M24 and M34 components (in row major order notation).
        /// </remarks>
        public double3 Offset
        {
            get { return new double3(Row0.w, Row1.w, Row2.w); }
            // No setter here - might be too confusing
        }

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
                        throw new ArgumentOutOfRangeException($"Index {i},{j} not eligible for a double4x4 type");
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

        #endregion Instance

        #region Static

        #region CreateFromAxisAngle

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static double4x4 CreateFromAxisAngle(double3 axis, double angle)
        {
            double4x4 result;

            var cos = System.Math.Cos(-angle);
            var sin = System.Math.Sin(-angle);
            var t = 1.0 - cos;

            axis = axis.Normalize();

            result = new double4x4(t * axis.x * axis.x + cos, t * axis.x * axis.y + sin * axis.z, t * axis.x * axis.z - sin * axis.y, 0.0,
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
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationX(double angle)
        {
            double4x4 result;

            var cos = (double)System.Math.Cos(angle);
            var sin = (double)System.Math.Sin(angle);

            result.Row0 = double4.UnitX;
            result.Row1 = new double4(0.0f, cos, -sin, 0.0f);
            result.Row2 = new double4(0.0f, sin, cos, 0.0f);
            result.Row3 = double4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationY(double angle)
        {
            double4x4 result;

            var cos = (double)System.Math.Cos(angle);
            var sin = (double)System.Math.Sin(angle);

            result.Row0 = new double4(cos, 0.0f, sin, 0.0f);
            result.Row1 = double4.UnitY;
            result.Row2 = new double4(-sin, 0.0f, cos, 0.0f);
            result.Row3 = double4.UnitW;

            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationZ(double angle)
        {
            double4x4 result;

            var cos = (double)System.Math.Cos(angle);
            var sin = (double)System.Math.Sin(angle);

            result.Row0 = new double4(cos, -sin, 0.0f, 0.0f);
            result.Row1 = new double4(sin, cos, 0.0f, 0.0f);
            result.Row2 = double4.UnitZ;
            result.Row3 = double4.UnitW;

            return result;
        }

        #endregion CreateRotation[XYZ]

        #region CreateTranslation

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
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
        /// <returns>The resulting Matrix4 instance.</returns>
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
            var cy = System.Math.Sqrt(System.Math.Pow(mat[i][i], 2.0) + System.Math.Pow(mat[i][j], 2.0));

            var FLT_EPSILON = 1.192092896e-07F;

            if (cy > 16.0f * FLT_EPSILON)
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

        /// <summary>
        /// Takes a rotation matrix and returns its euler angle representation as a double3.
        /// </summary>
        /// <param name="rotMat">The given rotation matrix.</param>
        /// <returns> The euler representation as a double3. </returns>
        //see Blender mathutils and Graphic Gems IV p. 222-229
        public static double3 RotMatToEuler(double4x4 rotMat)
        {
            //Matrix is beeing handled as a multidimentional array to ensure that the rotation order can be changed easily in the future.
            var m = new[] { rotMat.Row0.ToArray(), rotMat.Row1.ToArray(), rotMat.Row2.ToArray(), rotMat.Row3.ToArray() };

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

        #region CreateOrthographic

        /// <summary>
        /// Creates a left handed orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
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
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateOrthographicOffCenterRH(double left, double right, double bottom, double top, double zNear,
                                                         double zFar)
        {
            double4x4 result = new double4x4();

            double invRL = 1 / (right - left);
            double invTB = 1 / (top - bottom);
            double invFN = 1 / (zFar - zNear);

            // Column order notation
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
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear,
                                                           double zFar)
        {
            double4x4 result = new double4x4();

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
        /// <item>fovy is zero, less than zero or larger than Math.PI</item>
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
                throw new ArgumentOutOfRangeException("fovy");
            if (aspect <= 0)
                throw new ArgumentOutOfRangeException("aspect");
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            double yMax = zNear * (double)System.Math.Tan(0.5f * fovy);
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
        /// <remarks>Generates a matrix mapping a frustum shaped volume (the viewing frustum) to
        /// the unit cube (ranging from -1 to 1 in each dimension, also in z). The sign of the z-value will be
        /// flipped for vectors multiplied with this matrix. Given that the underlying rendering platform
        /// interprets z-values returned by the vertex shader to be in left-handed coordinates, where increasing
        /// z-values indicate locations further away from the view point (as BOTH, Direct3D AND OpenGL do), this
        /// type of matrix is widely called to be a "right handed" projection matrix as it assumes a right-handed
        /// camera coordinate system.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static double4x4 CreatePerspectiveOffCenterRH(double left, double right, double bottom, double top, double zNear,
                                                      double zFar)
        {
            double4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            double x = (2.0f * zNear) / (right - left);
            double y = (2.0f * zNear) / (top - bottom);
            // Right handed
            double a = (right + left) / (right - left);
            double b = (top + bottom) / (top - bottom);
            double c = -(zFar + zNear) / (zFar - zNear);
            double d = -(2.0f * zFar * zNear) / (zFar - zNear);

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
        public static double4x4 CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear,
                                                          double zFar)
        {
            double4x4 result;

            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            double x = (2.0f * zNear) / (right - left);
            double y = (2.0f * zNear) / (top - bottom);
            // Left Handed
            double a = (left + right) / (left - right);
            double b = (top + bottom) / (bottom - top);
            double c = (zFar + zNear) / (zFar - zNear);
            double d = -(2.0f * zFar * zNear) / (zFar - zNear);

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
            result.Row0 = double4.UnitX * x;
            result.Row1 = double4.UnitY * y;
            result.Row2 = double4.UnitZ * z;
            result.Row3 = double4.UnitW;
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
        /// <returns>A Matrix4 that transforms world space to camera space</returns>
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
        /// <returns>A Matrix4 that transforms world space to camera space</returns>
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
        public static double4x4 Add(double4x4 left, double4x4 right)
        {
            return new double4x4(left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13, left.M14 + right.M14,
                                left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23, left.M24 + right.M24,
                                left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33, left.M34 + right.M34,
                                left.M41 + right.M41, left.M42 + right.M42, left.M43 + right.M43, left.M44 + right.M44);
        }

        /// <summary>
        /// Substracts the right instance from the left instance.
        /// </summary>
        /// <param name="left">The left operand of the substraction.</param>
        /// <param name="right">The right operand of the substraction.</param>
        /// <returns>A new instance that is the result of the substraction.</returns>
        public static double4x4 Substract(double4x4 left, double4x4 right)
        {
            return new double4x4(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M14 - right.M14,
                                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M24 - right.M24,
                                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33, left.M34 - right.M34,
                                left.M41 - right.M41, left.M42 - right.M42, left.M43 - right.M43, left.M44 - right.M44);
        }

        #endregion Elementary Arithmetic Functions

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static double4x4 Mult(double4x4 left, double4x4 right)
        {
            if (left == Identity) return right;
            if (right == Identity) return left;
            if (left == Zero || right == Zero) return Zero;

            double4x4 result;

            if (left.IsAffine && right.IsAffine)
                result = new double4x4(
                    left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
                    left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
                    left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,
                    left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14,

                    left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
                    left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
                    left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,
                    left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24,

                    left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
                    left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
                    left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33,
                    left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34,

                    0, 0, 0, 1);
            else
                result = new double4x4(
                    left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41,
                    left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42,
                    left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43,
                    left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44,

                    left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41,
                    left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42,
                    left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43,
                    left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44,

                    left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41,
                    left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42,
                    left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43,
                    left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44,

                    left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41,
                    left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42,
                    left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43,
                    left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44);

            return result;
        }

        #endregion Multiply Functions

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix.
        /// </summary>
        /// <param name="mat">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular.</returns>
        public static double4x4 Invert(double4x4 mat)
        {
            if (mat == Identity || mat == Zero) return mat;
            if (mat.IsAffine) return InvertAffine(mat);

            mat = mat.Transpose();

            var tmp0 = mat.M33 * mat.M44;
            var tmp1 = mat.M34 * mat.M43;
            var tmp2 = mat.M32 * mat.M44;
            var tmp3 = mat.M34 * mat.M42;
            var tmp4 = mat.M32 * mat.M43;
            var tmp5 = mat.M33 * mat.M42;
            var tmp6 = mat.M31 * mat.M44;
            var tmp7 = mat.M34 * mat.M41;
            var tmp8 = mat.M31 * mat.M43;
            var tmp9 = mat.M33 * mat.M41;
            var tmp10 = mat.M31 * mat.M42;
            var tmp11 = mat.M32 * mat.M41;

            // calculate first 8 elements (cofactors)
            var m11 = tmp0 * mat.M22 + tmp3 * mat.M23 + tmp4 * mat.M24;
            m11 -= tmp1 * mat.M22 + tmp2 * mat.M23 + tmp5 * mat.M24;
            var m12 = tmp1 * mat.M21 + tmp6 * mat.M23 + tmp9 * mat.M24;
            m12 -= tmp0 * mat.M21 + tmp7 * mat.M23 + tmp8 * mat.M24;
            var m13 = tmp2 * mat.M21 + tmp7 * mat.M22 + tmp10 * mat.M24;
            m13 -= tmp3 * mat.M21 + tmp6 * mat.M22 + tmp11 * mat.M24;
            var m14 = tmp5 * mat.M21 + tmp8 * mat.M22 + tmp11 * mat.M23;
            m14 -= tmp4 * mat.M21 + tmp9 * mat.M22 + tmp10 * mat.M23;
            var m21 = tmp1 * mat.M12 + tmp2 * mat.M13 + tmp5 * mat.M14;
            m21 -= tmp0 * mat.M12 + tmp3 * mat.M13 + tmp4 * mat.M14;
            var m22 = tmp0 * mat.M11 + tmp7 * mat.M13 + tmp8 * mat.M14;
            m22 -= tmp1 * mat.M11 + tmp6 * mat.M13 + tmp9 * mat.M14;
            var m23 = tmp3 * mat.M11 + tmp6 * mat.M12 + tmp11 * mat.M14;
            m23 -= tmp2 * mat.M11 + tmp7 * mat.M12 + tmp10 * mat.M14;
            var m24 = tmp4 * mat.M11 + tmp9 * mat.M12 + tmp10 * mat.M13;
            m24 -= tmp5 * mat.M11 + tmp8 * mat.M12 + tmp11 * mat.M13;

            // calculate pairs for second 8 elements (cofactors)
            tmp0 = mat.M13 * mat.M24;
            tmp1 = mat.M14 * mat.M23;
            tmp2 = mat.M12 * mat.M24;
            tmp3 = mat.M14 * mat.M22;
            tmp4 = mat.M12 * mat.M23;
            tmp5 = mat.M13 * mat.M22;
            tmp6 = mat.M11 * mat.M24;
            tmp7 = mat.M14 * mat.M21;
            tmp8 = mat.M11 * mat.M23;
            tmp9 = mat.M13 * mat.M21;
            tmp10 = mat.M11 * mat.M22;
            tmp11 = mat.M12 * mat.M21;

            // calculate second 8 elements (cofactors)
            var m31 = tmp0 * mat.M42 + tmp3 * mat.M43 + tmp4 * mat.M44;
            m31 -= tmp1 * mat.M42 + tmp2 * mat.M43 + tmp5 * mat.M44;
            var m32 = tmp1 * mat.M41 + tmp6 * mat.M43 + tmp9 * mat.M44;
            m32 -= tmp0 * mat.M41 + tmp7 * mat.M43 + tmp8 * mat.M44;
            var m33 = tmp2 * mat.M41 + tmp7 * mat.M42 + tmp10 * mat.M44;
            m33 -= tmp3 * mat.M41 + tmp6 * mat.M42 + tmp11 * mat.M44;
            var m34 = tmp5 * mat.M41 + tmp8 * mat.M42 + tmp11 * mat.M43;
            m34 -= tmp4 * mat.M41 + tmp9 * mat.M42 + tmp10 * mat.M43;
            var m41 = tmp2 * mat.M33 + tmp5 * mat.M34 + tmp1 * mat.M32;
            m41 -= tmp4 * mat.M34 + tmp0 * mat.M32 + tmp3 * mat.M33;
            var m42 = tmp8 * mat.M34 + tmp0 * mat.M31 + tmp7 * mat.M33;
            m42 -= tmp6 * mat.M33 + tmp9 * mat.M34 + tmp1 * mat.M31;
            var m43 = tmp6 * mat.M32 + tmp11 * mat.M34 + tmp3 * mat.M31;
            m43 -= tmp10 * mat.M34 + tmp2 * mat.M31 + tmp7 * mat.M32;
            var m44 = tmp10 * mat.M33 + tmp4 * mat.M31 + tmp9 * mat.M32;
            m44 -= tmp8 * mat.M32 + tmp11 * mat.M33 + tmp5 * mat.M31;

            // calculate determinant
            var det = mat.M11 * m11 + mat.M12 * m12 + mat.M13 * m13 + mat.M14 * m14;

            if (det > M.EpsilonFloat || det < -M.EpsilonFloat)
            {
                det = 1 / det;

                mat = new double4x4(det * m11, det * m12, det * m13, det * m14,
                                   det * m21, det * m22, det * m23, det * m24,
                                   det * m31, det * m32, det * m33, det * m34,
                                   det * m41, det * m42, det * m43, det * m44);
            }
            else
                mat = mat.Transpose();

            return mat;
        }

        /// <summary>
        /// Calculate the inverse of a given matrix which represents an affine transformation.
        /// </summary>
        /// <param name="mat">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix.</returns>
        public static double4x4 InvertAffine(double4x4 mat)
        {
            // Row order notation
            //var val1 = -(mat.M11*mat.M41 + mat.M12*mat.M42 + mat.M13*mat.M43);
            //var val2 = -(mat.M21*mat.M41 + mat.M22*mat.M42 + mat.M23*mat.M43);
            //var val3 = -(mat.M31*mat.M41 + mat.M32*mat.M42 + mat.M33*mat.M43);

            //return
            //    new double4x4(new double4(mat.M11, mat.M21, mat.M31, 0),
            //                 new double4(mat.M12, mat.M22, mat.M32, 0),
            //                 new double4(mat.M13, mat.M23, mat.M33, 0),
            //                 new double4(val1, val2, val3, 1));

            // Column order notation ???
            var val1 = -(mat.M11 * mat.M14 + mat.M21 * mat.M24 + mat.M31 * mat.M34);
            var val2 = -(mat.M12 * mat.M14 + mat.M22 * mat.M24 + mat.M32 * mat.M34);
            var val3 = -(mat.M13 * mat.M14 + mat.M23 * mat.M24 + mat.M33 * mat.M34);

            return
                new double4x4(new double4(mat.M11, mat.M21, mat.M31, val1),
                             new double4(mat.M12, mat.M22, mat.M32, val2),
                             new double4(mat.M13, mat.M23, mat.M33, val3),
                             new double4(0, 0, 0, 1));
        }

        #endregion Invert Functions

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static double4x4 Transpose(double4x4 mat)
        {
            return new double4x4(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
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
        public static double3 Transform(double4x4 matrix, double3 vector)
        {
            double w = (matrix.M41 * vector.x) + (matrix.M42 * vector.y) + (matrix.M43 * vector.z) + matrix.M44;
            return new double3(
                ((matrix.M11 * vector.x) + (matrix.M12 * vector.y) + (matrix.M13 * vector.z) + matrix.M14) / w,
                ((matrix.M21 * vector.x) + (matrix.M22 * vector.y) + (matrix.M23 * vector.z) + matrix.M24) / w,
                ((matrix.M31 * vector.x) + (matrix.M32 * vector.y) + (matrix.M33 * vector.z) + matrix.M34) / w);
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
        /// Transform a double3 by the given Matrix, and project the resulting double4 back to a double3
        /// </summary>
        /// <param name="vec">The vector to transform</param>
        /// <param name="mat">The desired transformation</param>
        /// <returns>
        /// The transformed vector
        /// </returns>
        public static double3 TransformPerspective(double4x4 mat, double3 vec)
        {
            double3 result = new double3();

            double4 v = new double4(vec, 1);
            v = mat * v;
            result.x = v.x / v.w;
            result.y = v.y / v.w;
            result.z = v.z / v.w;

            return result;
        }

        #endregion Transform

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
            return Add(left, right);
        }

        /// <summary>
        /// Matrix substraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new double2x2 which holds the result of the multiplication</returns>
        public static double4x4 operator -(double4x4 left, double4x4 right)
        {
            return Substract(left, right);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static double4x4 operator *(double4x4 left, double4x4 right)
        {
            return Mult(left, right);
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
        /// Transforms a given vector by a matrix via matrix*vector (Postmultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 operator *(double4x4 matrix, double4 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 operator *(double4 vector, double4x4 matrix)
        {
            return TransformPremult(vector, matrix);
        }

        /// <summary>
        /// Transforms a given threedimensional vector by a matrix via matrix*vector (Postmultiplication of the vector).
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
        /// Transforms a given threedimensional vector by a matrix via vector*matrix (Premultiplication of the vector).
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

        #endregion Operators

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix44.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
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
            if (!(obj is double4x4))
                return false;

            return Equals((double4x4)obj);
        }

        #endregion public override bool Equals(object obj)

        #endregion Overrides

        #endregion Public Members

        #region IEquatable<Matrix4> Members

        /// <summary>
        /// Indicates whether the current matrix represents an affine transformation.
        /// </summary>
        /// <returns>true if the current matrix represents an affine transformation; otherwise, false.</returns>
        public bool IsAffine
        {
            get
            {
                // Row order notation
                // return (Column3 == double4.UnitW);

                // Column order notation
                return (Row3 == double4.UnitW);
            }
        }

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">A matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(double4x4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion IEquatable<Matrix4> Members

        /// <summary>
        /// Gets and sets the Converter object. Has the ability to convert a string to a double4x4.
        /// </summary>
        /// <value>
        /// The parse property.
        /// </value>
        public static Converter<string, double4x4> Parse { get; set; }
    }
}