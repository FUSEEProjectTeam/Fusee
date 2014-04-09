using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Fusee.Math;
using JSIL.Meta;

namespace Fusee.Engine
{
    /// <summary>
    ///     This class is used to load 3D Models in .obj format into memory.
    /// </summary>
    public static class MeshReader
    {
        #region Members

        /// <summary>
        ///     Replacement for double.Parse(s, [InvariantCulture])
        ///     Hack needed for JSIL.
        /// </summary>
        /// <param name="s">string to parse</param>
        /// <returns>A double number</returns>
        // TODO: Get rid of this hack
        [JSExternal]
        public static double Double_Parse(string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }

        /// <summary>
        ///     Parses the contents of the TextReader object passed to the method and tries to
        ///     interpret the contents as a <a href="http://en.wikipedia.org/wiki/Wavefront_.obj_file">Wavefront obj</a> file.
        ///     Creates a Mesh object from the contents.
        /// </summary>
        /// <param name="tr">The initialized TextReader (can be either a StreamReader or a StringReader)</param>
        /// <returns>The newly created Mesh object</returns>
        public static Geometry ReadWavefrontObj(TextReader tr)
        {
            const int nMaxInx = 256;
            var g = new Geometry();

            int nFaceRefs = 0;
            int lineNumber = 1;

            //Read the first line of text
            string line = tr.ReadLine();

            //Continue to read until you reach end of file
            while (line != null)
            {
                if (line.StartsWith("#"))
                {
                    // Nothing to read, these are comments.
                }
                else if (line.StartsWith("vt"))
                {
                    // Vertext texcoord.
                    string tmp = line.Substring(3);

                    string[] values = FilteredSplit(tmp, null);

                    g.AddTexCoord(new double2(Double_Parse(values[0]),
                        Double_Parse(values[1])));
                }
                else if (line.StartsWith("vn"))
                {
                    // Normals
                    string tmp = line.Substring(3);

                    string[] values = FilteredSplit(tmp, null);

                    g.AddNormal(new double3(Double_Parse(values[0]),
                                            Double_Parse(values[1]),
                                            -Double_Parse(values[2]))); // convert to lefthanded
                }
                else if (line.StartsWith("v"))
                {
                    // Positions.
                    string tmp = line.Substring(2);

                    string[] values = FilteredSplit(tmp, null);

                    g.AddVertex(new double3(Double_Parse(values[0]),
                                            Double_Parse(values[1]),
                                            -Double_Parse(values[2]))); // convert to lefthanded
                }
                else if (line.StartsWith("f"))
                {
                    // Face
                    string tmp = line.Substring(2);
                    string[] values = FilteredSplit(tmp, null);

                    if (!(3 <= values.Length && values.Length < nMaxInx))
                    {
                        throw new ArgumentOutOfRangeException("Error reading obj file (" + lineNumber +
                                            "). Face definition number of vertices must be within [3.." + nMaxInx + "].");
                    }

                    var vI = new int[values.Length];
                    int[] nI = null;
                    int[] tI = null;
                    int i = 0;
                    foreach (var vRef in values)
                    {
                        string[] vDef = vRef.Split('/');
                        if (nFaceRefs == 0)
                        {
                            if (!(1 <= vDef.Length && vDef.Length <= 3))
                                throw new ArgumentOutOfRangeException("Error reading obj file (" + lineNumber +
                                                    "). Face definitions must contain 1, 2 or 3 indices per vertex");
                            nFaceRefs = vDef.Length;
                        }
                        else
                        {
                            if (vDef.Length != nFaceRefs)
                                throw new ArgumentOutOfRangeException("Error reading obj file (" + lineNumber +
                                                    "). Inconsistent face definitions");
                        }

                        vI[i] = int.Parse(vDef[0]) - 1;

                        if (vDef.Length > 1 && !String.IsNullOrEmpty(vDef[1]))
                        {
                            if (tI == null)
                                tI = new int[values.Length];
                            tI[i] = int.Parse(vDef[1]) - 1;
                        }

                        if (vDef.Length > 2)
                        {
                            if (String.IsNullOrEmpty(vDef[1]))
                                throw new FormatException("Error reading obj file (" + lineNumber +
                                                    "). Syntax error in face definition");

                            if (nI == null)
                                nI = new int[values.Length];
                            nI[i] = int.Parse(vDef[2]) - 1;
                        }
                        i++;
                    }

                    g.AddFace(vI, tI, nI);
                }
                else if (line.StartsWith("mtllib"))
                {
                    /* TODO;
                    mtlFile = line.Substring(7);

                    // Then load it now so that we have our materials ready
                    LoadMtl(d3ddevice, mtlFile);
                     * */
                }
                else if (line.StartsWith("usemtl"))
                {
                    /* TODO:
                    bool found = false;

                    string matName = line.Substring(7);

                    for (int i = 0; i < materials.Count; i++)
                    {
                        if (matName.Equals(materials.name))
                        {
                            found = true;
                            currSubset = i;
                        }
                    }

                    if (!found)
                    {
                        throw new Exception("Materials are already loaded so we should have it!");
                    }*/
                }

                //Read the next line
                line = tr.ReadLine();
                lineNumber++;
            }

            if (!g.HasNormals)
                g.CreateNormals(80*3.141592/180.0);
            return g;
        }

