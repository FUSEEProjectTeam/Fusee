using System;
using System.Collections.ObjectModel;
using ProtoBuf;

namespace Fusee.Serialization
{
    /// <summary>
    /// Collection, which overrides the protected InsertItem() method to customize the behavior of Add() - in this case, to set the parent SceneNodeContainer.
    /// </summary>
    [ProtoContract]
    public class ChildList: Collection<SceneNodeContainer>
    {
        public event EventHandler<OnAddChild> OnAdd;

        protected override void InsertItem(int index, SceneNodeContainer snc)
        {

            if (snc.Parent == null)
            {
                OnAdd?.Invoke(this, new OnAddChild(snc));
                base.InsertItem(index, snc);
            }
            else
            {
                //remove from old parent's child list
                snc.Parent.Children.Remove(snc);

                OnAdd?.Invoke(this, new OnAddChild(snc));
                base.InsertItem(index, snc);

            }
        }
    }
}
