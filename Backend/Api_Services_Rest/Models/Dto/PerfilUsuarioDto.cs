using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class PerfilUsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Rfc { get; set; }
        public int IdUsuario { get; set; }
        public List<DireccionDto> DireccionDtos { get; set; }
        public List<TelefonosDto> TelefonosDtos { get; set; }
    }
}