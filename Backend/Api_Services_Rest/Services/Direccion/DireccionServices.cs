using System;
using System.Collections.Generic;
using System.Linq;

namespace Api_Services_Rest.Services.Direccion
{
    public class DireccionServices : IDisposable
    {
        private Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;

        public DireccionServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public List<Api_Services_Rest.Domain.Models.direcciones> GetDirecciones()
        {
            try
            {
                return dbEntities.direcciones
                    .Include("perfilUsuario")
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.direcciones>();
            }
        }

        public Api_Services_Rest.Domain.Models.direcciones GetDireccionById(int id)
        {
            try
            {
                return dbEntities.direcciones
                    .Include("perfilUsuario")
                    .FirstOrDefault(p => p.id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreateDireccion(Api_Services_Rest.Domain.Models.direcciones direccion)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                dbEntities.direcciones.Add(direccion);
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

        public bool UpdateDireccion(int id, Api_Services_Rest.Domain.Models.direcciones direccion)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var dir = dbEntities.direcciones.Find(id);
                if (dir == null)
                    return result;

                dir.calle = direccion.calle;
                dir.colonia = direccion.colonia;
                dir.NumInterior = direccion.NumInterior;
                dir.NumExterior = direccion.NumExterior;
                dir.Municipio = direccion.Municipio;
                dir.idPerfilUsuario = direccion.idPerfilUsuario;

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

        public bool DeleteDireccion(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var dir = dbEntities.direcciones.Find(id);
                if (dir == null)
                    return result;

                dbEntities.direcciones.Remove(dir);
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