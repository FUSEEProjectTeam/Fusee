using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using C4d;
using Fusee.Math.Core;

namespace RigPlugin
{
    class Human : Charakter
    {
        public Human(GeListNode node, DescriptionCommand desc){
            this.b = GetDataInstance(node);
            this.button = desc.id.GetAt(0).id;
            this.doc = C4dApi.GetActiveDocument();//Speichern aller Elemente im Dokument in die Variable doc
            this.myMesh = b.GetObjectLink(SKELETT_OBJ_AUSWAHL, doc); //selektiertes Object in die Variable ObjSe speichern
            this.mcd = new ModelingCommandData(this.doc, this.myMesh, this.myMesh.GetDataInstance(), MODELINGCOMMANDMODE.MODELINGCOMMANDMODE_ALL, MODELINGCOMMANDFLAGS.MODELINGCOMMANDFLAGS_CREATEUNDO, null, null);

            /**
             * Bisheriges speichern und in die Szene zeichnen
             */
            C4dApi.EventAdd();

            /**
             * Wichtungstag definieren
             */
            this.wtag = CAWeightTag.Alloc();
            /**
             * Wichtungstag in das BaseObject einbauen
             */
            this.myMesh.InsertTag(this.wtag);

            /**
             * Standard BaseObject vom Typ Joint erstellen, diese sind bei Menschen immer aktiv
             */
            BaseObject jointSpine = this.allocJoint("Spine");
            BaseObject jointAbs = this.allocJoint("Abs", jointSpine, new Vector3D(0, 200, 0));
            BaseObject jointChest = this.allocJoint("Chest", jointAbs, new Vector3D(0, 50, 0));
            BaseObject jointNeck = this.allocJoint("Neck", jointChest, new Vector3D(0, 25, 0));
            BaseObject jointHead = this.allocJoint("Head", jointNeck, new Vector3D(0, 50, 0));
            BaseObject jointL_Hip = this.allocJoint("L_Hip", jointSpine, new Vector3D(25, 0, 0));
            BaseObject jointL_Shoulder = this.allocJoint("L_Shoulder", jointChest, new Vector3D(25, 0, 0));
            
            /**
             * Spiegelung
             */
            BaseObject jointR_Hip = this.allocMirrorJoint("R_Hip", jointSpine, jointL_Hip);
            BaseObject jointR_Shoulder = this.allocMirrorJoint("R_Shoulder", jointChest, jointL_Shoulder);


            /***********
             * Übergabe an den Wichtungstag
             ***********/

            /**
             * Arme hinzufügen
             */
            this.AddArms(jointL_Shoulder, jointR_Shoulder);
            
            /**
             * Füße hinzufügen
             */
            this.AddFeet(jointL_Hip, jointR_Hip);
            /**
             * Standard
             */
            this.wtag.AddJoint(jointSpine);
            this.wtag.AddJoint(jointAbs);
            this.wtag.AddJoint(jointChest);
            this.wtag.AddJoint(jointNeck);
            this.wtag.AddJoint(jointHead);
            // Linke seite vom Körper
            this.wtag.AddJoint(jointL_Hip);
            this.wtag.AddJoint(jointL_Shoulder);
            // Rechte seite vom Körper
            this.wtag.AddJoint(jointR_Hip);
            this.wtag.AddJoint(jointR_Shoulder);

            /**
             * Debug Ausgabe erstellen
             */
            C4dApi.GePrint("Der Joint wurde angelegt");
            jointSpine.GetTag(C4dApi.Tweights);

            /**
             * Zur Szene hinzufügen
             */
            C4dApi.EventAdd();
        }

