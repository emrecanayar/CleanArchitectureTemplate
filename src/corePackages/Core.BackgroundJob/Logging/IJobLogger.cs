namespace Core.BackgroundJob.Logging
{
    public interface IJobLogger
    {
        void Log(string message);
    }
}
