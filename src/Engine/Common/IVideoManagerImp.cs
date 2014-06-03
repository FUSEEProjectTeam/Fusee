namespace Fusee.Engine
{
    public interface IVideoManagerImp
    {
        IVideoStreamImp CreateVideoStreamImpFromFile(string filename);
        IVideoStreamImp CreateVideoStreamImpFromCamera();
    }
}
