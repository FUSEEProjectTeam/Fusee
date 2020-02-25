﻿using Fusee.Math.Core;
using System.Collections.Generic;

namespace Fusee.Examples.AdvancedUI.Core
{
    internal struct UIInput
    {
        //"Position" and "Size" determine a rectangle - with "Position" as its center - that encloses the object we want to mark with UI elements.
        //Could be the tangent plane with the correct size and center at "Position".
        internal float3 Position;

        internal float2 Size;

        internal string SegmentationClass;
        internal float Probability;

        internal UIHelper.AnnotationKind AnnotationKind;

        //Triangles that belong to the object we want to mark.
        internal List<int> AffectedTriangles;

        internal bool IsVisible;
        internal float2 CircleCanvasPos;
        internal float2 CircleCanvasPosCache;
        internal float2 AnnotationCanvasPos;
        internal int Identifier;

        public UIInput(UIHelper.AnnotationKind annotationKind, float3 pos, float2 size, string segRes, float probability)
        {
            Position = pos;
            Size = size;
            AffectedTriangles = new List<int>();
            SegmentationClass = segRes;
            Probability = probability;
            IsVisible = false;
            AnnotationKind = annotationKind;

            CircleCanvasPos = new float2();
            CircleCanvasPosCache = new float2();
            AnnotationCanvasPos = new float2();

            Identifier = -1;
        }
    }
}