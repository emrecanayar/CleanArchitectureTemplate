namespace webAPI.Application.Features.UploadedFiles.Dtos
{
    public class UploadedFileTransferDto
    {
        public string Token { get; set; }
        public string NewFolderPath { get; set; }

        public UploadedFileTransferDto()
        {
            Token = string.Empty;
            NewFolderPath = string.Empty;
        }
    }
}
