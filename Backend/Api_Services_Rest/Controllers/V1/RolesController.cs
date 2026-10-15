using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [Authorize]
    [RoutePrefix("api/v1/roles")]
    public class RolesController : ApiController
    {
        private RolesServices _rolesService;

        public RolesController()
        {
            _rolesService = new RolesServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _rolesService.Dispose();
            base.Dispose(disposing);
        }

        // GET api/v1/Roles
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var dto = AutoMapperConfig.Mapper.Map<List<RolesDto>>(_rolesService.GetAllRoles());
                return Json(dto);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // GET api/v1/Roles/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var rol = _rolesService.GetRolById(id);
                if (rol == null || rol.id == 0)
                    return NotFound();

                var dto = AutoMapperConfig.Mapper.Map<RolesDto>(rol);
                return Json(dto);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // GET api/v1/Roles/nombre/{valor}
        [HttpGet]
        [Route("nombre/{valor}")]
        public IHttpActionResult GetByNombre(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return BadRequest("El valor no puede estar vacío.");

            try
            {
                var rol = _rolesService.GetRolByValor(valor);
                if (rol == null)
                    return NotFound();

                var dto = AutoMapperConfig.Mapper.Map<RolesDto>(rol);
                return Json(dto);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // POST api/v1/Roles
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [Route("")]
        public IHttpActionResult Create([FromBody] RolesDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.roles>(dto);
                var result = _rolesService.CreateRol(model);
                if (!result)
                    return InternalServerError();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // PUT api/v1/Roles/{id}
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}")]
        public IHttpActionResult Update([FromUri] int id, [FromBody] RolesDto dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.roles>(dto);
                var result = _rolesService.UpdateRol(id, model);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _rolesService.DeleteRole(id);

                if (!result.Found)
                    return NotFound();

                if (!result.Success && result.Assignado)
                    return Content(HttpStatusCode.Conflict, new { message = result.Message });

                if (!result.Success)
                    return InternalServerError();

                return Ok(new { message = "Rol eliminado correctamente." });
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
