using Fusee.Base.Core;
using Fusee.Serialization;
using System.IO;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Collection of conventient serialization methodes.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serialization version.
        /// </summary>
        public static int Version = SerializerLow.Version;

        /// <summary>
        /// Deserializes a SceneContainer.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SceneContainer DeserializeSceneContainer(Stream stream)
        {
            SceneContainer sceneContainer = SerializerLow.DeserializeRawSceneContainer(stream);

            if (sceneContainer.Header.Version != Version)
            {
                FileStream fs = stream as FileStream;
                if (fs != null)
                {
                    Diagnostics.Warn("File version of " + fs.Name + " (" + sceneContainer.Header.Version.ToString() + ") does not match used Fusee version (" + Version + "). This can result in unexpected behaviour.");
                }
                else
                {
                    Diagnostics.Warn("Serialization version of stream input (" + sceneContainer.Header.Version.ToString() + ") does not match used Fusee version (" + Version + "). This can result in unexpected behaviour.");
                }

            }

            return new ConvertSceneGraph().Convert(sceneContainer);
        }
    }
}