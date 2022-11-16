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
    public class CharacteristicViewModel : BaseViewModel
    {
        public List<UnitApi> UnitFilter { get; set; }

        private UnitApi selectedUnitFilter;
        public UnitApi SelectedUnitFilter
        {
            get => selectedUnitFilter;
            set
            {
                selectedUnitFilter = value;
                Task.Run(Search);
            }
        }

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

        private UnitApi selectedUnitFilter;
        public UnitApi SelectedUnitFilter
        {
            get => selectedUnitFilter;
            set
            {
                selectedUnitFilter = value;
                Task.Run(Search);
            }
        }

        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();

        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();
        public List<UnitApi> Units { get; set; } = new List<UnitApi>();

        public List<UnitApi> UnitFilter { get; set; }

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
            SearchType.AddRange(new string[] { "Характеристика"});
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
                Task.Run(GetCharacteristic);
            });

            EditCharacteristic = new CustomCommand(() =>
            {
                if(SelectedCharacteristic == null)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана характеристика для редактирования" });
                    return;
                }
                AddCharacteristicWindow addCharacteristic = new AddCharacteristicWindow(SelectedCharacteristic);
                addCharacteristic.ShowDialog();
                Task.Run(GetCharacteristic);
            });

            AddEquipment = new CustomCommand(() =>
            {
                AddEquipmentWindow equipmentWindow = new AddEquipmentWindow();
                equipmentWindow.ShowDialog();
                Task.Run(GetEquipment);
            });

            EditEquipment = new CustomCommand(() =>
            {
                AddEquipmentWindow addEquipment = new AddEquipmentWindow();
                addEquipment.ShowDialog();
                Task.Run(GetEquipment);
            });
        }

        private async Task GetCharacteristic()
        {
            UnitFilter = Units;
            UnitFilter.Add(new UnitApi { UnitName = "Все" });
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            FullTypes = Characteristics;

            UnitFilter = Units;
            UnitFilter.Add(new UnitApi { UnitName = "Все" });

            SelectedUnitFilter = UnitFilter.Last();
            SignalChanged(nameof(Characteristics));
        }

        private async Task GetEquipment()
        {
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            FullEquipments = Equipments;
            SignalChanged(nameof(Equipments));
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
                searchResult = await Api.SearchFilterAsync<List<CharacteristicApi>>(SelectedSearchType, "$", "Characteristic", SelectedUnitFilter.UnitName);
            else
                searchResult = await Api.SearchFilterAsync<List<CharacteristicApi>>(SelectedSearchType, search, "Characteristic",SelectedUnitFilter.UnitName);
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
