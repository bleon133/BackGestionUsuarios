using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class DireccionDto
    {
        public int Id { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string NumInterior { get; set; }
        public string NumExterior { get; set; }
        public string Municipio { get; set; }
        public int IdPerfilUsuario { get; set; }
        public PerfilUsuarioDto PerfilUsuarioDto { get; set; }
    }
}