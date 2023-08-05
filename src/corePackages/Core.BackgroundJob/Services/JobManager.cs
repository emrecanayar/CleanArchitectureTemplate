using Hangfire;
using System.Linq.Expressions;

namespace Core.BackgroundJob.Services
{
    public class JobManager : IJobService
    {
        /// <summary>
        /// Belirli bir ana işin tamamlanmasından sonra belirtilen metot çağrısını gerçekleştirir.
        /// Bu, Continuation Jobs (Devam Eden İşler) olarak adlandırılır.
        /// </summary>
        /// <typeparam name="T">Metodun türü</typeparam>
        /// <param name="parentJobId">Ana işin kimliği</param>
        /// <param name="methodCall">Çağrılacak metot ifadesi</param>
        public void ContinueWith<T>(string parentJobId, Expression<Action<T>> methodCall)
        {
            Hangfire.BackgroundJob.ContinueWith(parentJobId, methodCall);
        }

        /// <summary>
        /// Belirtilen metot çağrısını sıraya ekler.
        /// Bu, Fire-and-Forget Jobs (Yak ve Unut İşleri) olarak adlandırılır.
        /// </summary>
        /// <typeparam name="T">Metodun türü</typeparam>
        /// <param name="methodCall">Çağrılacak metot ifadesi</param>
        public void Enqueue<T>(Expression<Action<T>> methodCall)
        {
            Hangfire.BackgroundJob.Enqueue(methodCall);
        }

        /// <summary>
        /// Belirli bir zaman dilimi içinde belirtilen metotu tekrarlayan görevlere ekler veya günceller.
        /// Bu, Recurring Jobs (Yinelenen İşler) olarak adlandırılır.
        /// </summary>
        /// <typeparam name="T">Metodun türü</typeparam>
        /// <param name="methodCall">Çağrılacak metot ifadesi</param>
        /// <param name="cronExpression">Cron ifadesi, tekrarlama zamanlamasını tanımlar</param>
        public void Recurring<T>(Expression<Action<T>> methodCall, string cronExpression)
        {
            RecurringJob.AddOrUpdate(methodCall, cronExpression);
        }

        /// <summary>
        /// Belirtilen bir süre gecikme sonunda belirtilen metodu planlar.
        /// Bu, Delayed Jobs (Gecikmeli İşler) olarak adlandırılır.
        /// </summary>
        /// <typeparam name="T">Metodun türü</typeparam>
        /// <param name="methodCall">Çağrılacak metot ifadesi</param>
        /// <param name="delay">Gecikme süresi</param>
        public void Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            Hangfire.BackgroundJob.Schedule(methodCall, delay);
        }
    }


}
