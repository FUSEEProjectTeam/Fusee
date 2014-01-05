using Fusee.Engine;

namespace Examples.RocketGame
{
// ReSharper disable once InconsistentNaming
    class GUI
    {
        private readonly GUIHandler _guiHandler;

        private readonly IFont _fontSmall;
        private readonly IFont _fontMedium;
        private readonly IFont _fontBig;

        private GUIText _debug;

        public GUI(RenderContext rc)
        {
            _guiHandler = new GUIHandler();
            _guiHandler.AttachToContext(rc);

            _fontSmall = rc.LoadFont("Assets/Cabin.ttf", 12);
            _fontMedium = rc.LoadFont("Assets/Cabin.ttf", 18);
            _fontBig = rc.LoadFont("Assets/Cabin.ttf", 28);

            _debug = new GUIText("Hello World!", _fontSmall, 150, 150);

            _guiHandler.Add(_debug);
        }

        public void Render()
        {
            _guiHandler.RenderGUI();
        }

        public void SetDebugMsg(string debugMsg)
        {
            _guiHandler.Remove(_debug);
            _debug = new GUIText(debugMsg, _fontSmall, 150, 150);
            _guiHandler.Add(_debug);
        }
    }
}
