using C4d;
using System;
using Fusee.Math.Core;

namespace DoubleCircle
{

    [ObjectPlugin(1000002,
        Name = "Double Circle",
        IconFile = "DoubleCircleLogo.tif",
        Generator = true,
        IsSpline = true)
    ]
    class DoubleCircleClass : ObjectDataM
    {
        private static readonly int CIRCLEOBJECT_RAD = 10000;           // values 1000.3000 already reserved from includes
        private static readonly int CIRCLEOBJECT_NEWTAB = 10001;
        private static readonly int CIRCLEOBJECT_SUBGROUP = 10002;
        private static readonly int CIRCLEOBJECT_CHECKME = 10003;

		public override bool Init(GeListNode node)
        {
	        BaseContainer data = GetDataInstance(node);

	        data.SetReal(CIRCLEOBJECT_RAD, 200.0);
	        data.SetLong(C4dApi.PRIM_PLANE, 0);
	        data.SetBool(C4dApi.PRIM_REVERSE, false);
	        data.SetLong(C4dApi.SPLINEOBJECT_INTERPOLATION, C4dApi.SPLINEOBJECT_INTERPOLATION_ADAPTIVE);
	        data.SetLong(C4dApi.SPLINEOBJECT_SUB, 8);
	        data.SetReal(C4dApi.SPLINEOBJECT_ANGLE, 5.0 * Math.PI / 180.0);
	        data.SetReal(C4dApi.SPLINEOBJECT_MAXIMUMLENGTH, 5.0);

            return true;
        }

        static Vector3D SwapPoint(Vector3D p, int plane)
        {
	        switch (plane)
	        {
                case 1: return new Vector3D(-p.X, p.Y, p.Z);
                case 2: return new Vector3D(p.X, -p.Y, p.Z);
	        }
	        return p;
        }
 
        static Vector3D GetRTHandle(BaseObject op, int id)
        {
	        BaseContainer data = GetDataInstance(op);
	        double rad	  = data.GetReal(CIRCLEOBJECT_RAD);
	        int plane  = data.GetLong(C4dApi.PRIM_PLANE);
	        return SwapPoint(new Vector3D(rad,0.0,0.0),plane);
        }

		public override DRAWRESULT Draw(BaseObject op, DRAWPASS type, BaseDraw bd, BaseDrawHelp bh)
        {
	        if (type!=DRAWPASS.DRAWPASS_HANDLES) 
                return DRAWRESULT.DRAWRESULT_SKIP;

	        int hitid = op.GetHighlightHandle(bd);

	        Matrix4D m = bh.GetMg();

	        if (hitid==0)
                bd.SetPen(C4dApi.GetViewColor(C4dApi.VIEWCOLOR_SELECTION_PREVIEW));
	        else
                bd.SetPen(C4dApi.GetViewColor(C4dApi.VIEWCOLOR_ACTIVEPOINT));

            // Vector3D zeroPos = new Vector3D(0, 0, 0);
	        bd.SetMatrix_Matrix(op, ref m);
            bd.DrawHandle(GetRTHandle(op, 0), DRAWHANDLE.DRAWHANDLE_BIG,0);
	        bd.DrawLine(GetRTHandle(op, 0), new Vector3D(0, 0, 0),0);
	
	        return DRAWRESULT.DRAWRESULT_OK;
        }

        public override int DetectHandle(BaseObject op, BaseDraw bd, int x, int y, QUALIFIER qualifier)
        {
            if (0 != (qualifier & QUALIFIER.QUALIFIER_CTRL)) 
                return -1;
            Matrix4D mg = op.GetMg();
            if (bd.PointInRange(mg * GetRTHandle(op,0),x,y)) 
                return 0; // OK
            return -1; // Not OK
        }

