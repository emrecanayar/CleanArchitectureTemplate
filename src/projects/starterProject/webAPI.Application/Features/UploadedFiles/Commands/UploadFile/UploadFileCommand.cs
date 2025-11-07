using System.Net;
using Core.Application.ResponseTypes.Concrete;
using Core.Helpers.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Rules;
using webAPI.Application.Services.UploadedFileService;

namespace webAPI.Application.Features.UploadedFiles.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<CustomResponseDto<UploadedFileCreatedDto>>
    {
        public IFormFile File { get; set; }

        public string? WebRootPath { get; set; }

        public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, CustomResponseDto<UploadedFileCreatedDto>>
        {
            private readonly string _uploadedFileFolder = Path.Combine("Resources", "UploadedFiles", "DocumentPool");
            private readonly IUploadedFileService _uploadedFileService;
            private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

            public UploadFileCommandHandler(IUploadedFileService uploadedFileService, UploadedFileBusinessRules uploadedFileBusinessRules)
            {
                _uploadedFileService = uploadedFileService;
                _uploadedFileBusinessRules = uploadedFileBusinessRules;
            }

            public async Task<CustomResponseDto<UploadedFileCreatedDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
            {
                var fileData = FileHelper.GenerateURLForFileFast(request.File, request.WebRootPath!, _uploadedFileFolder);

                var uploadedFileDto = new UploadedFileDto
                {
                    FileType = _uploadedFileBusinessRules.DetectFileType(fileData.Path!),
                    FileName = fileData.FileName!,
                    Path = fileData.Path!,
                    Extension = fileData.Extension!,
                    Directory = Path.GetDirectoryName(fileData.Path),
                };

                var uploadedFile = await _uploadedFileService.AddOrUpdateDocument(uploadedFileDto);

                await FileHelper.UploadAsync(request.File, request.WebRootPath!, fileData.Path!);

                return CustomResponseDto<UploadedFileCreatedDto>.Success((int)HttpStatusCode.Created, new UploadedFileCreatedDto { Path = fileData.Path!, Token = uploadedFile.Token }, true);
            }
        }
    }
}
