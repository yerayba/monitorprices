namespace MonitorPrices.Domain.Entities;

public class Productos
{
    public Guid Id  { get; set; }
    public string Asin { get; set; } 
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string UltimoPrecio { get; set; }
    public string PrecioActual { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime FechaUltimoDescuento { get; set; }
    public DateTime FechaUltimaComprobación { get; set; }
}