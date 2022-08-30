using CommunityToolkit.Diagnostics;
using Fusee.Engine.Common;
using Fusee.Math.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Fusee.Engine.Core.Scene
{
    public sealed class MeshAttributes<T> where T : struct
    {
        private readonly T[] _attribData;

        public bool UpdateDataAfterRender { get; set; } = true;

        public bool DataChangedDirtyFlag { get; private set; }

        public readonly List<(int, int)> ChangedIndices = new();

        public int Length => _attribData.Length;

        public MeshAttributes(in IEnumerable<T> data) => _attribData = (T[])data;

        private MeshAttributes() { }

        /// <summary>
        /// Quick readonly access to data for read purposes
        /// Use <see cref="MeshAttributes{T}"/> indexer for read/write access
        /// </summary>
        public ReadOnlyCollection<T> ReadOnlyData => new(_attribData);

        public T this[int idx]
        {
            get => _attribData[idx];
            set
            {
                _attribData[idx] = value;
                DataChangedDirtyFlag = true;

                // TODO: Track changed indices in a fast and memory efficient way
            }
        }

        /// <summary>
        /// Returns the backing field value as array
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return _attribData;
        }

        /// <summary>
        /// Fills the array with given value beginning at index <paramref name="pos"/> until <paramref name="length"/> is reached.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pos">default = 0</param>
        /// <param name="length">default = -1, if -1 is given the entire length is being used</param>
        public void Fill(in T data, int pos = 0, int length = -1)
        {
            Guard.IsGreaterThanOrEqualTo(pos, 0);
            Guard.IsGreaterThanOrEqualTo(length, -1);
            Guard.IsNotNull(data);

            var calculatedLength = length == -1 ? (_attribData.Length + pos) : length;
            Guard.IsLessThan(calculatedLength, _attribData.Length);

            Array.Fill(_attribData, data, pos, calculatedLength);

            DataChangedDirtyFlag = true;
            ChangedIndices.Add((0, calculatedLength - 1));
        }

        /// <summary>
        /// Sets attributes by given <paramref name="data"/>, replacing all elements beginning at given <paramref name="startIdx"/>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIdx">default = 0</param>
        public void SetAttributeData(in T[] data, int startIdx = 0)
        {
            Guard.IsNotNull(data);
            Guard.IsGreaterThanOrEqualTo(startIdx, 0);

            var calculatedLength = data.Length + startIdx;
            Guard.IsLessThanOrEqualTo(calculatedLength, _attribData.Length);

            Array.Copy(data, 0, _attribData, startIdx, data.Length);

            DataChangedDirtyFlag = true;
            ChangedIndices.Add((startIdx, _attribData.Length - 1));
        }
    }

    /// <summary>
    /// Provides the ability to create or interact directly with the point data.
    /// </summary>
    public class Mesh : SceneComponent, IManagedMesh
    {
        #region RenderContext Asset Management

        /// <summary>
        /// MeshChanged event notifies observing MeshManager about property changes and the Mesh's disposal.
        /// </summary>
        public event EventHandler<MeshChangedEventArgs> DisposeData;

        /// <summary>
        /// SessionUniqueIdentifier is used to verify a Mesh's uniqueness in the current session.
        /// </summary>
        public Suid SessionUniqueIdentifier { get; } = Suid.GenerateSuid();

        #endregion

        #region Mesh data member

        public readonly MeshAttributes<uint> Triangles;
        public readonly MeshAttributes<float3> Vertices;
        public readonly MeshAttributes<float3>? Normals;
        public readonly MeshAttributes<float2>? UVs;

        public readonly MeshAttributes<float4>? BoneWeights;
        public readonly MeshAttributes<float4>? BoneIndices;

        public readonly MeshAttributes<float4>? Tangents;
        public readonly MeshAttributes<float3>? BiTangents;

        public readonly MeshAttributes<uint>? Colors;
        public readonly MeshAttributes<uint>? Colors1;
        public readonly MeshAttributes<uint>? Colors2;

        #endregion


        /// <summary>
        /// Generates a new <see cref="Mesh"/> instance
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="vertices"></param>
        /// <param name="normals"></param>
        /// <param name="uvs"></param>
        /// <param name="boneWeights"></param>
        /// <param name="boneIndices"></param>
        /// <param name="tangents"></param>
        /// <param name="biTangents"></param>
        /// <param name="colors"></param>
        /// <param name="colors1"></param>
        /// <param name="colors2"></param>
        public Mesh
        (
            IEnumerable<uint> triangles,
            IEnumerable<float3> vertices,
            IEnumerable<float3>? normals = null,
            IEnumerable<float2>? uvs = null,
            IEnumerable<float4>? boneWeights = null,
            IEnumerable<float4>? boneIndices = null,
            IEnumerable<float4>? tangents = null,
            IEnumerable<float3>? biTangents = null,
            IEnumerable<uint>? colors = null,
            IEnumerable<uint>? colors1 = null,
            IEnumerable<uint>? colors2 = null)
        {
            Guard.IsGreaterThan(triangles.Count(), 0);
            Guard.IsGreaterThan(vertices.Count(), 0);

            Triangles = new MeshAttributes<uint>(triangles);
            Vertices = new MeshAttributes<float3>(vertices);

            if (normals != null)
            {
                Guard.IsEqualTo(normals.Count(), vertices.Count());
                Normals = new MeshAttributes<float3>(normals);
            }

            if (uvs != null)
            {
                Guard.IsEqualTo(uvs.Count(), vertices.Count());
                UVs = new MeshAttributes<float2>(uvs);
            }

            if (boneWeights != null)
            {
                Guard.IsEqualTo(boneWeights.Count(), vertices.Count());
                BoneWeights = new MeshAttributes<float4>(boneWeights);
            }

            if (boneIndices != null)
            {
                Guard.IsEqualTo(boneIndices.Count(), vertices.Count());
                BoneIndices = new MeshAttributes<float4>(boneIndices);
            }

            if (tangents != null)
            {
                Guard.IsEqualTo(tangents.Count(), vertices.Count());
                Tangents = new MeshAttributes<float4>(tangents);
            }

            if (biTangents != null)
            {
                Guard.IsEqualTo(biTangents.Count(), vertices.Count());
                BiTangents = new MeshAttributes<float3>(biTangents);
            }

            if (colors != null)
            {
                Guard.IsEqualTo(colors.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors);
            }

            if (colors1 != null)
            {
                Guard.IsEqualTo(colors1.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors1);
            }

            if (colors2 != null)
            {
                Guard.IsEqualTo(colors2.Count(), vertices.Count());
                Colors = new MeshAttributes<uint>(colors2);
            }
        }

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
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet => Colors?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 => Colors1?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 => Colors2?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet => Normals?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet => UVs?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => BoneWeights?.Length > 0;

        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet => BoneIndices?.Length > 0;

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
        public AABBf BoundingBox { internal set; get; }

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
                    //dispose managed resources
                }
                DisposeData?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Disposed));

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