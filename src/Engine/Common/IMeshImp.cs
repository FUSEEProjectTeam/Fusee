using ProtoBuf;
namespace Fusee.Engine
{
    /// <summary>
    /// Interface for Mesh implementations. The implementation should handle typical mesh informations like: vertices, triangles, normals, colors, UV's.
    /// It is also required to implement a connection to the current Rendercontext in order to apply the Mesh for rendering.
    /// The Mesh should preferable use handles for its informations in order to communicate with a rendercontext. The handles are refering to so called BufferObjects.
    /// </summary>
    
    [ProtoContract]
    public interface IMeshImp
    {
        /// <summary>
        /// Implementation Task: Invalidates the vertices of the mesh, e.g. reset the VertexBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateVertices();

        /// <summary>
        /// Implementation Tasks: Get a value indicating whether [vertices set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if VertexBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool VerticesSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the normals of the mesh, e.g. reset the NormalBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateNormals();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [normals set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if NormalBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool NormalsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the colors, e.g. reset the ColorBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateColors();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [colors set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if ColorBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool ColorsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the triangles, e.g. reset the ElementBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateTriangles();

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [triangles set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if ElementBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool TrianglesSet { get; }

        /// <summary>
        /// Implementation Tasks: Gets a value indicating whether [UVs set].
        /// </summary>
        /// <value>
        ///   <c>true</c> if UVBufferObject is not 0; otherwise, <c>false</c>.
        /// </value>
        bool UVsSet { get; }

        /// <summary>
        /// Implementation Tasks: Invalidates the UV's, e.g. reset the UVBufferObject of this instance by setting it to 0.
        /// </summary>
        void InvalidateUVs();
    }
}