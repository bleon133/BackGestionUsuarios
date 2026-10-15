using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class TelefonosDto
    {
        public int Id { get; set; }

        [StringLength(10, ErrorMessage = "El celular no puede superar los 10 dígitos.")]
        [RegularExpression(@"^\d*$", ErrorMessage = "El celular solo puede contener dígitos.")]
        public string Celular { get; set; }

        [StringLength(10, ErrorMessage = "El teléfono de casa no puede superar los 10 dígitos.")]
        [RegularExpression(@"^\d*$", ErrorMessage = "El teléfono de casa solo puede contener dígitos.")]
        public string Casa { get; set; }

        [StringLength(10, ErrorMessage = "El teléfono de oficina no puede superar los 10 dígitos.")]
        [RegularExpression(@"^\d*$", ErrorMessage = "El teléfono de oficina solo puede contener dígitos.")]
        public string Oficina { get; set; }

        public int IdPerfilUsuario { get; set; }
        public PerfilUsuarioDto PerfilUsuarioDto { get; set; }
    }
}