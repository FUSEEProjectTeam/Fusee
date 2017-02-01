using System.Collections.Generic;

namespace Fusee.Jometri.DCEL
{
    /// <summary>
    /// Contains 3D geometry as a result of an extrusion.
    /// </summary>
    public class Extrusion : Geometry
    {
        /// <summary>
        /// Contains all faces which belong to the front of the geometry.
        /// </summary>
        public Dictionary<int, Face> FrontFaces;

        /// <summary>
        /// Contains all faces which belong to the back of the geometry.
        /// </summary>
        public Dictionary<int, Face> Backfaces;
    }
}
