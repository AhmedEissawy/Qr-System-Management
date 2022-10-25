using DomainLayer.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Qr_System.DTOs;
using Qr_System.ViewModels;
using RepositoryLayer;
using ServiceLayer.DTOs;
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

        private readonly ApplicationDbContext _context;

        private IHostingEnvironment _host;

        public AuthService(UserManager<ApplicationUser> userManager, IHostingEnvironment host, IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _host = host;
            _configuration = configuration;
            _context = context;
        }

        public async Task<ResponseModel> RegisterAsync(RegisterViewModel registerViewModel)
            {
            if (await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
                return new ResponseModel { message = "Email is already Exist" };

            if (await _userManager.FindByNameAsync(registerViewModel.UserName) != null)
                return new ResponseModel { message = "User Name is already Exist" };

            if (registerViewModel.Type == "user")
            {
                var applicationUser = new ApplicationUser
                {
                    Email = registerViewModel.Email,
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    PhoneNumber = registerViewModel.Phone,
                    UserName = registerViewModel.UserName,
                    Address = registerViewModel.Address,
                    Image = UploadPhoto(registerViewModel),
                    Type = registerViewModel.Type,
                };

                var appResult = await _userManager.CreateAsync(applicationUser, registerViewModel.Password);

                if (!appResult.Succeeded)
                {
                    return new ResponseModel
                    {
                        errors = appResult.Errors.Select(e => e.Description).ToList()
                    };
                }

                return new ResponseModel
                {
                    message = "User Registered Successfully",
                    isAuthenticated = true
                };
            }
            var user = new ApplicationUser
            {
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                PhoneNumber = registerViewModel.Phone,
                UserName = registerViewModel.UserName,
                Address = registerViewModel.Address,
                Image = UploadPhoto(registerViewModel),
                Type = registerViewModel.Type,
            };

            user.Owner = new Owner
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.PhoneNumber,
                UserName = user.UserName,
                Address = user.Address,
                ApplicationUserId = user.Id,
                Password = user.PasswordHash,
                Type = user.Type,  
                Image = UploadPhoto(registerViewModel),
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            var unit = await _context.Units.FindAsync(registerViewModel.UnitId);

            unit.OwnerId = user.Owner.Id;

            await _context.SaveChangesAsync();

            if (!result.Succeeded)
            {
                return new ResponseModel
                {
                    errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new ResponseModel
            {
                message = "Owner Registered Successfully",
                isAuthenticated = true
            };

        }

        public async Task<ResponseModel> LoginAsync(LoginViewModel loginViewModel)
        {

            var user = await _context.Users.Include(u=>u.Owner).Where(u => u.Email == loginViewModel.Email).FirstOrDefaultAsync();

            if (user == null)
                return new ResponseModel { message = "Bad Credentials" };

            if (user.Type == "owner" && !user.Owner.Switch)
            {
                return new ResponseModel { message = "Wait until check your data" };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);

            if (!result)
                return new ResponseModel { message = "invalid Credentials" };

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
                message = "user login successfully",
                isAuthenticated = true,
                token = tokenAsString,
                expiresOn = token.ValidTo,
                userName = user.UserName,
                id = user.Id,
                type = user.Type
            };

        }

        public async Task<ResponseModel> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new ResponseModel { message = "there is no user with that email" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Encoding.UTF8.GetBytes(token);

            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/ResetPassword?email={email}&token={validToken}";

            await SendEmailAsync(email, "Reset Your Password", "<h1>Follow The Instructions to Reset Your Password</h1>" +
                $"<p>To Reset Your Password <a href='{url}'>click here</a></p>");


            return new ResponseModel
            {
                isAuthenticated = true,
                token = validToken,
                email = user.Email,
                message = "Reset Password URL Sent to Your Email"
            };
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);

            if (user == null)
                return new ResponseModel { message = "there is no user with that email" };

            if (resetPasswordViewModel.NewPassword != resetPasswordViewModel.ConfirmNewPassword)
                return new ResponseModel { message = "New password did not Match ConfirmPassword" };

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.NewPassword);

            if (result.Succeeded)
            {
                return new ResponseModel
                {
                    message = "password Reset successfully",
                    isAuthenticated = true
                };
            }

            return new ResponseModel
            {
                message = "Something went wrong",
                errors = result.Errors.Select(e => e.Description).ToList()
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

            client.Connect(_configuration["MailSettings:Host"],Convert.ToInt32(_configuration["MailSettings:Port"]), true);

            client.Authenticate(_configuration["MailSettings:Email"], _configuration["MailSettings:Password"]);

            await client.SendAsync(message);

            client.Disconnect(true);

            client.Dispose();
        }

        public string UploadPhoto(RegisterViewModel model)
        {
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Users");
                string uniqueFileName = Guid.NewGuid() + ".jpg";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                return uniqueFileName;

            }

            return "user do not upload photo";
        }

        public async Task SwitchAccountAsync(SwitchViewModel switchViewModel)
        {
            var user = await _context.Users.Include(u=>u.Owner).Where(u => u.Id == switchViewModel.Id).FirstOrDefaultAsync();

            user.Owner.Switch = switchViewModel.Switch;

            await _context.SaveChangesAsync();

            await SendEmailAsync(user.Owner.Email,"Confirm Your Login","<h1>Welcome,Now You Can Login with that Email</h1>");
        }

        public async Task<OwnersCount> GetOwnersCountAsync()
        {
            var pendingOwners = await _context.Owners.Where(o => !o.Switch).CountAsync();

            var approvedOwners = await _context.Owners.Where(o => o.Switch).CountAsync();

            return new OwnersCount
            {
                PendingCount = pendingOwners,
                ApprovedCount = approvedOwners
            };
        }

        public async Task<List<OwnerDto>> GetAllOwnersAsync()
        {
            var owners = await _context.Owners.Include(o => o.Unit).Select(o=>new OwnerDto { 
                 id = o.Id,
                 ownerName = o.UserName,
                 ownerUnit = o.Unit.Name,
                 ownerPhone = o.Phone,
                 Image = o.Image

            }).ToListAsync();

            return owners;
        }

        public async Task<List<ResponseModel>> GetAllUsersAsync()
        {
            var users = await _context.Users.Where(u=>u.Type == "user").Select(u=> new ResponseModel { 
                
                 id = u.Id,
                 email = u.Email,
                 userName = u.UserName,
                 firstName = u.FirstName,
                 lastName = u.LastName,
                 address = u.Address,
                 phone = u.PhoneNumber

            }).ToListAsync();

            return users;
        }
    }
}