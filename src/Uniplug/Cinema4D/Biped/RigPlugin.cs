using C4d;


namespace RigPlugin
{
    [ObjectPlugin(1000003,
        Name = "Biped Plugin",
        IconFile = "RigPluginLogo.tif")
    ]

    class RigPlugin : ObjectDataM
    {
        public RigPlugin()
            : base(false)
        {
        }

        protected const int SKELETT_TYPE = 10000;
        const int SKELETT_BTN_CREATE = 10001;
        private const int SKELETT_OBJ_AUSWAHL = 10015;
        private const int SKELETT_WICHTUNG_BTN = 10024;
        protected CAWeightTag wtagGlob = CAWeightTag.Alloc();
        
        public bool makeJoints(GeListNode node, DescriptionCommand desc)
        {
            CharakterHuman Main = new CharakterHuman(node, desc);
            this.wtagGlob = Main.getwtagJoint();
            return true;
        }

        public bool makeAnimalJoints(GeListNode node, DescriptionCommand desc)
        {
            CharakterAnimal Main = new CharakterAnimal(node, desc);
            this.wtagGlob = Main.getwtagJoint();
            return true;
        }

        //wird aufgerufen, wenn ein Event ausgeführt wurde
        //TODO: Override gelöscht - DS
        public bool OnDescriptionCommand(GeListNode node, DescriptionCommand desc)
        {
            long button = desc.id.GetAt(0).id;
            //BaseList2D ObjSelector = bc.GetLink(SKELETT_OBJ_AUSWAHL, doc);
            switch (button)
            {
                case SKELETT_BTN_CREATE:
                    BaseContainer b = GetDataInstance(node);
                    BaseDocument doc = C4dApi.GetActiveDocument();
                    BaseList2D ObjSelector = b.GetLink(SKELETT_OBJ_AUSWAHL, doc);
                    /*Eingeforene Selektion Auswählen vom Typ Polygon ENDE*/
                    if (ObjSelector == null)
                    {
                        C4dApi.MessageDialog("Bitte wählen Sie zuerst ein Polygonobjekt aus, bevor Sie ein Biped erstellen!");
                    }
                    else
                    {
                        long skelett = b.GetLong(SKELETT_TYPE);
                        if (skelett == 3)
                        {
                            this.makeAnimalJoints(node, desc);
                        }
                        else
                        {
                            this.makeJoints(node, desc);
                        }
                    }
                    // C4dApi.GePrint("Der button Erstellen wurde gedrück "+objSe.GetNodeID()+objSe.GetName()); 
                    //ObjSe.GetName() liefert den namen des Selektierten Objekts zurück.
                    break;
                case SKELETT_WICHTUNG_BTN:
                    /*Eingefrorene Selektion Auswählen vom Typ Polygon START*/
                    BaseContainer b2 = GetDataInstance(node);
                    BaseDocument doc2 = C4dApi.GetActiveDocument();
                    BaseObject op2 = b2.GetObjectLink(SKELETT_OBJ_AUSWAHL, doc2);
                    BaseTag t = op2.GetTag(C4dApi.Tpolygonselection);
                    //string selekname = t.GetName();
                    //BaseTag t2 = op2.GetTag(C4dApi.Tpolygonselection,1);
                    //if (op2.GetTag(C4dApi.Tpolygonselection,2)==null)
                    //{ }
                    //C4dApi.GePrint(" blubb " + t2.GetName() + " der name der vorherigen Selektion lautet: " + selekname);
                    //C4dApi.GePrint(" blubb  der name der vorherigen Selektion lautet: " + selekname);
                    //BaseObject j = this.wtagGlob.GetJoint(1, doc2);
                    //C4dApi.GePrint("Knochenname "+j.GetName());
                    //string joinName = j.GetName();
                    //if (joinName == selekname)
                    //{
                    //    C4dApi.GePrint("stimmen ueberein");
                    //}
                    //SelectionTag st = SelectionTag.Alloc(C4dApi.Tpolygonselection);
                    //BaseSelect sel = st.GetBaseSelect();
                    //sel.Select(t.GetNodeID);
                    BaseTag joints = op2.GetTag(C4dApi.Tweights);
                    //this.wtagGlob = joints.Get
                    C4dApi.GePrint("wichtung setzen bones anzahl: "+this.wtagGlob.GetJointCount());
                    //Punkte der PolygonSelektion abrufen
                    //int pt = op2.GetTagDataCount(C4dApi.Tpolygonselection);
                    //C4dApi.GePrint("Anzahl der Punkte : " + pt);
                    //doc2.SetActiveObject(b2.GetObjectLink(SKELETT_OBJ_AUSWAHL, doc2));
                    SelectionTag st = SelectionTag.Alloc(C4dApi.Tpointselection);
                    BaseSelect bs = st.GetBaseSelect();
                    
                    PointObject pObj = PointObject.GetPointObject(op2);
                    //pObj.GetPointS();
                    //for (int i = 0; i < pObj.GetPointCount(); i++)
                    //{
                        
                    //}
                    C4dApi.GePrint("test"+bs.Select(0));
                    bs.Select(0);
                    BaseSelect SelectedPoints = pObj.GetPointS();
                    //for (BaseTag pointTag = op2.GetFirstTag(); pointTag; pointTag = pointTag.GetNext())
                    //{
                    //    desc.id.GetAt(0).id = 1000;
                    //    pointTag.Message(C4dApi.MSG_DESCRIPTION_COMMAND, desc);
                    //}
                    BaseTag btA = doc2.GetActiveTag();
                    C4dApi.GePrint("TypeName : "+btA.GetTypeName() +" Name "+btA.GetName());
                    if (btA.GetType() == C4dApi.Tpolygonselection)
                    {
                        for (int j = 0; j < wtagGlob.GetJointCount(); j++)
                        {
                            BaseObject JointName = wtagGlob.GetJoint(j, doc2);
                            if (btA.GetName() == JointName.GetName())
                            {
                                for (int i = 0; i < pObj.GetPointCount(); i++)
                                {
                                    if (SelectedPoints.IsSelected(i))
                                    {
                                        this.wtagGlob.SetWeight(j, i, 0.04);
                                        C4dApi.GePrint("Der Punkt mit dem Index " + i + " ist selektiert");
                                    }
                                }
                            }
                        }
                    }
                    else { }
                    //C4dApi.GePrint("anzahl der pt " + pObj.GetPointCount());
                    //pObj.SetPointAt(1, new Vector3D(1, 2, 3));
                    //XPressoTag xt = XPressoTag.Alloc();
                    //xt = (BaseSelect)t;
                  //  this.wtagGlob.SetWeight(1, 2, 1.0/100);  //Setweeight 1.0 == 100 entspricht 0.42 == 42.0 entspricht Deshalb ist es einfacher direkt durch hundert zu teilen
                    
                    
                    
                    break;
            }
            return true;
        }

        private void setJointWeight(int tagPos, BaseObject op2,PointObject pObj)
        { 
            BaseTag t = op2.GetTag(C4dApi.Tpointselection,tagPos);
            SelectionTag st = (SelectionTag) op2.GetTag(C4dApi.Tpolygonselection);
            BaseSelect SelectedPoints = st.GetBaseSelect();
            if (t.GetName() == null)
            { }
            else
            {
                string selectioname = t.GetName();
                for (int j = 0; j < this.wtagGlob.GetJointCount(); j++)
                {
                    if (selectioname == this.wtagGlob.GetName())
                    {
                        for (int i = 0; i < pObj.GetPointCount(); i++)
                        {
                            if (SelectedPoints.IsSelected(i))
                            {
                                this.wtagGlob.SetWeight(1, i, 0.04);
                                setJointWeight(tagPos + 1, op2, pObj);
                                //C4dApi.GePrint("Der Punkt mit dem Index " + i + " ist selektiert");
                            }
                        }
                    }
                }
            }
           
        }

        //GetDDescription für das erstellen des Interface
        public override bool GetDDescription(GeListNode node, DDescriptionParams descparams)
        {
            C4DInterface C4DInter = new C4DInterface(node, descparams);
            return true;
        }
    }
}
