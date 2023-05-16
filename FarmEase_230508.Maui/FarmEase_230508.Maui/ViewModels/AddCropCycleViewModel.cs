using FarmEase_230508.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.ViewModels {
    public enum CropTypeEnum {
        Corn,
        Cassava
    }
    public class AddCropCycleViewModel : BaseViewModel {
        private DateTime startDate = DateTime.Now;
        public List<CropData> Crops { get; }
        public AddCropCycleViewModel() {
            Title = "Add Crop Cycle";
            Crops = new List<CropData> {
                new CropData("Corn", "The crop cycle schedule for corn can vary depending on various factors such as the specific corn variety, climate, and local growing conditions.", "8eba8dea-9414-44b5-a83f-37262d4c3fcb"),
                new CropData("Cassava", "The crop cycle schedule for cassava can vary depending on various factors such as climate, cultivar, and intended use of the crop.", "4cdc549d-d5dc-4440-ad87-530380401d35")
            };
        }

        public DateTime StartDate {
            get => startDate;
            set => SetProperty(ref startDate, value);
        }
    }
}
