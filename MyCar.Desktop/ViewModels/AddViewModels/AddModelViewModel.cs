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
    public class AddModelViewModel : BaseViewModel
    {
        public List<MarkCarApi> Marks { get; set; }
        public MarkCarApi SelectedMark { get; set; }
        public ModelApi AddModelVM { get; set; }

        public CustomCommand SaveModel { get; set; }

        public CustomCommand Cancel { get; set; }
        public AddModelViewModel(ModelApi model)
        {
            Task task = Task.Run(GetList);
            task.Wait();
            if (model == null)
            {
                AddModelVM = new ModelApi();
            }

            else
            {
                AddModelVM = new ModelApi
                {
                    ID = model.ID,
                    MarkId = model.MarkId,
                    ModelName = model.ModelName
                };
                SelectedMark = Marks.FirstOrDefault(s => s.ID == model.MarkId);
            }
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
            SaveModel = new CustomCommand(async() =>
            {
                if( SelectedMark == null || SelectedMark.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана марка!" });
                    return;
                }
                if (AddModelVM.ModelName == null || AddModelVM.ModelName == "")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Заполнены не все поля!" });
                    return;
                }
                AddModelVM.MarkId = SelectedMark.ID;
                if (AddModelVM.ID == 0)
                {
                   await CreateModel(AddModelVM);
                }
                else
                {
                    await EditModel(AddModelVM);
                }
                UIManager.CloseWindow(this);
            });
        }

        private async Task CreateModel(ModelApi modelApi)
        {
            var model = await Api.PostAsync<ModelApi>(modelApi, "Model");
        }

        private async Task EditModel(ModelApi modelApi)
        {
            var model = await Api.PutAsync<ModelApi>(modelApi, "Model");
        }

        private async Task GetList()
        {
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            SignalChanged(nameof(Marks));
        }
    }
}
