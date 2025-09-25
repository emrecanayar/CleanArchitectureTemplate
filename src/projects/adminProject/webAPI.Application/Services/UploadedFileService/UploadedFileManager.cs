using System.Text.Json;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using Core.Security.Constants;
using Microsoft.AspNetCore.Hosting;
using webAPI.Application.Features.UploadedFiles.Dtos;
using webAPI.Application.Features.UploadedFiles.Rules;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Services.UploadedFileService
{
    public class UploadedFileManager : IUploadedFileService
    {
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly UploadedFileBusinessRules _uploadedFileBusinessRules;

        public UploadedFileManager(IUploadedFileRepository uploadedFileRepository, IWebHostEnvironment environment, UploadedFileBusinessRules uploadedFileBusinessRules)
        {
            _uploadedFileRepository = uploadedFileRepository;
            _environment = environment;
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
            //await this._uploadedFileBusinessRules.FileTokenCanNotBeDuplicatedWhenInserted(uploadedFile.Token);

            if (uploadedFile.Id != Guid.Empty)
                await this._uploadedFileRepository.UpdateAsync(uploadedFile);
            else
                await this._uploadedFileRepository.AddAsync(uploadedFile);

            return uploadedFile;
        }

        public async Task<UploadedFileResponseDto> TransferFile(string token, string newFolderPath)
        {
            string webRootPath = _environment.WebRootPath;// _configuration.GetSection("WebRootPath").Value;
            UploadedFile? uploadedFile = await this._uploadedFileRepository.GetAsync(x => x.Token == token, enableTracking: false);
            if (uploadedFile == null) return null;

            var decryptedProjectFileData = HashingHelper.AESDecrypt(uploadedFile.Token, SecurityKeyConstant.DOCUMENT_SECURITY_KEY);

            UploadedFileDto? uploadedFileDto = JsonSerializer.Deserialize<UploadedFileDto>(decryptedProjectFileData);

            string? newLocationFullPath = DocumentTransferNewLocation(uploadedFileDto.Path, newFolderPath);

            uploadedFileDto.Path = FileHelper.GetURLForFileFromFullPath(webRootPath, newLocationFullPath);
            uploadedFileDto.Directory = Path.GetDirectoryName(uploadedFileDto.Path);
            uploadedFile.Path = uploadedFileDto.Path;
            uploadedFile.Directory = uploadedFileDto.Directory;
            await this._uploadedFileRepository.UpdateAsync(uploadedFile);
            return new UploadedFileResponseDto
            {
                Id = uploadedFile.Id,
                Path = uploadedFileDto.Path,
                FileName = Path.GetFileName(uploadedFileDto.Path),
                Directory = uploadedFileDto.Directory,
                Extension = uploadedFileDto.Extension,
                FileType = uploadedFileDto.FileType,

            };
        }

        private string DocumentTransferNewLocation(string fileFullPath, string newFolder)
        {
            string webRootPath = _environment.WebRootPath;
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