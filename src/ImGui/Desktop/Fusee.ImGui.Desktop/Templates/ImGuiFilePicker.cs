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
        /// Allow resizing of file picker window
        /// </summary>
        public bool AllowFilePickerResize { get; set; } = true;

        /// <summary>
        /// Allow resizing of new folder window
        /// </summary>
        public bool AllowNewFolderResize { get; set; } = true;

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
        public bool ShowNewFolderButton { get; set; }

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

        protected readonly Vector2 WindowPadding = new(15, 15);
        protected readonly Vector2 BottomButtonSize = new(55, 26);
        protected readonly Vector2 TopButtonSize = new(35, 30);

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

            // close on ESC
            if (ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                filePickerOpen = false;
            }

            if (DoFocusPicker)
                ImGui.SetNextWindowFocus();

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, WindowPadding);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, _windowBackgroundUint);

            // Begin window
            ImGui.SetNextWindowSizeConstraints(new Vector2(500, 300), ImGui.GetWindowViewport().Size * 0.75f);
            var allowResizeFlag = AllowFilePickerResize ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoResize;
            ImGui.Begin(Id, ref filePickerOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | allowResizeFlag);

            // draw navigation buttons and folder selection on the same line
            DrawNavButtons();
            DrawFolderSelectionTextInput();

            // draw drive and file selector window
            ImGui.NewLine();
            ImGui.PushStyleColor(ImGuiCol.ChildBg, FileSelectionMenuBackground.ToUintColor());
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

            DrawDriveSelector();
            DrawFolderSelector(ref filePickerOpen);

            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            // draw okay, cancel button and file selector
            ImGui.NewLine();
            DrawFileSelector(ref filePickerOpen);

            ImGui.End();


            if (ShowNewFolderButton && IsNewFolderNameWindowOpen)
            {
                DrawNewFolderOverlay(CurrentOpenFolder);
            }

            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor();
        }

        private unsafe void DrawNavButtons()
        {
            if ((IntPtr)SymbolsFontPtr.NativePtr != IntPtr.Zero)
                ImGui.PushFont(SymbolsFontPtr);

            ImGui.BeginGroup();
            var parentFolderButtonSize = ImGui.CalcTextSize(ParentFolderTxt) + ImGui.GetStyle().FramePadding * 2;
            var backButtonSize = ImGui.CalcTextSize(ParentFolderTxt) + ImGui.GetStyle().FramePadding * 2;
            var newFolderButtonSize = ImGui.CalcTextSize(NewFolderButtonTxt) + ImGui.GetStyle().FramePadding * 2;

            parentFolderButtonSize += new Vector2(5, 0); // add a little offset as the arrows aren't wide enough
            backButtonSize += new Vector2(5, 0); // add a little offset as the arrows aren't wide enough

            if (ImGui.Button($"{ParentFolderTxt}##{_filePickerCount}", parentFolderButtonSize))
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

                if (ImGui.Button($"{BackTxt}##{_filePickerCount}", backButtonSize))
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
                ImGui.Button($"{BackTxt}##{_filePickerCount}", backButtonSize);
                ImGui.EndDisabled();
            }

            if (ShowNewFolderButton)
            {
                ImGui.SameLine();
                if (ImGui.Button($"{NewFolderButtonTxt}##{_filePickerCount}", newFolderButtonSize))
                {
                    _isNewFolderNameWindowOpen = true;
                }
            }

            if ((IntPtr)SymbolsFontPtr.NativePtr != IntPtr.Zero)
                ImGui.PopFont();

            ImGui.EndGroup();
        }

        private unsafe void DrawFolderSelectionTextInput()
        {
            var currentFolder = Environment.ExpandEnvironmentVariables(CurrentOpenFolder.FullName);

            ImGui.SameLine();
            // occupy the max available space, minus the label text length
            ImGui.SetNextItemWidth(-ImGui.CalcTextSize(FolderLabelTxt).X);
            ImGui.InputTextWithHint($"{FolderLabelTxt}##{_filePickerCount}", PathToFolderTxt, ref currentFolder, 4098, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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
        }

        private void DrawDriveSelector()
        {
            var driveSelectionWidth = ImGui.GetWindowSize().X * 0.25f; // 25% of windowSize.x
            // take all space in y, however shrink in y in item height + standard padding + WindowPadding
            var offsetFromBottom = ImGui.CalcTextSize(PickedFileTxt) + ImGui.GetStyle().FramePadding * 2 + ImGui.GetStyle().WindowPadding * 2;
            ImGui.BeginChild($"DriveSelection##{_filePickerCount}", new Vector2(driveSelectionWidth, -offsetFromBottom.Y), false, ImGuiWindowFlags.AlwaysUseWindowPadding);

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
        }

        private void DrawFolderSelector(ref bool filePickerOpen)
        {
            ImGui.SameLine();
            ImGui.GetWindowHeight();
            // take all space in y, however shrink in y in item height + standard padding + WindowPadding
            var offsetFromBottom = ImGui.CalcTextSize(PickedFileTxt) + ImGui.GetStyle().FramePadding * 2 + ImGui.GetStyle().WindowPadding * 2;
            if (ImGui.BeginChild($"#FolderBrowser##{_filePickerCount}", new Vector2(-1, -offsetFromBottom.Y), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.HorizontalScrollbar))
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
            }
            ImGui.EndChild();
        }

        private unsafe void DrawFileSelector(ref bool filePickerOpen)
        {
            var pickedFileButtonSize = ImGui.CalcTextSize(PickedFileTxt) + ImGui.GetStyle().FramePadding * 2;
            var cancelFileButtonSize = ImGui.CalcTextSize(CancelFileOpenTxt) + ImGui.GetStyle().FramePadding * 2;

            var selectedFile = SelectedFile?.Name ?? "";
            // take all available space minus the label text and minus both buttons
            var inputTextMaxLength = ImGui.CalcTextSize(FileLabelTxt).X + ImGui.GetStyle().ItemInnerSpacing.X * 4 + pickedFileButtonSize.X + cancelFileButtonSize.X;
            ImGui.SetNextItemWidth(-inputTextMaxLength);

            if (ImGui.InputTextWithHint(FileLabelTxt, FileInputHintTxt, ref selectedFile, 4096, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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

            // increase spacing between okay button and input
            var normalSpacing = ImGui.GetStyle().ItemSpacing;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(15, normalSpacing.Y));

            if (SelectedFile != null)
            {
                var fi = SelectedFile;
                if (AllowedExtensions != null && AllowedExtensions.Contains(fi.Extension))
                {
                    ImGui.SameLine();

                    if (ImGui.Button($"{PickedFileTxt}##{_filePickerCount}", pickedFileButtonSize) ||
                        (ImGui.IsKeyReleased(ImGuiKey.Enter) && !IsNewFolderNameWindowOpen))
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
                    ImGui.SameLine();
                    ImGui.BeginDisabled();
                    ImGui.Button(PickedFileTxt, pickedFileButtonSize);
                    ImGui.EndDisabled();
                }
            }
            else
            {
                ImGui.SameLine();
                ImGui.BeginDisabled();
                ImGui.Button(PickedFileTxt, pickedFileButtonSize);
                ImGui.EndDisabled();
            }

            ImGui.PopStyleVar();

            ImGui.SameLine();
            if (ImGui.Button($"{CancelFileOpenTxt}##{_filePickerCount}", cancelFileButtonSize))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                filePickerOpen = false;
            }
        }

        private unsafe void DrawNewFolderOverlay(DirectoryInfo currentFolder)
        {
            ImGui.SetNextWindowFocus();
            // Calculate min height with button size
            var createFolderButtonSize = ImGui.CalcTextSize(CreateFolderTxt) + ImGui.GetStyle().FramePadding * 2;
            var minWindowHeight = createFolderButtonSize.Y + ImGui.GetStyle().WindowPadding.Y * 4;
            var minWindowLength = createFolderButtonSize.X + ImGui.CalcTextSize(CreateNewFolderHintTxt).X + ImGui.GetStyle().FramePadding.X * 4 + ImGui.GetStyle().ItemSpacing.X * 4;
            ImGui.SetNextWindowSizeConstraints(new Vector2(minWindowLength, minWindowHeight), new Vector2(ImGui.GetWindowViewport().Size.X * 0.5f, minWindowHeight));
            ImGui.SetNextItemWidth(minWindowLength + ImGui.GetStyle().WindowPadding.X);

            var allowResizeFlag = AllowNewFolderResize ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoResize;
            ImGui.Begin($"{CreateNewFolderTxt}##{_filePickerCount}", ref _isNewFolderNameWindowOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | allowResizeFlag);

            // take the full width minus the button size
            ImGui.SetNextItemWidth(-createFolderButtonSize.X);
            ImGui.InputTextWithHint($"", $"{CreateNewFolderHintTxt}", ref _newFolderName, 4096, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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

            if (ImGui.Button($"{CreateFolderTxt}", createFolderButtonSize) ||
                ImGui.IsKeyReleased(ImGuiKey.Enter))
            {
                if (!string.IsNullOrEmpty(_newFolderName))
                {
                    var folderName = string.Empty;
                    try
                    {
                        if (Path.IsPathRooted(_newFolderName))
                        {
                            folderName = _newFolderName;
                            Directory.CreateDirectory(_newFolderName);
                        }
                        else
                        {
                            folderName = Path.Combine(currentFolder.FullName, _newFolderName);
                            Directory.CreateDirectory(folderName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _createFolderException = ex;
                        return;

                    }

                    // open new folder
                    CurrentlySelectedFolder = new DirectoryInfo(folderName);
                    LastOpenendFolders.Push(CurrentOpenFolder);
                    CurrentOpenFolder = new DirectoryInfo(folderName);
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