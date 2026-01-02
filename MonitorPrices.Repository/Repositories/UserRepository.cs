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

    public User? RegisterByEmail(string email, string password)
    {
        // Verificar si el usuario ya existe
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == email);
        if (existingUser != null)
            return null;

        var user = new User
        {
            Email = email,
            PasswordHas = password //TODO: contraseña en texto plano...
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }
}
