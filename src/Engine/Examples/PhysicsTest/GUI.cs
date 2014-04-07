using Fusee.Engine;
using Fusee.Math;

namespace Examples.PhysicsTest
{
    // ReSharper disable once InconsistentNaming
    class GUI
    {
        private readonly GUIHandler _guiHandler;

        private IFont _fontSmall;
        private IFont _fontMedium;
        private IFont _fontBig;

        private readonly GUIPanel _guiPanel;

        private GUIText _fps, _numberRb, _shapeType, _shapes, _info1, _info2, _info3;

        private readonly float4 _color1 = new float4(1f, 1f, 1f, 1);
        private readonly float4 _color2 = new float4(0, 0, 0, 1);

        public GUI(RenderContext rc)
        {
            //Basic Init

            _fontSmall = rc.LoadFont("Assets/Cabin.ttf", 12);
            _fontMedium = rc.LoadFont("Assets/Cabin.ttf", 18);
            _fontBig = rc.LoadFont("Assets/Cabin.ttf", 40);

            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(rc);

            //Start Pannel Init
            _guiPanel = new GUIPanel("PhysicsTest", _fontMedium, 10, 10, 330, 250);


            _info1 = new GUIText("Über die Tasten des Nummernblocks  " , _fontMedium, 30, 170, _color2);
            _info2 = new GUIText("(1-4) kann zwischen 4 verschiedenen ", _fontMedium, 30, 190, _color2);
            _info3 = new GUIText("Szenen gewechselt werden", _fontMedium, 30, 210, _color2);
            _fps = new GUIText("FPS", _fontMedium, 30, 55, _color2);
            _numberRb = new GUIText("Rigidbodies: ", _fontMedium, 30, 85, _color2);
            _shapeType = new GUIText("Collision Shapes: ", _fontMedium, 30, 115, _color2);
            _shapes = new GUIText("", _fontMedium, 50, 135, _color2);

            _guiPanel.ChildElements.Add(_info1);
            _guiPanel.ChildElements.Add(_info2);
            _guiPanel.ChildElements.Add(_info3);
            _guiPanel.ChildElements.Add(_fps);
            _guiPanel.ChildElements.Add(_numberRb);
            _guiPanel.ChildElements.Add(_shapeType);
            _guiPanel.ChildElements.Add(_shapes);

            ShowGUI();
        }


        public void SetUp(int numRb, string shapes)
        {
            _numberRb.Text = "Rigidbodies: " + numRb;
            _shapes.Text =  shapes;
        }

        public void Render(float fps)
        {
            _fps.Text = "FPS: " + fps;
            _guiHandler.RenderGUI();
        }

        public void UpdateScore()
        {
            

            //update framrate show

        }


        public void ShowGUI()
        {
            _guiHandler.Clear();
            
            _guiHandler.Add(_guiPanel);
        }


        public void Resize()
        {
            _guiHandler.Refresh();
        }
    }
}
