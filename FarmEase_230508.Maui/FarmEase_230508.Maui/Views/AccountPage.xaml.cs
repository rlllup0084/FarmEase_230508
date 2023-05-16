using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views;

public partial class AccountPage : ContentPage
{
	public AccountPage()
	{
		InitializeComponent();
        BindingContext = new AccountViewModel();
    }
}