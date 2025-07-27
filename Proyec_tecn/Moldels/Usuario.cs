using System.ComponentModel.DataAnnotations;

namespace Proyec_tecn.Moldels
{
    public class Usuario
    {
       
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; } = true;
    }
}

