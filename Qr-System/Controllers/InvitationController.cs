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

                    if (data == null)
                    {
                        return BadRequest("Something Went Wrong");
                    }

                    return Ok(await GenerateQr(data));

                    ////return Ok(new {
                    ////    visitorName =data.visitorName ,
                    ////    visitorIdentifer = data.sSN,
                    ////    ownerName = data.ownerName,
                    ////    unitName = data.unitName,
                    ////    startDate = data.startDate,
                    ////    endDate = data.endDate
                    //});
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

        private byte[] ImageToByteArray(Image imageIn)
        {
            using MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }

        [HttpGet("GenerateQr")]
        public async Task<ActionResult> GenerateQr(InvitationDto invitation)
        {
            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(invitation.ToString(), QRCodeGenerator.ECCLevel.Q);
            QRCode qRCode = new QRCode(qRCodeData);
            Image qrCodeImage = qRCode.GetGraphic(20);

            var bytes = await Task.FromResult( ImageToByteArray(qrCodeImage));

            return File(bytes, "Image/bmp");
        }
    }
}
