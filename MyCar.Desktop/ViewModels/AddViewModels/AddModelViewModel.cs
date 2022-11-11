using ModelsApi;
using MyCar.Desktop.Core;
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
        public ModelApi AddModelVM { get; set; }

        public CustomCommand SaveModel { get; set; }

        public AddModelViewModel(ModelApi model)
        {
            if(model == null)
            {
                AddModelVM = new ModelApi();
            }

            else
            {
                AddModelVM = new ModelApi
                {
                    ID = model.ID,
                    //MarkCar = model.MarkCar,
                    MarkId = model.MarkId,
                    ModelName = model.ModelName
                };
            }

            SaveModel = new CustomCommand(() =>
            {
                if(AddModelVM.ModelName == "")
                {
                    MessageBox.Show("Не введено имя модели");
                    return;
                }
                if(AddModelVM.ID == 0)
                {
                    CreateModel(AddModelVM);
                }
                else
                {
                    EditModel(AddModelVM);
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

        private async Task CreateModel(ModelApi modelApi)
        {
            var model = await Api.PostAsync<ModelApi>(modelApi, "Model");
        }

        private async Task EditModel(ModelApi modelApi)
        {
            var model = await Api.PutAsync<ModelApi>(modelApi, "Model");
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
        }
    }
}
