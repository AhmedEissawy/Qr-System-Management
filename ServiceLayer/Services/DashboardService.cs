using RepositoryLayer;
using ServiceLayer.DTOs;
using ServiceLayer.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public DashboardCount GetAllTablesCountAsync()
        {
            var userCount =  _context.Users.Where(u => u.Type == "user").Count();

            var ownerCount = _context.Owners.Count();

            var invitationCount = _context.Invitaions.Count();

            var unitCount = _context.Units.Count();

            var dashboardCount = new DashboardCount()
            {
                userCount = userCount,
                ownerCount = ownerCount,
                invitationCount = invitationCount,
                unitCount = unitCount
            };

            return dashboardCount;
        }
    }
}
