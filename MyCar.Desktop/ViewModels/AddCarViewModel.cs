using ModelsApi;
using MyCar.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCar.Desktop.ViewModels
{
    public class AddCarViewModel : BaseViewModel
    {
        public CarApi AddCarVM { get; set; }

        public List<MarkCarApi> Marks = new List<MarkCarApi>();
        public List<ModelApi> Models = new List<ModelApi>();

        private ModelApi selectedModel { get; set; }
        public ModelApi SelectedModel
        {
            get => selectedModel;
            set
            {
                selectedModel = value;
                SignalChanged();
            }
        }

        private MarkCarApi selectedMark { get; set; }
        public MarkCarApi SelectedMark 
        {
            get => selectedMark;
            set
            {
                selectedMark = value;
                SignalChanged();
            }
        }

        public AddCarViewModel(CarApi car)
        {
            if(car == null)
            {

            }

            else
            {

            }
        }
    }
}
