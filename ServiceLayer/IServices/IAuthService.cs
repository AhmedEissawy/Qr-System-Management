using Microsoft.AspNetCore.Http;
using Qr_System.DTOs;
using Qr_System.ViewModels;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IServices
{
    public interface IAuthService
    {
        Task<ResponseModel> RegisterAsync(RegisterViewModel registerViewModel);

        Task<ResponseModel> LoginAsync(LoginViewModel loginViewModel);

        Task<ResponseModel> ForgetPasswordAsync(string email);

        Task<ResponseModel> ResetPasswordAsync(ResetPasswordViewModel resetPasswordViewModel);

        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);

    }

}

