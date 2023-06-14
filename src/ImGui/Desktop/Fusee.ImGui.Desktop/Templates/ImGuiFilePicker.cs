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
        public EventHandler<FileInfo>? OnPicked;

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

        public List<string>? AllowedExtensions;

        /// <summary>
        /// Show a button which let's the user create a new folder at the current directory
        /// </summary>
        public bool ShowNewFolderButton
        {
            get => _showNewFolderButton;
            set
            {
                if (value)
                    DriveSelectionWidth = 120;
                else
                    DriveSelectionWidth = 100;

                _showNewFolderButton = value;
            }
        }
        public string NewFolderButtonTxt = "\uf65e";

        /// <summary>
        /// Caption of the create new folder window
        /// </summary>
        public string CreateNewFolderTxt = "Create new folder";

        /// <summary>
        /// Caption of the create folder button
        /// </summary>
        public string CreateFolderTxt = "Create folder";

        /// <summary>
        /// Create new folder name hint txt
        /// </summary>
        public string CreateNewFolderHintTxt = "Insert folder name";

        private bool _showNewFolderButton;
        private bool _isNewFolderNameWindowOpen;

        // as we cannot use the property as ref, we need to check and set all variables every time
        // so that, when we call if(IsNewFolderNameWindowOpen), the variables are being set properly, too even when the window itself was closed via 'x'
        private bool IsNewFolderNameWindowOpen
        {
            get
            {
                if (_isNewFolderNameWindowOpen)
                {
                    // push the folder window to the back
                    DoFocusPicker = false;
                }
                else
                {
                    // reset text and reset windows
                    _createFolderException = null;
                    _newFolderName = "";
                    DoFocusPicker = true;
                }
                return _isNewFolderNameWindowOpen;
            }
            set
            {
                _isNewFolderNameWindowOpen = value;
                if (_isNewFolderNameWindowOpen)
                {
                    // push the folder window to the back
                    DoFocusPicker = false;
                }
                else
                {
                    // reset text and reset windows
                    _createFolderException = null;
                    _newFolderName = "";
                    DoFocusPicker = true;
                }
            }
        }
        private string _newFolderName = "";
        private Exception? _createFolderException;


        public FileInfo? SelectedFile { get; protected set; }
        public DirectoryInfo RootFolder { get; protected set; }

        public int FontSize;
        public ImFontPtr SymbolsFontPtr = null;

        protected DirectoryInfo CurrentOpenFolder;
        protected readonly Stack<DirectoryInfo> LastOpenendFolders = new();
        protected DirectoryInfo CurrentlySelectedFolder;
        protected readonly DirectoryInfo StartingFolder;

        protected const float FolderTextInputWidth = 350;
        protected const float FileTextInputWidth = 300;
        protected static float DriveSelectionWidth = 100;
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
        /// Checks input before returning if file exists.
        /// Disable this check for file saving.
        /// </summary>
        public bool NonExistingFilesAllowed = false;

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
        /// <param name="startingPath">Starting path, defaults to <see cref="AppContext.BaseDirectory"/></param>
        /// <param name="allowedExtensions">search filter with dot. Example (".json")</param>
        public ImGuiFilePicker(DirectoryInfo? startingPath = null, string allowedExtensions = "")
        {
            _filePickerCount++;

            if (startingPath == null || !startingPath.Exists)
            {
                startingPath = new DirectoryInfo(AppContext.BaseDirectory);
            }

            RootFolder = startingPath;
            CurrentOpenFolder = startingPath;
            StartingFolder = startingPath;
            CurrentlySelectedFolder = startingPath;

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

            // close on ESC
            if (ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                filePickerOpen = false;
            }

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
            if (ImGui.Button($"{ParentFolderTxt}##{_filePickerCount}", TopButtonSize))
            {
                if (CurrentOpenFolder.Exists && CurrentOpenFolder.Parent != null)
                {
                    LastOpenendFolders.Push(CurrentOpenFolder);
                    CurrentOpenFolder = CurrentOpenFolder.Parent;
                    SelectedFile = null;
                }
            }
            ImGui.SameLine();

            if (LastOpenendFolders.Count != 0)
            {
                if (ImGui.Button($"{BackTxt}##{_filePickerCount}", TopButtonSize))
                {

                    var lastFolder = LastOpenendFolders.Pop();
                    var lastDi = lastFolder;
                    if (lastDi.Exists)
                    {
                        CurrentOpenFolder = lastFolder;
                        SelectedFile = null;
                    }
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button($"{BackTxt}##{_filePickerCount}", TopButtonSize);
                ImGui.EndDisabled();
            }

            if (ShowNewFolderButton)
            {
                ImGui.SameLine();
                if (ImGui.Button($"{NewFolderButtonTxt}##{_filePickerCount}", TopButtonSize))
                {
                    _isNewFolderNameWindowOpen = true;
                }
            }

            if ((IntPtr)SymbolsFontPtr.NativePtr != IntPtr.Zero)
                ImGui.PopFont();

            ImGui.EndGroup();

            // Folder Selection
            var currentFolder = Environment.ExpandEnvironmentVariables(CurrentOpenFolder.FullName);
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
            var envCurrentFolder = Environment.ExpandEnvironmentVariables(currentFolder);
            if (!string.IsNullOrEmpty(envCurrentFolder))
            {
                var currentFolderFi = new FileInfo(envCurrentFolder); // parse something like folder and file (e. g. C:\test\test.las)

                if (Directory.Exists(envCurrentFolder) || envCurrentFolder.Contains(Path.DirectorySeparatorChar) || envCurrentFolder.Contains(Path.AltDirectorySeparatorChar))
                {
                    if (Directory.Exists(envCurrentFolder))
                    {
                        CurrentOpenFolder = new DirectoryInfo(envCurrentFolder);
                        CurrentlySelectedFolder = new DirectoryInfo(envCurrentFolder);
                    }


                    if (File.Exists(currentFolderFi.FullName) && !string.IsNullOrEmpty(currentFolderFi.DirectoryName))
                    {
                        CurrentOpenFolder = new DirectoryInfo(currentFolderFi.DirectoryName);
                        CurrentlySelectedFolder = new DirectoryInfo(currentFolderFi.DirectoryName);
                        SelectedFile = currentFolderFi;
                    }
                    else if (currentFolderFi.Extension == ".las") // only check if extension is given, print error only when no las file is found
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                        ImGui.BeginTooltip();
                        ImGui.TextColored(WarningTextColor, FileNotFoundTxt);
                        ImGui.EndTooltip();
                        ImGui.PopStyleVar();
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
                if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                {
                    if (ImGui.Selectable($"{drive.Name} {drive.DriveType}##{_filePickerCount}"))
                    {
                        RootFolder = new DirectoryInfo(drive.Name);
                        LastOpenendFolders.Push(CurrentOpenFolder);
                        CurrentOpenFolder = new DirectoryInfo(drive.Name);
                        SelectedFile = null;
                    }
                    driveCount++;
                }
            }
            ImGui.EndChild();
            ImGui.SameLine();

            if (ImGui.BeginChild($"#FolderBrowser##{_filePickerCount}", new Vector2(FolderTextInputWidth, BrowserHeight), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.HorizontalScrollbar))
            {
                if (CurrentOpenFolder != null && CurrentOpenFolder.Exists)
                {
                    var fileSystemEntries = GetFileSystemEntries(CurrentOpenFolder.FullName);
                    foreach (var fse in fileSystemEntries)
                    {
                        if (fse.Attributes.HasFlag(FileAttributes.Directory))
                        {
                            var directory = new DirectoryInfo(fse.FullName);

                            ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());

                            if (ImGui.Selectable(directory.Name + "/", CurrentlySelectedFolder == directory, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                SelectedFile = null;
                                CurrentlySelectedFolder = directory;
                                if (ImGui.IsMouseDoubleClicked(0))
                                {
                                    if (SelectedFile == null && ImGui.GetIO().WantCaptureMouse)
                                    {
                                        LastOpenendFolders.Push(CurrentOpenFolder);
                                        CurrentOpenFolder = directory;
                                    }
                                }
                            }

                            ImGui.PopStyleColor();
                        }
                        else
                        {
                            var name = fse.Name;

                            ImGui.PushStyleColor(ImGuiCol.Header, SelectedColor.ToUintColor());

                            if (ImGui.Selectable(name, SelectedFile?.Name == name, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                            {
                                if (ImGui.IsMouseDoubleClicked(0))
                                {
                                    if (SelectedFile != null && ImGui.GetIO().WantCaptureMouse)
                                    {
                                        if (HandlePickedFile(SelectedFile))
                                        {
                                            filePickerOpen = false;
                                            OnPicked?.Invoke(this, SelectedFile);
                                        }
                                    }
                                }
                                else
                                {
                                    if (SelectedFile == fse)
                                        SelectedFile = null;
                                    else
                                        SelectedFile = new FileInfo(fse.FullName);
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

            var selectedFile = SelectedFile?.Name ?? "";
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
                if (!string.IsNullOrEmpty(selectedFile) && !char.IsWhiteSpace(selectedFile[0]) && CurrentOpenFolder != null)
                    SelectedFile = new FileInfo(Path.Combine(CurrentOpenFolder.FullName, selectedFile));
            }

            if (_sizeOfInputText == Vector2.Zero)
                _sizeOfInputText = ImGui.GetItemRectSize();

            var sameLineOffset = WinSize.X - WindowPadding.X - (BottomButtonSize.X * 2 + ImGui.GetStyle().ItemSpacing.X * 4);
            if (SelectedFile != null)
            {
                var fi = SelectedFile;
                if (AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    ImGui.SameLine(sameLineOffset);
                    if (ImGui.Button($"{PickedFileTxt}##{_filePickerCount}", BottomButtonSize))
                    {
                        if (HandlePickedFile(fi))
                        {
                            OnPicked?.Invoke(this, fi);
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

            if (ShowNewFolderButton && IsNewFolderNameWindowOpen)
            {
                ImGui.SetNextWindowFocus();
                ImGui.Begin($"{CreateNewFolderTxt}##{_filePickerCount}", ref _isNewFolderNameWindowOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking);
                ImGui.InputTextWithHint($"", $"{CreateNewFolderHintTxt}", ref _newFolderName, 400, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
                {
                    var arr = _newFolderName.ToCharArray();

                    if (x->SelectionStart < x->SelectionEnd && x->SelectionStart >= 0 && x->SelectionEnd <= arr.Length)
                    {
                        var selectedText = arr[x->SelectionStart..x->SelectionEnd];
                        if (selectedText != null)
                            ImGuiInputImp.CurrentlySelectedText = new string(selectedText);
                    }

                    return 0;
                });
                ImGui.SameLine();

                if (ImGui.Button($"{CreateFolderTxt}"))
                {
                    if (!string.IsNullOrEmpty(_newFolderName))
                    {
                        try
                        {
                            Directory.CreateDirectory(Path.Combine(currentFolder, _newFolderName));
                        }
                        catch (Exception ex)
                        {
                            _createFolderException = ex;
                            return;

                        }
                    }
                    IsNewFolderNameWindowOpen = false;
                }

                // display a possible exception during folder creation as a tooltip text
                if (_createFolderException != null)
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                    var size = ImGui.CalcTextSize(_createFolderException?.Message);
                    ImGui.SetNextWindowSize(new Vector2(size.X / 4, -1));
                    ImGui.BeginTooltip();
                    ImGui.PushStyleColor(ImGuiCol.Text, WarningTextColor);
                    ImGui.TextWrapped(_createFolderException?.Message);
                    ImGui.PopStyleColor();
                    ImGui.EndTooltip();
                    ImGui.PopStyleVar();
                }

                ImGui.End();
            }

            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor();

            return;
        }

        /// <summary>
        /// We differentiate between files and folders, as we want to print the folders first
        /// If we collect everything in one list all files and folders are being sorted alphabetically
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private List<FileSystemInfo> GetFileSystemEntries(string fullName)
        {
            var folders = new List<DirectoryInfo>();
            var files = new List<FileInfo>();

            foreach (var f in Directory.GetFileSystemEntries(fullName, ""))
            {
                var attr = File.GetAttributes(f);
                // skip unaccessible files and folders
                if (attr.HasFlag(FileAttributes.Encrypted)
                    || attr.HasFlag(FileAttributes.Hidden)
                    || attr.HasFlag(FileAttributes.System)
                    || attr.HasFlag(FileAttributes.Temporary))
                    continue;

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    folders.Add(new DirectoryInfo(f));
                }
                else
                {
                    var fse = new FileInfo(f);
                    if (AllowedExtensions != null)
                    {
                        var ext = fse.Extension;
                        if (AllowedExtensions.Contains(ext))
                            files.Add(fse);
                    }
                    else
                    {
                        files.Add(fse);
                    }
                }
            }

            var ret = new List<FileSystemInfo>(folders);
            ret.AddRange(files);
            return ret;
        }

        protected virtual bool HandlePickedFile(FileInfo selectedFile)
        {

            if (selectedFile.Directory != null)
            {
                if (NonExistingFilesAllowed || selectedFile.Exists)
                {
                    CurrentOpenFolder = selectedFile.Directory;
                    SelectedFile = selectedFile;
                    return true;
                }
                else if (selectedFile != null)
                {
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
                    ImGui.BeginTooltip();
                    ImGui.TextColored(WarningTextColor, FileNotFoundTxt);
                    ImGui.EndTooltip();
                    ImGui.PopStyleVar();

                    return false;
                }
            }


            return false;
        }
    }
}