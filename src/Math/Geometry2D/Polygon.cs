#region Copyright(C) Notice
//	Fusee.Math math library. Based on the Sharp3D math library originally developed by
//	Copyright (C) 2003-2004  
//	Eran Kampf
//	tentacle@zahav.net.il
//	http://www.ekampf.com/Fusee.Math/
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml.Serialization;

using Fusee.Math.Core;

namespace Fusee.Math.Geometry2D
{
	/// <summary>
	/// Represents a polygon in 2 dimentional space.
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Polygon : ICloneable, ISerializable
	{
		#region Private fields
		private Vector2FArrayList _points = new Vector2FArrayList();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class.
		/// </summary>
		public Polygon()
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="points">An <see cref="Vector2FArrayList"/> instance.</param>
		public Polygon(Vector2FArrayList points)
		{
			_points.AddRange(points);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using an array of coordinates.
		/// </summary>
		/// <param name="points">An array of <see cref="Vector2F"/> coordniates.</param>
		public Polygon(Vector2F[] points)
		{
			_points.AddRange(points);
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class using coordinates from another instance.
		/// </summary>
		/// <param name="polygon">A <see cref="Polygon"/> instance.</param>
		public Polygon(Polygon polygon)
		{
			_points = (Vector2FArrayList)polygon._points.Clone();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="Polygon"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected Polygon(SerializationInfo info, StreamingContext context)
		{
			_points = (Vector2FArrayList)info.GetValue("Points", typeof(Vector2FArrayList));
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the polygon's list of points.
		/// </summary>
		[XmlArrayItem(Type = typeof(Vector2F))]
		public Vector2FArrayList Points
		{
			get { return _points; }
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates an exact copy of this <see cref="Polygon"/> object.
		/// </summary>
		/// <returns>The <see cref="Polygon"/> object this method creates, cast as an object.</returns>
		object ICloneable.Clone()
		{
			return new Polygon(this);
		}
		/// <summary>
		/// Creates an exact copy of this <see cref="Polygon"/> object.
		/// </summary>
		/// <returns>The <see cref="Polygon"/> object this method creates.</returns>
		public Polygon Clone()
		{
			return new Polygon(this);
		}
		#endregion

		#region ISerializable Members
		/// <summary>
		/// Populates a <see cref="SerializationInfo"/> with the data needed to serialize this object.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
		/// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
		//[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Points", _points, typeof(Vector2FArrayList));
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Flips the polygon.
		/// </summary>
		public void Flip()
		{
			_points.Reverse();
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the polygon's vertex count.
		/// </summary>
		public int VertexCount
		{
			get { return _points.Count; }
		}
		#endregion
	}
}
