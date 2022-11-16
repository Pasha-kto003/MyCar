using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
using MyCar.Desktop.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddUnitViewModel : BaseViewModel
    {
        public UnitApi AddUnitVM { get; set; }
        
        public CustomCommand Save { get; set; }
        public CustomCommand Cancel { get; set; }

        public AddUnitViewModel(UnitApi unit)
        {
            if(unit == null)
            {
                AddUnitVM = new UnitApi();
            }
            else
            {
                AddUnitVM = new UnitApi
                {
                    ID = unit.ID,
                    UnitName = unit.UnitName
                };
            }

            Save = new CustomCommand(() =>
            {

                if(AddUnitVM.UnitName == "")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Не введена единица измерения" });
                    return;
                }
                if(AddUnitVM.ID == 0)
                {
                    CreateUnit(AddUnitVM);
                }
                else
                {
                    EditUnit(AddUnitVM);
                }
                UIManager.CloseWindow(this);
            });
        }

        private async Task CreateUnit(UnitApi unitApi)
        {
            var unit = await Api.PostAsync<UnitApi>(unitApi, "Unit");
        }

        private async Task EditUnit(UnitApi unitApi)
        {
            var unit = await Api.PutAsync<UnitApi>(unitApi, "Unit");
        }
    }
}
