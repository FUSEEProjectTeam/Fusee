using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
// ReSharper disable once InconsistentNaming
    class GUI
    {
        private readonly GameWorld _gw;
        private readonly GUIHandler _guiHandler;

        private const bool DebugFlag = true;
        private GUIText _debug;

        private IFont _fontSmall;
        private IFont _fontMedium;

        private readonly GUIPanel _startPanel;
        private readonly GUIButton _startPanelButtonStart;
        private readonly GUIButton _startPanelButtonStuff;

        private readonly float4 _color1 = new float4(0.8f, 0.1f, 0.1f, 1);
        private readonly float4 _color2 = new float4(0, 0, 0, 1);

        public GUI(RenderContext rc, GameWorld gw)
        {
            //Basic Init
            _gw = gw;

            _fontSmall = rc.LoadFont("Assets/Cabin.ttf", 12);
            _fontMedium = rc.LoadFont("Assets/Cabin.ttf", 18);

            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(rc);

            _debug = new GUIText("Bla", _fontSmall, 170, 20);

            //Start Pannel Init

            _startPanel = new GUIPanel("Titelbla", _fontMedium, 10, 10, 150, 110);
            _startPanelButtonStart = new GUIButton("Start", _fontSmall, 10, 30, 130, 30);
            _startPanelButtonStart.OnGUIButtonDown += OnGUIButtonDown;
            _startPanelButtonStart.OnGUIButtonUp += OnGUIButtonUp;
            _startPanelButtonStart.OnGUIButtonEnter += OnGUIButtonEnter;
            _startPanelButtonStart.OnGUIButtonLeave += OnGUIButtonLeave;
            _startPanelButtonStuff = new GUIButton("Stuff", _fontSmall, 10, 70, 130, 30);
            _startPanelButtonStuff.OnGUIButtonDown += OnGUIButtonDown;
            _startPanelButtonStuff.OnGUIButtonUp += OnGUIButtonUp;
            _startPanelButtonStuff.OnGUIButtonEnter += OnGUIButtonEnter;
            _startPanelButtonStuff.OnGUIButtonLeave += OnGUIButtonLeave;

            _startPanel.ChildElements.Add(_startPanelButtonStart);
            _startPanel.ChildElements.Add(_startPanelButtonStuff);

            ShowStartGUI();
        }

        private void OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;

            if (sender == _startPanelButtonStart)
            {
                _gw.StartGame();
            }
            else if (sender == _startPanelButtonStuff)
            {
                SetDebugMsg("Stuff");
                _guiHandler.Clear();
            }
        }
        
        private void OnGUIButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
        }

        private void OnGUIButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            sender.TextColor = _color1;
        }

        private void OnGUIButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.TextColor = _color2;
        }

        public void Render()
        {
            _guiHandler.RenderGUI();
        }

        public void SetDebugMsg(string debugMsg)
        {
            //_guiHandler.Remove(_debug);
            //_debug = new GUIText(debugMsg, _fontSmall, 170, 20);
            //_guiHandler.Add(_debug);
        }

        public void ShowStartGUI()
        {
            _guiHandler.Clear();
            _guiHandler.Add(_startPanel);
            if (DebugFlag) _guiHandler.Add(_debug);
        }
    }
}
