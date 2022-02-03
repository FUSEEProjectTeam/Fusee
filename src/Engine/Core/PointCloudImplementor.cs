using Fusee.Engine.Common;
using System;

namespace Fusee.Engine.Core
{
    //Properties that can be injected in the Main.cs of an example.
    public sealed class PointCloudImplementor : IDisposable
    {
        #region Singleton 
        private static readonly PointCloudImplementor _instance = new();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static PointCloudImplementor() { }

        private PointCloudImplementor() { }

        public static PointCloudImplementor Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        //Inject the method that knows how to create a GpuMesh - without passing the RenderContext down into the PointCloudImp
        public static CreateGpuMesh CreateGpuMesh
        {
            get { return Instance._createGpuMesh; }
            set { Instance._createGpuMesh = value; }
        }
        private CreateGpuMesh _createGpuMesh;

        //Object that can read "native" points from las/e57 or other common point file formats.
        //Allows the injection of platform specific implementation.
        public static IPointReader NativePointReader
        {
            get { return Instance._pointReader; }
            set { Instance._pointReader = value; }
        }
        private IPointReader _pointReader;

        #region Dispose
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        protected void Dispose(bool disposing)
        {

            if (!disposed)
            {

                if (disposing)
                {
                    // Dispose managed resources.
                    NativePointReader.Dispose();
                }
                disposed = true;
            }
        }

        ~PointCloudImplementor()
        {
            Dispose(disposing: false);
        }
        #endregion
    }
}
