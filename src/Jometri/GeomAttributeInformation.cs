using Fusee.Math.Core;

namespace Fusee.Jometri
{
    /// <summary>
    /// Contains additional information that can be added to Vertices.
    /// </summary>
    public struct VertexData
    {
        /// <summary>
        /// The position of the Vertex.
        /// </summary>
        public float3 Pos;
    }

    /// <summary>
    /// Contains additional information that can be added to HalfEdges.
    /// </summary>
    public struct HalfEdgeData
    {
        /// <summary>
        /// The normal belonging to this HalfEdge.
        /// </summary>
        public float3 Normal;

        /// <summary>
        /// Texture coordinates.
        /// </summary>
        public float2 TextureCoord;
    }

    /// <summary>
    /// Contains additional information that can be added to Faces.
    /// </summary>
    public struct FaceData
    {
        /// <summary>
        /// The normal of the Face.
        /// </summary>
        public float3 FaceNormal;
    }
}
