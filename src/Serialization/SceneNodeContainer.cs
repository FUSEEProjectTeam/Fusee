using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ProtoBuf;

namespace Fusee.Serialization
{
    public class OnAddChild : EventArgs
    {
        public OnAddChild(SceneNodeContainer snc)
        {
            Snc = snc;
        }

        public SceneNodeContainer Snc { get; }
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

        public SceneNodeContainer Parent;

        public SceneNodeContainer()
        {
            Components = new List<SceneComponentContainer>();
            Children = new ChildList();
            Children.OnAdd += (sender, e) => e.Snc.Parent = this;
        }
    }
}
