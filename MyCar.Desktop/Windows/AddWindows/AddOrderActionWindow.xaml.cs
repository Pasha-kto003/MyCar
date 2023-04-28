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
    /// Логика взаимодействия для AddOrderActionWindow.xaml
    /// </summary>
    public partial class AddOrderActionWindow : Window
    {
        public AddOrderActionWindow(WareHouseApi wareHouse,ActionTypeApi actionType,List<CountChangeHistoryApi> countChangeHistoryApis)
        {
            InitializeComponent();
            DataContext = new AddOrderActionViewModel(wareHouse, actionType, countChangeHistoryApis);
        }
    }
}
