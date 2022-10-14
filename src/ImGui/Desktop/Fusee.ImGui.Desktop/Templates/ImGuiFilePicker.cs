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
        /// Invoked on clicked "open".
        /// </summary>
        public EventHandler<string?>? OnPicked;

        /// <summary>
        /// Invoked on cancel.
        /// </summary>
        public EventHandler? OnCancel;

        /// <summary>
        /// Title of window (visible in top bar).
        /// </summary>
        public string Id = "Open File";

        /// <summary>
        /// Caption of the "Open" button.
        /// </summary>
        public string OpenFileTxt = "Open";

        /// <summary>
        /// Caption of the "Cancel" button.
        /// </summary>
        public string CancelFileOpenTxt = "Cancel";

        /// <summary>
        /// File input hint text.
        /// </summary>
        public string FileInputHintTxt = "Selected file";

        /// <summary>
        /// Path to folder text.
        /// </summary>
        public string PathToFolderTxt = "Path to folder";

        /// <summary>
        /// Folder not found warning text.
        /// </summary>
        public string FolderNotFoundTxt = "Folder not found!";

        /// <summary>
        /// File not found warning text.
        /// </summary>
        public string FileNotFoundTxt = "File not found!";

        /// <summary>
        /// Caption of folder input text
        /// </summary>
        public string FolderLabelTxt = "Folder";

        /// <summary>
        /// Caption of file input text
        /// </summary>
        public string FileLabelTxt = "File";

        public string ParentFolderTxt = "Parent";
        public string BackTxt = "Back";

        public readonly bool OnlyAllowFolders;
        public readonly List<string>? AllowedExtensions;

        public string? SelectedFile { get; private set; }
        public string RootFolder { get; private set; }

        private string _currentOpenFolder;
        private Stack<string> _lastOpenendFolders = new();
        private string _currentlySelectedFolder;
        private string _startingFolder;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            private set
            {
                if (value != _isOpen)
                {
                    _isOpen = value;
                    if (!_isOpen)
                        _currentOpenFolder = _startingFolder;
                }
            }
        }
        private bool _isOpen;

        // needed for width calculation
        private Vector2 _sizeOfInputText;

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
        /// Background color of one <see cref="ImGui.Selectable(string)"/> object
        /// </summary>
        public Vector4 SelectedColor = new(125, 75, 75, 255);

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
            _currentOpenFolder = startingPath;
            _startingFolder = startingPath;
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

        public unsafe void Draw(ref bool filePickerOpen)
        {
            IsOpen = filePickerOpen;
            if (!filePickerOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(15, 15));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, WindowBackground.ToUintColor());

            ImGui.SetNextWindowFocus();
            ImGui.SetNextWindowSize(new Vector2(350 + 100 + 5 + ImGui.CalcTextSize(FolderLabelTxt).X, 350));
            ImGui.Begin(Id, ref filePickerOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking);

            var di = new DirectoryInfo(_currentOpenFolder);
            if (ImGui.Button($"{ParentFolderTxt}##FilePickerParentDir"))
            {
                if (di.Exists && di.Parent != null && _currentOpenFolder != RootFolder)
                {
                    _currentOpenFolder = di.Parent.FullName;
                    SelectedFile = "";
                }
            }
            ImGui.SameLine();
            if (ImGui.Button($"{BackTxt}##FilePickerBackDir"))
            {
                if (_lastOpenendFolders.Count != 0)
                {
                    var lastFolder = _lastOpenendFolders.Pop();
                    var lastDi = new DirectoryInfo(lastFolder);
                    if (lastDi.Exists)
                    {
                        _currentOpenFolder = lastFolder;
                        SelectedFile = "";
                    }
                }
            }

            // Folder Selection
            var currentFolder = _currentOpenFolder;
            ImGui.SameLine();
            ImGui.PushItemWidth(350 - ImGui.CalcTextSize(FolderLabelTxt).X);
            ImGui.InputTextWithHint(FolderLabelTxt, PathToFolderTxt, ref currentFolder, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
            {
                ImGui.PopItemWidth();
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
                _currentOpenFolder = currentFolder;
            }
            else if (File.Exists(currentFolder))
            {
                var fi = new FileInfo(currentFolder);
                if (fi.DirectoryName != null)
                {
                    _currentOpenFolder = fi.DirectoryName;
                    SelectedFile = fi.Name;
                }
            }
            else
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                ImGui.BeginTooltip();
                ImGui.TextColored(WarningTextColor, FolderNotFoundTxt);
                ImGui.EndTooltip();
                ImGui.PopStyleVar();
            }

            // Folder Browser
            ImGui.NewLine();
            ImGui.PushStyleColor(ImGuiCol.ChildBg, FileSelectionMenuBackground.ToUintColor());
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

            ImGui.BeginChild("DiveSelection", new Vector2(100, 200), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize);
            // Drive Selection
            var driveCount = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    if (ImGui.Selectable($"{drive.Name} {drive.DriveType}"))
                    {
                        RootFolder = drive.Name;
                        _lastOpenendFolders.Push(_currentOpenFolder);
                        _currentOpenFolder = drive.Name;
                        SelectedFile = "";
                    }
                    driveCount++;
                }
            }
            ImGui.EndChild();
            ImGui.SameLine();

            if (ImGui.BeginChild("#FolderBrowser", new Vector2(350, 200), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.HorizontalScrollbar))
            {
                di = new DirectoryInfo(_currentOpenFolder);
                if (di.Exists)
                {
                    var fileSystemEntries = GetFileSystemEntries(di.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        if (Directory.Exists(fse))
                        {
                            var name = Path.GetFileName(fse);

                            ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());

                            if (ImGui.Selectable(name + "/", _currentlySelectedFolder == fse, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                SelectedFile = "";
                                _currentlySelectedFolder = fse;
                                if (ImGui.IsMouseDoubleClicked(0) && (SelectedFile == null || SelectedFile == "") && ImGui.GetIO().WantCaptureMouse)
                                {
                                    _lastOpenendFolders.Push(_currentOpenFolder);
                                    _currentOpenFolder = fse;
                                }
                            }

                            ImGui.PopStyleColor();
                        }
                        else
                        {
                            var name = Path.GetFileName(fse);

                            ImGui.PushStyleColor(ImGuiCol.Header, SelectedColor.ToUintColor());

                            if (ImGui.Selectable(name, SelectedFile == name, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                SelectedFile = name;
                                if (ImGui.IsMouseDoubleClicked(0) && (SelectedFile != null && SelectedFile != "") && ImGui.GetIO().WantCaptureMouse)
                                {
                                    filePickerOpen = false;
                                    OnPicked?.Invoke(this, Path.Combine(_currentOpenFolder, SelectedFile));

                                    ImGui.PopStyleVar(4);
                                    ImGui.PopStyleColor(3);
                                    return; // prevent double invoke due to double click
                                }
                            }

                            ImGui.PopStyleColor();


                        }
                    }
                }

                ImGui.PopStyleColor();
                ImGui.PopStyleVar();
                ImGui.EndChild();
            }

            // File Selector
            ImGui.NewLine();
            ImGui.BeginChild("#FileSelector", new Vector2(-1, -1), false, ImGuiWindowFlags.AlwaysAutoResize);

            var selectedFile = !string.IsNullOrWhiteSpace(SelectedFile) ? SelectedFile : "";
            ImGui.InputTextWithHint(FileLabelTxt, FileInputHintTxt, ref selectedFile, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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
            if (_sizeOfInputText == Vector2.Zero)
                _sizeOfInputText = ImGui.GetItemRectSize();

            if (File.Exists(selectedFile))
            {
                var fi = new FileInfo(selectedFile);
                if (fi.DirectoryName != null)
                {
                    _currentOpenFolder = fi.DirectoryName;
                    SelectedFile = fi.Name;
                }
            }
            else if (File.Exists(Path.Combine(_currentOpenFolder, selectedFile)))
            {
                SelectedFile = selectedFile;
            }
            else if (Directory.Exists(selectedFile))
            {
                SelectedFile = "";
                _currentOpenFolder = selectedFile;
            }
            else if (!string.IsNullOrWhiteSpace(selectedFile))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                ImGui.BeginTooltip();
                ImGui.TextColored(WarningTextColor, FileNotFoundTxt);
                ImGui.EndTooltip();
                ImGui.PopStyleVar();
            }

            if (!string.IsNullOrWhiteSpace(SelectedFile))
            {
                var fi = new FileInfo(Path.Combine(_currentOpenFolder, SelectedFile));
                if (fi.Exists && AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    ImGui.SameLine();
                    if (ImGui.Button(OpenFileTxt))
                    {
                        OnPicked?.Invoke(this, Path.Combine(_currentOpenFolder, selectedFile));
                        filePickerOpen = false;
                    }
                }
            }
            else
            {
                ImGui.SameLine();
                ImGui.BeginDisabled();
                ImGui.Button(OpenFileTxt);
                ImGui.EndDisabled();
            }

            ImGui.SameLine();
            if (ImGui.Button(CancelFileOpenTxt))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                filePickerOpen = false;
            }

            ImGui.EndChild();

            ImGui.End();

            ImGui.PopStyleVar(2);
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