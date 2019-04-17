using Fusee.Engine.Common;
using Fusee.Math.Core;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Special case of an <see cref="InputDevice"/> identifying itself as a <see cref="DeviceCategory.SixDOF"/>.
    /// Defines convenience methods to access the typical gamepad axes and buttons. Registers
    /// the gamepad dpad axes.
    /// </summary>
    public class SixDOFDevice : InputDevice
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SixDOFDevice"/> class.
        /// </summary>
        /// <param name="inpDeviceImp">The platform dependent connector to the underlying physical device.</param>
        public SixDOFDevice(IInputDeviceImp inpDeviceImp) : base(inpDeviceImp)
        {
        }

        /// <summary>
        /// Gets the x value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position in x direction.
        /// </value>
        public float X => GetAxis((int)SixDOF.XAxis);

        /// <summary>
        /// Gets the y value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position in y direction.
        /// </value>
        public float Y => GetAxis((int)SixDOF.YAxis);

        /// <summary>
        /// Gets the z value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position in z direction.
        /// </value>
        public float Z => GetAxis((int)SixDOF.ZAxis);

        /// <summary>
        /// Gets the x rotation value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position around the x axis.
        /// </value>
        public float XRot => GetAxis((int)SixDOF.XRotation);

        /// <summary>
        /// Gets the y rotation value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position around the y axis.
        /// </value>
        public float YRot => GetAxis((int)SixDOF.YRotation);

        /// <summary>
        /// Gets the z rotation value.
        /// </summary>
        /// <value>
        /// The SixDOF device´s deflection from its resting position around the z axis.
        /// </value>
        public float ZRot => GetAxis((int)SixDOF.ZRotation);
        


        /// <summary>
        /// Retrieves information about the x axis.
        /// </summary>
        /// <value>
        /// The description for the x axis.
        /// </value>
        public AxisDescription XDesc => GetAxisDescription((int)SixDOF.XAxis);
        /// <summary>
        /// Retrieves information about the y axis.
        /// </summary>
        /// <value>
        /// The description for the y axis.
        /// </value>
        public AxisDescription YDesc => GetAxisDescription((int)SixDOF.YAxis);
        /// <summary>
        /// Retrieves information about the z axis.
        /// </summary>
        /// <value>
        /// The description for the z axis.
        /// </value>
        public AxisDescription ZDesc => GetAxisDescription((int)SixDOF.ZAxis);
        /// <summary>
        /// Retrieves information about the x rotation axis.
        /// </summary>
        /// <value>
        /// The description for the x rotation axis.
        /// </value>
        public AxisDescription XRotDesc => GetAxisDescription((int)SixDOF.XRotation);
        /// <summary>
        /// Retrieves information about the y rotation axis.
        /// </summary>
        /// <value>
        /// The description for the y rotation axis.
        /// </value>
        public AxisDescription YRotDesc => GetAxisDescription((int)SixDOF.YRotation);
        /// <summary>
        /// Retrieves information about the z rotation axis.
        /// </summary>
        /// <value>
        /// The description for the z rotation axis.
        /// </value>
        public AxisDescription ZRotDesc => GetAxisDescription((int)SixDOF.ZRotation);


    }
}
