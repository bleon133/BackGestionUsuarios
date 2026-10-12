using Api_Services_Rest.Domain.Models;
using Api_Services_Rest.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Services.Roles
{
    public class RolesServices : IDisposable
    {
        public Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;
        public RolesServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public bool CreateRol(Api_Services_Rest.Domain.Models.roles roles)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                dbEntities.roles.Add(roles);
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

        public bool UpdateRol(int id, Api_Services_Rest.Domain.Models.roles role)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var rol = dbEntities.roles.Find(id);
                if (rol == null)
                {
                    return result;
                }
                if (rol.id != id)
                {
                    return result;
                }
                rol.valor = role.valor;
                rol.descripcion = role.descripcion;
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

        public DeleteRolResult DeleteRole(int id)
        {
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var rol = dbEntities.roles
                    .Include("usuariosRoles")
                    .Include("usuariosRoles.usuarios")
                    .FirstOrDefault(r => r.id == id);

                if (rol == null)
                    return DeleteRolResult.NotFound();

                // Bloquear si tiene usuarios asignados
                var usuarioRoles = rol.usuariosRoles.ToList();
                if (usuarioRoles.Count > 0)
                {
                    var usernames = usuarioRoles
                        .Where(ur => ur.usuarios != null)
                        .Select(ur => ur.usuarios.username)
                        .ToList();

                    return DeleteRolResult.Conflict(usernames);
                }

                dbEntities.roles.Remove(rol);
                dbEntities.SaveChanges();
                transaction.Commit();

                return DeleteRolResult.Ok();
            }
            catch (Exception)
            {
                transaction.Rollback();
                return DeleteRolResult.Error("No se pudo eliminar el rol.");
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public List<Api_Services_Rest.Domain.Models.roles> GetAllRoles()
        {
            try
            {
                return dbEntities.roles.ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.roles>();
            }
        }

        public Api_Services_Rest.Domain.Models.roles GetRolById(int id)
        {
            try
            {
                return dbEntities.roles.Find(id);
            }
            catch (Exception)
            {
                return new Api_Services_Rest.Domain.Models.roles();
            }
        }

        public Api_Services_Rest.Domain.Models.roles GetRolByValor(string valor)
        {
            try
            {
                return dbEntities.roles.FirstOrDefault(r => r.valor == valor);
            }
            catch (Exception)
            {
                return new Api_Services_Rest.Domain.Models.roles();
            }
        }
    }
}