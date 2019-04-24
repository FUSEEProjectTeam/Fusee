using System;
using System.Collections.Generic;
using System.Linq;
using Fusee.Engine.Common;

namespace Fusee.Engine.Core
{

    /// <summary>
    /// Input device instances expose access to underlying physical input devices such as mouse, keyboard, game pads etc.
    /// Users can either poll axis values or button state from an Input device or add event listeners reacting
    /// on value or state changes, no matter how the underlying physical device provides axis or button data.
    /// Additionally users can define their own axes by specifying calculation rules how to generate values from 
    /// existing axes or buttons.
    /// </summary>
    public class InputDevice
    {
        private IInputDeviceImp _inpDevImp;
        internal IInputDeviceImp DeviceImp => _inpDevImp;

        private readonly Dictionary<int, AxisDescription> _axes;        // All axes provided by this device. Includes polled, listened and calulated axes
        private readonly Dictionary<int, float> _axesToPoll;            // Axes that need to be polled 
        private readonly Dictionary<int, float> _axesToListen;          // Axes changed by event from the underlying implementation

        private readonly Dictionary<int, ButtonDescription> _buttons;        // All buttons provided by this device.
        private readonly Dictionary<int, bool> _buttonsToPoll;               // Buttons that need to be polled 
        private readonly Dictionary<int, bool> _buttonsToListen;             // Buttons changed by event from the underlying implementation
        private readonly Dictionary<int, bool> _buttonsToListenJustChanged;  // Temporary list of changed buttons to fire notification events right before rendering.

        private readonly HashSet<int> _buttonsUp;
        private readonly HashSet<int> _buttonsDown;

        private int _nextAxisId;

        internal InputDevice(IInputDeviceImp inpDeviceImp)
        {
            if (inpDeviceImp == null)
                throw new ArgumentNullException(nameof(inpDeviceImp));

            _inpDevImp = inpDeviceImp;
            _isConnected = true;

            #region Handle Axes
            _axes = new Dictionary<int, AxisDescription>();
            _axesToPoll = new Dictionary<int, float>();
            _axesToListen = new Dictionary<int, float>();
            _calculatedAxes = new Dictionary<int, CalculatedAxisDescription>();

            _nextAxisId = 0;
            // Look for each axis if its pollable or listenable. Prepare the axis to be accessed 
            // the other way (poll a listenable axis / listen to a pollable axis).
            foreach (var axisImpDescription in _inpDevImp.AxisImpDesc)
            {
                // Keep track of IDs (to keep them uniqe)
                int axisId = axisImpDescription.AxisDesc.Id;
                if (axisId > _nextAxisId)
                    _nextAxisId = axisId;

                // Store axis in the list of all axes
                _axes[axisId] = axisImpDescription.AxisDesc;

                // Prepare axis to be accessed either way (polled or listened)
                if (axisImpDescription.PollAxis)
                    _axesToPoll[axisId] = _inpDevImp.GetAxis(axisId);
                else
                    _axesToListen[axisId] = 0.0f;
            }
            _nextAxisId++;

            if (_axesToListen.Count > 0)
                _inpDevImp.AxisValueChanged += OnImpAxisValueChanged;
            #endregion

            #region Handle Buttons
            _buttons = new Dictionary<int, ButtonDescription>();
            _buttonsToPoll = new Dictionary<int, bool>();
            _buttonsToListen = new Dictionary<int, bool>();
            _buttonsUp = new HashSet<int>();
            _buttonsDown = new HashSet<int>();
            _buttonsToListenJustChanged = new Dictionary<int, bool>();

            // Do the same for buttons
            foreach (var buttonImpDescription in _inpDevImp.ButtonImpDesc)
            {
                int buttonId = buttonImpDescription.ButtonDesc.Id;

                _buttons[buttonId] = buttonImpDescription.ButtonDesc;

                if (buttonImpDescription.PollButton)
                    _buttonsToPoll[buttonId] = _inpDevImp.GetButton(buttonId);
                else
                    _buttonsToListen[buttonId] = false;
            }

            if (_buttonsToListen.Count > 0)
                _inpDevImp.ButtonValueChanged += OnImpButtonValueChanged;
            #endregion
        }

        /// <summary>
        /// Gets the new next possible new axis identifier. Use this when calling <see cref="RegisterCalculatedAxis"/>.
        /// </summary>
        /// <value>
        /// The new axis identifier.
        /// </value>
        public int NewAxisID => _nextAxisId + 1;

