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

        /// <summary>
        /// Title of window (visible in top bar)
        /// </summary>
        public string Id = "Open File";

        public readonly bool OnlyAllowFolders;
        public readonly List<string>? AllowedExtensions;

        public string? SelectedFile { get; private set; }
        public string RootFolder { get; private set; }

        private string _currentFolder;

        /// <summary>
        /// Text color of folder
        /// </summary>
        public Vector4 FolderColor = new(255, 0, 255, 255);

        /// <summary>
        /// Background color of pop up window
        /// </summary>
        public Vector4 WindowBackground = new(200, 200, 200, 255);

        /// <summary>
        /// Background of file selection menu
        /// </summary>
        public Vector4 FileSelectionMenuBackground = new(125, 125, 125, 255);

        /// <summary>
        /// Color of <see cref="ImGui.SetTooltip(string)"/> when an error occurs
        /// </summary>
        public Vector4 WarningTextColor = new(200, 0, 0, 255);

        /// <summary>
        /// Generate a new ImGuiFilePicker instance
        /// </summary>
        /// <param name="startingPath">Starting path, defaults to C:\</param>
        /// <param name="onlyAllowFolders">Allow folder picking only</param>
        /// <param name="allowedExtensions">search filter with dot. Example (".json")</param>
        public ImGuiFilePicker(string startingPath = "C:\\", bool onlyAllowFolders = true, string allowedExtensions = "")
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

            if (!string.IsNullOrEmpty(allowedExtensions))
            {
                if (AllowedExtensions != null)
                    AllowedExtensions.Clear();
                else
                    AllowedExtensions = new List<string>();

                AllowedExtensions.AddRange(allowedExtensions.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public unsafe void Draw(ref bool _filePickerOpen)
        {
            if (!_filePickerOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(15, 15));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 250));

            ImGui.PushStyleColor(ImGuiCol.WindowBg, WindowBackground.ToUintColor());

            ImGui.SetNextWindowFocus();
            ImGui.Begin(Id, ref _filePickerOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking);

            // Drive Selection
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

            // Folder Selection
            var currentFolder = _currentFolder;
            ImGui.SameLine();
            ImGui.SetNextItemWidth(400f);
            ImGui.InputTextWithHint("Folder", "Path to folder", ref currentFolder, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
            {
                var arr = currentFolder.ToCharArray();

                if (x->SelectionStart < x->SelectionEnd && x->SelectionStart >= 0 && x->SelectionEnd <= arr.Length)
                {
                    var selectedText = arr[x->SelectionStart..x->SelectionEnd];
                    if (selectedText != null)
                        ImGuiInputImp.CurrentlySelectedText = new string(selectedText);
                }

                return 0;
            });
            if (Directory.Exists(currentFolder))
            {
                _currentFolder = currentFolder;
            }
            else if (File.Exists(currentFolder))
            {
                var fi = new FileInfo(currentFolder);
                if (fi.DirectoryName != null)
                {
                    _currentFolder = fi.DirectoryName;
                    SelectedFile = fi.Name;
                }
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                ImGui.BeginTooltip();
                ImGui.TextColored(WarningTextColor, "Folder not found!");
                ImGui.EndTooltip();
                ImGui.PopStyleVar();
            }

            // Folder Browser
            ImGui.NewLine();
            ImGui.PushStyleColor(ImGuiCol.ChildBg, FileSelectionMenuBackground.ToUintColor());
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

            if (ImGui.BeginChild("#FolderBrowser", new Vector2(0, 200), false, ImGuiWindowFlags.AlwaysUseWindowPadding))
            {
                var di = new DirectoryInfo(_currentFolder);
                if (di.Exists)
                {
                    if (di.Parent != null && _currentFolder != RootFolder)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());

                        if (ImGui.Selectable("../", false, ImGuiSelectableFlags.DontClosePopups))
                        {
                            _currentFolder = di.Parent.FullName;
                            SelectedFile = "";
                        }

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
                            {
                                _currentFolder = fse;
                                SelectedFile = "";
                            }

                            ImGui.PopStyleColor();
                        }
                        else
                        {
                            var name = Path.GetFileName(fse);

                            if (ImGui.Selectable(name, SelectedFile == name, ImGuiSelectableFlags.DontClosePopups))
                                SelectedFile = name;

                            if (ImGui.IsMouseDoubleClicked(0) && SelectedFile != null && ImGui.GetIO().WantCaptureMouse)
                            {
                                _filePickerOpen = false;
                                OnPicked?.Invoke(this, Path.Combine(_currentFolder, SelectedFile));

                                ImGui.PopStyleVar(4);
                                ImGui.PopStyleColor(2);
                                return; // prevent double invoke due to double click
                            }
                        }
                    }
                }

                ImGui.PopStyleColor();
                ImGui.PopStyleVar();
                ImGui.EndChild();
            }



            // File Selector
            ImGui.NewLine();
            ImGui.BeginChild("#FileSelector", new Vector2(0, 30));

            var selectedFile = !string.IsNullOrWhiteSpace(SelectedFile) ? SelectedFile : "";
            ImGui.SetNextItemWidth(400f);
            ImGui.InputTextWithHint("File", "Selected file", ref selectedFile, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
            {
                var arr = selectedFile.ToCharArray();

                if (x->SelectionStart < x->SelectionEnd && x->SelectionStart >= 0 && x->SelectionEnd <= arr.Length)
                {
                    var selectedText = arr[x->SelectionStart..x->SelectionEnd];
                    if (selectedText != null)
                        ImGuiInputImp.CurrentlySelectedText = new string(selectedText);
                }
                return 0;
            });

            if (File.Exists(selectedFile))
            {
                var fi = new FileInfo(selectedFile);
                if (fi.DirectoryName != null)
                {
                    _currentFolder = fi.DirectoryName;
                    SelectedFile = fi.Name;
                }
            }
            else if (File.Exists(Path.Combine(_currentFolder, selectedFile)))
            {
                SelectedFile = selectedFile;
            }
            else if (Directory.Exists(selectedFile))
            {
                SelectedFile = "";
                _currentFolder = selectedFile;
            }
            else if (!string.IsNullOrWhiteSpace(selectedFile))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                ImGui.BeginTooltip();
                ImGui.TextColored(WarningTextColor, "File not found!");
                ImGui.EndTooltip();
                ImGui.PopStyleVar();
            }

            if (!string.IsNullOrWhiteSpace(SelectedFile))
            {
                var fi = new FileInfo(Path.Combine(_currentFolder, SelectedFile));
                if (fi.Exists && AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    ImGui.SameLine();
                    if (ImGui.Button("Open"))
                    {
                        OnPicked?.Invoke(this, Path.Combine(_currentFolder, selectedFile));
                        _filePickerOpen = false;
                    }
                }
            }
            else
            {
                ImGui.SameLine();
                ImGui.BeginDisabled();
                ImGui.Button("Open");
                ImGui.EndDisabled();
            }

            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                OnPicked?.Invoke(this, null);
                _filePickerOpen = false;
            }

            ImGui.EndChild();

            ImGui.End();

            ImGui.PopStyleVar(3);
            ImGui.PopStyleColor();

            return;
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