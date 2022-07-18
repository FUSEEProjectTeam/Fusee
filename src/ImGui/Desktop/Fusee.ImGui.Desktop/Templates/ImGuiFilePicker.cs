using Fusee.Math.Core;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Fusee.ImGuiImp.Desktop.Templates
{
    public class ImGuiFilePicker
    {
        /// <summary>
        /// On picked with path or no value on cancel
        /// </summary>
        public EventHandler<string?>? OnPicked;


        public readonly bool OnlyAllowFolders;
        public readonly List<string>? AllowedExtensions;

        public string? SelectedFile { get; private set; }
        public string RootFolder { get; private set; }

        private bool filePickerOpen = true;
        private string _currentFolder;

        public float4 FolderColor = new(255, 0, 255, 255);

        public float4 DriveSelectionBackground = new(45, 45, 45, 255);
        public float4 FileFolderSelectionBackground = new(45, 45, 45, 255);
        public float4 FileFolderInputBackground = new(45, 45, 45, 255);
        public float4 OpenCancelButtonBackground = new(45, 45, 45, 255);
        public float4 PopupBackground = new(45, 45, 45, 255);

        public bool DrawBorder = false;

        /// <summary>
        /// Generate a new ImGuiFilePicker instance
        /// </summary>
        /// <param name="startingPath">Starting path, defaults to C:\</param>
        /// <param name="onlyAllowFolders">Allow folder picking only</param>
        /// <param name="searchFilter">search filter with dot. Example (".json")</param>
        public ImGuiFilePicker(string startingPath = "C:\\", bool onlyAllowFolders = true, string searchFilter = "")
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

            RootFolder = startingPath;
            _currentFolder = startingPath;
            OnlyAllowFolders = onlyAllowFolders;

            if (!string.IsNullOrEmpty(searchFilter))
            {
                if (AllowedExtensions != null)
                    AllowedExtensions.Clear();
                else
                    AllowedExtensions = new List<string>();

                AllowedExtensions.AddRange(searchFilter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public void Draw(ref bool spwanOpenFilePopup)
        {
            ImGui.PushStyleColor(ImGuiCol.PopupBg, PopupBackground.ToUintColor());


            if (spwanOpenFilePopup)
            {
                ImGui.SetNextWindowSizeConstraints(new Vector2(700, 580), ImGui.GetWindowViewport().Size);
                ImGui.OpenPopup("open-file");
                spwanOpenFilePopup = false;
            }

            if (ImGui.BeginPopupModal("open-file", ref filePickerOpen, ImGuiWindowFlags.NoTitleBar))
            {

                ImGui.PushStyleColor(ImGuiCol.ChildBg, DriveSelectionBackground.ToUintColor());
                ImGui.BeginChild("DriveSelection", new Vector2(ImGui.GetWindowSize().X, 35), DrawBorder, ImGuiWindowFlags.AlwaysAutoResize);
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        ImGui.SameLine();
                        if (ImGui.Button(drive.Name))
                        {
                            RootFolder = drive.Name;
                            _currentFolder = drive.Name;
                            SelectedFile = "";
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.PopStyleColor();

                ImGui.PushStyleColor(ImGuiCol.ChildBg, FileFolderSelectionBackground.ToUintColor());
                if (ImGui.BeginChild(1, new Vector2(ImGui.GetWindowSize().X, 475), DrawBorder, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.NoMove))
                {

                    var di = new DirectoryInfo(_currentFolder);
                    if (di.Exists)
                    {
                        if (di.Parent != null && _currentFolder != RootFolder)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());
                            if (ImGui.Selectable("../", false, ImGuiSelectableFlags.DontClosePopups))
                                _currentFolder = di.Parent.FullName;

                            ImGui.PopStyleColor();
                        }

                        var fileSystemEntries = GetFileSystemEntries(di.FullName);
                        foreach (var fse in fileSystemEntries)
                        {
                            if (Directory.Exists(fse))
                            {
                                var name = Path.GetFileName(fse);
                                ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());
                                if (ImGui.Selectable(name + "/", false, ImGuiSelectableFlags.DontClosePopups))
                                    _currentFolder = fse;
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
                                    OnPicked?.Invoke(this, SelectedFile);
                                    ImGui.CloseCurrentPopup();
                                }
                            }
                        }
                    }
                }

                ImGui.EndChild();
                ImGui.PopStyleColor();


                ImGui.PushStyleColor(ImGuiCol.ChildBg, FileFolderInputBackground.ToUintColor());
                ImGui.BeginChild("SelectFolderFile", new Vector2(ImGui.GetWindowSize().X, 35), DrawBorder, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionMax().X - 80);
                if (OnlyAllowFolders)
                {
                    ImGui.InputTextWithHint("Folder", "Select folder", ref _currentFolder, uint.MaxValue - 1);
                }
                else
                {
                    var text = string.IsNullOrWhiteSpace(SelectedFile) ? _currentFolder : SelectedFile;
                    ImGui.InputTextWithHint("File", "Select file", ref text, uint.MaxValue - 1);
                }
                ImGui.NewLine();
                ImGui.EndChild();
                ImGui.PopStyleColor();

                ImGui.PushStyleColor(ImGuiCol.ChildBg, OpenCancelButtonBackground.ToUintColor());
                ImGui.BeginChild("OpenFile", new Vector2(ImGui.GetWindowSize().X - 50, 35), DrawBorder, ImGuiWindowFlags.AlwaysAutoResize);
                ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 50);
                ImGui.SetNextItemWidth(70);

                if (OnlyAllowFolders)
                {
                    if (ImGui.Button("Open"))
                    {
                        SelectedFile = _currentFolder;
                        OnPicked?.Invoke(this, SelectedFile);
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
                            OnPicked?.Invoke(this, SelectedFile);
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
                    OnPicked?.Invoke(this, null);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndChild();
                ImGui.PopStyleColor();

                ImGui.EndPopup();
            }

            ImGui.PopStyleColor();
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