        /// <summary>
        ///     This method loads an object file and returns it as a mesh.
        /// </summary>
        /// <param name="path">Path to the object to load</param>
        /// <returns>The newly created Mesh object</returns>
        public static Mesh LoadMesh(string path)
        {
            using (var obj = new StreamReader(path))
            {
                Geometry geo = ReadWavefrontObj(obj);
                return geo.ToMesh();
            }
        }

        /// <summary>
        ///     This method is used to split a string in a list of strings based on the separator passed to the method.
        /// </summary>
        /// <param name="strIn">The string.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>An array of string with all separated values.</returns>
        public static string[] FilteredSplit(string strIn, char[] separator)
        {
            // Sometime if we have a white space at the beginning of the string, split
            // will return an empty string. Let's remove that.
            return strIn.Split(separator).Where(str => str.Length > 0).ToArray();
        }

        #endregion
    }
}

#region Unused Code

/*
   namespace Fusee.Engine
{

  /// <summary>
  /// This structure is used to store material information.
  /// </summary>
  public struct MaterialFromObj
  {
      public string name;

      public Vector3 vAmbient;
      public Vector3 vDiffuse;
      public Vector3 vSpecular;

      public int nShininess;
      public float fAlpha;

      public bool bSpecular;

      public string strTexture;
      public Texture texture;
  }

 

      /// <summary>
      /// This method is used to load information stored in .mtl files referenced by the .obj file.
      /// </summary>
      /// <param name="d3ddevice"></param>
      /// <param name="file"></param>
      public void LoadMtl(Device d3ddevice, string file)
      {
          MaterialFromObj currentMaterial = new MaterialFromObj();
          bool first = true;

          //Pass the file path and file name to the StreamReader constructor
          StreamReader sr = new StreamReader(file);

          //Read the first line of text
          string line = sr.ReadLine();

          //Continue to read until you reach end of file
          while (line != null)
          {
              if (line.StartsWith("#"))
              {
                  // Nothing to read, these are comments.
              }
              else if (line.StartsWith("newmtl"))
              {
                  if (!first)
                  {
                      materials.Add(currentMaterial);
                      currentMaterial = new MaterialFromObj();
                  }
                  first = false;
                  currentMaterial.name = line.Substring(7);
              }
              else if (line.StartsWith("Ka"))
              {
                  string tmp = line.Substring(3);

                  string[] values = FilteredSplit(tmp, null);


                  currentMaterial.vAmbient = new Vector3(float.Parse(values[0]),
                                              float.Parse(values[1]),
                                              float.Parse(values[2]));
              }
              else if (line.StartsWith("Kd"))
              {
                  string tmp = line.Substring(3);

                  string[] values = FilteredSplit(tmp, null);

                  currentMaterial.vDiffuse = new Vector3(float.Parse(values[0]),
                          float.Parse(values[1]),
                          float.Parse(values[2]));
              }

              else if (line.StartsWith("Ks"))
              {
                  string tmp = line.Substring(3);

                  string[] values = FilteredSplit(tmp, null);

                  currentMaterial.vSpecular = new Vector3(float.Parse(values[0]),
                          float.Parse(values[1]),
                          float.Parse(values[2]));
              }
              else if (line.StartsWith("map_Kd"))
              {
                  string tmp = line.Substring(7);

                  //currentMaterial.texture = ResourceCache.GetGlobalInstance().CreateTextureFromFile(d3ddevice, tmp);

              }

              //Read the next line
              line = sr.ReadLine();
          }
          materials.Add(currentMaterial);

          //close the file
          sr.Close();
      }
*/

#endregion