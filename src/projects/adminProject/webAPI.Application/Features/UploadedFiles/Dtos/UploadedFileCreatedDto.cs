namespace webAPI.Application.Features.UploadedFiles.Dtos
{
    public class UploadedFileCreatedDto
    {
        public string Token { get; set; }

        public string Path { get; set; }

        public UploadedFileCreatedDto()
        {
            Token = string.Empty;
            Path = string.Empty;
        }
    }
}
