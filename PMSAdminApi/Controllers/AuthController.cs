using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorePMSAdmin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicePMSAdmin.Services;

namespace PMSAdminApi.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IPMSAdminService _service;
        public AuthController(IPMSAdminService service) => _service = service;

        #region GET
        [HttpGet("01")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> getUser()
        {
            return Json(await _service.getUser());
        }

        [HttpGet("02")]
        public async Task<IActionResult> logOff()
        {
            await _service.logOff();
            return Ok();
        }
        #endregion

        #region POST
        [HttpPost("01")]
        public async Task<IActionResult> auth([FromBody] Usuarios usuario)
        {
            return Json(await _service.auth(usuario));
        }
        #endregion
    }
}