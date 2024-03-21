﻿using System.ComponentModel;

namespace Fusee.Examples.Integrations.Wpf.Model
{
    internal class FpsModel : INotifyPropertyChanged
    {
        private float _fps;

        public float Fps
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
                OnPropertyChanged(nameof(Fps));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}