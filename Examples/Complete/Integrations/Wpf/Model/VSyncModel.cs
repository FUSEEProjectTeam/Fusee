using System.ComponentModel;

namespace Fusee.Examples.Integrations.Wpf.Model
{
    internal class VSyncModel : INotifyPropertyChanged
    {
        private bool _vsync;

        public bool VSync
        {
            get
            {
                return _vsync;
            }
            set
            {
                _vsync = value;
                OnPropertyChanged(nameof(VSync));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}