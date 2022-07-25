using Fusee.Math.Core;
using Fusee.PointCloud.Common.Accessors;
using Fusee.PointCloud.Core.Accessors;
using Fusee.PointCloud.Potree.V2.Data;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    internal static class Potree2NodeReader
    {
        public static TPoint[] LoadNodeData<TPoint>(PotreeNode potreeNode, PotreeMetadata potreeMetadata, IPointAccessor pointAccessor) where TPoint : new()
        {
            var octreeFilePath = Path.Combine(potreeMetadata.FolderPath, Constants.OctreeFileName);

            TPoint[] points = null;

            if (potreeNode != null)
            {
                var binaryReader = new BinaryReader(File.OpenRead(octreeFilePath));
                points = LoadNodeData<TPoint>(potreeNode, potreeMetadata, pointAccessor, binaryReader);

                potreeNode.IsLoaded = true;

                binaryReader.Close();
                binaryReader.Dispose();
            }

            return points;
        }

        private static TPoint[] LoadNodeData<TPoint>(PotreeNode node, PotreeMetadata potreeMetadata, IPointAccessor pointAccessor, BinaryReader binaryReader) where TPoint : new()
        {
            var points = new TPoint[node.NumPoints];
            for (int i = 0; i < node.NumPoints; i++)
            {
                points[i] = new TPoint();
            }

            //Get offsets
            // TODO: move to metadata structure

            var positionOffset = -1;
            var classificationOffset = -1;
            var colorOffset = -1;

            if (potreeMetadata.HasPositionAttribute)
            {
                positionOffset = potreeMetadata.Attributes.Find(n => n.Name.Equals("position")).AttributeOffset;
            }
            if (potreeMetadata.HasClassificationAttribute)
            {
                classificationOffset = potreeMetadata.Attributes.Find(n => n.Name.Equals("classification")).AttributeOffset;
            }
            if (potreeMetadata.HasColorAttribute)
            {
                colorOffset = potreeMetadata.Attributes.Find(n => n.Name.Equals("rgb")).AttributeOffset;
            }

            for (int i = 0; i < node.NumPoints; i++)
            {
                if (potreeMetadata.HasPositionAttribute)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + positionOffset + i * potreeMetadata.PointSize;

                    double x = binaryReader.ReadInt32() * potreeMetadata.Scale.x;
                    double y = binaryReader.ReadInt32() * potreeMetadata.Scale.y;
                    double z = binaryReader.ReadInt32() * potreeMetadata.Scale.z;

                    double3 position = new(x, y, z);
                    position = Constants.YZflip * position;

                    ((PointAccessor<TPoint>)pointAccessor).SetPositionFloat3_64(ref points[i], position);
                }

                //else if (metaitem.Name.Equals("intensity"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        Int16 intensity = binaryReader.ReadInt16();
                //    }
                //}
                //else if (metaitem.Name.Equals("return number"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte returnNumber = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("number of returns"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte numberOfReturns = binaryReader.ReadByte();
                //    }
                //}

                if (potreeMetadata.HasClassificationAttribute)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + classificationOffset + i * potreeMetadata.PointSize;

                    byte label = binaryReader.ReadByte();

                    ((PointAccessor<TPoint>)pointAccessor).SetLabelUInt_8(ref points[i], label);
                }

                //else if (metaitem.Name.Equals("scan angle rank"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte scanAnleRank = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("user data"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte userData = binaryReader.ReadByte();
                //    }
                //}
                //else if (metaitem.Name.Equals("point source id"))
                //{
                //    for (int i = 0; i < node.NumPoints; i++)
                //    {
                //        binaryReader.BaseStream.Position = node.ByteOffset + attributeOffset + i * Instance.Metadata.PointSize;

                //        byte pointSourceId = binaryReader.ReadByte();
                //    }
                //}

                if (potreeMetadata.HasColorAttribute)
                {
                    binaryReader.BaseStream.Position = node.ByteOffset + colorOffset + i * potreeMetadata.PointSize;

                    ushort r = binaryReader.ReadUInt16();
                    ushort g = binaryReader.ReadUInt16();
                    ushort b = binaryReader.ReadUInt16();

                    float3 color = float3.Zero;

                    color.r = ((byte)(r > 255 ? r / 256 : r));
                    color.g = ((byte)(g > 255 ? g / 256 : g));
                    color.b = ((byte)(b > 255 ? b / 256 : b));

                    ((PointAccessor<TPoint>)pointAccessor).SetColorFloat3_32(ref points[i], color);
                }
            }

            return points;
        }
    }
}