using Fusee.Pointcloud.PointAccessorCollections;
using Fusee.Pointcloud.Common;

namespace Fusee.Examples.PcRendering.Core
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
        public static void DoSetup(IPcRendering app, PointType ptType, int pointThreshold, string pathToFile)
        {
            switch (ptType)
            {
                case PointType.Pos64:
                    {
                        var appImp = (PcRendering<Pos64>)app;

                        appImp.AppSetup = () =>
                        {

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64

                        };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64Col32IShort:
                    {
                        var appImp = (PcRendering<Pos64Col32IShort>)app;

                        appImp.AppSetup = () =>
                        {
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Col32IShort>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Col32IShort_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Col32IShort
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64Col32IShort>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64IShort:
                    {
                        var appImp = (PcRendering<Pos64IShort>)app;

                        appImp.AppSetup = () =>
                        {
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64IShort>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64IShort_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64IShort
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64IShort>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64Col32:
                    {
                        var appImp = (PcRendering<Pos64Col32>)app;

                        appImp.AppSetup = () =>
                        { 
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Col32>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Col32_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Col32
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64Col32>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64Nor32Col32IShort:
                    {
                        var appImp = (PcRendering<Pos64Nor32Col32IShort>)app;

                        appImp.AppSetup = () =>
                        { 
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32Col32IShort>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32Col32IShort_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32Col32IShort
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64Nor32Col32IShort>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64Nor32IShort:
                    {
                        var appImp = (PcRendering<Pos64Nor32IShort>)app;

                        appImp.AppSetup = () =>
                        {
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32IShort>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32IShort_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32IShort
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64Nor32IShort>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                case PointType.Pos64Nor32Col32:
                    {
                        var appImp = (PcRendering<Pos64Nor32Col32>)app;

                        appImp.AppSetup = () =>
                        {
                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32Col32>(pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold,
                                PtAcc = new Pos64Nor32Col32_Accessor(),
                                GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32Col32
                            };
                            appImp.OocFileReader = new Pointcloud.OoCFileReaderWriter.PtOctreeFileReader<Pos64Nor32Col32>(pathToFile);
                        };

                        app = appImp;
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
