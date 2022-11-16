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
    public class BodyTypeViewModel : BaseViewModel
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
        public BodyTypeApi SelectedBodyType { get; set; }

        public CustomCommand AddType { get; set; }
        public CustomCommand EditType { get; set; }

        private List<BodyTypeApi> searchResult;
        private List<BodyTypeApi> FullTypes;

        public List<BodyTypeApi> BodyTypes { get; set; } = new List<BodyTypeApi>();

        public BodyTypeViewModel()
        {
            Task.Run(GetBodyTypes);

            AddType = new CustomCommand(() =>
            {
                AddBodyTypeWindow addBodyType = new AddBodyTypeWindow();
                addBodyType.ShowDialog();
                Task.Run(GetBodyTypes).Wait();
            });

            EditType = new CustomCommand(() =>
            {
                if(SelectedBodyType == null || SelectedBodyType.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбран тип кузова!" });
                    return;
                }
                else
                {
                    AddBodyTypeWindow editBodyType = new AddBodyTypeWindow(SelectedBodyType);
                    editBodyType.ShowDialog();
                    Task.Run(GetBodyTypes).Wait();
                }
            });
        }

        public async Task GetBodyTypes()
        {
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyTYpe");
            FullTypes = BodyTypes;
            SignalChanged(nameof(BodyTypes));
        }

        public async Task UpdateList()
        {
            BodyTypes = searchResult;
            SignalChanged(nameof(BodyTypes));
        }

        public async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            else
                searchResult = await Api.SearchAsync<List<BodyTypeApi>>("Кузов", search, "BodyType");
            UpdateList();
        }
    }
}
