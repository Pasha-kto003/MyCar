using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
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

            SaveBodyType = new CustomCommand(async () =>
            {
                if(AddBodyTypeVM.TypeName == "")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не введен тип кузова" });
                    return;
                }

                if (AddBodyTypeVM.ID == 0)
                {
                    Task create = CreateType(AddBodyTypeVM);
                    await create;
                }
                else
                {
                    Task edit = EditType(AddBodyTypeVM);
                    await edit;
                }

                UIManager.CloseWindow(this);
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
    }
}
