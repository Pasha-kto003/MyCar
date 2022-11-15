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
    public class AddCharacteristicViewModel : BaseViewModel
    {
        public List<UnitApi> AllUnits { get; set; } = new List<UnitApi>();
        public List<CharacteristicApi> Characteristics { get; set; } = new List<CharacteristicApi>();

        public UnitApi SelectedUnitThis { get; set; }

        public UnitApi SelectedUnit { get; set; }
        public CharacteristicApi AddCharacteristicVM { get; set; }

        private string searchText = "";
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                SeacrhUnit();
            }
        }

        private string selectedSearchType { get; set; }
        public string SelectedSearchType
        {
            get => selectedSearchType;
            set
            {
                selectedSearchType = value;
                SeacrhUnit();
            }
        }

        private List<UnitApi> searchResult;
        private List<UnitApi> FullUnits;

        public ObservableCollection<UnitApi> SelectedUnits { get; set; } = new ObservableCollection<UnitApi>();

        public string NameUnit { get; set; }

        public CustomCommand Cancel { get; set; }
        public CustomCommand Save { get; set; }
        public CustomCommand AddUnit { get; set; }
        public CustomCommand RemoveUnit { get; set; }
        public CustomCommand CreateUnit { get; set; }

        public AddCharacteristicViewModel(CharacteristicApi characteristic)
        {

            SelectedSearchType = "Единица измерения";
            Task.Run(GetList);

            GetCharacteristic(characteristic);

            if (characteristic == null)
            {
                AddCharacteristicVM = new CharacteristicApi();
            }
            else
            {
                AddCharacteristicVM = new CharacteristicApi
                {
                    ID = characteristic.ID,
                    CharacteristicName = characteristic.CharacteristicName,
                    UnitId = characteristic.UnitId
                };

                GetCharacteristic(AddCharacteristicVM);

                SelectedUnitThis = AddCharacteristicVM.Unit;
            }

            CreateUnit = new CustomCommand(() =>
            {
                if(SelectedUnit == null)
                {
                    AddUnitWindow unitWindow = new AddUnitWindow();
                    unitWindow.ShowDialog();
                    GetList();
                    SignalChanged(nameof(SelectedUnit));
                }
                else
                {
                    AddUnitWindow unitWindow = new AddUnitWindow(SelectedUnit);
                    unitWindow.ShowDialog();
                    GetList();
                    SignalChanged(nameof(SelectedUnit));
                }
            });

            AddUnit = new CustomCommand(() =>
            {
                if(SelectedUnit == null)
                {
                    SendMessage("Вы не выбрали единицу измерения");
                }
                else
                {
                    AddCharacteristicVM.Unit = SelectedUnit;
                    AddCharacteristicVM.Unit.UnitName = SelectedUnit.UnitName;
                    SignalChanged(AddCharacteristicVM.Unit.UnitName);
                    SelectedUnits.Add(SelectedUnit);
                    SignalChanged(nameof(SelectedUnits));

                    if (SelectedUnits.Count > 1)
                    {
                        GetUnit();
                    }
                }
            });
            Save = new CustomCommand(() =>
            {

                if (AddCharacteristicVM.CharacteristicName == "")
                {
                    SendMessage("Не введена характеристика");
                }

                if (AddCharacteristicVM.UnitId == 0)
                {
                    SendMessage("Вы не выбрали единицу измерения");
                }

                if(AddCharacteristicVM.ID == 0)
                {
                    foreach (var e in Characteristics)
                    {
                        if (e.CharacteristicName == AddCharacteristicVM.CharacteristicName)
                        {
                            SendMessage("Такая характеристика уже существует!!!");
                        }
                        return;
                    }
                    CreateCharacteristic(AddCharacteristicVM);
                    MessageBox.Show("Создана новая характеристика");
                }
                else
                {
                    EditCharacteristic(AddCharacteristicVM);
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

        public async Task GetUnit()
        {
            MessageBoxResult result = MessageBox.Show("Вы точно желаете заменить свойство?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SelectedUnits.Clear();
                AddCharacteristicVM.Unit = SelectedUnit;
                SelectedUnits.Add(SelectedUnit);
                SignalChanged(nameof(SelectedUnits));
                AddCharacteristicVM.UnitId = SelectedUnit.ID;
                EditCharacteristic(AddCharacteristicVM);
            }
        }

        public async Task CreateCharacteristic(CharacteristicApi characteristicApi)
        {
            var characteristic = await Api.PostAsync<CharacteristicApi>(characteristicApi, "Characteristic");
        }

        public async Task EditCharacteristic(CharacteristicApi characteristicApi)
        {
            var characteristic = await Api.PutAsync<CharacteristicApi>(characteristicApi, "Characteristic");
        }

        private async Task GetList()
        {
            Characteristics = await Api.GetListAsync<List<CharacteristicApi>>("Characteristic");
            var list = await Api.GetListAsync<List<UnitApi>>("Unit");
            AllUnits = new List<UnitApi>(list);
            FullUnits = AllUnits;
            SignalChanged(nameof(AllUnits));
        }

        public async Task GetCharacteristic(CharacteristicApi characteristicApi)
        {
            
            AllUnits = await Api.GetListAsync<List<UnitApi>>("Unit");
            AddCharacteristicVM.Unit = SelectedUnit;

            if (characteristicApi == null)
            {
                SelectedUnits = new ObservableCollection<UnitApi>();
                SelectedUnit = AllUnits.FirstOrDefault();
                SelectedUnitThis = SelectedUnits.FirstOrDefault();
            }

            else
            {
                GetUnits(characteristicApi);
                //SelectedUnits = AllUnits.Where(s => s.ID == characteristicApi.UnitId);
            }

            SignalChanged(nameof(SelectedUnits));
            SelectedUnitThis = SelectedUnits.FirstOrDefault(s => s.ID == characteristicApi.UnitId);
            SignalChanged(nameof(SelectedUnit));
        }

        private async Task SeacrhUnit()
        {
            var search = SearchText.ToLower();
            searchResult = await Api.SearchAsync<List<UnitApi>>(SelectedSearchType, search, "Unit");
            UpdateList();
        }

        public async Task GetUnits(CharacteristicApi characteristic)
        {
            var list = await Api.GetListAsync<List<UnitApi>>("Unit");
            var l = list.Where(s=> s.ID == characteristic.UnitId);
            SelectedUnits = new ObservableCollection<UnitApi>(l);
            SignalChanged(nameof(SelectedUnits));
        }

        private async Task UpdateList()
        {
            AllUnits = searchResult;
            SignalChanged(nameof(searchResult));
        }

        public void CloseWindow(object obj)
        {
            Window window = obj as Window;
            window.Close();
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
    }
}
