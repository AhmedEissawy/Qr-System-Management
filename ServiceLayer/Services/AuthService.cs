using DomainLayer.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Qr_System.DTOs;
using Qr_System.ViewModels;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        private IHostingEnvironment _host;

        public AuthService(UserManager<ApplicationUser> userManager, IHostingEnvironment host, IConfiguration configuration)
        {
            _userManager = userManager;
            _host = host;
            _configuration = configuration;
        }

        public async Task<ResponseModel> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
                return new ResponseModel { Message = "Email is already Exist" };

            if (await _userManager.FindByNameAsync(registerViewModel.UserName) != null)
                return new ResponseModel { Message = "User Name is already Exist" };

            var user = new ApplicationUser
            {
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PhoneNumber = registerViewModel.Phone,
                UserName = registerViewModel.UserName,
                Address = registerViewModel.Address,
                Type = registerViewModel.Type
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                return new ResponseModel
                {
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new ResponseModel
            {
                Message = "User Registered Successfully",
                IsAuthenticated = true
            };
        }

        public async Task<ResponseModel> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user == null)
                return new ResponseModel { Message = "there is no user with that user" };

            var result = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

            if (!result)
                return new ResponseModel { Message = "invalid password" };

            var userClaims = new[]
            {
                new Claim("Email",user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new ResponseModel
            {
                Message = "user login successfully",
                IsAuthenticated = true,
                Token = tokenAsString,
                ExpiresOn = token.ValidTo,
                UserName = user.UserName,
                UserId = user.Id
            };

        }

        public async Task<ResponseModel> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new ResponseModel { Message = "there is no user with that email" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Encoding.UTF8.GetBytes(token);

            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await SendEmailAsync(email, "Reset Your Password", "<h1>Follow The Instructions to Reset Your Password</h1>" +
                $"<p>To Reset Your Password <a href='{url}'>click here</a></p>");


            return new ResponseModel
            {
                IsAuthenticated = true,
                Token = validToken,
                Email = user.Email,
                Message = "Reset Password URL Sent to Your Email"
            };
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);

            if (user == null)
                return new ResponseModel { Message = "there is no user with that email" };

            if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmNewPassword)
                return new ResponseModel { Message = "New password did not Match ConfirmPassword" };

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);

            if (result.Succeeded)
            {
                return new ResponseModel
                {
                    Message = "password Reset successfully",
                    IsAuthenticated = true
                };
            }

            return new ResponseModel
            {
                Message = "Something went wrong",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };

        }

        public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_configuration["MailSettings:DisplayName"], _configuration["MailSettings:Email"]));

            message.To.Add(MailboxAddress.Parse(mailTo));

            message.Subject = subject;

            var builder = new BodyBuilder();

            if (attachments != null)
            {
                byte[] fileBytes;

                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        var ms = new MemoryStream();
                        file.CopyTo(ms);
                        fileBytes = ms.ToArray();
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }

            builder.HtmlBody = body;

            message.Body = builder.ToMessageBody();

            SmtpClient client = new SmtpClient();

            client.Connect(_configuration["MailSettings:Host"], int.Parse(_configuration["MailSettings:Port"]), true);

            client.Authenticate(_configuration["MailSettings:Email"], _configuration["MailSettings:Password"]);

            await client.SendAsync(message);

            client.Disconnect(true);

            client.Dispose();
        }

    }
}
