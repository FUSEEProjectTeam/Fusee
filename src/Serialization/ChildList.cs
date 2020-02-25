﻿using System;
using System.Collections.ObjectModel;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Collection, which overrides the protected method InsertItem() to customize the behavior of Add() - in this case, to set the parent SceneNodeContainer.
    /// </summary>
    [ProtoContract]
    public class ChildList: Collection<SceneNodeContainer>
    {
        /// <summary>
        /// Eventhandler for adding additional behavior to Add(). E.g. to add the parent for the element that was added to the child list.
        /// </summary>
        public event EventHandler<AddChildEventArgs> OnAdd;

        /// <summary>
        /// Inserts an item into the scenen node container at a specified position.
        /// </summary>
        /// <param name="index">Position in which the item is inserted into the container.</param>
        /// <param name="snc">The scene node container in which the item will be inserted.</param>
        protected override void InsertItem(int index, SceneNodeContainer snc)
        {
            AddSnc(snc);
            base.InsertItem(index, snc);
        }

        /// <summary>
        /// Sets an item at the specified position in the scene node container.
        /// </summary>
        /// <param name="index">The position int the scene node container.</param>
        /// <param name="snc">The scene node container in which the item should be set.</param>
        protected override void SetItem(int index, SceneNodeContainer snc)
        {
            AddSnc(snc);
            base.SetItem(index, snc);
        }

        /// <summary>
        /// Removes an item from the scene node container at a specified position.
        /// </summary>
        /// <param name="index">The position of the item in the container.</param>
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

        private void AddSnc(SceneNodeContainer snc)
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
