using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsApi;
using System.Windows.Forms;
using System.IO;
using MyCar.Desktop.ViewModels.Dialogs;
using MyCar.Desktop.Core.UI;

namespace MyCar.Desktop.ViewModels.AddViewModels
{
    public class AddPhotoCarViewModel : BaseViewModel
    {
        public string ImageSaleCar { get; set; }

        public CustomCommand SaveImage { get; set; }

        public CarPhotoApi AddCarPhoto { get; set; }

        public AddPhotoCarViewModel(CarPhotoApi carPhoto)
        {
            if(carPhoto == null)
            {
                AddCarPhoto = new CarPhotoApi
                {
                    PhotoName = "picture.png"
                };
            }
            else
            {
                AddCarPhoto = new CarPhotoApi
                {
                    ID = carPhoto.ID,
                    PhotoName = carPhoto.PhotoName,
                    SaleCarId = carPhoto.SaleCarId,
                };

                ImageSaleCar = AddCarPhoto.PhotoName;
            }

            string dir = Environment.CurrentDirectory;
            SaveImage = new CustomCommand(async () =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = dir + @"\CarImages\";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var info = new FileInfo(openFileDialog.FileName);
                        var newPath = Environment.CurrentDirectory + @"\CarImages\" + info.Name;
                        if (File.Exists(newPath))
                        {
                            MessageBoxDialogViewModel result = new MessageBoxDialogViewModel
                            { Title = "Подтверждение", Message = $"Файл с именем {info.Name} уже содержится в папке назначения\n                  Назначить уже существующий файл?" };
                            UIManager.ShowMessageYesNo(result);
                            if (!result.Result)
                            {
                                return;
                            }
                        }
                        else
                        {
                            File.Copy(openFileDialog.FileName, newPath);
                        }
                        ImageSaleCar = info.Name;
                        AddCarPhoto.PhotoName = info.Name;
                    }
                    catch (Exception e)
                    {
                        UIManager.ShowErrorMessage(new MessageBoxDialogViewModel { Message = e.Message });
                    }
                }

                if(AddCarPhoto.ID == 0)
                {
                    await CreateCarPhoto(AddCarPhoto);
                }
                else
                {
                    await EditCarPhoto(AddCarPhoto);
                }

            });
        }

        private async Task CreateCarPhoto(CarPhotoApi carPhoto)
        {
            var photo = await Api.PostAsync<CarPhotoApi>(carPhoto, "CarPhoto");
        }

        private async Task EditCarPhoto(CarPhotoApi carPhoto)
        {
            var photo = await Api.PutAsync<CarPhotoApi>(carPhoto, "CarPhoto");
        }
    }
}
