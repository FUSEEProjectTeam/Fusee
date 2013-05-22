using System;

namespace Fusee.Engine
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
        public string Name;
        public string Description;
        // TBI: public Icon Icon;

        public FuseeApplicationAttribute()
        {
            
        }
    }
}
