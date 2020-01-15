using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Symbolic value describing the double-touchpoint action with the highest intensity performed by a user with the first two touchpoints.
    /// </summary>
    public enum TwoPointAction
    {
        /// <summary>
        /// The user currently performs no action (no two touchpoints are active or the user
        /// currently does not move the two touchpoints).
        /// </summary>
        None,
        /// <summary>
        /// The user performs a pinch action, e.g. moves the two touchpoints toward each others or moves them into opposite
        /// directions. Use <see cref="TouchDevice.TwoPointDistance"/> or <see cref="TouchDevice.TwoPointDistanceVel"/> to retrieve
        /// values for derived actions based on pinch gestures.
        /// </summary>
        Pinch,
        /// <summary>
        /// The user moves the first two active touchpoints into similar directions maintaining their distance and relative position. 
        /// Use <see cref = "TouchDevice.TwoPointMidPoint" /> or <see cref = "TouchDevice.TwoPointMidPointVel" /> to retrieve
        /// values for derived actions based on two-point move-gestures.     
        /// </summary>
        Move,
        /// <summary>
        /// The user rotates the first two active touchpoints around a common rotation center while maintaining their distance.
        /// Use <see cref = "TouchDevice.TwoPointAngle" /> or <see cref = "TouchDevice.TwoPointAngleVel" /> to retrieve
        /// values for derived actions based on rotation gestures.     
        /// </summary>
        Rotate
    }

    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.Touch"/>.
    /// Defines convenience methods to access the touch buttons and axes.
    /// </summary>
    /// <remarks>
    /// A touch device represents input typically performed by fingers or a stylus. Touch devices support
    /// simultaneous presence of multiple touchpoints (performed by multiple fingers). Each touchpoint 
    /// is captured by four axes and one button. The button state (pressed or not pressed) signals the presence
    /// of the respective axis. Two positional axes for each touchpoint reveal the position in X/Y screen space 
    /// and two velocity axes represent the current velocity along X and Y.
    /// 
    /// In Addition to tracking individual touchpoints, this device also tracks the simultaneous movements of the 
    /// first two active touchpoints and generates derived information, like two-point-move, pinch or rotate.
    /// </remarks>
    public class TouchDevice : InputDevice
    {
        private readonly int[] _velocityIDs;
        // tp means "Two-(Touch)Point..."
        private readonly int _tpDistance;
        private readonly int _tpDistanceVel;
        private readonly int _tpAngle;
        private readonly int _tpAngleVel;
        private readonly int _tpMidPointX;
        private readonly int _tpMidPointVelX;
        private readonly int _tpMidPointY;
        private readonly int _tpMidPointVelY;

        /// <summary>
        /// Initializes a new instance of the <see cref="TouchDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The "driver".</param>
        public TouchDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
            int nTouchpoints = ButtonCount;
            int nVelAxes = nTouchpoints*2;
            _velocityIDs = new int[nVelAxes];
            int axisId = (int) TouchAxes.Touchpoint_0_X;
            int buttonId = (int) TouchPoints.Touchpoint_0;
            for (int i = 0; i < nVelAxes; i++)
            {
                _velocityIDs[i] = RegisterVelocityAxis(axisId++, buttonId).Id;
                i++;
                _velocityIDs[i] = RegisterVelocityAxis(axisId++, buttonId).Id;
            }
            _tpDistance = RegisterTtpDistanceAxis();
            _tpDistanceVel = RegisterVelocityAxis(_tpDistance, (int) TouchPoints.Touchpoint_1).Id;
            _tpAngle = RegisterTtpAngleAxis();
            _tpAngleVel = RegisterVelocityAxis(_tpAngle, (int)TouchPoints.Touchpoint_1).Id;
            _tpMidPointX = RegisterTtpMidpointAxis((int)TouchAxes.Touchpoint_0_X, (int)TouchAxes.Touchpoint_1_X, (int)TouchAxes.MinX, (int)TouchAxes.MaxX, AxisDirection.X, "Double-Touch Midpoint X");
            _tpMidPointVelX = RegisterVelocityAxis(_tpMidPointX, (int)TouchPoints.Touchpoint_1).Id;
            _tpMidPointY = RegisterTtpMidpointAxis((int)TouchAxes.Touchpoint_0_Y, (int)TouchAxes.Touchpoint_1_Y, (int)TouchAxes.MinY, (int)TouchAxes.MaxY, AxisDirection.Y, "Double-Touch Midpoint Y");
            _tpMidPointVelY = RegisterVelocityAxis(_tpMidPointY, (int)TouchPoints.Touchpoint_1).Id;
        }

        /// <summary>
        /// Registers a new (Two-) touch point angle and returns the id.
        /// </summary>
        /// <returns>The id of the newly registered angle axis.</returns>
        protected int RegisterTtpAngleAxis()
        {
            AxisValueCalculator calculator;

            calculator = delegate
            {
                if (!TwoPoint)
                    return 0;
                float2 p0 = GetPosition(TouchPoints.Touchpoint_0);
                float2 p1 = GetPosition(TouchPoints.Touchpoint_1);
                float2 delta = p1 - p0;
                float angle = (float)System.Math.Atan2(-delta.y, delta.x);  // flip y direction (up is positive) so angle values are "maths-conform".
                return angle;
            };

            int id = NewAxisID;

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id,
                Name = "Double-Touch Angle",
                Direction = AxisDirection.Unknown,
                Nature = AxisNature.Position,
                Bounded = AxisBoundedType.Constant,
                MaxValueOrAxis =  (float)System.Math.PI,
                MinValueOrAxis = -(float)System.Math.PI
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc.Id;
        }

        /// <summary>
        /// Registers a new (Two-) touch point distance and returns its id.
        /// </summary>
        /// <returns>The id of the newly registered distance.</returns>
        protected int RegisterTtpDistanceAxis()
        {
            AxisValueCalculator calculator;

            calculator = delegate
            {
                if (!TwoPoint)
                    return 0;
                float2 p0 = GetPosition(TouchPoints.Touchpoint_0);
                float2 p1 = GetPosition(TouchPoints.Touchpoint_1);
                float distance = (p1 - p0).Length;
                return distance;
            };

            int id = NewAxisID;

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id,
                Name = "Double-Touch Distance",
                Direction = AxisDirection.Unknown,
                Nature = AxisNature.Position,
                Bounded = AxisBoundedType.Unbound,
                // TODO: Set min and max axes to 0 and window diagonal as bounding axes.
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc.Id;
        }

        /// <summary>
        /// Registers a new (Two-) touch point midpoint and returns its id.
        /// </summary>
        /// <param name="axId0">The first point.</param>
        /// <param name="axId1">The second point.</param>
        /// <param name="axIdMin">The minimum value of the chosen axis.</param>
        /// <param name="axIdMax">The maximum value of the chosen axis.</param>
        /// <param name="dir">The direction of the axis.</param>
        /// <param name="name">The name of the axis.</param>
        /// <returns>The id of the registered midpoint.</returns>
        protected int RegisterTtpMidpointAxis(int axId0, int axId1, int axIdMin, int axIdMax, AxisDirection dir, string name)
        {
            AxisValueCalculator calculator;

            calculator = delegate
            {
                if (!TwoPoint)
                    return 0;
                float v0 = GetAxis(axId0);
                float v1 = GetAxis(axId1);
                float midpoint = (v0 + v1) * 0.5f;
                return midpoint;
            };

            int id = NewAxisID;

            var calculatedAxisDesc = new AxisDescription
            {
                Id = id,
                Name = name,
                Direction = dir,
                Nature = AxisNature.Position,
                Bounded = AxisBoundedType.OtherAxis,
                MinValueOrAxis = axIdMin,
                MaxValueOrAxis = axIdMax
            };

            RegisterCalculatedAxis(calculatedAxisDesc, calculator);
            return calculatedAxisDesc.Id;
        }


        /// <summary>
        /// Returns a value signalling if the given touchpoint is currently active (if something hits the screen).
        /// </summary>
        /// <param name="touch">The touchpoint to check.</param>
        /// <returns>true if this touch is acitve (a finger is on the screen), otherwise false.</returns>
        public bool GetTouchActive(TouchPoints touch)
        {
            return GetButton((int) touch);
        }

        /// <summary>
        /// Returns the current position of the given touchpoint. 
        /// The returned values are only valid if <see cref="GetTouchActive"/> holds true for the same touchpoint.
        /// </summary>
        /// <param name="touch">The touchpoint.</param>
        /// <returns>The X/Y postion of the given touchpoint.</returns>
        public float2 GetPosition(TouchPoints touch)
        {
            return new float2(
                GetAxis(2*(touch - TouchPoints.Touchpoint_0) + (int)TouchAxes.Touchpoint_0_X),
                GetAxis(2*(touch - TouchPoints.Touchpoint_0) + (int)TouchAxes.Touchpoint_0_Y)
                );
        }

        /// <summary>
        /// Retrieves the current velocity (in pixels per second) of the giben touch point. 
        /// The returned values are only valid if <see cref="GetTouchActive"/> holds true for the same touchpoint.
        /// </summary>
        /// <param name="touch">The touchpoint.</param>
        /// <returns>The two-dimensional velocitiy of the touchpoint.</returns>
        public float2 GetVelocity(TouchPoints touch)
        {
            return new float2(
                GetAxis(_velocityIDs[2*(touch - TouchPoints.Touchpoint_0)]),
                GetAxis(_velocityIDs[2*(touch - TouchPoints.Touchpoint_0) + 1])
                );
        }

        /// <summary>
        /// Retrieves the number of currently active touchpoints (e.g. the number of fingers currently touching the screen).
        /// </summary>
        /// <value>
        /// The number of active touchpoints.
        /// </value>
        public int ActiveTouchpoints => (int)GetAxis((int)TouchAxes.ActiveTouchpoints);

        /// <summary>
        /// Gets a value indicating whether two touchpoints are active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if currently two touchpoints are active; otherwise, <c>false</c>.
        /// </value>
        public bool TwoPoint => ActiveTouchpoints >= 2;


        // These values adjust the detection which of the different TwoPointActions, Move, Rotate or Pinch
        // is currently performed (to the highest amount). 
        private readonly float _angleVelNormalFactor = 500.0f;
        private readonly float _midpointVelNormalFactor = 1.0f;
        private readonly float _distanceVelNormalFactor = 1.0f;
        private readonly float _doubleTouchMovementThreshold = 0.001f;

        /// <summary>
        /// Gets a value indicating the <see cref="TwoPointAction"/> currently performed with the highest intensity.
        /// </summary>
        /// <value>
        /// The double touch action currently performed by the user with the highest intensity.
        /// </value>
        /// <remarks>
        /// Two moving touchpoints on the screen can be interpreted in various ways. It's up to the application to interpret the two individual changes in position
        /// as <see cref="TwoPointAction.Move"/> , as a <see cref="TwoPointAction.Rotate"/> or as a <see cref="TwoPointAction.Pinch"/> . Use this method to retrieve
        /// a symbolic value which of these three possibilites is currently performed with the highest intensity. Applications should sensibly decide whether to 
        /// allow to change their behavior during a two-point-gesture (as long as <see cref="TwoPoint"/> holds true) or to check this value only initially whenever
        /// a two-point gesture starts, or do something in-between. As a practice: Investigate how the 
        /// <a href="https://play.google.com/store/apps/details?id=com.google.earth">Google Earth mobile app</a> handles this question!
        /// </remarks>
        public TwoPointAction TwoPointGesture
        {
            get
            {
                if (!TwoPoint)
                    return TwoPointAction.None;

                float angleNormalized    = System.Math.Abs(TwoPointAngleVel *_angleVelNormalFactor);
                float midpointNormalized = TwoPointMidPointVel.Length * _midpointVelNormalFactor;
                float distanceNormalized = System.Math.Abs(TwoPointDistanceVel * _distanceVelNormalFactor);

                if (angleNormalized < _doubleTouchMovementThreshold && 
                    midpointNormalized < _doubleTouchMovementThreshold && 
                    distanceNormalized < _doubleTouchMovementThreshold)
                    return TwoPointAction.None;

                // Find out the maximum of normalized values.
                if (angleNormalized > midpointNormalized)
                {
                    if (angleNormalized > distanceNormalized)
                        return TwoPointAction.Rotate;
                    return TwoPointAction.Pinch;
                }
                if (midpointNormalized > distanceNormalized)
                    return TwoPointAction.Move;
                return TwoPointAction.Pinch;
            }
        }

        /// <summary>
        /// Gets the distance between the first two touchpoints. Use this value if you want to implement pinch-like actions based on the current absolute distance.
        /// </summary>
        /// <value>
        /// The distance between the first two active touchpoints, or zero, if no two touchpoints are active.
        /// </value>
        public float  TwoPointDistance => GetAxis(_tpDistance);
        /// <summary>
        /// Gets velocity of the distance between the first two touchpoints. Use this value if you want to implement pinch-like actions based on the current speed of the distance.
        /// </summary>
        /// <value>
        /// The distance velocity. Positive values mean fingers move away from each others (Zoom-In), negative values mean fingers approach each others (Zoom-Out).
        /// </value>
        public float  TwoPointDistanceVel => GetAxis(_tpDistanceVel);
        /// <summary>
        /// Gets the angle of a line between the first two active touchpoints measured from the positive screen x-axis. Use this value if you want to implement rotation-like actions 
        /// based on the current absolute angle.
        /// </summary>
        /// <value>
        /// The angle value in radians. Ranges between -PI and PI. An angle of zero denotes the positive x-axis.
        /// </value>
        public float  TwoPointAngle => GetAxis(_tpAngle);
        /// <summary>
        /// Gets the angular velocity of a line between the first two active touchpoints measured from the positive screen x-axis. Use this value if you want to implement rotation-like actions 
        /// based on the current rotation speed of the first two touchpoints.
        /// </summary>
        /// <value>
        /// The angular velocity of the rotation movement of the first two touchpoints. Positive values mean counter-clockwise rotation, negative values mean clockwise rotation.
        /// </value>
        public float  TwoPointAngleVel => GetAxis(_tpAngleVel);
        /// <summary>
        /// Gets midpoint between the first two active touch points. Use this value if you want to implement two-finger movement-like actions based on the current averaged
        /// absolute position of the first two touchpoints
        /// </summary>
        /// <value>
        /// The midpoint (halfway between) touchpoint 0 and touchpoint 1.
        /// </value>
        public float2 TwoPointMidPoint => new float2(GetAxis(_tpMidPointX), GetAxis(_tpMidPointY));
        /// <summary>
        /// Gets speed of the midpoint between the first two active touch points. Use this value if you want to implement two-finger movement-like actions based on the current 
        /// averaged speed of the first two touchpoints.
        /// </summary>
        /// <value>
        /// The two-dimenstional speed vector of the midpoint between touchpoint 0 and touchpoint 1.
        /// </value>
        public float2 TwoPointMidPointVel => new float2(GetAxis(_tpMidPointVelX), GetAxis(_tpMidPointVelY));
    }
}
