using System.ComponentModel.DataAnnotations;

namespace Api_Services_Rest.Models.Dto
{
    public class RolesDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El valor del rol es obligatorio.")]
        [StringLength(50, ErrorMessage = "El valor del rol no puede superar los 50 caracteres.")]
        public string Valor { get; set; }

        [StringLength(100, ErrorMessage = "La descripción no puede superar los 100 caracteres.")]
        public string Descripcion { get; set; }
    }
}