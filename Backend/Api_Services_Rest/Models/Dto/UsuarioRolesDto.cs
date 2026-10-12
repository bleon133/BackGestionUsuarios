using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class UsuarioRolesDto
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public RolesDto RolesDto { get; set; }
        public UsuariosDto UsuariosDto { get; set; }
    }
}