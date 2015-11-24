using System;

namespace Fusee.Engine.Common
{

    /// <summary>
    /// Use the InjectMe attribute to mark fields, properties or parameters of methods to be injectable by some
    /// (soon to be implemented) dependency injection framework. For the time being these values need to be set
    /// hardwired somewhere and the attribute us just intended as a markter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectMeAttribute : Attribute
    {
    }

    
    /// <summary>
    /// Use the FuseeApplication attribute to mark RenderCanvas derivatives as applications that can be collected
    /// by Application browsers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FuseeApplicationAttribute : Attribute
    {
        /// <summary>
        /// The name of the currently selected application.
        /// </summary>
        public string Name;

        /// <summary>
        /// The description of the currently selected application.
        /// </summary>
        public string Description;

        // TBI: public Icon Icon;

        /// <summary>
        /// The width of the application's window.
        /// </summary>
        public int Width = -1;

        /// <summary>
        /// The height of the application's window.
        /// </summary>
        public int Height = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuseeApplicationAttribute"/> class.
        /// </summary>
        public FuseeApplicationAttribute()
        {
            
        }
    }
}
