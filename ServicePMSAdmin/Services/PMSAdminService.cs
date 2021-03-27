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

    }
}
