using C4d;
using Fusee.Math;

namespace RigPlugin
{
    class Character : ObjectDataM
    {
        public Character()
            : base(false)
        {
        }


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
        protected BaseTag wtagBaseTag;
        protected BaseDocument doc;
        protected BaseObject myMesh;
        protected ModelingCommandData mcd;
        protected BaseContainer b;
        /**
         * Anlegen eines Joint Objekts
         * @param string Name des Joint Objekts
         * @param BaseObject Parent Joint, falls dieser gekuppelt werden soll
         * @param double3 Vektor in die der Joint zeigen soll
         * @return BaseObject Joint Objekt zurückgeben
         */
        protected BaseObject allocJoint(string name, BaseObject parentJoint = null, double3 vec = new double3())
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
            double3 vecparent = mirrorJoint.GetAbsPos();
            BaseObject myJoint = this.allocJoint(name, parentJoint, new double3(vecparent.x * -1, vecparent.y, vecparent.z));
            return myJoint;
        }

        protected bool allocBonesInBones(string mainname, string bonename, BaseObject parent, double3 vec = new double3(), double3 vecMulti = new double3(), double3 vecBone = new double3(), double3 vecBoneMulti = new double3(), long counter = 0)
        {
            int KnochenLinks;
            int bone = (int)counter;

            if (bone != 0)
            {
                while (bone != 0)
                {
                    BaseObject jointBone = this.allocJoint(mainname + "_" + bone, parent, new double3(vec.x += vecMulti.x, vec.y += vecMulti.y, vec.z += vecMulti.z));
                    //BaseObject jointBone = this.allocJoint(mainname +"_"+ bone, parent, new double3(vec.x, vec.y, vec.z + bone_z));
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
                        jointBone = this.allocJoint(bonename + "_" + KnochenLinks, jointBone, new double3(vecBone.x += vecBoneMulti.x, vecBone.y += vecBoneMulti.y, vecBone.z += vecBoneMulti.z));
                        this.wtag.AddJoint(jointBone);
                        KnochenLinks--;
                    }
                    bone--;
                }
            }
            return true;
        }

        protected BaseObject[] allocBonesSwing(string mainname, BaseObject parent, long counter = 0, double3 vec = new double3())
        {
            int bone = (int)counter;
            bone -= 2;

            /* 
             * Vor Swing joint
             */
            BaseObject myJoint = this.allocJoint(mainname, parent, new double3(vec.x, 0, 0));
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
                    vec.y = 0;
                    change = true;
                }
                BaseObject jointBone = this.allocJoint(mainname + "_" + bone, parent, new double3(vec.x, vec.y, vec.z));
                this.wtag.AddJoint(jointBone);
                parent = jointBone;
                bone--;
            }

            // Nach Swing Joint
            BaseObject myJoint1 = this.allocJoint(mainname, parent, new double3(vec.x, 0, 0));
            this.wtag.AddJoint(myJoint1);
            // Array als rückgabe vorbereiten
            BaseObject[] arrJoint = { myJoint, myJoint1 };
            return arrJoint;
        }

        protected BaseObject allocBonesCurveY(string mainname, BaseObject parent, long counter = 0, double3 vec = new double3())
        {
            int bone = (int)counter;
            BaseObject myJoint = null;
            double x = vec.x;
            double y = vec.y;
            if (x < 0)
            {
                x *= -1;
            }
            while (bone > 0)
            {
                vec.y += x / bone;
                if (vec.y > x)
                {
                    vec.y = x;
                }
                myJoint = this.allocJoint(mainname, parent, vec);
                this.wtag.AddJoint(myJoint);
                vec.x += vec.y;
                if (vec.x > 0)
                {
                    vec.x = 0;
                }

                parent = myJoint;
                bone--;
            }
            return myJoint;
        }
    }
}
