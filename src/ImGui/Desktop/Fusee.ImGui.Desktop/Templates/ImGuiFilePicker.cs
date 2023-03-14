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
        public string PickedFileTxt = "Open";

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

        public bool OnlyAllowFolders;
        public List<string>? AllowedExtensions;

        public string? SelectedFile { get; protected set; }
        public string RootFolder { get; protected set; }

        public int FontSize;
        public ImFontPtr SymbolsFontPtr = null;

        protected string CurrentOpenFolder;
        protected readonly Stack<string> LastOpenendFolders = new();
        protected string CurrentlySelectedFolder;
        protected readonly string StartingFolder;

        protected const float FolderTextInputWidth = 350;
        protected const float FileTextInputWidth = 300;
        protected const float DriveSelectionWidth = 100;
        protected const float BrowserHeight = 200;
        protected readonly Vector2 WindowPadding = new(15, 15);
        protected readonly Vector2 BottomButtonSize = new(55, 26);
        protected readonly Vector2 TopButtonSize = new(35, 30);
        protected Vector2 WinSize;
        protected bool DoFocusPicker = true;

        private static int _filePickerCount = 0;

        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                if (value != _isOpen)
                {
                    _isOpen = value;
                    if (!_isOpen)
                        CurrentOpenFolder = StartingFolder;
                }
            }
        }
        protected bool _isOpen;

        // needed for width calculation
        protected Vector2 _sizeOfInputText;

        /// <summary>
        /// Text color of folder
        /// </summary>
        public Vector4 FolderColor = new(255, 0, 255, 255);

        /// <summary>
        /// Background color of pop up window
        /// </summary>
        public Vector4 WindowBackground
        {
            get => _windowBackground;
            set
            {
                _windowBackground = value;
                _windowBackgroundUint = _windowBackground.ToUintColor();
            }
        }
        private Vector4 _windowBackground = new(200, 200, 200, 255);

        public uint _windowBackgroundUint = new Vector4(200, 200, 200, 255).ToUintColor();

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
            _filePickerCount++;

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
            CurrentOpenFolder = startingPath;
            StartingFolder = startingPath;
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

        public virtual unsafe void Draw(ref bool filePickerOpen)
        {
            IsOpen = filePickerOpen;
            if (!filePickerOpen) return;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, WindowPadding);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, _windowBackgroundUint);

            if (DoFocusPicker)
                ImGui.SetNextWindowFocus();
            var headerHeight = FontSize + WindowPadding.Y * 2;
            var itemSpacing = ImGui.GetStyle().ItemSpacing;
            WinSize = new Vector2(FolderTextInputWidth + DriveSelectionWidth + (WindowPadding.X * 2) + itemSpacing.X, headerHeight + BrowserHeight + TopButtonSize.Y + BottomButtonSize.Y + 4 * WindowPadding.Y + 3 * itemSpacing.Y + 5);
            ImGui.SetNextWindowSize(WinSize);
            ImGui.Begin(Id, ref filePickerOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking);

            if ((IntPtr)SymbolsFontPtr.NativePtr != IntPtr.Zero)
                ImGui.PushFont(SymbolsFontPtr);

            ImGui.BeginGroup();
            var di = new DirectoryInfo(CurrentOpenFolder);
            if (ImGui.Button($"{ParentFolderTxt}##{_filePickerCount}", TopButtonSize))
            {
                if (di.Exists && di.Parent != null)
                {
                    LastOpenendFolders.Push(CurrentOpenFolder);
                    CurrentOpenFolder = di.Parent.FullName;
                    SelectedFile = "";
                }
            }
            ImGui.SameLine();

            if (LastOpenendFolders.Count != 0)
            {
                if (ImGui.Button($"{BackTxt}##{_filePickerCount}", TopButtonSize))
                {

                    var lastFolder = LastOpenendFolders.Pop();
                    var lastDi = new DirectoryInfo(lastFolder);
                    if (lastDi.Exists)
                    {
                        CurrentOpenFolder = lastFolder;
                        SelectedFile = "";
                    }
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button($"{BackTxt}##{_filePickerCount}", TopButtonSize);
                ImGui.EndDisabled();
            }

            if ((IntPtr)SymbolsFontPtr.NativePtr != IntPtr.Zero)
                ImGui.PopFont();

            ImGui.EndGroup();

            // Folder Selection
            var currentFolder = CurrentOpenFolder;
            ImGui.SameLine(DriveSelectionWidth + WindowPadding.X + ImGui.GetStyle().ItemSpacing.X);
            ImGui.SetNextItemWidth(FolderTextInputWidth - ImGui.CalcTextSize(FolderLabelTxt).X - ImGui.GetStyle().ItemSpacing.X);
            ImGui.InputTextWithHint($"{FolderLabelTxt}##{_filePickerCount}", PathToFolderTxt, ref currentFolder, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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
                CurrentOpenFolder = currentFolder;
            }
            else if (File.Exists(currentFolder))
            {
                var fi = new FileInfo(currentFolder);
                if (fi.DirectoryName != null)
                {
                    CurrentOpenFolder = fi.DirectoryName;
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

            ImGui.BeginChild($"DriveSelection##{_filePickerCount}", new Vector2(DriveSelectionWidth, BrowserHeight), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize);
            // Drive Selection
            var driveCount = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    if (ImGui.Selectable($"{drive.Name} {drive.DriveType}##{_filePickerCount}"))
                    {
                        RootFolder = drive.Name;
                        LastOpenendFolders.Push(CurrentOpenFolder);
                        CurrentOpenFolder = drive.Name;
                        SelectedFile = "";
                    }
                    driveCount++;
                }
            }
            ImGui.EndChild();
            ImGui.SameLine();

            if (ImGui.BeginChild($"#FolderBrowser##{_filePickerCount}", new Vector2(FolderTextInputWidth, BrowserHeight), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.HorizontalScrollbar))
            {
                di = new DirectoryInfo(CurrentOpenFolder);
                if (di.Exists)
                {
                    var fileSystemEntries = GetFileSystemEntries(di.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        if (Directory.Exists(fse))
                        {
                            var name = Path.GetFileName(fse);

                            ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());

                            if (ImGui.Selectable(name + "/", CurrentlySelectedFolder == fse, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                SelectedFile = "";
                                CurrentlySelectedFolder = fse;
                                if (ImGui.IsMouseDoubleClicked(0) && (SelectedFile == null || SelectedFile == "") && ImGui.GetIO().WantCaptureMouse)
                                {
                                    LastOpenendFolders.Push(CurrentOpenFolder);
                                    CurrentOpenFolder = fse;
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
                                if (SelectedFile == name)
                                    SelectedFile = "";
                                else
                                    SelectedFile = name;

                                if (ImGui.IsMouseDoubleClicked(0) && (SelectedFile != null && SelectedFile != "") && ImGui.GetIO().WantCaptureMouse)
                                {
                                    if (HandlePickedFile(SelectedFile))
                                    {
                                        filePickerOpen = false;
                                        OnPicked?.Invoke(this, Path.Combine(CurrentOpenFolder, SelectedFile));
                                    }
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
            ImGui.BeginChild($"FileSelector##{_filePickerCount}", new Vector2(-1, -1), false, ImGuiWindowFlags.AlwaysAutoResize);

            var selectedFile = !string.IsNullOrWhiteSpace(SelectedFile) ? SelectedFile : "";
            ImGui.SetNextItemWidth(FileTextInputWidth - ImGui.CalcTextSize(FileLabelTxt).X - ImGui.GetStyle().ItemSpacing.X);
            if (ImGui.InputTextWithHint(FileLabelTxt, FileInputHintTxt, ref selectedFile, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
            {
                var arr = selectedFile.ToCharArray();

                if (x->SelectionStart < x->SelectionEnd && x->SelectionStart >= 0 && x->SelectionEnd <= arr.Length)
                {
                    var selectedText = arr[x->SelectionStart..x->SelectionEnd];
                    if (selectedText != null)
                        ImGuiInputImp.CurrentlySelectedText = new string(selectedText);
                }
                return 0;
            }))
            {
                SelectedFile = selectedFile;
            }

            if (_sizeOfInputText == Vector2.Zero)
                _sizeOfInputText = ImGui.GetItemRectSize();

            var sameLineOffset = WinSize.X - WindowPadding.X - (BottomButtonSize.X * 2 + ImGui.GetStyle().ItemSpacing.X * 4);
            if (!OnlyAllowFolders && !string.IsNullOrWhiteSpace(SelectedFile))
            {
                var fi = new FileInfo(Path.Combine(CurrentOpenFolder, SelectedFile));
                if (AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    ImGui.SameLine(sameLineOffset);
                    if (ImGui.Button($"{PickedFileTxt}##{_filePickerCount}", BottomButtonSize))
                    {
                        if (HandlePickedFile(selectedFile))
                        {
                            OnPicked?.Invoke(this, Path.Combine(CurrentOpenFolder, selectedFile));
                            filePickerOpen = false;
                        }
                    }
                }
                else
                {
                    ImGui.SameLine(sameLineOffset);
                    ImGui.BeginDisabled();
                    ImGui.Button(PickedFileTxt, BottomButtonSize);
                    ImGui.EndDisabled();
                }
            }
            else if (OnlyAllowFolders && !string.IsNullOrWhiteSpace(CurrentlySelectedFolder) && string.IsNullOrWhiteSpace(SelectedFile))
            {
                ImGui.SameLine(sameLineOffset);
                if (ImGui.Button($"{PickedFileTxt}##{_filePickerCount}", BottomButtonSize))
                {
                    OnPicked?.Invoke(this, Path.Combine(CurrentOpenFolder, selectedFile));
                    filePickerOpen = false;
                }
            }
            else
            {
                ImGui.SameLine(sameLineOffset);
                ImGui.BeginDisabled();
                ImGui.Button(PickedFileTxt, BottomButtonSize);
                ImGui.EndDisabled();
            }

            ImGui.SameLine();
            if (ImGui.Button($"{CancelFileOpenTxt}##{_filePickerCount}", BottomButtonSize))
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
                    else
                    {
                        if (!OnlyAllowFolders)
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

        protected virtual bool HandlePickedFile(string selectedFile)
        {
            if (File.Exists(selectedFile))
            {
                var fi = new FileInfo(selectedFile);
                if (fi.DirectoryName != null)
                {
                    CurrentOpenFolder = fi.DirectoryName;
                    SelectedFile = fi.Name;
                    return true;
                }
            }
            else if (File.Exists(Path.Combine(CurrentOpenFolder, selectedFile)))
            {
                SelectedFile = selectedFile;
                return true;
            }
            else if (Directory.Exists(selectedFile))
            {
                SelectedFile = "";
                CurrentOpenFolder = selectedFile;
                return false;
            }
            else if (!string.IsNullOrWhiteSpace(selectedFile))
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                ImGui.BeginTooltip();
                ImGui.TextColored(WarningTextColor, FileNotFoundTxt);
                ImGui.EndTooltip();
                ImGui.PopStyleVar();

                return false;
            }

            return false;
        }
    }
}