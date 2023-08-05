using System.Linq.Expressions;

namespace Core.BackgroundJob.Services
{
    public interface IJobService
    {
        void Enqueue<T>(Expression<Action<T>> methodCall);
        void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
        void ContinueWith<T>(string parentJobId, Expression<Action<T>> methodCall);
        void Recurring<T>(Expression<Action<T>> methodCall, string cronExpression);
    }
}
