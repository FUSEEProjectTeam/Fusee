using Fusee.Engine.Common;
using Fusee.Math.Core;
using Microsoft.Toolkit.Diagnostics;
using System;

namespace Fusee.Engine.Core.Scene
{
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

        #region Private mesh data member

        private float4[] _boneWeights;
        private float4[] _boneIndices;
        private float4[] _tangents;

        private float3[] _biTangents;
        private float3[] _vertices;
        private float3[] _normals;

        private float2[] _uvs;

        private ushort[] _triangles;
        private uint[] _colors;
        private uint[] _colors1;
        private uint[] _colors2;

        #endregion

        /// <summary>
        /// Gets the vertices.
        /// </summary>
        /// <value>
        /// The vertices.
        /// </value>
        public ReadOnlySpan<float3> Vertices => _vertices;

        /// <summary>
        /// Set all vertices of the mesh.
        /// </summary>
        /// <param name="vertices"></param>
        public void SetVertices(float3[] vertices)
        {
            Guard.IsNotNull(vertices, nameof(vertices));
            _vertices = vertices;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
        }

        /// <summary>
        /// Set/alter one vertex of given mesh.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="vertex"></param>
        public void SetVertex(int idx, float3 vertex)
        {
            Guard.IsNotNull(_vertices, nameof(_vertices));
            Guard.IsInRange(idx, 0, _vertices.Length, nameof(idx));
            _vertices[idx] = vertex;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Vertices));
        }

        /// <summary>
        /// Gets a value indicating whether vertices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if vertices are set; otherwise, <c>false</c>.
        /// </value>
        public bool VerticesSet => _vertices?.Length > 0;


        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public ReadOnlySpan<uint> Colors => _colors;


        /// <summary>
        /// Set all colors of the mesh.
        /// </summary>
        /// <param name="colors"></param>
        public void SetColors(uint[] colors)
        {
            Guard.IsNotNull(colors, nameof(colors));
            _colors = colors;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
        }

        /// <summary>
        /// Set/alter one color of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="color"></param>
        public void SetColor(int idx, uint color)
        {
            Guard.IsNotNull(_colors, nameof(_colors));
            Guard.IsInRange(idx, 0, _colors.Length, nameof(idx));
            _colors[idx] = color;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors));
        }

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet => _colors?.Length > 0;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public ReadOnlySpan<uint> Colors1 => _colors1;

        /// <summary>
        /// Set all colors1 of the mesh.
        /// </summary>
        /// <param name="colors1"></param>
        public void SetColors1(uint[] colors1)
        {
            Guard.IsNotNull(colors1, nameof(colors1));
            _colors1 = colors1;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
        }

        /// <summary>
        /// Set/alter one color1 of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="color1"></param>
        public void SetColor1(int idx, uint color1)
        {
            Guard.IsNotNull(_colors1, nameof(_colors1));
            Guard.IsInRange(idx, 0, _colors1.Length, nameof(idx));
            _colors1[idx] = color1;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors1));
        }

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet1 => _colors1?.Length > 0;

        /// <summary>
        /// Gets and sets the color of a single vertex.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public ReadOnlySpan<uint> Colors2 => _colors2;

        /// <summary>
        /// Set all colors2 of the mesh.
        /// </summary>
        /// <param name="colors2"></param>
        public void SetColors2(uint[] colors2)
        {
            Guard.IsNotNull(colors2, nameof(colors2));
            _colors2 = colors2;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
        }

        /// <summary>
        /// Set/alter one color2 of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="color2"></param>
        public void SetColor2(int idx, uint color2)
        {
            Guard.IsNotNull(_colors2, nameof(_colors2));
            Guard.IsInRange(idx, 0, _colors2.Length, nameof(idx));
            _colors2[idx] = color2;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Colors2));
        }

        /// <summary>
        /// Gets a value indicating whether a color is set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a color is set; otherwise, <c>false</c>.
        /// </value>
        public bool ColorsSet2 => _colors2?.Length > 0;


        /// <summary>
        /// Gets and sets the normals.
        /// </summary>
        /// <value>
        /// The normals..
        /// </value>
        public ReadOnlySpan<float3> Normals => _normals;

        /// <summary>
        /// Set all normals of the mesh.
        /// </summary>
        /// <param name="normals"></param>
        public void SetNormals(float3[] normals)
        {
            Guard.IsNotNull(normals, nameof(normals));
            _normals = normals;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));
        }

        /// <summary>
        /// Set/alter one normal of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="normal"></param>
        public void SetNormal(int idx, float3 normal)
        {
            Guard.IsNotNull(_normals, nameof(_normals));
            Guard.IsInRange(idx, 0, _normals.Length, nameof(idx));
            _normals[idx] = normal;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Normals));
        }

        /// <summary>
        /// Gets a value indicating whether normals are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if normals are set; otherwise, <c>false</c>.
        /// </value>
        public bool NormalsSet => _normals?.Length > 0;

        /// <summary>
        /// Gets and sets the UV-coordinates.
        /// </summary>
        /// <value>
        /// The UV-coordinates.
        /// </value>
        public ReadOnlySpan<float2> UVs => _uvs;

        /// <summary>
        /// Set all uvs of the mesh.
        /// </summary>
        /// <param name="uv"></param>
        public void SetUVs(float2[] uv)
        {
            Guard.IsNotNull(uv, nameof(uv));
            _uvs = uv;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));
        }

        /// <summary>
        /// Set/alter one uv of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="uv"></param>
        public void SetUV(int idx, float2 uv)
        {
            Guard.IsNotNull(_uvs, nameof(_uvs));
            Guard.IsInRange(idx, 0, _uvs.Length, nameof(idx));
            _uvs[idx] = uv;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Uvs));
        }

        /// <summary>
        /// Gets a value indicating whether UVs are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVs are set; otherwise, <c>false</c>.
        /// </value>
        public bool UVsSet => _uvs?.Length > 0;

        /// <summary>
        /// Gets and sets the bone weights.
        /// </summary>
        /// <value>
        /// The bone weights.
        /// </value>
        public ReadOnlySpan<float4> BoneWeights => _boneWeights;

        /// <summary>
        /// Set all bone weights of the mesh.
        /// </summary>
        /// <param name="boneWeights"></param>
        public void SetBoneWeights(float4[] boneWeights)
        {
            Guard.IsNotNull(boneWeights, nameof(boneWeights));
            _boneWeights = boneWeights;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));
        }

        /// <summary>
        /// Set/alter one bone weight of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="boneWeight"></param>
        public void SetBoneWeight(int idx, float4 boneWeight)
        {
            Guard.IsNotNull(_boneWeights, nameof(_boneWeights));
            Guard.IsInRange(idx, 0, _boneWeights.Length, nameof(idx));
            _boneWeights[idx] = boneWeight;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneWeights));
        }

        /// <summary>
        /// Gets a value indicating whether bone weights are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone weights are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneWeightsSet => _boneWeights?.Length > 0;

        /// <summary>
        /// Gets and sets the bone indices.
        /// </summary>
        /// <value>
        /// The bone indices.
        /// </value>
        public ReadOnlySpan<float4> BoneIndices => _boneIndices;

        /// <summary>
        /// Set all bone indices of the mesh.
        /// </summary>
        /// <param name="boneIndices"></param>
        public void SetBoneIndices(float4[] boneIndices)
        {
            Guard.IsNotNull(boneIndices, nameof(boneIndices));
            _boneIndices = boneIndices;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
        }

        /// <summary>
        /// Set/alter one bone index of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="boneIndex"></param>
        public void SetBoneIndex(int idx, float4 boneIndex)
        {
            Guard.IsNotNull(_boneIndices, nameof(_boneIndices));
            Guard.IsInRange(idx, 0, _boneIndices.Length, nameof(idx));
            _boneIndices[idx] = boneIndex;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BoneIndices));
        }

        /// <summary>
        /// Gets a value indicating whether bone indices are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bone indices are set; otherwise, <c>false</c>.
        /// </value>
        public bool BoneIndicesSet => _boneIndices?.Length > 0;

        /// <summary>
        /// Gets and sets the triangles.
        /// </summary>
        /// <value>
        /// The triangles.
        /// </value>
        public ReadOnlySpan<ushort> Triangles => _triangles;

        /// <summary>
        /// Set all triangles of the mesh.
        /// </summary>
        /// <param name="triangles"></param>
        public void SetTriangles(ushort[] triangles)
        {
            Guard.IsNotNull(triangles, nameof(triangles));
            _triangles = triangles;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
        }

        /// <summary>
        /// Set/alter one triangle of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="triangle"></param>
        public void SetBoneIndex(int idx, ushort triangle)
        {
            Guard.IsNotNull(_triangles, nameof(_triangles));
            Guard.IsInRange(idx, 0, _triangles.Length, nameof(idx));
            _triangles[idx] = triangle;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Triangles));
        }

        /// <summary>
        /// Gets a value indicating whether triangles are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if triangles are set; otherwise, <c>false</c>.
        /// </value>
        public bool TrianglesSet => _triangles?.Length > 0;

        /// <summary>
        /// The tangent of each triangle for normal mapping.
        /// w-component is handedness
        /// </summary>
        public ReadOnlySpan<float4> Tangents => _tangents;

        /// <summary>
        /// Set all tangents of the mesh.
        /// </summary>
        /// <param name="tangents"></param>
        public void SetTangents(float4[] tangents)
        {
            Guard.IsNotNull(tangents, nameof(tangents));
            _tangents = tangents;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents));
        }

        /// <summary>
        /// Set/alter one tangent of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="tangents"></param>
        public void SetTangent(int idx, float4 tangents)
        {
            Guard.IsNotNull(_tangents, nameof(_tangents));
            Guard.IsInRange(idx, 0, _tangents.Length, nameof(idx));
            _tangents[idx] = tangents;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.Tangents));
        }

        /// <summary>
        /// Gets a value indicating whether tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool TangentsSet => _tangents?.Length > 0;

        /// <summary>
        /// The bi tangent of each triangle for normal mapping.
        /// </summary>
        public ReadOnlySpan<float3> BiTangents => _biTangents;

        /// <summary>
        /// Set all bitangents of the mesh.
        /// </summary>
        /// <param name="biTangents"></param>
        public void SetBiTangents(float3[] biTangents)
        {
            Guard.IsNotNull(biTangents, nameof(biTangents));
            _biTangents = biTangents;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
        }

        /// <summary>
        /// Set/alter one bitangent of a single vertex.
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="biTangent"></param>
        public void SetBiTangent(int idx, float3 biTangent)
        {
            Guard.IsNotNull(_biTangents, nameof(_biTangents));
            Guard.IsInRange(idx, 0, _biTangents.Length, nameof(idx));
            _biTangents[idx] = biTangent;
            MeshChanged?.Invoke(this, new MeshChangedEventArgs(this, MeshChangedEnum.BiTangents));
        }

        /// <summary>
        /// Gets a value indicating whether bi tangents are set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if bi tangents are set; otherwise, <c>false</c>.
        /// </value>
        public bool BiTangentsSet => _biTangents?.Length > 0;

        /// <summary>
        /// The bounding box of this geometry chunk.
        /// </summary>
        public AABBf BoundingBox;

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