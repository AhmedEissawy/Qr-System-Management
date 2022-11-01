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
                try
                {
                    var data = await _authService.RegisterAsync(registerViewModel);

                    if (data != null)
                    {
                        return Ok(new { message = data.message });
                    }

                    return BadRequest("Something Went wrong");
                }
                catch (Exception ex)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
                
            }

            return BadRequest(ModelState);
            
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await _authService.LoginAsync(loginViewModel);

                    if (data.isAuthenticated)
                    {
                        return Ok(
                            new { 
                            id = data.id,
                            token = data.token,
                            name = data.userName,
                            type = data.type,
                            phone = data.phone,
                            email = data.email
                        });
                    }

                    return BadRequest(new { message = data.message });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,ex);
                }
          
            }

            return BadRequest(ModelState);

        }

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordViewModel forgetPasswordViewModel)
        {
            if (!string.IsNullOrEmpty(forgetPasswordViewModel.Email))
            {
                var data = await _authService.ForgetPasswordAsync(forgetPasswordViewModel);

                if (data.isAuthenticated)
                {
                    return Ok(new { message = data.message,token = data.token,email = data.email });
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

                if (data.isAuthenticated)
                {
                    return Ok(data);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, data);
            }

            return BadRequest(ModelState);

        }

        [HttpPost("SwitchOwner")]
        public async Task<ActionResult> SwitchAccount([FromBody]SwitchViewModel switchViewModel)
        {
            try
            {
                await _authService.SwitchAccountAsync(switchViewModel);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        } 

        [HttpGet("GetOwnersCount")]
        public async Task<ActionResult> GetOwnersCount()
        {
            var data = await _authService.GetOwnersCountAsync();

            return Ok(data);
        }

        [HttpGet("GetAllOwners")]
        public async Task<ActionResult> GetAllOwners()
        {
            try
            {
                var data = await _authService.GetAllOwnersAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var data = await _authService.GetAllUsersAsync();

                return Ok(data.Select(u => new
                {
                    id = u.id,
                    firstName = u.firstName,
                    lastName = u.lastName,
                    email = u.email,
                    phone = u.phone,
                    address = u.address,
                    userName = u.userName
                }));
                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<ActionResult> GetUserById(string id)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("id is null");
                }
                var data = await _authService.GetUserByIdAsync(id);

                return Ok(data);
         
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult> UpdateUser(string id ,UpdateUserViewModel updateUserViewModel)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("id is null");
                }
                var data = await _authService.UpdateUserAsync(id,updateUserViewModel);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            await _authService.DeleteUserAsync(id);
            return Ok();
        }

        [HttpGet("RejectOwner/{id}")]
        public async Task<ActionResult> RejectOwner(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                 await _authService.RejectOwnerAsync(id);

                return Ok();

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}