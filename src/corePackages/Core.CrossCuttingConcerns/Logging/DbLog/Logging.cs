using Core.CrossCuttingConcerns.Logging.DbLog.Dto;

namespace Core.CrossCuttingConcerns.Logging.DbLog
{
    public class Logging
    {
        public async Task CreateLog(ILogService logService, LogDto logDto)
        {
            await logService.CreateLog(logDto);
        }
    }
}
