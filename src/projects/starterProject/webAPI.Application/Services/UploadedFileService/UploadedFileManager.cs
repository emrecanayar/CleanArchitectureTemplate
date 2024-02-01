using Core.Domain.Entities;
using Core.Helpers.Helpers;
using Core.Security.Constants;
using System.Text.Json;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Rules;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Services.UploadedFileService
{
    public class UploadedFileManager : IUploadedFileService
    {
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

        public UploadedFileManager(IUploadedFileRepository uploadedFileRepository, UploadedFileBusinessRules uploadedFileBusinessRules)
        {
            _uploadedFileRepository = uploadedFileRepository;
            _uploadedFileBusinessRules = uploadedFileBusinessRules;
        }

        public async Task<UploadedFile> AddOrUpdateDocument(UploadedFileDto uploadedFileDto)
        {
            string uploadedFileData = JsonSerializer.Serialize(uploadedFileDto);
            string encryptedPath = HashingHelper.AESEncrypt(uploadedFileData, SecurityKeyConstant.DOCUMENT_SECURITY_KEY);
            UploadedFile uploadedFile = new UploadedFile
            {
                Id = uploadedFileDto.Id,
                Token = encryptedPath,
                FileName = uploadedFileDto.FileName,
                Directory = uploadedFileDto.Directory,
                Path = uploadedFileDto.Path,
                Extension = uploadedFileDto.Extension,
                FileType = uploadedFileDto.FileType
            };
            await this._uploadedFileBusinessRules.FileTokenCanNotBeDuplicatedWhenInserted(uploadedFile.Token);

            if (uploadedFile.Id != Guid.Empty)
                await this._uploadedFileRepository.UpdateAsync(uploadedFile);
            else
                await this._uploadedFileRepository.AddAsync(uploadedFile);


            return uploadedFile;
        }

        public async Task<UploadedFileResponseDto?> TransferFile(string token, string newFolderPath, string webRootPath)
        {
            UploadedFile? uploadedFile = await this._uploadedFileRepository.GetAsync(x => x.Token == token);
            if (uploadedFile == null) return null;

            var decryptedProjectFileData = HashingHelper.AESDecrypt(uploadedFile.Token, SecurityKeyConstant.DOCUMENT_SECURITY_KEY);

            UploadedFileDto? uploadedFileDto = JsonSerializer.Deserialize<UploadedFileDto>(decryptedProjectFileData);

            string? newLocationFullPath = DocumentTransferNewLocation(uploadedFileDto.Path, newFolderPath, webRootPath);

            uploadedFileDto.Path = FileHelper.GetURLForFileFromFullPath(webRootPath, newLocationFullPath);

            return new UploadedFileResponseDto { Id = uploadedFile.Id, Path = uploadedFileDto.Path };
        }

        private static string DocumentTransferNewLocation(string fileFullPath, string newFolder, string webRootPath)
        {
            var fileName = Path.GetFileName(fileFullPath);
            var oldLocation = Path.Combine(webRootPath, fileFullPath.Replace("/", "\\"));
            var newLocation = FileHelper.GetNewPath(webRootPath, newFolder, fileName);
            if (File.Exists(newLocation))
            {
                var extension = Path.GetExtension(fileName);
                fileName = Guid.NewGuid().ToString() + extension;
                newLocation = Path.Combine(webRootPath, newFolder, fileName);
            }
            File.Move(oldLocation, newLocation, false);
            return newLocation;
        }
    }
}
