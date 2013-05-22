using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace WinForms
{
    public partial class MainWindow : Form
    {
        private System.Windows.Forms.NativeWindow _child;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _child = new System.Windows.Forms.NativeWindow();
 
            
            var app = new WinFormsFusee(_child.Handle);
            app.Init();
 
            Application.Idle += delegate(object idleSender, EventArgs ea)
            {
                app.RenderAFrame();
            };
       
            // app.Run();
            // MessageBox.Show("Knopf gedrückt");
        }

    }
}


// Links
// http://www.opentk.com/node/2062
// create and attach a GraphicsContext to any window you like.

