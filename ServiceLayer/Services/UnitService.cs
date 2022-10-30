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

            var result = await _context.Units.AnyAsync(u => u.Name.Contains(unitViewModel.Name));

            if (!result)
            {
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
                    phone = unit.Phone,
                    isSuccess =true
                };
            }

            return new UnitDto
            {
                message = "these unit is already exist"
            };
           

        }

        public async Task DeleteAsync(int id)
        {
            var existUnit = await _context.Units.FindAsync(id);

             _context.Units.Remove(existUnit);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UnitDto>> GetAllAsync()
        {
            var units = await _context.Units.Select(u=> new UnitDto {id = u.Id,name =u.Name,phone = u.Phone }).ToListAsync();
            return units;
        }

        public async Task<UnitDto> GetByIdAsync(int id)
        {
            var unit = await _context.Units.Where(u => u.Id == id).FirstOrDefaultAsync();

            var unitDto = new UnitDto()
            {
                id = unit.Id,
                name = unit.Name,
                phone = unit.Phone,
            };

            return unitDto;

        }

        public async Task<UnitDto> UpdateAsync(int id, UnitViewModel unitViewModel)
        {
            var existUnit = await _context.Units.FindAsync(id);

            if (existUnit == null)
                return new UnitDto { message = $"there is no unit with that id = '{id}'" };

            if (string.IsNullOrEmpty(unitViewModel.Name))
                return new UnitDto { message = "Enter Unit Name " };

            if (string.IsNullOrEmpty(unitViewModel.Phone))
                return new UnitDto { message = "Enter Unit Phone " };

            existUnit.Name = unitViewModel.Name;

            existUnit.Phone = unitViewModel.Phone;

            _context.Units.Update(existUnit);

            await _context.SaveChangesAsync();

                return new UnitDto
                {
                    name = existUnit.Name,
                    phone = existUnit.Phone,
                    isSuccess =true
                };
            
        }
    }
}

