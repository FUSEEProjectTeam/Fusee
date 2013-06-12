using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusee.Xirkit
{
    /// <summary>
    /// Instances of the Node class wrap around normal 
    /// </summary>
    [DebuggerDisplay("{O}")]
    public class Node
    {
        private readonly List<IOutPin> _outPinList;
        private readonly List<IInPin>  _inPinList;
        private readonly Dictionary<IInPin, bool> _inPinActualList;

        private Object _o;
        private ICalculationPerformer _cp;
        public Object O
        {
            get { return _o; }
            
            // re-setting the object is allowed (and works)
            set
            {
                _o = value;

                // Now re-wire all pins with new MemberAccessors
                foreach (IInPin inPin in _inPinList)
                {
                    PinFactory.ReAttachInPin(this, inPin);
                }
                foreach (IOutPin outPin in _outPinList)
                {
                    PinFactory.ReAttachOutPin(this, outPin);
                }
            }
        }

        /// <summary>
        /// Constructs a new node
        /// </summary>
        /// <param name="o"></param>
        public Node(Object o)
        {
            _o = o;
            _cp = o as ICalculationPerformer;
            _outPinList = new List<IOutPin>();
            _inPinList = new List<IInPin>();
            _inPinActualList = new Dictionary<IInPin, bool>();
            Reset();
        }

        public void Reset()
        {
            foreach (IInPin inPin in _inPinList)
            {
                _inPinActualList[inPin] = false;
            }
        }

        public void Attach(string thisMember, Node other, string otherMember)
        {
            IOutPin outPin = GetOutPin(thisMember);
            IInPin inPin = other.GetInPin(otherMember, outPin.GetPinType());
            outPin.Attach(inPin);
        }

        private IOutPin GetOutPin(string member)
        {
            // See if the outpin pinning thisProperty already exists. If not create one.
            IOutPin outPin = _outPinList.Find(p => p.Member == member);
            if (outPin ==  null)
                outPin = PinFactory.CreateOutPin(this, member);
            _outPinList.Add(outPin);
            return outPin;
        }

        internal IInPin GetInPin(string member, Type targetType)
        {
            // See if the inpin already exists. If so, throw - because a property
            // can be inpinnned only once.
            IInPin inPin = _inPinList.Find(p => p.Member == member);
            if (inPin != null)
            {
                // TODO: throw an appropriate exception
                throw new Exception("Member " + member + " already connected as InPin!");
            }
            inPin = PinFactory.CreateInPin(this, member, targetType);

            _inPinList.Add(inPin);
            _inPinActualList[inPin] = false;
            inPin.ReceivedValue += OnReceivedValue;
            return inPin;
        }

        void OnReceivedValue(IInPin inPin, EventArgs args)
        {
            _inPinActualList[inPin] = true;
            if (AllPinsActual)
                Propagate();
        }

        public void Propagate()
        {
            if (_cp != null)
                _cp.PerformCalculation();

            foreach (IOutPin outPin in _outPinList)
            {
                outPin.Propagate();
            }
        }

        public Array OutpinToArray(Node node)
        {
             return node._outPinList.ToArray();     
        }

        public Array InpinToArray(Node node)
        {
            return node._inPinList.ToArray();
        }

        public bool AllPinsActual 
        { 
            get
            {
                bool ret = true;
                foreach (IInPin inPin in _inPinList)
                {
                    ret &= _inPinActualList[inPin];
                    if (!ret)
                        break;
                }
                return ret;
            }
        }

        public void RemoveAllPins()
        {
            _outPinList.Clear();
            _inPinList.Clear();
            _inPinActualList.Clear();
        }
    }
}

