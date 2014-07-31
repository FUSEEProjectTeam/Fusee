namespace Fusee.Engine
{
    public interface IVideoManagerImp
    {
        IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio);
        IVideoStreamImp CreateVideoStreamImpFromCamera(int cameraIndex, bool useAudio);
    }
}
