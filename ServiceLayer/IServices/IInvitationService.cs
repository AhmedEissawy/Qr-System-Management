using ServiceLayer.DTOs;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IServices
{
    public interface IInvitationService
    {
        Task<InvitationDto> CreateInvitationAsync(InvitationViewModel invitationViewModel);

        Task<IEnumerable<InvitationDto>> GetAllInvitationAsync();

        Task<IEnumerable<InvitationDto>> GetAllInvitationDailyAsync();

        Task<IEnumerable<InvitationDto>> SearchInvitationAsync(SearchInvitationModel searchInvitationModel);

        Task<InvitationDto> GetInvitationByIdAsync(int id);

        Task DeleteInvitationAsync(int id);

        Task ApproveInvitationAsync(ApproveInvitationViewModel approveInvitationViewModel);
    }
}