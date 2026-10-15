using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class PerfilUsuarioDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(100, ErrorMessage = "El apellido paterno no puede superar los 100 caracteres.")]
        public string ApellidoPaterno { get; set; }

        [StringLength(100, ErrorMessage = "El apellido materno no puede superar los 100 caracteres.")]
        public string ApellidoMaterno { get; set; }

        public System.DateTime FechaNacimiento { get; set; }

        [StringLength(30, ErrorMessage = "El RFC no puede superar los 30 caracteres.")]
        [RegularExpression(@"^[A-Za-zÑñ&]{3,4}\d{6}[A-Za-z0-9]{3}$",
            ErrorMessage = "El RFC no tiene un formato válido.")]
        public string Rfc { get; set; }

        public int IdUsuario { get; set; }
        public List<DireccionDto> DireccionDtos { get; set; }
        public List<TelefonosDto> TelefonosDtos { get; set; }
    }
}