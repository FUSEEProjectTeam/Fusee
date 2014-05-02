using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;
using Fusee.Serialization;

namespace FuExport
{
    class VertexList : Collection<float3>
    {
        public float3 Min;
        public float3 Max;

        public VertexList() : base()
        {
        }

        public new void Add(float3 v)
        {
            if (Count == 0)
            {
                Min = v;
                Max = v;
            }
            else
            {
                Min.x = (v.x < Min.x) ? v.x : Min.x;
                Min.y = (v.y < Min.y) ? v.y : Min.y;
                Min.z = (v.z < Min.z) ? v.z : Min.z;
                Max.x = (v.x > Max.x) ? v.x : Max.x;
                Max.y = (v.y > Max.y) ? v.y : Max.y;
                Max.z = (v.z > Max.z) ? v.z : Max.z;
            }
            base.Add(v);
        }
    }

    class FusConverter
    {
        private BaseDocument _polyDoc;
        private List<string> _textureFiles = new List<string>();
        private string _sceneRootDir;
        private Dictionary<long, MaterialContainer> _materialCache;

        public SceneContainer FuseefyScene(BaseDocument doc, string sceneRootDir, out List<string> textureFiles)
        {
            _materialCache = new Dictionary<long, MaterialContainer>();
            _sceneRootDir = sceneRootDir;
            _polyDoc = doc.Polygonize();

            Logger.Debug("Fuseefy Me!");

            String userName;
            using (BaseContainer machineFeatures = C4dApi.GetMachineFeatures())
            {
                GeData userNameData = machineFeatures.GetDataPointer(C4dApi.MACHINEINFO_USERNAME);
                userName = userNameData.GetString();
            }

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
            textureFiles = _textureFiles;
            return root;
        }


        private static float3 AdjustNormal(float3 normalOrig, float3 normalFace)
        {
            if (float3.Dot(normalOrig, normalFace) < 0)
            {
                return -1.0f*normalOrig;
            }
            else
            {
                return normalOrig;
            }
        }

        private static MeshContainer GetMesh(PolygonObject polyOb, float3[] normalsOb, UVWTag uvwTag, IEnumerable<int> range)
        {
            List<float3> normals = new List<float3>();

            ushort nNewVerts = 0;
            VertexList verts = new VertexList();
            List<float2> uvs = new List<float2>();
            List<ushort> tris = new List<ushort>();

            foreach (int i in range)
            {
                int iNorm = i*4;
                double3 a, b, c, d;
                using (CPolygon poly = polyOb.GetPolygonAt(i))
                {
                    a = polyOb.GetPointAt(poly.a);
                    b = polyOb.GetPointAt(poly.b);
                    c = polyOb.GetPointAt(poly.c);
                    d = polyOb.GetPointAt(poly.d);
                }

                float2 uvA = new float2(0, 0);
                float2 uvB = new float2(0, 1);
                float2 uvC = new float2(1, 1);
                float2 uvD = new float2(1, 0);

                if (uvwTag != null)
                {
                    using (UVWStruct uvw = uvwTag.GetSlow(i))
                    {
                        uvA = new float2((float) uvw.a.x, 1.0f - (float) uvw.a.y);
                        uvB = new float2((float) uvw.b.x, 1.0f - (float) uvw.b.y);
                        uvC = new float2((float) uvw.c.x, 1.0f - (float) uvw.c.y);
                        uvD = new float2((float) uvw.d.x, 1.0f - (float) uvw.d.y);
                    }
                }

                verts.Add((float3) a);
                verts.Add((float3) b);
                verts.Add((float3) c);

                uvs.Add(uvA);
                uvs.Add(uvB);
                uvs.Add(uvC);

                float3 faceNormal = CalcFaceNormal((float3)a, (float3)b, (float3)c);
                float3 normalD;
                if (normalsOb != null)
                {
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normals.Add(AdjustNormal(normalsOb[iNorm++], faceNormal));
                    normalD = AdjustNormal(normalsOb[iNorm++], faceNormal);
                }
                else
                {
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
                    uvs.Add(uvD);
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
                UVs = uvs.ToArray(),
                BoundingBox = new AABBf(verts.Min, verts.Max)
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
                string selRef = "";
                using (BaseContainer texData = texture.GetData())
                {
                    selRef = texData.GetString(C4dApi.TEXTURETAG_RESTRICTION);
                }
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
                        subSoc.Transform = new TransformContainer()
                        {
                            Translation = new float3(0, 0, 0),
                            Rotation = new float3(0, 0, 0), 
                            Scale = new float3(1, 1, 1)
                        };
                        subSoc.Material = GetMaterial(texSelItem.Value);
                        subSoc.Name = soc.Name + "_" + texSelItem.Key.GetName();
                        subSoc.Mesh = GetMesh(polyOb, normalOb, uvwTag, polyInxsSubset);

                        soc.Children.Add(subSoc);
                    }
                }

                // The remaining polygons directly go into the original mesh
                soc.Mesh = GetMesh(polyOb, normalOb, uvwTag, polyInxs);
            }
        }

