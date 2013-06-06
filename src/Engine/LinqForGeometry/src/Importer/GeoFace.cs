/*
	Author: Dominik Steffen
	E-Mail: dominik.steffen@hs-furtwangen.de, dominik.steffen@gmail.com
	Bachlor Thesis Summer Semester 2013
	'Computer Science in Media'
	Project: LinqForGeometry
	Professors:
	Mr. Prof. C. Müller
	Mr. Prof. W. Walter
*/

using System.Collections.Generic;
using Fusee.Math;

namespace LinqForGeometry.Importer
{
    public struct GeoFace
    {
        internal List<float3> _LFVertices;
        internal List<float2> _UV;
    }
}