        private void OnImpButtonValueChanged(object sender, ButtonValueChangedArgs args)
        {
            if (!_buttonsToListen.ContainsKey(args.Button.Id))
                throw new InvalidOperationException($"Unknown Button {args.Button.Name} ({args.Button.Id})");

            // Just save the information about the changed value. All other handling including 
            // firing the user event is done in PreRender.
            _buttonsToListenJustChanged[args.Button.Id] = args.Pressed;
        }

        private void OnImpAxisValueChanged(object sender, AxisValueChangedArgs args)
        {
            if (!_axesToListen.ContainsKey(args.Axis.Id))
                throw new InvalidOperationException($"Unknown Axis {args.Axis.Name} ({args.Axis.Id})");

            _axesToListen[args.Axis.Id] = args.Value;
            AxisValueChanged?.Invoke(this, args);
        }

        internal bool _isConnected;

        internal void Reconnect(IInputDeviceImp deviceImp)
        {
            if (_isConnected) throw  new InvalidOperationException($"Cannot reconnect already connected input device (connected to {_inpDevImp.Desc}). Disconnect first.");
            if (deviceImp == null) throw new ArgumentNullException(nameof(deviceImp));

            _inpDevImp = deviceImp;
            _inpDevImp.AxisValueChanged += OnImpAxisValueChanged;
            _inpDevImp.ButtonValueChanged += OnImpButtonValueChanged;

            _isConnected = true;
        }

        internal void Disconnect()
        {
            // The driver informed the input system that this device has disconnected. Set all axes to 0 and all buttons to false.
            foreach (var axisId in _axesToListen.Keys.ToArray())
            {
                if (AxisValueChanged != null && _axesToListen[axisId] != 0)
                {
                    // Inform axes-to-listen about the new value (the disconnected device won't do it)...
                    // Do NOT inform axes-to-poll here since they will be informed in PreRender anyway.
                    AxisValueChanged(this, new AxisValueChangedArgs {Axis = _axes[axisId], Value = 0});
                }
                _axesToListen[axisId] = 0;
            }

            foreach (var axisId in _axesToPoll.Keys.ToArray())
            {
                _axesToPoll[axisId] = 0;
            }

            foreach (var buttonId in _buttonsToListen.Keys.ToArray())
            {
                if (ButtonValueChanged != null && _buttonsToListen[buttonId])
                {
                    // Inform buttons-to-listen about the new value (the disconnected device won't do it)...
                    // Do NOT inform buttons-to-poll here since they will be informed in PreRender anyway.
                    ButtonValueChanged(this, new ButtonValueChangedArgs { Button = _buttons[buttonId], Pressed  = false });
                }
                _buttonsToListen[buttonId] = false;
            }

            foreach (var buttonId in _buttonsToPoll.Keys.ToArray())
            {
                _buttonsToPoll[buttonId] = false;
            }

            // Actually disconnect by releasing any reference to the implementation object.
            _inpDevImp.AxisValueChanged -= OnImpAxisValueChanged;
            _inpDevImp.ButtonValueChanged -= OnImpButtonValueChanged;
            _inpDevImp = null;

            _isConnected = false;
        }

        /// <summary>
        /// Gets and sets a value indicating whether this device is connected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this device is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Gets an identifier. Implementors take care that this
        /// id is unique across all devices managed by a driver.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id => _inpDevImp.Id;

        /// <summary>
        /// Gets the human readable description of this device. This
        /// parameter can be used in user dialogs to identify devices.
        /// </summary>
        /// <value>
        /// The deivce description.
        /// </value>
        public string Desc => _inpDevImp.Desc;

        /// <summary>
        /// Gets the category of this device. Device categories define a minimal common
        /// set of buttons and axes which are identical across all devices sharing the same
        /// category.
        /// </summary>
        /// <value>
        /// The device category.
        /// </value>
        public DeviceCategory Category => _inpDevImp.Category;

        /// <summary>
        /// Gets number of axes supported by this device.
        /// </summary>
        /// <value>
        /// The axes count.
        /// </value>
        public int AxesCount => _axes.Count;

