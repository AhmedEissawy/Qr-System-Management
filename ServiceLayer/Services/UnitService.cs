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
    public class UnitService : IUnitService
    {
        private readonly ApplicationDbContext _context;

        public UnitService(ApplicationDbContext  context)
        {
            _context = context;
        }
        public async Task<UnitDto> CreateAsync(UnitViewModel unitViewModel)
        {
            if (string.IsNullOrEmpty(unitViewModel.Name))
                return new UnitDto { message = "Enter Unit Name " };

            if (string.IsNullOrEmpty(unitViewModel.Phone))
                return new UnitDto { message = "Enter Unit Phone " };

            Unit unit = new Unit()
            {
                Name = unitViewModel.Name,
                Phone = unitViewModel.Phone
            };

            await _context.Units.AddAsync(unit);

            await _context.SaveChangesAsync();

            return new UnitDto
            {
                name = unit.Name,
                phone = unit.Phone
            };

        }

        public async Task<IEnumerable<UnitDto>> GetAllAsync()
        {
            var units = await _context.Units.Select(u=> new UnitDto {name =u.Name }).ToListAsync();
            return units;
        }
    }
}
