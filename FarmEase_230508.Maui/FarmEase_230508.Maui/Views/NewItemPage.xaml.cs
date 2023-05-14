using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage {
        public NewItemPage() {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}