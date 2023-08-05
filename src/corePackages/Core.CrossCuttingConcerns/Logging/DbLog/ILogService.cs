using Core.CrossCuttingConcerns.Logging.DbLog.Dto;

namespace Core.CrossCuttingConcerns.Logging.DbLog
{
    public interface ILogService
    {
        Task CreateLog(LogDto logDto);
    }
}
