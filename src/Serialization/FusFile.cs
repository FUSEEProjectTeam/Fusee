using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fusee.Serialization
{
    /// <summary>
    /// The header of a FUSEE file.
    /// </summary>
    [ProtoContract]
    public struct FusHeader
    {
        /// <summary>
        /// The file version number of this fus file.
        /// </summary>
        [ProtoMember(1)]
        public int FileVersion;

        /// <summary>
        /// The generator used to create this fus file.
        /// </summary>
        [ProtoMember(2)]
        public string Generator;

        /// <summary>
        /// The user who created this fus file.
        /// </summary>
        [ProtoMember(3)]
        public string CreatedBy;

        /// <summary>
        /// The creation date of the file.
        /// </summary>
        [ProtoMember(4)]
        public string CreationDate;
    }


    /// <summary>
    /// Polymorphic contents of a fus file. The actual contents is contained in a sub-class. Possible sub-classes are listed
    /// in <see cref="ProtoIncludeAttribute"/> decorators preceding this class declaration. This mechanism allows fus files
    /// to contain different types of contents as well as different versions of contents. 
    /// </summary>
    [ProtoInclude(201, typeof(V1.FusScene))]
    [ProtoContract]
    public class FusContents
    {
        // NOTHING HERE.
    }

    /// <summary>
    /// De-serialized raw contents of a .fus File. Contains a <see cref="FusHeader"/> and a very polymorphic <see cref="FusContents"/>.
    /// </summary>
    [ProtoContract]
    public class FusFile
    {
        /// <summary>
        /// The file header. Contains some meta data.
        /// </summary>
        [ProtoMember(1)]
        public FusHeader Header;

        /// <summary>
        /// The file contents. Check and cast to the concrete type to access it, e. g. using a C# 7.0 
        /// <a href="https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching">pattern matching in switch expression</a>.
        /// </summary>
        [ProtoMember(2)]
        public FusContents? Contents;
    }
}
