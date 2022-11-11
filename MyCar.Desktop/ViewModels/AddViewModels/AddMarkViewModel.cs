using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.Windows.AddWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddMarkViewModel : BaseViewModel
    {
        public MarkCarApi AddMark { get; set; }

        public ObservableCollection<ModelApi> AllModels { get; set; }
        public ObservableCollection<ModelApi> ThisMarkModels { get; set; } = new ObservableCollection<ModelApi>();

        public ModelApi SelectedModel { get; set; }
        public ModelApi SelectedMarkModel { get; set; }

        public CustomCommand Cancel { get; set; }
        public CustomCommand Save { get; set; }
        public CustomCommand AddModel { get; set; }
        public CustomCommand RemoveModel { get; set; }
        public CustomCommand CreateModel { get; set; }

        public AddMarkViewModel(MarkCarApi addmark)
        {
            Task.Run(GetList);

            if (addmark == null)
            {
                AddMark = new MarkCarApi();
            }
            else
            {
                AddMark = new MarkCarApi
                {
                    ID = addmark.ID,
                    MarkName = addmark.MarkName,
                    Models = addmark.Models,
                };
                ThisMarkModels = new ObservableCollection<ModelApi>(AddMark.Models);
            }

            AddModel = new CustomCommand(() =>
            {
                if (SelectedModel == null) return;
                    if (CheckContains(ThisMarkModels.ToList(), SelectedModel))
                    {
                        UIManager.ShowMessage(new Dialogs.MessageBoxDialogViewModel
                        {
                            Message = "Выбранная модель уже содержится!",
                            Title = "Ошибка!"
                        });
                        return;
                    }
                ThisMarkModels.Add(SelectedModel);
            });

            CreateModel = new CustomCommand(async () =>
            {
                if(SelectedModel == null || SelectedModel.ID == 0)
                {
                    AddModelWindow addModel = new AddModelWindow();
                    addModel.ShowDialog();
                    GetList();
                }
                else
                {
                    AddModelWindow addModel = new AddModelWindow(SelectedModel);
                    addModel.ShowDialog();
                    GetList();
                }
            });

            RemoveModel = new CustomCommand(() =>
            {
                if (SelectedMarkModel == null) return;
                ThisMarkModels.Remove(SelectedMarkModel);
            });

            Save = new CustomCommand(async () =>
            {
                try
                {
                    AddMark.Models = new List<ModelApi>(ThisMarkModels);
                    if (AddMark.ID == 0)
                      await Add(AddMark);
                    else
                      await Edit(AddMark);
                }
                catch (Exception e)
                {
                    UIManager.ShowMessage(new Dialogs.MessageBoxDialogViewModel
                    {
                        Message = e.Message,
                        Title = "Ошибка!"
                    });
                }
                UIManager.CloseWindow(this);
            });
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }
        private bool CheckContains(List<ModelApi> list, ModelApi model)
        {
            bool result = false;
            foreach (ModelApi item in list)
            {
                if (item.ID == model.ID)
                {
                    result = true;
                }
            }
            return result; 
        }

        private async Task Add(MarkCarApi mark)
        {
            var id = await Api.PostAsync<MarkCarApi>(mark, "MarkCar");
        }
        private async Task Edit(MarkCarApi mark)
        {
            var id = await Api.PutAsync<MarkCarApi>(mark, "MarkCar");
        }

        private async Task GetList()
        {
            var list = await Api.GetListAsync<List<ModelApi>>("Model");
            AllModels = new ObservableCollection<ModelApi>(list);
            SignalChanged(nameof(AllModels));
        }

    }
}
