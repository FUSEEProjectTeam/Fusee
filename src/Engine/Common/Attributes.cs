﻿using System;

namespace Fusee.Engine.Common
{
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
        public string? Name;

        /// <summary>
        /// The description of the currently selected application.
        /// </summary>
        public string? Description;

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