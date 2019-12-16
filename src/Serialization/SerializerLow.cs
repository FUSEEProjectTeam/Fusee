using System.IO;

namespace Fusee.Serialization
{
    /// <summary>
    /// Low level serializer for direct file content.
    /// </summary>
    public static class SerializerLow
    {
        /// <summary>
        /// Serialization version.
        /// </summary>
        public const int Version = 0;

        /// <summary>
        /// Deserializes a low level SceneContainer (direct file content).
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SceneContainer DeserializeRawSceneContainer(Stream stream)
        {
            var sceneContainer = ProtoBuf.Serializer.Deserialize<SceneContainer>(stream);

            return sceneContainer;
        }
    }
}