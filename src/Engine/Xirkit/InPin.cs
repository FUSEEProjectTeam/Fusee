using System;
namespace Fusee.Xirkit
{
    public class InPin<T> : Pin, IInPin
    {
        private IMemberAccessor<T> _memberAccessor;
        public IMemberAccessor<T> MemberAccessor
        {
            get { return _memberAccessor; }
            set { _memberAccessor = value; }
        }
        
        public InPin(Node n, string member, IMemberAccessor<T> memberAccessor)
            : base(n, member)
        {
            _memberAccessor = memberAccessor;
            // TODO: build a set accesssor for the property.
        }

       public void SetValue(T value)
       {
           _memberAccessor.Set(N.O, value);
           if (ReceivedValue != null)
               ReceivedValue(this, null);
       }

       public Type GetPinType()
       {
           return typeof(T);
       }

       public event ReceivedValueHandler ReceivedValue;
    }
}
