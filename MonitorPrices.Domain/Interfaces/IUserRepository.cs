
using MonitorPrices.Domain.Entities;

namespace MonitorPrices.Domain.Interfaces;

public interface IUserRepository
{
    User? GetByEmail(string email);
    User? RegisterByEmail(string email, string password);
   
}