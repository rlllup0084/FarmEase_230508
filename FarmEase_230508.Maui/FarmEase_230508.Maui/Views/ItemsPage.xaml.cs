﻿using FarmEase_230508.Maui.Models;
using FarmEase_230508.Maui.ViewModels;

namespace FarmEase_230508.Maui.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage {
        public ItemsPage() {
            InitializeComponent();
            BindingContext = ViewModel = new ItemsViewModel();
            ViewModel.OnAppearing();
        }

        ItemsViewModel ViewModel { get; }

        protected override void OnAppearing() {
            base.OnAppearing();
            ViewModel.OnAppearing();
        }
    }
}