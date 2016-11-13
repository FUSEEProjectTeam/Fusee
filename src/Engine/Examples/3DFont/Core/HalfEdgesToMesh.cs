using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Engine.Core;
using Geometry = Fusee.Jometri.DCEL.Geometry;

namespace Fusee.Engine.Examples.ThreeDFont.Core
{
    public class HalfEdgesToMesh : Mesh
    {
        public HalfEdgesToMesh(Geometry geometry)
        {
            ConvertToMesh(geometry);
        }

        //geometry has to be trinagulated
        private void ConvertToMesh(Geometry geometry)
        {
            foreach (var face in geometry.FaceHandles)
            {
                var faceVerts = geometry.GetFaceVertices(face).ToList();

            }
        }
    }
}
