using Core.Application.ResponseTypes.Concrete;
using Microsoft.AspNetCore.Mvc;
using webAPI.Application.Features.UploadedFiles.Commands.UploadFile;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Queries.GetUploadedFileByToken;
using webAPI.Controllers.Base;

namespace webAPI.Controllers
{
    public class UploadedFilesController : BaseController
    {
        private readonly IConfiguration _configuration;
        public UploadedFilesController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile file)
        {
            CustomResponseDto<UploadedFileCreatedDto> result = await Mediator.Send(new UploadFileCommand { File = file, WebRootPath = _configuration.GetSection("WebRootPath").Value });
            return Created("", result);
        }

        [HttpPost("GetFile")]
        public async Task<IActionResult> GetFile(GetUploadedFileByTokenQuery getUploadedFileByTokenQuery)
        {
            CustomResponseDto<UploadedFileDto> result = await Mediator.Send(getUploadedFileByTokenQuery);
            return Ok(result.Data);
        }
    }
}
