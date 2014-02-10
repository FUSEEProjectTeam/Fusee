using System.Diagnostics;
using System;
using System.Reflection;

namespace Fusee.Xirkit
{
    [DebuggerDisplay("{N.O}.{Member}")]
    public class Pin
    {
        private Node _n;
        public Node N
        {
            get { return _n; }
        }

        private string _member;
        public string Member
        {
            get { return _member; }
        }

        public Pin(Node n, string member)
        {
            _n = n;
            _member = member;
        }
    }
}
