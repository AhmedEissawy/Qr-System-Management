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
                        ownerName = data.ownerName,
                        unitName = data.unitName,
                        startDate = data.startDate,
                        endDate = data.endDate,
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

                return Ok(data);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        private byte[] BitMapToByteArray(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        [HttpGet("GenerateQr")]
        public async Task<ActionResult> GenerateQr(InvitationDto invitation)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(invitation.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            Bitmap qrCodeImage = qRCode.GetGraphic(20);

            var bytes = await Task.FromResult(BitMapToByteArray(qrCodeImage));

            return File(bytes,"image/jpeg");
        }
    }
}
