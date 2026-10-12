using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class UsuariosDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Suspendido { get; set; }
        public List<UsuarioRolesDto> UsuariosRolesDtos { get; set; }
        public List<PerfilUsuarioDto> PerfilUsuarioDtos { get; set; }
    }
}