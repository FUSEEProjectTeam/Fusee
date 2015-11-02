using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;
using ProtoBuf;

namespace FuExport
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class PropMemberAttribute : Attribute
    {
       public int ID;
       public PropMemberAttribute(int id)
       {
           ID = id;
       }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class PropContractAttribute : Attribute
    {
        public PropContractAttribute()
        {
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class TagTypeAttribute : Attribute
    {
        public int ID;
    public TagTypeAttribute(int id)
    {
        ID = id;
    }
}

    [PropContract]
    public class SampleData
    {
        [PropMember(1)]
        public int SomeInt;

        [PropMember(2)]
        public string SomeString;
    }


    // [TagType(1036156)]
    public class ManagedTagBase<TData> : BaseTag where TData : class, new()
    {
        public ManagedTagBase(IntPtr cPtr, bool memOwn) : base(cPtr, memOwn)
        {
            
        }

        public TData Data
        {
            get
            {
                if (GetTypeC4D() != TagID)
                    return null;

                var ret = new TData();
                return ret;
            }
            set
            {
                
            }
        }

        public int TagID
        {
            get
            {
                Type thisType = GetType();
                TagTypeAttribute attr = (TagTypeAttribute)Attribute.GetCustomAttribute(thisType, typeof(TagPluginAttribute), true);
                if (attr != null)
                    return attr.ID;
                return -1;
            }
        }
    }


    // Your Plugin Number for "FUSEE General Tag" is: 1036156 
    // [TagPlugin(1036156, "General FUSEE export settings", "FuExportLogo.tif", Visible = true)]
    public class ManagedTagPluginBase<TData> : TagDataM
    {
        public int TheInt { get; set; }
        public string TheName { get; set; }

        private static readonly int GENERAL_TAG_THEINT  = 10000;           // values 1000.3000 already reserved from includes
        private static readonly int GENERAL_TAG_THENAME = 10001;


        public string TagName
        {
            get
            {
                Type thisType = GetType();
                TagPluginAttribute attr = (TagPluginAttribute)Attribute.GetCustomAttribute(thisType, typeof(TagPluginAttribute), true);
                if (attr != null && !string.IsNullOrEmpty(attr.Name))
                    return attr.Name;
                return GetType().Name;
            }
        }

        public ManagedTagPluginBase() : base(false)
        {
            TheInt = 42;
            TheName = "Horst";
        }

        public override bool Init(GeListNode node)
        {
            BaseContainer data = GetDataInstance(node);
            data.SetInt32(GENERAL_TAG_THEINT, TheInt);
            data.SetString(GENERAL_TAG_THENAME, TheName);

            return true;
        }

        // Different ways how data (The...) is obtained and written.
        // Example 1: An override gets the baseTag/BaseObject:
        // 
        // Workgin example from DoubleCircle:
        // public override retval OverrideMethod(..., BaseTag tag, ...)
        // {
        //    BaseContainer di = tag.GetDataInstance();                       // Get the data instance from the object
        //    int theIntContents = di.GetInt(GENERAL_TAG_THEINT);             // Retrieve the data from the data instance
   
        //  Question: Would it be sufficient to simply use this.GetDataInstance()



        public override bool GetDDescription(GeListNode node, DDescriptionParams descparams)
        {
            // The main part of this code is taken from the "LookAtCamera.cpp" file from the original C4D API samples.

            // TODO: whatever this might be good for: if (!singleid || cid.IsPartOf(*singleid, NULL)) // important to check for speedup c4d!
            // {
            if (!descparams.Desc.LoadDescription("tbase"))
                return false;


            // According to https://developers.maxon.net/docs/Cinema4DCPPSDK/html/class_description.html and
            // with #define DESCID_ROOT DescID(DescLevel(1000491, 0, 0)) as set in lib_description.h
            // Set the tag's name to something meaningful (other than "Base Tag" or "Basis Tag"
            DescID nCid = new DescID(new DescLevel(1000491, 0, 0));                              // The ID of the radius value (GENERAL_TAG_THEINT)
            BaseContainer nBc = descparams.Desc.GetParameterI(nCid, null);                        // The type of the radius value (LONG)
            string OldName = nBc.GetString(C4dApi.DESC_NAME);
            nBc.SetString(C4dApi.DESC_NAME, TagName);




            //////////////////////////////////////////////////////////////////////////////////////
            // Create an int value named TheInt on the "Base" tab's main level
            DescID cid = new DescID(new DescLevel(GENERAL_TAG_THEINT, C4dApi.DTYPE_LONG, 0));               // The ID of the radius value (GENERAL_TAG_THEINT)
            BaseContainer bc = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);                  // The type of the radius value (LONG)
            bc.SetString(C4dApi.DESC_NAME, "TheInt");                                               // The user interface name (TheInt)
            bc.SetInt32(C4dApi.DESC_DEFAULT, 44);                                                    // The default value (44, but overridden to 42 in the Init method)
            if (!descparams.Desc.SetParameter(cid, bc, new DescID(new DescLevel(C4dApi.ID_TAGPROPERTIES))))
                return true;


            //////////////////////////////////////////////////////////////////////////////////////
            // Create a string value named TheName on the "Base" tab's main level
            cid = new DescID(new DescLevel(GENERAL_TAG_THENAME, C4dApi.DTYPE_LONG, 0));               // The ID of the radius value (GENERAL_TAG_THENAME)
            bc = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_STRING);                  // The type of the radius value (STRING)
            bc.SetString(C4dApi.DESC_NAME, "TheString");                                               // The user interface name (TheName)
            bc.SetString(C4dApi.DESC_DEFAULT, "Worscht");                                                    // The default value (Worscht, but overridden to Horst in the Init method)
            if (!descparams.Desc.SetParameter(cid, bc, new DescID(new DescLevel(C4dApi.ID_TAGPROPERTIES))))
                return true;



            /*
            /////////////////////////////////////////////////////////////////////////////////////
            // Create an entirely new Tab (called "Ein schöner Tab")
            DescID cid = new DescID(new DescLevel(CIRCLEOBJECT_NEWTAB, C4dApi.DTYPE_GROUP, 0));
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
            */

            descparams.Flags |= DESCFLAGS_DESC.DESCFLAGS_DESC_LOADED;

            return true; // base.GetDDescription(node, descparams);
        }


    }
}
