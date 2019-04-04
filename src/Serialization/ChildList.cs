using System;
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

        protected override void InsertItem(int index, SceneNodeContainer snc)
        {

            if (snc.Parent == null)
            {
                OnAdd?.Invoke(this, new AddChildEventArgs(snc));
                base.InsertItem(index, snc);
            }
            else
            {
                //remove from old parent's child list
                snc.Parent.Children.Remove(snc);

                OnAdd?.Invoke(this, new AddChildEventArgs(snc));
                base.InsertItem(index, snc);

            }
        }
    }
}
