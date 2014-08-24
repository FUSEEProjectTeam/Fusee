namespace Fusee.Engine
{
    /// <summary>
    /// The interface for VideoManager implementations. This interface should contain all functions
    /// to load a video.
    /// </summary>
    public interface IVideoManagerImp
    {
        IVideoStreamImp CreateVideoStreamImpFromFile(string filename, bool loopVideo, bool useAudio);
        IVideoStreamImp CreateVideoStreamImpFromCamera(int cameraIndex, bool useAudio);
    }
}
