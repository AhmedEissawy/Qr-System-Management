using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qr_System.DTOs;
using Qr_System.ViewModels;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qr_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromForm]RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = await _authService.RegisterAsync(registerViewModel);

                if (data.IsAuthenticated)
                {
                    return Ok(new { message = data.Message });
                }

                return StatusCode(StatusCodes.Status500InternalServerError,data);
            }

            return BadRequest(ModelState);
            
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = await _authService.LoginAsync(loginViewModel);

                if (data.IsAuthenticated)
                {
                    return Ok(new { id = data.UserId,token = data.Token,name = data.UserName });
                }

                return StatusCode(StatusCodes.Status500InternalServerError,data);
            }

            return BadRequest(ModelState);

        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var data = await _authService.ForgetPasswordAsync(email);

                if (data.IsAuthenticated)
                {
                    return Ok(new { message = data.Message,token = data.Token,email = data.Email });
                }

                return StatusCode(StatusCodes.Status500InternalServerError,data);
            }

            return BadRequest(ModelState);

        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromForm] ResetPasswordViewModel resetPasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = await _authService.ResetPasswordAsync(resetPasswordViewModel);

                if (data.IsAuthenticated)
                {
                    return Ok(data);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, data);
            }

            return BadRequest(ModelState);

        }
    }
}
