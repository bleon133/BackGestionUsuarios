using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Services.Perfiles
{
    public class PerfilUsuarioServices : IDisposable
    {
        private Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;

        public PerfilUsuarioServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public List<Api_Services_Rest.Domain.Models.perfilUsuario> GetAllPerfiles()
        {
            try
            {
                return dbEntities.perfilUsuario
                    .Include("direcciones")
                    .Include("Telefonos")
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.perfilUsuario>();
            }
        }

        public Api_Services_Rest.Domain.Models.perfilUsuario GetPerfilById(int id)
        {
            try
            {
                return dbEntities.perfilUsuario
                    .Include("direcciones")
                    .Include("Telefonos")
                    .FirstOrDefault(p => p.id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Api_Services_Rest.Domain.Models.perfilUsuario GetPerfilByIdUsuario(int idUsuario)
        {
            try
            {
                return dbEntities.perfilUsuario
                    .Include("direcciones")
                    .Include("Telefonos")
                    .FirstOrDefault(p => p.idUsuario == idUsuario);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreatePerfil(Api_Services_Rest.Domain.Models.perfilUsuario perfil)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                dbEntities.perfilUsuario.Add(perfil);
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

        public bool UpdatePerfil(int id, Api_Services_Rest.Domain.Models.perfilUsuario perfil)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var perfilDb = dbEntities.perfilUsuario.Find(id);
                if (perfilDb == null)
                    return result;

                perfilDb.nombre = perfil.nombre;
                perfilDb.apellidoPaterno = perfil.apellidoPaterno;
                perfilDb.apellidoMaterno = perfil.apellidoMaterno;
                perfilDb.fechaNacimiento = perfil.fechaNacimiento;
                perfilDb.rfc = perfil.rfc;
                perfilDb.idUsuario = perfil.idUsuario;

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

        public bool DeletePerfil(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var perfil = dbEntities.perfilUsuario
                    .Include("direcciones")
                    .Include("Telefonos")
                    .FirstOrDefault(p => p.id == id);

                if (perfil == null)
                    return result;

                foreach (var dir in perfil.direcciones.ToList())
                    dbEntities.direcciones.Remove(dir);

                foreach (var tel in perfil.Telefonos.ToList())
                    dbEntities.Telefonos.Remove(tel);

                dbEntities.perfilUsuario.Remove(perfil);
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