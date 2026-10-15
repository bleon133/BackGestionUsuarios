using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Perfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [Authorize]
    [RoutePrefix("api/v1/PerfilesUsuarios")]
    public class PerfilesUsuariosController : ApiController
    {
        private PerfilUsuarioServices _perfilServices;

        public PerfilesUsuariosController()
        {
            _perfilServices = new PerfilUsuarioServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _perfilServices.Dispose();
            base.Dispose(disposing);
        }

        // GET api/v1/PerfilUsuario
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var dto = AutoMapperConfig.Mapper.Map<List<PerfilUsuarioDto>>(_perfilServices.GetAllPerfiles());
            return Json(dto);
        }

        // GET api/v1/PerfilUsuario/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<PerfilUsuarioDto>(_perfilServices.GetPerfilById(id));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // GET api/v1/PerfilUsuario/usuario/{idUsuario}
        [HttpGet]
        [Route("usuario/{idUsuario:int}")]
        public IHttpActionResult GetByIdUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                return BadRequest("El ID de usuario debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<PerfilUsuarioDto>(_perfilServices.GetPerfilByIdUsuario(idUsuario));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // POST api/v1/PerfilUsuario
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] PerfilUsuarioDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.perfilUsuario>(dto);
                var result = _perfilServices.CreatePerfil(model);
                if (!result)
                    return InternalServerError();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // PUT api/v1/PerfilUsuario/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update([FromUri] int id, [FromBody] PerfilUsuarioDto dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.perfilUsuario>(dto);
                var result = _perfilServices.UpdatePerfil(id, model);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE api/v1/PerfilUsuario/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _perfilServices.DeletePerfil(id);
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
