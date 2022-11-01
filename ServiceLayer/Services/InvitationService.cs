using DomainLayer.Enums;
using DomainLayer.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using ServiceLayer.DTOs;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;

        private readonly IHubContext<SignalServer> _signalHub;

        public InvitationService(ApplicationDbContext context, IHubContext<SignalServer> signalHub)
        {
            _context = context;
            _signalHub = signalHub;
        }

        public async Task ApproveInvitationAsync(ApproveInvitationViewModel approveInvitationViewModel)
        {
            var invitation = await _context.Invitaions.FindAsync(approveInvitationViewModel.Id);

            invitation.Approve = approveInvitationViewModel.Approve;

            await _context.SaveChangesAsync();

            await _signalHub.Clients.All.SendAsync("LoadInvitations");

        }

        public async Task<InvitationDto> CreateInvitationAsync(InvitationViewModel invitationViewModel)
        {
            var unit = await _context.Units.Include(u => u.Owner).Where(u=>u.Id == invitationViewModel.UnitId).FirstOrDefaultAsync();

            

            var ownerName = await _context.Owners.Where(o => o.UserName == invitationViewModel.OwnerName.Trim()).FirstOrDefaultAsync();

            if (ownerName == null)
            {
                return new InvitationDto { message = "You Are Not Owner" };
            }

            if (unit.OwnerId != ownerName.Id)
            {
                return new InvitationDto { message = "Its Not Your Unit" };
            }

            Invitaion invitaion = new Invitaion()
            {
                VisitorName = invitationViewModel.VisitorName,
                VisitorIdentifier = invitationViewModel.SSN,
                VisitorPhone = invitationViewModel.VisitorPhone,
                UnitName = unit.Name,
                OwnerId = ownerName.Id,
                StartDate = invitationViewModel.StartDate,
                EndDate = invitationViewModel.EndDate,
                Approve = InvitationCase.Pending
            };

            await _context.Invitaions.AddAsync(invitaion);

            await _context.SaveChangesAsync();

            await _signalHub.Clients.All.SendAsync("LoadInvitations");

            var invitaionDto = new InvitationDto()
            {
                visitorName = invitaion.VisitorName,
                visitorPhone = invitaion.VisitorPhone,
                sSN = invitaion.VisitorIdentifier,
                unitName = invitaion.UnitName,
                ownerEmail = invitaion.Owner.Email,
                ownerPhone =invitaion.Owner.Phone,
                ownerName = invitaion.Owner.UserName,
                startDate = invitaion.StartDate,
                endDate = invitaion.EndDate,              
                id = invitaion.Id,
                isSuccess =true
            };

            return invitaionDto;

        }

        public async Task DeleteInvitationAsync(int id)
        {
            var invitation = await _context.Invitaions.FindAsync(id);

            _context.Invitaions.Remove(invitation);

            await _context.SaveChangesAsync();

            await _signalHub.Clients.All.SendAsync("LoadInvitations");
        }

        public async Task<IEnumerable<InvitationDto>> GetAllInvitationAsync()
        {
            var invitations = await _context.Invitaions.Include(i => i.Owner).Select(i => new InvitationDto { 
                 id = i.Id,
                 visitorName = i.VisitorName,
                 visitorPhone = i.VisitorPhone,
                 sSN = i.VisitorIdentifier,
                 unitName = i.UnitName,
                 ownerName = i.Owner.UserName,
                 ownerEmail = i.Owner.Email,
                 ownerPhone = i.Owner.Phone,
                 startDate = i.StartDate,
                 endDate = i.EndDate,
                 approve = i.Approve,
                 count = _context.Invitaions.Count(),
            }).ToListAsync();

            return invitations;
        }

        public async Task<IEnumerable<InvitationDto>> GetAllInvitationDailyAsync()
        {
            DateTime date = DateTime.Now.Date;

            var invitations = await _context.Invitaions.Include(i=>i.Owner).Where(i=>i.StartDate.Date == date).Select(i=> new InvitationDto { 
            
                 visitorName =i.VisitorName,
                 sSN = i.VisitorIdentifier,
                 ownerName = i.Owner.UserName,
                 ownerPhone =i.Owner.Phone,
                 unitName = i.UnitName,
                 startDate = i.StartDate,
                 endDate = i.EndDate,
                 visitorPhone = i.VisitorPhone,
                 ownerEmail = i.Owner.Email,
                 approve = i.Approve,
                 id =i.Id
            }).ToListAsync();

            return invitations;
        }

        public async Task<InvitationDto> GetInvitationByIdAsync(int id)
        {
            var invitation = await _context.Invitaions.Include(i=>i.Owner).FirstOrDefaultAsync(i=>i.Id == id);

            var invitaionDto = new InvitationDto()
            {
                startDate = invitation.StartDate,
                endDate = invitation.EndDate,
                visitorName = invitation.VisitorName,
                ownerName = invitation.Owner.UserName,
                ownerPhone =invitation.Owner.Phone,
                unitName = invitation.UnitName,
                sSN = invitation.VisitorIdentifier,
                visitorPhone =invitation.VisitorPhone,
                ownerEmail =invitation.Owner.Email,
                id =invitation.Id,
                isSuccess = true
            };

            return invitaionDto;
        }

        public async Task<IEnumerable<InvitationDto>> SearchInvitationAsync(SearchInvitationModel searchInvitationModel)
        {
            if (searchInvitationModel != null)
            {
                var invitations = await _context.Invitaions.Include(i => i.Owner)
                               .Where(i => i.Owner.UserName.StartsWith(searchInvitationModel.SearchText) ||
                               i.UnitName.StartsWith(searchInvitationModel.SearchText) ||
                               i.StartDate.Date == searchInvitationModel.StartDate ||
                               i.EndDate.Date == searchInvitationModel.EndDate
                               )
                               .Select(i => new InvitationDto
                               {
                                   id = i.Id,
                                   startDate = i.StartDate,
                                   endDate = i.EndDate,
                                   visitorName = i.VisitorName,
                                   visitorPhone = i.VisitorPhone,
                                   sSN = i.VisitorIdentifier,
                                   ownerName = i.Owner.UserName,
                                   ownerPhone = i.Owner.Phone,
                                   ownerEmail = i.Owner.Email,
                                   unitName = i.UnitName,
                                   approve = i.Approve
                               })
                               .ToListAsync();

                return invitations;
            }

            else
            {
                var invitations = await _context.Invitaions.Include(i => i.Owner).Select(i => new InvitationDto
                {
                    id = i.Id,
                    startDate = i.StartDate,
                    endDate = i.EndDate,
                    visitorName = i.VisitorName,
                    visitorPhone = i.VisitorPhone,
                    sSN = i.VisitorIdentifier,
                    ownerName = i.Owner.UserName,
                    ownerPhone = i.Owner.Phone,
                    ownerEmail = i.Owner.Email,
                    unitName = i.UnitName,
                    approve = i.Approve
                }).ToListAsync();


                return invitations;
            }
           
        }
    }
}