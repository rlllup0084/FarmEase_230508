using FarmEase_230508.Maui.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Linq;

namespace FarmEase_230508.Maui.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage {
        public LoginPage() {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }

        protected override bool OnBackButtonPressed() {
            Application.Current.Quit();
            return true;
        }
    }
}