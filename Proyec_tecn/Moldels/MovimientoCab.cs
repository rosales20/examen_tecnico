using System.ComponentModel.DataAnnotations;

namespace Proyec_tecn.Moldels
{
    public class MovimientoCab
    {
        [Key]
        public int Id_MovimientoCab { get; set; }

        [Required]
        public DateTime Fec_registro { get; set; }

        [Required]
        public int Id_TipoMovimiento { get; set; } // 1: Entrada, 2: Salida

        [Required]
        public int Id_DocumentoOrigen { get; set; }

        // Navegación
        public virtual ICollection<MovimientoDet> MovimientoDets { get; set; } = new List<MovimientoDet>();
    }
}
