namespace Core.BackgroundJob.Logging
{
    public class JobLogger : IJobLogger
    {
        public void Log(string message)
        {
            // Örneğin, burada konsola loglama yaptık.
            // Farklı bir loglama aracı (ör. Serilog, NLog vs.) kullanarak daha gelişmiş loglama yapabilirsiniz.
            Console.WriteLine($"Job Log: {message}");
        }
    }
}
