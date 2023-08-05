namespace Core.Application.Pipelines.Security
{
    public interface IDecryptService
    {
        string Decrypt(string encryptedData);
    }
}
