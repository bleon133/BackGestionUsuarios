using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Usuario_Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [Authorize]
    [RoutePrefix("api/v1/UsuarioRoles")]
    public class UsuarioRolesController : ApiController
    {
        private UsuarioRolesServices _usuarioRolesServices;

        public UsuarioRolesController()
        {
            _usuarioRolesServices = new UsuarioRolesServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _usuarioRolesServices.Dispose();
            base.Dispose(disposing);
        }

        // GET api/v1/UsuarioRoles
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var dto = AutoMapperConfig.Mapper.Map<List<UsuarioRolesDto>>(_usuarioRolesServices.GetAll());
            return Json(dto);
        }

        // GET api/v1/UsuarioRoles/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<UsuarioRolesDto>(_usuarioRolesServices.GetById(id));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // GET api/v1/UsuarioRoles/usuario/{idUsuario}
        [HttpGet]
        [Route("usuario/{idUsuario:int}")]
        public IHttpActionResult GetByIdUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                return BadRequest("El ID de usuario debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<List<UsuarioRolesDto>>(_usuarioRolesServices.GetByIdUsuario(idUsuario));
            return Json(dto);
        }

        // POST api/v1/UsuarioRoles
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("")]
        public IHttpActionResult Create([FromBody] UsuarioRolesDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.usuariosRoles>(dto);
                var result = _usuarioRolesServices.Create(model);
                if (!result)
                    return Conflict(); // ya existe esa combinación usuario-rol

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE api/v1/UsuarioRoles/{id}
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _usuarioRolesServices.Delete(id);
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
