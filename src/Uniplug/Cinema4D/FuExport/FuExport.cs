using System;
using System.Collections.Generic;
using System.IO;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    [SceneSaverPlugin(1000048,
       Name = "FUSEE 3D (*.fus)",
       Suffix = "fus")
    ]

    // The main class for your plugin
    public class FuExporter : SceneSaverData
    {

        /* Your Executeable code for the plugin
         * This is your main function
         * Never change return type - this is declarated in c++ code
         */

        public override bool Init(GeListNode node)
        {
            return true;
        }


        public override FILEERROR Save(BaseSceneSaver node, Filename name, BaseDocument doc, SCENEFILTER filterflags)
        {
            BaseDocument polyDoc = doc.Polygonize();

            Logger.Debug("Fuseefy Me!");

            SceneContainer root = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = "",
                    Generator = "FUSEE Export Plugin for Cinema4D",
                    Version = 1
                },
                Children = FuseefyOb(polyDoc.GetFirstObject())
            };

            var ser = new Serializer();
            using (var file = File.Create(name.GetString()))
            {
                ser.Serialize(file, root);
            }

            return FILEERROR.FILEERROR_NONE;
        }

        private List<SceneObjectContainer> FuseefyOb(BaseObject ob)
        {
            if (ob == null)
                return null;

            List<SceneObjectContainer> ret = new List<SceneObjectContainer>();
            do
            {
                SceneObjectContainer soc = new SceneObjectContainer();

                double4x4 mtxD = ob.GetMl();
                soc.Transform = (float4x4) mtxD;
                PolygonObject polyOb = ob as PolygonObject;
                if (polyOb != null)
                {
                    /*double3  padr = ToPoly(op)->GetPointR();
                    CPolygon vadr = ToPoly(op)->GetPolygonR();*/
                    int nPolys = polyOb.GetPolygonCount();
                    ushort nNewVerts = 0;
                    List<float3> verts = new List<float3>();
                    List<ushort> tris = new List<ushort>();

                    for (int i = 0; i < nPolys; i++)
                    {
                        CPolygon poly = polyOb.GetPolygonAt(i);
                        double3 a = polyOb.GetPointAt(poly.a);
                        double3 b = polyOb.GetPointAt(poly.b);
                        double3 c = polyOb.GetPointAt(poly.c);
                        double3 d = polyOb.GetPointAt(poly.d);

                        verts.Add((float3)a);
                        verts.Add((float3)b);
                        verts.Add((float3)c);

                        tris.AddRange(new ushort[] {nNewVerts, (ushort)(nNewVerts+1), (ushort)(nNewVerts+2)});

                        if (c != d)
                        {
                            // The Polyogon is not only a triangle, but a quad. Add the second triangle.
                            verts.Add((float3)d);
                            tris.AddRange(new ushort[] { nNewVerts, (ushort)(nNewVerts + 2), (ushort)(nNewVerts + 3) });
                            nNewVerts += 1;
                        }
                        nNewVerts += 3;
                    }
                    soc.Mesh = new MeshContainer()
                    {
                        Vertices = verts.ToArray(),
                        Triangles = tris.ToArray(),
                    };
                }
                soc.Children = FuseefyOb(ob.GetDown());
                ret.Add(soc);
                ob = ob.GetNext();
            } while (ob != null);
            return ret;
        }
    }
}