        /// <summary>
        /// Gets a description of the axis. This value can be used in user setup-dialogs or 
        /// to match axes of devices of different categories.
        /// </summary>
        /// <value>
        /// The description of the axis.
        /// </value>
        public IEnumerable<AxisDescription> AxisDesc => _axes.Values;

        /// <summary>
        /// Retrieves a description for the given axis
        /// </summary>
        /// <param name="axisId">The axis identifier.</param>
        /// <returns>A description of the axis.</returns>
        public AxisDescription GetAxisDescription(int axisId)
        {
            AxisDescription desc;
            if (_axes.TryGetValue(axisId, out desc))
                return desc;

            throw new InvalidOperationException($"Cannot retrieve axis information for unknon axis {axisId}.");
        }

        /// <summary>
        ///     Gets the value currently present at the given axis.
        /// </summary>
        /// <param name="axisId">The axis' Id as specified in <see cref="AxisDesc"/>.</param>
        /// <returns>The value currently set on the axis.</returns>
        /// <remarks>
        ///     See <see cref="AxisDescription"/> to get information about how to interpret the
        ///     values returned by a given axis.
        ///  </remarks>
        public float GetAxis(int axisId)
        {
            if (!_isConnected)
                return 0;

            float value;
            if (TryGetPolledAxis(axisId, out value))
                return value;

            if (_axesToListen.TryGetValue(axisId, out value))
                return value;

            throw new InvalidOperationException($"Axis Id {axisId} not supported by device {_inpDevImp.Desc}.");
        }

        private bool TryGetPolledAxis(int iAxisId, out float value)
        {
            CalculatedAxisDescription calculatedAxis;
            if (_calculatedAxes.TryGetValue(iAxisId, out calculatedAxis))
            {
                value = calculatedAxis.CurrentAxisValue;
                return true;
            }

            if (_axesToPoll.ContainsKey(iAxisId))
            {
                value = _inpDevImp.GetAxis(iAxisId);
                return true;
            }
            value = 0;
            return false;
        }

        /// <summary>
        /// Occurs when the value of a given axis has changed.
        /// </summary>
        public event EventHandler<AxisValueChangedArgs> AxisValueChanged;

        /// <summary>
        /// Gets the number of buttons supported by this device.
        /// </summary>
        /// <value>
        /// The button count.
        /// </value>
        public int ButtonCount => _buttons.Count;

        /// <summary>
        /// Gets the name of the button. This value can be used in user setup-dialogs or 
        /// to match buttons of devices of different categories.
        /// </summary>
        /// <value>
        /// The name of the button.
        /// </value>
        public IEnumerable<ButtonDescription> ButtonDesc => _buttons.Values;

        /// <summary>
        /// Retrieves a description for the given button.
        /// </summary>
        /// <param name="buttonId">The axis identifier.</param>
        /// <returns>A description of the axis.</returns>
        public ButtonDescription GetButtonDescription(int buttonId)
        {
            ButtonDescription desc;
            if (_buttons.TryGetValue(buttonId, out desc))
                return desc;

            throw new InvalidOperationException($"Cannot retrieve button information for unknon button {buttonId}.");
        }

        /// <summary>
        /// Gets the current state of the given button.
        /// </summary>
        /// <param name="buttonId">The buttonId of the button as specified in <see cref="ButtonDesc"/>.</param>
        /// <returns>true if the button is currently pressed. false, if the button is currently released.</returns>
        public bool GetButton(int buttonId)
        {
            if (!_isConnected)
                return false;

            bool state;
            if (_buttonsToListen.TryGetValue(buttonId, out state))
                return state;

            if (_buttonsToPoll.ContainsKey(buttonId))
            {
                var btnState = _inpDevImp.GetButton(buttonId);
                _buttonsToPoll[buttonId] = btnState;
                return btnState;
            }

            throw new InvalidOperationException($"Button Id {buttonId} not supported by device {_inpDevImp.Desc}.");
        }

        /// <summary>
        /// Determines whether the button was pressed down right before or during the current frame.
        /// This value is true only for one frame even if the button is pressed longer than one frame.
        /// </summary>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns>
        /// true if the button was pressed during this frame and is still pressed down. false, if the button is released 
        /// or if it was pressed some frames ago.
        /// </returns>
        public bool IsButtonDown(int buttonId)
        {
            if (!_buttons.ContainsKey(buttonId)) throw new InvalidOperationException($"IsButtonDown called for unknon button {buttonId}.");

            return _buttonsDown.Contains(buttonId);
        }

