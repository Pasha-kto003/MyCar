using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels.Dialogs
{
    public class MessageBoxDialogViewModel : BaseDialogViewModel
    {
        public string Message { get; set; }

        public string OkText { get; set; } = "OK";

    }
}
