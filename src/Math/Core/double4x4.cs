using System;
using System.Runtime.InteropServices;

namespace Fusee.Math
{
    /// <summary>
    /// Represents a 4x4 Matrix
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct double4x4 : IEquatable<double4x4>
    {
        #region Fields

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public double4 Row0;
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public double4 Row1;
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public double4 Row2;
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public double4 Row3;
 
        /// <summary>
        /// The identity matrix
        /// </summary>
        public static double4x4 Identity = new double4x4(double4.UnitX, double4.UnitY, double4.UnitZ, double4.UnitW);

        #endregion

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

        #endregion

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
                    Row0.x * Row1.y * Row2.z * Row3.w - Row0.x * Row1.y * Row2.w * Row3.z + Row0.x * Row1.z * Row2.w * Row3.y - Row0.x * Row1.z * Row2.y * Row3.w
                  + Row0.x * Row1.w * Row2.y * Row3.z - Row0.x * Row1.w * Row2.z * Row3.y - Row0.y * Row1.z * Row2.w * Row3.x + Row0.y * Row1.z * Row2.x * Row3.w
                  - Row0.y * Row1.w * Row2.x * Row3.z + Row0.y * Row1.w * Row2.z * Row3.x - Row0.y * Row1.x * Row2.z * Row3.w + Row0.y * Row1.x * Row2.w * Row3.z
                  + Row0.z * Row1.w * Row2.x * Row3.y - Row0.z * Row1.w * Row2.y * Row3.x + Row0.z * Row1.x * Row2.y * Row3.w - Row0.z * Row1.x * Row2.w * Row3.y
                  + Row0.z * Row1.y * Row2.w * Row3.x - Row0.z * Row1.y * Row2.x * Row3.w - Row0.w * Row1.x * Row2.y * Row3.z + Row0.w * Row1.x * Row2.z * Row3.y
                  - Row0.w * Row1.y * Row2.z * Row3.x + Row0.w * Row1.y * Row2.x * Row3.z - Row0.w * Row1.z * Row2.x * Row3.y + Row0.w * Row1.z * Row2.y * Row3.x;
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
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public double M11 { get { return Row0.x; } set { Row0.x = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public double M12 { get { return Row0.y; } set { Row0.y = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public double M13 { get { return Row0.z; } set { Row0.z = value; } }

        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public double M14 { get { return Row0.w; } set { Row0.w = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public double M21 { get { return Row1.x; } set { Row1.x = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public double M22 { get { return Row1.y; } set { Row1.y = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public double M23 { get { return Row1.z; } set { Row1.z = value; } }

        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public double M24 { get { return Row1.w; } set { Row1.w = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public double M31 { get { return Row2.x; } set { Row2.x = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public double M32 { get { return Row2.y; } set { Row2.y = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public double M33 { get { return Row2.z; } set { Row2.z = value; } }

        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public double M34 { get { return Row2.w; } set { Row2.w = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public double M41 { get { return Row3.x; } set { Row3.x = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public double M42 { get { return Row3.y; } set { Row3.y = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public double M43 { get { return Row3.z; } set { Row3.z = value; } }

        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public double M44 { get { return Row3.w; } set { Row3.w = value; } }

        /// <summary>
        /// Gets the offset part of the matrix as a <see cref="double3"/> instance.
        /// </summary>
        /// <remarks>
        /// The offset part of the matrix consists of the M14, M24 and M34 components (in row major order notation).
        /// </remarks>
        public double3 Offset
        {
            get
            {
                return new double3(Row0.w, Row1.w, Row2.w);
            }
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
            this = double4x4.Invert(this);
        }

        #endregion

        #region public void Transpose()

        /// <summary>
        /// Converts this instance into its transpose.
        /// </summary>
        public void Transpose()
        {
            this = double4x4.Transpose(this);
        }

        double[] ToArray()
        {
            return new double[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
            // return new double[] { M11, M21, M31, M41, M12, M22, M32, M42, M13, M23, M33, M43, M14, M24, M34, M44 };
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
        public static void CreateFromAxisAngle(double3 axis, double angle, out double4x4 result)
        {
            double cos = (double)System.Math.Cos(-angle);
            double sin = (double)System.Math.Sin(-angle);
            double t = 1.0f - cos;

            axis.Normalize();

            result = new double4x4(t * axis.x * axis.x + cos, t * axis.x * axis.y - sin * axis.z, t * axis.x * axis.z + sin * axis.y, 0.0f,
                                 t * axis.x * axis.y + sin * axis.z, t * axis.y * axis.y + cos, t * axis.y * axis.z - sin * axis.x, 0.0f,
                                 t * axis.x * axis.z - sin * axis.y, t * axis.y * axis.z + sin * axis.x, t * axis.z * axis.z + cos, 0.0f,
                                 0, 0, 0, 1);
        }
        
        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>A matrix instance.</returns>
        public static double4x4 CreateFromAxisAngle(double3 axis, double angle)
        {
            double4x4 result;
            CreateFromAxisAngle(axis, angle, out result);
            return result;
        }
        
        #endregion

        #region CreateRotation[XYZ]

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateRotationX(double angle, out double4x4 result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result.Row0 = double4.UnitX;
            result.Row1 = new double4(0.0f, cos, sin, 0.0f);
            result.Row2 = new double4(0.0f, -sin, cos, 0.0f);
            result.Row3 = double4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationX(double angle)
        {
            double4x4 result;
            CreateRotationX(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateRotationY(double angle, out double4x4 result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result.Row0 = new double4(cos, 0.0f, -sin, 0.0f);
            result.Row1 = double4.UnitY;
            result.Row2 = new double4(sin, 0.0f, cos, 0.0f);
            result.Row3 = double4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationY(double angle)
        {
            double4x4 result;
            CreateRotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateRotationZ(double angle, out double4x4 result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result.Row0 = new double4(cos, sin, 0.0f, 0.0f);
            result.Row1 = new double4(-sin, cos, 0.0f, 0.0f);
            result.Row2 = double4.UnitZ;
            result.Row3 = double4.UnitW;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateRotationZ(double angle)
        {
            double4x4 result;
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
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateTranslation(double x, double y, double z, out double4x4 result)
        {
            result = Identity;
            result.Row3 = new double4(x, y, z, 1);
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateTranslation(ref double3 vector, out double4x4 result)
        {
            result = Identity;
            result.Row3 = new double4(vector.x, vector.y, vector.z, 1);
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="x">X translation.</param>
        /// <param name="y">Y translation.</param>
        /// <param name="z">Z translation.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateTranslation(double x, double y, double z)
        {
            double4x4 result;
            CreateTranslation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateTranslation(double3 vector)
        {
            double4x4 result;
            CreateTranslation(vector.x, vector.y, vector.z, out result);
            return result;
        }

        #endregion

        #region CreateOrthographic

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateOrthographic(double width, double height, double zNear, double zFar, out double4x4 result)
        {
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="width">The width of the projection volume.</param>
        /// <param name="height">The height of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <rereturns>The resulting Matrix4 instance.</rereturns>
        public static double4x4 CreateOrthographic(double width, double height, double zNear, double zFar)
        {
            double4x4 result;
            CreateOrthographicOffCenter(-width / 2, width / 2, -height / 2, height / 2, zNear, zFar, out result);
            return result;
        }

        #endregion

        #region CreateOrthographicOffCenter

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <param name="result">The resulting Matrix4 instance.</param>
        public static void CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out double4x4 result)
        {
            result = new double4x4();

            double invRL = 1 / (right - left);
            double invTB = 1 / (top - bottom);
            double invFN = 1 / (zFar - zNear);

            result.M11 = 2 * invRL;
            result.M22 = 2 * invTB;
            result.M33 = -2 * invFN;

            result.M41 = -(right + left) * invRL;
            result.M42 = -(top + bottom) * invTB;
            result.M43 = -(zFar + zNear) * invFN;
            result.M44 = 1;
        }

        /// <summary>
        /// Creates an orthographic projection matrix.
        /// </summary>
        /// <param name="left">The left edge of the projection volume.</param>
        /// <param name="right">The right edge of the projection volume.</param>
        /// <param name="bottom">The bottom edge of the projection volume.</param>
        /// <param name="top">The top edge of the projection volume.</param>
        /// <param name="zNear">The near edge of the projection volume.</param>
        /// <param name="zFar">The far edge of the projection volume.</param>
        /// <returns>The resulting Matrix4 instance.</returns>
        public static double4x4 CreateOrthographicOffCenter(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            double4x4 result;
            CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }

        #endregion
        
        #region CreatePerspectiveFieldOfView
        
        /// <summary>
        /// Creates a perspective projection matrix.
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
        public static void CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar, out double4x4 result)
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
            
            double yMax = zNear * (double)System.Math.Tan(0.5f * fovy);
            double yMin = -yMax;
            double xMin = yMin * aspect;
            double xMax = yMax * aspect;

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out result);
        }
        
        /// <summary>
        /// Creates a perspective projection matrix.
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
            CreatePerspectiveFieldOfView(fovy, aspect, zNear, zFar, out result);
            return result;
        }
        
        #endregion
        
        #region CreatePerspectiveOffCenter
        
        /// <summary>
        /// Creates an perspective projection matrix.
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="zNear">Distance to the near clip plane</param>
        /// <param name="zFar">Distance to the far clip plane</param>
        /// <param name="result">A projection matrix that transforms camera space to raster space</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown under the following conditions:
        /// <list type="bullet">
        /// <item>zNear is negative or zero</item>
        /// <item>zFar is negative or zero</item>
        /// <item>zNear is larger than zFar</item>
        /// </list>
        /// </exception>
        public static void CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out double4x4 result)
        {
            if (zNear <= 0)
                throw new ArgumentOutOfRangeException("zNear");
            if (zFar <= 0)
                throw new ArgumentOutOfRangeException("zFar");
            if (zNear >= zFar)
                throw new ArgumentOutOfRangeException("zNear");
            
            double x = (2.0f * zNear) / (right - left);
            double y = (2.0f * zNear) / (top - bottom);
            double a = (right + left) / (right - left);
            double b = (top + bottom) / (top - bottom);
            double c = -(zFar + zNear) / (zFar - zNear);
            double d = -(2.0f * zFar * zNear) / (zFar - zNear);
            
            result = new double4x4(x, 0, 0,  0,
                                 0, y, 0,  0,
                                 a, b, c, -1,
                                 0, 0, d,  0);
        }
        
        /// <summary>
        /// Creates an perspective projection matrix.
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
            CreatePerspectiveOffCenter(left, right, bottom, top, zNear, zFar, out result);
            return result;
        }
        
        #endregion

        #region Obsolete Functions

        #region Translation Functions

        /// <summary>
        /// Builds a translation matrix.
        /// </summary>
        /// <param name="trans">The translation vector.</param>
        /// <returns>A new Matrix4 instance.</returns>
        [Obsolete("Use CreateTranslation instead.")]
        public static double4x4 Translation(double3 trans)
        {
            return Translation(trans.x, trans.y, trans.z);
        }

        /// <summary>
        /// Build a translation matrix with the given translation
        /// </summary>
        /// <param name="x">X translation</param>
        /// <param name="y">Y translation</param>
        /// <param name="z">Z translation</param>
        /// <returns>A Translation matrix</returns>
        [Obsolete("Use CreateTranslation instead.")]
        public static double4x4 Translation(double x, double y, double z)
        {
            double4x4 result = Identity;
            result.Row3 = new double4(x, y, z, 1.0f);
            return result;
        }

        #endregion

        #endregion

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

        #endregion

        #region Rotation Functions

        /// <summary>
        /// Build a rotation matrix that rotates about the x-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the x-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationX instead.")]
        public static double4x4 RotateX(double angle)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            double4x4 result;
            result.Row0 = double4.UnitX;
            result.Row1 = new double4(0.0f, cos, sin, 0.0f);
            result.Row2 = new double4(0.0f, -sin, cos, 0.0f);
            result.Row3 = double4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix that rotates about the y-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the y-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationY instead.")]
        public static double4x4 RotateY(double angle)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            double4x4 result;
            result.Row0 = new double4(cos, 0.0f, -sin, 0.0f);
            result.Row1 = double4.UnitY;
            result.Row2 = new double4(sin, 0.0f, cos, 0.0f);
            result.Row3 = double4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix that rotates about the z-axis
        /// </summary>
        /// <param name="angle">angle in radians to rotate counter-clockwise around the z-axis</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateRotationZ instead.")]
        public static double4x4 RotateZ(double angle)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            double4x4 result;
            result.Row0 = new double4(cos, sin, 0.0f, 0.0f);
            result.Row1 = new double4(-sin, cos, 0.0f, 0.0f);
            result.Row2 = double4.UnitZ;
            result.Row3 = double4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix to rotate about the given axis
        /// </summary>
        /// <param name="axis">the axis to rotate about</param>
        /// <param name="angle">angle in radians to rotate counter-clockwise (looking in the direction of the given axis)</param>
        /// <returns>A rotation matrix</returns>
        [Obsolete("Use CreateFromAxisAngle instead.")]
        public static double4x4 Rotate(double3 axis, double angle)
        {
            double cos = (double)System.Math.Cos(-angle);
            double sin = (double)System.Math.Sin(-angle);
            double t = 1.0f - cos;

            axis.Normalize();

            double4x4 result;
            result.Row0 = new double4(t * axis.x * axis.x + cos, t * axis.x * axis.y - sin * axis.z, t * axis.x * axis.z + sin * axis.y, 0.0f);
            result.Row1 = new double4(t * axis.x * axis.y + sin * axis.z, t * axis.y * axis.y + cos, t * axis.y * axis.z - sin * axis.x, 0.0f);
            result.Row2 = new double4(t * axis.x * axis.z - sin * axis.y, t * axis.y * axis.z + sin * axis.x, t * axis.z * axis.z + cos, 0.0f);
            result.Row3 = double4.UnitW;
            return result;
        }

        /// <summary>
        /// Build a rotation matrix from a QuaternionD
        /// </summary>
        /// <param name="q">the QuaternionD</param>
        /// <returns>A rotation matrix</returns>
        public static double4x4 Rotate(QuaternionD q)
        {
            double3 axis;
            double angle;
            q.ToAxisAngle(out axis, out angle);
            return CreateFromAxisAngle(axis, angle);
        }

        #endregion

        #region Camera Helper Functions

        /// <summary>
        /// Build a world space to camera space matrix
        /// </summary>
        /// <param name="eye">Eye (camera) position in world space</param>
        /// <param name="target">Target position in world space</param>
        /// <param name="up">Up vector in world space (should not be parallel to the camera direction, that is target - eye)</param>
        /// <returns>A Matrix4 that transforms world space to camera space</returns>
        public static double4x4 LookAt(double3 eye, double3 target, double3 up)
        {
            double3 z = double3.Normalize(eye - target);
            double3 x = double3.Normalize(double3.Cross(up, z));
            double3 y = double3.Normalize(double3.Cross(z, x));

            double4x4 rot = new double4x4(new double4(x.x, y.x, z.x, 0.0f),
                                        new double4(x.y, y.y, z.y, 0.0f),
                                        new double4(x.z, y.z, z.z, 0.0f),
                                        double4.UnitW);

            double4x4 trans = double4x4.CreateTranslation(-eye);

            return trans * rot;
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
        public static double4x4 LookAt(double eyeX, double eyeY, double eyeZ, double targetX, double targetY, double targetZ, double upX, double upY, double upZ)
        {
            return LookAt(new double3(eyeX, eyeY, eyeZ), new double3(targetX, targetY, targetZ), new double3(upX, upY, upZ));
        }

        /// <summary>
        /// Build a projection matrix
        /// </summary>
        /// <param name="left">Left edge of the view frustum</param>
        /// <param name="right">Right edge of the view frustum</param>
        /// <param name="bottom">Bottom edge of the view frustum</param>
        /// <param name="top">Top edge of the view frustum</param>
        /// <param name="near">Distance to the near clip plane</param>
        /// <param name="far">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        [Obsolete("Use CreatePerspectiveOffCenter instead.")]
        public static double4x4 Frustum(double left, double right, double bottom, double top, double near, double far)
        {
            double invRL = 1.0f / (right - left);
            double invTB = 1.0f / (top - bottom);
            double invFN = 1.0f / (far - near);
            return new double4x4(new double4(2.0f * near * invRL, 0.0f, 0.0f, 0.0f),
                               new double4(0.0f, 2.0f * near * invTB, 0.0f, 0.0f),
                               new double4((right + left) * invRL, (top + bottom) * invTB, -(far + near) * invFN, -1.0f),
                               new double4(0.0f, 0.0f, -2.0f * far * near * invFN, 0.0f));
        }

        /// <summary>
        /// Build a projection matrix
        /// </summary>
        /// <param name="fovy">Angle of the field of view in the y direction (in radians)</param>
        /// <param name="aspect">Aspect ratio of the view (width / height)</param>
        /// <param name="near">Distance to the near clip plane</param>
        /// <param name="far">Distance to the far clip plane</param>
        /// <returns>A projection matrix that transforms camera space to raster space</returns>
        [Obsolete("Use CreatePerspectiveFieldOfView instead.")]
        public static double4x4 Perspective(double fovy, double aspect, double near, double far)
        {
            double yMax = near * (double)System.Math.Tan(0.5f * fovy);
            double yMin = -yMax;
            double xMin = yMin * aspect;
            double xMax = yMax * aspect;

            return Frustum(xMin, xMax, yMin, yMax, near, far);
        }

        #endregion

        #region Multiply Functions

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <returns>A new instance that is the result of the multiplication</returns>
        public static double4x4 Mult(double4x4 left, double4x4 right)
        {
            double4x4 result;
            Mult(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static void Mult(ref double4x4 left, ref double4x4 right, out double4x4 result)
        {
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
        }

        #endregion

        #region Invert Functions

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4 is singular.</exception>
        public static double4x4 Invert(double4x4 mat)
        {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            double[,] inverse = {{mat.Row0.x, mat.Row0.y, mat.Row0.z, mat.Row0.w}, 
                                {mat.Row1.x, mat.Row1.y, mat.Row1.z, mat.Row1.w}, 
                                {mat.Row2.x, mat.Row2.y, mat.Row2.z, mat.Row2.w}, 
                                {mat.Row3.x, mat.Row3.y, mat.Row3.z, mat.Row3.w} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                double maxPivot = 0.0f;
                for (int j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                double absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 4; ++k)
                    {
                        double f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                double pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0f)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                double oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        double f = inverse[j, icol];
                        inverse[j, icol] = 0.0f;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k)
                {
                    double f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new double4(inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3]);
            mat.Row1 = new double4(inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3]);
            mat.Row2 = new double4(inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3]);
            mat.Row3 = new double4(inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
            return mat;
        }

        #endregion

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


        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <param name="result">The result of the calculation</param>
        public static void Transpose(ref double4x4 mat, out double4x4 result)
        {
            result.Row0 = mat.Column0;
            result.Row1 = mat.Column1;
            result.Row2 = mat.Column2;
            result.Row3 = mat.Column3;
        }

        #endregion

        #region Transform
        /// <summary>
        /// Transforms a given vector by a matrix.
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
        /// Transforms a given 3D vector by a matrix using perspective division.
        /// </summary>
        /// <remarks>
        /// Before the matrix multiplication the 3D vector is extended to 4D by setting its W component to 1.
        /// After the matrix multiplication the resulting 4D vector is transformed to 3D by dividing X, Y, and Z by W.
        /// (perspective division).
        /// </remarks>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double3"/> instance.</param>
        /// <returns>A new <see cref="double3"/> instance containing the result.</returns>
        public static double3 TransformPD(double4x4 matrix, double3 vector)
        {
            double w = (matrix.M41 * vector.x) + (matrix.M42 * vector.y) + (matrix.M43 * vector.z) + matrix.M44;
            return new double3(
                ((matrix.M11 * vector.x) + (matrix.M12 * vector.y) + (matrix.M13 * vector.z) + matrix.M14) / w,
                ((matrix.M21 * vector.x) + (matrix.M22 * vector.y) + (matrix.M23 * vector.z) + matrix.M24) / w,
                ((matrix.M31 * vector.x) + (matrix.M32 * vector.y) + (matrix.M33 * vector.z) + matrix.M34) / w);
        }

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Matrix multiplication
        /// </summary>
        /// <param name="left">left-hand operand</param>
        /// <param name="right">right-hand operand</param>
        /// <returns>A new Matrix44 which holds the result of the multiplication</returns>
        public static double4x4 operator *(double4x4 left, double4x4 right)
        {
            return double4x4.Mult(left, right);
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
        /// Transforms a given vector by a matrix.
        /// </summary>
        /// <param name="matrix">A <see cref="double4x4"/> instance.</param>
        /// <param name="vector">A <see cref="double4"/> instance.</param>
        /// <returns>A new <see cref="double4"/> instance containing the result.</returns>
        public static double4 operator *(double4x4 matrix, double4 vector)
        {
            return double4x4.Transform(matrix, vector);
        }
        /// <summary>
        /// Transforms a given threedimensional vector by a matrix.
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
            return double4x4.TransformPD(matrix, vector);
        }

        #endregion

        #region Overrides

        #region public override string ToString()

        /// <summary>
        /// Returns a System.String that represents the current Matrix44.
        /// </summary>
        /// <returns></returns>
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
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
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
            if (!(obj is double4x4))
                return false;

            return this.Equals((double4x4)obj);
        }

        #endregion

        #endregion

        #endregion

        #region IEquatable<Matrix4> Members

        /// <summary>Indicates whether the current matrix is equal to another matrix.</summary>
        /// <param name="other">An matrix to compare with this matrix.</param>
        /// <returns>true if the current matrix is equal to the matrix parameter; otherwise, false.</returns>
        public bool Equals(double4x4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        #endregion

    }
}
