using System;
using System.Collections.Generic;
using System.Diagnostics;
using Fusee.Math.Core;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Event Arguments for adding a new child to a SceneNodeContainer and set its parent.
    /// </summary>
    public class AddChildEventArgs : EventArgs
    {
        /// <summary>
        /// Returns the scene node container.
        /// </summary>
        public SceneNodeContainer Snc { get; }
        /// <summary>
        /// Adds eventa arguements to a scene node container.
        /// </summary>
        /// <param name="snc">The scene node container <see cref="SceneNodeContainer"></see>/></param>
        public AddChildEventArgs(SceneNodeContainer snc)
        {
            Snc = snc;
        }
        
    }

    /// <summary>
    /// The building block to create hierarchies.
    /// </summary>
    [ProtoContract]
    [DebuggerDisplay("{Name,nq}, {(Components==null)? \"No\" : Components.Count.ToString(),nq} components, {(Children==null)? \"No\" : Children.Count.ToString(),nq} children")]
    public class SceneNodeContainer
    {
        /// <summary>
        /// The name.
        /// </summary>
        [ProtoMember(1)]
        public string Name;

        /// <summary>
        /// The components this node is made of.
        /// </summary>
        [ProtoMember(2, AsReference = true)]
        public List<SceneComponentContainer> Components;

        /// <summary>
        /// Possible children. 
        /// </summary>
        [ProtoMember(3, AsReference = true)]
        public ChildList Children {
            get => _children;
            set
            {
                _children = value;
                foreach (var child in _children)
                {
                    child.Parent = this;
                }
            }
        }

        private ChildList _children;

        /// <summary>
        /// This SceneNodeContainer's snc. 
        /// </summary>
        public SceneNodeContainer Parent;

        /// <summary>
        /// Creates a new instance of te SceneNodeContainer class. 
        /// </summary>
        public SceneNodeContainer()
        {
            Components = new List<SceneComponentContainer>();
            Children = new ChildList();
            Children.OnAdd += (sender, e) => e.Snc.Parent = this;
        }
        
    }
}
