using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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
                try
                {
                    var data = await _unitService.CreateAsync(unitViewModel);

                    if (data.isSuccess)
                    {
                        return Ok(new { name = data.name, phone = data.phone });
                    }

                    return BadRequest(new {message = data.message });
                                    
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var data = await _unitService.GetAllAsync();

                return Ok(new { units = data.Select(u => u.name).ToList() });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}




