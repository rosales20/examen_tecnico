namespace Proyec_tecn.DTOs
{
    public class KardexDto
    {
        public int Id_producto { get; set; }
        public string Nombre_producto { get; set; } = string.Empty;
        public int Stock_actual { get; set; }
        public decimal Costo { get; set; }
        public decimal PrecioVenta { get; set; }
    }


    public class MovimientoProductoDto
    {
        public DateTime Fecha_registro { get; set; }
        public string Tipo_Movimiento { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}

