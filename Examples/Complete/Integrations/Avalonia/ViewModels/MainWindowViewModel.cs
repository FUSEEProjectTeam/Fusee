using Fusee.Example.Integrations.Avalonia.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Fusee.Examples.Integrations.Core;
using ReactiveUI;
using Avalonia.Threading;

namespace Fusee.Example.Integrations.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            if (MainWindow.FuseeApp is not null)
            {
                MainWindow.FuseeApp.FusToWpfEvents += (s, fps) =>
                {
                    if (fps is FpsEvent @event)
                    {
                        GetFPS = $"FPS: {@event.Fps}";
                    }
                };
            }
        }

        private string _fps;

        public string GetFPS
        {
            get => _fps;
            set
            {
                _fps = value;
                Dispatcher.UIThread.InvokeAsync(() => this.RaisePropertyChanged(nameof(GetFPS)));
            }
        }

        private float _x;
        public float xValue
        {
            get => _x;
            set
            {
                this.RaiseAndSetIfChanged(ref _x, value);
                if (MainWindow.FuseeApp is not null)
                    MainWindow.FuseeApp.ChangeRocketX(_x);
            }
        }

        private float _y;
        public float yValue
        {
            get => _y;
            set
            {
                this.RaiseAndSetIfChanged(ref _y, value);
                if (MainWindow.FuseeApp is not null)
                    MainWindow.FuseeApp.ChangeRocketY(_y);
            }
        }
        private float _z;
        public float zValue
        {
            get => _z;
            set
            {
                this.RaiseAndSetIfChanged(ref _z, value);
                if (MainWindow.FuseeApp is not null)
                    MainWindow.FuseeApp.ChangeRocketZ(_z);
            }
        }

    }
}
