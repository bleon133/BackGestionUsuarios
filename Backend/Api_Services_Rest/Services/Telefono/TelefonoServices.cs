using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Services.Telefono
{
    public class TelefonoServices : IDisposable
    {
        private Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;

        public TelefonoServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public List<Api_Services_Rest.Domain.Models.Telefonos> GetAllTelefonos()
        {
            try
            {
                return dbEntities.Telefonos
                    .Include("perfilUsuario")
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.Telefonos>();
            }
        }

        public Api_Services_Rest.Domain.Models.Telefonos GetTelefonoById(int id)
        {
            try
            {
                return dbEntities.Telefonos.FirstOrDefault(p => p.id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Api_Services_Rest.Domain.Models.Telefonos GetTelefonoByNumber(string number)
        {
            try
            {
                return dbEntities.Telefonos.FirstOrDefault(p => p.celular.Equals(number));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Api_Services_Rest.Domain.Models.Telefonos GetTelefonoByIdPerfil(int idPerfilUsuario)
        {
            try
            {
                return dbEntities.Telefonos
                    .Include("perfilUsuario")
                    .FirstOrDefault(p => p.idPerfilUsuario == idPerfilUsuario);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreateTelefono(Api_Services_Rest.Domain.Models.Telefonos telefono)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                dbEntities.Telefonos.Add(telefono);
                dbEntities.SaveChanges();
                transaction.Commit();
                result = true;
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return result;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public bool UpdateTelefono(int id, Api_Services_Rest.Domain.Models.Telefonos telefono)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var tel = dbEntities.Telefonos.Find(id);
                if (tel == null)
                    return result;

                tel.celular = telefono.celular;
                tel.casa = telefono.casa;
                tel.oficina = telefono.oficina;
                tel.idPerfilUsuario = telefono.idPerfilUsuario;

                dbEntities.SaveChanges();
                transaction.Commit();
                result = true;
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return result;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public bool DeleteTelefono(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var tel = dbEntities.Telefonos.Find(id);
                if (tel == null)
                    return result;

                dbEntities.Telefonos.Remove(tel);
                dbEntities.SaveChanges();
                transaction.Commit();
                result = true;
                return result;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return result;
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}