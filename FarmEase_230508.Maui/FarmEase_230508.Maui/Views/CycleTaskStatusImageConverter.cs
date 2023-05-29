using FarmEase_230508.Maui.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.Views {
    public class CycleTaskStatusImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is CropCycleStatus status) {
                switch (status) {
                    case CropCycleStatus.NotStarted:
                        return "notstarted.png";
                    case CropCycleStatus.InProgress:
                        return "inprogress.png";
                    case CropCycleStatus.Completed:
                        return "completed.png";
                    case CropCycleStatus.Cancelled:
                        return "cancelled.png";
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
