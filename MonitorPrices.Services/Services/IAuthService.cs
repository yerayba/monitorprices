namespace MonitorPrices.Services.Services;

public interface IAuthService
{
    string Login(string email, string password);
    string Register(string email, string password);
}
