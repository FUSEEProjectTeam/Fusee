using C4d;


namespace RigPlugin
{
    class C4DInterface : ObjectDataM
    {
        public C4DInterface()
            : base(false)
        {
        }

        private GeListNode node; // Aktuelle Scene
        private DDescriptionParams descparams; //

        /**
         * Konstanten für das Interface
         */
        private const int SKELETT_TYPE = 10000;
        private const int SKELETT_BTN_CREATE = 10001;
        private const int SKELETT_ARME = 10002;
        private const int SKELETT_HAND = 10003;
        private const int SKELETT_FINGER = 10004;
        private const int SKELETT_BEINE = 10005;
        private const int SKELETT_FUESSE = 10006;
        private const int SKELETT_ZEHEN = 10007;
        private const int SKELETT_TRENNER = 10008;
        private const int SKELETT_OBERKOERPER_BOX = 10009;
        private const int SKELETT_TRENNER_BUTTON = 10010;
        private const int SKELETT_TRENNER_OBERKOERPER = 10011;
        private const int SKELETT_UNTERKOERPER_BOX = 10012;
        private const int SKELETT_SCHWANZ = 10013;
        private const int CIRCLEOBJECT_RAD = 10014;
        private const int SKELETT_OBJ_AUSWAHL = 10015;
        private const int MCOMMAND_CURRENTSTATEOBJECT = 10017;
        private const int MSG_DESCRIPTION_COMMAND = 10018;
        private const int SKELETT_RUECKENWIRBEL = 10019;
        private const int SKELETT_HALSWIRBEL = 10020;
        private const int SKELETT_TRENNER_BEINE_WIRBEL = 10021;
        private const int SKELETT_TRENNER_WIRBEL_SCHWANZ = 10022;
        private const int SKELETT_TRENNER_SCHWANZ_OBJAUSWAHL = 10023;
        private const int SKELETT_WICHTUNG_BTN = 10024;
        /**
         * Bool
         */
        private bool Hand = false;
        private bool Bein = false;
        private bool Schwanz = false;

        public C4DInterface(GeListNode node, DDescriptionParams descparams) : base(false)
        {
            this.node = node;
            this.descparams = descparams;
            this.generate();
        }

