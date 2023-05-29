using System.Windows.Input;

namespace FarmEase_230508.Maui.ViewModels {
    public class AboutViewModel : BaseViewModel {
        public const string ViewName = "AboutPage";
        public AboutViewModel() {
            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.farmease.ph/"));
        }

        public ICommand OpenWebCommand { get; }
    }
}