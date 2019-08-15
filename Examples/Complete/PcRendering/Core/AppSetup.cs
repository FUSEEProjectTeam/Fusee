using Fusee.Pointcloud.PointAccessorCollections;

namespace Fusee.Examples.PcRendering.Core
{
    public static class AppSetup
    {
        public static void DoSetup(Pointcloud.Common.IPcRendering app, PointType ptType, int pointThreshold, string pathToFile)
        {
            switch (ptType)
            {
                case PointType.Pos64:
                    {
                        var appImp = (PcRendering<Pos64>)app;

                        appImp.AppSetup = () =>
                        {
                            var ptAcc = new Pos64_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64Col32IShort_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Col32IShort;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Col32IShort>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64IShort_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64IShort;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64IShort>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64Col32_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Col32;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Col32>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64Nor32Col32IShort_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32Col32IShort;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32Col32IShort>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64Nor32IShort_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32IShort;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32IShort>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
                            var ptAcc = new Pos64Nor32Col32_Accessor();
                            appImp.PtAccessor = ptAcc;
                            appImp.GetMeshsForNode = MeshFromOocFile.GetMeshsForNode_Pos64Nor32Col32;

                            appImp.OocLoader = new Pointcloud.OoCFileReaderWriter.PtOctantLoader<Pos64Nor32Col32>(appImp.InitCameraPos, pathToFile, appImp.GetRc())
                            {
                                PointThreshold = pointThreshold
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
