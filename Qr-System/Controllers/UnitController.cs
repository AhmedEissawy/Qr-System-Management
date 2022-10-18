using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qr_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody]UnitViewModel unitViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = await _unitService.CreateAsync(unitViewModel);

                return Ok(new { name = data.name, phone = data.phone });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            var data = await _unitService.GetAllAsync();

            return Ok(new { units = data.Select(u=>u.name).ToList()});
        }
    }
}




