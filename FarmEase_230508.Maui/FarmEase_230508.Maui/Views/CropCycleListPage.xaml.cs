using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views;

public partial class CropCycleListPage : ContentPage
{
	public CropCycleListPage()
	{
		InitializeComponent();
        BindingContext = new CropCycleListViewModel();
    }
}