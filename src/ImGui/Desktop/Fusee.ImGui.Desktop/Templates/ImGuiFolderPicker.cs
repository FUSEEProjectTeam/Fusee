using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Fusee.ImGuiImp.Desktop.Templates
{
    public class ImGuiFolderPicker
    {
        /// <summary>
        /// Invoked on clicked "open".
        /// </summary>
        public EventHandler<DirectoryInfo>? OnPicked;

        /// <summary>
        /// Invoked on cancel.
        /// </summary>
        public EventHandler? OnCancel;

        /// <summary>
        /// Allow resizing of file picker window
        /// </summary>
        public bool AllowFolderPickerResize { get; set; } = true;

        /// <summary>
        /// Allow resizing of new folder window
        /// </summary>
        public bool AllowNewFolderResize { get; set; } = true;

        /// <summary>
        /// Title of window (visible in top bar).
        /// </summary>
        public string Id = "Open Folder";

        /// <summary>
        /// Caption of the "Open" button.
        /// </summary>
        public string PickedFolderTxt = "Open";

        /// <summary>
        /// Caption of the "Cancel" button.
        /// </summary>
        public string CancelFolderOpenTxt = "Cancel";

        /// <summary>
        /// Path to folder text.
        /// </summary>
        public string PathToFolderTxt = "Path to folder";

        /// <summary>
        /// Folder not found warning text.
        /// </summary>
        public string FolderNotFoundTxt = "Folder not found!";

        /// <summary>
        /// Caption of folder input text
        /// </summary>
        public string FolderLabelTxt = "Folder";

        /// <summary>
        /// Caption of folder input text (bottom)
        /// </summary>
        public string SelectedFolderLabelTxt = "Folder";

        public string ParentFolderTxt = "Parent";
        public string BackTxt = "Back";

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

        public DirectoryInfo? SelectedFolder { get; protected set; }
        public DirectoryInfo RootFolder { get; protected set; }

        public int FontSize;
        public ImFontPtr SymbolsFontPtr = null;

        protected DirectoryInfo CurrentOpenFolder;
        protected readonly Stack<DirectoryInfo> LastOpenendFolders = new();
        protected DirectoryInfo? CurrentlySelectedFolder;
        protected readonly DirectoryInfo StartingFolder;

        protected readonly Vector2 WindowPadding = new(15, 15);
        protected readonly Vector2 BottomButtonSize = new(55, 26);
        protected readonly Vector2 TopButtonSize = new(35, 30);

        protected bool DoFocusPicker = true;

        private static int _folderPickerCount = 0;

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
        public Vector4 FolderSelectionMenuBackground = new(125, 125, 125, 255);

        /// <summary>
        /// Color of <see cref="ImGui.SetTooltip(string)"/> when an error occurs
        /// </summary>
        public Vector4 WarningTextColor = new(200, 0, 0, 255);

        /// <summary>
        /// Background color of one <see cref="ImGui.Selectable(string)"/> object
        /// </summary>
        public Vector4 SelectedColor = new(125, 75, 75, 255);

        /// <summary>
        /// Color of one <see cref="ImGui.Selectable(string)"/> file object
        /// This should be a lighter color, as these elements are being printed, but are not selectable
        /// </summary>
        public Vector4 LightFileColor = new(125, 125, 125, 255);

        /// <summary>
        /// Generate a new ImGuiFolderPicker instance
        /// </summary>
        /// <param name="startingPath">Starting path, defaults to <see cref="AppContext.BaseDirectory"/></param>
        public ImGuiFolderPicker(DirectoryInfo? startingPath = null)
        {
            _folderPickerCount++;

            if (startingPath == null || !startingPath.Exists)
            {
                startingPath = new DirectoryInfo(AppContext.BaseDirectory);
            }

            RootFolder = startingPath;
            CurrentOpenFolder = startingPath;
            StartingFolder = startingPath;
            SelectedFolder = startingPath;
        }


        public virtual unsafe void Draw(ref bool folderPickerOpen)
        {
            IsOpen = folderPickerOpen;
            if (!folderPickerOpen) return;

            // close on ESC
            if (ImGui.IsKeyReleased(ImGuiKey.Escape))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                folderPickerOpen = false;
            }

            if (DoFocusPicker)
                ImGui.SetNextWindowFocus();

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, WindowPadding);
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, _windowBackgroundUint);

            // Begin window
            ImGui.SetNextWindowSizeConstraints(new Vector2(500, 300), ImGui.GetWindowViewport().Size * 0.75f);
            var allowResizeFlag = AllowFolderPickerResize ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoResize;
            ImGui.Begin(Id, ref folderPickerOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | allowResizeFlag);

            // draw navigation buttons and folder selection on the same line
            DrawNavButtons();
            DrawFolderSelectionTextInput();

            // draw drive and file selector window
            ImGui.NewLine();
            ImGui.PushStyleColor(ImGuiCol.ChildBg, FolderSelectionMenuBackground.ToUintColor());
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(10, 10));

            DrawDriveSelector();
            DrawFolderSelector(ref folderPickerOpen);

            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            // draw okay, cancel button
            ImGui.NewLine();
            DrawFolderSelectorButtons(ref folderPickerOpen);

            ImGui.End();


            if (ShowNewFolderButton && IsNewFolderNameWindowOpen)
            {
                DrawNewFolderOverlay(CurrentOpenFolder);
            }

            ImGui.PopStyleVar(2);
            ImGui.PopStyleColor();

            return;
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

            if (ImGui.Button($"{ParentFolderTxt}##{_folderPickerCount}", parentFolderButtonSize))
            {
                if (CurrentOpenFolder.Exists && CurrentOpenFolder.Parent != null)
                {
                    LastOpenendFolders.Push(CurrentOpenFolder);
                    CurrentOpenFolder = CurrentOpenFolder.Parent;
                }
            }
            ImGui.SameLine();

            if (LastOpenendFolders.Count != 0)
            {
                if (ImGui.Button($"{BackTxt}##{_folderPickerCount}", backButtonSize))
                {

                    var lastFolder = LastOpenendFolders.Pop();
                    if (lastFolder.Exists)
                    {
                        CurrentOpenFolder = lastFolder;
                    }
                }
            }
            else
            {
                ImGui.BeginDisabled();
                ImGui.Button($"{BackTxt}##{_folderPickerCount}", backButtonSize);
                ImGui.EndDisabled();
            }

            if (ShowNewFolderButton)
            {
                ImGui.SameLine();
                if (ImGui.Button($"{NewFolderButtonTxt}##{_folderPickerCount}", newFolderButtonSize))
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
            // Folder Selection
            var currentFolder = Environment.ExpandEnvironmentVariables(CurrentOpenFolder.FullName);

            ImGui.SameLine();
            // occupy the max available space, minus the label text length
            ImGui.SetNextItemWidth(-ImGui.CalcTextSize(FolderLabelTxt).X);
            ImGui.InputTextWithHint($"{FolderLabelTxt}##{_folderPickerCount}", PathToFolderTxt, ref currentFolder, 4098, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.CallbackAlways, (x) =>
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

            if (Directory.Exists(envCurrentFolder))
            {
                CurrentOpenFolder = new DirectoryInfo(envCurrentFolder);
                CurrentlySelectedFolder = new DirectoryInfo(envCurrentFolder);
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

        private void DrawDriveSelector()
        {
            // take all space in y, however shrink in y in item height + standard padding + WindowPadding
            var offsetFromBottom = ImGui.CalcTextSize(PickedFolderTxt) + ImGui.GetStyle().FramePadding * 2 + ImGui.GetStyle().WindowPadding * 2;
            var driveSelectionWidth = ImGui.GetWindowSize().X * 0.25f; // 25% of windowSize.x

            ImGui.BeginChild($"DriveSelection##{_folderPickerCount}", new Vector2(driveSelectionWidth, -offsetFromBottom.Y), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize);
            // Drive Selection
            var driveCount = 0;
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && (drive.DriveType == DriveType.Fixed || drive.DriveType == DriveType.Removable))
                {
                    if (ImGui.Selectable($"{drive.Name} {drive.DriveType}##{_folderPickerCount}"))
                    {
                        RootFolder = new DirectoryInfo(drive.Name);
                        LastOpenendFolders.Push(CurrentOpenFolder);
                        CurrentOpenFolder = new DirectoryInfo(drive.Name);
                    }
                    driveCount++;
                }
            }
            ImGui.EndChild();
        }

        private void DrawFolderSelector(ref bool filePickerOpen)
        {

            ImGui.SameLine();
            // take all space in y, however shrink in y in item height + standard padding + WindowPadding
            var offsetFromBottom = ImGui.CalcTextSize(PickedFolderTxt) + ImGui.GetStyle().FramePadding * 2 + ImGui.GetStyle().WindowPadding * 2;
            if (ImGui.BeginChild($"#FolderBrowser##{_folderPickerCount}", new Vector2(-1, -offsetFromBottom.Y), false, ImGuiWindowFlags.AlwaysUseWindowPadding | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.HorizontalScrollbar))
            {
                var fileSystemEntries = GetFileSystemEntries(CurrentOpenFolder.FullName);
                foreach (var fse in fileSystemEntries)
                {
                    var name = fse.Name;

                    if (fse.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, FolderColor.ToUintColor());
                        ImGui.PushStyleColor(ImGuiCol.Header, SelectedColor.ToUintColor());
                        if (ImGui.Selectable(name + "/", CurrentlySelectedFolder?.Name == name, ImGuiSelectableFlags.DontClosePopups | ImGuiSelectableFlags.AllowDoubleClick))
                        {
                            if (ImGui.IsMouseDoubleClicked(0))
                            {
                                if (ImGui.GetIO().WantCaptureMouse)
                                {
                                    CurrentlySelectedFolder = new DirectoryInfo(fse.FullName);
                                    LastOpenendFolders.Push(CurrentOpenFolder);
                                    CurrentOpenFolder = new DirectoryInfo(fse.FullName);
                                }
                            }
                        }
                        ImGui.PopStyleColor();
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        // just print the files, but with lighter color
                        ImGui.PushStyleColor(ImGuiCol.Text, LightFileColor.ToUintColor());
                        ImGui.Selectable(name, false, ImGuiSelectableFlags.DontClosePopups);
                        ImGui.PopStyleColor();
                    }
                }

            }
            ImGui.EndChild();
        }

        private void DrawFolderSelectorButtons(ref bool filePickerOpen)
        {
            var pickedFileButtonSize = ImGui.CalcTextSize(PickedFolderTxt) + ImGui.GetStyle().FramePadding * 2;
            var cancelFileButtonSize = ImGui.CalcTextSize(CancelFolderOpenTxt) + ImGui.GetStyle().FramePadding * 2;

            ImGui.BeginChild($"FolderSelector##{_folderPickerCount}", new Vector2(-1, -1), false, ImGuiWindowFlags.AlwaysAutoResize);

            // take all available window space minus the minus both buttons
            // push buttons therefore to the right
            var dummyMaxLength = ImGui.GetWindowSize().X - (ImGui.GetStyle().ItemInnerSpacing.X * 4 + pickedFileButtonSize.X + cancelFileButtonSize.X);
            ImGui.Dummy(new Vector2(dummyMaxLength, -1));
            if (CurrentlySelectedFolder != null && CurrentlySelectedFolder.Exists)
            {
                ImGui.SameLine();

                if (ImGui.Button($"{PickedFolderTxt}##{_folderPickerCount}", pickedFileButtonSize) ||
                    (ImGui.IsKeyReleased(ImGuiKey.Enter) && !IsNewFolderNameWindowOpen))
                {
                    if (CurrentlySelectedFolder != null)
                        OnPicked?.Invoke(this, CurrentlySelectedFolder);
                    else
                        OnPicked?.Invoke(this, CurrentOpenFolder);
                    filePickerOpen = false;
                }
            }
            else
            {
                ImGui.SameLine();
                ImGui.BeginDisabled();
                ImGui.Button(PickedFolderTxt, pickedFileButtonSize);
                ImGui.EndDisabled();
            }

            ImGui.SameLine();
            if (ImGui.Button($"{CancelFolderOpenTxt}##{_folderPickerCount}", cancelFileButtonSize))
            {
                OnCancel?.Invoke(this, EventArgs.Empty);
                filePickerOpen = false;
            }

            ImGui.EndChild();
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
            ImGui.Begin($"{CreateNewFolderTxt}##{_folderPickerCount}", ref _isNewFolderNameWindowOpen, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoCollapse | allowResizeFlag);

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
        private static List<FileSystemInfo> GetFileSystemEntries(string fullName)
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
                    files.Add(fse);
                }
            }

            var ret = new List<FileSystemInfo>(folders);
            ret.AddRange(files);
            return ret;

        }
    }
}