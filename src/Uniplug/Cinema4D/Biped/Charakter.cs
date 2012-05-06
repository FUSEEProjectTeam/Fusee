using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using C4d;
using Fusee.Math.Core;

namespace RigPlugin
{
    class Charakter : ObjectDataM
    {
        protected const int SKELETT_ARME = 10002;
        protected const int SKELETT_HAND = 10003;
        protected const int SKELETT_FINGER = 10004;
        protected const int SKELETT_BEINE = 10005;
        protected const int SKELETT_FUESSE = 10006;
        protected const int SKELETT_ZEHEN = 10007;
        protected const int SKELETT_TRENNER = 10008;
        protected const int SKELETT_OBERKOERPER_BOX = 10009;
        protected const int SKELETT_TRENNER_BUTTON = 10010;
        protected const int SKELETT_TRENNER_OBERKOERPER = 10011;
        protected const int SKELETT_UNTERKOERPER_BOX = 10012;
        protected const int SKELETT_SCHWANZ = 10013;
        protected const int CIRCLEOBJECT_RAD = 10014;
        protected const int SKELETT_OBJ_AUSWAHL = 10015;
        protected const int MCOMMAND_CURRENTSTATEOBJECT = 10017;
        protected const int MSG_DESCRIPTION_COMMAND = 10018;
        protected const int SKELETT_RUECKENWIRBEL = 10019;
        protected const int SKELETT_HALSWIRBEL = 10020;
        protected const int SKELETT_TRENNER_BEINE_WIRBEL = 10021;
        protected const int SKELETT_TRENNER_WIRBEL_SCHWANZ = 10022;
        protected const int SKELETT_TRENNER_SCHWANZ_OBJAUSWAHL = 10023;

        protected long button;
        protected long arms;
        protected long hands;
        protected long finger;
        protected long feet;
        protected long toes;
        protected long tail;
        protected long legs;

        protected CAWeightTag wtag;
        protected BaseDocument doc;
        protected BaseObject myMesh;
        protected ModelingCommandData mcd;
        protected BaseContainer b;
        /**
         * Anlegen eines Joint Objekts
         * @param string Name des Joint Objekts
         * @param BaseObject Parent Joint, falls dieser gekuppelt werden soll
         * @param Vector3D Vektor in die der Joint zeigen soll
         * @return BaseObject Joint Objekt zurückgeben
         */
        protected BaseObject allocJoint(string name, BaseObject parentJoint = null, Vector3D vec = new Vector3D())
        {
            BaseObject myJoint = BaseObject.Alloc(C4dApi.Ojoint); // Speicher reservieren für den Joint
            myJoint.SetName(name); // Joint mit einem Namen versehen
            if (parentJoint == null)
            {
                parentJoint = this.myMesh;
            }
            this.doc.InsertObject(myJoint, parentJoint, null); // Kuppeln der Joints
            myJoint.SetAbsPos(vec); // Richtung vorgeben
            return myJoint;
        }

        protected BaseObject allocMirrorJoint(string name, BaseObject parentJoint, BaseObject mirrorJoint)
        {
            Vector3D vecparent = mirrorJoint.GetAbsPos();
            BaseObject myJoint = this.allocJoint(name, parentJoint, new Vector3D(vecparent.X * -1, vecparent.Y, vecparent.Z));
            return myJoint;
        }

        protected bool allocBonesInBones(string mainname, string bonename, BaseObject parent, Vector3D vec = new Vector3D(), Vector3D vecMulti = new Vector3D(), Vector3D vecBone = new Vector3D(), Vector3D vecBoneMulti = new Vector3D(), long counter = 0)
        {
            int KnochenLinks;
            int bone = (int)counter;

            if (bone != 0)
            {
                while (bone != 0)
                {
                    BaseObject jointBone = this.allocJoint(mainname + "_" + bone, parent, new Vector3D(vec.X += vecMulti.X, vec.Y += vecMulti.Y, vec.Z += vecMulti.Z));
                    //BaseObject jointBone = this.allocJoint(mainname +"_"+ bone, parent, new Vector3D(vec.X, vec.Y, vec.Z + bone_z));
                    this.wtag.AddJoint(jointBone);

                    if (bone == 1)
                    {
                        KnochenLinks = 2;
                    }
                    else
                    {
                        KnochenLinks = 3;
                    }

                    while (KnochenLinks != 0)
                    {
                        jointBone = this.allocJoint(bonename + "_" + KnochenLinks, jointBone, new Vector3D(vecBone.X += vecBoneMulti.X, vecBone.Y += vecBoneMulti.Y, vecBone.Z += vecBoneMulti.Z));
                        this.wtag.AddJoint(jointBone);
                        KnochenLinks--;
                    }
                    bone--;
                }
            }
            return true;
        }

        protected BaseObject[] allocBonesSwing(string mainname, BaseObject parent, long counter = 0, Vector3D vec = new Vector3D())
        {
            int bone = (int)counter;
            bone -= 2;

            /* 
             * Vor Swing joint
             */
            BaseObject myJoint = this.allocJoint(mainname, parent, new Vector3D(vec.X,0,0));
            parent = myJoint;
            this.wtag.AddJoint(myJoint);

            /*
             * Swing einbauen
             */
            double temp = bone;
            bool change = false;
            while (bone > 0)
            {
                if (temp / 2 > bone && change == false)
                {
                    vec.Y = 0;
                    change = true;
                }
                BaseObject jointBone = this.allocJoint(mainname + "_" + bone, parent, new Vector3D(vec.X, vec.Y, vec.Z));
                this.wtag.AddJoint(jointBone);
                parent = jointBone;
                bone--;
            }

            // Nach Swing Joint
            BaseObject myJoint1 = this.allocJoint(mainname, parent, new Vector3D(vec.X, 0, 0));
            this.wtag.AddJoint(myJoint1);
            // Array als rückgabe vorbereiten
            BaseObject[] arrJoint = {myJoint, myJoint1};
            return arrJoint;
        }

        protected BaseObject allocBonesCurveY(string mainname, BaseObject parent, long counter = 0, Vector3D vec = new Vector3D())
        {
            int bone = (int)counter;
            BaseObject myJoint = null;
            double x = vec.X;
            double y = vec.Y;
            if (x < 0)
            {
                x *= -1;
            }
            while (bone > 0)
            {
                vec.Y += x / bone;
                if (vec.Y > x)
                {
                    vec.Y = x;
                }
                myJoint = this.allocJoint(mainname, parent, vec);
                this.wtag.AddJoint(myJoint);
                vec.X += vec.Y;
                if (vec.X > 0)
                {
                    vec.X = 0;
                }

                parent = myJoint;
                bone--;
            }
            return myJoint;
        }
    }
}