        //TODO: deleted 'override'
        public bool MoveHandle(BaseObject op, BaseObject undo, ref Matrix4D tm, int hit_id, QUALIFIER qualifier)
        {
            BaseContainer src = undo.GetDataInstance();
            BaseContainer dst = op.GetDataInstance();
	
            Vector3D handle_dir = new Vector3D(1.0,0.0,0.0); 
            handle_dir = SwapPoint(handle_dir,src.GetLong(C4dApi.PRIM_PLANE));
	
            double val = Vector3D.Dot(tm.Offset, handle_dir);

            dst.SetReal(CIRCLEOBJECT_RAD, MathFunctions.Saturate(src.GetReal(CIRCLEOBJECT_RAD)+val, 0.0, C4dApi.MAXRANGE));
            return true;
        }

        public SplineObject GenerateCircle(double rad)
        {
	        const double TANG = 0.415;

	        double sn, cs;
	        int i, sub=4;

            SplineObject op = SplineObject.Alloc(sub * 2, SPLINETYPE.SPLINETYPE_BEZIER);
	        if (null == op || null == op.MakeVariableTag(C4dApi.Tsegment,2)) 
            { 
                C4dApi.blDelete_cs(op); 
                return null; 
            }
          
            op.GetDataInstance().SetBool(C4dApi.SPLINEOBJECT_CLOSED, true);

            Segment seg = new Segment();
            seg.closed = true;
            seg.cnt = sub;
            op.SetSegmentAt(0, seg);
            op.SetSegmentAt(1, seg);

            for (i = 0; i < sub; i++)
            {
                double angle = 2.0 * Math.PI * (double)i / (double)sub;
                sn = Math.Sin(angle);
                cs = Math.Cos(angle);

                Vector3D vOuter = new Vector3D(cs * rad, sn * rad, 0.0);
                Vector3D vOuterTangentL = new Vector3D(sn * rad * TANG, -cs * rad * TANG, 0.0);
                Vector3D vOuterTangentR = -vOuterTangentL;
                Vector3D vInner = vOuter * 0.5;
                Vector3D vInnerTangentL = vOuterTangentL * 0.5;
                Vector3D vInnerTangentR = -vInnerTangentL;

                op.SetPointAt(i, vOuter);
                Tangent outerTangent = new Tangent();
                outerTangent.vl = vOuterTangentL;
                outerTangent.vr = vOuterTangentR;
                op.SetTangentAt(i, outerTangent);

                op.SetPointAt(i + sub, vInner);
                Tangent innerTangent = new Tangent();
                innerTangent.vl = vInnerTangentL;
                innerTangent.vr = vInnerTangentR;
                op.SetTangentAt(i + sub, innerTangent);
            }
	        op.Message(C4dApi.MSG_UPDATE);

	        return op;
        }

