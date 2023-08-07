namespace Core.Persistence.Logs
{
    public class EntityFrameworkQueryLog
    {
        public static void LogQuery(string query)
        {
            string currentPath = $@"{Directory.GetCurrentDirectory()}\Logs\EntityFrameworkQueryLogs\Logs.txt";
            using (StreamWriter writer = new StreamWriter(currentPath, true))
            {
                writer.WriteLine(query);
            }
        }
    }
}