        /// <summary>
        /// Determines whether the button was released right before or during the current frame.
        /// This value is true only for one frame even if the button is released longer than one frame.
        /// </summary>
        /// <param name="buttonId">The button identifier.</param>
        /// <returns>
        /// true if the button was released during this frame and is still released. false, if the button is pressed 
        /// or if it was released some frames ago.
        /// </returns>
        public bool IsButtonUp(int buttonId)
        {
            if (!_buttons.ContainsKey(buttonId)) throw new InvalidOperationException($"IsButtonDown called for unknon button {buttonId}.");

            return _buttonsUp.Contains(buttonId);
        }

        /// <summary>
        /// Occurs when the value of a given button has changed.
        /// </summary>
        public event EventHandler<ButtonValueChangedArgs> ButtonValueChanged;

        #region Derived Axis Handling
        //public delegate float AxisValueCalculator(float oldVal, float newVal, float time);

        // Any state the calculation depends upon should be queried from the device or 
        // "statically" stored in the closure.
        public delegate float AxisValueCalculator(float time);
        private class CalculatedAxisDescription
        {
            public AxisDescription AxisDesc;
            public float CurrentAxisValue;
            public AxisValueCalculator Calculator;
        }

        private Dictionary<int, CalculatedAxisDescription> _calculatedAxes;

        /// <summary>
        /// Registers a calculated axis. Calculated axes behave like axes exposed by the underlying
        /// hardware device but can be calculated from one or more existing axes or buttons.
        /// </summary>
        /// <param name="calculatedAxisDescription">The axis description for the new calculated axis.</param>
        /// <param name="calculator">The calculator method performing the calculation once per frame.</param>
        /// <param name="initialValue">The initial value for the new axis.</param>
        /// <remarks>
        /// To register your own axis you need to provide a working <see cref="AxisValueCalculator"/>. This method
        /// is called whenever the axis value needs to be present.
        /// Any state the calculation depends upon should be queried from existing axes presented by the input device 
        /// or "statically" stored in the closure around the calculator. The methodes
        /// <list type="bullet"></list>
        /// <item><see cref="RegisterSingleButtonAxis"/></item>
        /// <item><see cref="RegisterTwoButtonAxis"/></item>
        /// <item><see cref="RegisterVelocityAxis"/></item>
        /// </remarks>
        /// provide pre-defined calculators for certain purposes.
        public void RegisterCalculatedAxis(AxisDescription calculatedAxisDescription, AxisValueCalculator calculator, float initialValue = 0)
        {
            if (calculatedAxisDescription.Id < _nextAxisId)
                throw new InvalidOperationException($"Invalid Id for calculated axis '{calculatedAxisDescription.Name}'. Id must be bigger or equal to {_nextAxisId}.");

            _nextAxisId = calculatedAxisDescription.Id;


            var calculatedAxis = new CalculatedAxisDescription
            {
                AxisDesc = calculatedAxisDescription,
                CurrentAxisValue = initialValue,
                Calculator = calculator
            };

            // Calculated Axes are always to-be-polled axes.
            _calculatedAxes[calculatedAxisDescription.Id] = calculatedAxis;
            _axesToPoll[calculatedAxisDescription.Id] = calculatedAxis.CurrentAxisValue;

            _axes[calculatedAxisDescription.Id] = calculatedAxisDescription;
        }


