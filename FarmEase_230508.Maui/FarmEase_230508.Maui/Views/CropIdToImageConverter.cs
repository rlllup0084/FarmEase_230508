using System.Globalization;

namespace FarmEase_230508.Maui.Views;

public class CropIdToImageConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value.ToString() is string status) {
            switch (status) {
                case "8eba8dea-9414-44b5-a83f-37262d4c3fcb":
                    return "hold.png"; // replace with your current status image file name
                case "4cdc549d-d5dc-4440-ad87-530380401d35":
                    return "release.png"; // replace with your approve status image file name
                                          // add more cases for other status values if needed
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return null;
    }
}
