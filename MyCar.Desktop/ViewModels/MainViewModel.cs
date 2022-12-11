using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Pages;

namespace MyCar.Desktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Window settings


        private Window mWindow;

        private int mOuterMarginSize = 0;

        private int mWindowRadius = 5;

        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        public double WindowMinimumWidth { get; set; } = 900;

        public double WindowMinimumHeight { get; set; } = 600;

        public bool Borderless { get { return (mWindow.WindowState == WindowState.Maximized || mDockPosition != WindowDockPosition.Undocked); } }

        public int ResizeBorder { get { return Borderless ? 0 : 6; } }

        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }

        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        public int OuterMarginSize
        {
            get
            {

                return Borderless ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        public int WindowRadius
        {
            get
            {
                return Borderless ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        public int TitleHeight { get; set; } = 30;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        #endregion

        public string UserString { get; set; }

        public Page CurrentPage { get; set; }

        public CustomCommand MinimizeCommand { get; set; }
        public CustomCommand MaximizeCommand { get; set; }
        public CustomCommand CloseCommand { get; set; }
        public CustomCommand MenuCommand { get; set; }
        public CustomCommand UserPageCommand { get; set; }
        public CustomCommand OpenCar { get; set; }
        public CustomCommand MarkPageCommand { get; set; }
        public CustomCommand CharcteristicPageCommand { get; set; }
        public CustomCommand CarSalesPageCommand { get; set; }
        public CustomCommand OrdersPageCommand { get; set; }
        public CustomCommand AddOrderPageCommand { get; set; }

        public MainViewModel(Window window, UserApi user)
        {
            mWindow = window;

            GetUserString(user);

            MinimizeCommand = new CustomCommand(() => mWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new CustomCommand(() => mWindow.WindowState ^= WindowState.Maximized);
            CloseCommand = new CustomCommand(() => mWindow.Close());
            MenuCommand = new CustomCommand(() => SystemCommands.ShowSystemMenu(mWindow, GetMousePosition()));

            CurrentPage = new UserPage();

            CharcteristicPageCommand = new CustomCommand(() => CurrentPage = new CharacteristicPage());
            CarSalesPageCommand = new CustomCommand(() => CurrentPage = new SaleCarsPage());
            UserPageCommand = new CustomCommand(() => CurrentPage = new UserPage());
            OpenCar = new CustomCommand(() => CurrentPage = new CarPage());
            MarkPageCommand = new CustomCommand(() => CurrentPage = new MarkPage());
            OrdersPageCommand = new CustomCommand(() => CurrentPage = new OrderPage());
            AddOrderPageCommand = new CustomCommand(() => CurrentPage = new AddOrderPage());

            var resizer = new WindowResizer(mWindow);
            resizer.WindowDockChanged += (dock) =>
            {

                mDockPosition = dock;


                WindowResized();
            };
        }

        private void GetUserString(UserApi user)
        {
            UserString = user.Passport.FirstName + " " + user.Passport.LastName;
            SignalChanged(nameof(UserString));
        }
        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(mWindow);
            return new Point(position.X + mWindow.Left, position.Y + mWindow.Top);
        }
        private void WindowResized()
        {
            SignalChanged(nameof(Borderless));
            SignalChanged(nameof(ResizeBorderThickness));
            SignalChanged(nameof(OuterMarginSize));
            SignalChanged(nameof(OuterMarginSizeThickness));
            SignalChanged(nameof(WindowRadius));
            SignalChanged(nameof(WindowCornerRadius));
        }

    }
}
