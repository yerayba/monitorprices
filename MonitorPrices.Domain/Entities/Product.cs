namespace MonitorPrices.Domain.Entities;

public class Product
{
    public int Id  { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public string PrecioUltimo { get; set; }
    public string PrecioActual { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime FechaUltimoDescuento { get; set; }
    public DateTime FechaUltimaComprobación { get; set; }
}