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

        public async Task ApproveInvitationAsync(int id)
        {
            var invitation = await _context.Invitaions.FindAsync(id);

            invitation.Approve = true;

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
                UnitName = unit.Name,
                OwnerId = ownerName.Id,
                StartDate = invitationViewModel.StartDate,
                EndDate = invitationViewModel.EndDate
            };

            await _context.Invitaions.AddAsync(invitaion);

            await _context.SaveChangesAsync();

            var invitaionDto = new InvitationDto()
            {
                visitorName = invitaion.VisitorName,
                sSN = invitaion.VisitorIdentifier,
                unitName = invitaion.UnitName,
                ownerName = invitaion.Owner.UserName,
                startDate = invitaion.StartDate,
                endDate = invitaion.EndDate,
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
            var date = DateTime.Today.ToString("MM/dd/yyyy");

            var invitations = await _context.Invitaions.Include(i=>i.Owner).Select(i=> new InvitationDto { 
            
                 visitorName =i.VisitorName,
                 sSN = i.VisitorIdentifier,
                 ownerName = i.Owner.UserName,
                 unitName = i.UnitName,
                 startDate = i.StartDate,
                 endDate = i.EndDate,

            }).ToListAsync();

            return invitations;
        }

    }
}
