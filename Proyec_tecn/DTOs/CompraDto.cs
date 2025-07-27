namespace Proyec_tecn.DTOs
{
    public class CompraDto
    {
        public int Id_CompraCab { get; set; }
        public DateTime FecRegistro { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
        public List<CompraDetDto> Detalles { get; set; } = new();
    }

    public class CompraDetDto
    {
        public int Id_producto { get; set; }
        public string Nombre_producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Sub_Total { get; set; }
        public decimal Igv { get; set; }
        public decimal Total { get; set; }
    }

    public class CreateCompraDto
    {
        public List<CreateCompraDetDto> Detalles { get; set; } = new();
    }

    public class CreateCompraDetDto
    {
        public int Id_producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}

