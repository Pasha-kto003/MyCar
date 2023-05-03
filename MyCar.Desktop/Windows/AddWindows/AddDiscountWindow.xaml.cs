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
    /// Логика взаимодействия для AddDiscountWindow.xaml
    /// </summary>
    public partial class AddDiscountWindow : Window
    {
        public AddDiscountWindow()
        {
            InitializeComponent();
            DataContext = new AddDiscountViewModel(null);
        }

        public AddDiscountWindow(DiscountApi discount)
        {
            InitializeComponent();
            DataContext = new AddDiscountViewModel(discount);
        }
    }
}
