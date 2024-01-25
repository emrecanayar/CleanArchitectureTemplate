using static Core.Domain.ComplexTypes.Enums;

namespace webAPI.Application.Features.UploadedFiles.Dtos
{
    public class UploadedFileResponseDto
    {
        public Guid Id { get; set; }
        public string NameKey { get; set; }
        public string Path { get; set; }
        public FileType FileType { get; set; }

        public UploadedFileResponseDto()
        {
            Id = Guid.NewGuid();
            NameKey = string.Empty;
            Path = string.Empty;
        }
    }
}
