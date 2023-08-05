namespace Core.BackgroundJob.Services
{
    public interface IRetryPolicyService
    {
        void ApplyPolicy();
    }
}
