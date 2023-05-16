using FarmEase_230508.Maui.Data;
using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views;

public partial class CropCycleListPage : ContentPage
{
	public CropCycleListPage()
	{
		InitializeComponent();
		CropCycleDatabase cropCycleDatabase = new CropCycleDatabase();
        BindingContext = new CropCycleListViewModel(cropCycleDatabase);
    }
}