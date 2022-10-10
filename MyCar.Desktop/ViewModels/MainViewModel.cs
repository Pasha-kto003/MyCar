using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsApi;
using MyCar.Desktop.Core;

namespace MyCar.Desktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        public string UserStringTest { get; set; }   

        public MainViewModel(UserApi user)
        {
            UserStringTest = user.UserName + user.PasswordHash.ToString();
        }

    }
}