        /// <summary>
        /// Registers a calculated axis exhibiting the derivative after the time (Velocity) of the value on the specified original axis.
        /// </summary>
        /// <param name="origAxisId">The original axis identifier.</param>
        /// <param name="triggerButtonId">If a valid id is passed, the derived axis only produces values if the specified button is pressed. The velocity is only
        /// calculated based on the axis value when the trigger button is pressed. This allows touch velocities to always start with a speed of zero when the touch starts (e.g. the 
        /// button identifying that a touchpoint has contact). Otherwise touch velocites would become huge between two click-like touches on different screen locations. 
        /// If this parameter is 0 (zero), the derived axis will always be calculated based on the original axis only.</param>
        /// <param name="velocityAxisId">The derived axis identifier. Note this value must be bigger than all existing axis Ids. Leave this value
        /// zero to have a new identifier calculated automatically.</param>
        /// <param name="name">The name of the new axis.</param>
        /// <param name="direction">The direction of the new axis.</param>
        /// <returns>
        /// The axis description of the newly created calculated axis.
        /// </returns>
        /// <remarks>
        /// A derived axis is helpful if you have a device delivering absolute positional values but you need the current
        /// speed of the axis. Imagine a mouse where the speed of the mouse over the screen is important rather than the absolute
        /// position.
        /// </remarks>
        public AxisDescription RegisterVelocityAxis(int origAxisId, int triggerButtonId = 0,
            int velocityAxisId = 0, string name = null, AxisDirection direction = AxisDirection.Unknown)
        {
            AxisDescription origAxisDesc;
            if (!_axes.TryGetValue(origAxisId, out origAxisDesc))
            {
                throw new InvalidOperationException($"Axis Id {origAxisId} is not known. Cannot register derived axis based on unknown axis.");
            }

            //switch (origAxisDesc.Bounded)
            //{
            //    case AxisBoundedType.Constant:
            //        scale = 1.0f / (origAxisDesc.MaxValueOrAxis - origAxisDesc.MinValueOrAxis);
            //        break;
            //    case AxisBoundedType.OtherAxis:
            //        scale = 1.0f / (GetAxis((int)origAxisDesc.MaxValueOrAxis) - GetAxis((int)origAxisDesc.MinValueOrAxis));
            //        break;
            //}

            AxisValueCalculator calculator;
            if (triggerButtonId != 0)
            {
                ButtonDescription triggerButtonDesc;
                if (!_buttons.TryGetValue(triggerButtonId, out triggerButtonDesc))
                {
                    throw new InvalidOperationException($"Button Id {triggerButtonId} is not known. Cannot register derived axis based on unknown trigger button id.");
                }
                float closureLastValue = GetAxis(origAxisId);
                bool closureOffLastTime = true;
                calculator = delegate (float deltaTime)
                {
                    if (deltaTime <= float.Epsilon) // avoid infinite velocites
                        return 0;

                    if (!GetButton(triggerButtonId))
                    {
                        closureOffLastTime = true;
                        return 0.0f;
                    }

                    if (closureOffLastTime)
                    {
                        closureLastValue = GetAxis(origAxisId);
                        closureOffLastTime = false;
                    }

                    float newVal = GetAxis(origAxisId);
                    float ret = (newVal - closureLastValue) / deltaTime; // v = dr / dt: velocity is position derived after time.
                    closureLastValue = newVal;
                    return ret;
                };
            }
            else
            {
                float closureLastValue = GetAxis(origAxisId);
                calculator = delegate (float deltaTime)
                {
                    if (deltaTime <= float.Epsilon) // avoid infinite velocites
                        return 0;

                    float newVal = GetAxis(origAxisId);
                    float ret = (newVal - closureLastValue) / deltaTime; // v = dr / dt: velocity is position derived after time.
                    closureLastValue = newVal;
                    return ret;
                };
            }


            int id = _nextAxisId + 1;
            if (velocityAxisId > id)
            {
                id = velocityAxisId;
            }

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id,
                Name = name ?? origAxisDesc.Name + " Velocity",
                Direction = (direction == AxisDirection.Unknown) ? origAxisDesc.Direction : direction,
                Nature = AxisNature.Speed,
                Bounded = AxisBoundedType.Unbound,
                MaxValueOrAxis = float.NaN,
                MinValueOrAxis = float.NaN
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc;
        }

