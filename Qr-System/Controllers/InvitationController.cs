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
using System.Runtime.InteropServices;
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

        [HttpPost("CreateInvitation")]
        public async Task<ActionResult> CreateInvitation([FromBody]InvitationViewModel invitationViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var data = await _invitationService.CreateInvitationAsync(invitationViewModel);

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

        [HttpGet("GetAllInvitation")]
        public async Task<ActionResult> GetAllInvitation()
        {
            try
            {
                var data = await _invitationService.GetAllInvitationAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetAllInvitationDaily")]
        public async Task<ActionResult> GetAllInvitationDaily()
        {
            try
            {
                var data = await _invitationService.GetAllInvitationDailyAsync();

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

        [HttpGet("GetInvitationById/{id}")]
        public async Task<ActionResult> GetInvitationById(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest($"there is no invitation with that id = '{id}'");
                }

                var data = await _invitationService.GetInvitationByIdAsync(id);

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

        [HttpDelete("DeleteInvitation/{id}")]
        public async Task<ActionResult> DeleteInvitation(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest($"there is no invitation with that id = '{id}'");
                }

                await _invitationService.DeleteInvitationAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("ApproveInvitation")]
        public async Task<ActionResult> ApproveInvitation([FromBody] ApproveInvitationViewModel approveInvitationViewModel)
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
        [HttpPost("SearchInvitation")]
        public async Task<ActionResult> SearchInvitation([FromBody] SearchInvitationModel searchInvitationModel)
        {
            try
            {
                var data = await _invitationService.SearchInvitationAsync(searchInvitationModel);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}