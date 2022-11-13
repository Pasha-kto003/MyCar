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
    /// Логика взаимодействия для AddUnitWindow.xaml
    /// </summary>
    public partial class AddUnitWindow : Window
    {
        public AddUnitWindow()
        {
            InitializeComponent();
            DataContext = new AddUnitViewModel(null);
        }

        public AddUnitWindow(UnitApi unit)
        {
            InitializeComponent();
            DataContext = new AddUnitViewModel(unit);
        }
    }
}