        static void OrientObject(SplineObject op, int plane, bool reverse)
        {
            // Vector3D padr = op.GetPointW();
            // Tangent hadr = op.GetTangentW(), h;
            int nPoints = op.GetPointCount(); 
            int i;

            bool bTangents = op.GetTangentCount()!= 0;

            if (plane >= 1)
            {
                switch (plane)
                {
                    case 1: // ZY
                        for (i = 0; i < nPoints; i++)
                        {
                            Vector3D v  = op.GetPointAt(i);
                            op.SetPointAt(i, new Vector3D(-v.Z, v.Y, v.X));
                            if (!bTangents) continue;
                            Tangent t = op.GetTangentAt(i);
                            Tangent t2 = new Tangent();
                            t2.vl = new Vector3D(-t.vl.Z, t.vl.Y, t.vl.X);
                            t2.vr = new Vector3D(-t.vr.Z, t.vr.Y, t.vr.X);
                            op.SetTangentAt(i, t2);
                        }
                        break;

                    case 2: // XZ
                        for (i = 0; i < nPoints; i++)
                        {
                            Vector3D v = op.GetPointAt(i);
                            op.SetPointAt(i, new Vector3D(v.X, -v.Z, v.Y));
                            if (!bTangents) continue;
                            Tangent t = op.GetTangentAt(i);
                            Tangent t2 = new Tangent();
                            t2.vl = new Vector3D(t.vl.X, -t.vl.Z, t.vl.Y);
                            t2.vr = new Vector3D(t.vr.X, -t.vr.Z, t.vr.Y);
                            op.SetTangentAt(i, t2);
                        }
                        break;
                }
            }

            if (reverse)
            {
                Vector3D p;
                int to = nPoints / 2;
                if ((nPoints % 2) != 0) 
                    to++;
                for (i = 0; i < to; i++)
                {
                    p = op.GetPointAt(i); 
                    op.SetPointAt(i, op.GetPointAt(nPoints - 1 - i)); 
                    op.SetPointAt(nPoints-1-i, p);
                    if (!bTangents) 
                        continue;
                    Tangent h1 = op.GetTangentAt(i);
                    Tangent h2 = op.GetTangentAt(nPoints-1-i);
                    Tangent hTmp1 = new Tangent();
                    Tangent hTmp2 = new Tangent();
                    hTmp1.vl = new Vector3D(h2.vr);
                    hTmp1.vr = new Vector3D(h2.vl);
                    hTmp2.vl = new Vector3D(h1.vr);
                    hTmp2.vr = new Vector3D(h1.vl);
                    op.SetTangentAt(i, hTmp1);
                    op.SetTangentAt(nPoints-1-i, hTmp2);
                }
            }

            op.Message(C4dApi.MSG_UPDATE);
        }
 
        public override SplineObject GetContour(BaseObject op, BaseDocument doc, double lod, BaseThread bt)
        {
            BaseContainer bc = op.GetDataInstance();
            SplineObject bp = GenerateCircle(bc.GetReal(CIRCLEOBJECT_RAD));
            if (bp == null) 
                return null;
            BaseContainer bb = bp.GetDataInstance();

            bb.SetLong(C4dApi.SPLINEOBJECT_INTERPOLATION, bc.GetLong(C4dApi.SPLINEOBJECT_INTERPOLATION));
            bb.SetLong(C4dApi.SPLINEOBJECT_SUB, bc.GetLong(C4dApi.SPLINEOBJECT_SUB));
            bb.SetReal(C4dApi.SPLINEOBJECT_ANGLE, bc.GetReal(C4dApi.SPLINEOBJECT_ANGLE));
            bb.SetReal(C4dApi.SPLINEOBJECT_MAXIMUMLENGTH, bc.GetReal(C4dApi.SPLINEOBJECT_MAXIMUMLENGTH));

            OrientObject(bp, bc.GetLong(C4dApi.PRIM_PLANE), bc.GetBool(C4dApi.PRIM_REVERSE));

            return bp;
        }      

