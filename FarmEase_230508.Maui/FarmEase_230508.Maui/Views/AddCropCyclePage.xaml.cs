using FarmEase_230508.Maui.Models;
using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views;

public partial class AddCropCyclePage : ContentPage
{
	public AddCropCyclePage()
	{
		InitializeComponent();
		BindingContext = new AddCropCycleViewModel();
    }
}