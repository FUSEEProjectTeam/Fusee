using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Math;
using OpenTK.Graphics.OpenGL;

namespace Fusee.Engine
{
    public class MeshImp : IMeshImp
    {
        internal int VertexBufferObject;
        internal int NormalBufferObject;
        internal int ColorBufferObject;
        internal int ElementBufferObject;
        internal int NElements;

        public void InvalidateVertices()
        {
            VertexBufferObject = 0;
        }
        public bool VerticesSet { get { return VertexBufferObject != 0; } }

        public void InvalidateNormals()
        {
            NormalBufferObject = 0;
        }
        public bool NormalsSet { get { return NormalBufferObject != 0; } }

        public void InvalidateColors()
        {
            ColorBufferObject = 0;
        }
        public bool ColorsSet { get { return ColorBufferObject != 0; } }

        public void InvalidateTriangles()
        {
            ElementBufferObject = 0;
            NElements = 0;
        }
        public bool TrianglesSet { get { return ElementBufferObject != 0; } }

    }
}
