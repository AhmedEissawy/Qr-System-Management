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

                return Ok(data.Select(u=>new {id = u.id,name = u.name,phone = u.phone}));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetUnitById/{id}")]
        public async Task<ActionResult> GetUnitById(int id)
        {
            try
            {
                var data = await _unitService.GetByIdAsync(id);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<ActionResult> Update(int id,[FromBody] UnitViewModel unitViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        return BadRequest($"there is no unit whit that id = '{id}'");
                    }

                    var data = await _unitService.UpdateAsync(id, unitViewModel);

                    if (data.isSuccess)
                    {
                        return Ok(new { name = data.name, phone = data.phone });
                    }

                    return BadRequest(new { message = data.message});
                }
                catch (Exception ex)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }

            }

            return BadRequest(ModelState);
            
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest($"there is no unit with that id = '{id}'");
                }

                await _unitService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}





