// The only purpose of these implementations are to be cross-compiled with JSIL. 
// Implementations of class elemets can be found in handcoded .js files.

using System;
using Fusee.Engine.Common;
using JSIL.Meta;

namespace Fusee.Engine.Imp.Graphics.Web
{
    public class MeshImp : IMeshImp, IDisposable
    {
        [JSExternal]
        public MeshImp()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void InvalidateVertices()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool VerticesSet { get; }
        [JSExternal]
        public void InvalidateNormals()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool NormalsSet { get; }
        [JSExternal]
        public void InvalidateColors()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool ColorsSet { get; }
        [JSExternal]
        public void InvalidateTriangles()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool TrianglesSet { get; }
        [JSExternal]
        public bool UVsSet { get; }
        [JSExternal]
        public void InvalidateUVs()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public void InvalidateBoneWeights()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool BoneWeightsSet { get; }
        [JSExternal]
        public void InvalidateBoneIndices()
        {
            throw new System.NotImplementedException();
        }

        [JSExternal]
        public bool BoneIndicesSet { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
