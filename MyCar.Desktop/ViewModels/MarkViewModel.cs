using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Windows.AddWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class MarkViewModel : BaseViewModel
    {
        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                Task.Run(Search);
            }
        }
        public List<string> SearchType { get; set; }
        private string selectedSearchType;
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
            }
        }

        private List<MarkCarApi> searchResult;
        private List<MarkCarApi> FullMarks;
        private List<CarApi> Cars;

        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<MarkCarApi> Marks { get; set; } = new List<MarkCarApi>();
        public MarkCarApi SelectedMark { get; set; }
        public ModelApi SelectedModel { get; set; }
        public CustomCommand DeleteMark { get; set; }
        public CustomCommand EditMark { get; set; }
        public CustomCommand AddMark { get; set; }
        public CustomCommand AddModel { get; set; }
        public CustomCommand EditModel { get; set; }
        public CustomCommand DeleteModel { get; set; }

        public MarkViewModel()
        {
            Task.Run(GetMarkList).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Марка" });
            selectedSearchType = SearchType.First();

            AddMark = new CustomCommand(() =>
            {
                AddMarkWindow addMark = new AddMarkWindow();
                addMark.ShowDialog();
                Task.Run(GetMarkList);
            });

            EditMark = new CustomCommand(() => 
            {
                if (SelectedMark == null || SelectedMark.ID == 0) return;
                AddMarkWindow addMark = new AddMarkWindow(SelectedMark);
                addMark.ShowDialog();
                Task.Run(GetMarkList);
            });

            DeleteMark = new CustomCommand(async () =>
            {
                if (SelectedMark == null || SelectedMark.ID == 0) return;
                if (SelectedMark.Models.Count != 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Марка связана с другими сущностями!" });
                    return;
                }
                await DeleteMarkAsync(SelectedMark);
                Task.Run(GetMarkList);
            });

            DeleteModel = new CustomCommand(async () =>
            {
                if (SelectedModel == null || SelectedModel.ID == 0) return;
                if (Cars.Any(s=>s.ModelId == SelectedModel.ID))
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Модель связана с другими сущностями!" });
                    return;
                }
                await DeleteModelAsync(SelectedModel);
                Task.Run(GetMarkList);
            });

            AddModel = new CustomCommand(() =>
            {
                AddModelWindow addModel = new AddModelWindow();
                addModel.ShowDialog();
                Task.Run(GetMarkList);
            });
            EditModel = new CustomCommand(() =>
            {
                if (SelectedModel == null || SelectedModel.ID == 0) return;
                AddModelWindow addModel = new AddModelWindow(SelectedModel);
                addModel.ShowDialog();
                Task.Run(GetMarkList);
            });
        }
        private async Task DeleteMarkAsync(MarkCarApi markCar)
        {
            var ma = await Api.DeleteAsync<MarkCarApi>(markCar, "MarkCar");
        }
        private async Task DeleteModelAsync(ModelApi model)
        {
            var mo = await Api.DeleteAsync<ModelApi>(model, "Model");
        }
        

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            else
                searchResult = await Api.SearchAsync<List<MarkCarApi>>(SelectedSearchType, search, "MarkCar");
            UpdateList();
        }
        private void UpdateList()
        {
           Marks = searchResult;
           SignalChanged(nameof(Marks));
        }

        private async Task GetMarkList()
        {
            Marks = await Api.GetListAsync<List<MarkCarApi>>("MarkCar");
            Models = await Api.GetListAsync<List<ModelApi>>("Model");
            FullMarks = Marks;
            Cars = await Api.GetListAsync<List<CarApi>>("Car");
            SignalChanged(nameof(Marks));
        }
    }
}
