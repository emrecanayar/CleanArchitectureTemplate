namespace Core.Helpers.Helpers
{
    public class FileInfoHelper
    {
        public static string GetFileName(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            var fileName = fileInfo.Name.Split(".");
            return fileName[0].ToString();
        }

        public static string GetFileExtension(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Extension?.ToLower();
        }

        public static string GetFileNameAndExtension(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            var fileName = fileInfo.Name.Split(".");
            return fileName[0].ToString() + fileInfo.Extension?.ToLower();
        }
    }
}