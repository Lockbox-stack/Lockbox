namespace Lockbox.Api.Domain
{
    public interface IEncrypter
    {
        string GetRandomSecureKey();
        string GetSalt(string value);
        string GetHash(string value, string salt);
        string Encrypt(string value, string salt, string key);
        string Decrypt(string value, string salt, string key);
    }
}