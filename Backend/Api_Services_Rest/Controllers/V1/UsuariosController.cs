using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Seguridad;
using Api_Services_Rest.Services.Usuario;
using Api_Services_Rest.Services.Usuario_Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [Authorize]
    [RoutePrefix("api/v1/Usuarios")]
    public class UsuariosController : ApiController
    {
        private UsuariosServices _usuariosServices;
        private UsuarioRolesServices _usuarioRolesServices;

        public UsuariosController()
        {
            _usuariosServices = new UsuariosServices();
            _usuarioRolesServices = new UsuarioRolesServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _usuariosServices.Dispose();
                _usuarioRolesServices.Dispose();
            }
            base.Dispose(disposing);
        }

        private static string GetClientIpAddress()
        {
            var context = HttpContext.Current;
            if (context != null && context.Request != null && !string.IsNullOrEmpty(context.Request.UserHostAddress))
                return context.Request.UserHostAddress;

            return null;
        }

        // GET api/v1/Usuarios
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var dto = AutoMapperConfig.Mapper.Map<List<UsuariosDto>>(_usuariosServices.GetAllUsuarios());
            return Json(dto);
        }

        // GET api/v1/Usuarios/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<UsuariosDto>(_usuariosServices.GetUsuarioById(id));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // GET api/v1/Usuarios/username/{username}
        [HttpGet]
        [Route("username/{username}")]
        public IHttpActionResult GetByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest("El username no puede estar vacío.");

            var dto = AutoMapperConfig.Mapper.Map<UsuariosDto>(_usuariosServices.GetUsuarioByUsername(username));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // POST api/v1/Usuarios
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] UsuariosDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.usuarios>(dto);
                if (string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("La contraseña es obligatoria.");

                var result = _usuariosServices.CreateUsuario(model);
                if (!result)
                    return Conflict(); // username ya existe

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // POST api/v1/Usuarios/login
        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Usuario y contraseña son obligatorios.");

            string attemptKey = LoginAttemptTracker.BuildKey(login.Username, GetClientIpAddress());

            if (LoginAttemptTracker.IsBlocked(attemptKey))
                return Content((HttpStatusCode)429, new { message = "Demasiados intentos fallidos. Inténtalo de nuevo más tarde." });

            // Sin un secreto válido no se pueden emitir tokens: nunca se usa uno por defecto.
            string secretError;
            if (!JwtService.IsConfigured(out secretError))
                return Content(HttpStatusCode.InternalServerError, new { message = "Error interno del servidor." });

            bool suspendido;
            var usuario = _usuariosServices.Autenticar(login.Username, login.Password, out suspendido);

            // Misma respuesta tanto si el usuario no existe como si la contraseña es incorrecta.
            if (usuario == null)
            {
                LoginAttemptTracker.RegisterFailure(attemptKey);
                return Content(HttpStatusCode.Unauthorized, new { message = "Usuario o contraseña incorrectos." });
            }

            if (suspendido)
                return Content(HttpStatusCode.Forbidden, new { message = "La cuenta está suspendida." });

            LoginAttemptTracker.Reset(attemptKey);

            var roles = _usuarioRolesServices.GetByIdUsuario(usuario.id)
                .Where(ur => ur.roles != null && !string.IsNullOrWhiteSpace(ur.roles.valor))
                .Select(ur => ur.roles.valor)
                .ToList();

            DateTime expiresAt;
            string token = JwtService.GenerateToken(usuario.id, usuario.username, roles, DateTime.UtcNow, out expiresAt);

            var dto = AutoMapperConfig.Mapper.Map<UsuariosDto>(usuario);
            dto.Password = null;

            return Ok(new
            {
                token = token,
                expiresAt = expiresAt.ToString("o"),
                usuario = dto
            });
        }

        // PUT api/v1/Usuarios/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update([FromUri] int id, [FromBody] UsuariosDto dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.usuarios>(dto);
                var result = _usuariosServices.UpdateUsuario(id, model);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE api/v1/Usuarios/{id}
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _usuariosServices.DeleteUsuario(id);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // PATCH api/v1/Usuarios/{id}/toggle-suspendido
        [HttpPatch]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}/toggle-suspendido")]
        public IHttpActionResult ToggleSuspendido([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _usuariosServices.ToggleSuspendido(id);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