        /*
        public override bool GetDEnabling(GeListNode node, DescID id, GeData t_data,DESCFLAGS_ENABLE flags, BaseContainer itemdesc)      
        {
            int inter;
            BaseContainer data = ((BaseObject)node).GetDataInstance();
            switch (id[0].id)
            {
                case SPLINEOBJECT_SUB:		
	                inter=data.GetLong(SPLINEOBJECT_INTERPOLATION);
	                return inter==SPLINEOBJECT_INTERPOLATION_NATURAL || inter==SPLINEOBJECT_INTERPOLATION_UNIFORM;

                case SPLINEOBJECT_ANGLE:	
	                inter = data.GetLong(SPLINEOBJECT_INTERPOLATION);
	                return inter==SPLINEOBJECT_INTERPOLATION_ADAPTIVE || inter==SPLINEOBJECT_INTERPOLATION_SUBDIV;

                case SPLINEOBJECT_MAXIMUMLENGTH:	
	                return data.GetLong(SPLINEOBJECT_INTERPOLATION)==SPLINEOBJECT_INTERPOLATION_SUBDIV;
            }
            return true;
        }     
        */
        public override bool GetDDescription(GeListNode node, DDescriptionParams descparams)
        {
            // The main part of this code is taken from the "LookAtCamera.cpp" file from the original C4D API samples.
            // Be aware that the original LookAtCamera is not an object created from the Plugin menu but a
            // Tag type that can be added to existing objects from the "Objekte" context (right mouse button) menu under 
            // the "Cinema4dsdk Tags" sub menu.

            // TODO: whatever this might be good for: if (!singleid || cid.IsPartOf(*singleid, NULL)) // important to check for speedup c4d!
            // {


            // This will load the main object attribute tabs ("Basis", "Koord", "Objekt")
            if (!descparams.Desc.LoadDescription("obase")) 
                return false;
            
            //////////////////////////////////////////////////////////////////////////////////////
            // Create a double value named radius on the "Objekt" tab's main level
            DescID cid = new DescID(new DescLevel(CIRCLEOBJECT_RAD, C4dApi.DTYPE_LONG, 0));               // The ID of the radius value (CIRCLEOBJECT_RAD)
            BaseContainer bcRadius = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_REAL);                  // The type of the radius value (REAL)
            bcRadius.SetString(C4dApi.DESC_NAME, "Radius");                                               // The user interface name (Radius)
            bcRadius.SetLong(C4dApi.DESC_DEFAULT, 44);                                                    // The default value (44, but overridden to 200 in the Init method)
            // Create the new radius value as a child of the "Objekt" Tab (ID_OBJECTPROPERTIES)
            if (!descparams.Desc.SetParameter(cid, bcRadius, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;


            /////////////////////////////////////////////////////////////////////////////////////
            // Create an entirely new Tab (called "Ein schöner Tab")
            cid = new DescID(new DescLevel(CIRCLEOBJECT_NEWTAB, C4dApi.DTYPE_GROUP, 0));
            BaseContainer bcMaingroup = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_GROUP);
            bcMaingroup.SetString(C4dApi.DESC_NAME, "Ein schöner Tab");
            // Create the new Group on the top level (DecLevel(0))
            if (!descparams.Desc.SetParameter(cid, bcMaingroup, new DescID(new DescLevel(0)))) 
                return true;

            /////////////////////////////////////////////////////////////////////////////////////
            // Create an new sub group (called "Hübsches Grüppchen")
            cid = new DescID(new DescLevel(CIRCLEOBJECT_SUBGROUP, C4dApi.DTYPE_GROUP, 0));
            BaseContainer bcSubgroup = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_GROUP);
            bcSubgroup.SetString(C4dApi.DESC_NAME, "Hübsches Grüppchen");
            // Create the sub group on the "Ein schöner Tab" main tab (CIRCLEOBJECT_NEWTAB)
            if (!descparams.Desc.SetParameter(cid, bcSubgroup, new DescID(new DescLevel(CIRCLEOBJECT_NEWTAB))))
                return true;

            /////////////////////////////////////////////////////////////////////////////////////
            // Create an new boolean value (as a checkbox) called "Check mich"
            cid = new DescID(new DescLevel(CIRCLEOBJECT_CHECKME, C4dApi.DTYPE_BOOL, 0));
            BaseContainer bcCheckMich = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BOOL);
            bcCheckMich.SetString(C4dApi.DESC_NAME, "Check mich");
            bcCheckMich.SetBool(C4dApi.DESC_DEFAULT, true);
            // Create the boolean check box under the previously created sub group (CIRCLEOBJECT_SUBGROUP)
            if (!descparams.Desc.SetParameter(cid, bcCheckMich, new DescID(new DescLevel(CIRCLEOBJECT_SUBGROUP))))
                return true;


            descparams.Flags |= DESCFLAGS_DESC.DESCFLAGS_DESC_LOADED;
            return true; // base.GetDDescription(node, descparams);
        }
    }
}
