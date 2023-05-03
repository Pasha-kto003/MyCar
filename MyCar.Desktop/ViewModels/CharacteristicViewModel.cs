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
        #region CharacteristicProperties
        public List<UnitApi> UnitFilter { get; set; } = new List<UnitApi>();

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
        public List<string> SearchType { get; set; }
        public string SelectedSearchType { get; set; }

        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();

        public CharacteristicApi SelectedCharacteristic { get; set; }

        private List<CharacteristicApi> searchResult;

        private List<CharacteristicApi> FullTypes;
        #endregion

        #region EqupmentProperties

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
        public List<string> SearchTypeEquipment { get; set; }
        public string SelectedSearchTypeEquipment { get; set; }

        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();
        public EquipmentApi SelectedEquipment { get; set; }

        private List<EquipmentApi> FullEquipments;
        private List<EquipmentApi> searchResultEquipment;
        #endregion

        #region BodyTypeProperties
        private string searchTextBody = "";
        public string SearchTextBody
        {
            get => searchTextBody;
            set
            {
                searchTextBody = value;
                Task.Run(SearchBodyTypes);
            }
        }

        private List<BodyTypeApi> searchResultBody;
        private List<BodyTypeApi> FullBodyTypes;

        public List<BodyTypeApi> BodyTypes { get; set; } = new List<BodyTypeApi>();
        public BodyTypeApi SelectedBodyType { get; set; }
        #endregion

        #region UnitProperties
        public List<UnitApi> Units { get; set; } = new List<UnitApi>();
        public UnitApi SelectedUnit { get; set;  }
        #endregion

        #region PhotoProperties
        public List<CarPhotoApi> CarPhotos { get; set; } = new List<CarPhotoApi>();
        private CarPhotoApi selectedPhotoCar { get; set; }
        public CarPhotoApi SelectedPhotoCar
        {
            get => selectedPhotoCar;
            set
            {
                selectedPhotoCar = value;
                SignalChanged();
            }
        }

        private List<CarPhotoApi> FullPhotoCars { get; set; }

        public CustomCommand AddPhotoCar { get; set; }
        public CustomCommand EditPhotoCar { get; set; }
        public CustomCommand DeletePhotoCar { get; set; }
        #endregion

        #region DiscountProperties
        public List<DiscountApi> Discounts { get; set; } = new List<DiscountApi>();
        private DiscountApi selectedDiscount { get; set; }
        public DiscountApi SelectedDiscount
        {
            get => selectedDiscount;
            set
            {
                selectedDiscount = value;
                SignalChanged();
            }
        }

        private List<DiscountApi> FullDiscounts;

        public CustomCommand AddDiscount { get; set; }
        public CustomCommand EditDiscount { get; set; }
        public CustomCommand DeleteDiscount { get; set; }
        #endregion

        public CustomCommand AddType { get; set; }
        public CustomCommand EditType { get; set; }
        public CustomCommand AddCharacteristic { get; set; }
        public CustomCommand EditCharacteristic { get; set; }
        public CustomCommand AddUnit { get; set; }
        public CustomCommand EditUnit { get; set; }
        public CustomCommand AddEquipment { get; set; }
        public CustomCommand EditEquipment { get; set; }

        public CharacteristicViewModel()
        { 
            Task.Run(GetCharacteristic).Wait();

            Task.Run(GetEquipment).Wait();

            Task.Run(GetBodyTypes).Wait();

            Task.Run(GetUnit).Wait();
            
            UnitFilter.Add(new UnitApi { UnitName = "Все" });
            SelectedUnitFilter = UnitFilter.Last();

            Task.Run(GetDiscount).Wait();

            SearchType = new List<string>();
            SearchType.AddRange(new string[] { "Характеристика" });
            SelectedSearchType = SearchType.First();

            SearchTypeEquipment = new List<string>();
            SearchTypeEquipment.AddRange(new string[] { "Комплектация" });
            SelectedSearchTypeEquipment = SearchTypeEquipment.First();

            AddCharacteristic = new CustomCommand(() =>
            {
                AddCharacteristicWindow addCharacteristic = new AddCharacteristicWindow();
                addCharacteristic.ShowDialog();
                Task.Run(GetCharacteristic).Wait();
            });

            EditCharacteristic = new CustomCommand(() =>
            {
                if(SelectedCharacteristic == null || SelectedCharacteristic.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана характеристика для редактирования" });
                    return;
                }
                AddCharacteristicWindow addCharacteristic = new AddCharacteristicWindow(SelectedCharacteristic);
                addCharacteristic.ShowDialog();
                Task.Run(GetCharacteristic).Wait();
            });

            AddUnit = new CustomCommand(() =>
            {
                AddUnitWindow addUnit = new AddUnitWindow();
                addUnit.ShowDialog();
                Task.Run(GetUnit).Wait();
                Task.Run(GetCharacteristic).Wait();
            });

            AddDiscount = new CustomCommand(() =>
            {
                AddDiscountWindow addDiscount = new AddDiscountWindow();
                addDiscount.ShowDialog();
                Task.Run(GetDiscount).Wait();
            });

            EditDiscount = new CustomCommand(() =>
            {
                if(SelectedDiscount == null || SelectedDiscount.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана скидка" });
                    return;
                }
                AddDiscountWindow addDiscount = new AddDiscountWindow(SelectedDiscount);
                addDiscount.ShowDialog();
                Task.Run(GetDiscount).Wait();
            });

            EditUnit = new CustomCommand(() =>
            {
                if (SelectedUnit == null || SelectedUnit.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана ед. измерения" });
                    return;
                }
                AddUnitWindow addUnit = new AddUnitWindow(SelectedUnit);
                addUnit.ShowDialog();
                Task.Run(GetUnit).Wait();
                Task.Run(GetCharacteristic).Wait();
            });

            AddEquipment = new CustomCommand(() =>
            {
                AddEquipmentWindow equipmentWindow = new AddEquipmentWindow();
                equipmentWindow.ShowDialog();
                Task.Run(GetEquipment).Wait();
            });

            EditEquipment = new CustomCommand(() =>
            {
                if (SelectedEquipment == null || SelectedEquipment.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана комплектация для редактирования" });
                    return;
                }
                AddEquipmentWindow addEquipment = new AddEquipmentWindow(SelectedEquipment);
                addEquipment.ShowDialog();
                Task.Run(GetEquipment).Wait();
            });

            AddType = new CustomCommand(() =>
            {
                AddBodyTypeWindow addBodyType = new AddBodyTypeWindow();
                addBodyType.ShowDialog();
                Task.Run(GetBodyTypes).Wait();
            });

            EditType = new CustomCommand(() =>
            {
                if (SelectedBodyType == null || SelectedBodyType.ID == 0)
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

        #region Characteristic
        private async Task GetCharacteristic()
        {
            UnitFilter = await Api.GetListAsync<List<UnitApi>>("Unit");
            UnitFilter.Add(new UnitApi { UnitName = "Все" });
            selectedUnitFilter = UnitFilter.Last();
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            FullTypes = Characteristics;       
        }
        private void UpdateList()
        {
            Characteristics = searchResult;
            SignalChanged(nameof(Characteristics));
        }
        private async Task Search()
        {
            var search = SearchText.ToLower();
            if (SelectedUnitFilter == null)
                SelectedUnitFilter = UnitFilter.Last();
            if (search == "")
                searchResult = await Api.SearchFilterAsync<List<CharacteristicApi>>(SelectedSearchType, "$", "Characteristic", SelectedUnitFilter.UnitName);
            else
                searchResult = await Api.SearchFilterAsync<List<CharacteristicApi>>(SelectedSearchType, search, "Characteristic", SelectedUnitFilter.UnitName);
            UpdateList();
        }
        #endregion
        #region GetDiscount
        private async Task GetDiscount()
        {
            Discounts = await Api.GetListAsync<List<DiscountApi>>("Discount");
            FullDiscounts = Discounts;
            SignalChanged(nameof(Discounts));
        }
        #endregion

        #region PhotoCars
        private async Task GetPhotoCar()
        {
            CarPhotos = await Api.GetListAsync<List<CarPhotoApi>>("CarPhoto");
            FullPhotoCars = CarPhotos;
            SignalChanged(nameof(CarPhotos));
        }

        private async Task DeletePhoto(CarPhotoApi carPhoto)
        {
            var photo = await Api.DeleteAsync<CarPhotoApi>(carPhoto, "CarPhoto");
        }

        #endregion
        
        #region Equipment
        private async Task GetEquipment()
        {
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
            FullEquipments = Equipments;
            SignalChanged(nameof(Equipments));
        }
        private void UpdateListEquipment()
        {
            Equipments = searchResultEquipment;
            SignalChanged(nameof(Equipments));
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
        #endregion

        #region BodyTypes
        public async Task GetBodyTypes()
        {
            BodyTypes = await Api.GetListAsync<List<BodyTypeApi>>("BodyTYpe");
            FullBodyTypes = BodyTypes;
            SignalChanged(nameof(BodyTypes));
        }

        private void UpdateListBody()
        {
            BodyTypes = searchResultBody;
            SignalChanged(nameof(BodyTypes));
        }

        public async Task SearchBodyTypes()
        {
            var search = SearchTextBody.ToLower();
            if (search == "")
                searchResultBody = await Api.GetListAsync<List<BodyTypeApi>>("BodyType");
            else
                searchResultBody = await Api.SearchAsync<List<BodyTypeApi>>("Кузов", search, "BodyType");
            UpdateListBody();
        }
        #endregion

        #region Units
        private async Task GetUnit()
        {
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
        }
        #endregion
    }
}
