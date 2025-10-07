namespace Core.Helpers.Helpers
{
    public static class LogHelper
    {
        public static async Task LogToFileAsync(string fileName, string content)
        {
            var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            var filePath = Path.Combine(logPath, fileName);

            using (var writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {content}");
                await writer.WriteLineAsync($"-----------------------------------------------------");
            }
        }
    }
}
