using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    class FusConverter
    {
        private BaseDocument _polyDoc;

        public SceneContainer FuseefyScene(BaseDocument doc)
        {

            _polyDoc = doc.Polygonize();

            Logger.Debug("Fuseefy Me!");

            BaseContainer machineFeatures = C4dApi.GetMachineFeatures();
            GeData userNameData = machineFeatures.GetDataPointer(C4dApi.MACHINEINFO_USERNAME);
            String userName = userNameData.GetString();

            SceneContainer root = new SceneContainer()
            {
                Header = new SceneHeader()
                {
                    CreatedBy = userName,
                    Generator = "FUSEE Export Plugin for Cinema4D",
                    Version = 1,
                    CreationDate = DateTime.Now.ToString("d-MMM-yyyy", CultureInfo.CreateSpecificCulture("en-US"))
                },
                Children = FuseefyOb(_polyDoc.GetFirstObject())
            };
            return root;
        }


        private static MeshContainer GetMesh(PolygonObject polyOb, float3[] normalsOb, IEnumerable<int> range)
        {
            List<float3> normals = new List<float3>();

            ushort nNewVerts = 0;
            List<float3> verts = new List<float3>();
            List<ushort> tris = new List<ushort>();

            foreach (int i in range)
            {
                int iNorm = i*4;
                CPolygon poly = polyOb.GetPolygonAt(i);
                double3 a = polyOb.GetPointAt(poly.a);
                double3 b = polyOb.GetPointAt(poly.b);
                double3 c = polyOb.GetPointAt(poly.c);
                double3 d = polyOb.GetPointAt(poly.d);

                verts.Add((float3) a);
                verts.Add((float3) b);
                verts.Add((float3) c);

                float3 normalD;
                if (normalsOb != null)
                {
                    normals.Add(normalsOb[iNorm++]);
                    normals.Add(normalsOb[iNorm++]);
                    normals.Add(normalsOb[iNorm++]);
                    normalD = normalsOb[iNorm++];
                }
                else
                {
                    float3 faceNormal = CalcFaceNormal((float3) a, (float3) b, (float3) c);
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normals.Add(faceNormal);
                    normalD = faceNormal;
                }

                tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 2), (ushort) (nNewVerts + 1)});

                if (c != d)
                {
                    // The Polyogon is not a triangle, but a quad. Add the second triangle.
                    verts.Add((float3) d);
                    normals.Add(normalD);
                    tris.AddRange(new ushort[] {nNewVerts, (ushort) (nNewVerts + 3), (ushort) (nNewVerts + 2)});
                    nNewVerts += 1;
                }
                nNewVerts += 3;
            }
            return new MeshContainer()
            {
                Normals = normals.ToArray(),
                Vertices = verts.ToArray(),
                Triangles = tris.ToArray(),
            };
        }

        public static float3 CalcFaceNormal(float3 vert0, float3 vert1, float3 vert2)
        {
            float3 v1 = vert0 - vert1;
            float3 v2 = vert0 - vert2;
            return float3.Normalize(float3.Cross(v1, v2));
        }

        /// <summary>
        /// This method tries to make the best out of C4Ds seldom relationship between objects with 
        /// multiple materials which can or can not be restricted to polygon selections and one or more UV sets.
        /// But there are unhandled cases:
        /// Multple UV tags are not supported. Overlapping polygon selections are probably handled differently.
        /// The awkward side effects when changing the tags' order in C4D are not reproduced.
        /// </summary>
        /// <param name="ob"></param>
        /// <param name="soc"></param>
        private void VisitObject(BaseObject ob, SceneObjectContainer soc)
        {
            Collection<TextureTag> textureTags = new Collection<TextureTag>();
            Dictionary<string,  SelectionTag> selectionTags = new Dictionary<string, SelectionTag>();
            UVWTag uvwTag = null;

            // Iterate over the object's tags 
            for (BaseTag tag = ob.GetFirstTag(); tag != null; tag = tag.GetNext())
            {
                // TextureTag (Material - there might be more than one)
                TextureTag tex = tag as TextureTag;
                if (tex != null)
                {
                    textureTags.Add(tex);
                    continue;
                }
                // UVWTag - the texutre coordinates. We handle only one
                UVWTag uvw = tag as UVWTag;
                if (uvw != null)
                {
                    if (uvwTag == null)
                    {
                        uvwTag = uvw;
                    }
                    else
                    {
                        Logger.Error("Object " + ob.GetName() + " contains more than one texture tags. Cannot handle this. Only the first texture tag will be recognized.");
                    }
                    continue;
                }
                // Selection tag. Only recognize the polygon selections as they might be referenced in a TextureTag
                SelectionTag selection = tag as SelectionTag;
                string selTagName = tag.GetName();
                if (selection != null && selection.GetType() == C4dApi.Tpolygonselection && !string.IsNullOrEmpty(selTagName))    // One Type and three TypeIDs - You C4D programmer guys really suck
                {
                    selectionTags[selTagName] = selection;
                }
                // XPresso Tags - TBD
                XPressoTag xPresso = tag as XPressoTag;
                if (xPresso != null)
                {
                    // Handle XPresso tag
                    continue;
                }
            }

            TextureTag lastUnselectedTag = null;
            Collection<KeyValuePair<SelectionTag, TextureTag>> texSelList = new Collection<KeyValuePair<SelectionTag, TextureTag>>(); // Abused KeyValuePair. Should have been Pair...
            // Now iterate over the textureTags 
            foreach (TextureTag texture in textureTags)
            {
                BaseContainer texData = texture.GetData();
                string selRef = texData.GetString(C4dApi.TEXTURETAG_RESTRICTION);
                if (string.IsNullOrEmpty(selRef))
                {
                    // This material is not restricted to any polygon selection
                    lastUnselectedTag = texture;
                }
                else
                {
                    SelectionTag sel;
                    if (selectionTags.TryGetValue(selRef, out sel))
                    {
                        texSelList.Add(new KeyValuePair<SelectionTag, TextureTag>(sel, texture));
                    }
                }
            }

            // At this point we have the last texture tag not restricted to a seletion. This will become the Material of this FuseeObjectContainer
            // no matter if this object contains geometry or not
            if (lastUnselectedTag != null)
                soc.Material = GetMaterial(lastUnselectedTag);

            // Further processing only needs to take place if the object contains any geometry at all.
            PolygonObject polyOb = ob as PolygonObject;
            if (polyOb != null)
            {
                float3[] normalOb = polyOb.CreatePhongNormals();

                // Initialize the polygon index set
                int nPolys = polyOb.GetPolygonCount();
                HashSet<int> polyInxs = new HashSet<int>();
                for (int i = 0; i < nPolys; i++)
                    polyInxs.Add(i);

                foreach (KeyValuePair<SelectionTag, TextureTag> texSelItem in texSelList)
                {
                    HashSet<int> polyInxsSubset = new HashSet<int>();
                    BaseSelect bs = texSelItem.Key.GetBaseSelect();
                    int nSegments = bs.GetSegments();
                    for (int iSeg = 0; iSeg < nSegments; iSeg++)
                    {
                        int from = bs.GetRangeA(iSeg);
                        int to = bs.GetRangeB(iSeg);
                        for (int iSel = from; iSel <= to; iSel++)
                        {
                            polyInxs.Remove(iSel);
                            polyInxsSubset.Add(iSel);
                        }
                    }

                    // Now generate Polygons for this subset
                    if (polyInxsSubset.Count > 0)
                    {
                        if (soc.Children == null)
                            soc.Children = new List<SceneObjectContainer>();

                        SceneObjectContainer subSoc = new SceneObjectContainer();
                        subSoc.Transform = float4x4.Identity;
                        subSoc.Material = GetMaterial(texSelItem.Value);
                        subSoc.Name = soc.Name + "_" + texSelItem.Key.GetName();
                        subSoc.Mesh = GetMesh(polyOb, normalOb, polyInxsSubset);

                        soc.Children.Add(subSoc);
                    }
                }

                // The remaining polygons directly go into the original mesh
                soc.Mesh = GetMesh(polyOb, normalOb, polyInxs);
            }
        }

        private MaterialContainer GetMaterial(TextureTag texTag)
        {
            BaseMaterial material = texTag.GetMaterial();
            MaterialContainer mcRet = new MaterialContainer();
                
            BaseChannel diffuseChannel = material.GetChannel(C4dApi.CHANNEL_COLOR);
            if (diffuseChannel != null)
            {
                mcRet.HasDiffuse = true;
                BaseContainer data = diffuseChannel.GetData();
                mcRet.DiffuseColor = (float3) data.GetVector(C4dApi.BASECHANNEL_COLOR_EX);
                string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
                diffuseChannel.InitTexture(new InitRenderStruct(_polyDoc));
                BaseBitmap bitmap = diffuseChannel.GetBitmap();
                if (bitmap != null)
                {
                    BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY);
                    compressionContainer.SetReal(C4dApi.JPGSAVER_QUALITY, 70.0);
                    bitmap.Save(new Filename("C:\\Users\\mch\\Temp\\FuseeWebPlayer\\ABitmap.jpg"), C4dApi.FILTER_JPG, compressionContainer, SAVEBIT.SAVEBIT_0);
                }
                diffuseChannel.FreeTexture();
            }

            BaseChannel specularColorChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
            if (specularColorChannel != null)
            {
                mcRet.HasSpecular = true;
                BaseContainer data = specularColorChannel.GetData();
                mcRet.SpecularColor = (float3) data.GetVector(C4dApi.BASECHANNEL_COLOR_EX);
                string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
                BaseBitmap bitmap = specularColorChannel.GetBitmap();
                if (bitmap != null)
                {
                    BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY);
                    compressionContainer.SetReal(C4dApi.JPGSAVER_QUALITY, 70.0);
                    bitmap.Save(new Filename("C:\\Users\\mch\\Temp\\FuseeWebPlayer\\ABitmapS.jpg"), C4dApi.FILTER_JPG, compressionContainer, SAVEBIT.SAVEBIT_0);
                }
            }

            BaseChannel specularChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
            if (specularChannel != null)
            {
                mcRet.HasSpecular = true;
                BaseContainer data = specularChannel.GetData();
                for (int i=0, id=0; -1 != (id = data.GetIndexId(i)); i++)
                {
                    if (data.GetType(id) == C4dApi.DA_REAL)
                    {
                        double d = data.GetReal(id);
                    }
                    else if (data.GetType(id) == C4dApi.DA_VECTOR)
                    {
                        double3 v = data.GetVector(id);
                    }
                } 
 
            }
            return mcRet;
        }


        private List<SceneObjectContainer> FuseefyOb(BaseObject ob)
        {
            if (ob == null)
                return null;

            List<SceneObjectContainer> ret = new List<SceneObjectContainer>();
            do
            {
                SceneObjectContainer soc = new SceneObjectContainer();

                soc.Name = ob.GetName();
                double4x4 mtxD = ob.GetMl();
                soc.Transform = (float4x4) mtxD;

                VisitObject(ob, soc);
                /*
                soc.Material = GetMaterial(ob);
                soc.Mesh = GetMesh(ob);
                */
                var childList = FuseefyOb(ob.GetDown());
                if (childList != null)
                {
                    if (soc.Children == null)
                    {
                        soc.Children = childList;
                    }
                    else
                    {
                        soc.Children.AddRange(childList);
                    }
                }
                ret.Add(soc);
                ob = ob.GetNext();
            } while (ob != null);
            return ret;
        }
    }
}
