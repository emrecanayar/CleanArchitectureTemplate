using Core.Application.ResponseTypes.Concrete;
using Core.Helpers.Helpers;
using MediatR;
using System.Net;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Rules;
using webAPI.Application.Services.UploadedFileService;

namespace webAPI.Application.Features.UploadedFiles.Commands.UploadFileTransfer
{
    public class UploadFileTransferCommand : IRequest<CustomResponseDto<UploadedFileTransferDto>>
    {
        public string Token { get; set; }
        public string NewFolderPath { get; set; }
        public string WebRootPath { get; set; }

        public UploadFileTransferCommand()
        {
            Token = string.Empty;
            NewFolderPath = string.Empty;
            WebRootPath = string.Empty;

        }

        public class UploadFileTransferCommandHandler : IRequestHandler<UploadFileTransferCommand, CustomResponseDto<UploadedFileTransferDto>>
        {
            private readonly IUploadedFileService _uploadedFileService;
            private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

            public UploadFileTransferCommandHandler(IUploadedFileService uploadedFileService, UploadedFileBusinessRules uploadedFileBusinessRules)
            {
                _uploadedFileService = uploadedFileService;
                _uploadedFileBusinessRules = uploadedFileBusinessRules;
            }

            public async Task<CustomResponseDto<UploadedFileTransferDto>> Handle(UploadFileTransferCommand request, CancellationToken cancellationToken)
            {
                UploadedFileResponseDto result = await this._uploadedFileService.TransferFile(request.Token, request.NewFolderPath, request.WebRootPath);

                var addedOrUpdatedResult = await this._uploadedFileService.AddOrUpdateDocument(new UploadedFileDto
                {
                    FileType = this._uploadedFileBusinessRules.DetectFileType(result.Path),
                    FileName = "",
                    Path = result.Path,
                    Extension = FileInfoHelper.GetFileExtension(result.Path),
                });

                return CustomResponseDto<UploadedFileTransferDto>.Success((int)HttpStatusCode.OK, new UploadedFileTransferDto { NewFolderPath = result.Path, Token = addedOrUpdatedResult.Token }, true);
            }
        }
    }
}
