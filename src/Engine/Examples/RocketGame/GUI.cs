using Fusee.Engine;
using Fusee.Math;

namespace Examples.RocketGame
{
    // ReSharper disable once InconsistentNaming
    class GUI
    {
        private readonly GameWorld _gw;
        private readonly GUIHandler _guiHandler;

        private IFont _fontSmall;
        private IFont _fontMedium;
        private IFont _fontBig;

        private readonly GUIPanel _startPanel1;
        private readonly GUIButton _startPanelButtonStart;
        private readonly GUIButton _startPanelButtonStuff;
        private readonly GUIPanel _startPanel2;

        private readonly GUIPanel _playPanel;
        private GUIText _playScore;

        private readonly GUIText _overText;

        private readonly float4 _color1 = new float4(0.8f, 0.1f, 0.1f, 1);
        private readonly float4 _color2 = new float4(0, 0, 0, 1);

        public GUI(RenderContext rc, GameWorld gw)
        {
            //Basic Init
            _gw = gw;

            _fontSmall = rc.LoadFont("Assets/Cabin.ttf", 12);
            _fontMedium = rc.LoadFont("Assets/Cabin.ttf", 18);
            _fontBig = rc.LoadFont("Assets/Cabin.ttf", 40);

            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(rc);

            //Start Pannel Init
            _startPanel1 = new GUIPanel("RocketGame", _fontMedium, 10, 10, 150, 110);
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
            _startPanel1.ChildElements.Add(_startPanelButtonStart);
            _startPanel1.ChildElements.Add(_startPanelButtonStuff);

            _startPanel2 = new GUIPanel("Find and activate all the red cubes!", _fontMedium, 170, 20, 300, 30);

            _playPanel = new GUIPanel("Goals found:", _fontMedium, 10, 10, 150, 60);
            _playScore = new GUIText("", _fontMedium, 48, 45, new float4(1, 0, 0, 1));
            _playPanel.ChildElements.Add(_playScore);

            _overText = new GUIText("Game Over, you Win!", _fontBig, 200, 100, new float4(0, 1, 0, 1));

            ShowStartGUI();
        }

        private void OnGUIButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;

            if (sender == _startPanelButtonStart)
            {
                _gw.SetGamestate((int)GameState.Running);
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

        public void UpdateScore()
        {
            string curScore = _gw.GetScore().ToString();
            string maxScore = _gw.GetMaxScore().ToString();

            if (curScore.Length < 2)
                curScore = "0" + curScore;
            if (maxScore.Length < 2)
                maxScore = "0" + maxScore;

            _playScore.Text = curScore + "/" + maxScore;
        }

        public void ShowStartGUI()
        {
            _guiHandler.Clear();
            _guiHandler.Add(_startPanel1);
            _guiHandler.Add(_startPanel2);
        }

        public void ShowPlayGUI()
        {
            UpdateScore();
            _guiHandler.Clear();
            _guiHandler.Add(_playPanel);
        }

        public void ShowOverGUI()
        {
            _guiHandler.Add(_overText);
        }

        public void Resize()
        {
            _guiHandler.Refresh();
        }
    }
}
