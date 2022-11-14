using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.Windows.AddWindows;
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

        private string searchTextEquipment = "";
        public string SearchTextEquipment
        {
            get => searchTextEquipment;
            set
            {
                searchTextEquipment = value;
                Task.Run(SearchEquipment);
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

        public List<string> SearchTypeEquipment { get; set; }
        private string selectedSearchTypeEquipment;
        public string SelectedSearchTypeEquipment
        {
            get => selectedSearchTypeEquipment;
            set
            {
                selectedSearchTypeEquipment = value;
            }
        }

        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();

        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();
        public List<UnitApi> Units { get; set; } = new List<UnitApi>();

        public CharacteristicApi SelectedCharacteristic { get; set; }
        public UnitApi SelectedUnit { get; set; }
        public EquipmentApi SelectedEquipment { get; set; }

        public CustomCommand AddCharacteristic { get; set; }
        public CustomCommand EditCharacteristic { get; set; }

        public CustomCommand AddEquipment { get; set; }
        public CustomCommand EditEquipment { get; set; }

        private List<CharacteristicApi> searchResult;
        private List<CharacteristicApi> FullTypes;

        private List<EquipmentApi> FullEquipments;
        private List<EquipmentApi> searchResultEquipment;

        public CharacteristicViewModel()
        {
            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Характеристика", "Единица измерения" });
            selectedSearchType = SearchType.First();

            SearchTypeEquipment = new List<string>();
            SearchTypeEquipment.AddRange(new string[] { "Комплектация", "Цена" });
            selectedSearchTypeEquipment = SearchTypeEquipment.First(); 

            Task.Run(GetCharacteristic);

            Task.Run(GetEquipment);

            AddCharacteristic = new CustomCommand(() =>
            {
                AddCharacteristicWindow addCharacteristic = new AddCharacteristicWindow();
                addCharacteristic.ShowDialog();
                GetCharacteristic();
            });

            EditCharacteristic = new CustomCommand(() =>
            {
                if(SelectedCharacteristic == null)
                {
                    SendMessage("Не выбрана характеристика для редактирования");
                }
                AddCharacteristicWindow addCharacteristic = new AddCharacteristicWindow(SelectedCharacteristic);
                addCharacteristic.ShowDialog();
                GetCharacteristic();
            });

            AddEquipment = new CustomCommand(() =>
            {
                AddEquipmentWindow equipmentWindow = new AddEquipmentWindow();
                equipmentWindow.ShowDialog();
                GetEquipment();
            });

            EditEquipment = new CustomCommand(() =>
            {
                AddEquipmentWindow addEquipment = new AddEquipmentWindow();
                addEquipment.ShowDialog();
                GetEquipment();
            });
        }

        private async Task GetCharacteristic()
        {
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            FullTypes = Characteristics;
            SignalChanged(nameof(Characteristics));
        }

        private async Task GetEquipment()
        {
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            FullEquipments = Equipments;
            SignalChanged(nameof(Equipments));
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

        public async Task UpdateListEquipment()
        {
            Equipments = searchResultEquipment;
            SignalChanged(nameof(Equipments));
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

        private async Task SearchEquipment()
        {
            var search = SearchTextEquipment.ToLower();
            if (search == "")
                searchResultEquipment = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            else
                searchResultEquipment = await Api.SearchAsync<List<EquipmentApi>>(SelectedSearchTypeEquipment, search, "Equipment");
            UpdateListEquipment();
        }
    }
}
