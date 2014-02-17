using System;
using System.Drawing;
using System.Windows.Forms;
using Examples.WinFormsFusee.Properties;
using Fusee.Engine;

namespace Examples.WinFormsFusee
{
    public partial class MainWindow : Form
    {
        // private RenderControl _renderControl;

        private int _currentInx;
        private RenderCanvas _currentApp;
        private RenderControl _currentControl;
        private WinformsHost _currentHost;

        private readonly ApplicationFinder _appFinder;

        public MainWindow()
        {
            InitializeComponent();

            _appFinder = new ApplicationFinder();
            _appFinder.PerformSearch();

            PopulateApplist();
        }

        private void PopulateApplist()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < _appFinder.Length; i++)
                listBox1.Items.Add(_appFinder.GetNameAt(i));
            _currentInx = -1;
        }

        private void StartCurrentApp()
        {
            Width = 1440;
            Height = 602;

            //
            //  STEP ONE - Create the Winforms Control
            //
            _currentControl = new RenderControl
            {
                BackColor = Color.Black,
                Location = new System.Drawing.Point(0, 0),
                Size = splitContainer1.Panel2.Size,
                Dock = DockStyle.Fill,
                Name = "RenderControl",
                TabIndex = 0
            };

            _currentControl.HandleCreated += renderControl_HandleCreated; // <- This is crucial: Prepare for STEP TWO.

            splitContainer1.Panel2.Controls.Add(_currentControl);
        }

        private void renderControl_HandleCreated(object sender, EventArgs e)
        {
            //
            //  STEP TWO - Now the underlying Windows Window was created - we can hook OpenGL on it.
            //

            // Take this as an example how to hook up any FUSEE application on a given Winforms form:

            // First create a WinformsHost around the control
            _currentHost = new WinformsHost(_currentControl, this);

            // Then instantiate your app (could be as well _currentApp = new MyOwnRenderCanvasDerivedClass(); )
            _currentApp = _appFinder.Instantiate(_currentInx);

            // Now use the host as the canvas AND the input implementation of your App
            _currentApp.CanvasImplementor = _currentHost;
            _currentApp.InputImplementor = _currentHost;

            // Then you can run the app
            _currentApp.Run();

            // If not already done, show the window.
            _currentControl.Show();
        }

        private void CloseCurrentApp()
        {
            richTextBox1.Text = "";

            // Clean up
            _currentApp = null;

            if (_currentControl != null)
            {
                _currentControl.HandleCreated -= renderControl_HandleCreated;
                splitContainer1.Panel2.Controls.Remove(_currentControl);
                _currentControl.Dispose();
                _currentControl = null;
            }
            if (_currentHost != null)
            {
                _currentHost.Dispose();
                _currentHost = null;
            }

            // Just in case...
            GC.Collect();
            GC.WaitForFullGCComplete();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentInx != listBox1.SelectedIndex)
            {
                CloseCurrentApp();
                _currentInx = listBox1.SelectedIndex;
                StartCurrentApp();
                richTextBox1.Text = "";
                var heading = new Font(richTextBox1.Font, FontStyle.Bold | FontStyle.Italic);
                richTextBox1.SelectionFont = heading;
                richTextBox1.AppendText("File\r\n");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText(_appFinder.GetFileNameAt(_currentInx));
                richTextBox1.SelectionFont = heading;
                richTextBox1.AppendText("\r\n\r\nDescription\r\n");
                richTextBox1.SelectionFont = richTextBox1.Font;
                richTextBox1.AppendText(_appFinder.GetDescriptionAt(_currentInx));
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {
            var folderBrowser = new FolderBrowserDialog
            {
                SelectedPath = _appFinder.SearchRoot,
                ShowNewFolderButton = false,
                Description = Resources
                    .MainWindow_label1_Click_1_Select_the_directory_where_FUSEE_App_browser_should_look_for_Apps__Subdirectories_are_included_in_the_search_
            };

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                CloseCurrentApp();
                _appFinder.SearchRoot = folderBrowser.SelectedPath;
                PopulateApplist();
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics,
                    e.Font,
                    e.Bounds,
                    e.Index,
                    e.State ^ DrawItemState.Selected,
                    e.ForeColor,
                    Color.FromArgb(0, 156, 121)); //Choose the color

                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Draw the current item text
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds,
                    StringFormat.GenericDefault);
            }
            else
            {
                // Draw the background of the ListBox control for each item.
                e.DrawBackground();
                // Draw the current item text
                e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), e.Font, Brushes.Black, e.Bounds,
                    StringFormat.GenericDefault);
            }

            // If the ListBox has focus, draw a focus rectangle around the selected item.
            // e.DrawFocusRectangle();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseCurrentApp();
        }

        public override bool PreProcessMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case 0x100: //WM_KEYDOWN
                    return false;
            }
            return base.PreProcessMessage(ref msg);
        }

        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}