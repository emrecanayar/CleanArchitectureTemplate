using System.Text;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Core.Helpers.Helpers
{
    public static class FileHelper
    {
        public static void AppendText(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }

        public static void ClearFile(string filePath)
        {
            File.Delete(filePath);
            CreateFile(filePath);
        }

        public static void CreateFile(string filePath)
        {
            try
            {
                if (!IsExistFile(filePath))
                {
                    FileInfo file = new FileInfo(filePath);

                    FileStream fs = file.Create();

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                if (!IsExistFile(filePath))
                {
                    FileInfo file = new FileInfo(filePath);

                    FileStream fs = file.Create();

                    fs.Write(buffer, 0, buffer.Length);

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void Copy(string sourceFilePath, string destFilePath)
        {
            File.Copy(sourceFilePath, destFilePath, true);
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        public static void ExistsFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                FileStream fs = File.Create(filePath);

                fs.Close();
            }
        }

        public static string GetDateFile()
        {
            return DateTime.Now.ToString("HHmmssff");
        }

        public static string GetExtension(string filePath)
        {
            FileInfo fi = new(filePath);

            return fi.Extension;
        }

        public static string GetFileName(string filePath)
        {
            FileInfo fi = new(filePath);

            return fi.Name;
        }

        public static string GetFileNameNoExtension(string filePath)
        {
            FileInfo fi = new(filePath);

            return fi.Name.Split('.')[0];
        }

        public static int GetFileSize(string filePath)
        {
            FileInfo fi = new(filePath);

            return (int)fi.Length;
        }

        public static int GetLineCount(string filePath)
        {
            string[] rows = File.ReadAllLines(filePath);

            return rows.Length;
        }

        public static void Move(string sourceFilePath, string descDirectoryPath)
        {
            string sourceFileName = GetFileName(sourceFilePath);

            if (IsExistDirectory(descDirectoryPath))
            {
                string destFilePath = Path.Combine(descDirectoryPath, sourceFileName);
                if (IsExistFile(destFilePath))
                {
                    DeleteFile(destFilePath);
                }

                File.Move(sourceFilePath, destFilePath);
            }
        }

        public static void WriteText(string filePath, string text, Encoding encoding)
        {
            File.WriteAllText(filePath, text, encoding);
        }

        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static GenerateUrl GenerateURLForFile(IFormFile file, string webRootPath, string folderPath)
        {
            var directoryPath = Path.Combine(webRootPath, folderPath.Replace("/", "\\"));
            CheckDirectoryExists(directoryPath);

            var name = Path.GetFileNameWithoutExtension(file.FileName);
            var type = file.ContentType.StartsWith("image/") ? ".webp" : Path.GetExtension(file.FileName);
            string fullPath;
            string uniqueName;
            int count = 1;

            do
            {
                uniqueName = count == 1 ? name : $"{name}_{count}";
                fullPath = Path.Combine(directoryPath, uniqueName + type);
                count++;
            }
            while (File.Exists(fullPath));
            string path = $"{folderPath.Replace(" ", string.Empty)}/{uniqueName}{type}".Replace("\\", "/");
            return new GenerateUrl
            {
                FileType = type,
                FileName = name,
                Path = path,
                Directory = Path.GetDirectoryName(path),
                Extension = FileInfoHelper.GetFileExtension(path),
            };
        }

        public static string Upload(IFormFile file, string webRootPath, string filePath)
        {
            var isNotValid = CheckFileTypeValid(Path.GetExtension(file.FileName));
            if (isNotValid)
            {
                throw new InvalidOperationException("Type is not valid!");
            }

            CreateFile(Path.Combine(webRootPath, filePath), file);
            return $"{filePath}";
        }

        public static bool Delete(string webRootPath, string path)
        {
            DeleteOldFile(Path.Combine(webRootPath, path).Replace("/", "\\"));
            return true;
        }

        public static string GetNewPath(string webRootPath, string newFolder, string fileName)
        {
            CheckDirectoryExists(Path.Combine(webRootPath, newFolder.Replace("/", "\\")));
            var path = Path.Combine(webRootPath, newFolder.Replace("/", "\\"), fileName);
            return path;
        }

        public static string GetURLForFileFromFullPath(string webRootPath, string fullPath)
        {
            var path = fullPath.Replace(webRootPath, string.Empty);
            return path.Substring(1).Replace("\\", "/");
        }

        private static bool CheckFileTypeValid(string type)
        {
            return type.ToLower() != ".xlsx" &&
                type.ToLower() != ".xls" &&
                type.ToLower() != ".xlsm" &&
                type.ToLower() != ".xlm" &&
                type.ToLower() != ".docx" &&
                type.ToLower() != ".doc" &&
                type.ToLower() != ".potx" &&
                type.ToLower() != ".pot" &&
                type.ToLower() != ".ppsx" &&
                type.ToLower() != ".pps" &&
                type.ToLower() != ".pptx" &&
                type.ToLower() != ".ppt" &&
                type.ToLower() != ".pdf" &&
                type.ToLower() != ".jpeg" &&
                type.ToLower() != ".png" &&
                type.ToLower() != ".jpg" &&
                type.ToLower() != ".svg" &&
                type.ToLower() != ".webp" &&
                type.ToLower() != ".mp4";
        }

        private static void CheckDirectoryExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static void CreateFile(string directory, IFormFile file)
        {
            using (FileStream fs = File.Create(directory.Replace("/", "\\")))
            {
                if (!file.ContentType.StartsWith("image/"))
                {
                    using var image = Image.Load(file.OpenReadStream());

                    var encoder = new WebpEncoder()
                    {
                        Quality = 100,
                    };

                    image.Save(fs, encoder);
                }
                else
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
        }

        private static void DeleteOldFile(string directory)
        {
            if (File.Exists(directory.Replace("/", "\\")))
            {
                File.Delete(directory.Replace("/", "\\"));
            }
        }

        public class GenerateUrl
        {
            public string? FileType { get; set; }

            public string? FileName { get; set; }

            public string? Path { get; set; }

            public string? Extension { get; set; }

            public string? Directory { get; set; }
        }
    }
}