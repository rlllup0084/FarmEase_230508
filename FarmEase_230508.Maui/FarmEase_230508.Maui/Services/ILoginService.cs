namespace FarmEase_230508.Maui.Services {
    public interface ILoginService {
        Task<string> Login(string email, string password);

        Task<bool> Logout();
    }
}
