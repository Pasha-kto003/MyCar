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

namespace MyCar.Desktop.Windows.OrderActions
{
    /// <summary>
    /// Логика взаимодействия для AddOrderInWindow.xaml
    /// </summary>
    public partial class AddOrderInWindow : Window
    {
        public AddOrderInWindow(WareHouseApi wareHouse,ActionTypeApi actionType)
        {
            InitializeComponent();
            DataContext = new AddOrderActionViewModel(wareHouse, actionType);
        }
    }
}
