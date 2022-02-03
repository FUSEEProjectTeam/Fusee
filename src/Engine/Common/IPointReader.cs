using System;

namespace Fusee.Engine.Common
{
    //Used to inject the file reader (LAS, e57 etc.)
    public interface IPointReader : IDisposable
    {
        public void OpenFile(string pathToFile);
    }
}
