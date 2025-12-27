namespace MonitorPrices.Repository.Entities;

public class Product
{
    public int Id  { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime FechaUltimoDescuento { get; set; }
}