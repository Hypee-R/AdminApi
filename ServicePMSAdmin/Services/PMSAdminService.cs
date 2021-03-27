using AccessControl.Services;
using CorePMSAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ServicePMSAdmin.Services
{
    public class PMSAdminService : IPMSAdminService
    {
        private readonly BDContext _ctx;
        private readonly IHttpContextAccessor _accessor;
        private readonly ITokenManager _manager;
        private readonly IJwtHandler _jwt;

        public PMSAdminService(
            BDContext ctx,
            IHttpContextAccessor accessor,
            ITokenManager manager,
            IJwtHandler jwt
            )
        {
            _ctx = ctx;
            _accessor = accessor;
            _manager = manager;
            _jwt = jwt;
        }

        private string _getToken()
        {
            string header = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
            return string.IsNullOrEmpty(header) ? null : header.Split(" ").Last();
        }

        #region Auth
        public async Task<object> auth(Usuarios usuario)
        {
            Usuarios _usuario = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Correo == usuario.Correo && u.Contrasena == usuario.Contrasena);

            if (_usuario != null)
            {
                if (_usuario.Estatus)
                {
                    var token = _jwt.Create(usuario.Correo);
                    _usuario.Token = token.AccessToken;

                    if (await _ctx.SaveChangesAsync() > 0)
                    {
                        _manager.Activate(_usuario.Token);

                        return new
                        {
                            _usuario.Correo,
                            _usuario.Usuario,
                            _usuario.Contrasena,
                            _usuario.Sexo,
                            _usuario.FechaCreacion,
                            _usuario.Token
                        };
                    }
                }
                return null;
            }
            return null;
        }

        public async Task<object> getUser()
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                Usuarios _usuario = await _ctx.Usuarios
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(x => x.Token == token);


                return new
                {
                    _usuario.Id,
                    _usuario.Correo,
                    _usuario.Usuario,
                    _usuario.Estatus,
                    _usuario.FechaCreacion,
                    _usuario.Sexo
                };
            }
            else
            {
                _manager.Deactivate();
                return null;
            }
        }

        public async Task logOff()
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                var _usuario = await _ctx.Usuarios.FirstOrDefaultAsync(u => u.Token == token);
                if (_usuario != null)
                {
                    _usuario.Token = null;
                    if (await _ctx.SaveChangesAsync() > 0)
                        _manager.Deactivate();
                }
            }
        }
        #endregion

        #region Users
        public async Task<object> getUsers()
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                var listaUsuarios = await _ctx.Usuarios
                                        .Where(x => x.Token != token && x.Estatus == true)
                                        .Select(z => new
                                        {
                                            z.Id,
                                            z.Usuario,
                                            z.Correo,
                                            z.Sexo,
                                            z.Estatus,
                                            z.FechaCreacion
                                        }).OrderBy(x => x.Usuario)
                                        .AsNoTracking().ToListAsync();

                return listaUsuarios;
            }
            else
            {
                _manager.Deactivate();
                return null;
            }
        }

        public async Task<object> postUser(Usuarios usuario)
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                Usuarios existeUsuario = await _ctx.Usuarios
                                             .FirstOrDefaultAsync(x => x.Correo == usuario.Correo);
                if (existeUsuario == null)
                {
                    usuario.Estatus = true;

                    _ctx.Entry(usuario).State = EntityState.Added;
                    if (_ctx.SaveChanges() > 0)
                    {
                        return new
                        {
                            usuario.Id,
                            usuario.Usuario,
                            usuario.Correo,
                            usuario.Sexo,
                            usuario.Estatus,
                            fechaCreacion = _ctx.Usuarios.FirstOrDefault(x => x.Correo == usuario.Correo).FechaCreacion
                        };
                    }
                    else
                    {
                        _manager.Deactivate();
                        return false;
                    }
                }
                else
                    return false;
            }
            else
            {
                _manager.Deactivate();
                return false;
            }
        }

        public async Task<object> putUser(Usuarios usuario)
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                Usuarios existeUsuario = await _ctx.Usuarios.FirstOrDefaultAsync(x => x.Id == usuario.Id);

                if (existeUsuario != null)
                {
                   
                        existeUsuario.Usuario = usuario.Usuario;
                        existeUsuario.Correo = usuario.Correo;
                        existeUsuario.Sexo = usuario.Sexo;
                        existeUsuario.Contrasena = usuario.Contrasena;

                        _ctx.Entry(existeUsuario).State = EntityState.Modified;

                        if (_ctx.SaveChanges() > 0)
                        {
                            return new
                            {
                                existeUsuario.Id,
                                existeUsuario.Usuario,
                                existeUsuario.Correo,
                                existeUsuario.Sexo,
                                existeUsuario.Estatus,
                                existeUsuario.FechaCreacion
                            };
                        }
                        else
                            return null;
                   
                }
                else
                    return null;
            }
            else
            {
                _manager.Deactivate();
                return null;
            }
        }

        public async Task<bool> deleteUser(int idUsuario)
        {
            string token = _getToken();
            if (!string.IsNullOrEmpty(token))
            {
                Usuarios existeUsuario = await _ctx.Usuarios
                                                  .FirstOrDefaultAsync(x => x.Id == idUsuario);
                if (existeUsuario != null)
                {

                    existeUsuario.Estatus = false;

                    _ctx.SaveChanges();
                    _ctx.Entry(existeUsuario).State = EntityState.Modified;
                    if (_ctx.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                    return false;
            }
            else
            {
                _manager.Deactivate();
                return false;
            }
        }
        #endregion

    }
}
