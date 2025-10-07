using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class UploadedFile : Entity<Guid>
    {
        public string Token { get; set; }

        public string FileName { get; set; }

        public string? Directory { get; set; } = null;

        public string Path { get; set; }

        public string Extension { get; set; }

        public FileType? FileType { get; set; } = null;

        public UploadedFile()
        {
            Token = string.Empty;
            FileName = string.Empty;
            Path = string.Empty;
            Extension = string.Empty;
        }

        public UploadedFile(Guid id, string token, string fileName, string? directory, string path, string extension, FileType? fileType)
            : this()
        {
            Id = id;
            Token = token;
            FileName = fileName;
            Directory = directory;
            Path = path;
            Extension = extension;
            FileType = fileType;
        }
    }
}
