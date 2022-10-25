using Microsoft.AspNetCore.Http;
using Qr_System.DTOs;
using Qr_System.ViewModels;
using ServiceLayer.DTOs;
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

        Task SwitchAccountAsync(SwitchViewModel switchViewModel);

        Task<OwnersCount> GetOwnersCountAsync();

        Task<List<OwnerDto>> GetAllOwnersAsync();

        Task<List<ResponseModel>> GetAllUsersAsync();

        Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null);

    }
}