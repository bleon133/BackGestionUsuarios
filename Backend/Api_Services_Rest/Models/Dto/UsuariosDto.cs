using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class UsuariosDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede superar los 100 caracteres.")]
        public string Username { get; set; }

        [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Password { get; set; }

        public bool Suspendido { get; set; }
        public List<UsuarioRolesDto> UsuariosRolesDtos { get; set; }
        public List<PerfilUsuarioDto> PerfilUsuarioDtos { get; set; }
    }
}