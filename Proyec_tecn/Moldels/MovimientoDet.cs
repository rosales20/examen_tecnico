using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyec_tecn.Moldels
{
    public class MovimientoDet
    {
        [Key]
        public int Id_MovimientoDet { get; set; }

        [Required]
        public int Id_movimientocab { get; set; }

        [Required]
        public int Id_Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        // Navegación
        [ForeignKey("Id_movimientocab")]
        public virtual MovimientoCab MovimientoCab { get; set; } = null!;

        [ForeignKey("Id_Producto")]
        public virtual Producto Producto { get; set; } = null!;
    }
}
