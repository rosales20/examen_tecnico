using System.ComponentModel.DataAnnotations;

namespace Proyec_tecn.Moldels
{
    public class Producto
    {
        
        [Key]
        public int Id_producto { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre_producto { get; set; } = string.Empty;

        [StringLength(50)]
        public string NroLote { get; set; } = string.Empty;

        public DateTime Fec_registro { get; set; }

        [Required]
        public decimal Costo { get; set; }

        [Required]
        public decimal PrecioVenta { get; set; }

        // Navegación
        public virtual ICollection<CompraDet> CompraDets { get; set; } = new List<CompraDet>();
        public virtual ICollection<VentaDet> VentaDets { get; set; } = new List<VentaDet>();
        public virtual ICollection<MovimientoDet> MovimientoDets { get; set; } = new List<MovimientoDet>();
    

}
}
