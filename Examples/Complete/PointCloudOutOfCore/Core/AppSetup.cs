using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
using Fusee.PointCloud.OoCReaderWriter;
using System;

namespace Fusee.Examples.PointCloudOutOfCore.Core
{
    public static class AppSetup
    {
        /// <summary>
        /// Contains the Setup methods for all point types that are supported.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="ptType">The point type.</param>
        /// <param name="pointThreshold">Initial point threshold.</param>
        /// <param name="pathToFile">Path to the ooc file.</param>
        public static void DoSetup(out IPcRendering app, PointType ptType, int pointThreshold, string pathToFile)
        {
            switch (ptType)
            {
                case PointType.Pos64:
                    {
                        app = new PointCloudOutOfCore<Pos64>(

                             new PtOctantLoader<Pos64>(pathToFile, PointType.Pos64)
                             {
                                 PointThreshold = pointThreshold,
                                 PtAcc = new Pos64_Accessor()

                             },
                             new PtOctreeFileReader<Pos64>(pathToFile)
                        );

                        break;
                    }
                case PointType.Pos64Col32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Col32IShort>
                        (
                            new PtOctantLoader<Pos64Col32IShort>(pathToFile, PointType.Pos64Col32IShort)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Col32IShort_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Col32IShort>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64IShort>
                        (
                            new PtOctantLoader<Pos64IShort>(pathToFile, PointType.Pos64IShort)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64IShort_Accessor()

                            },
                            new PtOctreeFileReader<Pos64IShort>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64Col32:
                    {
                        app = new PointCloudOutOfCore<Pos64Col32>
                        (
                            new PtOctantLoader<Pos64Col32>(pathToFile, PointType.Pos64Col32)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Col32_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Col32>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64Label8:
                    {
                        app = new PointCloudOutOfCore<Pos64Label8>
                        (
                            new PtOctantLoader<Pos64Label8>(pathToFile, PointType.Pos64Label8)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Label8_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Col32>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64Nor32Col32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32Col32IShort>
                        (
                            new PtOctantLoader<Pos64Nor32Col32IShort>(pathToFile, PointType.Pos64Nor32Col32IShort)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32Col32IShort_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Nor32Col32IShort>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64Nor32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32IShort>
                        (
                            new PtOctantLoader<Pos64Nor32IShort>(pathToFile, PointType.Pos64Nor32IShort)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32IShort_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Nor32IShort>(pathToFile)
                        );
                        break;
                    }
                case PointType.Pos64Nor32Col32:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32Col32>
                        (
                            new PtOctantLoader<Pos64Nor32Col32>(pathToFile, PointType.Pos64Nor32Col32)
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32Col32_Accessor()

                            },
                            new PtOctreeFileReader<Pos64Nor32Col32>(pathToFile)
                        );
                        break;
                    }
                default:
                    throw new ArgumentException($"Unsupported point type: {ptType}");
            }
        }
    }
}