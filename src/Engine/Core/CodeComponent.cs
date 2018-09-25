using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusee.Serialization;

namespace Fusee.Engine.Core
{
    /// <summary>
    /// Use this component to add Code to Objects in the Scenegraph. Will not serialize/deserialize
    /// </summary>
    public class CodeComponent: SceneComponentContainer
    {
        public delegate void OnClick();

        public delegate void OnMouseEnter();

        public delegate void OnMouseLeave();

        public delegate void OnMouseOver();

        public delegate void OnMouseDown();

        public delegate void OnMouseUp();

    }
}
