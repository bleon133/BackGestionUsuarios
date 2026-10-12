using Api_Services_Rest.Mapper;
using Api_Services_Rest.Models.Dto;
using Api_Services_Rest.Services.Telefono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api_Services_Rest.Controllers.V1
{
    [RoutePrefix("api/v1/Telefonos")]
    public class TelefonosController : ApiController
    {
        private TelefonoServices _telefonoServices;

        public TelefonosController()
        {
            _telefonoServices = new TelefonoServices();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _telefonoServices.Dispose();
            base.Dispose(disposing);
        }

        // GET api/v1/Telefonos
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var dto = AutoMapperConfig.Mapper.Map<List<TelefonosDto>>(_telefonoServices.GetAllTelefonos());
            return Json(dto);
        }

        // GET api/v1/Telefonos/{id}
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetTelefonoById(int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<TelefonosDto>(_telefonoServices.GetTelefonoById(id));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // GET api/v1/Telefonos/numero/{celular}
        [HttpGet]
        [Route("numero/{celular}")]
        public IHttpActionResult GetTelefonoByNumber(string celular)
        {
            if (string.IsNullOrWhiteSpace(celular))
                return BadRequest("El número de celular no puede estar vacío.");

            var dto = AutoMapperConfig.Mapper.Map<TelefonosDto>(_telefonoServices.GetTelefonoByNumber(celular));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // GET api/v1/Telefonos/perfil/{idPerfil}
        [HttpGet]
        [Route("perfil/{idPerfil:int}")]
        public IHttpActionResult GetByIdPerfil(int idPerfil)
        {
            if (idPerfil <= 0)
                return BadRequest("El ID de perfil debe ser mayor a 0.");

            var dto = AutoMapperConfig.Mapper.Map<TelefonosDto>(_telefonoServices.GetTelefonoByIdPerfil(idPerfil));

            if (dto == null)
                return NotFound();

            return Json(dto);
        }

        // POST api/v1/Telefonos
        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] TelefonosDto dto)
        {
            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.Telefonos>(dto);
                var result = _telefonoServices.CreateTelefono(model);
                if (!result)
                    return InternalServerError(new Exception("No se pudo crear el teléfono."));

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // PUT api/v1/Telefonos/{id}
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update([FromUri] int id, [FromBody] TelefonosDto dto)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            if (dto == null)
                return BadRequest("El cuerpo de la solicitud no puede ser nulo.");

            try
            {
                var model = AutoMapperConfig.Mapper.Map<Api_Services_Rest.Domain.Models.Telefonos>(dto);
                var result = _telefonoServices.UpdateTelefono(id, model);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        // DELETE api/v1/Telefonos/{id}
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete([FromUri] int id)
        {
            if (id <= 0)
                return BadRequest("El ID debe ser mayor a 0.");

            try
            {
                var result = _telefonoServices.DeleteTelefono(id);
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
