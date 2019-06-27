using Fusee.Engine.Core;
using Fusee.Math.Core;
using Fusee.Pointcloud.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fusee.Pointcloud.OoCFileGen
{
    public class GridPtAccessor<TPoint> : PointAccessor<TPoint>
    {
        public bool HasGridIdx = true;

        public virtual ref int3 GetGridIdx(ref TPoint point)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support GetPositionFloat32");
        }

        public virtual void SetGridIdx(ref TPoint point, int3 val)
        {
            throw new NotSupportedException($"Point {typeof(TPoint).Name} does not support SetPositionFloat32");
        }

        public List<string> GetPointType()
        {
            return GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(this, null)).Select(p => p.Name).ToList();
        }
    }

    public class PtOctreeFileWriter<TPoint>
    {
        private string _fileFolderPath;

        public PtOctreeFileWriter(string pathToNodeFileFolder)
        {
            _fileFolderPath = pathToNodeFileFolder;
        }

        private void WritePos(BinaryWriter writer, GridPtAccessor<TPoint> ptAccessor, TPoint pt)
        {
            if(ptAccessor.HasPositionFloat3_32)
            {
                var ptPos = ptAccessor.GetPositionFloat3_32(ref pt);
                writer.Write(ptPos.x);
                writer.Write(ptPos.y);
                writer.Write(ptPos.z);

            }
            else if (ptAccessor.HasPositionFloat3_64)
            {
                var ptPos = ptAccessor.GetPositionFloat3_64(ref pt);
                writer.Write(ptPos.x);
                writer.Write(ptPos.y);
                writer.Write(ptPos.z);
            }
        }

        private void WriteNormal(BinaryWriter writer, GridPtAccessor<TPoint> ptAccessor, TPoint pt)
        {
            if (ptAccessor.HasNormalFloat3_32)
            {
                var ptNormal = ptAccessor.GetNormalFloat3_32(ref pt);
                writer.Write(ptNormal.x);
                writer.Write(ptNormal.y);
                writer.Write(ptNormal.z);

            }
            else if (ptAccessor.HasNormalFloat3_64)
            {
                var ptNormal = ptAccessor.GetNormalFloat3_64(ref pt);
                writer.Write(ptNormal.x);
                writer.Write(ptNormal.y);
                writer.Write(ptNormal.z);
            }
        }

        private void WriteIntensity(BinaryWriter writer, GridPtAccessor<TPoint> ptAccessor, TPoint pt)
        {
            if (ptAccessor.HasIntensityInt_8)
            {
                var ptIntensity = ptAccessor.GetIntensityInt_8(ref pt);
                writer.Write(ptIntensity); 
            }
            else if (ptAccessor.HasIntensityInt_16)
            {
                var ptIntensity = ptAccessor.GetIntensityInt_16(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityInt_32)
            {
                var ptIntensity = ptAccessor.GetIntensityInt_32(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityInt_64)
            {
                var ptIntensity = ptAccessor.GetIntensityInt_64(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityUInt_8)
            {
                var ptIntensity = ptAccessor.GetIntensityUInt_8(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityUInt_16)
            {
                var ptIntensity = ptAccessor.GetIntensityUInt_16(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityUInt_32)
            {
                var ptIntensity = ptAccessor.GetIntensityUInt_32(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityUInt_64)
            {
                var ptIntensity = ptAccessor.GetIntensityUInt_64(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityFloat32)
            {
                var ptIntensity = ptAccessor.GetIntensityFloat32(ref pt);
                writer.Write(ptIntensity);
            }
            else if (ptAccessor.HasIntensityFloat64)
            {
                var ptIntensity = ptAccessor.GetIntensityFloat64(ref pt);
                writer.Write(ptIntensity);
            }
        }

        private void WriteColor(BinaryWriter writer, GridPtAccessor<TPoint> ptAccessor, TPoint pt)
        {
            if (ptAccessor.HasColorInt_8)
            {
                var ptColor = ptAccessor.GetColorInt_8(ref pt);
                writer.Write(ptColor);
            }
            else if (ptAccessor.HasColorInt_16)
            {
                var ptColor = ptAccessor.GetColorInt_16(ref pt);
                writer.Write(ptColor);                
            }
            else if (ptAccessor.HasColorInt_32)
            {
                var ptColor = ptAccessor.GetColorInt_32(ref pt);
                writer.Write(ptColor);
            }
            else if (ptAccessor.HasColorInt_64)
            {
                var ptColor = ptAccessor.GetColorInt_64(ref pt);
                writer.Write(ptColor);
            }
        }

        private IEnumerable<TPoint> GetPointsFromGrid(PtOctant<TPoint> node)
        {
            foreach (var cell in node.Grid.GridCells)
            {
                if (cell != null)
                    yield return cell.Occupant;
            }
        }

        private string GetPathToFile(PtOctant<TPoint> node)
        {
            return _fileFolderPath + "\\" + GetFilename(node);
        }

        private string GetFilename(PtOctant<TPoint> node)
        {            
            var fileName = node.Guid.ToString("N");            

            fileName += ".node";
            return fileName;
        }

        /// <summary>
        /// Writes the contents, i.e. points, into a single file of the nodes folder.
        /// </summary>
        public void WriteNode(GridPtAccessor<TPoint> ptAccessor, PtOctant<TPoint> node)
        {
            var points = GetPointsFromGrid(node).ToList();

            if (node.IsLeaf)
                points.AddRange(node.Payload);

            if (points.Count == 0)            
                return;            

            var stream = File.Open(GetPathToFile(node), FileMode.OpenOrCreate);

            var writer = new BinaryWriter(stream);

            // first, write point count
            writer.Write(node.Payload.Count);

            foreach (var point in points)
            {
                WritePos(writer, ptAccessor, point);

                //Write other
            }
            stream.Dispose();
        }

        /// <summary>
        /// Writes the hierarchy of the octree into a separate file.
        /// </summary>
        public void WriteHierarchy(PtOctree<TPoint> octree)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(_fileFolderPath + "\\octree.hierarchy", FileMode.OpenOrCreate)))
            {
                octree.Traverse((PtOctant<TPoint> node) =>
                {
                    // write loadable properties (in which file the node's content - i.e. points - are stored)

                    bw.Write(node.Guid.ToByteArray()); // 16 bytes
                    bw.Write(node.Level);
                    //bw.Write(node.StreamPosition);

                    // write child indices (1 byte)                    
                    byte childIndices = 0;

                    int exp = 0;
                    foreach (var childNode in node.Children)
                    {
                        if (childNode != null)                        
                            childIndices += (byte)System.Math.Pow(2, exp);                       

                        exp++;
                    }

                    bw.Write(childIndices);
                });
            }
        }

        /// <summary>
        /// Writes some header information into a .json file.
        /// </summary>
        public static void WriteMeta(string pathToJson, PtOctree<PtOctant<TPoint>> octree, GridPtAccessor<TPoint> ptAccessor)
        {
            var rootCenter = octree.Root.Center;
            var rootSize = octree.Root.Size;
            var spacing = octree.Root.Resolution;

            JObject jsonObj = new JObject(
                //new JProperty("numberOfPoints", octree.PointCount),
                //new JProperty("boundingBox",
                //    new JObject(
                //        new JProperty("center", new JArray(center.X, center.Y, center.Z)),
                //        new JProperty("size", new JArray(length.X, length.Y, length.Z))
                //    )
                //),
                new JProperty("octree",
                    new JObject(
                        //new JProperty("numberOfNodes", metrics.NodeCount),
                        new JProperty("spacingFactor", spacing),
                        new JProperty("rootNode",
                            new JObject(
                                new JProperty("center", new JArray(rootCenter.x, rootCenter.y, rootCenter.z)),
                                new JProperty("size", rootSize)
                            )
                        )
                    )
                )                
            );

            //Add JProperty for "pointType" that contains all bools from the point accessor that are set to true.
            var ptTypeObj = new JObject();
            foreach (var propertyName in ptAccessor.GetPointType())
            {
                ptTypeObj.Add(propertyName, true);
            }
            var ptType = new JProperty("pointType", ptTypeObj);
            jsonObj.Add(ptType);

            //Write file
            using (StreamWriter file = File.CreateText(pathToJson))
            {
                file.Write(jsonObj.ToString());
            }
        }

        
    }    
}
