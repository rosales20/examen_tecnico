using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyec_tecn.Moldels
{
    public class VentaDet
    {
        [Key]
        public int Id_VentaDet { get; set; }

        [Required]
        public int Id_VentaCab { get; set; }

        [Required]
        public int Id_producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public decimal Sub_Total { get; set; }

        [Required]
        public decimal Igv { get; set; }

        [Required]
        public decimal Total { get; set; }

        // Navegación
        [ForeignKey("Id_VentaCab")]
        public virtual VentaCab VentaCab { get; set; } = null!;

        [ForeignKey("Id_producto")]
        public virtual Producto Producto { get; set; } = null!;
    }
}
