using System;

namespace Fusee.Base.Common
{
    /// <summary>
    /// Use the InjectMe attribute to mark fields, properties or parameters of methods to be injectable by some
    /// (soon to be implemented) dependency injection framework. For the time being these values need to be set
    /// hardwired somewhere and the attribute is just intended as a marker.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class InjectMeAttribute : Attribute
    {
    }

}
