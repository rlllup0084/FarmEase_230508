using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmEase_230508.Maui.ViewModels {
    public class AccountViewModel : BaseViewModel {
        private string userName;
        public AccountViewModel() {
            LogoutCommand = new Command(ExecuteLogoutCommmand);
            UserName = SecureStorage.GetAsync("auth_id").Result;
        }

        async void ExecuteLogoutCommmand() {
            if (await LoginService.Logout()) {
                //await Navigation.NavigateToAsync<LoginViewModel>(true);
                await Shell.Current.GoToAsync("///login");
            }
        }

        public Command LogoutCommand { get; }

        public string UserName {
            get => this.userName;
            set => SetProperty(ref this.userName, value);
        }
    }
}
