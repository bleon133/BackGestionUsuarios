using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Services.Usuario_Roles
{
    public class UsuarioRolesServices : IDisposable
    {
        private Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;

        public UsuarioRolesServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public List<Api_Services_Rest.Domain.Models.usuariosRoles> GetAll()
        {
            try
            {
                return dbEntities.usuariosRoles
                    .Include("roles")
                    .Include("usuarios")
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.usuariosRoles>();
            }
        }

        public Api_Services_Rest.Domain.Models.usuariosRoles GetById(int id)
        {
            try
            {
                return dbEntities.usuariosRoles
                    .Include("roles")
                    .Include("usuarios")
                    .FirstOrDefault(u => u.id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<Api_Services_Rest.Domain.Models.usuariosRoles> GetByIdUsuario(int idUsuario)
        {
            try
            {
                return dbEntities.usuariosRoles
                    .Include("roles")
                    .Where(u => u.idUsuario == idUsuario)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.usuariosRoles>();
            }
        }

        public bool Create(Api_Services_Rest.Domain.Models.usuariosRoles usuarioRol)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                // Evitar duplicados
                bool existe = dbEntities.usuariosRoles
                    .Any(u => u.idUsuario == usuarioRol.idUsuario && u.idRol == usuarioRol.idRol);
                if (existe)
                    return result;

                dbEntities.usuariosRoles.Add(usuarioRol);
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

        public bool Delete(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var ur = dbEntities.usuariosRoles.Find(id);
                if (ur == null)
                    return result;

                dbEntities.usuariosRoles.Remove(ur);
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