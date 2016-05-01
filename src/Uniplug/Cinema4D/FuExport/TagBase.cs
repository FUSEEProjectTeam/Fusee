using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using C4d;
// using ProtoBuf;

namespace FuExport
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TagItemAttribute : Attribute
    {
        public string DisplayName;
    }


    // Your Plugin Number for "FUSEE General Tag" is: 1036156 
    // [TagPlugin(1036156, "FUSEE Step2016 Data", "FuExportLogo.tif", Visible = true)]
    public class TagBase : TagDataM
    {
        // values 1000.3000 already reserved from includes
        private static readonly int FirstItemID = 10000;
        

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

        public TagBase() : base(false)
        {
        }

        public delegate void PropVisitorFunc(int itemID, string itemName, PropertyInfo property);

        protected void VisitProps(PropVisitorFunc propVisitor)
        {
            int itemID = FirstItemID;
            foreach (var prop in GetType().GetProperties())
            {
                var tagItem = prop.GetCustomAttributes(false).FirstOrDefault(s => s is TagItemAttribute) as TagItemAttribute;
                if (tagItem != null)
                {
                    string itemName = (string.IsNullOrEmpty(tagItem.DisplayName)) ? prop.Name : tagItem.DisplayName;
                    propVisitor(itemID++, itemName, prop);
                }
            }
        }

        public override bool Init(GeListNode node)
        {
            BaseContainer data = GetDataInstance(node);

            VisitProps(delegate(int itemID, string itemName, PropertyInfo pi)
            {
                if (pi.PropertyType == typeof(int))
                    data.SetInt32(itemID, (int) pi.GetValue(this, null));
                else if (pi.PropertyType == typeof(string))
                    data.SetString(itemID, (string)pi.GetValue(this, null));
            });

            return true;
        }

        public override bool GetDDescription(GeListNode node, DDescriptionParams descparams)
        {
            if (!descparams.Desc.LoadDescription("tbase"))
                return false;

            VisitProps(delegate (int itemID, string itemName, PropertyInfo pi)
            {
                DescID cid = new DescID(new DescLevel(itemID, C4dApi.DTYPE_LONG, 0));
                BaseContainer bc = null;
                if (pi.PropertyType == typeof (int))
                {
                    bc = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_LONG);
                }
                else if (pi.PropertyType == typeof (string))
                {
                    bc = C4dApi.GetCustomDataTypeDefault(C4dApi.DTYPE_STRING);
                }
                bc.SetString(C4dApi.DESC_NAME, itemName);
                descparams.Desc.SetParameter(cid, bc, new DescID(new DescLevel(C4dApi.ID_TAGPROPERTIES)));
            });

            descparams.Flags |= DESCFLAGS_DESC.DESCFLAGS_DESC_LOADED;

            return true; // base.GetDDescription(node, descparams);
        }
    }
}
