using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Generates a simple scene header.
    /// </summary>
    [ProtoContract]
    public class SimpleType
    {
        /// <summary>
        /// The scene header (Test).
        /// </summary>
        [ProtoMember(1)]
        public SceneHeaderTest Header;
    }
}

/// <summary>
/// Generates a scene header with the version, the generator, the name of the creator and the creation date
/// </summary>
[ProtoContract]
public struct SceneHeaderTest
{
    /// <summary>
    /// Version number.
    /// </summary>
    [ProtoMember(1)]
    public int Version;
    /// <summary>
    /// Generator used to create the scene.
    /// </summary>
    [ProtoMember(2)]
    public string Generator;
    /// <summary>
    /// Name of the creator.
    /// </summary>
    [ProtoMember(3)]
    public string CreatedBy;
    /// <summary>
    /// Date of creation.
    /// </summary>
    [ProtoMember(4)]
    public string CreationDate;
}