        private MaterialContainer GetMaterial(TextureTag texTag)
        {
            Material material = texTag.GetMaterial() as Material;
            if (material == null)
                return null;

            long materialUid = material.RefUID();
            MaterialContainer mcRet;
            if (_materialCache.TryGetValue(materialUid, out mcRet))
                return mcRet;

            using (BaseContainer materialData = material.GetData())
            {
                mcRet = new MaterialContainer();
                // Just for debugging purposes
                for (int i = 0, id = 0; -1 != (id = materialData.GetIndexId(i)); i++)
                {
                    if (materialData.GetType(id) == C4dApi.DA_LONG)
                    {
                        int iii = materialData.GetInt32(id);
                    }
                    if (materialData.GetType(id) == C4dApi.DA_REAL)
                    {
                        double d = materialData.GetFloat(id);
                    }
                    else if (materialData.GetType(id) == C4dApi.DA_VECTOR)
                    {
                        double3 v = materialData.GetVector(id);
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_COLOR))
                {
                    BaseChannel diffuseChannel = material.GetChannel(C4dApi.CHANNEL_COLOR);
                    if (diffuseChannel != null)
                    {
                        mcRet.Diffuse = new MatChannelContainer();

                        using (BaseContainer data = diffuseChannel.GetData())
                        {
                            mcRet.Diffuse.Color = (float3) (data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Diffuse.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Diffuse.Texture = GetTexture(data, diffuseChannel);
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_SPECULARCOLOR))
                {
                    BaseChannel specularChannel = material.GetChannel(C4dApi.CHANNEL_SPECULARCOLOR);
                    if (specularChannel != null)
                    {
                        mcRet.Specular = new SpecularChannelContainer();

                        using (BaseContainer data = specularChannel.GetData())
                        {
                            mcRet.Specular.Color = (float3)(data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Specular.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Specular.Texture = GetTexture(data, specularChannel);
                            mcRet.Specular.Shininess =
                                CalculateShininess((float)materialData.GetFloat(C4dApi.MATERIAL_SPECULAR_WIDTH));
                            mcRet.Specular.Intensity = (float)(1.5 * materialData.GetFloat(C4dApi.MATERIAL_SPECULAR_HEIGHT));
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_LUMINANCE))
                {
                    BaseChannel emissiveChannel = material.GetChannel(C4dApi.CHANNEL_LUMINANCE);
                    if (emissiveChannel != null)
                    {
                        mcRet.Emissive = new MatChannelContainer();

                        using (BaseContainer data = emissiveChannel.GetData())
                        {
                            mcRet.Emissive.Color = (float3)(data.GetVector(C4dApi.BASECHANNEL_COLOR_EX) * data.GetFloat(C4dApi.BASECHANNEL_BRIGHTNESS_EX));
                            mcRet.Emissive.Mix = (float)data.GetFloat(C4dApi.BASECHANNEL_MIXSTRENGTH_EX);
                            mcRet.Emissive.Texture = GetTexture(data, emissiveChannel);
                        }
                    }
                }

                if (material.GetChannelState(C4dApi.CHANNEL_NORMAL))
                {
                    BaseChannel bumpChannel = material.GetChannel(C4dApi.CHANNEL_NORMAL);
                    if (bumpChannel != null)
                    {
                        mcRet.Bump = new BumpChannelContainer();

                        using (BaseContainer data = bumpChannel.GetData())
                        {
                            mcRet.Bump.Intensity = (float)materialData.GetFloat(C4dApi.MATERIAL_NORMAL_STRENGTH);
                            mcRet.Bump.Texture = GetTexture(data, bumpChannel);
                        }
                    }
                }
            }

            /*
            // Just for debugging purposes
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
            */
            _materialCache[materialUid] = mcRet;
            return mcRet;
        }

        private static float CalculateShininess(float p)
        {
            // The minimum shininess
            float minS = 2.0f;

            // The maximum shininess
            float maxS = 5000;

            return (float)(minS*Math.Pow(maxS/minS, 1.0 - p));
        }

        private string GetTexture(BaseContainer data, BaseChannel matChannel)
        {
            string resultName = null;
            string texture = data.GetString(C4dApi.BASECHANNEL_TEXTURE);
            if (!string.IsNullOrEmpty(texture))
            {
                texture = Path.GetFileNameWithoutExtension(texture);
                texture += ".jpg";
                resultName = texture;

                if (!_textureFiles.Contains(texture))
                {
                    matChannel.InitTexture(new InitRenderStruct(_polyDoc));
                    using (BaseBitmap bitmap = matChannel.GetBitmap())
                    {
                        if (bitmap != null)
                        {
                            using (BaseContainer compressionContainer = new BaseContainer(C4dApi.JPGSAVER_QUALITY))
                            {
                                bool bRet = bitmap.SetColorProfile(ColorProfile.GetDefaultSRGB());
                                compressionContainer.SetFloat(C4dApi.JPGSAVER_QUALITY, 70.0);
                                string textureFileAbs = Path.Combine(_sceneRootDir, texture);
                                bitmap.Save(new Filename(textureFileAbs), C4dApi.FILTER_JPG, compressionContainer,
                                    SAVEBIT.SAVEBIT_0);
                                _textureFiles.Add(texture);
                            }
                        }
                        else
                        {
                            resultName = null;
                        }
                    }
                    matChannel.FreeTexture();
                }
            }
            return resultName;
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
                float3 rotC4d = (float3) ob.GetRelRot();
                soc.Transform = new TransformContainer{
                    Translation = (float3) ob.GetRelPos(),
                    Rotation = new float3(-rotC4d.y, -rotC4d.x, -rotC4d.z),
                    Scale = (float3) ob.GetRelScale()
                };
                /*
                double4x4 mtxD = ob.GetMl();
                soc.Transform = (float4x4) mtxD;
                */

                VisitObject(ob, soc);

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
