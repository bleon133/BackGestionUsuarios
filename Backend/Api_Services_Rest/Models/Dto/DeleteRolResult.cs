using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Models.Dto
{
    public class DeleteRolResult
    {
        public bool Success { get; private set; }
        public bool Found { get; private set; }
        public bool Assignado { get; private set; }
        public string Message { get; private set; }

        // Eliminado correctamente
        public static DeleteRolResult Ok() =>
            new DeleteRolResult { Success = true, Found = true };

        // No encontrado
        public static DeleteRolResult NotFound() =>
            new DeleteRolResult
            {
                Success = false,
                Found = false,
                Message = "El rol no existe."
            };

        // Asignado a usuarios → no se puede eliminar
        public static DeleteRolResult Conflict(System.Collections.Generic.List<string> usernames) =>
            new DeleteRolResult
            {
                Success = false,
                Found = true,
                Assignado = true,
                Message = $"No se puede eliminar. El rol está asignado a: {string.Join(", ", usernames)}."
            };

        // Error inesperado
        public static DeleteRolResult Error(string detail) =>
            new DeleteRolResult
            {
                Success = false,
                Found = true,
                Message = $"Error interno: {detail}"
            };
    }
}