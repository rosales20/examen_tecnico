using System.ComponentModel.DataAnnotations;

namespace Proyec_tecn.Moldels
{
    public class CompraCab
    {
        [Key]
        public int Id_CompraCab { get; set; }

        [Required]
        public DateTime FecRegistro { get; set; }

        [Required]
        public decimal SubTotal { get; set; }

        [Required]
        public decimal Igv { get; set; }

        [Required]
        public decimal Total { get; set; }

        // Navegación
        public virtual ICollection<CompraDet> CompraDets { get; set; } = new List<CompraDet>();
    }
}
