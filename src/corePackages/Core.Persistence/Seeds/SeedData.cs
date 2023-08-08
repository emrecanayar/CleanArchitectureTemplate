namespace Core.Persistence.Seeds
{
    public static class SeedData
    {
        public static Guid AdminUserId { get; } = Guid.NewGuid();
        public static Guid AdminOperationClaimId { get; } = Guid.NewGuid();
    }
}
