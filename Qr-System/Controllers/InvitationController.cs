using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using ServiceLayer.DTOs;
using ServiceLayer.IServices;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Qr_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> Create([FromBody]InvitationViewModel invitationViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await _invitationService.CreateAsync(invitationViewModel);

                    if (!data.isSuccess)
                    {
                        return BadRequest(new { message =data.message});
                    }

                    return Ok(new
                    {
                        visitorName = data.visitorName,
                        visitorIdentifer = data.sSN,
                        visitorPhone = data.visitorPhone,
                        ownerName = data.ownerName,
                        ownerPhone = data.ownerPhone,
                        ownerEmail = data.ownerEmail,
                        unitName = data.unitName,
                        startDate = data.startDate,
                        endDate = data.endDate,
                        id = data.id
                    });
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
                var data = await _invitationService.GetAllAsync();

                if (data.Any())
                {
                    return Ok(data);
                }

                return Ok("There Is No Invitation Today ");
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest($"there is no invitation with that id = '{id}'");
                }

                var data = await _invitationService.GetByIdAsync(id);

                    return Ok(new
                    {
                        visitorName = data.visitorName,
                        visitorIdentifer = data.sSN,
                        visitorPhone = data.visitorPhone,
                        ownerName = data.ownerName,
                        ownerEmail = data.ownerEmail,
                        ownerPhone = data.ownerPhone,
                        unitName = data.unitName,
                        startDate = data.startDate,
                        endDate = data.endDate,
                        id = data.id
                    });

            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest($"there is no invitation with that id = '{id}'");
                }

                await _invitationService.DeleteAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("Approve")]
        public async Task<ActionResult> Approve(ApproveInvitationViewModel approveInvitationViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _invitationService.ApproveInvitationAsync(approveInvitationViewModel);

                    return Ok();
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
