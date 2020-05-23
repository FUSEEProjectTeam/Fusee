using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fusee.Xirkit
{
    /// <summary>
    /// <para>
    /// Major building block of the Xirkit library. Each <see cref="Circuit"/> contains Node instances.
    /// </para>
    /// <para>
    /// A single object instance of any type can be hosted inside a node. This way any arbitrary object 
    /// can participate in a Circuit. Node instances within a <see cref="Circuit"/> are interconnected 
    /// using in-pins and out-pins.
    /// The internal connection from pins to actual members (properties and fields) of the contained object
    /// are done using <see cref="IMemberAccessor{T}"/>s.
    /// </para>
    /// </summary>
    [DebuggerDisplay("{O}")]
    public class Node
    {
        private readonly List<IOutPin> _outPinList;
        private readonly List<IInPin> _inPinList;
        private readonly Dictionary<IInPin, bool> _inPinActualList;

        private object _o;
        private ICalculationPerformer _cp;
        /// <summary>
        /// Gets and sets the object to host by this node.
        /// </summary>
        /// <value>
        /// The object to be hosted.
        /// </value>
        public object O
        {
            get { return _o; }

            // re-setting the object is allowed (and works)
            set
            {
                _o = value;
                _cp = value as ICalculationPerformer;

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
        /// Constructs a new node.
        /// </summary>
        /// <param name="o">The object to be hosted.</param>
        public Node(object o)
        {
            if (!o.GetType().IsClass)
                throw new ArgumentException("Not a reference type.", nameof(o));

            _o = o;
            _cp = o as ICalculationPerformer;
            _outPinList = new List<IOutPin>();
            _inPinList = new List<IInPin>();
            _inPinActualList = new Dictionary<IInPin, bool>();
            Reset();
        }

        /// <summary>
        /// Resets this instance by considering all in-pins to contain "dirty" values.
        /// </summary>
        /// <remarks>
        /// Keeping a dirty/actual state on the in-pins is necessary if the node hosts
        /// an instance of <see cref="ICalculationPerformer"/>. In this case the calculation
        /// should only be performed once in an execution step and not each time an in-pin
        /// changes its value.
        /// </remarks>
        public void Reset()
        {
            foreach (IInPin inPin in _inPinList)
            {
                _inPinActualList[inPin] = false;
            }
        }

        /// <summary>
        /// Attaches this Node's object's member (specified by thisMember) as outgoing pin to the specified member of the object hosted by the other node. 
        /// A member can be any field or property.
        /// </summary>
        /// <param name="thisMemberOut">The member of this node emitting values.</param>
        /// <param name="other">The other node.</param>
        /// <param name="otherMemberIn">The other node's object's member receiving values.</param>
        /// <remarks>
        /// This is a high-level method users can call to do the wiring inside a <see cref="Circuit"/>. It creates all the necessary in- and out-pins
        /// together with their respective member accessors.
        /// </remarks>
        public void Attach(string thisMemberOut, Node other, string otherMemberIn)
        {
            IOutPin outPin = GetOutPin(thisMemberOut);
            IInPin inPin = other.GetInPin(otherMemberIn, outPin.GetPinType());
            outPin.Attach(inPin);
        }

        /// <summary>
        /// Finds an existing out-pin already present an wired to the given member (field or property).
        /// If no such out-pin exists, a new one is created.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>A newly created or already existing out-pin</returns>
        private IOutPin GetOutPin(string member)
        {
            // See if the outpin pinning the given member already exists. If not create one.
            IOutPin outPin = _outPinList.Find(p => p.Member == member);
            if (outPin == null)
            {
                outPin = PinFactory.CreateOutPin(this, member);
                _outPinList.Add(outPin);
            }
            return outPin;
        }

        /// <summary>
        /// Create a new in-pin for the given member (field or property). If the member is already exposed by an in-pin
        /// an exception is thrown because a member cannot be governed by two in-pins. If a non-null target type is specified,
        /// the member will be connected to the pin using <see cref="ConvertingFieldAccessor{TPin,TObj}"/> or 
        /// <see cref="ConvertingPropertyAccessor{TPin,TObj}"/>.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">A member is already connected as InPin.</exception>
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


        internal void OnReceivedValue(IInPin inPin, EventArgs args)
        {
            _inPinActualList[inPin] = true;
            if (AllPinsActual)
                Propagate();
        }

        /// <summary>
        /// Propagates the values at the object's members' to all in-pins connected to each out-pin connected
        /// to a member of this Node's hosted object.
        /// </summary>
        public void Propagate()
        {
            if (_cp != null)
                _cp.PerformCalculation();

            foreach (IOutPin outPin in _outPinList)
            {
                outPin.Propagate();
            }
        }

        /// <summary>
        /// Gets the out pins.
        /// </summary>
        /// <value>
        /// The out pins.
        /// </value>
        public IEnumerable<IOutPin> OutPins
        {
            get { return _outPinList; }
        }

        /// <summary>
        /// Gets the in pins.
        /// </summary>
        /// <value>
        /// The in pins.
        /// </value>
        public IEnumerable<IInPin> InPins
        {
            get { return _inPinList; }
        }

        /// <summary>
        /// Gets a value indicating whether all in-pins have been updated actual since the last call to <see cref="Reset"/>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all pins are actual; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Removes all pins.
        /// </summary>
        public void RemoveAllPins()
        {
            _outPinList.Clear();
            _inPinList.Clear();
            _inPinActualList.Clear();
        }
    }
}