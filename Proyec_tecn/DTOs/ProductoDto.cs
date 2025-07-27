namespace Proyec_tecn.DTOs
{
    public class ProductoDto
    {
        public int Id_producto { get; set; }
        public string Nombre_producto { get; set; } = string.Empty;
        public string NroLote { get; set; } = string.Empty;
        public DateTime Fec_registro { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
    }

    public class CreateProductoDto
    {
        public string Nombre_producto { get; set; } = string.Empty;
        public string NroLote { get; set; } = string.Empty;
        public decimal Costo { get; set; }
    }

    public class UpdateProductoDto
    {
        public int Id_producto { get; set; }
        public string Nombre_producto { get; set; } = string.Empty;
        public string NroLote { get; set; } = string.Empty;
        public decimal Costo { get; set; }
    }
}
