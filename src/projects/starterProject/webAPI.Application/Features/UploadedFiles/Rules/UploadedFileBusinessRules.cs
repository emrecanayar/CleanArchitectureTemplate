using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Domain.Entities;
using Core.Helpers.Helpers;
using Core.Persistence.Paging;
using webAPI.Application.Services.Repositories;
using Core.Domain.ComplexTypes.Enums;

namespace webAPI.Application.Features.UploadedFiles.Rules
{
    public class UploadedFileBusinessRules : BaseBusinessRules
    {
        private readonly IUploadedFileRepository _uploadedFileRepository;

        public UploadedFileBusinessRules(IUploadedFileRepository uploadedFileRepository)
        {
            _uploadedFileRepository = uploadedFileRepository;
        }

        public async Task FileTokenCanNotBeDuplicatedWhenInserted(string token)
        {
            IPaginate<UploadedFile> result = await _uploadedFileRepository.GetListAsync(d => d.Token == token);
            if (result.Items.Any()) throw new BusinessException("File token exists");
            await Task.CompletedTask;
        }

        public void FileShouldExistWhenRequested(UploadedFile projectFile)
        {
            if (projectFile == null) throw new BusinessException("Requested file does not exist");
        }

        public FileType DetectFileType(string filePath)
        {
            var extension = FileInfoHelper.GetFileExtension(filePath);
            switch (extension)
            {
                case ".xlsx":
                case ".xls":
                case ".xlsm":
                case ".xlm":
                    return FileType.Xls;
                case ".docx":
                case ".doc":
                    return FileType.Doc;
                case ".potx":
                case ".pot":
                case ".ppsx":
                case ".pps":
                case ".pptx":
                case ".ppt":
                    return FileType.Pps;
                case ".pdf":
                    return FileType.Pdf;
                case ".jpeg":
                case ".png":
                case ".jpg":
                case ".svg":
                    return FileType.Img;
                case ".mp4":
                    return FileType.Mp4;
                default:
                    return default(FileType);
            }
        }
    }
}
