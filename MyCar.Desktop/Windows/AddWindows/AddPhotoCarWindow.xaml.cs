using ModelsApi;
using MyCar.Desktop.ViewModels.AddViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyCar.Desktop.Windows.AddWindows
{
    /// <summary>
    /// Логика взаимодействия для AddPhotoCarWindow.xaml
    /// </summary>
    public partial class AddPhotoCarWindow : Window
    {
        public AddPhotoCarWindow()
        {
            InitializeComponent();
            DataContext = new AddPhotoCarViewModel(null);
        }

        public AddPhotoCarWindow(CarPhotoApi carPhoto)
        {
            InitializeComponent();
            DataContext = new AddPhotoCarViewModel(carPhoto);
        }
    }
}
