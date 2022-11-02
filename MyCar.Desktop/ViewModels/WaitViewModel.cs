using MyCar.Desktop.Core;
using MyCar.Desktop.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyCar.Desktop.ViewModels
{
    public class WaitViewModel : BaseViewModel
    {
        Window window = new Window();
        DispatcherTimer _timer;
        TimeSpan _time;

        private string time;
        public string Time
        {
            get => time;
            set
            {
                time = value;
                SignalChanged();
            }
        }

        public WaitViewModel(Window window)
        {
            _time = TimeSpan.FromSeconds(30);

            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                Time = _time.ToString();
                if(_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                    MessageBox.Show("Возвращение в окно авторизации", "Подтверждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.DataContext == this)
                        {
                            CloseModalWindow(window);
                        }
                    }

                    AuthWindow authWindow = new AuthWindow();
                    authWindow.ShowDialog();
                }

                _time = _time.Add(TimeSpan.FromSeconds(-1));
                SignalChanged(nameof(Time));
            }, Application.Current.Dispatcher);

            _timer.Start();
        }

        public void CloseModalWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
