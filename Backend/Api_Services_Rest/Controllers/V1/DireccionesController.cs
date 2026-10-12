using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Direccion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [RoutePrefix("api/v1/Direcciones")]
    public class DireccionesController : ApiController
    {
        private DireccionServices _direccionServices;

        public DireccionesController()
        {
            _direccionServices = new DireccionServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _direccionServices.Dispose();
            base.Dispose(disposing);
        }

        // GET api/v1/Direcciones
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetDirecciones()
        {
            var dto = AutoMapperConfig.Mapper.Map<List<DireccionDto>>(_direccionServices.GetDirecciones());
            return Json(dto);
        }

        // GET api/v1/Direcciones/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetDireccionById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var direccion = AutoMapperConfig.Mapper.Map<DireccionDto>(_direccionServices.GetDireccionById(id));

            if (direccion == null)
                return NotFound();

            return Json(direccion);
        }

        // POST api/v1/Direcciones
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] DireccionDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.direcciones>(dto);
                var result = _direccionServices.CreateDireccion(model);
                if (!result)
                    return InternalServerError(new Exception("No se pudo crear la dirección."));

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // PUT api/v1/Direcciones/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update([FromUri] int id, [FromBody] DireccionDto dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.direcciones>(dto);
                var result = _direccionServices.UpdateDireccion(id, model);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE api/v1/Direcciones/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _direccionServices.DeleteDireccion(id);
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
