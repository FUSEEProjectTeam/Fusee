using Fusee.Math.Core;
using Fusee.PointCloud.Potree.V2.Data;
using System;
using System.IO;

namespace Fusee.PointCloud.Potree.V2
{
    /// <summary>
    /// Writes Potree data 
    /// </summary>
    public class Potree2Writer : Potree2AccessBase
    {
        /// <summary>
        /// Generate a <see cref="Potree2Writer"/> instance.
        /// </summary>
        public Potree2Writer(PotreeData potreeData) : base(potreeData) { }

        /// <summary>
        /// Directly writes the action of one given set of selectors to disk.
        /// </summary>
        /// <param name="nodeSelector"></param>
        /// <param name="pointSelector"></param>
        /// <param name="action"></param>
        /// <param name="dryrun"></param>
        /// <returns></returns>
        public (long octants, long points) Write(Predicate<PotreeNode> nodeSelector, Predicate<VisualizationPoint> pointSelector, Action<VisualizationPoint> action, bool dryrun = false)
        {
            long octantCount = 0;
            long pointsCount = 0;

            using (Stream readStream = File.Open(Path.Combine(PotreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Stream writeStream = File.Open(Path.Combine(PotreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName), FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                BinaryReader binaryReader = new BinaryReader(readStream);
                BinaryWriter binaryWriter = new BinaryWriter(writeStream);

                //foreach (var node in _potreeData.Hierarchy.Nodes)
                //{
                //    if (nodeSelector(node))
                //    {
                //        octantCount++;

                //        var point = new PotreePoint();

                //        for (int i = 0; i < node.NumPoints; i++)
                //        {
                //            if (offsetPosition > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

                //                double x = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.x;
                //                double y = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.y;
                //                double z = binaryReader.ReadInt32() * _potreeData.Metadata.Scale.z;

                //                double3 position = new(x, y, z);
                //                position = Potree2Consts.YZflip * position;

                //                point.Position = position;
                //            }

                //            if (offsetIntensity > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
                //                point.Intensity = binaryReader.ReadInt16();
                //            }

                //            if (offsetReturnNumber > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
                //                point.ReturnNumber = binaryReader.ReadByte();
                //            }

                //            if (offsetNumberOfReturns > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
                //                point.NumberOfReturns = binaryReader.ReadByte();
                //            }

                //            if (offsetClassification > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;
                //                point.Classification = binaryReader.ReadByte();
                //            }

                //            if (offsetScanAngleRank > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
                //                point.ScanAngleRank = binaryReader.ReadByte();
                //            }

                //            if (offsetUserData > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
                //                point.UserData = binaryReader.ReadByte();
                //            }

                //            if (offsetPointSourceId > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
                //                point.PointSourceId = binaryReader.ReadByte();
                //            }

                //            if (offsetColor > -1)
                //            {
                //                binaryReader.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

                //                ushort r = binaryReader.ReadUInt16();
                //                ushort g = binaryReader.ReadUInt16();
                //                ushort b = binaryReader.ReadUInt16();

                //                float3 color = float3.Zero;

                //                color.r = ((byte)(r > 255 ? r / 256 : r));
                //                color.g = ((byte)(g > 255 ? g / 256 : g));
                //                color.b = ((byte)(b > 255 ? b / 256 : b));

                //                point.Color = color;
                //            }

                //            if (pointSelector(point))
                //            {
                //                action(point);

                //                if (!dryrun)
                //                {
                //                    if (offsetPosition > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

                //                        var position = Potree2Consts.YZflip * point.Position;

                //                        int x = Convert.ToInt32(position.x / _potreeData.Metadata.Scale.x);
                //                        int y = Convert.ToInt32(position.y / _potreeData.Metadata.Scale.y);
                //                        int z = Convert.ToInt32(position.z / _potreeData.Metadata.Scale.z);

                //                        binaryWriter.Write(x);
                //                        binaryWriter.Write(y);
                //                        binaryWriter.Write(z);
                //                    }

                //                    if (offsetIntensity > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.Intensity);
                //                    }

                //                    if (offsetReturnNumber > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.ReturnNumber);
                //                    }

                //                    if (offsetNumberOfReturns > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.NumberOfReturns);
                //                    }

                //                    if (offsetClassification > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.Classification);
                //                    }

                //                    if (offsetScanAngleRank > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.ScanAngleRank);
                //                    }

                //                    if (offsetUserData > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.UserData);
                //                    }

                //                    if (offsetPointSourceId > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
                //                        binaryWriter.Write(point.PointSourceId);
                //                    }

                //                    if (offsetColor > -1)
                //                    {
                //                        binaryWriter.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

                //                        ushort r = Convert.ToUInt16(point.Color.r * 256);
                //                        ushort g = Convert.ToUInt16(point.Color.g * 256);
                //                        ushort b = Convert.ToUInt16(point.Color.b * 256);

                //                        binaryWriter.Write(r);
                //                        binaryWriter.Write(g);
                //                        binaryWriter.Write(b);
                //                    }
                //                }

                //                pointsCount++;
                //            }
                //        }
                //    }
                //}

                binaryWriter.Close();
                binaryReader.Dispose();

                binaryReader.Close();
                binaryReader.Dispose();
            }

            return (octantCount, pointsCount);
        }

        public (long octants, long points) Label(Predicate<PotreeNode> nodeSelector, Predicate<double3> pointSelector, byte Label, bool dryrun = false)
        {
            long octantCount = 0;
            long pointsCount = 0;

            using (Stream readStream = File.Open(Path.Combine(PotreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (Stream writeStream = File.Open(Path.Combine(PotreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName), FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            {
                BinaryReader binaryReader = new BinaryReader(readStream);
                BinaryWriter binaryWriter = new BinaryWriter(writeStream);

                double3 point = double3.Zero;

                foreach (var node in PotreeData.Hierarchy.Nodes)
                {
                    if (nodeSelector(node))
                    {
                        octantCount++;

                        for (int i = 0; i < node.NumPoints; i++)
                        {
                            binaryReader.BaseStream.Position = node.ByteOffset + 0 + i * PotreeData.Metadata.PointSize;

                            point.x = (binaryReader.ReadInt32() * PotreeData.Metadata.Scale.x);
                            point.z = (binaryReader.ReadInt32() * PotreeData.Metadata.Scale.y);
                            point.y = (binaryReader.ReadInt32() * PotreeData.Metadata.Scale.z);

                            if (pointSelector(point))
                            {
                                if (!dryrun)
                                {
                                    binaryWriter.BaseStream.Position = node.ByteOffset + 16 + i * PotreeData.Metadata.PointSize;
                                    binaryWriter.Write(Label);
                                }

                                pointsCount++;
                            }
                        }
                    }
                }

                binaryWriter.Close();
                binaryReader.Dispose();

                binaryReader.Close();
                binaryReader.Dispose();
            }

            return (octantCount, pointsCount);
        }

        //public void WriteRawPoints(OctantId oid, byte[] points)
        //{
        //    var node = FindNode(ref _potreeData.Hierarchy, oid);

        //    if (points.Length != node.NumPoints)
        //    {
        //        //TODO: (throw) correct error
        //        throw new Exception();
        //    }

        //    using (Stream writeStream = File.Open(Path.Combine(PotreeData.Metadata.FolderPath, Potree2Consts.OctreeFileName), FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
        //    {
        //        BinaryWriter binaryWriter = new BinaryWriter(writeStream);

        //        for (int i = 0; i < points.Length; i++)
        //        {
        //            var point = points[i];

        //            if (offsetPosition > -1)
        //            {
        //                // TODO: Fix position conversion
        //                //binaryWriter.BaseStream.Position = node.ByteOffset + offsetPosition + i * _potreeData.Metadata.PointSize;

        //                //var position = Potree2Consts.YZflip * point.Position;

        //                //binaryWriter.Write(position.x);
        //                //binaryWriter.Write(position.y);
        //                //binaryWriter.Write(position.z);
        //            }

        //            if (offsetIntensity > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetIntensity + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.Intensity);
        //            }

        //            if (offsetReturnNumber > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetReturnNumber + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.ReturnNumber);
        //            }

        //            if (offsetNumberOfReturns > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetNumberOfReturns + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.NumberOfReturns);
        //            }

        //            if (offsetClassification > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetClassification + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.Classification);
        //            }

        //            if (offsetScanAngleRank > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetScanAngleRank + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.ScanAngleRank);
        //            }

        //            if (offsetUserData > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetUserData + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.UserData);
        //            }

        //            if (offsetPointSourceId > -1)
        //            {
        //                binaryWriter.BaseStream.Position = node.ByteOffset + offsetPointSourceId + i * _potreeData.Metadata.PointSize;
        //                binaryWriter.Write(point.PointSourceId);
        //            }

        //            if (offsetColor > -1)
        //            {
        //                // TODO: Fix color conversion
        //                //binaryWriter.BaseStream.Position = node.ByteOffset + offsetColor + i * _potreeData.Metadata.PointSize;

        //                //ushort r = (ushort)MathF.Floor(point.Color.r >= 1f ? 255 : point.Color.r * 256f);
        //                //ushort g = (ushort)MathF.Floor(point.Color.g >= 1f ? 255 : point.Color.g * 256f);
        //                //ushort b = (ushort)MathF.Floor(point.Color.b >= 1f ? 255 : point.Color.b * 256f);

        //                //binaryWriter.Write(r);
        //                //binaryWriter.Write(g);
        //                //binaryWriter.Write(b);
        //            }
        //        }

        //        binaryWriter.Close();
        //        binaryWriter.Dispose();
        //    }
        //}
    }
}