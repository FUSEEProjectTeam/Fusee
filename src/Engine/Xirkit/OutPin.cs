using System.Collections.Generic;
using System;

namespace Fusee.Xirkit
{
    public class OutPin<T> : Pin, IOutPin
    {
        private List<InPin<T>> _links;
        private IMemberAccessor<T> _memberAccessor;
        public IMemberAccessor<T> MemberAccessor
        {
            get { return _memberAccessor; }
            set { _memberAccessor = value; } 
        }

        public OutPin(Node n, string member, IMemberAccessor<T> memberAccessor)
            : base(n, member)
        {
            _links = new List<InPin<T>>();
            _memberAccessor = memberAccessor;
        }

        public void Attach(IInPin other)
        {
            _links.Add((InPin<T>)other);
        }

        public void Detach(IInPin other)
        {
            _links.Remove((InPin<T>)other);
        }

        public T GetValue()
        {
            return _memberAccessor.Get(N.O);
        }

        public void Propagate()
        {
            foreach(InPin<T> inPin in _links)
            {
                inPin.SetValue(GetValue());
            }
        }
        public Array linksToArray()
        {
            return _links.ToArray();
        }

        public Type GetPinType()
        {
            return typeof(T);
        }
    }
}
