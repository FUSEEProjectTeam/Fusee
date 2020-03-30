using System;
using System.Collections.ObjectModel;

namespace Fusee.Engine.Core.Scene
{
    /// <summary>
    /// List of child nodes used in <see cref="SceneNode"/> instances. In addition to normal collection operations
    /// this type keeps track of each object's <see cref="SceneNode.Parent"/> property when added or removed to a
    /// node.
    /// </summary>
    public class ChildList : Collection<SceneNode>
    {
        /// <summary>
        /// Event handler for adding additional behavior to Add(). E.g. to add the parent for the element that was added to the child list.
        /// </summary>
        public event EventHandler<AddChildEventArgs> OnAdd;

        /// <summary>
        /// Inserts the item at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="sn">The scene node to insert.</param>
        protected override void InsertItem(int index, SceneNode sn)
        {
            AddSceneNode(sn);
            base.InsertItem(index, sn);
        }

        /// <summary>
        /// Sets the item at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="sn">The scene node to add.</param>
        protected override void SetItem(int index, SceneNode sn)
        {
            AddSceneNode(sn);
            base.SetItem(index, sn);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.ObjectModel.Collection`1"></see>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            var snc = this[index];

            //Replace the parent of this snc's children with the parent of this scn
            if (snc.Children.Count != 0)
            {
                foreach (var child in snc.Children)
                {
                    child.Parent = snc.Parent;
                }
            }
            base.RemoveItem(index);
        }

        private void AddSceneNode(SceneNode snc)
        {
            if (snc.Parent == null)
            {
                OnAdd?.Invoke(this, new AddChildEventArgs(snc));
            }
            else
            {
                //remove from old parent's child list
                snc.Parent.Children.Remove(snc);
                OnAdd?.Invoke(this, new AddChildEventArgs(snc));
            }
        }
    }
}