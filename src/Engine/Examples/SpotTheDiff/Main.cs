using Fusee.Engine;
using Fusee.Math;

namespace Examples.SpotTheDiff
{
    /// <summary>
    ///     This is part of a tutorial about FUSEE's GUI system:
    ///     https://github.com/FUSEEProjectTeam/Fusee/wiki/HowTo:-Graphical-User-Interface-(2D-Games-in-Fusee)
    /// </summary>
    [FuseeApplication(Name = "SpotTheDiff", Description = "Simple 2D game to showcase FUSEE's GUI system.")]
    public class SpotTheDiff : RenderCanvas
    {
        private GUIButton[] _guiBDiffs;
        private IFont _guiFontCabin18;
        private IFont _guiFontCabin24;
        private GUIHandler _guiHandler;

        private GUIImage _guiImage;

        private GUIPanel _guiPanel;

        private GUIButton _guiResetButton;
        private GUIButton _guiSolveButton;
        private GUIText _guiText;

        private GUIButton[] _guiUDiffs;

        public override void Init()
        {
            // is called on startup
            Width = 616;
            Height = 688;

            // GUIHandler
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(RC);

            // font + text
            _guiFontCabin18 = RC.LoadFont("Assets/Cabin.ttf", 18);
            _guiFontCabin24 = RC.LoadFont("Assets/Cabin.ttf", 24);

            _guiText = new GUIText("Spot all seven differences!", _guiFontCabin24, 310, 35);
            _guiText.TextColor = new float4(1, 1, 1, 1);

            _guiHandler.Add(_guiText);

            // image
            _guiImage = new GUIImage("Assets/spot_the_diff.png", 0, 0, -5, 600, 650);
            _guiHandler.Add(_guiImage);

            // buttons / rectangles
            _guiUDiffs = new GUIButton[7];
            _guiBDiffs = new GUIButton[7];

            _guiUDiffs[0] = new GUIButton(240, 3, 40, 40);
            _guiBDiffs[0] = new GUIButton(240, 328, 40, 40);

            _guiUDiffs[1] = new GUIButton(3, 270, 40, 40);
            _guiBDiffs[1] = new GUIButton(3, 595, 40, 40);

            _guiUDiffs[2] = new GUIButton(220, 255, 40, 40);
            _guiBDiffs[2] = new GUIButton(220, 580, 40, 40);

            _guiUDiffs[3] = new GUIButton(325, 170, 40, 40);
            _guiBDiffs[3] = new GUIButton(325, 495, 40, 40);

            _guiUDiffs[4] = new GUIButton(265, 110, 40, 40);
            _guiBDiffs[4] = new GUIButton(265, 435, 40, 40);

            _guiUDiffs[5] = new GUIButton(490, 215, 40, 40);
            _guiBDiffs[5] = new GUIButton(490, 540, 40, 40);

            _guiUDiffs[6] = new GUIButton(495, 280, 40, 40);
            _guiBDiffs[6] = new GUIButton(495, 605, 40, 40);

            for (int i = 0; i < 7; i++)
            {
                _guiUDiffs[i].ButtonColor = new float4(0, 0, 0, 0);
                _guiBDiffs[i].ButtonColor = new float4(0, 0, 0, 0);

                _guiUDiffs[i].BorderColor = new float4(0, 0, 0, 1);
                _guiBDiffs[i].BorderColor = new float4(0, 0, 0, 1);

                _guiUDiffs[i].BorderWidth = 0;
                _guiBDiffs[i].BorderWidth = 0;

                _guiUDiffs[i].Tag = _guiBDiffs[i];
                _guiBDiffs[i].Tag = _guiUDiffs[i];

                _guiBDiffs[i].OnGUIButtonDown += OnDiffButtonDown;
                _guiUDiffs[i].OnGUIButtonDown += OnDiffButtonDown;

                _guiHandler.Add(_guiUDiffs[i]);
                _guiHandler.Add(_guiBDiffs[i]);
            }

            // panel
            _guiPanel = new GUIPanel("Menu", _guiFontCabin18, 10, 10, 150, 110);
            _guiHandler.Add(_guiPanel);

            // reset button
            _guiResetButton = new GUIButton("Reset", _guiFontCabin18, 25, 40, 100, 25);

            _guiResetButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiResetButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiResetButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiResetButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiResetButton);

            // solve button
            _guiSolveButton = new GUIButton("Solve", _guiFontCabin18, 25, 70, 100, 25);

            _guiSolveButton.OnGUIButtonDown += OnMenuButtonDown;
            _guiSolveButton.OnGUIButtonUp += OnMenuButtonUp;
            _guiSolveButton.OnGUIButtonEnter += OnMenuButtonEnter;
            _guiSolveButton.OnGUIButtonLeave += OnMenuButtonLeave;

            _guiPanel.ChildElements.Add(_guiSolveButton);
        }

        private void OnDiffButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;

            var guiButton = sender.Tag as GUIButton;
            if (guiButton != null) guiButton.BorderWidth = 2;
        }

        private void OnMenuButtonDown(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 2;
        }

        private void OnMenuButtonUp(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;

            var bWidth = (sender == _guiSolveButton) ? 2 : 0;

            foreach (var guiButton in _guiUDiffs)
                guiButton.BorderWidth = bWidth;
            foreach (var guiButton in _guiBDiffs)
                guiButton.BorderWidth = bWidth;
        }

        private static void OnMenuButtonEnter(GUIButton sender, MouseEventArgs mea)
        {
            if (Input.Instance.IsButton(MouseButtons.Left))
                sender.BorderWidth = 2;

            sender.TextColor = new float4(0.8f, 0.1f, 0.1f, 1);
        }

        private static void OnMenuButtonLeave(GUIButton sender, MouseEventArgs mea)
        {
            sender.BorderWidth = 1;
            sender.TextColor = new float4(0f, 0f, 0f, 1);
        }

        public override void RenderAFrame()
        {
            RC.Clear(ClearFlags.Color | ClearFlags.Depth);

            _guiHandler.RenderGUI();

            Present();
        }

        public override void Resize()
        {
            // var left = (int) Math.Round(Width/2.0f) - 300;
            // var top = (int) Math.Round(Height/2.0f) - 325;

            RC.Viewport(0, 0, Width, Height);

            var aspectRatio = Width/(float) Height;
            RC.Projection = float4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 1, 10000);

            // refresh all elements
            _guiHandler.Refresh();
        }

        public static void Main()
        {
            var app = new SpotTheDiff();
            app.Run();
        }
    }
}