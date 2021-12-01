using Fusee.Engine.Core;
using Fusee.PointCloud.Common;
using Fusee.PointCloud.Core;
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
        public static void DoSetup(out IPointCloudOutOfCore app, PointType ptType, string pathToFile)
        {
            switch (ptType)
            {
                case PointType.Pos64:
                    {
                        app = new PointCloudOutOfCore<Pos64>(ptType, pathToFile, new Pos64Accessor());
                        break;
                    }
                case PointType.Pos64Col32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Col32IShort>(ptType, pathToFile, new Pos64Col32IShortAccessor());
                        break;
                    }
                case PointType.Pos64IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64IShort>(ptType, pathToFile, new Pos64IShortAccessor());
                        break;
                    }
                case PointType.Pos64Col32:
                    {
                        app = new PointCloudOutOfCore<Pos64Col32>(ptType, pathToFile, new Pos64Col32Accessor());
                        break;
                    }
                case PointType.Pos64Label8:
                    {
                        app = new PointCloudOutOfCore<Pos64Label8>(ptType, pathToFile, new Pos64Label8Accessor());
                        break;
                    }
                case PointType.Pos64Nor32Col32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32Col32IShort>(ptType, pathToFile, new Pos64Nor32Col32IShortAccessor());
                        break;
                    }
                case PointType.Pos64Nor32IShort:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32IShort>(ptType, pathToFile, new Pos64Nor32IShortAccessor());
                        break;
                    }
                case PointType.Pos64Nor32Col32:
                    {
                        app = new PointCloudOutOfCore<Pos64Nor32Col32>(ptType, pathToFile, new Pos64Nor32Col32Accessor());
                        break;
                    }
                default:
                    throw new ArgumentException($"Unsupported point type: {ptType}");
            }
        }
    }
}