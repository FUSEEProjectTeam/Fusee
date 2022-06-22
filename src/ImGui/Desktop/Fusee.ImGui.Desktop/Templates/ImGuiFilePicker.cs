using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Fusee.ImGuiDesktop.Templates
{
    /// <summary>
    /// Very rough implementation of a File(Picker-)Dialog without any dependencies to any os and/or winforms/wpf
    /// based upon: https://github.com/mellinoe/synthapp/blob/master/src/synthapp/Widgets/FilePicker.cs
    /// </summary>
    public class ImGuiFileDialog
    {
        private static readonly Dictionary<object, ImGuiFileDialog> _filePickers = new();

        public string RootFolder = AppContext.BaseDirectory;
        public string CurrentFolder = Environment.CurrentDirectory;
        public string SelectedFile = "";
        public List<string> AllowedExtensions = new();
        public bool OnlyAllowFolders;

        private float4 _folderColor;

        /// <summary>
        /// Returns a folder picker instance
        /// </summary>
        /// <param name="o"></param>
        /// <param name="startingPath"></param>
        /// <returns></returns>
        public static ImGuiFileDialog GetFolderPicker(object o, string startingPath)
            => GetFilePicker(o, startingPath, new float4(255, 0, 255, 255), "", true);

        /// <summary>
        /// Returns a file picker instance, bind one file picker to one class via <see cref="GetFilePicker(object, string, float4, string, bool)"/>
        /// </summary>
        /// <param name="o"></param>
        /// <param name="startingPath"></param>
        /// <param name="folderColor"></param>
        /// <param name="searchFilter"></param>
        /// <param name="onlyAllowFolders"></param>
        /// <returns></returns>
        public static ImGuiFileDialog GetFilePicker(object o, string startingPath, float4 folderColor, string searchFilter = "", bool onlyAllowFolders = false)
        {
            if (!_filePickers.TryGetValue(o, out var fp))
            {


                if (File.Exists(startingPath))
                {
                    startingPath = new FileInfo(startingPath)?.DirectoryName ?? "C:\\";
                }
                else if (string.IsNullOrEmpty(startingPath) || !Directory.Exists(startingPath))
                {
                    startingPath = Environment.CurrentDirectory;
                    if (string.IsNullOrEmpty(startingPath))
                        startingPath = AppContext.BaseDirectory;
                }

                fp = new ImGuiFileDialog
                {
                    RootFolder = startingPath,
                    CurrentFolder = startingPath,
                    OnlyAllowFolders = onlyAllowFolders,
                    _folderColor = folderColor
                };

                if (searchFilter != null)
                {
                    if (fp.AllowedExtensions != null)
                        fp.AllowedExtensions.Clear();
                    else
                        fp.AllowedExtensions = new List<string>();

                    fp.AllowedExtensions.AddRange(searchFilter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                }

                _filePickers.Add(o, fp);
            }

            return fp;
        }

        public static void RemoveFilePicker(object o) => _filePickers.Remove(o);

        public bool Draw()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    ImGui.SameLine();
                    if (ImGui.Button(drive.Name))
                    {
                        RootFolder = drive.Name;
                        CurrentFolder = drive.Name;
                        SelectedFile = "";
                    }
                }
            }

            bool result = false;

            if (ImGui.BeginChildFrame(1, new Vector2(ImGui.GetWindowSize().X, 400)))
            {

                var di = new DirectoryInfo(CurrentFolder);
                if (di.Exists)
                {
                    if (di.Parent != null && CurrentFolder != RootFolder)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, _folderColor.ToUintColor());
                        if (ImGui.Selectable("../", false, ImGuiSelectableFlags.DontClosePopups))
                            CurrentFolder = di.Parent.FullName;

                        ImGui.PopStyleColor();
                    }

                    var fileSystemEntries = GetFileSystemEntries(di.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        if (Directory.Exists(fse))
                        {
                            var name = Path.GetFileName(fse);
                            ImGui.PushStyleColor(ImGuiCol.Text, _folderColor.ToUintColor());
                            if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.DontClosePopups))
                                CurrentFolder = fse;
                            ImGui.PopStyleColor();
                        }
                        else
                        {
                            var name = Path.GetFileName(fse);
                            bool isSelected = SelectedFile == fse;
                            if (ImGui.Selectable(name, isSelected, ImGuiSelectableFlags.DontClosePopups))
                                SelectedFile = fse;

                            if (ImGui.IsMouseDoubleClicked(0))
                            {
                                result = true;
                                ImGui.CloseCurrentPopup();
                            }
                        }
                    }
                }
            }

            ImGui.EndChildFrame();

            ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionMax().X - 80);
            if (OnlyAllowFolders)
            {
                ImGui.InputTextWithHint("Folder", "Select folder", ref CurrentFolder, uint.MaxValue - 1);
            }
            else
            {
                var text = string.IsNullOrWhiteSpace(SelectedFile) ? CurrentFolder : SelectedFile;
                ImGui.InputTextWithHint("File", "Select file", ref text, uint.MaxValue - 1);
            }
            ImGui.NewLine();

            ImGui.BeginChild("OpenFile");
            ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 50);
            ImGui.SetNextItemWidth(70);

            if (OnlyAllowFolders)
            {
                if (ImGui.Button("Open"))
                {
                    result = true;
                    SelectedFile = CurrentFolder;
                    ImGui.CloseCurrentPopup();
                }
            }
            else if (!string.IsNullOrWhiteSpace(SelectedFile))
            {
                var fi = new FileInfo(SelectedFile);
                if (fi.Exists && AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    if (ImGui.Button("Open"))
                    {
                        result = true;
                        ImGui.CloseCurrentPopup();
                    }
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button("Open");
                ImGui.EndDisabled();
            }

            ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 110);
            ImGui.SetNextItemWidth(70);
            if (ImGui.Button("Cancel"))
            {
                result = false;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndChild();

            return result;
        }

        private bool TryGetFileInfo(string fileName, out FileInfo realFile)
        {
            try
            {
                realFile = new FileInfo(fileName);
                return true;
            }
            catch
            {
                realFile = null;
                return false;
            }
        }

        private List<string> GetFileSystemEntries(string fullName)
        {
            try
            {
                var files = new List<string>();
                var dirs = new List<string>();

                foreach (var fse in Directory.GetFileSystemEntries(fullName, ""))
                {
                    if (Directory.Exists(fse))
                    {
                        dirs.Add(fse);
                    }
                    else if (!OnlyAllowFolders)
                    {
                        if (AllowedExtensions != null)
                        {
                            var ext = Path.GetExtension(fse);
                            if (AllowedExtensions.Contains(ext))
                                files.Add(fse);
                        }
                        else
                        {
                            files.Add(fse);
                        }
                    }
                }

                var ret = new List<string>(dirs);
                ret.AddRange(files);


                return ret;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

    }
}