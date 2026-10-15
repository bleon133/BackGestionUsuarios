using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class UsuarioRolesDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El id de usuario debe ser mayor a 0.")]
        public int IdUsuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El id de rol debe ser mayor a 0.")]
        public int IdRol { get; set; }

        public RolesDto RolesDto { get; set; }
        public UsuariosDto UsuariosDto { get; set; }
    }
}