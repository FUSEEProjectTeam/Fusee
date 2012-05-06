using System;

namespace Fusee.Math.GDI
{
	/// <summary>
	/// Contains functions to convert GDI+ structures to\from Fusee.Math structures.
	/// </summary>
	public sealed class Convert
	{
		/// <summary>
		/// Converts a <see cref="Fusee.Math.Core.Vector2F"/> to <see cref="System.Drawing.PointF"/>.
		/// </summary>
		/// <param name="value">A <see cref="Fusee.Math.Core.Vector2F"/> instance to convert.</param>
		/// <returns>A <see cref="System.Drawing.PointF"/> instance.</returns>
		public static System.Drawing.PointF ToGDI(Fusee.Math.Core.Vector2F value)
		{
			return new System.Drawing.PointF(value.X, value.Y);
		}
		/// <summary>
		/// Converts a <see cref="System.Drawing.PointF"/> to <see cref="Fusee.Math.Core.Vector2F"/>.
		/// </summary>
		/// <param name="value">A <see cref="Fusee.Math.Core.Vector2F"/> instance to convert.</param>
		/// <returns>A <see cref="Fusee.Math.Core.Vector2F"/> instance.</returns>
		public static Fusee.Math.Core.Vector2F FromGDI(System.Drawing.PointF value)
		{
			return new Fusee.Math.Core.Vector2F(value.X, value.Y);
		}

		/// <summary>
		/// Coberts a <see cref="Fusee.Math.Core.Matrix2F"/> to <see cref="System.Drawing.Drawing2D.Matrix"/>.
		/// </summary>
		/// <param name="value">A <see cref="Fusee.Math.Core.Matrix2F"/> instance to convert.</param>
		/// <returns>A <see cref="System.Drawing.Drawing2D.Matrix"/> instance.</returns>
		public static System.Drawing.Drawing2D.Matrix ToGDI(Fusee.Math.Core.Matrix2F value)
		{
			return new System.Drawing.Drawing2D.Matrix(
				value.M11, value.M12, value.M21, value.M22, 0.0f, 0.0f);
		}
		/// <summary>
		/// Coberts a <see cref="System.Drawing.Drawing2D.Matrix"/> to <see cref="Fusee.Math.Core.Matrix2F"/>.
		/// </summary>
		/// <param name="value">A <see cref="System.Drawing.Drawing2D.Matrix"/> instance to convert.</param>
		/// <returns>A <see cref="Fusee.Math.Core.Matrix2F"/> instance.</returns>
		public static Fusee.Math.Core.Matrix2F ToGDI(System.Drawing.Drawing2D.Matrix value)
		{
			return new Fusee.Math.Core.Matrix2F(
				value.Elements[0], value.Elements[1],
				value.Elements[2], value.Elements[3]);
		}

		#region Private Constructor
		private Convert()
		{
		}
		#endregion
	}
}
