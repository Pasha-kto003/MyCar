using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddBodyTypeViewModel : BaseViewModel
    {
        public BodyTypeApi AddBodyTypeVM { get; set; }

        public CustomCommand SaveBodyType { get; set; }

        public AddBodyTypeViewModel(BodyTypeApi bodyType)
        {
            if(bodyType == null)
            {
                AddBodyTypeVM = new BodyTypeApi();
            }

            else
            {
                AddBodyTypeVM = new BodyTypeApi
                {
                    ID = bodyType.ID,
                    TypeName = bodyType.TypeName
                };
            }

            SaveBodyType = new CustomCommand(() =>
            {
                if(AddBodyTypeVM.TypeName == "")
                {
                    SendMessage("Не введен тип кузова");
                }

                if(AddBodyTypeVM.ID == 0)
                {
                    CreateType(AddBodyTypeVM);
                }
                else
                {
                    EditType(AddBodyTypeVM);
                }

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        CloseWindow(window);
                    }
                }
            });
        }

        public async Task CreateType(BodyTypeApi bodyType)
        {
            var body = await Api.PostAsync<BodyTypeApi>(bodyType, "BodyType");
        }

        public async Task EditType(BodyTypeApi bodyType)
        {
            var body = await Api.PutAsync<BodyTypeApi>(bodyType, "BodyType");
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }

        public void SendMessage(string message)
        {
            UIManager.ShowMessage(new Dialogs.MessageBoxDialogViewModel
            {
                Message = message,
                OkText = "ОК",
                Title = "Ошибка!"
            });
            return;
        }
    }
}
