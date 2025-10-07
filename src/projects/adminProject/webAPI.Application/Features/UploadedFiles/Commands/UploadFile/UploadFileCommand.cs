using System.Net;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities;
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
            private readonly string _uPLOADEDFILEFOLDER = Path.Combine("Resources", "UploadedFiles", "DocumentPool");
            private readonly IUploadedFileService _uploadedFileService;
            private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

            public UploadFileCommandHandler(IUploadedFileService uploadedFileService, UploadedFileBusinessRules uploadedFileBusinessRules)
            {
                _uploadedFileService = uploadedFileService;
                _uploadedFileBusinessRules = uploadedFileBusinessRules;
            }

            public async Task<CustomResponseDto<UploadedFileCreatedDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
            {
                FileHelper.GenerateUrl file = FileHelper.GenerateURLForFile(request.File, request.WebRootPath!, _uPLOADEDFILEFOLDER);

                var uploadedFileDto = new UploadedFileDto
                {
                    FileType = this._uploadedFileBusinessRules.DetectFileType(file.Path!),
                    FileName = string.Concat(file.FileName, file.Extension),
                    Path = file.Path,
                    Extension = file.Extension,
                    Directory = Path.GetDirectoryName(file.Path),
                };

                UploadedFile uploadedFile = await this._uploadedFileService.AddOrUpdateDocument(uploadedFileDto);

                FileHelper.Upload(request.File, request.WebRootPath!, file.Path!);
                return CustomResponseDto<UploadedFileCreatedDto>.Success((int)HttpStatusCode.Created, new UploadedFileCreatedDto { Path = file.Path, Token = uploadedFile.Token }, true);
            }
        }
    }
}
