using Core.Domain.Entities;
using webAPI.Application.Features.UploadedFiles.Dtos;

namespace webAPI.Application.Services.UploadedFileService
{
    public interface IUploadedFileService
    {
        Task<UploadedFile> AddOrUpdateDocument(UploadedFileDto uploadedFileDto);
        Task<UploadedFileResponseDto> TransferFile(string token, string newFolderPath);
    }
}