using C4d;
using Fusee.Math;

namespace RigPlugin
{
    class CharakterHuman : Character
    {
        private const int ID_CA_SKIN_OBJECT_INCLUDE = 0;
        public CharakterHuman(){}
        public CharakterHuman(GeListNode node, DescriptionCommand desc){
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
            //TODO - Geaendert von DS
            //this.wtagBaseTag = BaseTag.Alloc(C4dApi.Tweights);
            /**
             * Wichtungstag in das BaseObject einbauen
             */
            this.myMesh.InsertTag(this.wtag);

            /**
             * Standard BaseObject vom Typ Joint erstellen, diese sind bei Menschen immer aktiv
             */
            BaseObject jointSpine = this.allocJoint("Spine");
            BaseObject jointAbs = this.allocJoint("Abs", jointSpine, new double3(0, 100, 0));
            BaseObject jointChest = this.allocJoint("Chest", jointAbs, new double3(0, 50, 0));
            BaseObject jointNeck = this.allocJoint("Neck", jointChest, new double3(0, 25, 0));
            BaseObject jointHead = this.allocJoint("Head", jointNeck, new double3(0, 50, 0));
            BaseObject jointL_Hip = this.allocJoint("L_Hip", jointSpine, new double3(25, 0, 0));
            BaseObject jointL_Shoulder = this.allocJoint("L_Shoulder", jointChest, new double3(25, 0, 0));
            
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
             * Skin Objekt einfügen
             */
            
            BaseObject skin = BaseObject.Alloc(C4dApi.Oskin);
            if (skin != null)
            {
                
                skin.InsertUnder(this.myMesh);
                skin.SetEditorMode(C4dApi.MODE_ON);
                
               //BaseContainer bcc = skin.GetDataInstance();
               //InExcludeData list=new InExcludeData(0);
               //BaseList2D b2d = bcc.GetLink(SKELETT_OBJ_AUSWAHL, doc);
               //list.InsertObject(b2d,0);
                //  InExcludeData list = (InExcludeData)bcc.GetCustomDataType(ID_CA_SKIN_OBJECT_INCLUDE, C4dApi.CUSTOMDATATYPE_INEXCLUDE_LIST);
          //     list.InsertObject(this.myMesh, 0);
                //bcc.SetLong(ID_CA_SKIN_OBJECT_INCEXC, 1);
               //bcc.GetCustomDataType(ID_CA_SKIN_OBJECT_INCEXC_INCLUDE, C4dApi.CUSTOMDATATYPE_INEXCLUDE_LIST);
               
               // InExcludeData list = (InExcludeData) 
                //bcc.SetData(ID_CA_SKIN_OBJECT_INCEXC,true);
                //bcc.GetCustomDataType(ID_CA_SKIN_OBJECT_INCLUDE, C4dApi.CUSTOMDATATYPE_INEXCLUDE_LIST);
                //int t = bcc.GetId();
                //C4dApi.GePrint("list " + t);
               // list.InsertObject(this.myMesh, 0);
                //C4dApi.GePrint("list OBJ " + list.GetObjectCount());
                
                /*c++ Variante 
                   BaseObject *skin;    // a pointer to a skin object

                    BaseContainer *bc = skin->GetDataInstance();

                    InExcludeData *list = (InExcludeData*)bc->GetCustomDataType(ID_CA_SKIN_OBJECT_INCLUDE, CUSTOMDATATYPE_INEXCLUDE_LIST);

                    BaseObject *mesh;   // the object you want to insert into the list

                    list->InsertObject(mesh, 0);

                 */

                //InExcludeData iex = autoAlloc();
               // InExcludeCustomGui iex = CustomDataType
             
                //iex.InsertObject(b.GetLink(SKELETT_OBJ_AUSWAHL,this.doc),1);
            }
            
            /**s
             * Zur Szene hinzufügen
             */
            
            //rem.GetName();
            
            //rem.Remove();
            C4dApi.EventAdd();
            //C4dApi.GetObjectName(1000023); // gibt "Biped Plugin" aus
            //C4dApi.GePrint("anzal der bones: "+this.wtag.GetJointCount());
            
            doc.SetActiveObject(this.myMesh);//Selektiert das Model im OM
            
            //BaseList2D te = this.b.GetLink(1000023, this.doc);
            //te.Remove();
            
           
            
        }

        public CAWeightTag getwtagJoint()
        {
            return this.wtag;
        }
        private void AddArms(BaseObject parentJoint, BaseObject mirrorJoint = null)
        {
            this.arms = this.b.GetLong(SKELETT_ARME);
            this.hands = this.b.GetLong(SKELETT_HAND);
            this.finger = this.b.GetLong(SKELETT_FINGER);

            if (this.arms > 0)
            {
                BaseObject jointL_Upperam = this.allocJoint("L_Upperarm", parentJoint, new double3(75, 0, 0));
                BaseObject jointL_Forearm = this.allocJoint("L_Forearm", jointL_Upperam, new double3(75, 0, 0));
                BaseObject jointL_Hand = null;

                this.wtag.AddJoint(jointL_Upperam);
                this.wtag.AddJoint(jointL_Forearm);

                if (this.hands > 0)
                {
                    jointL_Hand = this.allocJoint("L_Hand", jointL_Forearm, new double3(25, 0, 0));

                    if (this.finger > 0)
                    {
                        this.allocBonesInBones("L_Finger", "L_FingerBone", jointL_Hand, new double3(25, 0, 10), new double3(0, 0, -5), new double3(5, 0, 0), new double3(0, 0, 0), this.finger);
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
                            this.allocBonesInBones("R_Finger", "R_FingerBone", jointR_Hand, new double3(-25, 0, 10), new double3(0, 0, -5), new double3(-5, 0, 0), new double3(0, 0, 0), this.finger);
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
                BaseObject jointL_Leg = this.allocJoint("L_Leg", parentJoint, new double3(0, -100, 0));
                BaseObject jointL_Cnemials = this.allocJoint("L_Cnemials", jointL_Leg, new double3(0, -100, 0));
                BaseObject jointL_Foot = null;

                this.wtag.AddJoint(jointL_Leg);
                this.wtag.AddJoint(jointL_Cnemials);

                if (this.feet > 0)
                {
                    jointL_Foot = this.allocJoint("L_Foot", jointL_Cnemials, new double3(0, 0, -50));

                    if (this.toes > 0)
                    {
                        this.allocBonesInBones("L_Feet", "L_FeetBone", jointL_Foot, new double3(15, 0, -25), new double3(-5, 0, 0), new double3(0, 0, -5), new double3(0, 0, 0), this.toes);
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
                            this.allocBonesInBones("R_Feet", "R_FeetBone", jointR_Foot, new double3(15, 0, -25), new double3(-5, 0, 0), new double3(0, 0, -5), new double3(0, 0, 0), this.toes);
                        }
                    }
                }
            }
        }
    }
}
