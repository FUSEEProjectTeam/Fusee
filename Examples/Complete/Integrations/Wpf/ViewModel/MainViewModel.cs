using Fusee.Examples.Integrations.Wpf.Model;

namespace Fusee.Examples.Integrations.Wpf.ViewModel
{
    class MainViewModel
    {
        public PositionModel Position { get; set; }
        public FpsModel Fps { get; set; }
        public VSyncModel VSync { get; set; }

        public MainViewModel()
        {
            Position = new PositionModel();
            Fps = new FpsModel();
            VSync = new VSyncModel();
        }
    }
}
