using ServiceLayer.DTOs;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.IServices
{
    public interface IUnitService
    {
        Task<UnitDto> CreateAsync(UnitViewModel unitViewModel);

        Task<IEnumerable<UnitDto>> GetAllAsync();
    }
}

