using ModelsApi;
using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyCar.Desktop.ViewModels
{
    public class ChooseSaleCarViewModel : BaseViewModel
    {
        public ObservableCollection<DataCar> DataCars = new ObservableCollection<DataCar>();

        public ObservableCollection<DataSaleCar> DataSaleCars = new ObservableCollection<DataSaleCar>();

        public ObservableCollection<DataSaleCarColor> DataSaleCarColors = new ObservableCollection<DataSaleCarColor>();

        public CollectionViewSource CarsCollectionView { get; set; } = new CollectionViewSource();


        public List<SaleCarApi> SaleCars = new List<SaleCarApi>();
        public ChooseSaleCarViewModel()
        {
            Task.Run(GetSales).Wait();
            CarsCollectionView.Source = SaleCars;
            CarsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Car.Model.MarkCar.MarkName"));
        }
        //public void GetData()
        //{
        //    foreach (var dataCar in SaleCars.GroupBy(p => p.CarId).Select(g => g.First()))
        //    {
        //        foreach (var dataSaleCar in SaleCars.Where(s=>s.CarId == dataCar.CarId).GroupBy(p => p.EquipmentId).Select(g => g.First()))
        //        {
        //            foreach (var dataSaleCarColor in SaleCars.Where(s => s.CarId == dataCar.CarId).GroupBy(p => p.EquipmentId).Select(g => g.First()).GroupBy(q=>q.ColorCar).Select(g => g.First()))
        //            {

        //            }
        //        }
        //        DataCars.Add(new DataCar { CarName = dataCar.Car.CarName, DataSaleCars = new  });
        //    }
        //}

        public async Task GetSales()
        {
            SaleCars = await Api.GetListAsync<List<SaleCarApi>>("CarSales");
        }

    }
}
