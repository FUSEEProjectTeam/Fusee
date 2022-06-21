using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Drop in class for previously used arrays as properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableArray<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Get's the number of elements inside the collection
        /// </summary>
        public int Length => Count;

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableArray{T}"/>
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ObservableArray{T}"/>
        ///     The collection itself cannot be null, but it can contain elements that are null,
        ///     if type T is a reference type.
        /// </param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection to the <see cref="ObservableArray{T}"/>, replaces all elements
        /// </summary>
        /// <param name="collection">The collection whose elements should be assigned to the <see cref="ObservableArray{T}"/>
        ///     The collection itself cannot be null, but it can contain elements that are null,
        ///     if type T is a reference type.
        /// </param>
        public void Assign(IEnumerable<T> collection)
        {
            Clear();
            foreach (var item in collection)
            {
                Add(item);
            }
        }



        /// <summary>
        /// Convert any observable array to an <see cref="Array"/> array.
        /// </summary>
        /// <param name="o"></param>
        public static implicit operator T[](ObservableArray<T> o)
        {
            return o.ToArray();
        }
    }

    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IManagedMesh, IDisposable
    {
        #region RenderContext Asset Management

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> MeshChanged;

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        #endregion

        /// <summary>
        /// Gets and sets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public readonly ObservableArray<float3> Vertices = new();

        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet => Vertices?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool TangentsSet => Tangents?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bi tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bi tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool BiTangentsSet => BiTangents?.Length > 0;


        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public readonly ObservableArray<uint> Colors = new();

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet => Colors?.Length > 0;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public readonly ObservableArray<uint> Colors1 = new();

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 => Colors1?.Length > 0;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public readonly ObservableArray<uint> Colors2 = new();

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 => Colors2?.Length > 0;


        /// <summary>
        /// Gets and sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        public readonly ObservableArray<float3> Normals = new();

        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet => Normals?.Length > 0;

        /// <summary>
        /// Gets and sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        public readonly ObservableArray<float2> UVs = new();

        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet => UVs?.Length > 0;

        /// <summary>
        /// Gets and sets the bone weights.
        /// </summary>
        /// <value>
        /// The bone weights.
        /// </value>
        public readonly ObservableArray<float4> BoneWeights = new();

        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => BoneWeights?.Length > 0;

        /// <summary>
        /// Gets and sets the bone indices.
        /// </summary>
        /// <value>
        /// The bone indices.
        /// </value>
        public readonly ObservableArray<float4> BoneIndices = new();

        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet => BoneIndices?.Length > 0;

        /// <summary>
        /// Gets and sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        public readonly ObservableArray<ushort> Triangles = new();

        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet => Triangles?.Length > 0;

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox;

        /// <summary>
        /// The tangent of each triangle for normal mapping.
        /// w-component is handedness
        /// </summary>
        public readonly ObservableArray<float4> Tangents = new();

        /// <summary>
        /// The bi tangent of each triangle for normal mapping.
        /// </summary>
        public readonly ObservableArray<float3> BiTangents = new();

        /// <summary>
        /// This is being called from the mesh manager to wire up all changes
        /// </summary>
        internal void InitMesh()
        {
            Vertices.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
            Triangles.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
            Normals.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));

            UVs.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));

            BiTangents.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
            Tangents.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents));

            BoneIndices.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
            BoneWeights.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));

            Colors2.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
            Colors1.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
            Colors.CollectionChanged += (s, e) => MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
        }

        /// <summary>
        /// The type of mesh which is represented by this instance (e. g. triangle mesh, point, line, etc...)
        /// </summary>
        public PrimitiveType MeshType { get; set; }

        #region IDisposable Support

        private bool disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Finalizers (historically referred to as destructors) are used to perform any necessary final clean-up when a class instance is being collected by the garbage collector.
        /// </summary>
        ~Mesh()
        {
            Dispose(false);
        }

        #endregion

    }
}