        private void AddArms(BaseObject parentJoint, BaseObject mirrorJoint = null)
        {
            this.arms = this.b.GetLong(SKELETT_ARME);
            this.hands = this.b.GetLong(SKELETT_HAND);
            this.finger = this.b.GetLong(SKELETT_FINGER);

            if (this.arms > 0)
            {
                BaseObject jointL_Upperam = this.allocJoint("L_Upperarm", parentJoint, new Vector3D(75, 0, 0));
                BaseObject jointL_Forearm = this.allocJoint("L_Forearm", jointL_Upperam, new Vector3D(75, 0, 0));
                BaseObject jointL_Hand = null;

                this.wtag.AddJoint(jointL_Upperam);
                this.wtag.AddJoint(jointL_Forearm);

                if (this.hands > 0)
                {
                    jointL_Hand = this.allocJoint("L_Hand", jointL_Forearm, new Vector3D(25, 0, 0));

                    if (this.finger > 0)
                    {
                        this.allocBonesInBones("L_Finger", "L_FingerBone", jointL_Hand, new Vector3D(25, 0, 10), new Vector3D(0, 0, -5), new Vector3D(5, 0, 0), new Vector3D(0, 0, 0), this.finger);
                    }
                    this.wtag.AddJoint(jointL_Hand);
                }

                if (this.arms == 2)
                {
                    BaseObject jointR_Upperarm = this.allocMirrorJoint("R_Upperarm", mirrorJoint, jointL_Upperam);
                    BaseObject jointR_Forearm = this.allocMirrorJoint("R_Forearm", jointR_Upperarm, jointL_Forearm);

                    this.wtag.AddJoint(jointR_Upperarm);
                    this.wtag.AddJoint(jointR_Forearm);

                    if (this.hands == 2)
                    {
                        BaseObject jointR_Hand = this.allocMirrorJoint("R_Hand", jointR_Forearm, jointL_Hand);
                        this.wtag.AddJoint(jointR_Hand);

                        if (this.finger > 0)
                        {
                            this.allocBonesInBones("R_Finger", "R_FingerBone", jointR_Hand, new Vector3D(-25, 0, 10), new Vector3D(0, 0, -5), new Vector3D(-5, 0, 0), new Vector3D(0, 0, 0), this.finger);
                        }
                    }
                }
            }
        }

        private void AddFeet(BaseObject parentJoint, BaseObject mirrorJoint = null)
        {
            this.feet = this.b.GetLong(SKELETT_FUESSE);
            this.toes = this.b.GetLong(SKELETT_ZEHEN);
            this.legs = this.b.GetLong(SKELETT_BEINE);
            //this.tail = this.b.GetLong(SKELETT_SCHWANZ); (Animal Klasse)

            if (this.legs > 0)
            {
                BaseObject jointL_Leg = this.allocJoint("L_Leg", parentJoint, new Vector3D(0, -100, 0));
                BaseObject jointL_Cnemials = this.allocJoint("L_Cnemials", jointL_Leg, new Vector3D(0, -100, 0));
                BaseObject jointL_Foot = null;

                this.wtag.AddJoint(jointL_Leg);
                this.wtag.AddJoint(jointL_Cnemials);

                if (this.feet > 0)
                {
                    jointL_Foot = this.allocJoint("L_Foot", jointL_Cnemials, new Vector3D(0, 0, -50));

                    if (this.toes > 0)
                    {
                        this.allocBonesInBones("L_Feet", "L_FeetBone", jointL_Foot, new Vector3D(15, 0, -25), new Vector3D(-5, 0, 0), new Vector3D(0, 0, -5), new Vector3D(0, 0, 0), this.toes);
                    }
                    this.wtag.AddJoint(jointL_Foot);
                }

                if (this.legs == 2)
                {
                    BaseObject jointR_Leg = this.allocMirrorJoint("R_Leg", mirrorJoint, jointL_Leg);
                    BaseObject jointR_Cnemials = this.allocMirrorJoint("R_Cnemials", jointR_Leg, jointL_Cnemials);
                    BaseObject jointR_Foot = null;
                    
                    this.wtag.AddJoint(jointR_Leg);
                    this.wtag.AddJoint(jointR_Cnemials);

                    if (this.feet == 2)
                    {
                        jointR_Foot = this.allocMirrorJoint("R_Foot", jointR_Cnemials, jointL_Foot);
                        this.wtag.AddJoint(jointR_Foot);

                        if (this.toes > 0)
                        {
                            this.allocBonesInBones("R_Feet", "R_FeetBone", jointR_Foot, new Vector3D(15, 0, -25), new Vector3D(-5, 0, 0), new Vector3D(0, 0, -5), new Vector3D(0, 0, 0), this.toes);
                        }
                    }
                }
            }
        }
    }
}