        private bool getWeightBtn()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_WICHTUNG_BTN, C4dApi.DTYPE_BUTTON, 0));
            BaseContainer bcButton = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BUTTON);

            //Erstellt einen GUI (Button)
            bcButton.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_BUTTON);

            //Erstellt das Label für den Button
            bcButton.SetString(C4dApi.DESC_NAME, "Wichtung setzen");
            if (!descparams.Desc.SetParameter(cid, bcButton, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;
            return false;
        }

        private bool getComboBoxCharakter()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TYPE, C4dApi.DTYPE_LONG, 0));

            //Erstellt das Label vor die Combobox
            BaseContainer bcComboName = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcComboName.SetString(C4dApi.DESC_NAME, "Skelett wählen");

            //Erstellen der ComboBox mit Werten
            BaseContainer bcComboValues = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_NONE);
            bcComboValues.SetString(0, "Skelett");
            bcComboValues.SetString(1, "Männlich");
            bcComboValues.SetString(2, "Weiblich");
            bcComboValues.SetString(3, "4 Beiner");

            //Führt beide Container zusammen
            bcComboName.SetContainer(C4dApi.DESC_CYCLE, bcComboValues);

            //Erstellt die Combobox in dem Reiter Object, wegen "ID_OBJECTPROPERTIES"
            if (!descparams.Desc.SetParameter(cid, bcComboName, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSeperator()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TRENNER, C4dApi.DTYPE_SEPARATOR, 0));

            BaseContainer bcTrenner = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_SEPARATOR);
            bcTrenner.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_SEPARATOR);
            bcTrenner.SetBool(C4dApi.DESC_SEPARATORLINE, true);
            bcTrenner.SetLong(C4dApi.DESC_ANIMATE, C4dApi.DESC_ANIMATE_OFF);
            bcTrenner.SetBool(C4dApi.DESC_REMOVEABLE, false);
            bcTrenner.SetString(C4dApi.DESC_NAME, "");
            bcTrenner.SetString(C4dApi.DESC_SHORT_NAME, "");

            if (!descparams.Desc.SetParameter(cid, bcTrenner, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getTorso()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_OBERKOERPER_BOX, C4dApi.DTYPE_BOOL, 0));

            BaseContainer bcOberKoerper = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BOOL);
            bcOberKoerper.SetString(C4dApi.DESC_NAME, "Arme Aktivieren");
            bcOberKoerper.SetBool(C4dApi.DESC_DEFAULT, true);
            bcOberKoerper.SetBool(C4dApi.DESC_HIDE, false);

            // Create the boolean check box under the previously created sub group (CIRCLEOBJECT_SUBGROUP)
            if (!descparams.Desc.SetParameter(cid, bcOberKoerper, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getArms(BaseContainer dataKoerper)
        {
            //Erstellt ein weiteres Feld für die Arme
            DescID cid = new DescID(new DescLevel(SKELETT_ARME, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcArme = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_REAL);
            bcArme.SetString(C4dApi.DESC_NAME, "Arme");
            //Definiert den minimalen Wert
            bcArme.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcArme.SetLong(C4dApi.DESC_MAX, 2);

            bool temp = dataKoerper.GetBool(SKELETT_OBERKOERPER_BOX);

            if (dataKoerper.GetLong(SKELETT_TYPE) == 3)
            {
                temp = false;
                bcArme.SetLong(C4dApi.DESC_DEFAULT, 0);
            }

            if (temp == true)
            {
                bcArme.SetBool(C4dApi.DESC_HIDE, false);
                Hand = true;
            }
            else
            {
                bcArme.SetBool(C4dApi.DESC_HIDE, true);
                Hand = false;
            }

            //bcArme.SetBool(C4dApi.DESC_HIDE, true);
            if (!descparams.Desc.SetParameter(cid, bcArme, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getHands(BaseContainer dataKoerper)
        {
            //Erstellt ein weiteres Feld für die Hände
            DescID cid = new DescID(new DescLevel(SKELETT_HAND, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcHand = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcHand.SetString(C4dApi.DESC_NAME, "Hände");
            //Definiert den minimalen Wert
            bcHand.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert

            if (Hand == true)
            {

                //C4dApi.GePrint("anzahl Arme = " + dataKoerper.GetLong(SKELETT_ARME));
                if (dataKoerper.GetLong(SKELETT_ARME) > 0)
                {
                    bcHand.SetLong(C4dApi.DESC_MAX, dataKoerper.GetLong(SKELETT_ARME));
                    bcHand.SetBool(C4dApi.DESC_HIDE, false);
                }
                else
                {
                    bcHand.SetBool(C4dApi.DESC_HIDE, true);
                }

            }
            else
            {
                bcHand.SetBool(C4dApi.DESC_HIDE, true);

            }

            if (!descparams.Desc.SetParameter(cid, bcHand, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getFingers(BaseContainer dataKoerper)
        {
            //Erstellt ein weiteres Feld für die Finger
            DescID cid = new DescID(new DescLevel(SKELETT_FINGER, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcFinger = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcFinger.SetString(C4dApi.DESC_NAME, "Finger");
            //Definiert den minimalen Wert
            bcFinger.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcFinger.SetLong(C4dApi.DESC_MAX, 5);
            if (Hand == true)
            {
                if (dataKoerper.GetLong(SKELETT_HAND) > 0)
                {
                    bcFinger.SetBool(C4dApi.DESC_HIDE, false);
                }
                else
                {
                    bcFinger.SetBool(C4dApi.DESC_HIDE, true);
                }
            }
            else
            {
                bcFinger.SetBool(C4dApi.DESC_HIDE, true);

            }
            if (!descparams.Desc.SetParameter(cid, bcFinger, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSeperatorTorso()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TRENNER_OBERKOERPER, C4dApi.DTYPE_SEPARATOR, 0));

            BaseContainer bcTrennerOberKoerper = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_SEPARATOR);
            bcTrennerOberKoerper.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_SEPARATOR);
            bcTrennerOberKoerper.SetBool(C4dApi.DESC_SEPARATORLINE, true);
            bcTrennerOberKoerper.SetLong(C4dApi.DESC_ANIMATE, C4dApi.DESC_ANIMATE_OFF);
            bcTrennerOberKoerper.SetBool(C4dApi.DESC_REMOVEABLE, false);
            bcTrennerOberKoerper.SetString(C4dApi.DESC_NAME, "");
            bcTrennerOberKoerper.SetString(C4dApi.DESC_SHORT_NAME, "");

            if (!descparams.Desc.SetParameter(cid, bcTrennerOberKoerper, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getLegs(BaseContainer dataKoerper)
        {
            DescID cid = new DescID(new DescLevel(SKELETT_UNTERKOERPER_BOX, C4dApi.DTYPE_BOOL, 0));
            BaseContainer bcUnterKoerper = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BOOL);
            bcUnterKoerper.SetString(C4dApi.DESC_NAME, "Beine Aktivieren");
            bcUnterKoerper.SetBool(C4dApi.DESC_DEFAULT, true);
            // Create the boolean check box under the previously created sub group (CIRCLEOBJECT_SUBGROUP)
            if (!descparams.Desc.SetParameter(cid, bcUnterKoerper, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            BaseContainer dataUnterKoerper = GetDataInstance(node);

            //Erstellt ein weiteres Feld für die Beine
            cid = new DescID(new DescLevel(SKELETT_BEINE, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcBeine = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcBeine.SetString(C4dApi.DESC_NAME, "Beine");
            //Definiert den minimalen Wert
            bcBeine.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert


            bool temp2 = dataKoerper.GetBool(SKELETT_UNTERKOERPER_BOX);
            if (temp2 == true && dataKoerper.GetLong(SKELETT_TYPE) == 3)//Schaltung zwischen 4 Beiner und Menschen Ist der Type ein 4 Beiner so ist die Maximal zahl der Beine 4
            {
                bcBeine.SetLong(C4dApi.DESC_MAX, 4);
                bcBeine.SetBool(C4dApi.DESC_HIDE, false);
                Bein = true;
                Schwanz = true;
            }
            else if (temp2 == true && dataKoerper.GetLong(SKELETT_TYPE) != 3)//Schaltung zwischen 4 Beiner und Menschen Ist der Type ein Mensch so ist die Maximal zahl der Beine 2
            {
                bcBeine.SetLong(C4dApi.DESC_MAX, 2);
                bcBeine.SetBool(C4dApi.DESC_HIDE, false);
                Bein = true;
            }
            else
            {
                bcBeine.SetBool(C4dApi.DESC_HIDE, true);
                Bein = false;
            }

            if (!descparams.Desc.SetParameter(cid, bcBeine, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getFeets(BaseContainer dataKoerper)
        {
            //Erstellt ein weiteres Feld für die Füße
            DescID cid = new DescID(new DescLevel(SKELETT_FUESSE, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcFuesse = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);

            bcFuesse.SetString(C4dApi.DESC_NAME, "Füße");
            //Definiert den minimalen Wert
            bcFuesse.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcFuesse.SetLong(C4dApi.DESC_MAX, 2);
            if (Bein == true && Schwanz == false)
            {
                if (dataKoerper.GetLong(SKELETT_BEINE) > 0)
                {
                    bcFuesse.SetLong(C4dApi.DESC_MAX, 2);
                    bcFuesse.SetBool(C4dApi.DESC_HIDE, false);
                }
                else
                {
                    bcFuesse.SetBool(C4dApi.DESC_HIDE, true);
                }
            }
            else if (Bein == true && Schwanz == true)
            {
                if (dataKoerper.GetLong(SKELETT_BEINE) > 0)
                {
                    bcFuesse.SetLong(C4dApi.DESC_MAX, 4);
                    bcFuesse.SetBool(C4dApi.DESC_HIDE, false);
                }
                else
                {
                    bcFuesse.SetBool(C4dApi.DESC_HIDE, true);
                }
            }
            else
            {
                bcFuesse.SetBool(C4dApi.DESC_HIDE, true);

            }

            if (!descparams.Desc.SetParameter(cid, bcFuesse, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getToes(BaseContainer dataKoerper)
        {
            //Erstellt ein weiteres Feld für die Zehen
            DescID cid = new DescID(new DescLevel(SKELETT_ZEHEN, C4dApi.DTYPE_LONG, 0));
            BaseContainer bcZehen = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcZehen.SetString(C4dApi.DESC_NAME, "Zehen");
            //Definiert den minimalen Wert
            bcZehen.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcZehen.SetLong(C4dApi.DESC_MAX, 5);
            BaseContainer dataUnterKoerper = GetDataInstance(node);
            if (Bein == true && dataUnterKoerper.GetLong(SKELETT_TYPE) != 3)//Überprüfung auf 4 Beiner, ist es kein 4 Beiner so wird das Feld Zehen eingeblendet
            {
                if (dataKoerper.GetLong(SKELETT_FUESSE) > 0)
                {
                    bcZehen.SetBool(C4dApi.DESC_HIDE, false);
                }
                else
                {
                    bcZehen.SetBool(C4dApi.DESC_HIDE, true);
                }
            }
            else
            {
                bcZehen.SetBool(C4dApi.DESC_HIDE, true);
            }

            if (!descparams.Desc.SetParameter(cid, bcZehen, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSpineLink()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_RUECKENWIRBEL, C4dApi.DTYPE_LONG, 0));

            BaseContainer bcRueckenWirbel = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcRueckenWirbel.SetString(C4dApi.DESC_NAME, "Rückenwirbel Knochen");
            //Definiert den minimalen Wert
            bcRueckenWirbel.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcRueckenWirbel.SetLong(C4dApi.DESC_MAX, 30);
            if (Schwanz == true)
            {
                bcRueckenWirbel.SetBool(C4dApi.DESC_HIDE, false);
                //Schwanz = false;
            }
            else
            {
                bcRueckenWirbel.SetBool(C4dApi.DESC_HIDE, true);
            }


            if (!descparams.Desc.SetParameter(cid, bcRueckenWirbel, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getNeckLink()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_HALSWIRBEL, C4dApi.DTYPE_LONG, 0));

            BaseContainer bcHalsWirbel = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcHalsWirbel.SetString(C4dApi.DESC_NAME, "Halswirbel Knochen");
            //Definiert den minimalen Wert
            bcHalsWirbel.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcHalsWirbel.SetLong(C4dApi.DESC_MAX, 15);
            if (Schwanz == true)
            {
                bcHalsWirbel.SetBool(C4dApi.DESC_HIDE, false);
                //Schwanz = false;
            }
            else
            {
                bcHalsWirbel.SetBool(C4dApi.DESC_HIDE, true);
            }


            if (!descparams.Desc.SetParameter(cid, bcHalsWirbel, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getTail()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_SCHWANZ, C4dApi.DTYPE_LONG, 0));

            BaseContainer bcSchwanz = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
            bcSchwanz.SetString(C4dApi.DESC_NAME, "Schwanz Knochen");
            //Definiert den minimalen Wert
            bcSchwanz.SetLong(C4dApi.DESC_MIN, 0);
            //Definiert den maximalen Wert
            bcSchwanz.SetLong(C4dApi.DESC_MAX, 5);
            if (Schwanz == true)
            {
                bcSchwanz.SetBool(C4dApi.DESC_HIDE, false);
                Schwanz = false;
            }
            else
            {
                bcSchwanz.SetBool(C4dApi.DESC_HIDE, true);
            }


            if (!descparams.Desc.SetParameter(cid, bcSchwanz, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSeperatorFeetSpine()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TRENNER_BEINE_WIRBEL, C4dApi.DTYPE_SEPARATOR, 0));

            BaseContainer bcTrennerBeineWirbel = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_SEPARATOR);
            bcTrennerBeineWirbel.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_SEPARATOR);
            bcTrennerBeineWirbel.SetBool(C4dApi.DESC_SEPARATORLINE, true);
            bcTrennerBeineWirbel.SetLong(C4dApi.DESC_ANIMATE, C4dApi.DESC_ANIMATE_OFF);
            bcTrennerBeineWirbel.SetBool(C4dApi.DESC_REMOVEABLE, false);
            bcTrennerBeineWirbel.SetString(C4dApi.DESC_NAME, "");
            bcTrennerBeineWirbel.SetString(C4dApi.DESC_SHORT_NAME, "");

            if (!descparams.Desc.SetParameter(cid, bcTrennerBeineWirbel, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSeperatorSpineTail()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TRENNER_WIRBEL_SCHWANZ, C4dApi.DTYPE_SEPARATOR, 0));

            BaseContainer bcTrennerWirbelSchwanz = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_SEPARATOR);
            bcTrennerWirbelSchwanz.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_SEPARATOR);
            bcTrennerWirbelSchwanz.SetBool(C4dApi.DESC_SEPARATORLINE, true);
            bcTrennerWirbelSchwanz.SetLong(C4dApi.DESC_ANIMATE, C4dApi.DESC_ANIMATE_OFF);
            bcTrennerWirbelSchwanz.SetBool(C4dApi.DESC_REMOVEABLE, false);
            bcTrennerWirbelSchwanz.SetString(C4dApi.DESC_NAME, "");
            bcTrennerWirbelSchwanz.SetString(C4dApi.DESC_SHORT_NAME, "");

            if (!descparams.Desc.SetParameter(cid, bcTrennerWirbelSchwanz, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool getSeperatorTailSelector()
        {
            DescID cid = new DescID(new DescLevel(SKELETT_TRENNER_SCHWANZ_OBJAUSWAHL, C4dApi.DTYPE_SEPARATOR, 0));

            BaseContainer bcTrennerSchwanzAuswahl = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_SEPARATOR);
            bcTrennerSchwanzAuswahl.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_SEPARATOR);
            bcTrennerSchwanzAuswahl.SetBool(C4dApi.DESC_SEPARATORLINE, true);
            bcTrennerSchwanzAuswahl.SetLong(C4dApi.DESC_ANIMATE, C4dApi.DESC_ANIMATE_OFF);
            bcTrennerSchwanzAuswahl.SetBool(C4dApi.DESC_REMOVEABLE, false);
            bcTrennerSchwanzAuswahl.SetString(C4dApi.DESC_NAME, "");
            bcTrennerSchwanzAuswahl.SetString(C4dApi.DESC_SHORT_NAME, "");

            if (!descparams.Desc.SetParameter(cid, bcTrennerSchwanzAuswahl, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;

            return false;
        }

        private bool generate()
        {
            // Diese Abfrage lädt die den Reiter Objekt
            if (!descparams.Desc.LoadDescription("obase"))
                return false;

            //Erstellen der ComboBox zur auswahl des Skelettes
            if (this.getComboBoxCharakter() == true)
            {
                return false;
            }

            //Erstellen der Trennlinie
            if (this.getSeperator() == true)
            {
                return false;
            }

            BaseContainer dataKoerper = GetDataInstance(node); //Speichert alle elemente von der Liste in den Container
            if (dataKoerper.GetLong(SKELETT_TYPE) != 3)//Ist die ID 3 so wurde ein 4 Beiner ausgewählt, da diese keine Arme haben wird der Komplette Block für die Arme im Intrface ausgeblendet
            {
                if (this.getTorso() == true)
                {
                    return false;
                }
            }
            //Abfragen ob ein Oberkörper erstellt werden soll

            //DescID singleid = descparams.Desc.GetSingleDescID();
            //data = ((BaseList2D)node).GetDataInstance();

            // Arme erstellen
            if (this.getArms(dataKoerper) == true)
            {
                return false;
            }

            // Hände erstellen
            if (this.getHands(dataKoerper) == true)
            {
                return false;
            }

            // Finger erstellen
            if (this.getFingers(dataKoerper) == true)
            {
                return false;
            }

            if (this.getSeperatorTorso() == true)
            {
                return false;
            }

            if (this.getLegs(dataKoerper) == true)
            {
                return false;
            }

            if (this.getFeets(dataKoerper) == true)
            {
                return false;
            }

            if (this.getToes(dataKoerper) == true)
            {
                return false;
            }

            if (this.getSeperatorFeetSpine() == true)
            {
                return false;
            }

            if (this.getSpineLink() == true)
            {
                return false;
            }

            if (this.getNeckLink() == true)
            {
                return false;
            }

            if (this.getSeperatorSpineTail() == true)
            {
                return false;
            }

            if (this.getTail() == true)
            {
                return false;
            }

            if (this.getSeperatorTailSelector() == true)
            {
                return false;
            }


            //Objektauswahlfeld erstellen
            DescID cid = new DescID(new DescLevel(SKELETT_OBJ_AUSWAHL, C4dApi.DTYPE_BASELISTLINK, 0));
            BaseDocument doc = C4dApi.GetActiveDocument(); // alle vorhandenen Objekte im Dokument in die Variable doc Speicerhn          
            BaseContainer objSel = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BASELISTLINK); // Basecontainer vom Typ BaseLink da wird Objekte verlinken , referenzieren
            objSel.SetLink(SKELETT_OBJ_AUSWAHL, doc); //SelectFeld erstellen in dem ein Objekct gebunden werden kann, ist nötig damit später knochen dem Modell zugewiesen werden können!
            objSel.SetString(C4dApi.DESC_NAME, "Polygonobjekt Wählen");
            if (!descparams.Desc.SetParameter(cid, objSel, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return true;
            //Erstellt eine Schaltfläsche
            cid = new DescID(new DescLevel(SKELETT_BTN_CREATE, C4dApi.DTYPE_BUTTON, 0));
            BaseContainer bcButton = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_BUTTON);

            //Erstellt einen GUI (Button)
            bcButton.SetLong(C4dApi.DESC_CUSTOMGUI, C4dApi.DTYPE_BUTTON);

            //Erstellt das Label für den Button
            bcButton.SetString(C4dApi.DESC_NAME, "Erstellen");
            if (!descparams.Desc.SetParameter(cid, bcButton, new DescID(new DescLevel(C4dApi.ID_OBJECTPROPERTIES))))
                return false;

            if (this.getWeightBtn() == true)
            {
                return false;
            }

            descparams.Flags |= DESCFLAGS_DESC.DESCFLAGS_DESC_LOADED;

            return true;
        }
    }
}
