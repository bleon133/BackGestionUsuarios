using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class TelefonosDto
    {
        public int Id { get; set; }
        public string Celular { get; set; }
        public string Casa { get; set; }
        public string Oficina { get; set; }
        public int IdPerfilUsuario { get; set; }
        public PerfilUsuarioDto PerfilUsuarioDto { get; set; }
    }
}