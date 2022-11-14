using ModelsApi;
using MyCar.Desktop.Core;
using MyCar.Desktop.Core.UI;
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

            Task.Run(Get);

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

            SaveEquipment = new CustomCommand(() =>
            {

                foreach (var e in Equipments)
                {
                    if(e.NameEquipment == AddEquipmentVM.NameEquipment)
                    {
                        SendMessage("Такая комплектация уже существует!!!");
                    }
                }

                if(AddEquipmentVM.NameEquipment == "")
                {
                    SendMessage("Введите название комплектации!!!");
                }

                if(AddEquipmentVM.MinPrice == null)
                {
                    SendMessage("Введите цену комплектации!!!");
                }

                if (AddEquipmentVM.ID == 0)
                {
                    Add(AddEquipmentVM);
                }

                else
                {
                    Edit(AddEquipmentVM);
                }
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
