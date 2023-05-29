using System.Globalization;

namespace FarmEase_230508.Maui.Views;

public class CropIdToImageConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value.ToString() is string status) {
            switch (status) {
                case "8eba8dea-9414-44b5-a83f-37262d4c3fcb":
                    return "corn.png";
                default:
                    return "corn.png";
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return null;
    }
}
