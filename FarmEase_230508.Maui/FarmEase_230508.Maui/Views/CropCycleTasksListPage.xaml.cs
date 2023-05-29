using DevExpress.Maui.DataGrid;
using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views;

public partial class CropCycleTasksListPage : ContentPage
{
	public CropCycleTasksListPage()
	{
		InitializeComponent();
        BindingContext = new CropCycleTasksListViewModel();
	}

    private void grid_CustomCellAppearance(object sender, CustomCellAppearanceEventArgs e) {
        e.FontSize = 12;
        if (e.RowHandle % 2 == 0)
            e.BackgroundColor = Color.FromArgb("#F7F7F7");
        //if (e.FieldName == "Vendor") {
        //    e.FontAttributes = FontAttributes.Bold;
        //}
        //if (e.FieldName == "Total") {
        //    e.FontSize = 14;
        //    e.FontAttributes = FontAttributes.Bold;
        //}
    }
}