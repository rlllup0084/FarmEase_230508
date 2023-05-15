namespace FarmEase_230508.Maui.Views;

public partial class LoadingPage : ContentPage
{
	public LoadingPage()
	{
		InitializeComponent();
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args) {
        if (await isAuthenticated()) {
            await Shell.Current.GoToAsync("///about");
        } else {
            await Shell.Current.GoToAsync("///login");
        }
        base.OnNavigatedTo(args);
    }

    async Task<bool> isAuthenticated() {
        await Task.Delay(2000);
        var hasAuth = await SecureStorage.GetAsync("jwt_token");
        return !(hasAuth == null);
    }
}