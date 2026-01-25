using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MonitorPrices.Domain.Entities;
using MonitorPrices.Repository;

public interface IPriceMonitorService
{
    Task UpdateAllPricesAsync(CancellationToken ct);
    Task UpdatePriceAsync(Productos product);
}

