using System;
using System.Runtime.InteropServices;
using ProtoBuf;

namespace Fusee.Math
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
    /// matrix used in homogeneus coordinate calculations can be found in the the leftmost column 
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
    /// bigger positive z-coordinates wheras in right-handed coordinate systems positions further away have smaller
    /// negative z-coordinates. By default, float4x4 will assume a left-handed coordinate system, but contains
    /// convenience construction methods to also create right-handed matrices if necessary. The right-handed versions
    /// of methods are postfixed with "RH".
    /// </para>
    /// </remarks> 
    [Serializable]
    [ProtoContract]
    [StructLayout(LayoutKind.Sequential)]
// ReSharper disable InconsistentNaming
    public struct float4x4 : IEquatable<float4x4>
// ReSharper restore InconsistentNaming
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

        #endregion

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

        #endregion

        #region Public Members

        #region Properties

        /// <summary>
        /// The determinant of this matrix
        /// </summary>
        public float Determinant
        {
            get
            {
                return
                    Row0.x*Row1.y*Row2.z*Row3.w - Row0.x*Row1.y*Row2.w*Row3.z + Row0.x*Row1.z*Row2.w*Row3.y -
                    Row0.x*Row1.z*Row2.y*Row3.w
                    + Row0.x*Row1.w*Row2.y*Row3.z - Row0.x*Row1.w*Row2.z*Row3.y - Row0.y*Row1.z*Row2.w*Row3.x +
                    Row0.y*Row1.z*Row2.x*Row3.w
                    - Row0.y*Row1.w*Row2.x*Row3.z + Row0.y*Row1.w*Row2.z*Row3.x - Row0.y*Row1.x*Row2.z*Row3.w +
                    Row0.y*Row1.x*Row2.w*Row3.z
                    + Row0.z*Row1.w*Row2.x*Row3.y - Row0.z*Row1.w*Row2.y*Row3.x + Row0.z*Row1.x*Row2.y*Row3.w -
                    Row0.z*Row1.x*Row2.w*Row3.y
                    + Row0.z*Row1.y*Row2.w*Row3.x - Row0.z*Row1.y*Row2.x*Row3.w - Row0.w*Row1.x*Row2.y*Row3.z +
                    Row0.w*Row1.x*Row2.z*Row3.y
                    - Row0.w*Row1.y*Row2.z*Row3.x + Row0.w*Row1.y*Row2.x*Row3.z - Row0.w*Row1.z*Row2.x*Row3.y +
                    Row0.w*Row1.z*Row2.y*Row3.x;
            }
        }

        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public float4 Column0
        {
            get { return new float4(Row0.x, Row1.x, Row2.x, Row3.x); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public float4 Column1
        {
            get { return new float4(Row0.y, Row1.y, Row2.y, Row3.y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public float4 Column2
        {
            get { return new float4(Row0.z, Row1.z, Row2.z, Row3.z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public float4 Column3
        {
            get { return new float4(Row0.w, Row1.w, Row2.w, Row3.w); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11
        {
            get { return Row0.x; }
            set { Row0.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12
        {
            get { return Row0.y; }
            set { Row0.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13
        {
            get { return Row0.z; }
            set { Row0.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public float M14
        {
            get { return Row0.w; }
            set { Row0.w = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21
        {
            get { return Row1.x; }
            set { Row1.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22
        {
            get { return Row1.y; }
            set { Row1.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23
        {
            get { return Row1.z; }
            set { Row1.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public float M24
        {
            get { return Row1.w; }
            set { Row1.w = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31
        {
            get { return Row2.x; }
            set { Row2.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32
        {
            get { return Row2.y; }
            set { Row2.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33
        {
            get { return Row2.z; }
            set { Row2.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public float M34
        {
            get { return Row2.w; }
            set { Row2.w = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public float M41
        {
            get { return Row3.x; }
            set { Row3.x = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public float M42
        {
            get { return Row3.y; }
            set { Row3.y = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public float M43
        {
            get { return Row3.z; }
            set { Row3.z = value; }
        }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public float M44
        {
            get { return Row3.w; }
            set { Row3.w = value; }
        }

        /// <summary>
        /// Gets the offset part of the matrix as a <see cref="float3"/> instance.
        /// </summary>
        /// <remarks>
        /// The offset part of the matrix consists of the M14, M24 and M34 components (in row major order notation).
        /// </remarks>
        public float3 Offset
        {
            get { return new float3(Row0.w, Row1.w, Row2.w); }
            // No setter here - might be too confusing
        }

        #endregion

        #region Instance

        #region public void Invert()

        /// <summary>
        /// Converts this instance into its inverse.
        /// </summary>
        public void Invert()
        {
            this = Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

        #region float[] ToArray()

// ReSharper disable UnusedMember.Local
        private float[] ToArray()
        {
            return new[] {M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44};
            // return new float[] { M11, M21, M31, M41, M12, M22, M32, M42, M13, M23, M33, M43, M14, M24, M34, M44 };
        }
// ReSharper restore UnusedMember.Local

        #endregion

        #endregion

        #region public void Round()

        /// <summary>
        /// Rounds this instance to 6 digits (max float precision).
        /// </summary>
        public void Round()
        {
            Row0.Round();
            Row1.Round();
            Row2.Round();
            Row3.Round();
        }

        #endregion

        #endregion

        #region Static

        #region CreateFromAxisAngle

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <param name="result">A matrix instance.</param>
        public static void CreateFromAxisAngle(float3 axis, float angle, out float4x4 result)
        {
            var cos = (float) System.Math.Cos(-angle);
            var sin = (float) System.Math.Sin(-angle);
            var t = 1.0f - cos;

            axis.Normalize();

            //// Row vector notation:
            //result = new float4x4(t*axis.x*axis.x + cos,         t*axis.x*axis.y - sin*axis.z,  t*axis.x*axis.z + sin*axis.y,  0.0f,
            //                      t*axis.x*axis.y + sin*axis.z,  t*axis.y*axis.y + cos,         t*axis.y*axis.z - sin*axis.x,  0.0f,
            //                      t*axis.x*axis.z - sin*axis.y,  t*axis.y*axis.z + sin*axis.x,  t*axis.z*axis.z + cos,         0.0f,
            //                      0,                             0,                             0,                             1);
        
            // Column vector notation:
            result = new float4x4(t*axis.x*axis.x + cos,         t*axis.x*axis.y + sin*axis.z,  t*axis.x*axis.z - sin*axis.y,  0.0f,
                                  t*axis.x*axis.y - sin*axis.z,  t*axis.y*axis.y + cos,         t*axis.y*axis.z + sin*axis.x,  0.0f,
                                  t*axis.x*axis.z + sin*axis.y,  t*axis.y*axis.z - sin*axis.x,  t*axis.z*axis.z + cos,         0.0f,
                                  0.0f,                          0.0f,                          0.0f,                          1.0f);

        }

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static float4x4 CreateFromAxisAngle(float3 axis, float angle)
        {
            float4x4 result;
            CreateFromAxisAngle(axis, angle, out result);
            return result;
        }

        #endregion

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateRotationX(float angle, out float4x4 result)
        {
            var cos = (float) System.Math.Cos(angle);
            var sin = (float) System.Math.Sin(angle);

            // Row vector notation
            //result.Row0 = float4.UnitX;
            //result.Row1 = new float4(0.0f, cos, sin, 0.0f);
            //result.Row2 = new float4(0.0f, -sin, cos, 0.0f);
            //result.Row3 = float4.UnitW;

            // Column vector notation
            result.Row0 = float4.UnitX;
            result.Row1 = new float4(0.0f, cos, -sin, 0.0f);
            result.Row2 = new float4(0.0f, sin,  cos, 0.0f);
            result.Row3 = float4.UnitW;
        
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis. Only for testing and debugging purposes during the 
        /// row notation -> column notation code transition of the math library.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static void CreateRotationX_ROW(float angle, out float4x4 result)
        {
            var cos = (float)System.Math.Cos(angle);
            var sin = (float)System.Math.Sin(angle);

            // Row vector notation
            result.Row0 = float4.UnitX;
            result.Row1 = new float4(0.0f, cos, sin, 0.0f);
            result.Row2 = new float4(0.0f, -sin, cos, 0.0f);
            result.Row3 = float4.UnitW;
        }


        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationX(float angle)
        {
            float4x4 result;
            CreateRotationX(angle, out result);
            return result;
        }


        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis. Only for testing and debugging purposes during the 
        /// row notation -> column notation code transition of the math library.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 CreateRotationX_ROW(float angle)
        {
            float4x4 result;
            CreateRotationX_ROW(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateRotationY(float angle, out float4x4 result)
        {
            var cos = (float) System.Math.Cos(angle);
            var sin = (float) System.Math.Sin(angle);

            // Row vector notation
            //result.Row0 = new float4(cos, 0.0f, -sin, 0.0f);
            //result.Row1 = float4.UnitY;
            //result.Row2 = new float4(sin, 0.0f, cos, 0.0f);
            //result.Row3 = float4.UnitW;

            // Column vector notation
            result.Row0 = new float4(cos, 0.0f, sin, 0.0f);
            result.Row1 = float4.UnitY;
            result.Row2 = new float4(-sin, 0.0f, cos, 0.0f);
            result.Row3 = float4.UnitW;
        }


        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis. Only for testing and debugging purposes during the 
        /// row notation -> column notation code transition of the math library.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static void CreateRotationY_ROW(float angle, out float4x4 result)
        {
            var cos = (float)System.Math.Cos(angle);
            var sin = (float)System.Math.Sin(angle);

            // Row vector notation
            result.Row0 = new float4(cos, 0.0f, -sin, 0.0f);
            result.Row1 = float4.UnitY;
            result.Row2 = new float4(sin, 0.0f, cos, 0.0f);
            result.Row3 = float4.UnitW;

        }


        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationY(float angle)
        {
            float4x4 result;
            CreateRotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="angle">the angle.</param>
        /// <returns></returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 CreateRotationY_ROW(float angle)
        {
            float4x4 result;
            CreateRotationY_ROW(angle, out result);
            return result;
        }



        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateRotationZ(float angle, out float4x4 result)
        {
            var cos = (float) System.Math.Cos(angle);
            var sin = (float) System.Math.Sin(angle);

            // Row vector notation
            //result.Row0 = new float4(cos, sin, 0.0f, 0.0f);
            //result.Row1 = new float4(-sin, cos, 0.0f, 0.0f);
            //result.Row2 = float4.UnitZ;
            //result.Row3 = float4.UnitW;

            // Column vector notation
            result.Row0 = new float4(cos, -sin, 0.0f, 0.0f);
            result.Row1 = new float4(sin, cos, 0.0f, 0.0f);
            result.Row2 = float4.UnitZ;
            result.Row3 = float4.UnitW;

        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateRotationZ(float angle)
        {
            float4x4 result;
            CreateRotationZ(angle, out result);
            return result;
        }

        #endregion

        #region CreateTranslation
        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateTranslation(float x, float y, float z, out float4x4 result)
        {
            result = Identity;

            // Row order notation
            // result.Row3 = new float4(x, y, z, 1);
            
            // Column order notation
            result.M14 = x;
            result.M24 = y;
            result.M34 = z;
            
        }
        
        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="result">Translation matrix</param>
        /// <returns></returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static void CreateTranslation_ROW(float x, float y, float z, out float4x4 result)
        {
            result = Identity;

            // Row order notation
            result.Row3 = new float4(x, y, z, 1);
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateTranslation(ref float3 vector, out float4x4 result)
        {
            result = Identity;

            // Row order notation
            // result.Row3 = new float4(vector.x, vector.y, vector.z, 1);

            // Column order notation
            result.M14 = vector.x;
            result.M24 = vector.y;
            result.M34 = vector.z;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateTranslation(float x, float y, float z)
        {
            float4x4 result;
            CreateTranslation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <returns>A matrix</returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 CreateTranslation_ROW(float x, float y, float z)
        {
            float4x4 result;
            CreateTranslation_ROW(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateTranslation(float3 vector)
        {
            float4x4 result;
            CreateTranslation(vector.x, vector.y, vector.z, out result);
            return result;
        }

        #endregion

        #region CreateScale

        /// <summary>
        /// Creates a uniform scale matrix with the same scale value along all three dimensions.
        /// </summary>
        /// <param name="scale">The value to scale about x, y, and z.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateScale(float scale, out float4x4 result)
        {
            result = Identity;
            result.M11 = scale;
            result.M22 = scale;
            result.M33 = scale;
        }

        /// <summary>
        /// Creates a uniform scale matrix with the same scale value along all three dimensions.
        /// </summary>
        /// <param name="scale">The value to scale about x, y, and z.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateScale(float scale)
        {
            float4x4 result;
            CreateScale(scale, out result);
            return result;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="x">X scale.</param>
        /// <param name="y">Y scale.</param>
        /// <param name="z">Z scale.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateScale(float x, float y, float z, out float4x4 result)
        {
            result = Identity;
            result.M11= x;
            result.M22= y;
            result.M33= z;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="vector">The scale vector.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateScale(ref float3 vector, out float4x4 result)
        {
            result = Identity;
            result.M11 = vector.x;
            result.M22 = vector.y;
            result.M33 = vector.z;
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
            float4x4 result;
            CreateScale(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a scale matrix.
        /// </summary>
        /// <param name="vector">The scale vector.</param>
        /// <returns>The resulting float4x4 instance.</returns>
        public static float4x4 CreateScale(float3 vector)
        {
            float4x4 result;
            CreateScale(vector.x, vector.y, vector.z, out result);
            return result;
        }

        #endregion



        #region CreateOrthographic

        /// <summary>
        /// Creates a left handed orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateOrthographic(float width, float height, float zNear, float zFar, out float4x4 result)
        {
            CreateOrthographicOffCenter(-width/2, width/2, -height/2, height/2, zNear, zFar, out result);
        }

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
            float4x4 result;
            CreateOrthographicOffCenter(-width/2, width/2, -height/2, height/2, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates a left handed orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting float4x4 instance.</param>
        public static void CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNear,
                                                       float zFar, out float4x4 result)
        {
            result = new float4x4();

            float invRL = 1 / (right - left);
            float invTB = 1 / (top - bottom);
            float invFN = 1 / (zFar - zNear);

            // Row order notation
            //result.M11 = 2 * invRL;
            //result.M22 = 2 * invTB;
            //result.M33 = 2 * invFN;

            //result.M41 = -(right + left) * invRL;
            //result.M42 = -(top + bottom) * invTB;
            //result.M43 = -(zFar + zNear) * invFN;
            //result.M44 = 1;

            // Column order notation
            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = 2 * invFN;

            result.M14 = -(right + left) * invRL;
            result.M24 = -(top + bottom) * invTB;
            result.M34 = -(zFar + zNear) * invFN;
            result.M44 = 1;
        
        }

        /// <summary>
        /// Creates a right handed orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateOrthographicOffCenterRH(float left, float right, float bottom, float top, float zNear,
                                                         float zFar, out float4x4 result)
        {
            result = new float4x4();

            float invRL = 1/(right - left);
            float invTB = 1/(top - bottom);
            float invFN = 1/(zFar - zNear);

            // Row order notation
            //result.M11 = 2*invRL;
            //result.M22 = 2*invTB;
            //result.M33 = -2*invFN;

            //result.M41 = -(right + left)*invRL;
            //result.M42 = -(top + bottom)*invTB;
            //result.M43 = -(zFar + zNear)*invFN;
            //result.M44 = 1;

            // Column order notation
            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = -2 * invFN;

            result.M14 = -(right + left) * invRL;
            result.M24 = -(top + bottom) * invTB;
            result.M34 = -(zFar + zNear) * invFN;
            result.M44 = 1;
        
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
            float4x4 result;
            CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreatePerspectiveFieldOfView

        /// <summary>
        /// Creates a left handed perspective projection matrix.
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
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
        public static void CreatePerspectiveFieldOfView(float fovy, float aspect, float zNear, float zFar,
                                                        out float4x4 result)
        {
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

            float yMax = zNear*(float) System.Math.Tan(0.5f*fovy);
            float yMin = -yMax;
            float xMin = yMin*aspect;
            float xMax = yMax*aspect;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="fovy">fovy</param>
        /// <param name="aspect">aspect</param>
        /// <param name="zNear">zNear</param>
        /// <param name="zFar">zFar</param>
        /// <param name="result">result</param>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static void CreatePerspectiveFieldOfView_ROW(float fovy, float aspect, float zNear, float zFar,
                                                        out float4x4 result)
        {
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

            CreatePerspectiveOffCenter_ROW(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }


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
            CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
            return result;
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <returns></returns>
        /// <param name="fovy">fovy</param>
        /// <param name="aspect">aspect</param>
        /// <param name="zNear">zNear</param>
        /// <param name="zFar">zFar</param>
        /// <returns>Matrix</returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 CreatePerspectiveFieldOfView_ROW(float fovy, float aspect, float zNear, float zFar)
        {
            float4x4 result;
            CreatePerspectiveFieldOfView_ROW(fovy, aspect, zNear, zFar, out result);
            return result;
        }
        #endregion

        #region CreatePerspectiveOffCenter

        /// <summary>
        /// Creates a left handed perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <remarks>Generates a matrix mapping a frustum shaped volume (the viewing frustum) to
        /// the unit cube (ranging from -1 to 1 in each dimension, also in z). The sign of the z-value is not
        /// flipped. Given that the underlying rendering platform interprets z-values returned by the
        /// vertex shader to be in left-handed coordinates, where increasing z-values indicate locations
        /// further away from the view point (as BOTH, Direct3D AND OpenGL do), this type of matrix is widely
        /// called to be a "left handed" projection matrix as it assumes a left-handed camera coordinate system.</remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear,
                                                      float zFar, out float4x4 result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float x = (2.0f*zNear)/(right - left);
            float y = (2.0f*zNear)/(top - bottom);
            // Left Handed
            float a = (left + right) / (left - right);
            float b = (top + bottom) / (bottom - top);
            float c = (zFar + zNear) / (zFar - zNear);
            float d = -(2.0f * zFar * zNear) / (zFar - zNear);

            // Row order notation
            //result = new float4x4(x, 0, 0, 0,
            //                      0, y, 0, 0,
            //                      a, b, c, 1,
            //                      0, 0, d, 0);

            // Column order notation
            result = new float4x4(x, 0, a, 0,
                                  0, y, b, 0,
                                  0, 0, c, d,
                                  0, 0, 1, 0);
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <param name="bottom">bottom</param>
        /// <param name="top">top</param>
        /// <param name="zNear">zNear</param>
        /// <param name="zFar">zFar</param>
        /// <param name="result">result</param>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static void CreatePerspectiveOffCenter_ROW(float left, float right, float bottom, float top, float zNear,
                                                      float zFar, out float4x4 result)
        {
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

            // Row order notation
            result = new float4x4(x, 0, 0, 0,
                                  0, y, 0, 0,
                                  a, b, c, 1,
                                  0, 0, d, 0);
        }


        /// <summary>
        /// Creates a right handed perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
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
        public static void CreatePerspectiveOffCenterRH(float left, float right, float bottom, float top, float zNear,
                                                      float zFar, out float4x4 result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");

            float x = (2.0f * zNear) / (right - left);
            float y = (2.0f * zNear) / (top - bottom);
            // Right handed
            float a = (right + left)/(right - left);
            float b = (top + bottom)/(top - bottom);
            float c = -(zFar + zNear)/(zFar - zNear);
            float d = -(2.0f*zFar*zNear)/(zFar - zNear);
            // Row order notation
            //result = new float4x4(x, 0, 0,  0,
            //                      0, y, 0,  0,
            //                      a, b, c, -1,
            //                      0, 0, d,  0);
            result = new float4x4(x, 0,  a, 0,
                                  0, y,  b, 0,
                                  0, 0,  c, d,
                                  0, 0, -1, 0);
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
        public static float4x4 CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float zNear,
                                                          float zFar)
        {
            float4x4 result;
            CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion

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
            result.Row0 = float4.UnitX*x;
            result.Row1 = float4.UnitY*y;
            result.Row2 = float4.UnitZ*z;
            result.Row3 = float4.UnitW;
            return result;
        }

        #endregion


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
                                0,   0,   0,   1);
        }

        /// <summary>
        /// Only for testing and debugging purposes during the row notation -> column notation code transition of the math library
        /// </summary>
        /// <param name="eye">eye</param>
        /// <param name="target">target</param>
        /// <param name="up">up</param>
        /// <returns>A matrix.</returns>
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 LookAt_ROW(float3 eye, float3 target, float3 up)
        {
            var z = float3.Normalize(target - eye);
            var x = float3.Normalize(float3.Cross(up, z));
            var y = float3.Cross(z, x);

            // Row order notation
            return new float4x4(new float4(x.x, y.x, z.x, 0),
                                new float4(x.y, y.y, z.y, 0),
                                new float4(x.z, y.z, z.z, 0),
                                new float4(-float3.Dot(x, eye), -float3.Dot(y, eye), -float3.Dot(z, eye), 1));

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

        /// <summary>
        /// Build a row notation world space to camera space matrix. Only for testing and debugging purposes during the 
        /// row notation -> column notation code transition of the math library.
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
        [Obsolete("Only for testing and debugging purposes during the row notation -> column notation code transition of the math library")]
        public static float4x4 LookAt_ROW(float eyeX, float eyeY, float eyeZ, float targetX, float targetY, float targetZ,
                                      float upX, float upY, float upZ)
        {
            return LookAt_ROW(new float3(eyeX, eyeY, eyeZ), new float3(targetX, targetY, targetZ), new float3(upX, upY, upZ));
        }
        
        
        #endregion

        #region Elementary Arithmetic Functions

        /// <summary>
        /// Adds two instances.
        /// </summary>
        /// <param name="left">The left operand of the addition.</param>
        /// <param name="right">The right operand of the addition.</param>
        /// <returns>A new instance that is the result of the addition.</returns>
        public static float4x4 Add(float4x4 left, float4x4 right)
        {
            return new float4x4(left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13, left.M14 + right.M14,
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
        public static float4x4 Substract(float4x4 left, float4x4 right)
        {
            return new float4x4(left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13, left.M14 - right.M14,
                                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23, left.M24 - right.M24,
                                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33, left.M34 - right.M34,
                                left.M41 - right.M41, left.M42 - right.M42, left.M43 - right.M43, left.M44 - right.M44);
        }

        #endregion

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static float4x4 Mult(float4x4 left, float4x4 right)
        {
            if (left == Identity) return right;
            if (right == Identity) return left;
            if (left == Zero || right == Zero) return Zero;

            float4x4 result;

            if (left.IsAffine && right.IsAffine)
                MultAffine(ref left, ref right, out result);
            else
                Mult(ref left, ref right, out result);

            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref float4x4 left, ref float4x4 right, out float4x4 result)
        {
            result = new float4x4(
                left.M11*right.M11 + left.M12*right.M21 + left.M13*right.M31 + left.M14*right.M41,
                left.M11*right.M12 + left.M12*right.M22 + left.M13*right.M32 + left.M14*right.M42,
                left.M11*right.M13 + left.M12*right.M23 + left.M13*right.M33 + left.M14*right.M43,
                left.M11*right.M14 + left.M12*right.M24 + left.M13*right.M34 + left.M14*right.M44,

                left.M21*right.M11 + left.M22*right.M21 + left.M23*right.M31 + left.M24*right.M41,
                left.M21*right.M12 + left.M22*right.M22 + left.M23*right.M32 + left.M24*right.M42,
                left.M21*right.M13 + left.M22*right.M23 + left.M23*right.M33 + left.M24*right.M43,
                left.M21*right.M14 + left.M22*right.M24 + left.M23*right.M34 + left.M24*right.M44,

                left.M31*right.M11 + left.M32*right.M21 + left.M33*right.M31 + left.M34*right.M41,
                left.M31*right.M12 + left.M32*right.M22 + left.M33*right.M32 + left.M34*right.M42,
                left.M31*right.M13 + left.M32*right.M23 + left.M33*right.M33 + left.M34*right.M43,
                left.M31*right.M14 + left.M32*right.M24 + left.M33*right.M34 + left.M34*right.M44,

                left.M41*right.M11 + left.M42*right.M21 + left.M43*right.M31 + left.M44*right.M41,
                left.M41*right.M12 + left.M42*right.M22 + left.M43*right.M32 + left.M44*right.M42,
                left.M41*right.M13 + left.M42*right.M23 + left.M43*right.M33 + left.M44*right.M43,
                left.M41*right.M14 + left.M42*right.M24 + left.M43*right.M34 + left.M44*right.M44);
        }

        /// <summary>
        /// Multiplies two instances if both matrices represents affine transformations.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void MultAffine(ref float4x4 left, ref float4x4 right, out float4x4 result)
        {
            // Row order notation
            //result = new float4x4(
            //    left.M11*right.M11 + left.M12*right.M21 + left.M13*right.M31,
            //    left.M11*right.M12 + left.M12*right.M22 + left.M13*right.M32,
            //    left.M11*right.M13 + left.M12*right.M23 + left.M13*right.M33,
            //    0f,

            //    left.M21*right.M11 + left.M22*right.M21 + left.M23*right.M31,
            //    left.M21*right.M12 + left.M22*right.M22 + left.M23*right.M32,
            //    left.M21*right.M13 + left.M22*right.M23 + left.M23*right.M33,
            //    0f,

            //    left.M31*right.M11 + left.M32*right.M21 + left.M33*right.M31,
            //    left.M31*right.M12 + left.M32*right.M22 + left.M33*right.M32,
            //    left.M31*right.M13 + left.M32*right.M23 + left.M33*right.M33,
            //    0f,

            //    left.M41*right.M11 + left.M42*right.M21 + left.M43*right.M31 + right.M41,
            //    left.M41*right.M12 + left.M42*right.M22 + left.M43*right.M32 + right.M42,
            //    left.M41*right.M13 + left.M42*right.M23 + left.M43*right.M33 + right.M43,
            //    1f);

            // Column order notation
            result = new float4x4(
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
        }

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix.
        /// </summary>
        /// <param name="mat">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular.</returns>
        public static float4x4 Invert(float4x4 mat)
        {
            if (mat == Identity || mat == Zero) return mat;
            // InvertAffine is broken (probably since column order notation
            // if (mat.IsAffine) return InvertAffine(mat);

            mat.Transpose();

            var tmp0 = mat.M33*mat.M44;
            var tmp1 = mat.M34*mat.M43;
            var tmp2 = mat.M32*mat.M44;
            var tmp3 = mat.M34*mat.M42;
            var tmp4 = mat.M32*mat.M43;
            var tmp5 = mat.M33*mat.M42;
            var tmp6 = mat.M31*mat.M44;
            var tmp7 = mat.M34*mat.M41;
            var tmp8 = mat.M31*mat.M43;
            var tmp9 = mat.M33*mat.M41;
            var tmp10 = mat.M31*mat.M42;
            var tmp11 = mat.M32*mat.M41;

            // calculate first 8 elements (cofactors)
            var m11 = tmp0*mat.M22 + tmp3*mat.M23 + tmp4*mat.M24;
            m11 -= tmp1*mat.M22 + tmp2*mat.M23 + tmp5*mat.M24;
            var m12 = tmp1*mat.M21 + tmp6*mat.M23 + tmp9*mat.M24;
            m12 -= tmp0*mat.M21 + tmp7*mat.M23 + tmp8*mat.M24;
            var m13 = tmp2*mat.M21 + tmp7*mat.M22 + tmp10*mat.M24;
            m13 -= tmp3*mat.M21 + tmp6*mat.M22 + tmp11*mat.M24;
            var m14 = tmp5*mat.M21 + tmp8*mat.M22 + tmp11*mat.M23;
            m14 -= tmp4*mat.M21 + tmp9*mat.M22 + tmp10*mat.M23;
            var m21 = tmp1*mat.M12 + tmp2*mat.M13 + tmp5*mat.M14;
            m21 -= tmp0*mat.M12 + tmp3*mat.M13 + tmp4*mat.M14;
            var m22 = tmp0*mat.M11 + tmp7*mat.M13 + tmp8*mat.M14;
            m22 -= tmp1*mat.M11 + tmp6*mat.M13 + tmp9*mat.M14;
            var m23 = tmp3*mat.M11 + tmp6*mat.M12 + tmp11*mat.M14;
            m23 -= tmp2*mat.M11 + tmp7*mat.M12 + tmp10*mat.M14;
            var m24 = tmp4*mat.M11 + tmp9*mat.M12 + tmp10*mat.M13;
            m24 -= tmp5*mat.M11 + tmp8*mat.M12 + tmp11*mat.M13;

            // calculate pairs for second 8 elements (cofactors)
            tmp0 = mat.M13*mat.M24;
            tmp1 = mat.M14*mat.M23;
            tmp2 = mat.M12*mat.M24;
            tmp3 = mat.M14*mat.M22;
            tmp4 = mat.M12*mat.M23;
            tmp5 = mat.M13*mat.M22;
            tmp6 = mat.M11*mat.M24;
            tmp7 = mat.M14*mat.M21;
            tmp8 = mat.M11*mat.M23;
            tmp9 = mat.M13*mat.M21;
            tmp10 = mat.M11*mat.M22;
            tmp11 = mat.M12*mat.M21;

            // calculate second 8 elements (cofactors)
            var m31 = tmp0*mat.M42 + tmp3*mat.M43 + tmp4*mat.M44;
            m31 -= tmp1*mat.M42 + tmp2*mat.M43 + tmp5*mat.M44;
            var m32 = tmp1*mat.M41 + tmp6*mat.M43 + tmp9*mat.M44;
            m32 -= tmp0*mat.M41 + tmp7*mat.M43 + tmp8*mat.M44;
            var m33 = tmp2*mat.M41 + tmp7*mat.M42 + tmp10*mat.M44;
            m33 -= tmp3*mat.M41 + tmp6*mat.M42 + tmp11*mat.M44;
            var m34 = tmp5*mat.M41 + tmp8*mat.M42 + tmp11*mat.M43;
            m34 -= tmp4*mat.M41 + tmp9*mat.M42 + tmp10*mat.M43;
            var m41 = tmp2*mat.M33 + tmp5*mat.M34 + tmp1*mat.M32;
            m41 -= tmp4*mat.M34 + tmp0*mat.M32 + tmp3*mat.M33;
            var m42 = tmp8*mat.M34 + tmp0*mat.M31 + tmp7*mat.M33;
            m42 -= tmp6*mat.M33 + tmp9*mat.M34 + tmp1*mat.M31;
            var m43 = tmp6*mat.M32 + tmp11*mat.M34 + tmp3*mat.M31;
            m43 -= tmp10*mat.M34 + tmp2*mat.M31 + tmp7*mat.M32;
            var m44 = tmp10*mat.M33 + tmp4*mat.M31 + tmp9*mat.M32;
            m44 -= tmp8*mat.M32 + tmp11*mat.M33 + tmp5*mat.M31;

            // calculate determinant
            var det = mat.M11*m11 + mat.M12*m12 + mat.M13*m13 + mat.M14*m14;

            if (det > MathHelper.EpsilonFloat || det < -MathHelper.EpsilonFloat)
            {
                det = 1/det;

                mat = new float4x4(det*m11, det*m12, det*m13, det*m14,
                                   det*m21, det*m22, det*m23, det*m24,
                                   det*m31, det*m32, det*m33, det*m34,
                                   det*m41, det*m42, det*m43, det*m44);
            }
            else
                mat.Transpose();

            return mat;
        }

        /// <summary>
        /// Calculate the inverse of a given matrix which represents an affine transformation.
        /// </summary>
        /// <param name="mat">The matrix to invert.</param>
        /// <returns>The inverse of the given matrix.</returns>
        public static float4x4 InvertAffine(float4x4 mat)
        {
            // Row order notation
            //var val1 = -(mat.M11*mat.M41 + mat.M12*mat.M42 + mat.M13*mat.M43);
            //var val2 = -(mat.M21*mat.M41 + mat.M22*mat.M42 + mat.M23*mat.M43);
            //var val3 = -(mat.M31*mat.M41 + mat.M32*mat.M42 + mat.M33*mat.M43);

            //return
            //    new float4x4(new float4(mat.M11, mat.M21, mat.M31, 0),
            //                 new float4(mat.M12, mat.M22, mat.M32, 0),
            //                 new float4(mat.M13, mat.M23, mat.M33, 0),
            //                 new float4(val1, val2, val3, 1));


            throw new ApplicationException("InvertAffine is broken (probably since column order notation)");
            // TODO: fix this!
            //  Column order notation ???
            var val1 = -(mat.M11 * mat.M14 + mat.M21 * mat.M24 + mat.M31 * mat.M34);
            var val2 = -(mat.M12 * mat.M14 + mat.M22 * mat.M24 + mat.M32 * mat.M34);
            var val3 = -(mat.M13 * mat.M14 + mat.M23 * mat.M24 + mat.M33 * mat.M34);

            return
                new float4x4(new float4(mat.M11, mat.M21, mat.M31, val1),
                             new float4(mat.M12, mat.M22, mat.M32, val2),
                             new float4(mat.M13, mat.M23, mat.M33, val3),
                             new float4(0,       0,       0,       1));
        
        }

        #endregion

        #region Transpose

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static float4x4 Transpose(float4x4 mat)
        {
            return new float4x4(mat.Column0, mat.Column1, mat.Column2, mat.Column3);
        }


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref float4x4 mat, out float4x4 result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

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
                (matrix.M11*vector.x) + (matrix.M12*vector.y) + (matrix.M13*vector.z) + (matrix.M14*vector.w),
                (matrix.M21*vector.x) + (matrix.M22*vector.y) + (matrix.M23*vector.z) + (matrix.M24*vector.w),
                (matrix.M31*vector.x) + (matrix.M32*vector.y) + (matrix.M33*vector.z) + (matrix.M34*vector.w),
                (matrix.M41*vector.x) + (matrix.M42*vector.y) + (matrix.M43*vector.z) + (matrix.M44*vector.w));
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
        public static float3 TransformPD(float4x4 matrix, float3 vector)
        {
            float w = (matrix.M41*vector.x) + (matrix.M42*vector.y) + (matrix.M43*vector.z) + matrix.M44;
            return new float3(
                ((matrix.M11*vector.x) + (matrix.M12*vector.y) + (matrix.M13*vector.z) + matrix.M14)/w,
                ((matrix.M21*vector.x) + (matrix.M22*vector.y) + (matrix.M23*vector.z) + matrix.M24)/w,
                ((matrix.M31*vector.x) + (matrix.M32*vector.y) + (matrix.M33*vector.z) + matrix.M34)/w);
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
        public static float3 TransformPremultPD(float3 vector,float4x4 matrix)
        {
            float w = (matrix.M14 * vector.x) + (matrix.M24 * vector.y) + (matrix.M34 * vector.z) + matrix.M44;
            return new float3(
                ((matrix.M11 * vector.x) + (matrix.M21 * vector.y) + (matrix.M31 * vector.z) + matrix.M41) / w,
                ((matrix.M12 * vector.x) + (matrix.M22 * vector.y) + (matrix.M32 * vector.z) + matrix.M42) / w,
                ((matrix.M13 * vector.x) + (matrix.M23 * vector.y) + (matrix.M33 * vector.z) + matrix.M43) / w);
        }

        #endregion

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

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix addition
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float4x4 which holds the result of the multiplication</returns>
        public static float4x4 operator +(float4x4 left, float4x4 right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Matrix substraction
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new float2x2 which holds the result of the multiplication</returns>
        public static float4x4 operator -(float4x4 left, float4x4 right)
        {
            return Substract(left, right);
        }

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static float4x4 operator *(float4x4 left, float4x4 right)
        {
            return Mult(left, right);
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
        /// Transforms a given vector by a matrix via matrix*vector (Postmultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 operator *(float4x4 matrix, float4 vector)
        {
            return Transform(matrix, vector);
        }

        /// <summary>
        /// Transforms a given vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float4"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float4 operator *(float4 vector, float4x4 matrix)
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
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float4x4 matrix, float3 vector)
        {
            return TransformPD(matrix, vector);
        }

        /// <summary>
        /// Transforms a given threedimensional vector by a matrix via vector*matrix (Premultiplication of the vector).
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="float4x4"/> instance.</param>
        /// <param name="vector">A <see cref="float3"/> instance.</param>
        /// <returns>A new <see cref="float4"/> instance containing the result.</returns>
        public static float3 operator *(float3 vector,float4x4 matrix)
        {
            return TransformPremultPD(vector,matrix);
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
        #endregion

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

        #endregion

        #region public override int GetHashCode()

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A System.Int32 containing the unique hashcode for this instance.</returns>
        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        #endregion

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

            return Equals((float4x4) obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<float4x4> Members

        /// <summary>
        /// Indicates whether the current matrix represents an affine transformation.
        /// </summary>
        /// <returns>true if the current matrix represents an affine transformation; otherwise, false.</returns>
        public bool IsAffine
        {
            get
            {
                // Row order notation
                // return (Column3 == float4.UnitW);

                // Column order notation
                return (Row3 == float4.UnitW);
            }
        }

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

        #endregion

        public static Converter<string, float4x4> Parse { get; set; }

    }
}