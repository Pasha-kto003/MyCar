using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyCar.Desktop.ViewModels.Dialogs
{
    public class DialogWindowViewModel : BaseViewModel
    {
        #region Window Settings


        private Window mWindow;

        private int mOuterMarginSize = 0;

        private int mWindowRadius = 5;

        private WindowDockPosition mDockPosition = WindowDockPosition.Undocked;

        public double WindowMinimumWidth { get; set; } = 250;

        public double WindowMinimumHeight { get; set; } = 100;

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

        public string Title { get; set; } = "Диалоговое окно";

        public bool Result { get; set; } = false;

        public Control Content { get; set; }

        public CustomCommand YesCommand { get; set; }
        public CustomCommand CloseCommand { get; set; }
        public DialogWindowViewModel(Window window)
        {
            mWindow = window;

            CloseCommand = new CustomCommand(() =>
            {
                mWindow.Close();
            });
        }
    }
}
