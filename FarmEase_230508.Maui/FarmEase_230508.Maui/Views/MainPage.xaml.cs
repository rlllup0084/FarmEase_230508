using FarmEase_230508.Maui.ViewModels;
using System.Runtime.CompilerServices;

namespace FarmEase_230508.Maui.Views {
    public partial class MainPage : Shell {
        public MainPage() {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
}