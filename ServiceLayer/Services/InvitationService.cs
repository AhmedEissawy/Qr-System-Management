using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer;
using ServiceLayer.DTOs;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;

        public InvitationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ApproveInvitationAsync(ApproveInvitationViewModel approveInvitationViewModel)
        {
            var invitation = await _context.Invitaions.FindAsync(approveInvitationViewModel.Id);

            invitation.Approve = approveInvitationViewModel.approve;

            _context.Invitaions.Update(invitation);

            await _context.SaveChangesAsync();

        }

        public async Task<InvitationDto> CreateAsync(InvitationViewModel invitationViewModel)
        {
            var unit = await _context.Units.FindAsync(invitationViewModel.UnitId);

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
            };

            await _context.Invitaions.AddAsync(invitaion);

            await _context.SaveChangesAsync();

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
                Approve =invitaion.Approve,
                id = invitaion.Id,
                isSuccess =true
            };

            return invitaionDto;

        }

        public async Task DeleteAsync(int id)
        {
            var invitation = await _context.Invitaions.FindAsync(id);

            _context.Invitaions.Remove(invitation);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InvitationDto>> GetAllAsync()
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
                 Approve =i.Approve,
                 visitorPhone =i.VisitorPhone,
                 ownerEmail =i.Owner.Email,
                 count =_context.Invitaions.Where(i => i.StartDate.Date == date).Count(),
                 id =i.Id
            }).ToListAsync();

            return invitations;
        }

        public async Task<InvitationDto> GetByIdAsync(int id)
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
                Approve =invitation.Approve,
                visitorPhone =invitation.VisitorPhone,
                ownerEmail =invitation.Owner.Email,
                id =invitation.Id,
                isSuccess = true
            };

            return invitaionDto;
        }
    }
}
