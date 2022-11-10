using ModelsApi;
using MyCar.Desktop.Core;
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

        public List<ModelApi> Models { get; set; } = new List<ModelApi>();
        public List<MarkCarApi> Marks { get; set; } = new List<MarkCarApi>();
        public MarkCarApi SelectedMark { get; set; }
        public CustomCommand EditMark { get; set; }
        public CustomCommand AddMark { get; set; }

        public MarkViewModel()
        {
            Task.Run(GetMarkList);

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Марка" });
            selectedSearchType = SearchType.First();

            AddMark = new CustomCommand(() =>
            {
                AddMarkWindow addMark = new AddMarkWindow();
                addMark.ShowDialog();
            });

            EditMark = new CustomCommand(() => 
            {
                if (SelectedMark == null || SelectedMark.ID == 0) return;
                AddMarkWindow addMark = new AddMarkWindow(SelectedMark);
                addMark.ShowDialog();
            });
           

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
            SignalChanged(nameof(Marks));
        }
    }
}
