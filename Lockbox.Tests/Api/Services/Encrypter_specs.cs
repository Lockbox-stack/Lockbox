using FluentAssertions;
using Lockbox.Api.Domain;
using Lockbox.Api.Services;
using Machine.Specifications;

namespace Lockbox.Tests.Api.Services
{
    public abstract class Encrypter_specs
    {
        protected static IEncrypter Encrypter;
        protected static string Value;
        protected static string Hash;
        protected static string Salt;
        protected static string EncryptionKey;

        protected static void Initialize()
        {
            Value = "secret";
            Encrypter = new Encrypter();
        }
    }

    [Subject("Encrypter")]
    public class when_invoking_get_salt : Encrypter_specs
    {
        Establish context = () => Initialize();

        Because of = () => Salt = Encrypter.GetSalt(Value);

        It should_create_non_empty_salt = () => Salt.ShouldNotBeEmpty();
    }

    [Subject("Encrypter")]
    public class when_invoking_get_random_secure_key : Encrypter_specs
    {
        Establish context = () => Initialize();

        Because of = () => EncryptionKey = Encrypter.GetRandomSecureKey();

        It should_create_non_empty_key = () => EncryptionKey.ShouldNotBeEmpty();
    }

    [Subject("Encrypter")]
    public class when_invoking_get_hash : Encrypter_specs
    {
        Establish context = () => Initialize();

        Because of = () =>
        {
            Salt = Encrypter.GetSalt(Value);
            Hash = Encrypter.GetHash(Value, Salt);
        };

        It should_create_non_empty_hash = () => Hash.ShouldNotBeEmpty();
    }

    [Subject("Encrypter")]
    public class when_encrypting : Encrypter_specs
    {
        static string EncryptedValue;

        Establish context = () => Initialize();

        Because of = () =>
        {
            EncryptionKey = Encrypter.GetRandomSecureKey();
            Salt = Encrypter.GetSalt(Value);
            Hash = Encrypter.GetHash(Value, Salt);
            EncryptedValue = Encrypter.Encrypt(Value, Salt, EncryptionKey);
        };

        It should_encrypt_value = () =>
        {
            EncryptedValue.ShouldNotBeEmpty();
            EncryptedValue.ShouldNotEqual(Value);
        };
    }

    [Subject("Encrypter")]
    public class when_decrypting : Encrypter_specs
    {
        static string EncryptedValue;
        static string DecryptedValue;

        Establish context = () => Initialize();

        Because of = () =>
        {
            EncryptionKey = Encrypter.GetRandomSecureKey();
            Salt = Encrypter.GetSalt(Value);
            Hash = Encrypter.GetHash(Value, Salt);
            EncryptedValue = Encrypter.Encrypt(Value, Salt, EncryptionKey);
            DecryptedValue = Encrypter.Decrypt(EncryptedValue, Salt, EncryptionKey);
        };

        It should_decrypt_value = () =>
        {
            DecryptedValue.ShouldBeEquivalentTo(Value);
        };
    }
}