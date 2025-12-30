namespace MonitorPrices.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? PasswordHas { get; set; }
    public string? Role { get; set; }
}