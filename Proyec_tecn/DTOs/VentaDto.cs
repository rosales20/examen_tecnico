namespace Proyec_tecn.DTOs
{
    public class VentaDto
    {
        
        public int Id_VentaCab { get; set; }
        public DateTime FecRegistro { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public List<VentaDetDto> Detalles { get; set; } = new();
    }

    public class VentaDetDto
    {
        public int Id_producto { get; set; }
        public string Nombre_producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public int Stock { get; set; }
    }

    public class CreateVentaDto
    {
        public List<CreateVentaDetDto> Detalles { get; set; } = new();
    }

    public class CreateVentaDetDto
    {
        public int Id_producto { get; set; }
        public int Cantidad { get; set; }
    }

}
