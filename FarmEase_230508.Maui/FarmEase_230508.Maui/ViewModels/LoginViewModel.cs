using Microsoft.Maui.Controls;
using System;

namespace FarmEase_230508.Maui.ViewModels {
    public class LoginViewModel : BaseViewModel {
        string userName;
        string password = string.Empty;
        string errorText;
        bool hasError;
        bool isAuthInProcess;

        public LoginViewModel() {
            LoginCommand = new Command(OnLoginClicked, ValidateLogin);
            AccountRequestCommand = new Command(OnAccountRequestClicked);
            PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }

        public string UserName {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }

        public string Password {
            get => this.password;
            set => SetProperty(ref this.password, value);
        }

        public string ErrorText {
            get => errorText;
            set => SetProperty(ref errorText, value);
        }

        public bool HasError {
            get => hasError;
            set => SetProperty(ref hasError, value);
        }

        public bool IsAuthInProcess {
            get => isAuthInProcess;
            set => SetProperty(ref isAuthInProcess, value);
        }

        public Command LoginCommand { get; }

        public Command AccountRequestCommand { get; }

        async void OnLoginClicked() {
            IsAuthInProcess = true;
            var response = await LoginService.Login(userName, password);
            IsAuthInProcess = false;

            if (!string.IsNullOrEmpty(response)) {
                ErrorText = response;
                HasError = true;
                return;
            }
            HasError = false;
            await Navigation.NavigateToAsync<AboutViewModel>(true);
        }

        bool ValidateLogin() {
            return !String.IsNullOrWhiteSpace(UserName)
                && !String.IsNullOrWhiteSpace(Password);
        }

        async void OnAccountRequestClicked()
                => await Shell.Current.DisplayAlert("Request Account", "Please ask your system administrator to register you in the corporate system", "OK");
    }
}