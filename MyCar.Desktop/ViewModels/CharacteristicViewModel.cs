using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class CharacteristicViewModel : BaseViewModel
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

        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();
        public List<UnitApi> Units { get; set; } = new List<UnitApi>();

        public CharacteristicApi SelectedCharacteristic { get; set; }
        public UnitApi SelectedUnit { get; set; }

        public CustomCommand AddCharacteristic { get; set; }
        public CustomCommand EditCharacteristic { get; set; }

        private List<CharacteristicApi> searchResult;
        private List<CharacteristicApi> FullTypes;


        public CharacteristicViewModel()
        {
            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Характеристика", "Единица измерения" });
            selectedSearchType = SearchType.First();

            Task.Run(GetCharacteristic);

            AddCharacteristic = new CustomCommand(() =>
            {

            });

            EditCharacteristic = new CustomCommand(() =>
            {

            });
        }

        private async Task GetCharacteristic()
        {
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            FullTypes = Characteristics;
            SignalChanged(nameof(Characteristics));
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

        public async Task UpdateList()
        {
            Characteristics = searchResult;
            SignalChanged(nameof(Characteristics));
        }


        private async Task Search()
        {
            var search = SearchText.ToLower();
            if (search == "")
                searchResult = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            else
                searchResult = await Api.SearchAsync<List<CharacteristicApi>>(SelectedSearchType, search, "Characteristic");
            UpdateList();
        }
    }
}
