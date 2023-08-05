using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using Microsoft.AspNetCore.Http;
using System.Text;

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

                File.Delete(file);
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
            FileInfo fi = new FileInfo(filePath);

            return fi.Extension;
        }

        public static string GetFileName(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            return fi.Name;
        }

        public static string GetFileNameNoExtension(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            return fi.Name.Split('.')[0];
        }

        public static int GetFileSize(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

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
                if (IsExistFile(descDirectoryPath + "\\" + sourceFileName))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceFileName);
                }

                File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFileName);
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

        public static GenerateURL GenerateURLForFile(IFormFile file, string webRootPath, string folderPath)
        {
            var name = file.FileName.Split('.')[0].Replace(" ", string.Empty);
            var type = file.ContentType.ToLower().Contains("image") ? ".webp" : Path.GetExtension(file.FileName);
            CheckDirectoryExists(Path.Combine(webRootPath, folderPath.Replace("/", "\\")));

            return new GenerateURL
            {
                FileType = type,
                FileName = name,
                Path = $"{folderPath.Replace(" ", string.Empty)}/{name}{type}".Replace("\\", "/"),
                Extension = FileInfoHelper.GetFileExtension($"{folderPath.Replace(" ", string.Empty)}/{name}{type}".Replace("\\", "/")),
            };
        }
        public static string Upload(IFormFile file, string webRootPath, string filePath)
        {
            var isNotValid = CheckFileTypeValid(Path.GetExtension(file.FileName));
            if (isNotValid)
                throw new Exception("Type is not valid!");
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
            var path = fullPath.Replace(webRootPath, "");
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
                if (file.ContentType.ToLower().Contains("image"))

                {
                    using (ImageFactory imageFactory = new ImageFactory())
                    {
                        imageFactory.Load(file.OpenReadStream())
                                    .Format(new WebPFormat())
                                    .Save(fs);
                    }
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


        public class GenerateURL
        {
            public string FileType { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }
            public string Extension { get; set; }
            public string Directory { get; set; }
        }
    }
}