        /// <summary>
        /// Registers a calculated axis from a button. The axis' value changes between 0 and 1 as the user hits the button or releases it.
        /// The time it takes to change the value can be set.
        /// </summary>
        /// <param name="origButtonId">The original button identifier.</param>
        /// <param name="direction">The direction the new axis is heading towards.</param>
        /// <param name="rampUpTime">The time it takes to change the value from 0 to 1 (in seconds).</param>
        /// <param name="rampDownTime">The time it takes to change the value from 1 to 0 (in seconds).</param>
        /// <param name="buttonAxisId">The new identifier of the button axis. Note this value must be bigger than all existing axis Ids. Leave this value
        /// zero to have a new identifier calculated automatically.</param>
        /// <param name="name">The name of the new axis.</param>
        /// <returns>The axis description of the newly created calculated axis.</returns>
        /// <remarks>
        ///   Button axes are useful to simulate a trigger or thrust panel with the help of individual buttons. There is a user-defineable acceleration and 
        ///   deceleration period, so a simulation resulting on this input delivers a feeling of inertance.
        /// </remarks>
        public AxisDescription RegisterSingleButtonAxis(int origButtonId, AxisDirection direction = AxisDirection.Unknown, float rampUpTime = 0.2f, float rampDownTime = 0.2f, int buttonAxisId = 0, string name = null)
        {
            ButtonDescription origButtonDesc;
            if (!_buttons.TryGetValue(origButtonId, out origButtonDesc))
            {
                throw new InvalidOperationException($"Button Id {origButtonId} is not known. Cannot register button axis based on unknown button.");
            }

            // Ramp cannot be 90° as this would require special case handling
            if (rampUpTime <= float.MinValue)
                rampUpTime = 2*float.MinValue;
            if (rampDownTime <= float.MinValue)
                rampDownTime = 2*float.MinValue;

            bool closureLastBtnState = GetButton(origButtonId);
            float closureLastAxisValue = 0;
            float closureAnimDirection = 0;
            AxisValueCalculator calculator = delegate(float deltaTime)
            {
                float ret;
                bool newBtnState = GetButton(origButtonId);
                if (newBtnState != closureLastBtnState)
                {
                    // The state of the button has changed
                    closureAnimDirection = (newBtnState ? 0 : 1) - (closureLastBtnState ? 0 : 1);
                    closureLastBtnState = newBtnState;
                }

                if (closureAnimDirection > 0)
                {
                    ret = closureLastAxisValue + deltaTime/rampUpTime;
                    if (ret >= 1)
                    {
                        closureAnimDirection = 0;
                        ret = 1;
                    }
                }
                else if (closureAnimDirection < 0)
                {
                    ret = closureLastAxisValue - deltaTime/rampDownTime;
                    if (ret < 0)
                    {
                        closureAnimDirection = 0;
                        ret = 0;
                    }
                }
                else
                    ret = closureLastAxisValue;

                closureLastAxisValue = ret;
                return ret;
            };

            int id = _nextAxisId + 1;
            if (buttonAxisId > id)
            {
                id = buttonAxisId;
            }

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id, Name = name ?? $"{origButtonDesc.Name} Axis", Bounded = AxisBoundedType.Constant, Direction = direction, Nature = AxisNature.Speed, MaxValueOrAxis = 1.0f, MinValueOrAxis = 0.0f
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc;
        }

