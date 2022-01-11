using Fusee.Xene;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// Event Arguments for adding a new child to a SceneNodeContainer and set its parent.
    /// </summary>
    public class AddChildEventArgs : EventArgs
    {
        /// <summary>
        /// Returns the scene node container.
        /// </summary>
        public SceneNode Snc { get; }
        /// <summary>
        /// Adds event arguments to a scene node container.
        /// </summary>
        /// <param name="snc">The scene node container <see cref="SceneNode"></see>/></param>
        public AddChildEventArgs(SceneNode snc)
        {
            Snc = snc;
        }
    }

    /// <summary>
    /// The building block to create hierarchies.
    /// </summary>
    [DebuggerDisplay("{Name,nq}, {(Components==null)? \"No\" : Components.Count.ToString(),nq} components, {(Children==null)? \"No\" : Children.Count.ToString(),nq} children")]
    public class SceneNode : Xene.INode
    {
        private ChildList _children;

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The components this node is made of.
        /// </summary>           
        public List<SceneComponent> Components;

        /// <summary>
        /// This SceneNodeContainer's snc. 
        /// </summary>
        public SceneNode Parent;

        /// <summary>
        /// Creates a new instance of this SceneNode class. 
        /// </summary>
        public SceneNode()
        {
            Components = new List<SceneComponent>();
            Children = new ChildList();            
        }

        /// <summary>
        /// Possible children. 
        /// </summary>         
        public ChildList Children
        {
            get => _children;
            set
            {
                _children = value;
                _children.OnAdd += (sender, e) => e.Snc.Parent = this;
                foreach (var child in _children)
                {
                    child.Parent = this;
                }
            }
        }

        /// <summary>
        /// Returns all children of this SceneNode
        /// </summary>
        public IEnumerable<INode> EnumChildren => Children;

        /// <summary>
        /// Returns all components of this SceneNode
        /// </summary>
        public IEnumerable<IComponent> EnumComponents => Components;
    }
}