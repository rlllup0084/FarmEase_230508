using System.Net.Http.Headers;
using DevExpress.Maui.Core.Internal;
using Newtonsoft.Json;
using System.Text;
using FarmEase_230508.Maui.Models;

namespace FarmEase_230508.Maui.Services {
    public class LoginService : ILoginService {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string _apiUrl = ON.Platform(android: "http://192.168.1.102:5000/api/Authentication/", iOS: "http://192.168.1.102:5000/api/Authentication/");

        public LoginService() {
        }

        public async Task<string> Login(string email, string password) {
            var loginData = new LoginData {
                UserName = email,
                Password = password
            };

            var json = JsonConvert.SerializeObject(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await HttpClient.PostAsync($"{_apiUrl}Authenticate", content);

            var reposeContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode) {
                await SecureStorage.SetAsync("jwt_token", reposeContent);
                await SecureStorage.SetAsync("auth_id", loginData.UserName);
                return string.Empty;
            }

            return reposeContent;
        }

        public async Task<bool> Logout() {
            await Task.CompletedTask;

            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("auth_id");

            return true;
        }
    }
}
