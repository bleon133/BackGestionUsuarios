using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api_Services_Rest.Services.Usuario
{
    public class UsuariosServices : IDisposable
    {
        private Api_Services_Rest.Domain.Models.productosDbEntities dbEntities = null;

        public UsuariosServices()
        {
            dbEntities = new Api_Services_Rest.Domain.Models.productosDbEntities();
        }

        public void Dispose()
        {
            dbEntities.Dispose();
        }

        public List<Api_Services_Rest.Domain.Models.usuarios> GetAllUsuarios()
        {
            try
            {
                return dbEntities.usuarios
                    .Include("usuariosRoles")
                    .Include("usuariosRoles.roles")
                    .Include("perfilUsuario")
                    .Include("perfilUsuario.direcciones")
                    .Include("perfilUsuario.Telefonos")
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Api_Services_Rest.Domain.Models.usuarios>();
            }
        }

        public Api_Services_Rest.Domain.Models.usuarios GetUsuarioById(int id)
        {
            try
            {
                return dbEntities.usuarios
                    .Include("usuariosRoles")
                    .Include("usuariosRoles.roles")
                    .Include("perfilUsuario")
                    .Include("perfilUsuario.direcciones")
                    .Include("perfilUsuario.Telefonos")
                    .FirstOrDefault(u => u.id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Api_Services_Rest.Domain.Models.usuarios GetUsuarioByUsername(string username)
        {
            try
            {
                return dbEntities.usuarios
                    .Include("usuariosRoles")
                    .Include("perfilUsuario")
                    .FirstOrDefault(u => u.username == username);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool CreateUsuario(Api_Services_Rest.Domain.Models.usuarios usuario)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                // Verificar que el username no exista
                bool exists = dbEntities.usuarios.Any(u => u.username == usuario.username);
                if (exists)
                    return result;

                dbEntities.usuarios.Add(usuario);
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

        public bool UpdateUsuario(int id, Api_Services_Rest.Domain.Models.usuarios usuario)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var usuarioDb = dbEntities.usuarios.Find(id);
                if (usuarioDb == null)
                    return result;

                usuarioDb.username = usuario.username;
                usuarioDb.password = usuario.password;
                usuarioDb.suspendido = usuario.suspendido;

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

        // ─── ELIMINAR ─────────────────────────────────────────────────────────

        public bool DeleteUsuario(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var usuario = dbEntities.usuarios
                    .Include("usuariosRoles")
                    .Include("perfilUsuario")
                    .Include("perfilUsuario.direcciones")
                    .Include("perfilUsuario.Telefonos")
                    .FirstOrDefault(u => u.id == id);

                if (usuario == null)
                    return result;

                // 1. Eliminar roles del usuario
                var roles = usuario.usuariosRoles.ToList();
                foreach (var rol in roles)
                    dbEntities.usuariosRoles.Remove(rol);

                // 2. Eliminar direcciones y teléfonos de cada perfil
                var perfiles = usuario.perfilUsuario.ToList();
                foreach (var perfil in perfiles)
                {
                    var direcciones = perfil.direcciones.ToList();
                    foreach (var dir in direcciones)
                        dbEntities.direcciones.Remove(dir);

                    var telefonos = perfil.Telefonos.ToList();
                    foreach (var tel in telefonos)
                        dbEntities.Telefonos.Remove(tel);

                    dbEntities.perfilUsuario.Remove(perfil);
                }

                // 3. Eliminar el usuario
                dbEntities.usuarios.Remove(usuario);

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

        public bool ToggleSuspendido(int id)
        {
            bool result = false;
            var transaction = dbEntities.Database.BeginTransaction();
            try
            {
                var usuario = dbEntities.usuarios.Find(id);
                if (usuario == null)
                    return result;

                usuario.suspendido = !usuario.suspendido;

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