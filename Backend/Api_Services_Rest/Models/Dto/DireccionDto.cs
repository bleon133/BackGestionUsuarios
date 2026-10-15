using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class DireccionDto
    {
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "La calle no puede superar los 100 caracteres.")]
        public string Calle { get; set; }

        [StringLength(150, ErrorMessage = "La colonia no puede superar los 150 caracteres.")]
        public string Colonia { get; set; }

        [StringLength(30, ErrorMessage = "El número interior no puede superar los 30 caracteres.")]
        public string NumInterior { get; set; }

        [StringLength(30, ErrorMessage = "El número exterior no puede superar los 30 caracteres.")]
        public string NumExterior { get; set; }

        [StringLength(150, ErrorMessage = "El municipio no puede superar los 150 caracteres.")]
        public string Municipio { get; set; }

        public int IdPerfilUsuario { get; set; }
        public PerfilUsuarioDto PerfilUsuarioDto { get; set; }
    }
}