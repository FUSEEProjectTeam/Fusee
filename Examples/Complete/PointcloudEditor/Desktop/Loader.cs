using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Fusee.Examples.PointcloudEditor.Desktop
{
    public class Loader : Fusee.ImGuiDesktop.Templates.ImGuiFileDialog
    {

        //will implement methods to load data from files

        public string LoadPointcloud() 
        {
            return PickerDialog(".laz|.las"); 
        }

        public string LoadImage()
        {
            return PickerDialog(".png|.jpeg|.jpg");
        }

        public string LoadPositionData()
        {
            return PickerDialog(".csv|.json|.log");
        }

        private string PickerDialog(string searchFilter) 
        {
            //TODO: Figure out how to access and return the filepath
            var picker = Fusee.ImGuiDesktop.Templates.ImGuiFileDialog.GetFilePicker(this, Path.Combine(Environment.CurrentDirectory, ""), new float4(30, 180, 30, 255), searchFilter);
            string filePath = "";
            return filePath; 
        
        }
    }
}
