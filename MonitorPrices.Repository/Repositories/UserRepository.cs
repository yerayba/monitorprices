using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Domain.Interfaces;
namespace MonitorPrices.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MonitorPricedbContext _context;

    public UserRepository(MonitorPricedbContext context)
    {
        _context = context;
    }

    public User? GetByEmail(string email)
    {
        return _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email == email);
    }
}