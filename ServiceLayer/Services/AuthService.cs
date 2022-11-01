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
using System.ComponentModel.DataAnnotations;
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

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        private IHostingEnvironment _host;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHostingEnvironment host,
            IConfiguration configuration,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _host = host;
            _configuration = configuration;
            _context = context;
            _signInManager = signInManager;
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

            else if (registerViewModel.Type == "owner")
            {
                if (registerViewModel.UnitId == null)
                {
                    return new ResponseModel
                    {
                        message = "owner should select unit"
                    };
                }

                var unit = await _context.Units.Include(u => u.Owner).Where(u => u.Id == registerViewModel.UnitId).FirstOrDefaultAsync();

                bool ownedUnit = unit.OwnerId.HasValue;

                if (ownedUnit)
                {
                    return new ResponseModel
                    {
                        message = "Unit Already Owned"
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
                    Type = user.Type,
                    View = true,
                    Image = UploadPhoto(registerViewModel),
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

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

                return new ResponseModel
                {
                    message = "SomeThing went Wrong Check Your Data"
                };
           
        }

        public async Task<ResponseModel> LoginAsync(LoginViewModel loginViewModel)
        {

            var user = await _context.Users.Include(u => u.Owner)
                     .Where(u => u.Email == loginViewModel.Email || u.UserName == loginViewModel.Email).FirstOrDefaultAsync();

            if (user == null)
                return new ResponseModel { message = "Bad Credentials" };

            if (user.Type == "owner" && !user.Owner.Switch)
            {
                return new ResponseModel { message = "Wait until check your data" };
            }

            var userName = new EmailAddressAttribute().IsValid(loginViewModel.Email) ?
                _userManager.FindByEmailAsync(loginViewModel.Email).Result.UserName : loginViewModel.Email;

            var result = await _signInManager.PasswordSignInAsync(userName, loginViewModel.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
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
                type = user.Type,
                phone = user.PhoneNumber,
                email = user.Email
            };

        }

        public async Task<ResponseModel> ForgetPasswordAsync(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordViewModel.Email);

            if (user == null)
                return new ResponseModel { message = "there is no user with that email" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url = $"localhost:4200/dashboard/resetPassword";

            await SendEmailAsync(forgetPasswordViewModel.Email, "Reset Your Password", "<h1>Follow The Instructions to Reset Your Password</h1>" +
                $"<p>To Reset Your Password <a href='{url}'>click here</a></p>");


            return new ResponseModel
            {
                isAuthenticated = true,
                token = token,
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
                const decimal maxFileInKb = 200;

                if (model.Photo.Length/1024 > maxFileInKb)
                {
                   throw new Exception( "file size is more than 200 kb");
                }

                string uploadFolder = Path.Combine(_host.WebRootPath, "Images/Users");
                string uniqueFileName = Guid.NewGuid() + ".jpg";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

                return uniqueFileName;

            }

            return null;
        }

        public async Task SwitchAccountAsync(SwitchViewModel switchViewModel)
        {
            var owner = await _context.Owners.Where(u => u.Id == switchViewModel.Id).FirstOrDefaultAsync();

            owner.Switch = switchViewModel.Switch;

            await _context.SaveChangesAsync();

            await SendEmailAsync(owner.Email,"Confirm Your Login","<h1>Welcome,Now You Can Login with that Email</h1>");
        }

        public async Task<OwnersCount> GetOwnersCountAsync()
        {
            var pendingOwners = await _context.Owners.Where(o => !o.Switch).CountAsync();

            var approvedOwners = await _context.Owners.Where(o => o.Switch).CountAsync();

            var rejectOwners = await _context.Owners.Where(o => !o.View).CountAsync();

            return new OwnersCount
            {
                PendingCount = pendingOwners,
                ApprovedCount = approvedOwners,
                RejectedOwners = rejectOwners
            };
        }

        public async Task<List<OwnerDto>> GetAllOwnersAsync()
        {
            var owners = await _context.Owners.Include(ou => ou.Unit).Where(o => o.View).Select(o=>new OwnerDto { 
                 id = o.Id,
                 ownerName = o.UserName,
                 ownerUnit = o.Unit.Name,
                 ownerPhone = o.Phone,
                 ownerEmail = o.Email,
                 Image = o.Image == null ? null : $"Images/Users/{o.Image}",
                 Switch = o.Switch
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

        public async Task<ResponseModel> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return new ResponseModel
                {
                    message = "there is no user with that id"
                };
            }

            return new ResponseModel
            {
                id = user.Id,
                firstName = user.FirstName,
                lastName = user.LastName,
                userName = user.UserName,
                email = user.Email,
                address = user.Address,
                phone = user.PhoneNumber,
                image = user.Image != null ? $"Images/Users/{user.Image}" : null
            };
        }

        public async Task RejectOwnerAsync(int id)
        {
            var owner = await _context.Owners.FindAsync(id);

            owner.View = false;

            await _context.SaveChangesAsync();
        }

        public async Task<ResponseModel> UpdateUserAsync(string id, UpdateUserViewModel updateUserViewModel)
        {
            var existUser = await _userManager.FindByIdAsync(id);

            existUser.FirstName = updateUserViewModel.FirstName;

            existUser.LastName = updateUserViewModel.LastName;

            existUser.PhoneNumber = updateUserViewModel.Phone;

            existUser.Address = updateUserViewModel.Address;

            existUser.PasswordHash =  _userManager.PasswordHasher.HashPassword(existUser,updateUserViewModel.Password);

            await _userManager.UpdateAsync(existUser);

            return new ResponseModel
            {
                 id = existUser.Id,
                 firstName = existUser.FirstName,
                 lastName = existUser.LastName,
                 phone = existUser.PhoneNumber,
                 address = existUser.Address,
                 email = existUser.Email,
                 userName = existUser.UserName
            };
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            await _userManager.DeleteAsync(user);
        }
    }
}