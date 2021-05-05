using Fusee.Serialization.V1;

namespace Fusee.Serialization
{
    /// <summary>
    /// <para>
    /// The classes in this module are the building blocks for storing
    /// FUSEE's scene graph file contents.
    /// </para>
    /// <para>
    /// All classes and their properties are decorated with Protobuf.net
    /// attributes to mark them as serializable in the Google Protobuf 
    /// binary format.
    /// </para>
    /// <para>
    /// Serialized FUSEE scene graphs are stored in *.fus files. 
    /// </para>
    /// <para>
    /// In Addition Protobuf.net's Serializer class can
    /// generate a .proto schema from the attributed C# classes. This .proto
    /// schema can then be used to generate serializer/deserializer code for
    /// different languages. This is how the Blender .fus exporter Add-In 
    /// implemented in python serializes into the .fus format.
    /// </para>
    /// <para>
    /// The three
    /// main building block types for scene graphs are 
    /// <list type="bullet">
    ///   <item>
    ///     <term><see cref="FusScene"/></term>
    ///     <description>
    ///       Root object of each .fus file. Contains a file header
    ///       and a list of root nodes.</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="FusNode"/></term>
    ///     <description>
    ///       Instances of this type (called nodes) make up the hierarchy of objects.
    ///       Each node contains a list of child nodes and a list of components.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="FusComponent"/></term>
    ///     <description>
    ///       Instances of this type (called components) store the scene's payload. 
    ///       Various component types (inherited from <see cref="FusComponent"/>)
    ///       exist to keep the different components a scene is made of.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    static class NamespaceDoc
    {
        // This class only exists to keep the namespace XML documentation 
    }
}