        /// <summary>
        /// Registers a calculated axis from two buttons. The axis' value changes between -1 and 1 as the user hits the button or releases it.
        /// The time it takes to change the value can be set.
        /// </summary>
        /// <param name="origButtonIdNegative">The original button identifier for negative movements.</param>
        /// <param name="origButtonIdPositive">The original button identifier for positive movements.</param>
        /// <param name="direction">The direction the new axis is heading towards.</param>
        /// <param name="rampUpTime">The time it takes to change the value from 0 to 1 (or -1) (in seconds) when one of the buttons is pushed.</param>
        /// <param name="rampDownTime">The time it takes to change the value from -1 of 1 back to 0 (in seconds) when a pushed button is released.</param>
        /// <param name="buttonAxisId">The new identifier of the button axis. Note this value must be bigger than all existing axis Ids. Leave this value
        /// zero to have a new identifier calculated automatically.</param>
        /// <param name="name">The name of the new axis.</param>
        /// <returns>
        /// The axis description of the newly created calculated axis.
        /// </returns>
        /// <remarks>
        /// Button axes are useful to simulate one axis of a joypad or a joystick with the help of two individual buttons. One button acts as pushing the 
        /// joystick into the positve direction along the given axis by animating the axis' value to 1 and the a second button acts as pushing the joystick 
        /// into the negative direction by animating the value to -1. Releasing both buttons will animate the value to 0. Pushing both buttons simultaneously
        /// will stop the animation and keep the value at its current amount.
        /// There is a user-defineable acceleration and deceleration period, so a simulation resulting on this input delivers a feeling of inertance.
        /// </remarks>
        public AxisDescription RegisterTwoButtonAxis(int origButtonIdNegative, int origButtonIdPositive, AxisDirection direction = AxisDirection.Unknown, float rampUpTime = 0.15f, float rampDownTime = 0.35f, int buttonAxisId = 0, string name = null)
        {
            ButtonDescription origButtonDescPos;
            if (!_buttons.TryGetValue(origButtonIdPositive, out origButtonDescPos))
            {
                throw new InvalidOperationException($"Button Id {origButtonIdPositive} is not known. Cannot register button axis based on unknown button.");
            }

            ButtonDescription origButtonDescNeg;
            if (!_buttons.TryGetValue(origButtonIdNegative, out origButtonDescNeg))
            {
                throw new InvalidOperationException($"Button Id {origButtonIdNegative} is not known. Cannot register button axis based on unknown button.");
            }


            // Ramp cannot be 90° as this would require special case handling
            if (rampUpTime <= float.MinValue)
                rampUpTime = 2*float.MinValue;
            if (rampDownTime <= float.MinValue)
                rampDownTime = 2*float.MinValue;


            bool closureLastBtnStatePos = GetButton(origButtonIdPositive);
            bool closureLastBtnStateNeg = GetButton(origButtonIdNegative);
            float closureLastAxisValue = 0;
            float closureAnimDirection = 0;
            AxisValueCalculator calculator = delegate(float deltaTime)
            {
                float ret;
                bool newBtnStatePos = GetButton(origButtonIdPositive);
                if (newBtnStatePos != closureLastBtnStatePos)
                {
                    // The state of the button has changed
                    closureAnimDirection = (newBtnStatePos ? 1 : 0) - (closureLastBtnStatePos ? 1 : 0);
                    closureLastBtnStatePos = newBtnStatePos;
                }

                bool newBtnStateNeg = GetButton(origButtonIdNegative);
                if (newBtnStateNeg != closureLastBtnStateNeg)
                {
                    // The state of the button has changed
                    closureAnimDirection = - ((newBtnStateNeg ? 1 : 0) - (closureLastBtnStateNeg ? 1 : 0));
                    closureLastBtnStateNeg = newBtnStateNeg;
                }

                // No button pressed: Goal is 0 (middle) and speed is rampdown
                var animGoal = 0.0f;
                var animTime = rampDownTime;
                if (newBtnStatePos || newBtnStateNeg)
                {
                    // Some button pressed: Goal is -1 or 1 and speed is rampup
                    animGoal = closureAnimDirection;
                    animTime = rampUpTime;
                }

                if (closureAnimDirection > 0)
                {
                    ret = closureLastAxisValue + deltaTime/animTime;
                    if (ret >= animGoal)
                    {
                        closureAnimDirection = 0;
                        ret = animGoal;
                    }
                }
                else if (closureAnimDirection < 0)
                {
                    ret = closureLastAxisValue - deltaTime/animTime;
                    if (ret <= animGoal)
                    {
                        closureAnimDirection = 0;
                        ret = animGoal;
                    }
                }
                else
                    ret = closureLastAxisValue;

                closureLastAxisValue = ret;
                return ret;
            };

            int id = _nextAxisId + 1;
            if (buttonAxisId > id)
            {
                id = buttonAxisId;
            }

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id, Name = name ?? $"{origButtonDescPos.Name} {origButtonDescNeg.Name} Axis", Bounded = AxisBoundedType.Constant, Direction = direction, Nature = AxisNature.Speed, MaxValueOrAxis = 1.0f, MinValueOrAxis = -1.0f
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc;
        }

        #endregion

        internal void PreRender()
        {
            if (Time.DeltaTime != 0)
            {
                // Calculate derived axes first.
                foreach (var derivedAxis in _calculatedAxes) //.OrderBy(a => a.Key))
                {
                    derivedAxis.Value.CurrentAxisValue = derivedAxis.Value.Calculator(Time.DeltaTime);
                }
            }

            // See if any listeners need to be triggered about changes on axes.
            if (AxisValueChanged != null)
            {
                foreach (var axisId in _axesToPoll.Keys.ToArray()) // ToArray: get the list up-front because we will change the _axesToPoll dictionary during iteration
                {
                    float curVal;
                    if (!TryGetPolledAxis(axisId, out curVal))
                        throw new InvalidOperationException($"Invalid axis Id {axisId} - should be polled or derived.");

                    if (_axesToPoll[axisId] != curVal)
                    {
                        AxisValueChanged(this, new AxisValueChangedArgs {Axis = _axes[axisId], Value = curVal});
                        _axesToPoll[axisId] = curVal;
                    }
                }
            }

            // Do this for all polled buttons no matter if a listener is registered to maintain IsKeyDown/IsKeyUp funtionality
            foreach (var buttonId in _buttonsToPoll.Keys.ToArray()) // ToArray: get the list up-front because we will change the _buttonsToPoll dictionary during iteration
            {
                var curVal = _inpDevImp.GetButton(buttonId);
                if (_buttonsToPoll[buttonId] != curVal)
                {
                    if (curVal)
                        _buttonsDown.Add(buttonId);
                    else
                        _buttonsUp.Add(buttonId);

                    ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs {Button = _buttons[buttonId], Pressed = curVal});
                    _buttonsToPoll[buttonId] = curVal;
                }
            }

