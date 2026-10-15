using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El usuario no puede superar los 100 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Password { get; set; }
    }
}
