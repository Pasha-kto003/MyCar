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
    public class AddEquipmentViewModel : BaseViewModel
    {

        public List<EquipmentApi> Equipments { get; set; } = new List<EquipmentApi>();

        public CustomCommand SaveEquipment { get; set; }
        public CustomCommand Cancel { get; set; }

        public EquipmentApi AddEquipmentVM { get; set; }

        public AddEquipmentViewModel(EquipmentApi equipment)
        {

            Task.Run(Get).Wait();

            if (equipment == null)
            {
                AddEquipmentVM = new EquipmentApi();
            }

            else
            {
                AddEquipmentVM = new EquipmentApi
                {
                    ID = equipment.ID,
                    MinPrice = equipment.MinPrice,
                    NameEquipment = equipment.NameEquipment
                };
            }

            SaveEquipment = new CustomCommand( async () =>
            {
                foreach (var e in Equipments)
                {
                    if(e.NameEquipment == AddEquipmentVM.NameEquipment)
                    {
                        UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Такая комплектация уже существует!" });
                        return;
                    }
                }
                if(AddEquipmentVM.NameEquipment == "")
                {
                    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Введите название комплектации!" });
                    return;
                }
                //if (AddEquipmentVM.MinPrice == null)
                //{
                //    UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = "Введите цену комплектации!" });
                //    return;
                //}

                if (AddEquipmentVM.ID == 0)
                {
                    await Add(AddEquipmentVM); 
                }
                else
                {
                    await Edit(AddEquipmentVM);
                }           
                UIManager.CloseWindow(this);
            });
            Cancel = new CustomCommand(() =>
            {
                UIManager.CloseWindow(this);
            });
        }

        private async Task Add(EquipmentApi equipmentApi)
        {
            var equipment = await Api.PostAsync<EquipmentApi>(equipmentApi, "Equipment");
        }

        private async Task Edit(EquipmentApi equipmentApi)
        {
            var equipment = await Api.PutAsync<EquipmentApi>(equipmentApi, "Equipment");
        }

        private async Task Get()
        {
            Equipments = await Api.GetListAsync<List<EquipmentApi>>("Equipment");
        }

        
    }
}
