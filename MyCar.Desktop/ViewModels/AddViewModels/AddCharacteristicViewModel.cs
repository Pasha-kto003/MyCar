using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
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
        public List<UnitApi> Units { get; set; }
        public UnitApi SelectedUnit { get; set; }
        public CharacteristicApi AddCharacteristicVM { get; set; }

        public CustomCommand SaveCharacteristic { get; set; }
        public CustomCommand Cancel { get; set; }
        public CustomCommand EditUnit { get; set; }

        public AddCharacteristicViewModel(CharacteristicApi characteristic)
        {
            Task.Run(GetList).Wait();

            if (characteristic == null)
            {
                AddCharacteristicVM = new CharacteristicApi();
            }

            else
            {
                AddCharacteristicVM = new CharacteristicApi
                {
                    ID = characteristic.ID,
                    UnitId = characteristic.UnitId,
                    Unit = characteristic.Unit,
                    CharacteristicName = characteristic.CharacteristicName
                };
                SelectedUnit = Units.FirstOrDefault(s => s.ID == characteristic.UnitId);
            }
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });

            EditUnit = new CustomCommand(() =>
            {
                if(SelectedUnit == null || SelectedUnit.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана ед. измерения!" });
                    return;
                }
                else
                {
                    AddUnitWindow addUnit = new AddUnitWindow(SelectedUnit);
                    addUnit.ShowDialog();
                    Task.Run(GetList).Wait();
                }
            });

            SaveCharacteristic = new CustomCommand(async () =>
            {
                if (SelectedUnit == null || SelectedUnit.ID == 0)
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не выбрана ед. измерения!" });
                    return;
                }
                AddCharacteristicVM.UnitId = SelectedUnit.ID;
                AddCharacteristicVM.Unit = SelectedUnit;
                if (AddCharacteristicVM.ID == 0)
                {
                    await CreateCharacteristic(AddCharacteristicVM);
                }
                else
                {
                    await EditCharacteristic(AddCharacteristicVM);
                }
                UIManager.CloseWindow(this);
            });
        }

        private async Task CreateCharacteristic(CharacteristicApi characteristic)
        {
            var characteristics = await Api.PostAsync<CharacteristicApi>(characteristic, "Characteristic");
        }

        private async Task EditCharacteristic(CharacteristicApi characteristic)
        {
            var characteristics = await Api.PutAsync<CharacteristicApi>(characteristic, "Characteristic");
        }

        private async Task GetList()
        {
            Units = await Api.GetListAsync<List<UnitApi>>("Unit");
            SignalChanged(nameof(Units));
        }
    }
}
