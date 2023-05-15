using FarmEase_230508.Maui.Services;
using FarmEase_230508.Maui.Views;
using Application = Microsoft.Maui.Controls.Application;

namespace FarmEase_230508.Maui {
    public partial class App : Application {
        public App() {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<NavigationService>();
            DependencyService.Register<LoginService>();

            Routing.RegisterRoute(typeof(LoginPage).FullName, typeof(LoginPage));
            Routing.RegisterRoute(typeof(ItemDetailPage).FullName, typeof(ItemDetailPage));
            Routing.RegisterRoute(typeof(NewItemPage).FullName, typeof(NewItemPage));
            MainPage = new MainPage();

            AppTheme currentTheme = Application.Current.RequestedTheme;
            if (currentTheme == AppTheme.Dark) {
                LoadTheme(currentTheme);
            } else {
                LoadTheme(AppTheme.Light);
            }

            // Use the NavigateToAsync<ViewModelName> method
            // to display the corresponding view.
            // Code lines below show how to navigate to a specific page.
            // Comment out all but one of these lines
            // to open the corresponding page.
            //var navigationService = DependencyService.Get<INavigationService>();
            //navigationService.NavigateToAsync<LoginViewModel>(true);
        }

        private void LoadTheme(AppTheme theme) {
            if (!MainThread.IsMainThread) {
                MainThread.BeginInvokeOnMainThread(() => LoadTheme(theme));
                return;
            }

            ResourceDictionary dictionary = theme switch {
                AppTheme.Dark => new Themes.Dark(),
                AppTheme.Light => new Themes.Light(),
                _ => null
            };

            if (dictionary != null) {
                Resources.MergedDictionaries.Clear();

                Resources.MergedDictionaries.Add(dictionary);
            }
        }
    }
}
