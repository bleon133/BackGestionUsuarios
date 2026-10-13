using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [RoutePrefix("api/v1/Usuarios")]
    public class UsuariosController : ApiController
    {
        private UsuariosServices _usuariosServices;

        public UsuariosController()
        {
            _usuariosServices = new UsuariosServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _usuariosServices.Dispose();
            base.Dispose(disposing);
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
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Usuario y contraseña son obligatorios.");

            bool suspendido;
            var usuario = _usuariosServices.Autenticar(login.Username, login.Password, out suspendido);

            if (usuario == null)
                return Content(HttpStatusCode.Unauthorized, new { message = "Usuario o contraseña incorrectos." });

            if (suspendido)
                return Content(HttpStatusCode.Forbidden, new { message = "La cuenta está suspendida." });

            return Ok(AutoMapperConfig.Mapper.Map<UsuariosDto>(usuario));
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
