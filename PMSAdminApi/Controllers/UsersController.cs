using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorePMSAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServicePMSAdmin.Services;

namespace PMSAdminApi.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly IPMSAdminService _service;
        public UsersController(IPMSAdminService service) => _service = service;

        #region GET
        [HttpGet("01")]
        public async Task<IActionResult> getAllUsers()
        {
            return Json(await _service.getUsers());
        }
        #endregion

        #region POST
        [HttpPost("02")]
        public async Task<IActionResult> postUser([FromBody] JObject data)
        {
            Usuarios usuario = new Usuarios();
            var extraerUsuario = data["usuario"].ToObject<JObject>();

            usuario.Correo = extraerUsuario["correo"].ToObject<string>();
            usuario.Usuario = extraerUsuario["usuario"].ToObject<string>();
            usuario.Contrasena = extraerUsuario["contrasena"].ToObject<string>();
            usuario.Sexo = extraerUsuario["sexo"].ToObject<string>();

            return Json(await _service.postUser(usuario));
        }
        #endregion

        #region PUT
        [HttpPut("03")]
        public async Task<IActionResult> putUser([FromBody] JObject data)
        {
            Usuarios usuario = new Usuarios();
            var extraerUsuario = data["usuario"].ToObject<JObject>();

            usuario.Correo = extraerUsuario["correo"].ToObject<string>();
            usuario.Usuario = extraerUsuario["usuario"].ToObject<string>();
            usuario.Contrasena = extraerUsuario["contrasena"].ToObject<string>();
            usuario.Sexo = extraerUsuario["sexo"].ToObject<string>();
            usuario.Id = extraerUsuario["id"].ToObject<int>();

            return Json(await _service.putUser(usuario));
        }
        #endregion

        #region DELETE
        [HttpDelete("04")]
        public async Task<IActionResult> deleteUser(int idUsuario)
        {
            return Json(await _service.deleteUser(idUsuario));
        }
        #endregion
    }
}