            // Now handle to-be-listened-to buttons that fired during the last frame until just now.
            foreach (var b in _buttonsToListenJustChanged)
            {
                if (b.Value != _buttonsToListen[b.Key])
                {
                    if (b.Value)
                        _buttonsDown.Add(b.Key);
                    else
                        _buttonsUp.Add(b.Key);
                }

                _buttonsToListen[b.Key] = b.Value;
                ButtonValueChanged?.Invoke(this, new ButtonValueChangedArgs {Button = _buttons[b.Key], Pressed = b.Value});
            }
            _buttonsToListenJustChanged.Clear();
        }
        /// <summary>
        /// Clears the button presses after rendering an image.
        /// </summary>
        public void PostRender()
        {
            _buttonsDown.Clear();
            _buttonsUp.Clear();
        }
    }

    /*
    /// <summary>
    /// Represents one instance of an input device other than keyboard or mouse
    /// </summary>
    public class InputDeviceOld
    {
        public enum Axis
        {
            Horizontal,
            Vertical,
            Z
        }

        private readonly IInputDeviceImp _inputDeviceImp;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevice"/> class.
        /// </summary>
        /// <param name="inputDeviceImp">The input device imp.</param>
        public InputDevice(IInputDeviceImp inputDeviceImp)
        {
            _inputDeviceImp = inputDeviceImp;
        }

        public InputDevice()
        {

        }

        /// <summary>
        /// Gets the current value of one axis (i.e. joystick or trigger).
        /// </summary>
        /// <param name="axis">Specifies the desired axis, can be "horizontal", "vertical" or "z".</param>
        /// <returns>
        /// The current value (between -1.0 and +1.0) for the specified axis.
        /// </returns>
        public float GetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    return _inputDeviceImp.GetXAxis();

                case Axis.Vertical:
                    return _inputDeviceImp.GetYAxis();

                case Axis.Z:
                    return _inputDeviceImp.GetZAxis();

                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Gets the name of the instance 
        /// </summary>
        /// <returns>The product name of the queried input device.</returns>
        public string Name()
        {
            return _inputDeviceImp.GetName();
        }

        /// <summary>
        /// Gets the index of the currently pressed button on the input device.
        /// </summary>
        /// <returns>The index of the currently pressed button</returns>
        public int GetPressedButton()
        {
            return _inputDeviceImp.GetPressedButton();
        }

        /// <summary>
        /// Checks if a specified button is pressed in the current frame on the input device.
        /// </summary>
        /// <param name="buttonIndex">The index of the button that is checked.</param>
        /// <returns>True if the button at the specified index is pressed in the current frame and false if not.</returns>
        public bool IsButtonDown(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonDown(buttonIndex);
        }

        /// <summary>
        /// Checks if a specified button is held down for more than one frame.
        /// </summary>
        /// <param name="buttonIndex">The index of the button that is checked.</param>
        /// <returns>True if the button at the specified index is held down for more than one frame and false if not.</returns>
        public bool IsButtonPressed(int buttonIndex)
        {
            return _inputDeviceImp.IsButtonPressed(buttonIndex);
        }

        /// <summary>
        /// Counts the buttons on the input device.
        /// </summary>
        /// <returns>The amount of buttons on the device.</returns>
        public int GetButtonCount()
        {
            return _inputDeviceImp.GetButtonCount();
        }

        /// <summary>
        /// Gets the category of the input device.
        /// </summary>
        /// <returns>The name of the Device Category.</returns>
        public String GetCategory()
        {
            return _inputDeviceImp.GetCategory();
        }
    }
    */
}
