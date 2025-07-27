using System.ComponentModel.DataAnnotations;

namespace Proyec_tecn.Moldels
{
    public class VentaCab
    {
        [Key]
        public int Id_VentaCab { get; set; }

        [Required]
        public DateTime FecRegistro { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public decimal Igv { get; set; }

        [Required]
        public decimal Total { get; set; }

        // Navegación
        public virtual ICollection<VentaDet> VentaDets { get; set; } = new List<VentaDet>();
    }
}
