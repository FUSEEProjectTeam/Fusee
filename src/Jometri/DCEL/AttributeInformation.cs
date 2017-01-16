using Fusee.Math.Core;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// Contains additional information that can be added to vertices.
    /// </summary>
    public struct VertexData
    {
        /// <summary>
        /// The vertex' position.
        /// </summary>
        public float3 Pos;
    }

    /// <summary>
    /// Contains additional information that can be added to half edges.
    /// </summary>
    public struct HalfEdgeData
    {
        /// <summary>
        /// The normal belonging to this half edge.
        /// </summary>
        public float3 Normal;

        /// <summary>
        /// Texture coordinate.
        /// </summary>
        public float2 TextureCoord;
    }

    /// <summary>
    /// Contains additional information that can be added to faces.
    /// </summary>
    public struct FaceData
    {
        /// <summary>
        /// The normal of the face.
        /// </summary>
        public float3 FaceNormal;
    }
}
